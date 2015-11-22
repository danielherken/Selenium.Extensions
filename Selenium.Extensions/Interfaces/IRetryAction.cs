using System;
using System.Diagnostics;

namespace Selenium.Extensions.Interfaces
{
    public interface IRetryAction
    {
        void DoUntil(Action perform, Func<bool> until, TimeSpan? timeout = null);
        void DontDoUntil(Action perform, Func<bool> whenFulfilled, TimeSpan? timeout = null);

        IDo Do(Action action);
    }

    public interface IDo
    {
        IDoFor ForNoLongerThan(TimeSpan fromSeconds);
    }

    public interface IDoFor
    {
        void Until(Func<bool> until);
    }

    public class RetryTimerFactory
        : IRetryTimerFactory
    {
        public IRetryTimer Create(TimeSpan timeoutLimit)
        {
            return new RetryTimer(timeoutLimit);
        }
    }

    public interface IRetryTimerFactory
    {
        IRetryTimer Create(TimeSpan timeoutLimit);
    }

    public class RetryTimer
        : IRetryTimer
    {
        private readonly Stopwatch _stopwatch;
        private readonly TimeSpan _timeoutLimit;

        public RetryTimer(TimeSpan timeoutLimit)
        {
            _timeoutLimit = timeoutLimit;
            _stopwatch = Stopwatch.StartNew();
        }

        public bool TimedOut()
        {
            return _stopwatch.Elapsed > _timeoutLimit;
        }
    }

    public interface IRetryTimer
    {
        bool TimedOut();
    }
}