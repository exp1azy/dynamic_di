using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace DynamicDI
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            var assemblies = GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var servicesToRegister = assembly.DefinedTypes.Where(t => t.IsDefined(typeof(RegisterServiceAttribute)));
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
                            new List<Type> { interfaces.First() } : interfaces;

                        foreach (var iface in interfacesToRegister)
                            Register(services, iface, service, attribute!.LifeCycle);
                    }
                }
            }
        }

        public static void RegisterDbContexts(this IServiceCollection services)
        {
            var assemblies = GetAssemblies();

            var method = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m =>
                    m.Name == "AddDbContext" &&
                    m.IsGenericMethodDefinition &&
                    m.GetGenericArguments().Length == 1 &&
                    m.GetParameters().Any(p => p.ParameterType == typeof(IServiceCollection))
                );

            foreach (var assembly in assemblies)
            {
                var dbContexts = assembly.DefinedTypes.Where(t => t.IsDefined(typeof(RegisterDbContextAttribute)));
                foreach (var contextType in dbContexts)
                {
                    var genericMethod = method!.MakeGenericMethod(contextType);
                    _ = genericMethod.Invoke(null, new object[] { services, null!, ServiceLifetime.Scoped, ServiceLifetime.Scoped });
                }
            }
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            return DependencyContext.Default!.RuntimeLibraries
                .Where(lib => lib.Type == "project")
                .Select(lib => Assembly.Load(new AssemblyName(lib.Name)));
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
