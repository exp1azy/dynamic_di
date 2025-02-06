
namespace DIalect.Test.WebApi
{
    [RegisterService(ServiceLifeCycle.Transient, InterfaceRegistrationStrategy.AllInterfaces)]
    public class TestService(ITestRepository repository) : ITestService, ITestable
    {
        private readonly ITestRepository _repository = repository;

        public string GetHelloMessage() => "Hello World!";
        public List<string> GetMessages() => _repository.GetMessages();
        public bool IsThisATest() => true;
    }
}
