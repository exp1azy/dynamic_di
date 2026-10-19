namespace DynamicDI.Test.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.RegisterServices();

            foreach (var service in builder.Services)
            {
                Console.WriteLine($"Service registered: <{service.ServiceType.Name}, {service.ImplementationType?.Name}>, Lifetime: {service.Lifetime}");
            }

            var app = builder.Build();

            app.MapControllers();
            app.Run();
        }
    }
}
