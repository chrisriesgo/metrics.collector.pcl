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
			throw new NotImplementedException();
			await Task.Delay(1);
		}
	}
}

