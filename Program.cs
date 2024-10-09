using Sempi5.Domain.Shared;
using Sempi5.Infrastructure.Shared;
using IDatabase = Sempi5.Infrastructure.Databases.IDatabase;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sempi5.Infrastructure.UserRepository;
using Sempi5.Infrastructure.StaffRepository;
using Sempi5.Domain.Staff;
using Sempi5.Infrastructure.PatientRepository;
using Sempi5.Domain.Patient;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Sempi5.Domain.User;

namespace Sempi5
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
 
            /////////////////////////////////////////
            // IAM
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Staff",
                policy => policy.RequireRole("Doctor", "Nurse", "Admin", "Technician"));

                options.AddPolicy("Patient",
                policy => policy.RequireRole("Patient"));
                
            }).AddAuthentication(options => 
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;                
            }
            ).AddGoogle(options => 
            {
                options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
                options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];

                options.Events.OnCreatingTicket = async context =>
                {
                    var claimsIdentity = (ClaimsIdentity) context.Principal.Identity;

                    var services = context.HttpContext.RequestServices;

                    try
                    {
                        IUserRepository userRepository = services.GetRequiredService<IUserRepository>();
                        IPatientRepository patientRepository = services.GetRequiredService<IPatientRepository>();
                        IStaffRepository staffRepository = services.GetRequiredService<IStaffRepository>();

                        var allUsers = await userRepository.GetAllUsers(); 
                        var allStaff = await staffRepository.GetAllStaffMembers();
                        var allPacient = await patientRepository.GetAllPatients();

                        var email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
                        var user = await userRepository.GetUserByEmail(email);

                        if (user != null && user.Email.Equals(email))
                        { 
                            // User already exists
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role));                                                  
                        }
                        else
                        {
                            // New user so it will be a patient
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Patient"));
                            
                            await userRepository.AddUser(new SystemUserDTO { Username = email, Email = email, Role = "Patient" });
                            await patientRepository.AddPatient(new PatientDTO { 
                                Name = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value,
                                Email = email,
                                Phone = "12121212",
                                Conditions = new List<string> { "None" },
                                EmergencyContact = "1212121212",
                                DateOfBirth = "12/12/1121"
                            });
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
   
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false
                    };

                    await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                }; 
                
            });
            /////////////////////////////////////////

            // Add services to the container
            builder.Services.AddControllersWithViews();
            CreateDataBase(builder);            
            ConfigureMyServices(builder.Services);
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

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
            app.Run();
        }
        
        // See what database is
        public static void CreateDataBase(WebApplicationBuilder builder){
          
            string name = "Sempi5.Infrastructure.Databases." + builder.Configuration["DataBase:Type"];
            Type? dbType = Type.GetType(name);
            
            if (dbType == null)
            {
                // Exit application
                Console.WriteLine("Database Type Invalid. Please check the configuration file!\nApplication will exit");
                Environment.Exit(2);
            }   

            try{ 
                ((IDatabase) Activator.CreateInstance(dbType)).connectDB(builder);                           
            }
            catch (Exception)
            {
                // Exit application
                Console.WriteLine("Database not found\nApplication will exit");
                Environment.Exit(3);
            }
                
        }

        public static void ConfigureMyServices(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork,UnitOfWork>();
            
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<SystemUserService>();

            services.AddTransient<IStaffRepository,StaffRepository>();
            services.AddTransient<StaffService>();
        
            services.AddTransient<IPatientRepository,PatientRepository>();
            services.AddTransient<PatientService>();
        }
    }
}