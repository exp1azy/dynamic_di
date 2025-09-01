namespace DynamicDI
{
    /// <summary>
    /// Attribute to mark a class as a service that should be registered in the DI container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class RegisterServiceAttribute : Attribute
    {
        /// <summary>
        /// Gets the service lifetime for registration.
        /// </summary>
        public ServiceLifeCycle LifeCycle { get; }

        /// <summary>
        /// Gets the strategy for interface registration.
        /// </summary>
        public InterfaceRegistrationStrategy InterfaceRegistrationStrategy { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterServiceAttribute"/> class.
        /// </summary>
        /// <param name="lifeTime">The service lifetime (default: Transient).</param>
        /// <param name="strategy">The interface registration strategy (default: FirstOnly).</param>
        public RegisterServiceAttribute(ServiceLifeCycle lifeTime = ServiceLifeCycle.Transient, InterfaceRegistrationStrategy strategy = InterfaceRegistrationStrategy.FirstOnly)
        {
            LifeCycle = lifeTime;
            InterfaceRegistrationStrategy = strategy;
        }
    }
}
