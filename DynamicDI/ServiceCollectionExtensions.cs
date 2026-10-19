using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace DynamicDI
{
    /// <summary>
    /// Provides extension methods for registering services marked with <see cref="ServiceAttribute"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers services marked with <see cref="ServiceAttribute"/> from project assemblies
        /// discovered through <see cref="DependencyContext.Default"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection to populate.</param>
        /// <remarks>
        /// Only runtime libraries whose <c>Type</c> is <c>"project"</c> are scanned.
        /// </remarks>
        public static void RegisterServices(this IServiceCollection serviceCollection)
        {
            var assemblies = DependencyContext.Default!.RuntimeLibraries
                .Where(lib => lib.Type == "project")
                .Select(lib => Assembly.Load(new AssemblyName(lib.Name)))
                .ToArray();

            HandleRegisterServices(serviceCollection, assemblies);
        }

        /// <summary>
        /// Registers services marked with <see cref="ServiceAttribute"/> from the specified assemblies.
        /// </summary>
        /// <param name="serviceCollection">The service collection to populate.</param>
        /// <param name="assemblies">The assemblies to scan for decorated services.</param>
        public static void RegisterServices(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            HandleRegisterServices(serviceCollection, assemblies);
        }

        private static void HandleRegisterServices(this IServiceCollection serviceCollection, Assembly[] assemblies)
        {
            var servicesToRegister = assemblies
                .SelectMany(a => a.DefinedTypes)
                .Where(t => t.IsDefined(typeof(ServiceAttribute), inherit: false));

            foreach (var serviceToRegister in servicesToRegister)
            {
                var serviceAttribute = serviceToRegister.GetCustomAttribute<ServiceAttribute>();
                if (serviceAttribute == null) continue;

                Type[] implementedTypes;

                if (serviceAttribute.ImplementedTypes == null || serviceAttribute.ImplementedTypes.Length == 0)
                    implementedTypes = serviceToRegister.ImplementedInterfaces.ToArray();
                else
                    implementedTypes = serviceAttribute.ImplementedTypes.ToArray();

                if (implementedTypes.Length == 0)
                {
                    switch (serviceAttribute.Lifetime)
                    {
                        case ServiceLifetime.Transient:
                            serviceCollection.AddTransient(serviceToRegister);
                            break;
                        case ServiceLifetime.Scoped:
                            serviceCollection.AddScoped(serviceToRegister);
                            break;
                        case ServiceLifetime.Singleton:
                            serviceCollection.AddSingleton(serviceToRegister);
                            break;
                    }
                }
                else
                {
                    foreach (var implementedType in implementedTypes)
                    {
                        switch (serviceAttribute.Lifetime)
                        {
                            case ServiceLifetime.Transient:
                                serviceCollection.AddTransient(implementedType, serviceToRegister);
                                break;
                            case ServiceLifetime.Scoped:
                                serviceCollection.AddScoped(implementedType, serviceToRegister);
                                break;
                            case ServiceLifetime.Singleton:
                                serviceCollection.AddSingleton(implementedType, serviceToRegister);
                                break;
                        }
                    }    
                }
            }
        }
    }
}
