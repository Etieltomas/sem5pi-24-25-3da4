using Sempi5.Domain.Shared;
using Sempi5.Infrastructure.Shared;
using Sempi5.Infrastructure.TodoItemRepository;
using IDatabase = Sempi5.Infrastructure.Databases.IDatabase;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Sempi5
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
 
            /////////////////////////////////////////
            // IAM
            builder.Services.AddAuthentication(options => 
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = "/Login/login";
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
            }
            ).AddGoogle(options => 
            {
                options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
                options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
            });
            /////////////////////////////////////////


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            CreateDataBase(builder);
            

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

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

        public void ConfigureMyServices(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork,UnitOfWork>();

            services.AddTransient<ITodoItemRepository,TodoItemRepository>();
            //services.AddTransient<CategoryService>();
        }
    }
}