using System;
using System.Collections.Generic;

namespace AntSK.Domain.BackgroundTask
{
	public class BackgroundTaskBrokerOptions
	{
		public Dictionary<Type, BackgroundTaskBrokerConfig> BrokerConfigs { get; set; }

		public BackgroundTaskBrokerOptions()
		{
			BrokerConfigs = new Dictionary<Type, BackgroundTaskBrokerConfig>();
		}
	}
}
