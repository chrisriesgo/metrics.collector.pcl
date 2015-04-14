using System;
using System.Threading;

namespace metric.collector.pcl
{
    public class Timer : IObservable<Metric>, IDisposable
    {
        public string KeyName { get; protected set; }
        public ThreadLocal<DateTime> Start { get; protected set; }
        public IObserver<Metric> Observer { get; protected set; }

        public double Record()
        {
            var now = DateTime.UtcNow;
            var diff = now.Subtract(Start.Value).TotalMilliseconds;
            Observer.OnNext( Metric.Duration(KeyName,diff));
            return diff;
        }

        public void Time(Action action)
        {
            var start = DateTime.UtcNow;
            action();
            var diff = DateTime.UtcNow.Subtract(start).TotalMilliseconds;
            Observer.OnNext(Metric.Duration(KeyName, diff));
        }

        public TResult Time<TResult>(Func<TResult> function)
        {
            var start = DateTime.UtcNow;
            var result = function();
            var diff = DateTime.UtcNow.Subtract(start).TotalMilliseconds;
            Observer.OnNext(Metric.Duration(KeyName, diff));
            return result;
        }

        public void SetStart()
        {
            if (Start == null)
            {
                Start = new ThreadLocal<DateTime>(() => DateTime.UtcNow);
            }
            Start.Value = DateTime.UtcNow;
        }

        public void Reset()
        {
            SetStart();
        }

        public IDisposable Subscribe(IObserver<Metric> observer)
        {
            Observer = observer;
            return this;
        }

        public void Dispose()
        {
            if (Observer != null)
            {
                Observer = null;
            }
            if (Start != null)
            {
                Start.Dispose();
                Start = null;
            }
        }

        public Timer(string keyName )
        {
            KeyName = keyName;
            SetStart();
        }
    }
}