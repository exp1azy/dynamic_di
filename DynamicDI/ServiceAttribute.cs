namespace DynamicDI
{
    /// <summary>
    /// Marks a class for automatic registration in an <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceAttribute : Attribute
    {
        /// <summary>
        /// Gets the lifetime used when registering the service.
        /// </summary>
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// Gets the service types that the decorated class should be registered as.
        /// </summary>
        public Type[]? ImplementedTypes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with the <see cref="ServiceLifetime.Transient"/> lifetime.
        /// </summary>
        /// <remarks>
        /// Service types are inferred from the interfaces implemented by the decorated class.
        /// </remarks>
        public ServiceAttribute()
        {
            Lifetime = ServiceLifetime.Transient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with the specified lifetime.
        /// </summary>
        /// <param name="lifetime">The lifetime to use when registering the service.</param>
        /// <remarks>
        /// Service types are inferred from the interfaces implemented by the decorated class.
        /// </remarks>
        public ServiceAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with the <see cref="ServiceLifetime.Transient"/> lifetime and explicit service types.
        /// </summary>
        /// <param name="implementedTypes">The service types to register the decorated class as.</param>
        public ServiceAttribute(params Type[] implementedTypes)
        {
            Lifetime = ServiceLifetime.Transient;
            ImplementedTypes = implementedTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with the specified lifetime and explicit service types.
        /// </summary>
        /// <param name="lifeTime">The lifetime to use when registering the service.</param>
        /// <param name="implementedTypes">The service types to register the decorated class as.</param>
        public ServiceAttribute(ServiceLifetime lifeTime, params Type[] implementedTypes)
        {
            Lifetime = lifeTime;
            ImplementedTypes = implementedTypes;
        }
    }
}
