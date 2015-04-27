using System;
using System.Threading.Tasks;

namespace metric.collector.pcl
{
	public class LocalStorage : ILocalStorage
	{
		public LocalStorage()
		{
		}

		public async System.Threading.Tasks.Task StoreAsync()
		{
			Console.WriteLine("iOS Storage");
			await Task.Delay(1);
		}
	}
}

