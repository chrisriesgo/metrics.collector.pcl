using System;

using Xamarin.Forms;
using metric.collector.sample.Pages;
using System.Linq;

namespace metric.collector.sample
{
	public class App : Application
	{
		Page page;
		bool resuming;
		public App()
		{
			page = new MainPage();
			MainPage = new NavigationPage(page);
		}

		protected override void OnStart()
		{
			// Handle unhandled Metrics
		}

		protected override async void OnResume()
		{
			if(resuming) return;
			resuming = true;

			var mainPage = page as MainPage;
			if(mainPage == null) return;

			var vm = mainPage.BindingContext as metric.collector.sample.ViewModels.MetricCollectorViewModel;
			if(vm == null) return;

			if(vm.Metrics.Any())
			{
				await mainPage.DisplayAlert("Sync", "Psssst -- You have unhandled metrics lurking about", "Ok");
			}
			resuming = false;
		}
	}
}