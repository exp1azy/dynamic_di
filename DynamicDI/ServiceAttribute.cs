namespace DynamicDI
{
    /// <summary>
    /// Attribute to mark a class as a service that should be registered in the dependency injection container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceAttribute : Attribute
    {
        /// <summary>
        /// Gets the service lifetime for registration.
        /// </summary>
        public ServiceLifetimeType Lifetime { get; }

        /// <summary>
        /// Gets the specific types that this service should be registered as.
        /// </summary>
        public Type[]? ImplementedTypes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with transient lifetime and automatic implemented types registration.
        /// </summary>
        public ServiceAttribute()
        {
            Lifetime = ServiceLifetimeType.Transient;
            ImplementedTypes = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with specified lifetime and automatic implemented types registration.
        /// </summary>
        /// <param name="lifetime">The service lifetime for the dependency injection container.</param>
        public ServiceAttribute(ServiceLifetimeType lifetime)
        {
            Lifetime = lifetime;
            ImplementedTypes = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with transient lifetime and explicit implemented types registration.
        /// </summary>
        /// <param name="implementedTypes">The specific types to register this service as.</param>
        public ServiceAttribute(Type[] implementedTypes)
        {
            Lifetime = ServiceLifetimeType.Transient;
            ImplementedTypes = implementedTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with specified lifetime and explicit implemented types registration.
        /// </summary>
        /// <param name="lifeTime">The service lifetime for the dependency injection container.</param>
        /// <param name="implementedTypes">The specific types to register this service as.</param>
        public ServiceAttribute(ServiceLifetimeType lifeTime, Type[]? implementedTypes)
        {
            Lifetime = lifeTime;
            ImplementedTypes = implementedTypes;
        }
    }
}
