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
                    var notRegistered = false;
                    var claimsIdentity = (ClaimsIdentity) context.Principal.Identity;

                    var services = context.HttpContext.RequestServices;

                    try
                    {
                        IUserRepository userRepository = services.GetRequiredService<IUserRepository>();
                        IStaffRepository staffRepository = services.GetRequiredService<IStaffRepository>();
                        IPatientRepository patientRepository = services.GetRequiredService<IPatientRepository>();

                        IUnitOfWork unitOfWork = services.GetRequiredService<IUnitOfWork>();        

                        var allStaff = await staffRepository.GetAllAsync(); 
                        var allPacient = await patientRepository.GetAllAsync();

                        var email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
                        
                        var staff = await staffRepository.GetStaffMemberByEmail(email);
                        var patient = await patientRepository.GetPatientByEmail(email);
                        var user = await userRepository.GetUserByEmail(email);

                        if (user != null && user.Email.Equals(email))
                        { 
                            // User already exists
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role));                                                  
                        } else {
                        
                            SystemUser newUser = null;
                            if (staff != null) {
                                newUser = new SystemUser { Username = email, Email = email, Role = "Staff" };
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, newUser.Role));
                                staff.SystemUser = newUser;


                            } else if (patient != null) {
                                newUser = new SystemUser { Username = email, Email = email, Role = "Patient" };
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, newUser.Role));
                                patient.SystemUser = newUser;
                            }

                            if (newUser == null) {
                                notRegistered = true;

                            } else {
                                await userRepository.AddAsync(newUser);

                                await unitOfWork.CommitAsync();    
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        Console.WriteLine(ex.StackTrace);
                    }
   
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false
                    };

                    if (!notRegistered) {
                        await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    }

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