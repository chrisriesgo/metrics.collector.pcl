using System;

namespace metric.collector.pcl
{
    public class MetricCollector : IObserver<Metric>
    {
        public string BaseNamespace { get; set; }
        public Action<string> OnMetric { get; set; }

        public Timer Timer(string measurement)
        {
            var timer = new Timer(BaseNamespace + "." + measurement);
            timer.Subscribe(this);
            return timer;
        }

        public Meter Meter(string measurement)
        {
            var meter = new Meter(BaseNamespace + "." + measurement);
            meter.Subscribe(this);
            return meter;
        }

        public Custom Custom(string type, string units, string measurement)
        {
            var custom = new Custom(type, units, BaseNamespace + "." + measurement);
            custom.Subscribe(this);
            return custom;
        }

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public void OnNext(Metric value)
        {
            if (OnMetric != null)
            {
                OnMetric(value.ToString());
            }
        }

        public MetricCollector(string baseNamespace, Action<string> onMetric)
        {
            BaseNamespace = baseNamespace;
            OnMetric = onMetric;
        }
    }
}
