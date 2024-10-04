using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sempi5.Infrastructure.Databases
    {
    interface IDatabase {
        void connectDB(WebApplicationBuilder builder);
    }
}