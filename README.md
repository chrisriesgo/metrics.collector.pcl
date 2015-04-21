# metrics.collector.pcl
A simple lib for collecting durations and meters for deferred shipping to an HTTP endpoint.

## Metric Data
The data structure of the metric captured is:

```js
{
    "type": the type name of the metric being collected
    "key": the base namespace + the metric key name seperated by a `.`
    "value": the duration (for timers) or value (for meters)
    "units": "ms" for durations, "" for meters
    "timestamp": ISO8601 timestamp
}
```

> Note: Metadata properties will be added directly to the metric data when supplied.

## API

### MetricCollector(string baseNamespace, Action&lt;string&gt; onMetric)
An observer for all created metrics that invokes the onMetric callback with a JSON representation of each metric as it is recorded.

#### Meter(string keyName)
Creates a new meter with the given key name for tracking values over time.

#### Timer(string keyName)
Creates a new timer with the given key name for tracking durations over time or for capturing the duration of an Action or a Func&lt;T&gt;.

#### Custom(string type, string units, string keyName)
Creates a metric with a custom type and units.

### Meter
Used to record values over time. Each call to `Record` creates a new metric.

#### void Record(double value)
Records the value specified.

#### void Record(double value, IDictionary&lt;string, string&gt; metadata)
Records the value specified and includes metadata about the measurement.

```csharp
var metrics = new List<string>();
var collector = new MetricCollector("example", metrics.Add);
using (var meter = collector.Meter("meter"))
{
    meter.Record(1);

    var metadata = Dictionary<string, string> n = new Dictionary<string, string>() {
        { "user", "userName" }, { "account", "accountId" }
    };
    meter.Record(1, metadata);
}
// metrics now contains two JSON strings that look like:
// { "type": "time", "key": "example.meter", "value": 1, "units": "", "timestamp": "2015-04-14T02:28:15.0000000Z" }
// { "type": "time", "key": "example.meter", "value": 1, "units": "", "timestamp": "2015-04-14T02:28:15.0000000Z, "user": "userId", "account": accountId"" }
```

### Timer
Timer provides two ways to capture durations. The simplest way is via the `Time` call which will record the wallclock time taken to execute an anonymous function.

Timer also provides a thread-safe means of recording the elapsed time since the timer was set/reset.

> Note: The `SetStart` call should be made at the entry point of each thread. Refer to MetricCollectorFixture for example.

#### void Time(Action action)
Captures and records the time taken to perform the action.

#### void Time(Action action, IDictionary&lt;string, string&gt; metadata)
Captures and records the time taken to perform the action with included metadata added to the metric.

#### T Time(Func&lt;T&gt; function)
Captures and records the time taken to perform the function. Returns the result of executing the function.

#### T Time(Func&lt;T&gt; function, IDictionary&lt;string, string&gt; metadata)
Captures and records the time taken to perform the function with included metadata added to the metric. Returns the result of executing the function.

#### void SetStart()
Resets the start time of the timer for which all subsequent `Record` calls will be based off of.

#### double Record()
Captures the time elapsed since the last time the timer's start time was set.

#### double Record(IDictionary&lt;string, string&gt; metadata)
Captures the time elapsed with included metadata since the last time the timer's start time was set.

#### void Reset()
Resets the start time of the timer instance to now.

```csharp
var metrics = new List<string>();
var collector = new MetricCollector("example", metrics.Add);
var metadata = Dictionary<string, string> n = new Dictionary<string, string>() {
    { "user", "userName" }, { "account", "accountId" }
};

using (var timer = collector.Timer("timer"))
{
    timer.Time(() =>
    {
        // do stuff
    });

    timer.Time(() =>
    {
        // do stuff
    }, metadata);

    var x = timer.Time(() =>
    {
        return "ohhai";
    });

    var x = timer.Time(() =>
    {
        return "ohhai";
    }, metadata);

    timer.Record();

    timer.Record(metadata)
}
// metrics now contains six JSON strings that look like:
// { "type": "meter", "key": "example.timer", "value": 10, "units": "ms", "timestamp": "2015-04-14T02:28:15.0000000Z" }
// { "type": "meter", "key": "example.timer", "value": 10, "units": "ms", "timestamp": "2015-04-14T02:28:15.0000000Z", "user": "userId", "account": accountId" }
// { "type": "meter", "key": "example.timer", "value": 1, "units": "ms", "timestamp": "2015-04-14T02:28:15.0000000Z" }
// { "type": "meter", "key": "example.timer", "value": 1, "units": "ms", "timestamp": "2015-04-14T02:28:15.0000000Z", "user": "userId", "account": accountId" }
// { "type": "meter", "key": "example.timer", "value": 1, "units": "ms", "timestamp": "2015-04-14T02:28:15.0000000Z" }
// { "type": "meter", "key": "example.timer", "value": 1, "units": "ms", "timestamp": "2015-04-14T02:28:15.0000000Z", "user": "userId", "account": accountId" }
```

### Custom
Used to record values over time where you specify the `type` and `units`.

#### void Record(double value)
Records the value specified.

#### void Record(double value, IDictionary&lt;string, string&gt; metadata)
Records the value specified and includes metadata about the measurement.

```csharp
var metrics = new List<string>();
var collector = new MetricCollector("example", metrics.Add);
using (var custom = collector.Custom("custom", "thingies", "custom"))
{
    custom.Record(1);

    var metadata = Dictionary<string, string> n = new Dictionary<string, string>() {
        { "user", "userName" }, { "account", "accountId" }
    };
    custom.Record(1, metadata);
}
// metrics now contains two JSON strings that look like:
// { "type": "custom", "key": "example.custom", "value": 1, "units": "thingies", "timestamp": "2015-04-14T02:28:15.0000000Z" }
// { "type": "custom", "key": "example.custom", "value": 1, "units": "thingies", "timestamp": "2015-04-14T02:28:15.0000000Z, "user": "userId", "account": accountId"" }
```

## Uploading Metrics
The MetricLoader publishes metrics from a stream to a URL specified in the constructor.

```csharp
// where textStream is any IO stream of newline delimited JSON metrics
var loader = new MetricLoader("http://localhost:8898/api/metrics");
var result = loader.Upload(textStream);
```
