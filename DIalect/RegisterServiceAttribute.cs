namespace DIalect
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class RegisterServiceAttribute : Attribute
    {
        public ServiceLifeCycle LifeCycle { get; }

        public InterfaceRegistrationStrategy InterfaceRegistrationStrategy { get; }

        public RegisterServiceAttribute(ServiceLifeCycle lifeTime = ServiceLifeCycle.Transient, InterfaceRegistrationStrategy strategy = InterfaceRegistrationStrategy.FirstOnly)
        {
            LifeCycle = lifeTime;
            InterfaceRegistrationStrategy = strategy;
        }
    }
}
