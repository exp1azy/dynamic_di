using DynamicDI.Test.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace DynamicDI.Test.WebApi
{
    [RegisterService]
    public class TestRepository : ITestRepository
    {
        private readonly DataContext _dbContext;

        public TestRepository(DataContext dataContext)
        {
            _dbContext = dataContext;
        }

        public async Task<IEnumerable<CriticalSituationImage>> GetCsiAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.CriticalSituationImages.AsNoTracking().ToListAsync(cancellationToken);
        }

        public List<string> GetMessages()
        {
            return [ "Hello", "World", "!" ];
        }
    }
}
