using System;
using System.Collections.Generic;
using System.Linq;
using metric.collector.pcl;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace metric.collector.test
{
    [TestFixture]
    public class MetricCollectionFixture
    {
        public MetricCollector Collector;
        public List<string> Metrics;

        [SetUp]
        public void Before()
        {
            Metrics = new List<string>();
            Collector = new MetricCollector("test", x => Metrics.Add(x));
        }

        [Test]
        public void RecordActionTime()
        {
            using (var timer = Collector.Timer("action"))
            {
                timer.Time(() => Task.Delay(100).Wait());
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
        public void RecordActionTimeWithMetadata()
        {
            var metadata = new Dictionary<string, string>()
            {
                { "one", "1" },
                { "two", "2" }
            };
            using (var timer = Collector.Timer("action"))
            {
                timer.Time(() => Task.Delay(100).Wait(), metadata);
            }

            Assert.IsTrue(Metrics.Any(x =>
            {
                var metric = JsonConvert.DeserializeObject<MetricWithMeta>(x);
                return metric.Key == "test.action" &&
                       metric.Units == "ms" &&
                       metric.Value > 100 &&
                       metric.One == "1" &&
                       metric.Two == "2";
            }));
        }

        [Test]
        public void RecordFuncTime()
        {
            string result;
            using (var timer = Collector.Timer("func"))
            {
                result = timer.Time(() =>
                {
                    Task.Delay(100).Wait();
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
        public void RecordFuncTimeWithMetadata()
        {
            var metadata = new Dictionary<string, string>()
            {
                { "one", "1" },
                { "two", "2" }
            };
            string result;
            using (var timer = Collector.Timer("func"))
            {
                result = timer.Time(() =>
                {
                    Task.Delay(100).Wait();
                    return "hiya";
                }, metadata);
            }

            Assert.IsTrue(Metrics.Any(x =>
            {
                var metric = JsonConvert.DeserializeObject<MetricWithMeta>(x);
                return metric.Key == "test.func" &&
                       metric.Units == "ms" &&
                       metric.Value > 100 &&
                       metric.One == "1" &&
                       metric.Two == "2";
            }));

            Assert.AreEqual(result, "hiya");
        }

        [Test]
        public void RecordTimeDiff()
        {
            var timer = Collector.Timer("timer");

            Task.Delay(100).Wait();
            timer.Record();
            Task.Delay(100).Wait();
            timer.Record();
            timer.Reset();
            Task.Delay(100).Wait();
            timer.Record();

            var times = Metrics
                .Select(JsonConvert.DeserializeObject<Metric>)
                .Select(x => Tuple.Create(x.Key, x.Value)).ToArray();
            Assert.AreEqual( times.Select(x => x.Item1).ToArray(), new [] {"test.timer", "test.timer", "test.timer"} );
            Assert.Greater(times[0].Item2, 100);
            Assert.Greater(times[1].Item2, 200);
            Assert.Greater(times[2].Item2, 100);
        }

        [Test]
        public void RecordTimesInParallel()
        {
            var timer = Collector.Timer("timer");

            Parallel.For(0, 4, x =>
            {
                timer.SetStart();
                Task.Delay(100).Wait();
                timer.Record();
            });

            var times = Metrics
                .Select(JsonConvert.DeserializeObject<Metric>)
                .Select(x => Tuple.Create(x.Key, x.Value)).ToArray();
            Assert.AreEqual(times.Select(x => x.Item1).ToArray(), new [] { "test.timer", "test.timer", "test.timer", "test.timer" });
            Assert.Greater(times[0].Item2, 100);
            Assert.Greater(times[1].Item2, 100);
            Assert.Greater(times[2].Item2, 100);
            Assert.Greater(times[3].Item2, 100);
        }

        [Test]
        public void RecordMeterValues()
        {
            using (var meter = Collector.Meter("meter"))
            {
                meter.Record(100);
                meter.Record(200);
                meter.Record(50);
            }

            var meters = Metrics
                .Select(JsonConvert.DeserializeObject<Metric>)
                .Select(x => Tuple.Create(x.Key, x.Value)).ToArray();

            Assert.AreEqual(meters.Select(x => x.Item1).ToArray(), new [] { "test.meter", "test.meter", "test.meter" });
            Assert.AreEqual(meters.Select(x => x.Item2).ToArray(), new double[] { 100, 200, 50 });
        }

        [Test]
        public void RecordMeterValuesWithMetadata()
        {
            var metadata = new Dictionary<string, string>()
            {
                { "one", "1" },
                { "two", "2" }
            };
            using (var meter = Collector.Meter("meter"))
            {
                meter.Record(100, metadata);
                meter.Record(200, metadata);
                meter.Record(50, metadata);
            }

            var meters = Metrics
                .Select(JsonConvert.DeserializeObject<MetricWithMeta>)
                .Select(x => Tuple.Create(x.Key, x.One, x.Two)).ToArray();

            Assert.AreEqual(meters.Select(x => x.Item1).ToArray(), new [] { "test.meter", "test.meter", "test.meter" });
            Assert.AreEqual(meters.Select(x => x.Item2).ToArray(), new [] { "1", "1", "1" });
            Assert.AreEqual(meters.Select(x => x.Item3).ToArray(), new [] { "2", "2", "2" });
        }

        [Test]
        public void RecordCustomValues()
        {
            using (var custom = Collector.Custom("mine","stuff","custom"))
            {
                custom.Record(100);
                custom.Record(200);
                custom.Record(50);
            }

            var meters = Metrics
                .Select(JsonConvert.DeserializeObject<Metric>)
                .Select(x => Tuple.Create(x.Key, x.Value, x.Type, x.Units)).ToArray();

            Assert.AreEqual(meters.Select(x => x.Item1).ToArray(), new [] { "test.custom", "test.custom", "test.custom" });
            Assert.AreEqual(meters.Select(x => x.Item2).ToArray(), new double[] { 100, 200, 50 });
            Assert.AreEqual(meters.Select(x => x.Item3).ToArray(), new [] { "mine", "mine", "mine" });
            Assert.AreEqual(meters.Select(x => x.Item4).ToArray(), new [] { "stuff", "stuff", "stuff" });
        }

        [Test]
        public void RecordCustomValuesWithMetadata()
        {
            var metadata = new Dictionary<string, string>()
            {
                { "one", "1" },
                { "two", "2" }
            };
            using (var custom = Collector.Custom("mine", "stuff", "custom"))
            {
                custom.Record(100, metadata);
                custom.Record(200, metadata);
                custom.Record(50, metadata);
            }

            var meters = Metrics
                .Select(JsonConvert.DeserializeObject<MetricWithMeta>)
                .Select(x => Tuple.Create(x.Key, x.Value, x.Type, x.Units, x.One, x.Two)).ToArray();

            Assert.AreEqual(meters.Select(x => x.Item1).ToArray(), new [] { "test.custom", "test.custom", "test.custom" });
            Assert.AreEqual(meters.Select(x => x.Item2).ToArray(), new double[] { 100, 200, 50 });
            Assert.AreEqual(meters.Select(x => x.Item3).ToArray(), new [] { "mine", "mine", "mine" });
            Assert.AreEqual(meters.Select(x => x.Item4).ToArray(), new [] { "stuff", "stuff", "stuff" });
            Assert.AreEqual(meters.Select(x => x.Item5).ToArray(), new [] { "1", "1", "1" });
            Assert.AreEqual(meters.Select(x => x.Item6).ToArray(), new [] { "2", "2", "2" });
        }
    }

    public struct MetricWithMeta
    {
        public string Key;
        public string Timestamp;
        public double Value;
        public string Units;
        public string Type;
        public string One;
        public string Two;

        public MetricWithMeta(string key, string timestamp, double value, string units, string type, string one, string two)
        {
            Key = key;
            Timestamp = timestamp;
            Value = value;
            Units = units;
            Type = type;
            One = one;
            Two = two;
        }
    }
}
