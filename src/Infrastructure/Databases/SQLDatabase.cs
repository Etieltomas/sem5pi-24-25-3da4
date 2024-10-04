using Microsoft.EntityFrameworkCore;
using Sempi5.Domain;
using Sempi5.Domain.TodoItem;


namespace Sempi5.Infrastructure.Databases;

public class SQLDatabase : IDatabase
{

    public void connectDB(WebApplicationBuilder builder)
    {
        var connectionString = "Server="+builder.Configuration["DataBase:Server"]+
                                  ";Port="+builder.Configuration["DataBase:Port"]+
                                  ";Database="+builder.Configuration["DataBase:Name"]+
                                  ";User="+builder.Configuration["DataBase:User"]+
                                  ";Password="+builder.Configuration["DataBase:Password"]+";";


        builder.Services.AddDbContext<TodoContext>(opt =>
            opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    }
     

}