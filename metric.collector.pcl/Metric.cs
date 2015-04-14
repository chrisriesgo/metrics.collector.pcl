using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace metric.collector.pcl
{
    public struct Metric
    {
        public string Key;
        public string Timestamp;
        public double Value;
        public string Units;

        public static Metric Duration(string key, double value)
        {
            return new Metric()
            {
                Key = key,
                Timestamp = DateTime.UtcNow.ToString("O"),
                Value = value,
                Units = "ms"
            };
        }

        public static Metric Meter(string key, double value)
        {
            return new Metric()
            {
                Key = key,
                Timestamp = DateTime.UtcNow.ToString("O"),
                Value = value,
                Units = ""
            };
        }

        public override string ToString()
        {
            return string.Format(@"{{""key"":""{0}"",""value"":{1},""units"":""{2}"",""timestamp"":""{3}""}}", Key,
                Value, Units, Timestamp);
        }
    }
}
