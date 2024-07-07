using Backend_DV_YTe.Repository;
using Backend_DV_YTe.Repository.Interface;

namespace Backend_DV_YTe.Service
{
    public class ExpiringLoThuocChecker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timerLoThuoc;
        private Timer _timerTB;

        public ExpiringLoThuocChecker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timerLoThuoc = new Timer(CheckExpiringLoThuoc, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            _timerTB = new Timer(CheckAndNotifyExpiringLoThietBiYTe, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void CheckExpiringLoThuoc(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var loThuocService = scope.ServiceProvider.GetRequiredService<ILoThuocRepository>();
                await loThuocService.CheckAndNotifyExpiringLoThuoc();
            }
        }

        private async void CheckAndNotifyExpiringLoThietBiYTe(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var loThietBiYTeService = scope.ServiceProvider.GetRequiredService<ILoThietBiYTeRepository>();
                await loThietBiYTeService.CheckAndNotifyExpiringLoThietBiYTe();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timerLoThuoc?.Change(Timeout.Infinite, 0);
            _timerTB?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }

}
