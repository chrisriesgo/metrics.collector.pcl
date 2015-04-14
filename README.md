# metrics.collector.pcl
A simple lib for collecting durations and meters for deferred shipping to an HTTP endpoint.

## Metric Data
The data structure of the metric captured is:

```js
{
    "key": the base namespace + the metric key name seperated by a `.`
    "value": the duration (for timers) or value (for meters)
    "units": "ms" for durations, "" for meters
    "timestamp": ISO8601 timestamp
}
```

## API

### MetricCollector(string baseNamespace, Action&lt;string&gt; onMetric)
An observer for all created timers and meters that invokes the onMetric callback with a JSON representation of each metric as it is recorded.

#### Meter(string keyName)
Creates a new meter with the given key name for tracking values over time.

#### Timer(string keyName)
Creates a new timer with the given key name for tracking durations over time or for capturing the duration of an Action or a Func&lt;T&gt;.

### Meter
Used to record values over time. Each call to `Record` creates a new metric.

#### void Record(double value)
Records the value specified.

```csharp
var metrics = new List<string>();
var collector = new MetricCollector("example", metrics.Add);
using (var meter = collector.Meter("meter"))
{
    meter.Record(1);
}
// metrics now contains a JSON string that looks like:
// { "key": "example.meter", "value": 1, "units": "", "timestamp": "2015-04-14T02:28:15.0000000Z" }
```

### Timer
Timer provides two ways to capture durations. The simplest way is via the `Time` call which will record the wallclock time taken to execute an anonymous function.

Timer also provides a thread-safe means of recording the elapsed time since the timer was set/reset.

> Note: The `SetStart` call should be made at the entry point of each thread. Refer to MetricCollectorFixture for example.

#### void Time(Action action)
Captures and records the time taken to perform the action.

#### T Time(Func&lt;T&gt; function)
Captures and records the time taken to perform the function. Returns the result of executing the function.

#### void SetStart()
Resets the start time of the timer for which all subsequent `Record` calls will be based off of.

#### double Record()
Captures the time elapsed since the last time the timer's start time was set.

#### void Reset()
Resets the start time of the timer instance to now.

```csharp
var metrics = new List<string>();
var collector = new MetricCollector("example", metrics.Add);
using (var timer = collector.Timer("timer"))
{
    timer.Time(() =>
    {
        // do stuff
    });

    var x = timer.Time(() =>
    {
        return "ohhai";
    });

    timer.Record();
}
// metrics now contains three JSON strings that look like:
// { "key": "example.timer", "value": 10, "units": "ms", "timestamp": "2015-04-14T02:28:15.0000000Z" }
// { "key": "example.timer", "value": 1, "units": "ms", "timestamp": "2015-04-14T02:28:15.0000000Z" }
// { "key": "example.timer", "value": 1, "units": "ms", "timestamp": "2015-04-14T02:28:15.0000000Z" }
```
