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
        public ServiceLifeCycle LifeCycle { get; }

        /// <summary>
        /// Gets the specific types that this service should be registered as.
        /// </summary>
        public Type[]? ImplementedTypes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with transient lifetime and automatic implemented types registration.
        /// </summary>
        public ServiceAttribute()
        {
            LifeCycle = ServiceLifeCycle.Transient;
            ImplementedTypes = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with specified lifetime and automatic implemented types registration.
        /// </summary>
        /// <param name="lifeCycle">The service lifetime for the dependency injection container.</param>
        public ServiceAttribute(ServiceLifeCycle lifeCycle)
        {
            LifeCycle = lifeCycle;
            ImplementedTypes = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with transient lifetime and explicit implemented types registration.
        /// </summary>
        /// <param name="implementedTypes">The specific types to register this service as.</param>
        public ServiceAttribute(Type[] implementedTypes)
        {
            LifeCycle = ServiceLifeCycle.Transient;
            ImplementedTypes = implementedTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAttribute"/> class with specified lifetime and explicit implemented types registration.
        /// </summary>
        /// <param name="lifeTime">The service lifetime for the dependency injection container.</param>
        /// <param name="implementedTypes">The specific types to register this service as.</param>
        public ServiceAttribute(ServiceLifeCycle lifeTime, Type[]? implementedTypes)
        {
            LifeCycle = lifeTime;
            ImplementedTypes = implementedTypes;
        }
    }
}
