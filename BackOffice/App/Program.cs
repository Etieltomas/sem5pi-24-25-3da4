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
                options.Cookie.Domain = ".sarm.com";
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
                await SeedUsersAsync(services);
                await SeedOperationTypeAsync(services);
                await SeedRooms(services);
                await SeedPlanning(services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
            }
        }

        private static async Task SeedPlanning(IServiceProvider services)
        {
            var roomRep = services.GetRequiredService<IRoomRepository>();
            var staffRep = services.GetRequiredService<IStaffRepository>();
            var request = services.GetRequiredService<IOperationRequestRepository>();
            var specRepo = services.GetRequiredService<ISpecializationRepository>();
            var patientRep = services.GetRequiredService<IPatientRepository>();
            var operationTypeRep = services.GetRequiredService<IOperationTypeRepository>();

            var unitOfWork = services.GetRequiredService<IUnitOfWork>();

            if (
                ((await roomRep.GetAllAsync()).Count() == 0) && 
                ((await request.GetAllAsync()).Count() == 0) &&
                ((await staffRep.GetAllAsync()).Count() == 0) &&
                ((await operationTypeRep.GetAllAsync()).Count() == 0)
            )
            {
                return;
            }

            var room9 = new Room {
                Capacity = new Capacity(2),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Bisturi", "Scalpels" }),
                RoomStatus = RoomStatus.Available,
                Slots = new List<Slot> { 
                    new Slot("10-10-2025T09:00:00 - 10-10-2025T10:00:00"),
                    new Slot("10-10-2025T18:00:00 - 10-10-2025T20:00:00")
                },
                Type = RoomType.OperatingRoom
            };
            await roomRep.AddAsync(room9);
            await unitOfWork.CommitAsync();


            var staff1 = new Staff {
                Name = new Name("Tomás Leite"),
                Email = new Email("tomasandreleite@gmail.com"),
                Specialization = await specRepo.GetByName("Cardiology"),
                LicenseNumber = new LicenseNumber("123456"),
                Phone = new Phone("912345678"),
                Address = new Address("Rua do ISEP", "Porto", "Portugal"),
                AvailabilitySlots = new List<AvailabilitySlot> { 
                    new AvailabilitySlot("10-10-2025T07:00:00 - 10-10-2025T10:00:00"),
                    new AvailabilitySlot("10-10-2025T11:30:00 - 10-10-2025T12:00:00"),
                    new AvailabilitySlot("10-10-2025T13:00:00 - 10-10-2025T15:00:00"),
                    new AvailabilitySlot("10-10-2025T16:00:00 - 10-10-2025T18:00:00")
                }
            };
            var staff2 = new Staff {
                Name = new Name("Simão Lopes"),
                Email = new Email("sblsimaolopes@gmail.com"),
                Specialization = await specRepo.GetByName("Neurology"),
                LicenseNumber = new LicenseNumber("654321"),
                Phone = new Phone("987654321"),
                Address = new Address("Rua do ISEP", "Porto", "Portugal"),
                AvailabilitySlots = new List<AvailabilitySlot> { 
                    new AvailabilitySlot("10-10-2025T07:00:00 - 10-10-2025T10:00:00"),
                    new AvailabilitySlot("10-10-2025T11:30:00 - 10-10-2025T12:00:00"),
                    new AvailabilitySlot("10-10-2025T14:00:00 - 10-10-2025T16:00:00"),
                    new AvailabilitySlot("10-10-2025T16:00:00 - 10-10-2025T18:00:00")
                }
            };
            await staffRep.AddAsync(staff1);
            await staffRep.AddAsync(staff2);
            await unitOfWork.CommitAsync();

            Patient pat = new Patient {
                    Name = new Name("Simão Lopes"),
                    Email = new Email("teste@gmail.com"),
                    Phone = new Phone("912345678"),
                    Address = new Address("Rua do ISEP", "Porto", "Portugal"),
                    DateOfBirth = new DateTime(1998, 10, 10),
                    Gender = Gender.Male,
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
                Staffs = new List<Staff> { 
                    await staffRep.GetStaffMemberByEmail(staff1.Email.ToString()),
                    await staffRep.GetStaffMemberByEmail(staff2.Email.ToString()) 
                }
            };
            await request.AddAsync(operationRequest1);

            await unitOfWork.CommitAsync();
        }

        private static async Task SeedRooms(IServiceProvider services)
        {
            var roomRep = services.GetRequiredService<IRoomRepository>();
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
                Type = RoomType.OperatingRoom
            };

            var room2 = new Room {
                Capacity = new Capacity(4),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Monitor", "Defibrillator" }),
                RoomStatus = RoomStatus.Occupied,
                Slots = new List<Slot> { 
                    new Slot("22-10-2025T09:00:00 - 22-10-2025T11:00:00"),
                    new Slot("22-10-2025T14:00:00 - 22-10-2025T16:00:00")
                },
                Type = RoomType.OperatingRoom
            };

            var room3 = new Room {
                Capacity = new Capacity(1),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Ultrasound", "ECG" }),
                RoomStatus = RoomStatus.Available,
                Slots = new List<Slot> { 
                    new Slot("23-10-2025T09:00:00 - 23-10-2025T11:00:00"),
                    new Slot("23-10-2025T14:00:00 - 23-10-2025T16:00:00")
                },
                Type = RoomType.OperatingRoom
            };

            var room4 = new Room {
                Capacity = new Capacity(3),
                AssignedEquipment = new AssignedEquipment(new List<string> { "X-Ray", "MRI" }),
                RoomStatus = RoomStatus.UnderMaintenance,
                Slots = new List<Slot> { 
                    new Slot("24-10-2025T09:00:00 - 24-10-2025T11:00:00"),
                    new Slot("24-10-2025T14:00:00 - 24-10-2025T16:00:00")
                },
                Type = RoomType.OperatingRoom
            };

            var room5 = new Room {
                Capacity = new Capacity(2),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Ventilator", "Suction Machine" }),
                RoomStatus = RoomStatus.Available,
                Slots = new List<Slot> { 
                    new Slot("25-10-2025T09:00:00 - 25-10-2025T11:00:00"),
                    new Slot("25-10-2025T14:00:00 - 25-10-2025T16:00:00")
                },
                Type = RoomType.OperatingRoom
            };

            var room6 = new Room {
                Capacity = new Capacity(5),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Infusion Pump", "Patient Monitor" }),
                RoomStatus = RoomStatus.Occupied,
                Slots = new List<Slot> { 
                    new Slot("26-10-2025T09:00:00 - 26-10-2025T11:00:00"),
                    new Slot("26-10-2025T14:00:00 - 26-10-2025T16:00:00")
                },
                Type = RoomType.OperatingRoom
            };

            var room7 = new Room {
                Capacity = new Capacity(1),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Oxygen Cylinder", "Nebulizer" }),
                RoomStatus = RoomStatus.Available,
                Slots = new List<Slot> { 
                    new Slot("27-10-2025T09:00:00 - 27-10-2025T11:00:00"),
                    new Slot("27-10-2025T14:00:00 - 27-10-2025T16:00:00")
                },
                Type = RoomType.OperatingRoom
            };

            var room8 = new Room {
                Capacity = new Capacity(3),
                AssignedEquipment = new AssignedEquipment(new List<string> { "Dialysis Machine", "Blood Warmer" }),
                RoomStatus = RoomStatus.UnderMaintenance,
                Slots = new List<Slot> { 
                    new Slot("28-10-2025T09:00:00 - 28-10-2025T11:00:00"),
                    new Slot("28-10-2025T14:00:00 - 28-10-2025T16:00:00")
                },
                Type = RoomType.OperatingRoom
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
            var specialization = new Specialization{Name = "Cardiology"};
            var specialization1 = new Specialization{Name = "Neurology"};
            
            await specRep.AddAsync(specialization);
            await specRep.AddAsync(specialization1);

            await unitOfWork.CommitAsync();
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

            await userRep.AddAsync(user1);
            await userRep.AddAsync(user2);
            await userRep.AddAsync(user3);
            await userRep.AddAsync(user4);
            await userRep.AddAsync(user5);
            await userRep.AddAsync(user6);

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

            builder.Services.AddTransient<IRoomRepository, RoomRepository>();
            builder.Services.AddTransient<RoomService>();

            builder.Services.AddHttpClient<AllergyService>()
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
