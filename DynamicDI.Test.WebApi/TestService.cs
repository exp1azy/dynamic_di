using DynamicDI.Test.WebApi.Data;

namespace DynamicDI.Test.WebApi
{
    [RegisterService(ServiceLifeCycle.Transient, InterfaceRegistrationStrategy.AllInterfaces)]
    public class TestService(ITestRepository repository) : ITestService, ITestable
    {
        private readonly ITestRepository _repository = repository;

        public async Task<IEnumerable<CriticalSituationImage>> GetCsiAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetCsiAsync(cancellationToken);
        }

        public string GetHelloMessage() => "Hello World!";
        public List<string> GetMessages() => _repository.GetMessages();
        public bool IsThisATest() => true;
    }
}
