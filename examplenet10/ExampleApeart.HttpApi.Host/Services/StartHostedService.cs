using ExampleApeart.EntityFrameworkCore;
using ExampleApeart.Handler;
using ExampleApeart.usermodels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler;
using PasteApeart.Handler.Extensions;
using PasteApeart.Handler.rabbit;

namespace ExampleApeart
{
    /// <summary>
    /// 系统初始化
    /// </summary>
    public class StartHostedService : IHostedService, IAsyncDisposable
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StartHostedService> _logger;
        private readonly IChannelHelper _channelHelper;
        private readonly IEventActionHandler _eventHandler;
        private readonly AppConfig _config;
        private readonly IRabbitMultiConsumerHandler _rabbitReviceHandler;
        private readonly RabbitConfig _rabbitConfig;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="rabbitConfig"></param>
        /// <param name="rabbitMultiConsumerHandler"></param>
        /// <param name="logger"></param>
        /// <param name="channelHelper"></param>
        /// <param name="eventHandler"></param>
        /// <param name="config"></param>
        public StartHostedService(IServiceProvider serviceProvider, IOptions<RabbitConfig> rabbitConfig, IRabbitMultiConsumerHandler rabbitMultiConsumerHandler, ILogger<StartHostedService> logger, IChannelHelper channelHelper, IEventActionHandler eventHandler, IOptions<AppConfig> config)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _channelHelper = channelHelper;
            _eventHandler = eventHandler;
            _config = config.Value;
            _rabbitConfig = rabbitConfig.Value;
            _rabbitReviceHandler = rabbitMultiConsumerHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_rabbitConfig.SubscribeEnable && _config.RabbitQueue)
            {
                var appclient = "default";
                if (Environment.GetEnvironmentVariable("APPID") != null)
                {
                    appclient = Environment.GetEnvironmentVariable("APPID");
                }
                //这个RabbitConfig到底是不是单例... .. . 
                _rabbitConfig.SubscribeQueue = $"Ud_Subscribe_{appclient}";
                Console.WriteLine($"配置了广播模式，设置的当前队列名称为:{_rabbitConfig.SubscribeQueue}");
                //下发将进行 消费者初始化
            }

            await InitAuditDataUpdateAsync();

            await InstanceSystemDataUpdateAsync();

            if (!_config.RabbitQueue)
            {
                _ = ReahChannelEventModelAsync(cts.Token);
            }
            else
            {
                //启动rabbitmq的消费者
                _ = _rabbitReviceHandler.InitAsync();
            }
            //Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} StartHostedService.StartAsync!");
        }

        /// <summary>
        /// 初始化系统
        /// </summary>
        private async Task InstanceSystemDataUpdateAsync()
        {
            //await Task.Delay(3000);

            try
            {

                using var _scope = _serviceProvider.CreateScope();
                using var _dbContext = _scope.ServiceProvider.GetRequiredService<IExampleApeartDbContext>();

                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }

                var configValue = _scope.ServiceProvider.GetRequiredService<IOptions<AppConfig>>();

                var config = configValue.Value;

                //创建基础角色 下面最好用事务来做
                var adminid = 0;
                var admin = await _dbContext.UserInfo.Where(x => x.Email == config.RootAccount).OrderByDescending(x => x.Id).AsNoTracking().FirstOrDefaultAsync();
                if (admin == null || admin == default)
                {

                    try
                    {
                        await _dbContext.Database.BeginTransactionAsync();
                        var one = new UserInfo();
                        one.Email = config.RootAccount;
                        one.UserName = "管理员";
                        one.CreateDate = DateTime.Now;
                        //one.Grade = "admin";
                        one.IsEnable = true;
                        one.PassWord = $"{config.RootPassword}".ToMd5Lower();
                        //one.GradeId = 1;
                        _dbContext.Add(one);
                        await _dbContext.SaveChangesAsync();
                        adminid = one.Id;

                        //var _handler = _scope.ServiceProvider.GetRequiredService<UserHandler>();

                        //创建组
                        var item = new GradeInfo
                        {
                            Name = "超管",
                            Code = "root",
                            Desc = "管理组",
                            IsEnable = true
                        };
                        _dbContext.Add(item);
                        await _dbContext.SaveChangesAsync();
                        var groupid = item.Id;
                        _dbContext.Add(new UserGrade() { GradeId = groupid, UserId = adminid });
                        await _dbContext.SaveChangesAsync();

                        await UserHandler.LoadAsync(_dbContext, false);
                        await _dbContext.Database.CommitTransactionAsync();
                    }
                    catch (Exception exlinit)
                    {
                        await _dbContext.Database.RollbackTransactionAsync();
                        _logger.LogException(exlinit);
                        throw;
                    }
                }
                else
                {
                    adminid = admin.Id;
                }
                //_dbContext.Dispose();
                //_scope.Dispose();
            }
            catch (Exception exl)
            {
                _logger.LogError(exl.ToString());
                throw;
            }
        }

        /// <summary>
        /// 初始化审计日志的数据库等
        /// </summary>
        /// <returns></returns>
        private async Task InitAuditDataUpdateAsync()
        {
            try
            {
                using var sc = _serviceProvider.CreateScope();
                using var _dbContext = sc.ServiceProvider.GetRequiredService<MyAuditingDb>();
                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }

            }
            catch (Exception exl)
            {
                _logger.LogException(exl);
            }
        }

        /// <summary>
        /// 处理Channel中的EventModel
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task ReahChannelEventModelAsync(CancellationToken token = default)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var read = await _channelHelper.ChannelEventModel.Reader.ReadAsync(token);
                    if (read != null)
                    {
                        await _eventHandler.ActionMessageAsync(read);
                    }
                    else
                    {
                        await Task.Delay(500);
                    }
                }
            }
            catch (Exception exl)
            {
                _logger.LogException(exl);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            await cts.CancelAsync();

            if (_rabbitReviceHandler != null)
            {
                await _rabbitReviceHandler.DisposeAsync();
            }
            await Task.Delay(500);
        }
    }
}
