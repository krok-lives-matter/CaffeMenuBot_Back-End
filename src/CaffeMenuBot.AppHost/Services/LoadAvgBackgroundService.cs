using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CaffeMenuBot.AppHost.Services
{
    public class LoadAvgBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private Queue _loadAvg5MinInterval = new Queue();
        private Timer? _timer;

        public LoadAvgBackgroundService(ILogger<LoadAvgBackgroundService> logger)
        {
            _logger = logger;
        }

        public Queue? GetLoadAvg()
        {
            if(_loadAvg5MinInterval.Count < 5)
                return null;
            return _loadAvg5MinInterval;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LoadAvg Timed Background Service is starting.");

            _timer = new Timer(GetLoadAvg, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            await base.StopAsync(cancellationToken);
        }

        private void GetLoadAvg(object? state) 
        {
            _logger.LogInformation("Timed LoadAvg Background Service is working.");

            // run shell command and split by space char
            string[] totalInfo = "cat /proc/loadavg".Bash().Split();

            // get loadavg 1 min and map to 0-100 range
            double oneMin = double.Parse(totalInfo[0]) * 100;

            if(_loadAvg5MinInterval.Count == 5)
                _loadAvg5MinInterval.Dequeue();

            _loadAvg5MinInterval.Enqueue(oneMin);
        }

        public override void Dispose()
        {  
            _timer?.Dispose();
            base.Dispose();
        }
    }
}