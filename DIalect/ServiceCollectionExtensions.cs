using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace DIalect
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            var assemblies = DependencyContext.Default!.RuntimeLibraries
                .Where(lib => lib.Type == "project") 
                .Select(lib => Assembly.Load(new AssemblyName(lib.Name)))
                .ToList();

            foreach (var assembly in assemblies)
            {
                var servicesToRegister = assembly.DefinedTypes.Where(t => t.IsDefined(typeof(RegisterServiceAttribute))).ToList();

                foreach (var service in servicesToRegister)
                {
                    var attribute = service.GetCustomAttribute<RegisterServiceAttribute>();
                    var interfaces = service.ImplementedInterfaces.ToList();

                    if (interfaces.Count == 0)
                    {
                        Register(services, service, service, attribute!.LifeCycle);
                    }
                    else
                    {
                        var interfacesToRegister = attribute!.InterfaceRegistrationStrategy == InterfaceRegistrationStrategy.FirstOnly ?
                            [interfaces.First()] : interfaces;

                        foreach (var iface in interfacesToRegister)
                            Register(services, iface, service, attribute!.LifeCycle);
                    }
                }
            }
        }

        private static void Register(IServiceCollection services, Type serviceType, Type implementationType, ServiceLifeCycle lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifeCycle.Transient:
                    services.AddTransient(serviceType, implementationType);
                    break;
                case ServiceLifeCycle.Scoped:
                    services.AddScoped(serviceType, implementationType);
                    break;
                case ServiceLifeCycle.Singleton:
                    services.AddSingleton(serviceType, implementationType);
                    break;
            }
        }
    }
}
