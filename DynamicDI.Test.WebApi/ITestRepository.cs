using DynamicDI.Test.WebApi.Data;

namespace DynamicDI.Test.WebApi
{
    public interface ITestRepository
    {
        public List<string> GetMessages();

        public Task<IEnumerable<CriticalSituationImage>> GetCsiAsync(CancellationToken cancellationToken = default);
    }
}
