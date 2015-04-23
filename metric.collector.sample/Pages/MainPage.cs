using Xamarin.Forms;
using metric.collector.sample.ViewModels;

namespace metric.collector.sample.Pages
{
	public class MainPage : ContentPage
	{
		MetricCollectorViewModel viewModel;
		public MainPage()
		{
			Title = "Metric Collector";

			viewModel = new MetricCollectorViewModel();
			BindingContext = viewModel;

			var actionTime = new Button { Text = "Record Action Time", HorizontalOptions = LayoutOptions.Center };
			actionTime.SetBinding<MetricCollectorViewModel>(Button.CommandProperty, p => p.RecordActionTimeCommand);

			var timeDiff = new Button { Text = "Record Time Diff", HorizontalOptions = LayoutOptions.Center };
			timeDiff.SetBinding<MetricCollectorViewModel>(Button.CommandProperty, p => p.RecordTimeDiffCommand);

			var resetButton = new Button { Text = "Reset", HorizontalOptions = LayoutOptions.Center };
			resetButton.SetBinding<MetricCollectorViewModel>(Button.CommandProperty, p => p.ResetCommand);

			var layout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children = { actionTime, timeDiff, resetButton }
			};

			Content = layout;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			viewModel.PropertyChanged += MetricCollectorPropertyChanged;
		}

		private void MetricCollectorPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "Result")
			{
				ShowResult(viewModel.Result);
			}
			if(e.PropertyName == "Metrics")
			{
				ShowResult("Metrics have been reset");
			}
		}

		private async void ShowResult(string result)
		{
			await DisplayAlert("Result", result, "Close");
		}
	}
}

