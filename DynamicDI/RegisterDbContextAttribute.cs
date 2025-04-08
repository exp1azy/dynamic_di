namespace DynamicDI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RegisterDbContextAttribute : Attribute
    {
    }
}
