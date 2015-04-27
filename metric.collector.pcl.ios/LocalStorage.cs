using System;

namespace metric.collector.pcl
{
	public class LocalStorage : ILocalStorage
	{
		public LocalStorage()
		{
		}

		public System.Threading.Tasks.Task StoreAsync()
		{
			Console.WriteLine("iOS Storage");
		}
	}
}

