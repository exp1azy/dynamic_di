using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace DynamicDI
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/> to dynamically register services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Automatically registers all services decorated with <see cref="ServiceAttribute"/> from all project assemblies.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        public static void RegisterServices(this IServiceCollection services) => HandleRegisterServices(services, GetAssemblies());

        /// <summary>
        /// Automatically registers all services decorated with <see cref="ServiceAttribute"/> from the specified assemblies.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="assemblies">The assemblies to scan for services decorated with <see cref="ServiceAttribute"/>.</param>
        public static void RegisterServices(this IServiceCollection services, params Assembly[] assemblies) => HandleRegisterServices(services, assemblies);

        private static void HandleRegisterServices(this IServiceCollection services, Assembly[] assemblies)
        {
            foreach (var type in GetTypesWithAttribute<ServiceAttribute>(assemblies))
            {
                var attribute = type.GetCustomAttribute<ServiceAttribute>();
                if (attribute == null) continue;

                Type[] implTypes;

                if (attribute.ImplementedTypes == null || attribute.ImplementedTypes.Length == 0)
                    implTypes = type.ImplementedInterfaces.ToArray();
                else
                    implTypes = attribute.ImplementedTypes.ToArray();

                if (implTypes.Length == 0)
                {
                    Register(services, type, type, attribute.LifeCycle);
                }
                else
                {
                    foreach (var implType in implTypes)
                        Register(services, implType, type, attribute.LifeCycle);
                }
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

        private static void Register(IServiceCollection services, Type implementedType, Type serviceType, ServiceLifeCycle lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifeCycle.Transient:
                    services.AddTransient(implementedType, serviceType);
                    break;
                case ServiceLifeCycle.Scoped:
                    services.AddScoped(implementedType, serviceType);
                    break;
                case ServiceLifeCycle.Singleton:
                    services.AddSingleton(implementedType, serviceType);
                    break;
            }
        }
    }
}
