namespace DynamicDI
{
    /// <summary>
    /// Attribute to mark a class as a DbContext that should be registered in the DI container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RegisterDbContextAttribute : Attribute
    {
    }
}