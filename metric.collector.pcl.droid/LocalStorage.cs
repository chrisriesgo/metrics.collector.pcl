using System;

namespace metric.collector.pcl.droid
{
	public class LocalStorage : ILocalStorage
	{
		public LocalStorage()
		{
		}

		public System.Threading.Tasks.Task StoreAsync()
		{
			Console.WriteLine("Droid Storage");
		}
	}
}

