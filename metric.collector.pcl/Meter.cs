using System;

namespace metric.collector.pcl
{
    public class Meter : IObservable<Metric>, IDisposable
    {
        public string KeyName { get; protected set; }
        public IObserver<Metric> Observer { get; protected set; }

        public void Record(double value)
        {
            Observer.OnNext( Metric.Meter( KeyName, value ));
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
        }

        public Meter(string keyName)
        {
            KeyName = keyName;
        }
    }
}