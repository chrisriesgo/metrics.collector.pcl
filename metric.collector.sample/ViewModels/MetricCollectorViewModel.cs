using System;
using metric.collector.pcl;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Windows.Input;
using Xamarin.Forms;
using System.ComponentModel;
using System.Text;

namespace metric.collector.sample.ViewModels
{
	public class MetricCollectorViewModel : INotifyPropertyChanged
	{	
		private MetricCollector collector;
		private List<string> _metrics;
		private string _result;

		public MetricCollectorViewModel()
		{
			Metrics = new List<string>();
			Result = string.Empty;

			collector = new MetricCollector("test", x => Metrics.Add(x));
			RecordActionTimeCommand = new Command(RecordActionTime);
			RecordTimeDiffCommand = new Command(RecordTimeDiff);
			ResetCommand = new Command(Reset);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ICommand RecordActionTimeCommand { get; private set; }
		public ICommand RecordTimeDiffCommand { get; private set; }
		public ICommand ResetCommand { get; private set; }
		public List<string> Metrics
		{ 
			get { return _metrics; }
			private set
			{
				if(_metrics != value)
				{
					_metrics = value;
					OnPropertyChanged("Metrics");
				}
			}
		}

		public string Result 
		{ 
			get { return _result; } 
			private set
			{
				if(_result != value)
				{
					_result = value;
					OnPropertyChanged("Result");
				}
			}
		}

		private void RecordActionTime()
		{
			using (var timer = collector.Timer("action"))
			{
				timer.Time(() => System.Threading.Tasks.Task.Delay(100).Wait());
			}

			var result = Metrics.Any(x =>
			{
				var metric = JsonConvert.DeserializeObject<Metric>(x);
				return metric.Key == "test.action" &&
					metric.Units == "ms" &&
					metric.Value > 100;
			} );
		
			Result = string.Join(System.Environment.NewLine, Metrics);
		}

		private void RecordTimeDiff()
		{
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

			var result = new StringBuilder();
			foreach(var t in times)
			{
				result.AppendLine(string.Format("{0}: {1}", t.Item1, t.Item2));
			}
			Result = result.ToString();
		}

		private void Reset()
		{
			Metrics.Clear();
			OnPropertyChanged("Metrics");
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this,
					new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}

