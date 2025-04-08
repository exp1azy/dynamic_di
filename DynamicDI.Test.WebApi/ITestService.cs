using DynamicDI.Test.WebApi.Data;

namespace DynamicDI.Test.WebApi
{
    public interface ITestService
    {
        public string GetHelloMessage();
        public List<string> GetMessages();
        public Task<IEnumerable<CriticalSituationImage>> GetCsiAsync(CancellationToken cancellationToken = default);
    }
}
