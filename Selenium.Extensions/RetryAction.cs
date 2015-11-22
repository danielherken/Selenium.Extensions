using System;
using Selenium.Extensions.Interfaces;

namespace Selenium.Extensions
{
    public class RetryAction: IRetryAction, IDo, IDoFor
    {
        private readonly IRetryTimerFactory _retryTimerFactory;
        private Action _action;

        private TimeSpan _timeoutLimit;
        private Func<bool> _until;

        public RetryAction(IRetryTimerFactory retryTimerFactory)
        {
            _retryTimerFactory = retryTimerFactory;
            _timeoutLimit = TimeSpan.FromSeconds(5);
            _until = () => true;
            _action = () => { throw new InvalidOperationException("No state changing action defined."); };
        }

        IDoFor IDo.ForNoLongerThan(TimeSpan timeoutLimit)
        {
            _timeoutLimit = timeoutLimit;
            return this;
        }

        void IDoFor.Until(Func<bool> until)
        {
            _until = until;
            ((IRetryAction) this).DoUntil(_action, _until, _timeoutLimit);
        }

        void IRetryAction.DoUntil(Action action, Func<bool> condition, TimeSpan? timeout)
        {
            _timeoutLimit = timeout ?? _timeoutLimit;

            var timer = _retryTimerFactory.Create(_timeoutLimit);

            while (!condition() && !timer.TimedOut())
            {
                action();
            }
        }

        void IRetryAction.DontDoUntil(Action perform, Func<bool> whenFulfilled, TimeSpan? timeout)
        {
            _timeoutLimit = timeout ?? _timeoutLimit;

            var timer = _retryTimerFactory.Create(_timeoutLimit);
            bool fulfilled;
            while (!(fulfilled = whenFulfilled()) && !timer.TimedOut())
            {
            }

            if (fulfilled)
            {
                perform();
            }
        }

        IDo IRetryAction.Do(Action action)
        {
            _action = action;
            return this;
        }
    }
}