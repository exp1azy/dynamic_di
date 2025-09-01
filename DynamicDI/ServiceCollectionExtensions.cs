using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace DynamicDI
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/> to dynamically register services and DbContexts.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all services marked with <see cref="RegisterServiceAttribute"/> in all project assemblies.
        /// </summary>
        /// <param name="services">The service collection to register with.</param>
        public static void RegisterServices(this IServiceCollection services) => HandleRegisterServices(services, GetAssemblies());

        /// <summary>
        /// Registers services marked with <see cref="RegisterServiceAttribute"/> from specific assemblies.
        /// </summary>
        /// <param name="services">The service collection to register with.</param>
        /// <param name="assemblies">The assemblies to scan for services.</param>
        public static void RegisterServices(this IServiceCollection services, params Assembly[] assemblies) => HandleRegisterServices(services, assemblies);

        /// <summary>
        /// Registers all DbContexts marked with <see cref="RegisterDbContextAttribute"/> in all project assemblies.
        /// </summary>
        /// <param name="services">The service collection to register with.</param>
        public static void RegisterDbContexts(this IServiceCollection services) => HandleRegisterDbContexts(services, GetAssemblies());

        /// <summary>
        /// Registers DbContexts marked with <see cref="RegisterDbContextAttribute"/> from specific assemblies.
        /// </summary>
        /// <param name="services">The service collection to register with.</param>
        /// <param name="assemblies">The assemblies to scan for DbContexts.</param>
        public static void RegisterDbContexts(this IServiceCollection services, params Assembly[] assemblies) => HandleRegisterDbContexts(services, assemblies);

        private static void HandleRegisterServices(this IServiceCollection services, Assembly[] assemblies)
        {
            foreach (var type in GetTypesWithAttribute<RegisterServiceAttribute>(assemblies))
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
                        ? new[] { interfaces[0] }
                        : interfaces;

                    foreach (var iface in interfacesToRegister)
                        Register(services, iface, type, attribute.LifeCycle);
                }
            }
        }

        private static void HandleRegisterDbContexts(this IServiceCollection services, Assembly[] assemblies)
        {
            var addDbContextMethod = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m =>
                    m.Name == "AddDbContext" &&
                    m.IsGenericMethodDefinition &&
                    m.GetGenericArguments().Length == 1 &&
                    m.GetParameters().First().ParameterType == typeof(IServiceCollection)
                );

            foreach (var contextType in GetTypesWithAttribute<RegisterDbContextAttribute>(assemblies))
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

        private static IEnumerable<TypeInfo> GetTypesWithAttribute<TAttribute>(Assembly[] assemblies) where TAttribute : Attribute
        {
            return assemblies
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
