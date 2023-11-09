using Hangfire;
using MealApplication.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using MimeKit;
using src.Core.Domains;
using src.Core;

namespace MealApplication.Services
{
    public class BackgroundJobService : IHostedService, IDisposable
    {
        private Timer _timer;

        public Task StartAsync(CancellationToken stoppingToken)
        {
          
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            System.Diagnostics.Debug.WriteLine("HELLO"); 
        }

        public Task StopAsync(CancellationToken stoppingToken)
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
