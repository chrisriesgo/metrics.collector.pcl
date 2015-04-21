using System;
using System.Collections.Generic;

namespace metric.collector.pcl
{
    public class Custom : IObservable<Metric>, IDisposable
    {
        public string TypeName { get; protected set; }
        public string KeyName { get; protected set; }
        public IObserver<Metric> Observer { get; protected set; }
        public string Units { get; protected set; }

        public void Record(double value)
        {
            Observer.OnNext(Metric.Custom(TypeName, Units, KeyName, value));
        }

        public void Record(double value, Dictionary<string, string> metadata)
        {
            Observer.OnNext( Metric.Custom(TypeName, Units, KeyName, value, metadata) );
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

        public Custom(string typeName, string units, string keyName)
        {
            TypeName = typeName;
            Units = units;
            KeyName = keyName;
        }
    }
}
