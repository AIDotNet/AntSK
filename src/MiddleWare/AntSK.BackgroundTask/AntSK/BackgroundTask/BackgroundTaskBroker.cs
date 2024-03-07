using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AntSK.BackgroundTask
{
	public class BackgroundTaskBroker<TItem> : IBackgroundTaskBroker
	{
		private class BackgroundTaskWorker<T>
		{
			private readonly BackgroundTaskBroker<T> _broker;

			private readonly IServiceProvider _serviceProvider;

			private readonly ILogger _logger;

			private readonly IBackgroundTaskHandler<T> _handler;

			public Task WorkerTask { get; private set; }

			public BackgroundTaskWorker(IServiceProvider serviceProvider, BackgroundTaskBroker<T> broker)
			{
				_broker = broker;
				_serviceProvider = serviceProvider;
				_logger = _serviceProvider.GetRequiredService<ILogger<BackgroundTaskWorker<T>>>();
				_handler = serviceProvider.GetRequiredService<IBackgroundTaskHandler<T>>();
			}

			public void Start(CancellationToken cancellationToken)
			{
				WorkerTask = Task.Factory.StartNew((Func<Task>)async delegate
				{
					while (!cancellationToken.IsCancellationRequested && !_broker.IsCompleted)
					{
						try
						{
							List<Task> tasks = new List<Task>();
							foreach (T item in _broker.TakeMany())
							{
								Task t2 = _handler.ExecuteAsync(item);
								Task cont = t2.ContinueWith(delegate(Task ct)
								{
									AggregateException exception2 = ct.Exception;
									if (exception2 != null)
									{
										_logger.LogError(exception2.ToString());
									}
								});
								tasks.Add(cont);
							}
							if (tasks.Any())
							{
								try
								{
									_logger.LogInformation($"等待所有 tasks {tasks.Count}");
									await Task.WhenAll(tasks).ConfigureAwait(continueOnCapturedContext: false);
									_logger.LogInformation("所有任务已完成");
								}
								catch (Exception ex3)
								{
									Exception ex2 = ex3;
									_logger.LogError(ex2.ToString());
								}
							}
						}
						catch (Exception ex)
						{
							_logger.LogError(ex.ToString());
						}
					}
					if (cancellationToken.IsCancellationRequested)
					{
						_logger.LogInformation("Cancellation was requested");
					}
					if (_broker.IsCompleted)
					{
						_logger.LogInformation("Broker 已完成");
					}
				}, cancellationToken, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
				WorkerTask.ContinueWith(delegate(Task t)
				{
					AggregateException exception = t.Exception;
					if (exception != null)
					{
						_logger.LogError("报错了: {0}", exception);
					}
				}, TaskContinuationOptions.OnlyOnFaulted);
			}
		}

		private readonly BlockingCollection<TItem> _data = new BlockingCollection<TItem>();

		private readonly List<BackgroundTaskWorker<TItem>> _workers = new List<BackgroundTaskWorker<TItem>>();

		private readonly object _lockWorkers = new object();

		private readonly IServiceProvider _serviceProvider;

		private readonly ILogger _logger;

		private bool IsCompleted => _data.IsCompleted;

		private bool IsRunning { get; set; }

		public BackgroundTaskBroker(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			_logger = _serviceProvider.GetRequiredService<ILogger<BackgroundTaskBroker<TItem>>>();
		}

		public void QueueWorkItem(TItem workItem)
		{
			_data.Add(workItem);
		}

		private IEnumerable<TItem> TakeMany()
		{
			return _data.GetConsumingEnumerable();
		}

		public void Start(int workerCount, CancellationToken cancellationToken)
		{
			if (workerCount <= 0)
			{
				throw new ArgumentException("workerCount 必须大于0");
			}
			IsRunning = true;
			lock (_lockWorkers)
			{
				while (_workers.Count < workerCount)
				{
					BackgroundTaskWorker<TItem> backgroundTaskWorker = new BackgroundTaskWorker<TItem>(_serviceProvider, this);
					_workers.Add(backgroundTaskWorker);
					backgroundTaskWorker.Start(cancellationToken);
				}
			}
		}

		public void Stop()
		{
			IsRunning = false;
			_data.CompleteAdding();
			lock (_lockWorkers)
			{
				Task[] tasks = _workers.Select((BackgroundTaskWorker<TItem> sw) => sw.WorkerTask).ToArray();
				_logger.LogInformation("开始停止所有tasks");
				Task.WaitAll(tasks);
				_logger.LogInformation("所有task已停止");
				_workers.Clear();
			}
		}
	}
}
