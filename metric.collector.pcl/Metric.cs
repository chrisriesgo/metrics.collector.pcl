using System;
using System.Collections.Generic;
using System.Linq;

namespace metric.collector.pcl
{
    public struct Metric
    {
        public string Key;
        public string Timestamp;
        public double Value;
        public string Units;
        public string Type;
        public Dictionary<string, string> Metadata;

        public static Metric Duration(string key, double value)
        {
            return Duration(key, value, null);
        }

        public static Metric Duration(string key, double value, Dictionary<string, string> metadata )
        {
            return Custom("time", "ms", key, value, metadata);
        }

        public static Metric Meter(string key, double value)
        {
            return Meter(key, value, null);
        }

        public static Metric Meter(string key, double value, Dictionary<string, string> metadata)
        {
            return Custom("meter", null, key, value, metadata);
        }

        public static Metric Custom(string type, string units, string key, double value)
        {
            return Custom(type, units, key, value, null);
        }

        public static Metric Custom(string type, string units, string key, double value, Dictionary<string, string> metadata)
        {
            return new Metric()
            {
                Key = key,
                Metadata = metadata ?? new Dictionary<string, string>(),
                Type = type,
                Timestamp = DateTime.UtcNow.ToString("O"),
                Value = value,
                Units = units ?? "count"
            };
        }

        public override string ToString()
        {
            var pairs = Metadata.Aggregate( new List<string>(), (acc, x) =>
            {
                var pair = string.Format(@"""{0}"":""{1}""", x.Key, x.Value);
                acc.Add(pair);
                return acc;
            } );
            var metadata = string.Join(",", pairs);
            return string.Format(@"{{""type"":""{0}"",""key"":""{1}"",""value"":{2},""units"":""{3}"",""timestamp"":""{4}""{5}}}", Type, Key,
                Value, Units, Timestamp, metadata.Length > 0 ? "," + metadata : "" );
        }
    }
}
