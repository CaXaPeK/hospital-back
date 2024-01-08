using Hospital.Database;

namespace Hospital.Middlewares
{
    public class BannedTokenAutodeleteMiddleware : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;

        public BannedTokenAutodeleteMiddleware(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DeleteExpiredRecords, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void DeleteExpiredRecords(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var currentTime = DateTime.UtcNow;
                var recordsToDelete = dbContext.BannedTokens
                    .Where(x => (currentTime - x.AddedAt).TotalMinutes >= 90)
                    .ToList();

                dbContext.BannedTokens.RemoveRange(recordsToDelete);
                dbContext.SaveChanges();
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
