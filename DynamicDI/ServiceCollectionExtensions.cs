using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace DynamicDI
{
    public static class ServiceCollectionExtensions
    {
        private static readonly Assembly[] _assemblies = GetAssemblies();

        public static void RegisterServices(this IServiceCollection services)
        {
            foreach (var type in GetTypesWithAttribute<RegisterServiceAttribute>())
            {
                var attribute = type.GetCustomAttribute<RegisterServiceAttribute>();
                if (attribute == null) continue;

                var interfaces = type.ImplementedInterfaces.ToArray();

                if (interfaces.Length == 0)
                {
                    Register(services, type, type, attribute.LifeCycle);
                }
                else
                {
                    var interfacesToRegister = attribute.InterfaceRegistrationStrategy == InterfaceRegistrationStrategy.FirstOnly
                        ? new[] { interfaces.First() }
                        : interfaces;

                    foreach (var iface in interfacesToRegister)
                        Register(services, iface, type, attribute.LifeCycle);
                }
            }
        }

        public static void RegisterDbContexts(this IServiceCollection services)
        {
            var addDbContextMethod = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m =>
                    m.Name == "AddDbContext" &&
                    m.IsGenericMethodDefinition &&
                    m.GetGenericArguments().Length == 1 &&
                    m.GetParameters().First().ParameterType == typeof(IServiceCollection)
                );

            foreach (var contextType in GetTypesWithAttribute<RegisterDbContextAttribute>())
            {
                var genericMethod = addDbContextMethod.MakeGenericMethod(contextType);
                _ = genericMethod.Invoke(null, new object[]
                {
                    services,
                    (Action<DbContextOptionsBuilder>)(_ => { }),
                    ServiceLifetime.Scoped,
                    ServiceLifetime.Scoped
                });
            }
        }

        private static Assembly[] GetAssemblies()
        {
            return DependencyContext.Default!.RuntimeLibraries
                .Where(lib => lib.Type == "project")
                .Select(lib => Assembly.Load(new AssemblyName(lib.Name)))
                .ToArray();
        }

        private static IEnumerable<TypeInfo> GetTypesWithAttribute<TAttribute>() where TAttribute : Attribute
        {
            return _assemblies
                .SelectMany(a => a.DefinedTypes)
                .Where(t => t.IsDefined(typeof(TAttribute), inherit: false));
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
