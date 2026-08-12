using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace PasteApeart
{
    /// <summary>
    /// 
    /// </summary>
    public class RabbitMQProducer : ISingletonDependency, IRabbitMQProducer
    {

        // synchronization locks
        private static int MaxChannelCount = 4;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim[] _modelLocks;// = new SemaphoreSlim[MaxChannelCount];
        private readonly CancellationTokenSource _closeTokenSource = new CancellationTokenSource();
        private readonly CancellationToken _closeToken;
        private int _currentModelIndex = -1;

        // configuration member
        private readonly RabbitConfig _config;
        //private readonly PublicationAddress _publicationAddress;

        // endpoint members
        private readonly IConnectionFactory _connectionFactory;
        private readonly IChannel[] _models;// = new IModel[MaxChannelCount];
        //private readonly IChannel[] _channels;
        private readonly BasicProperties[] _properties;// = new IBasicProperties[MaxChannelCount];
        private volatile IConnection _connection;

        //private readonly Random _random;

        private readonly ILogger<RabbitMQProducer> _logger;

        /// <summary>
        /// Constructor for RabbitMqClient
        /// </summary>
        /// <param name="config">mandatory</param>
        public RabbitMQProducer(IOptions<RabbitConfig> config, ILogger<RabbitMQProducer> logger)
        {
            _logger = logger;
            _config = config.Value;
            _closeToken = _closeTokenSource.Token;

            MaxChannelCount = _config.MaxChannelCount;
            _modelLocks = new SemaphoreSlim[MaxChannelCount];
            _models = new IChannel[MaxChannelCount];
            //_channels = new IChannel[MaxChannelCount];
            _properties = new BasicProperties[MaxChannelCount];
            // RabbitMQ channels are not thread-safe.
            // https://www.rabbitmq.com/dotnet-api-guide.html#model-sharing
            // Create a pool of channels and give each call to Publish one channel.
            for (var i = 0; i < MaxChannelCount; i++)
            {
                _modelLocks[i] = new SemaphoreSlim(1);
            }

            // load configuration

            //_publicationAddress = new PublicationAddress(_config.ExpireExchangeType, _config.ExpiredExchange, _config.ExpireRouteKey);

            // initialize
            _connectionFactory = GetConnectionFactory();

            //_random = new Random();
        }

        /// <summary>
        /// Configures a new ConnectionFactory, and returns it
        /// </summary>
        /// <returns></returns>
        private IConnectionFactory GetConnectionFactory()
        {
            // prepare connection factory
            var connectionFactory = new ConnectionFactory
            {
                UserName = _config.UserName,
                Password = _config.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(2),
                //UseBackgroundThreadsForIO = _config.UseBackgroundThreadsForIO,
                ClientProvidedName = _config.ClientName,
                HostName = _config.HostName
            };

            //if (_config.SslOption != null)
            //{
            //    connectionFactory.Ssl.Version = _config.SslOption.Version;
            //    connectionFactory.Ssl.CertPath = _config.SslOption.CertPath;
            //    connectionFactory.Ssl.ServerName = _config.SslOption.ServerName;
            //    connectionFactory.Ssl.Enabled = _config.SslOption.Enabled;
            //    connectionFactory.Ssl.AcceptablePolicyErrors = _config.SslOption.AcceptablePolicyErrors;
            //}
            // setup heartbeat if needed
            //if (_config.Heartbeat > 0)
            //{
            //    connectionFactory.RequestedHeartbeat = TimeSpan.FromMilliseconds(_config.Heartbeat);
            //}
            // only set, if has value, otherwise leave default
            if (_config.Port > 0) connectionFactory.Port = _config.Port;
            if (!string.IsNullOrEmpty(_config.VirtualHost)) connectionFactory.VirtualHost = _config.VirtualHost;

            // return factory
            return connectionFactory;
        }

        ///// <summary>
        ///// 推送消息
        ///// </summary>
        ///// <param name="bytes"></param>
        ///// <returns></returns>
        //public async Task PublishAsync(byte[] bytes)
        //{
        //    var currentModelIndex = Interlocked.Increment(ref _currentModelIndex);
        //    // https://stackoverflow.com/a/14997413/263003
        //    currentModelIndex = (currentModelIndex % MaxChannelCount + MaxChannelCount) % MaxChannelCount;
        //    var modelLock = _modelLocks[currentModelIndex];
        //    await modelLock.WaitAsync(_closeToken);
        //    try
        //    {
        //        var model = _models[currentModelIndex];
        //        var properties = _properties[currentModelIndex];
        //        if (model == null)
        //        {
        //            var connection = await GetConnectionAsync();
        //            model = await connection.CreateChannelAsync();
        //            _models[currentModelIndex] = model;

        //            properties = new BasicProperties();
        //            properties.DeliveryMode = (DeliveryModes)2; // persistence
        //            _properties[currentModelIndex] = properties;
        //        }
        //        await model.BasicPublishAsync(_publicationAddress, properties, bytes);
        //    }
        //    finally
        //    {
        //        modelLock.Release();
        //    }
        //}


        ///// <summary>
        ///// 推送消息
        ///// </summary>
        ///// <param name="message"></param>
        ///// <returns></returns>
        //public async Task PublishAsync(string message)
        //{
        //    var currentModelIndex = Interlocked.Increment(ref _currentModelIndex);

        //    //Console.WriteLine($"mq push message:{message}");
        //    // Interlocked.Increment can overflow and return a negative currentModelIndex.
        //    // Ensure that currentModelIndex is always in the range of [0, MaxChannelCount) by using this formula.
        //    // https://stackoverflow.com/a/14997413/263003
        //    currentModelIndex = (currentModelIndex % MaxChannelCount + MaxChannelCount) % MaxChannelCount;
        //    var modelLock = _modelLocks[currentModelIndex];
        //    await modelLock.WaitAsync(_closeToken);
        //    try
        //    {
        //        var model = _models[currentModelIndex];
        //        var properties = _properties[currentModelIndex];
        //        if (model == null)
        //        {
        //            var connection = await GetConnectionAsync();
        //            model = await connection.CreateChannelAsync();
        //            _models[currentModelIndex] = model;
        //            //model.
        //            properties = new BasicProperties();
        //            properties.DeliveryMode = (DeliveryModes)_config.DeliveryMode; // persistence
        //            _properties[currentModelIndex] = properties;
        //        }
        //        var bytes = System.Text.Encoding.UTF8.GetBytes(message);
        //        // push message to exchange
        //        await model.BasicPublishAsync(_publicationAddress, properties, bytes);

        //    }
        //    finally
        //    {
        //        modelLock.Release();
        //    }
        //}


        ///// <summary>
        ///// 推送消息
        ///// </summary>
        ///// <param name="exchange"></param>
        ///// <param name="message"></param>
        ///// <param name="routingkey"></param>
        ///// <returns></returns>
        //public async Task SendAsync(string exchange, string message, string routingkey = "")
        //{

        //    var currentModelIndex = Interlocked.Increment(ref _currentModelIndex);

        //    //Console.WriteLine($"mq push message:{message}");
        //    // Interlocked.Increment can overflow and return a negative currentModelIndex.
        //    // Ensure that currentModelIndex is always in the range of [0, MaxChannelCount) by using this formula.
        //    // https://stackoverflow.com/a/14997413/263003
        //    currentModelIndex = (currentModelIndex % MaxChannelCount + MaxChannelCount) % MaxChannelCount;
        //    var modelLock = _modelLocks[currentModelIndex];
        //    await modelLock.WaitAsync(_closeToken);
        //    try
        //    {
        //        var model = _models[currentModelIndex];
        //        var properties = _properties[currentModelIndex];
        //        if (model == null)
        //        {
        //            var connection = await GetConnectionAsync();
        //            model = await connection.CreateChannelAsync();
        //            _models[currentModelIndex] = model;

        //            //properties = model.CreateBasicProperties();
        //            properties = new BasicProperties();
        //            //properties.Expiration = "3000";//设置过期时间
        //            //if (expiration != 0)
        //            //{
        //            //    properties.Expiration = $"{expiration}";
        //            //}

        //            //properties.Priority = priority;

        //            //properties.MessageId
        //            properties.DeliveryMode = (DeliveryModes)2; // persistence
        //            _properties[currentModelIndex] = properties;
        //        }

        //        // push message to exchange
        //        await model.BasicPublishAsync(exchange, routingkey, false, properties, System.Text.Encoding.UTF8.GetBytes(message));
        //    }
        //    finally
        //    {
        //        modelLock.Release();
        //    }

        //}


        ///// <summary>
        ///// 推送消息
        ///// </summary>
        ///// <param name="exchange"></param>
        ///// <param name="message"></param>
        ///// <param name="routingkey"></param>
        ///// <param name="expiration">过期时间，0为不设置，单位毫秒</param>
        ///// <param name="priority">优先级，默认为0，越大优先级越高</param>
        ///// <returns></returns>
        //public async Task SendExpireMessageAsync(string exchange, string message, string routingkey = "", long expiration = 0, byte priority = 0)
        //{

        //    var currentModelIndex = Interlocked.Increment(ref _currentModelIndex);

        //    //Console.WriteLine($"mq push message:{message}");
        //    // Interlocked.Increment can overflow and return a negative currentModelIndex.
        //    // Ensure that currentModelIndex is always in the range of [0, MaxChannelCount) by using this formula.
        //    // https://stackoverflow.com/a/14997413/263003
        //    currentModelIndex = (currentModelIndex % MaxChannelCount + MaxChannelCount) % MaxChannelCount;
        //    var modelLock = _modelLocks[currentModelIndex];
        //    await modelLock.WaitAsync(_closeToken);
        //    try
        //    {
        //        var model = _models[currentModelIndex];

        //        //var properties = _properties[currentModelIndex];
        //        if (model == null)
        //        {
        //            var connection = await GetConnectionAsync();
        //            model = await connection.CreateChannelAsync();
        //            _models[currentModelIndex] = model;
        //        }

        //        //var properties = model.CreateBasicProperties();
        //        var properties = new BasicProperties();
        //        //properties.Expiration = "3000";//设置过期时间
        //        if (expiration != 0)
        //        {
        //            properties.Expiration = $"{expiration}";
        //        }
        //        properties.Priority = priority;
        //        //properties.MessageId
        //        properties.DeliveryMode = (DeliveryModes)2; // persistence

        //        // push message to exchange
        //        await model.BasicPublishAsync(exchange, routingkey, false, properties, System.Text.Encoding.UTF8.GetBytes(message));


        //    }
        //    finally
        //    {
        //        modelLock.Release();
        //    }

        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="message"></param>
        /// <param name="routingkey"></param>
        /// <param name="maxRetries"></param>
        /// <returns></returns>
        public async Task<bool> SendWithRetryAsync(string exchange, string message, string routingkey, int maxRetries = 3)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    await SendInternalAsync(exchange, message, routingkey);
                    return true;
                }
                catch (BrokerUnreachableException ex)
                {
                    _logger.LogWarning(ex, "[RabbitMQ] 代理不可达，尝试 {Attempt}/{MaxRetries}", attempt, maxRetries);
                    if (attempt == maxRetries) return false;
                }
                catch (Exception ex) when (IsTransientException(ex))
                {
                    _logger.LogWarning(ex, "[RabbitMQ]  transient error，尝试 {Attempt}/{MaxRetries}", attempt, maxRetries);
                    if (attempt == maxRetries) return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[RabbitMQ]  permanent error，停止重试");
                    return false;
                }
                // 指数退避
                await Task.Delay(CalculateBackoff(attempt));
            }

            return false;
        }

        /// <summary>
        /// 写入订阅模式的消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task WriteSubscribeAsync(string message)
        {
            try
            {
                await SendInternalAsync(_config.SubscribeExchange, message, "");
            }
            catch (Exception exl)
            {
                _logger.LogException(exl);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="message"></param>
        /// <param name="routingkey"></param>
        /// <returns></returns>
        private async Task SendInternalAsync(string exchange, string message, string routingkey)
        {
            var currentModelIndex = Interlocked.Increment(ref _currentModelIndex);
            currentModelIndex = (currentModelIndex % MaxChannelCount + MaxChannelCount) % MaxChannelCount;
            var modelLock = _modelLocks[currentModelIndex];
            await modelLock.WaitAsync(_closeToken);
            try
            {
                var model = _models[currentModelIndex];
                if (model == null || !model.IsOpen)
                {
                    var connection = await GetConnectionAsync();
                    model = await connection.CreateChannelAsync();
                    _models[currentModelIndex] = model;
                    _properties[currentModelIndex] = null;
                }
                BasicProperties properties1;
                //if (expiration == 0)
                //{
                properties1 = _properties[currentModelIndex];
                if (properties1 == null)
                {
                    properties1 = new BasicProperties();
                    properties1.DeliveryMode = DeliveryModes.Persistent;
                    properties1.Persistent = true;
                    _properties[currentModelIndex] = properties1;
                }
                //}
                //else
                //{
                //    properties1 = new BasicProperties();
                //    properties1.DeliveryMode = DeliveryModes.Persistent;
                //    properties1.Persistent = true;
                //    properties1.Expiration = expiration.ToString();
                //}
                var bytes = Encoding.UTF8.GetBytes(message);
                await model.BasicPublishAsync(exchange, routingkey, false, properties1, System.Text.Encoding.UTF8.GetBytes(message));
            }
            finally
            {
                modelLock.Release();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool IsTransientException(Exception ex)
        {
            // 判断是否为可重试的异常
            return ex is BrokerUnreachableException ||
                   ex is TimeoutException ||
                   ex is OperationCanceledException ||
                   (ex.InnerException != null && IsTransientException(ex.InnerException));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attempt"></param>
        /// <returns></returns>
        private TimeSpan CalculateBackoff(int attempt)
        {
            // 指数退避：500ms, 1s, 2s, 4s...
            return TimeSpan.FromMilliseconds(500 * Math.Pow(2, attempt - 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="AggregateException"></exception>
        public void Close()
        {
            IList<Exception> exceptions = new List<Exception>();
            try
            {
                _closeTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            // Disposing channel and connection objects is not enough, they must be explicitly closed with the API methods.
            // https://www.rabbitmq.com/dotnet-api-guide.html#disconnecting
            for (var i = 0; i < _models.Length; i++)
            {
                try
                {
                    _modelLocks[i].Wait(10);
                    //_models[i]?.Close();
                    _models[i].Dispose();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            try
            {
                _connectionLock.Wait(10);
                _connectionLock.Dispose();
                //_connection?.Close();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _closeTokenSource.Dispose();

            _connectionLock.Dispose();
            foreach (var modelLock in _modelLocks)
            {
                modelLock.Dispose();
            }

            foreach (var model in _models)
            {
                model?.Dispose();
            }

            _connection?.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<IConnection> GetConnectionAsync()
        {
            if (_connection == null)
            {
                await _connectionLock.WaitAsync(_closeToken);
                try
                {
                    if (_connection == null)
                    {
                        _connection = await _connectionFactory.CreateConnectionAsync();
                        //_connection = _connectionFactory.CreateConnection();
                    }
                }
                finally
                {
                    _connectionLock.Release();
                }
            }
            return _connection;
        }

    }
}
