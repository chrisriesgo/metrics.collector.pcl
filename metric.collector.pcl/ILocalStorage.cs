using System;

namespace metric.collector.pcl
{
	public interface ILocalStorage
	{
		System.Threading.Tasks.Task StoreAsync();
	}
}

