namespace Widgets.Common
{
    public class Schedule
    {
        private readonly Dictionary<string, Timer> _timers = [];

        // Secondly method
        public string Secondly(Action action, int intervalInSeconds, DateTime? startTime = null)
        {
            return StartTimer(action, TimeSpan.FromSeconds(intervalInSeconds), startTime);
        }

        // Minutely method
        public string Minutely(Action action, int intervalInMinutes, DateTime? startTime = null)
        {
            return StartTimer(action, TimeSpan.FromMinutes(intervalInMinutes), startTime);
        }

        // Hourly method
        public string Hourly(Action action, int intervalInHours, DateTime? startTime = null)
        {
            return StartTimer(action, TimeSpan.FromHours(intervalInHours), startTime);
        }

        // Daily method
        public string Daily(Action action, int intervalInDays, DateTime? startTime = null)
        {
            return StartTimer(action, TimeSpan.FromDays(intervalInDays), startTime);
        }

        // Weekly method
        public string Weekly(Action action, int intervalInWeeks, DateTime? startTime = null)
        {
            return StartTimer(action, TimeSpan.FromDays(intervalInWeeks * 7), startTime);
        }

        // Monthly method (approximate by 30 days)
        public string Monthly(Action action, int intervalInMonths, DateTime? startTime = null)
        {
            return StartTimer(action, TimeSpan.FromDays(intervalInMonths * 30), startTime);
        }

        // Yearly method (approximate by 365 days)
        public string Yearly(Action action, int intervalInYears, DateTime? startTime = null)
        {
            return StartTimer(action, TimeSpan.FromDays(intervalInYears * 365), startTime);
        }

        // Generic timer start method
        private string StartTimer(Action action, TimeSpan interval, DateTime? startTime)
        {
            DateTime nextRunTime = CalculateNextRunTime(startTime ?? DateTime.UtcNow, interval);
            TimeSpan initialDelay = nextRunTime - DateTime.UtcNow;

            string timerId = Guid.NewGuid().ToString();
            Timer timer = new(_ => action(), null, initialDelay, interval);
            _timers[timerId] = timer;

            return timerId;
        }

        // Method to stop a timer by ID
        public void Stop(string timerId)
        {
            if (_timers.TryGetValue(timerId, out var timer))
            {
                timer.Dispose();
                _timers.Remove(timerId);
            }
        }

        // Helper method to calculate the next run time based on start time and interval
        private static DateTime CalculateNextRunTime(DateTime startTime, TimeSpan interval)
        {
            DateTime now = DateTime.UtcNow;

            while (startTime < now)
                startTime = startTime.Add(interval);

            return startTime;
        }
    }
}