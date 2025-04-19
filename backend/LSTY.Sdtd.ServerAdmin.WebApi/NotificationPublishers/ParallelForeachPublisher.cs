using MediatR;

namespace LSTY.Sdtd.ServerAdmin.WebApi.NotificationPublishers
{
    /// <summary>
    /// 
    /// </summary>
    public class ParallelForeachPublisher : INotificationPublisher
    {
        private readonly ILogger<ParallelForeachPublisher> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public ParallelForeachPublisher(ILogger<ParallelForeachPublisher> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerExecutors"></param>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
        {
            await Parallel.ForEachAsync(handlerExecutors, cancellationToken, async (handler, token) =>
            {
                try
                {
                    await handler.HandlerCallback(notification, token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while executing notification handler {HandlerType}", handler.HandlerInstance.GetType());
                }
            });
        }
    }
}
