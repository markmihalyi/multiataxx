using Backend.Data;

namespace Backend.Services
{
    public class ScopedExecutor(IServiceScopeFactory scopeFactory)
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public async Task<T> RunInScope<T>(Func<AppDbContext, Task<T>> action)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await action(dbContext);
        }

        public async Task RunInScope(Func<AppDbContext, Task> action)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await action(dbContext);
        }
    }
}