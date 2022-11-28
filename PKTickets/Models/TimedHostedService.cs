using PKTickets.Models.DTO;

namespace PKTickets.Models
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> _logger;
        private Timer? _timer = null;
        private readonly PKTicketsDbContext db;

        public TimedHostedService(ILogger<TimedHostedService> logger, PKTicketsDbContext _db)
        {
            _logger = logger;
            this.db = _db;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);
            var schedules = db.Schedules.Where(x => x.IsActive == true).ToList();
            DateTime date = DateTime.Now;
            var schedulesDate = schedules.Where(x => x.Date <= date.Date).ToList();
            TimeSpan time = new TimeSpan(date.Hour, date.Minute, 0);
            var timeValue = TimingConvert.ConvertToInt(Convert.ToString(time));
            foreach (Schedule schedule in schedulesDate)
            {
                var times = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == schedule.ShowTimeId);
                if (times.ShowTiming < timeValue)
                {
                    schedule.IsActive = false;
                    db.SaveChanges();
                }
            }
            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {Count}", count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
   
