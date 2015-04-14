using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using metric.collector.pcl;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace metric.collector.test
{
    [TestFixture]
    public class MetricCollectionFixture
    {
        public MetricCollector collector;
        public List<string> Metrics;

        [SetUp]
        public void Before()
        {
            Metrics = new List<string>();
            collector = new MetricCollector("test", x => Metrics.Add(x));
        }

        [Test]
        public void RecordActionTime()
        {
            using (var timer = collector.Timer("action"))
            {
                timer.Time(() => System.Threading.Tasks.Task.Delay(100).Wait());
            }

            Assert.IsTrue( Metrics.Any(x =>
            {
                var metric = JsonConvert.DeserializeObject<Metric>(x);
                return metric.Key == "test.action" &&
                       metric.Units == "ms" &&
                       metric.Value > 100;
            } ) );
        }

        [Test]
        public void RecordFuncTime()
        {
            string result;
            using (var timer = collector.Timer("func"))
            {
                result = timer.Time(() =>
                {
                    System.Threading.Tasks.Task.Delay(100).Wait();
                    return "hiya";
                });
            }

            Assert.IsTrue(Metrics.Any(x =>
            {
                var metric = JsonConvert.DeserializeObject<Metric>(x);
                return metric.Key == "test.func" &&
                       metric.Units == "ms" &&
                       metric.Value > 100;
            }));

            Assert.AreEqual(result,"hiya");
        }

        [Test]
        public void RecordTimeDiff()
        {
            string result;
            var timer = collector.Timer("timer");

            System.Threading.Tasks.Task.Delay(100).Wait();
            timer.Record();
            System.Threading.Tasks.Task.Delay(100).Wait();
            timer.Record();
            timer.Reset();
            System.Threading.Tasks.Task.Delay(100).Wait();
            timer.Record();

            var times = Metrics
                .Select(JsonConvert.DeserializeObject<Metric>)
                .Select(x => Tuple.Create(x.Key, x.Value)).ToArray();
            Assert.AreEqual( times.Select(x => x.Item1).ToArray(), new string [] {"test.timer", "test.timer", "test.timer"} );
            Assert.Greater(times[0].Item2, 100);
            Assert.Greater(times[1].Item2, 200);
            Assert.Greater(times[2].Item2, 100);
        }

        [Test]
        public void RecordTimesInParallel()
        {
            string result;
            var timer = collector.Timer("timer");

            Parallel.For(0, 4, x =>
            {
                timer.SetStart();
                System.Threading.Tasks.Task.Delay(100).Wait();
                timer.Record();
            });

            var times = Metrics
                .Select(JsonConvert.DeserializeObject<Metric>)
                .Select(x => Tuple.Create(x.Key, x.Value)).ToArray();
            Assert.AreEqual(times.Select(x => x.Item1).ToArray(), new string[] { "test.timer", "test.timer", "test.timer", "test.timer" });
            Assert.Greater(times[0].Item2, 100);
            Assert.Greater(times[1].Item2, 100);
            Assert.Greater(times[1].Item2, 100);
            Assert.Greater(times[2].Item2, 100);
        }

        [Test]
        public void RecordMeterValues()
        {
            using (var meter = collector.Meter("meter"))
            {
                meter.Record(100);
                meter.Record(200);
                meter.Record(50);
            }

            var meters = Metrics
                .Select(JsonConvert.DeserializeObject<Metric>)
                .Select(x => Tuple.Create(x.Key, x.Value)).ToArray();

            Assert.AreEqual(meters.Select(x => x.Item1).ToArray(), new string[] { "test.meter", "test.meter", "test.meter" });
            Assert.AreEqual(meters.Select(x => x.Item2).ToArray(), new double[] { 100, 200, 50 });
        }
    }
}
