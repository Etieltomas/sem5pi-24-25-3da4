using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.Shared;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Domain.StaffEntity;
using Sempi5.Domain.TokenEntity;
using Sempi5.Domain.UserEntity;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Infrastructure.PatientRepository;
using Sempi5.Infrastructure.Shared;
using Sempi5.Infrastructure.SpecializationRepository;
using Sempi5.Infrastructure.StaffRepository;
using Sempi5.Infrastructure.TokenRepository;
using Sempi5.Infrastructure.UserRepository;
using Serilog;
using Serilog.Events;
using Sempi5.Infrastructure;
using Sempi5.Domain.RoomEntity;
using Sempi5.Infrastructure.RoomRepository;
using Sempi5.Domain.AllergyEntity;
using Sempi5.Domain.MedicalRecordEntity;
using Sempi5.Domain.AppointmentEntity;
using Sempi5.Infrastructure.AppointmentRepository;
using Sempi5.Domain.RoomTypeEntity;
using Sempi5.Infrastructure.RoomTypeRepository;
using Sempi5.Domain.MedicalConditionEntity;

namespace Sempi5
{
    public class Program
    {
        public static void Main(string[] args)
        {  
            var builder = WebApplication.CreateBuilder(args);
        
            CreateLogginsMechanism(builder);
            
            ConfigureIAM(builder);

            builder.Services.AddControllersWithViews();
            
            CreateDataBase(builder);            
            ConfigureMyServices(builder);
            
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            ConfigureMiddleware(app);

            // TODO: Check if I can do this?
            Bootstrap(app);

            app.Run();
        }

        private static void CreateLogginsMechanism(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e =>
                        e.Properties.ContainsKey("CustomLogLevel") && e.Properties["CustomLogLevel"].ToString() == "\"CustomLevel\"")
                    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day))
                .CreateLogger();

            builder.Host.UseSerilog();
        }

        private static void ConfigureIAM(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options => 
            {
                options.AddPolicy("AllowSpecificOrigin",
                    policy =>
                    {
                        policy
                            .WithOrigins(
                                builder.Configuration["IpAddresses:FrontEnd"], 
                                builder.Configuration["IpAddresses:3D"]
                            ) // Make sure to use correct protocol (https/http)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            })
            .AddAuthorization(options =>
            {
                options.AddPolicy("Staff", policy => 
                    policy.RequireRole("Doctor", "Nurse", "Admin", "Other"));
                options.AddPolicy("Patient", policy => 
                    policy.RequireRole("Patient"));
            })
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/api/Login/login";
                options.ExpireTimeSpan = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = false;
                options.Cookie.Domain = builder.Configuration["IpAddresses:FrontEnd"].Contains("localhost") ? null : ".sarm.com";
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
            })
            .AddGoogle(options => 
            {
                options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
                options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
                options.CallbackPath = "/signin-google";
                options.SaveTokens = true;

                options.Events.OnCreatingTicket = async context =>
                {
                    await HandleGoogleLoginAsync(context);
                };
            });
        }

        private static async Task HandleGoogleLoginAsync(OAuthCreatingTicketContext context)
        {
            var notRegistered = false;
            var cookie = (ClaimsIdentity)context.Principal.Identity;
            var services = context.HttpContext.RequestServices;

            try
            {
                var userRepository = services.GetRequiredService<IUserRepository>();
                var staffRepository = services.GetRequiredService<IStaffRepository>();
                var patientRepository = services.GetRequiredService<IPatientRepository>();
                var unitOfWork = services.GetRequiredService<IUnitOfWork>();

                var email = cookie?.FindFirst(ClaimTypes.Email)?.Value;
                var user = await userRepository.GetUserByEmail(email);

                if (user != null && user.Email.Equals(new Email(email)))
                {
                    // User already exists
                    if (user.Active == true) {
                        cookie.AddClaim(new Claim(ClaimTypes.Role, user.Role));                                               
                    }
                } 
                else
                {
                    var staff = await staffRepository.GetStaffMemberByEmail(email);
                    var patient = await patientRepository.GetPatientByEmail(email);
                    SystemUser? newUser = null;

                    if (staff != null)
                    {
                        var username = email.Split("@").First();

                        string userRole;
                        switch (username[0].ToString().ToUpper())
                        {
                            case "D":
                                userRole = "Doctor";
                                break;
                            case "N":
                                userRole = "Nurse";
                                break;
                            default:
                                userRole = "Other";
                                break;
                        }
                        newUser = new SystemUser { Username = username, Email = new Email(email), Role = userRole, Active = false };
                        staff.SystemUser = newUser;
                    } 
                    else if (patient != null)
                    {
                        newUser = new SystemUser { Username = email, Email = new Email(email), Role = "Patient", Active = false };
                        patient.SystemUser = newUser;
                    }

                    if (newUser == null)
                    {
                        notRegistered = true;
                    }
                    else
                    {
                        var token = new Token {
                            Email = newUser.Email,
                            ExpirationDate = DateTime.UtcNow.AddHours(24),
                            IsUsed = false
                        };
                        var tokenRepo = services.GetRequiredService<ITokenRepository>();

                        var _emailService = services.GetRequiredService<EmailService>();

                        await tokenRepo.AddAsync(token);
                        await userRepository.AddAsync(newUser);
                        await unitOfWork.CommitAsync();    

                        var tokenValue = await tokenRepo.GetTokenByEmail(newUser.Email.ToString());

                        var _configuration = services.GetRequiredService<IConfiguration>();

                        var message = "<b>Hello,</b><br>" +
                        "Thank you for signing up! Please confirm your account by clicking the link below:<br><br>" +
                        "<a href='"+_configuration["IpAddresses:This"]+"/api/SystemUser/confirm/" + tokenValue.Id.AsString() + "/true'>Click here to confirm your account</a><br><br>" +
                        "If you didn't sign up, please ignore this email.";

                        var subject = "Confirmation of Account";
                        _emailService.sendEmail(newUser.Username, newUser.Email.ToString(), subject, message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            if (notRegistered) 
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
 
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };
            await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(cookie), authProperties);
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            app.UseMiddleware<CatchRedirectMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("AllowSpecificOrigin");

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
        }

        public static void CreateDataBase(WebApplicationBuilder builder)
        {
            string name = "Sempi5.Infrastructure.Databases." + builder.Configuration["DataBase:Type"];
            Type? dbType = Type.GetType(name);
            
            if (dbType == null)
            {
                // Exit application
                Console.WriteLine("Database Type Invalid. Please check the configuration file!\nApplication will exit");
                Environment.Exit(2);
            }   

            try
            { 
                ((IDatabase)Activator.CreateInstance(dbType)).connectDB(builder);                           
            }
            catch (Exception)
            {
                // Exit application
                Console.WriteLine("Database not found\nApplication will exit");
                Environment.Exit(3);
            }
        }

        private static async void Bootstrap(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                await SeedSpecializationsAsync(services);
                await SeedRoomTypes(services);
                await SeedUsersAsync(services);
                await SeedOperationTypeAsync(services);
                await SeedRooms(services);
                await SeedPlanning(services);
                await SeedAllergiesAsync(services);
                await SeedConditionsAsync(services);
                await SeedPatientAsync(services);
                await SeedStaffAsync(services);
                await SeedOperationRequestsAsync(services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
            }
        }

        private static async Task SeedRoomTypes(IServiceProvider services)
        {
            var roomTypeRep = services.GetRequiredService<IRoomTypeRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();
            if ((await roomTypeRep.GetAllAsync()).Count > 0)
            {
                return;
            }
            var roomType1 = new RoomType ("Operating Room");
            var roomType2 = new RoomType ("Intensive Care Unit");
            var roomType3 = new RoomType ("Consultation Room");
            await roomTypeRep.AddAsync(roomType1);
            await roomTypeRep.AddAsync(roomType2);
            await roomTypeRep.AddAsync(roomType3);
            await unitOfWork.CommitAsync();
        }

        private static async Task SeedPlanning(IServiceProvider services)
        {
            var roomRep = services.GetRequiredService<IRoomRepository>();
            var staffRep = services.GetRequiredService<IStaffRepository>();
            var request = services.GetRequiredService<IOperationRequestRepository>();
            var specRepo = services.GetRequiredService<ISpecializationRepository>();
            var patientRep = services.GetRequiredService<IPatientRepository>();
            var operationTypeRep = services.GetRequiredService<IOperationTypeRepository>();
            var SystemUserRep = services.GetRequiredService<IUserRepository>();
            var roomTypeRep = services.GetRequiredService<IRoomTypeRepository>();

            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            if (
                ((await roomRep.GetAllAsync()).Count() > 0) && 
                ((await request.GetAllAsync()).Count() > 0) &&
                ((await staffRep.GetAllAsync()).Count() > 0) &&
                ((await operationTypeRep.GetAllAsync()).Count() > 0)
            )
            {
                return;
            }

            var room9 = new Room {
                Capacity = new Capacity(2),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Bisturi", "Scalpels" }),
                RoomStatus = RoomStatus.Available,
                Slots = new List<Slot>(),
                Type = await roomTypeRep.GetByNameAsync("Operating Room")
            };
            await roomRep.AddAsync(room9);
            await unitOfWork.CommitAsync();

            var specialization = new Specialization{Code = "0000", Name = "Orthopaedist"};
            var specialization1 = new Specialization{Code = "0001", Name = "Anaesthetist"};
            await specRepo.AddAsync(specialization);
            await specRepo.AddAsync(specialization1);
            await unitOfWork.CommitAsync();

            var user1 = new SystemUser{
                Username = "laura",
                Email = new Email("lauramurias@gmail.com"),
                Role = "Doctor",
                Active = true
            };
            var user2 = new SystemUser{
                Username = "sara",
                Email = new Email("saramacedo@gmail.com"),
                Role = "Doctor",
                Active = true
            };
            var user3 = new SystemUser{
                Username = "sofia",
                Email = new Email("sofiacardoso@gmail.com"),
                Role = "Assistant",
                Active = true
            };
            await SystemUserRep.AddAsync(user1);
            await SystemUserRep.AddAsync(user2);
            await SystemUserRep.AddAsync(user3);
            await unitOfWork.CommitAsync();

            var staff1 = new Staff {
                Name = new Name("Laura Múrias"),
                Email = new Email("left4deadgame2@gmail.com"),
                Specialization = await specRepo.GetByName("Orthopaedist"),
                LicenseNumber = new LicenseNumber("123456"),
                Phone = new Phone("912345678"),
                Address = new Address("Rua do ISEP", "Porto", "Portugal"),
                AvailabilitySlots = new List<AvailabilitySlot> { 
                    new AvailabilitySlot("10-10-2025T08:00:00 - 10-10-2025T20:00:00")
                },
                SystemUser = user1
            };
            var staff2 = new Staff {
                Name = new Name("Sara Macedo"),
                Email = new Email("saramacedo@gmail.com"),
                Specialization = await specRepo.GetByName("Anaesthetist"),
                LicenseNumber = new LicenseNumber("654321"),
                Phone = new Phone("987654321"),
                Address = new Address("Rua do ISEP", "Porto", "Portugal"),
                AvailabilitySlots = new List<AvailabilitySlot> { 
                    new AvailabilitySlot("10-10-2025T08:00:00 - 10-10-2025T20:00:00")
                },
                SystemUser = user2
            };
            var staff3 = new Staff {
                Name = new Name("Sofia Cardoso"),
                Email = new Email("sofiacardoso@gmail.com"),
                Specialization = await specRepo.GetByName("Neurology"),
                LicenseNumber = new LicenseNumber("154321"),
                Phone = new Phone("187654321"),
                Address = new Address("Rua do ISEP", "Porto", "Portugal"),
                AvailabilitySlots = new List<AvailabilitySlot> { 
                    new AvailabilitySlot("10-10-2025T08:00:00 - 10-10-2025T20:00:00")
                },
                SystemUser = user3
            };
            await staffRep.AddAsync(staff1);
            await unitOfWork.CommitAsync();
            await staffRep.AddAsync(staff2);
            await unitOfWork.CommitAsync();
            await staffRep.AddAsync(staff3);
            await unitOfWork.CommitAsync();

            Patient pat = new Patient {
                Name = new Name("Matilde Gonçalves"),
                Email = new Email("matilde@gmail.com"),
                Phone = new Phone("912345678"),
                Address = new Address("Rua do ISEP", "Porto", "Portugal"),
                DateOfBirth = new DateTime(1998, 10, 10),
                Gender = Gender.Female,
                EmergencyContact = new Phone("912345678"),
            };

            await patientRep.AddAsync(pat);
            await unitOfWork.CommitAsync();

            var operationRequest1 = new OperationRequest {
                Patient = await patientRep.GetPatientByEmail(pat.Email.ToString()),
                Staff = await staffRep.GetStaffMemberByEmail(staff1.Email.ToString()),
                OperationType = await operationTypeRep.GetByIdAsync(new OperationTypeID(1)),
                Priority = Priority.High,
                Deadline = new Deadline(new DateTime(2025, 10, 10, 10, 0, 0)),
                Status = Status.Pending,
                Staffs = new List<StaffID> { 
                    (await staffRep.GetStaffMemberByEmail(staff1.Email.ToString())).Id,
                    (await staffRep.GetStaffMemberByEmail(staff2.Email.ToString())).Id,
                    (await staffRep.GetStaffMemberByEmail(staff3.Email.ToString())).Id 
                }
            };
            var operationRequest2 = new OperationRequest {
                Patient = await patientRep.GetPatientByEmail(pat.Email.ToString()),
                Staff = await staffRep.GetStaffMemberByEmail(staff1.Email.ToString()),
                OperationType = await operationTypeRep.GetByIdAsync(new OperationTypeID(1)),
                Priority = Priority.High,
                Deadline = new Deadline(new DateTime(2025, 10, 10, 10, 0, 0)),
                Status = Status.Pending,
                Staffs = new List<StaffID> { 
                    (await staffRep.GetStaffMemberByEmail(staff1.Email.ToString())).Id,
                    (await staffRep.GetStaffMemberByEmail(staff2.Email.ToString())).Id,
                    (await staffRep.GetStaffMemberByEmail(staff3.Email.ToString())).Id 
                }
            };
            await request.AddAsync(operationRequest1);
            await request.AddAsync(operationRequest2);
            await unitOfWork.CommitAsync();
        }

        private static async Task SeedRooms(IServiceProvider services)
        {
            var roomRep = services.GetRequiredService<IRoomRepository>();
            var roomTypeRep = services.GetRequiredService<IRoomTypeRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            if ((await roomRep.GetAllAsync()).Count > 0)
            {
                return;
            }

            var room1 = new Room {
                Capacity = new Capacity(2),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Bisturi", "Scalpels" }),
                RoomStatus = RoomStatus.Available,
                Slots = new List<Slot> { 
                    new Slot("21-10-2025T09:00:00 - 21-10-2025T11:00:00"),
                    new Slot("21-10-2025T14:00:00 - 21-10-2025T16:00:00"),
                    new Slot("21-10-2025T18:00:00 - 21-10-2025T20:00:00")
                },
                Type = await roomTypeRep.GetByNameAsync("Operating Room")
            };

            var room2 = new Room {
                Capacity = new Capacity(4),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Monitor", "Defibrillator" }),
                RoomStatus = RoomStatus.Occupied,
                Slots = new List<Slot> { 
                    new Slot("22-10-2025T09:00:00 - 22-10-2025T11:00:00"),
                    new Slot("22-10-2025T14:00:00 - 22-10-2025T16:00:00")
                },
                Type = await roomTypeRep.GetByNameAsync("Operating Room")
            };

            var room3 = new Room {
                Capacity = new Capacity(1),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Ultrasound", "ECG" }),
                RoomStatus = RoomStatus.Available,
                Slots = new List<Slot> { 
                    new Slot("23-10-2025T09:00:00 - 23-10-2025T11:00:00"),
                    new Slot("23-10-2025T14:00:00 - 23-10-2025T16:00:00")
                },
                Type = await roomTypeRep.GetByNameAsync("Operating Room")
            };

            var room4 = new Room {
                Capacity = new Capacity(3),
                AssignedEquipment = new AssignedEquipment(new List<string> { "X-Ray", "MRI" }),
                RoomStatus = RoomStatus.UnderMaintenance,
                Slots = new List<Slot> { 
                    new Slot("24-10-2025T09:00:00 - 24-10-2025T11:00:00"),
                    new Slot("24-10-2025T14:00:00 - 24-10-2025T16:00:00")
                },
                Type = await roomTypeRep.GetByNameAsync("Operating Room")
            };

            var room5 = new Room {
                Capacity = new Capacity(2),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Ventilator", "Suction Machine" }),
                RoomStatus = RoomStatus.Available,
                Slots = new List<Slot> { 
                    new Slot("25-10-2025T09:00:00 - 25-10-2025T11:00:00"),
                    new Slot("25-10-2025T14:00:00 - 25-10-2025T16:00:00")
                },
                Type = await roomTypeRep.GetByNameAsync("Operating Room")
            };

            var room6 = new Room {
                Capacity = new Capacity(5),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Infusion Pump", "Patient Monitor" }),
                RoomStatus = RoomStatus.Occupied,
                Slots = new List<Slot> { 
                    new Slot("26-10-2025T09:00:00 - 26-10-2025T11:00:00"),
                    new Slot("26-10-2025T14:00:00 - 26-10-2025T16:00:00")
                },
                Type = await roomTypeRep.GetByNameAsync("Operating Room")
            };

            var room7 = new Room {
                Capacity = new Capacity(1),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Oxygen Cylinder", "Nebulizer" }),
                RoomStatus = RoomStatus.Available,
                Slots = new List<Slot> { 
                    new Slot("27-10-2025T09:00:00 - 27-10-2025T11:00:00"),
                    new Slot("27-10-2025T14:00:00 - 27-10-2025T16:00:00")
                },
                Type = await roomTypeRep.GetByNameAsync("Operating Room")
            };

            var room8 = new Room {
                Capacity = new Capacity(3),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Dialysis Machine", "Blood Warmer" }),
                RoomStatus = RoomStatus.UnderMaintenance,
                Slots = new List<Slot> { 
                    new Slot("28-10-2025T09:00:00 - 28-10-2025T11:00:00"),
                    new Slot("28-10-2025T14:00:00 - 28-10-2025T16:00:00")
                },
                Type = await roomTypeRep.GetByNameAsync("Operating Room")
            };
            
            await roomRep.AddAsync(room1);
            await roomRep.AddAsync(room2);
            await roomRep.AddAsync(room3);
            await roomRep.AddAsync(room4);
            await roomRep.AddAsync(room5);
            await roomRep.AddAsync(room6);
            await roomRep.AddAsync(room7);
            await roomRep.AddAsync(room8);


            await unitOfWork.CommitAsync();
        }

        private static async Task SeedSpecializationsAsync(IServiceProvider services)
        {
            var specRep = services.GetRequiredService<ISpecializationRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            if ((await specRep.GetAllAsync()).Count > 0)
            {
                return; 
            }

            // Adicionar especializações com nome e ID
            var specialization = new Specialization{Code = "1212", Name = "Cardiology"};
            var specialization1 = new Specialization{Code = "1213", Name = "Neurology"};
            
            await specRep.AddAsync(specialization);
            await specRep.AddAsync(specialization1);

            await unitOfWork.CommitAsync();
        }

        private static async Task SeedAllergiesAsync(IServiceProvider services)
        {
            var allergySer = services.GetRequiredService<AllergyService>();

            // Adicionar alergia com nome e ID
            var allergy = new AllergyDTO { Code = 1234, Name = "Pollen", Description = "Allergy to pollen (e.g., grass, ragweed)" };
            var allergy1 = new AllergyDTO { Code = 1235, Name = "Peanuts", Description = "Allergy to peanuts" };
            var allergy2 = new AllergyDTO { Code = 1236, Name = "Dust", Description = "Allergy to dust" };
            var allergy3 = new AllergyDTO { Code = 1237, Name = "Shellfish", Description = "Allergy to shellfish (e.g., shrimp, lobster)" };
            var allergy4 = new AllergyDTO { Code = 1238, Name = "Milk", Description = "Allergy to milk (Dairy products)" };
            var allergy5 = new AllergyDTO { Code = 1239, Name = "Eggs", Description = "Allergy to eggs" };
            var allergy6 = new AllergyDTO { Code = 1240, Name = "Tree Nut", Description = "Allergy to tree nut (e.g., almonds, walnuts)" };
            var allergy7 = new AllergyDTO { Code = 1241, Name = "Wheat", Description = "Allergy to wheat" };
            var allergy8 = new AllergyDTO { Code = 1242, Name = "Penicillin", Description = "Allergy to penicillin" };
            var allergy9 = new AllergyDTO { Code = 1243, Name = "Sulfa Drugs", Description = "Allergy to sulfa drugs (e.g., sulfamethoxazole)" };
            var allergy10 = new AllergyDTO { Code = 1244, Name = "Aspirin", Description = "Allergy to aspirin" };
            var allergy11 = new AllergyDTO { Code = 1245, Name = "Local Anesthetics", Description = "Allergy to local anesthetics (e.g., Lidocaine)" };
            var allergy12 = new AllergyDTO { Code = 1246, Name = "Mold", Description = "Allergy to mold" };
            var allergy13 = new AllergyDTO { Code = 1247, Name = "Cat Dander", Description = "Allergy to cat dander" };
            var allergy14 = new AllergyDTO { Code = 1248, Name = "Dog Dander", Description = "Allergy to dog dander" };
            var allergy15 = new AllergyDTO { Code = 1249, Name = "Latex", Description = "Allergy to latex" };
            var allergy16 = new AllergyDTO { Code = 1250, Name = "Nickel Allergy", Description = "Allergy to nickel allergy (common in jewelry or metal objects)" };
            var allergy17 = new AllergyDTO { Code = 1251, Name = "Bee Sting", Description = "Allergy to bee sting" };
            var allergy18 = new AllergyDTO { Code = 1252, Name = "Fire Ant Sting", Description = "Allergy to fire ant sting" };
            var allergy19 = new AllergyDTO { Code = 1253, Name = "Perfume", Description = "Allergy to perfume (fragrance sensitivity)" };

            var result = await allergySer.AddAllergy(allergy);
            var result1 = await allergySer.AddAllergy(allergy1);
            var result2 = await allergySer.AddAllergy(allergy2);
            var result3 = await allergySer.AddAllergy(allergy3);
            var result4 = await allergySer.AddAllergy(allergy4);
            var result5 = await allergySer.AddAllergy(allergy5);
            var result6 = await allergySer.AddAllergy(allergy6);
            var result7 = await allergySer.AddAllergy(allergy7);
            var result8 = await allergySer.AddAllergy(allergy8);
            var result9 = await allergySer.AddAllergy(allergy9);
            var result10 = await allergySer.AddAllergy(allergy10);
            var result11 = await allergySer.AddAllergy(allergy11);
            var result12 = await allergySer.AddAllergy(allergy12);
            var result13 = await allergySer.AddAllergy(allergy13);
            var result14 = await allergySer.AddAllergy(allergy14);
            var result15 = await allergySer.AddAllergy(allergy15);
            var result16 = await allergySer.AddAllergy(allergy16);
            var result17 = await allergySer.AddAllergy(allergy17);
            var result18 = await allergySer.AddAllergy(allergy18);
            var result19 = await allergySer.AddAllergy(allergy19);

            if (result == null || result1 == null || result2 == null || result3 == null || result4 == null || result5 == null || result6 == null || result7 == null || result8 == null || result9 == null || result10 == null || result11 == null || result12 == null || result13 == null || result14 == null || result15 == null || result16 == null || result17 == null || result18 == null || result19 == null)
            {
                // Log an error or handle the failure case
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError("Failed to create some allergy");
            }
            else
            {
                // Log success or handle the success case
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Successfully created allergies");
            }
        }

        private static async Task SeedConditionsAsync(IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            var conditionSer = services.GetRequiredService<MedicalConditionService>();

            try
            {
                var condition = new MedicalConditionDTO { Code = "X437", Name = "Asthma", Description = "Asthma condition" };
                var condition1 = new MedicalConditionDTO { Code = "X438", Name = "Diabetes", Description = "Diabetes condition" };
                var condition2 = new MedicalConditionDTO { Code = "X439", Name = "Hypertension", Description = "Hypertension condition" };

                var result = await conditionSer.AddMedicalCondition(condition);
                var result1 = await conditionSer.AddMedicalCondition(condition1);
                var result2 = await conditionSer.AddMedicalCondition(condition2);

                if (result == null || result1 == null || result2 == null)
                {
                    logger.LogError("Failed to create some conditions. Results: {Result}, {Result1}, {Result2}", result, result1, result2);
                }
                else
                {
                    logger.LogInformation("Successfully created conditions");
                }
            }
            catch (HttpRequestException httpEx)
            {
                logger.LogError(httpEx, "HTTP request error while creating conditions");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating conditions");
            }
        }

        private static async Task SeedUsersAsync(IServiceProvider services)
        {
            var userRep = services.GetRequiredService<IUserRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            if ((await userRep.GetAllAsync()).Count > 0)
            {
                return;
            }

            // TODO: Add more users as needed
            var user1 = new SystemUser{
                Username = "tomas",
                Email = new Email("tomasandreleite@gmail.com"),
                Role = "Admin",
                Active = true
            };
            var user2 = new SystemUser{
                Username = "simao",
                Email = new Email("simooncat@gmail.com"),
                Role = "Admin",
                Active = true
            };
            var user3 = new SystemUser{
                Username = "ricardo",
                Email = new Email("ricardo.guimaraes400@gmail.com"),
                Role = "Admin",
                Active = true
            };
            var user4 = new SystemUser{
                Username = "tomasStaff",
                Email = new Email("left4deadgame2@gmail.com"),
                Role = "Staff",
                Active = true
            };
            var user5 = new SystemUser{
                Username = "simaoPatient",
                Email = new Email("sblsimaolopes@gmail.com"),
                Role = "Patient",
                Active = true
            };
            var user6 = new SystemUser{
                Username = "tomasPatient",
                Email = new Email("sem5pi.isep@gmail.com"),
                Role = "Patient",
                Active = true
            };
            var user7 = new SystemUser{
                Username = "joana",
                Email = new Email("joanarebelo@gmail.com"),
                Role = "Doctor",
                Active = true
            };
            var user8 = new SystemUser{
                Username = "francisco",
                Email = new Email("franciscodoc@gmail.com"),
                Role = "Doctor",
                Active = true
            };
            var user9 = new SystemUser{
                Username = "jasmin",
                Email = new Email("jasminfidalgo@gmail.com"),
                Role = "Assistant",
                Active = true
            };
            var user10 = new SystemUser{
                Username = "luis",
                Email = new Email("luisgameplay@gmail.com"),
                Role = "Patient",
                Active = true
            };
            var user11 = new SystemUser{
                Username = "beatriz",
                Email = new Email("biamarques@gmail.com"),
                Role = "Patient",
                Active = true
            };
            var user12 = new SystemUser{
                Username = "miguel",
                Email = new Email("safetyplace4all@gmail.com"),
                Role = "Doctor",
                Active = true
            };

            await userRep.AddAsync(user1);
            await userRep.AddAsync(user2);
            await userRep.AddAsync(user3);
            await userRep.AddAsync(user4);
            await userRep.AddAsync(user5);
            await userRep.AddAsync(user6);
            await userRep.AddAsync(user7);
            await userRep.AddAsync(user8);
            await userRep.AddAsync(user9);
            await userRep.AddAsync(user10);
            await userRep.AddAsync(user11);
            await userRep.AddAsync(user12);

            await unitOfWork.CommitAsync();
        }

        private static async Task SeedPatientAsync(IServiceProvider services)
        {
            var patientRep = services.GetRequiredService<IPatientRepository>();
            var userRep = services.GetRequiredService<IUserRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            Patient pat1 = new Patient {
                Name = new Name("Luís Pedro"),
                Email = new Email("luisgameplay@gmail.com"),
                Phone = new Phone("91454345"),
                Address = new Address("Rua de Cima", "Coimbra", "Portugal"),
                DateOfBirth = new DateTime(1999, 01, 09),
                Gender = Gender.Male,
                EmergencyContact = new Phone("918967345"),
                SystemUser = await userRep.GetUserByEmail("luisgameplay@gmail.com")
            };

            Patient pat2 = new Patient {
                Name = new Name("Beatriz Marques"),
                Email = new Email("biamarques@gmail.com"),
                Phone = new Phone("912434561"),
                Address = new Address("Rua do Baixo", "Lisboa", "Portugal"),
                DateOfBirth = new DateTime(1997, 05, 15),
                Gender = Gender.Other,
                EmergencyContact = new Phone("96342098"),
                SystemUser = await userRep.GetUserByEmail("biamarques@gmail.com")
            };

            Patient pat3 = new Patient {
                Name = new Name("Simao Patient"),
                Email = new Email("sblsimaolopes@gmail.com"),
                Phone = new Phone("987364512"),
                Address = new Address("Rua do Alto", "Porto", "Portugal"),
                DateOfBirth = new DateTime(2000, 10, 10),
                Gender = Gender.Male,
                EmergencyContact = new Phone("921232657"),
                SystemUser = await userRep.GetUserByEmail("sblsimaolopes@gmail.com")
            };

            await patientRep.AddAsync(pat1);
            await unitOfWork.CommitAsync();
            await patientRep.AddAsync(pat2);
            await unitOfWork.CommitAsync();
            await patientRep.AddAsync(pat3);
            await unitOfWork.CommitAsync();
           
            await unitOfWork.CommitAsync();
        }

        private static async Task SeedStaffAsync(IServiceProvider services)
        {
            var staffRep = services.GetRequiredService<IStaffRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();
            var specRepo = services.GetRequiredService<ISpecializationRepository>();
            var userRep = services.GetRequiredService<IUserRepository>();

            //Staff Creation
            var staff1 = new Staff {
                Name = new Name("Joana Coutinho"),
                Email = new Email("joanarebelo@gmail.com"),
                Specialization = await specRepo.GetByName("Orthopaedist"),
                LicenseNumber = new LicenseNumber("983012"),
                Phone = new Phone("917864938"),
                Address = new Address("Rua Vermelha", "Alentejo", "Portugal"),
                AvailabilitySlots = new List<AvailabilitySlot> { 
                    new AvailabilitySlot("01-05-2025T08:00:00 - 01-05-2025T20:00:00")
                },
                SystemUser = await userRep.GetUserByEmail("joanarebelo@gmail.com")
            };
            var staff2 = new Staff {
                Name = new Name("Francisco Trindade"),
                Email = new Email("franciscodoc@gmail.com"),
                Specialization = await specRepo.GetByName("Anaesthetist"),
                LicenseNumber = new LicenseNumber("546392"),
                Phone = new Phone("981232786"),
                Address = new Address("Rua da Paz", "Porto", "Portugal"),
                AvailabilitySlots = new List<AvailabilitySlot> { 
                    new AvailabilitySlot("08-03-2025T08:00:00 - 08-03-2025T20:00:00")
                },
                SystemUser = await userRep.GetUserByEmail("franciscodoc@gmail.com")
            };
            var staff3 = new Staff {
                Name = new Name("Jasmin Fidalgo"),
                Email = new Email("jasminfidalgo@gmail.com"),
                Specialization = await specRepo.GetByName("Neurology"),
                LicenseNumber = new LicenseNumber("938435"),
                Phone = new Phone("932456187"),
                Address = new Address("Rua Montanhosa", "Guarda", "Portugal"),
                AvailabilitySlots = new List<AvailabilitySlot> { 
                    new AvailabilitySlot("22-06-2025T08:00:00 - 22-06-2025T20:00:00")
                },
                SystemUser = await userRep.GetUserByEmail("jasminfidalgo@gmail.com")
            };
            var staff4 = new Staff {
                Name = new Name("Miguel Silva"),
                Email = new Email("safetyplace4all@gmail.com"),
                Specialization = await specRepo.GetByName("Anaesthetist"),
                LicenseNumber = new LicenseNumber("987395"),
                Phone = new Phone("91918276"),
                Address = new Address("Rua do Porto", "Aveiro", "Portugal"),
                AvailabilitySlots = new List<AvailabilitySlot> { 
                    new AvailabilitySlot("10-10-2025T08:00:00 - 10-10-2025T20:00:00")
                },
                SystemUser = await userRep.GetUserByEmail("safetyplace4all@gmail.com")
            };

            await staffRep.AddAsync(staff1);
            await unitOfWork.CommitAsync();
            await staffRep.AddAsync(staff2);
            await unitOfWork.CommitAsync();
            await staffRep.AddAsync(staff3);
            await unitOfWork.CommitAsync();
            await staffRep.AddAsync(staff4);
            await unitOfWork.CommitAsync();
        }

        private static async Task SeedOperationRequestsAsync(IServiceProvider services)
        {
            var request = services.GetRequiredService<IOperationRequestRepository>();
            var specRepo = services.GetRequiredService<ISpecializationRepository>();
            var patientRep = services.GetRequiredService<IPatientRepository>();
            var operationTypeRep = services.GetRequiredService<IOperationTypeRepository>();
            var staffRep = services.GetRequiredService<IStaffRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            var operationRequest1 = new OperationRequest {
                Patient = await patientRep.GetPatientByEmail("luisgameplay@gmail.com"),
                Staff = await staffRep.GetStaffMemberByEmail("safetyplace4all@gmail.com"),
                OperationType = await operationTypeRep.GetByIdAsync(new OperationTypeID(1)),
                Priority = Priority.Medium,
                Deadline = new Deadline(new DateTime(2025, 10, 10, 10, 0, 0)),
                Status = Status.Pending,
            };
            var operationRequest2 = new OperationRequest {
                Patient = await patientRep.GetPatientByEmail("biamarques@gmail.com"),
                Staff = await staffRep.GetStaffMemberByEmail("safetyplace4all@gmail.com"),
                OperationType = await operationTypeRep.GetByIdAsync(new OperationTypeID(1)),
                Priority = Priority.High,
                Deadline = new Deadline(new DateTime(2025, 10, 10, 10, 0, 0)),
                Status = Status.Pending,
            };
            await request.AddAsync(operationRequest1);
            await request.AddAsync(operationRequest2);
            await unitOfWork.CommitAsync();

        }

        private static async Task SeedOperationTypeAsync(IServiceProvider services)
        {
            var operationTypeRep = services.GetRequiredService<IOperationTypeRepository>();
            var specsRep = services.GetRequiredService<ISpecializationRepository>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();
            var operationTypes = await operationTypeRep.GetAllAsync();

            if (operationTypes.Count() > 0)
            {
                return;
            }
                var operationType1 = new OperationType{
                    Name = "Cardiology Operation",
                    Anesthesia_Duration = 10,
                    Surgery_Duration = 15,
                    Cleaning_Duration = 10,
                    Specialization = await specsRep.GetByName("Cardiology")
                };
                var operationType2 = new OperationType{
                    Name = "Neurology Operation",
                    Anesthesia_Duration = 15,
                    Surgery_Duration = 20,
                    Cleaning_Duration = 15,
                    Specialization = await specsRep.GetByName("Neurology")
                };

            await operationTypeRep.AddAsync(operationType1);
            await operationTypeRep.AddAsync(operationType2);

            
            await unitOfWork.CommitAsync();
        }



        public static void ConfigureMyServices(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<SystemUserService>();

            builder.Services.AddTransient<IStaffRepository, StaffRepository>();
            builder.Services.AddTransient<StaffService>();

            builder.Services.AddTransient<IPatientRepository, PatientRepository>();
            builder.Services.AddTransient<PatientService>();

            builder.Services.AddTransient<ISpecializationRepository, SpecializationRepository>();
            builder.Services.AddTransient<SpecializationService>();

            builder.Services.AddTransient<ITokenRepository, TokenRepository>();
            builder.Services.AddHttpClient<MedicalRecordService>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                });

            builder.Services.AddTransient<EmailService>();

            builder.Services.AddTransient<Cryptography>();

            builder.Services.AddSingleton(Log.Logger);

            builder.Services.AddTransient<IOperationRequestRepository, OperationRequestRepository>();
            builder.Services.AddTransient<OperationRequestService>();

            builder.Services.AddTransient<IOperationTypeRepository, OperationTypeRepository>();
            //services.AddTransient<OperationTypeService>();

            builder.Services.AddTransient<IRoomTypeRepository, RoomTypeRepository>();
            builder.Services.AddTransient<RoomTypeService>();

            builder.Services.AddTransient<IRoomRepository, RoomRepository>();
            builder.Services.AddTransient<RoomService>();

            builder.Services.AddHttpClient<AllergyService>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                });

            builder.Services.AddHttpClient<MedicalConditionService>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                });


            builder.Services.AddHostedService<AccountDeletionBackgroundService>();

            builder.Services.AddTransient<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddTransient<AppointmentService>();
            builder.Services.AddTransient<PlanningService>();

            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
        }
    }
}
