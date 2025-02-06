namespace DIalect.Test.WebApi
{
    [RegisterService(ServiceLifeCycle.Singleton)]
    public class TestRepository : ITestRepository
    {
        public List<string> GetMessages()
        {
            return [ "Hello", "World", "!" ];
        }
    }
}
