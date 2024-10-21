using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sempi5.Domain.Patient;
using Sempi5.Domain.Shared;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Domain.Staff;
using Sempi5.Domain.User;
using Sempi5.Infrastructure.PatientRepository;
using Sempi5.Infrastructure.Shared;
using Sempi5.Infrastructure.SpecializationRepository;
using Sempi5.Infrastructure.StaffRepository;
using Sempi5.Infrastructure.UserRepository;

namespace Sempi5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            ConfigureIAM(builder);

            builder.Services.AddControllersWithViews();
            
            CreateDataBase(builder);            
            ConfigureMyServices(builder.Services);
            
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            ConfigureMiddleware(app);

            // TODO: Check if I can do this?
            Bootstrap(app);

            app.Run();
        }
        
        private static void ConfigureIAM(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(options =>
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
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;                
            })
            .AddGoogle(options => 
            {
                options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
                options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
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
                    cookie.AddClaim(new Claim(ClaimTypes.Role, user.Role));                                               
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
                        var _emailService = services.GetRequiredService<EmailService>();

                        await userRepository.AddAsync(newUser);
                        await unitOfWork.CommitAsync();    

                        var message = "<b>Hello,</b><br>" +
                        "Thank you for signing up! Please confirm your account by clicking the link below:<br><br>" +
                        "<a href='http://localhost:5012/SystemUser/confirm/" + newUser.Id.AsLong() + "/true'>Click here to confirm your account</a><br><br>" +
                        "If you didn't sign up, please ignore this email.";

                        var subject = "Confirmation of Account";
                        _emailService.sendEmail(newUser.Username, newUser.Email.ToString(), subject, message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            if (notRegistered) 
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(new { error = "User is not registered in the system" }));
                return;
            }
            else 
            {
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };
                await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(cookie), authProperties);
            }
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            app.UseMiddleware<CatchRedirectMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

            var specRep = services.GetRequiredService<ISpecializationRepository>();
            if (specRep.GetAllAsync().Result.Count > 0)
            {
                return;
            }
            try
            {           
                var unitOfWork = services.GetRequiredService<IUnitOfWork>();

                // TODO: Create more specializations 
                var specialization = new Specialization { Id = new SpecializationID("Cardiology") };
                await specRep.AddAsync(specialization);

                await unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
            }
        }


        public static void ConfigureMyServices(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<SystemUserService>();

            services.AddTransient<IStaffRepository, StaffRepository>();
            services.AddTransient<StaffService>();

            services.AddTransient<IPatientRepository, PatientRepository>();
            services.AddTransient<PatientService>();

            services.AddTransient<ISpecializationRepository, SpecializationRepository>();

            services.AddTransient<EmailService>();
        }
    }
}
