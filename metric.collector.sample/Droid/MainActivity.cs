using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace metric.collector.sample.Droid
{
	[Activity(Label = "Metric Collector", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication(new App());

			if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
			{
				ActionBar.SetIcon(new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.Transparent));
			}
		}
	}
}

