namespace DynamicDI.Test.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.RegisterServices();

            var app = builder.Build();

            app.MapControllers();
            app.Run();
        }
    }
}
