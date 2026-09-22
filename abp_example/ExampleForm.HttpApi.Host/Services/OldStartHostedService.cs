//using ExampleForm.Handler;
//using ExampleForm.usermodels;
//using Microsoft.EntityFrameworkCore;
//using Z.EntityFramework.Plus;

//namespace ExampleForm
//{
//    /// <summary>
//    /// 系统初始化
//    /// </summary>
//    public class OldStartHostedService : IHostedService
//    {

//        private readonly IServiceProvider _serviceProvider;
//        private readonly ILogger<OldStartHostedService> _logger;



//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="serviceProvider"></param>
//        /// <param name="logger"></param>
//        public OldStartHostedService(IServiceProvider serviceProvider, ILogger<OldStartHostedService> logger)
//        {
//            _serviceProvider = serviceProvider;
//            _logger = logger;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        /// <exception cref="System.NotImplementedException"></exception>
//        public async Task StartAsync(CancellationToken cancellationToken)
//        {

//            //foreach(string item in System.Environment.GetEnvironmentVariables().Keys)
//            //{
//            //    _logger.LogWarning($"{item} === {Environment.GetEnvironmentVariable(item)}");
//            //}

//            await SystemInitAsync();


//            //return Task.CompletedTask;
//            //throw new System.NotImplementedException();
//        }

//        private void FuncTask(GradeInfo abc)
//        {
//            Task.Run(async void () =>
//            {
//                Console.WriteLine(abc.Name);
//                if (abc.Id != 0)
//                {
//                    await Task.Delay(1);
//                }
//            });
//        }

//        /// <summary>
//        /// 初始化系统
//        /// </summary>
//        private async Task SystemInitAsync()
//        {
//            //await Task.Delay(3000);

//            try
//            {
//                using var _scope = _serviceProvider.CreateScope();
//                using var _dbContext = _scope.ServiceProvider.GetRequiredService<IExampleFormDbContext>();

//                if (_dbContext.Database.GetPendingMigrations().Any())
//                {
//                    _dbContext.Database.Migrate();
//                }

//                //创建基础角色 下面最好用事务来做
//                var adminid = 0;
//                var admin = await _dbContext.UserInfo.Where(x => x.Email == "admin@admin.com").AsNoTracking().FirstOrDefaultAsync();
//                if (admin == null || admin == default)
//                {

//                    try
//                    {
//                        await _dbContext.Database.BeginTransactionAsync();
//                        var one = new UserInfo();
//                        one.Email = "admin@admin.com";
//                        one.UserName = "管理员";
//                        one.CreateDate = DateTime.Now;
//                        //one.Grade = "admin";
//                        one.IsEnable = true;
//                        one.PassWord = $"123456".ToMd5Lower();
//                        //one.GradeId = 1;
//                        _dbContext.Add(one);
//                        await _dbContext.SaveChangesAsync();
//                        adminid = one.Id;

//                        var _handler = _scope.ServiceProvider.GetRequiredService<RoleBackLoadHandler>();

//                        //创建组
//                        var item = new GradeInfo
//                        {
//                            Name = "超管",
//                            Code = "root",
//                            Desc = "管理组",
//                            IsEnable = true
//                        };
//                        _dbContext.Add(item);
//                        await _dbContext.SaveChangesAsync();
//                        var groupid = item.Id;
//                        _dbContext.Add(new UserGrade() { GradeId = groupid, UserId = adminid });
//                        await _dbContext.SaveChangesAsync();

//                        await _handler.LoadAsync(_dbContext, false);
//                        await _dbContext.Database.CommitTransactionAsync();
//                    }
//                    catch (Exception exlinit)
//                    {
//                        await _dbContext.Database.RollbackTransactionAsync();
//                        _logger.LogException(exlinit);
//                        throw;
//                    }
//                }
//                else
//                {
//                    adminid = admin.Id;
//                }
//                //_dbContext.Dispose();
//                //_scope.Dispose();
//            }
//            catch (Exception exl)
//            {
//                _logger.LogException(exl);
//                throw;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        /// <exception cref="System.NotImplementedException"></exception>
//        public Task StopAsync(CancellationToken cancellationToken)
//        {
//            return Task.CompletedTask;
//            //throw new System.NotImplementedException();
//        }
//    }
//}
