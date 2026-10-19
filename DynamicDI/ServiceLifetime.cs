namespace DynamicDI
{
    /// <summary>
    /// Specifies the lifetime of a service registered using <see cref="ServiceAttribute"/>.
    /// </summary>
    /// <remarks>
    /// The values correspond to the lifetimes supported by <see cref="Microsoft.Extensions.DependencyInjection.ServiceLifetime"/>.
    /// </remarks>
    public enum ServiceLifetime
    {
        /// <summary>
        /// A new instance is created each time the service is requested.
        /// </summary>
        Transient,

        /// <summary>
        /// A single instance is created per scope.
        /// </summary>
        Scoped,

        /// <summary>
        /// A single instance is created for the lifetime of the application.
        /// </summary>
        Singleton
    }
}
