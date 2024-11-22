namespace SIFOYarpGateway
{
    public class RefreshService : IHostedService, IDisposable
    {
        private readonly DatabaseProxyConfigProvider _configProvider;
        private Timer _timer;
        private readonly IConfiguration _configuration;

        public RefreshService(DatabaseProxyConfigProvider configProvider, IConfiguration configuration)
        {
            _configProvider = configProvider;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var intervalMinutes = _configuration.GetValue<int>("ProxyConfig:RefreshIntervalMinutes");
            _timer = new Timer(RefreshConfig, null, TimeSpan.Zero, TimeSpan.FromMinutes(intervalMinutes));
            return Task.CompletedTask;
        }

        private void RefreshConfig(object state)
        {
            _configProvider.RefreshConfig();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
