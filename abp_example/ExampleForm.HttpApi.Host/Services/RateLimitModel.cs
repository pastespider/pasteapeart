//using Microsoft.AspNetCore.RateLimiting;
//using System.Threading.RateLimiting;
//using Volo.Abp.Modularity;

//namespace ExampleForm.HttpApi.Host.Services
//{

//    /// <summary>
//    /// 
//    /// </summary>
//    public class PasteRateLimitOptions
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public FixedWindowOptions FixedWindow { get; set; }

//        /// <summary>
//        /// 
//        /// </summary>
//        public SlidingWindowOptions SlidingWindow { get; set; }

//        /// <summary>
//        /// 
//        /// </summary>
//        public TokenBucketOptions TokenBucket { get; set; }

//        /// <summary>
//        /// 
//        /// </summary>
//        public ConcurrencyOptions Concurrency { get; set; }
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class FixedWindowOptions
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public string PolicyName { get; set; } = "fixed";

//        /// <summary>
//        /// 
//        /// </summary>
//        public int PermitLimit { get; set; } = 10;

//        /// <summary>
//        /// 
//        /// </summary>
//        public int WindowInSeconds { get; set; } = 10;

//        /// <summary>
//        /// 
//        /// </summary>
//        public int QueueLimit { get; set; } = 5;
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class SlidingWindowOptions
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public string PolicyName { get; set; } = "sliding";

//        /// <summary>
//        /// 
//        /// </summary>
//        public int PermitLimit { get; set; } = 20;

//        /// <summary>
//        /// 
//        /// </summary>
//        public int WindowInSeconds { get; set; } = 30;

//        /// <summary>
//        /// 
//        /// </summary>
//        public int Segments { get; set; } = 3;

//        /// <summary>
//        /// 
//        /// </summary>
//        public int QueueLimit { get; set; } = 3;
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class TokenBucketOptions
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public string PolicyName { get; set; } = "token";
//        /// <summary>
//        /// 
//        /// </summary>
//        public int TokenLimit { get; set; } = 50;
//        /// <summary>
//        /// 
//        /// </summary>
//        public int TokensPerPeriod { get; set; } = 10;
//        /// <summary>
//        /// 
//        /// </summary>
//        public int ReplenishmentPeriodInSeconds { get; set; } = 1;
//        /// <summary>
//        /// 
//        /// </summary>
//        public int QueueLimit { get; set; } = 5;
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class ConcurrencyOptions
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public string PolicyName { get; set; } = "concurrency";
//        /// <summary>
//        /// 
//        /// </summary>
//        public int PermitLimit { get; set; } = 5;
//        /// <summary>
//        /// 
//        /// </summary>
//        public int QueueLimit { get; set; } = 2;
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public static class PasteRateLimiter
//    {


//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="builder"></param>
//        /// <param name="SectionMap"></param>
//        public static void UsePasteRateLimiter(this ServiceConfigurationContext builder, string SectionMap = "RateLimitOptions")
//        {

//            var rateConfig = builder.Services.GetConfiguration().GetSection(SectionMap).Get<PasteRateLimitOptions>();

//            // 绑定配置到模型
//            //builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection("RateLimitOptions"));

//            var configuration = builder.Services.GetConfiguration();

//            // 配置速率限制服务
//            builder.Services.AddRateLimiter(options =>
//            {
//                //if (rateConfig?.FixedWindow != null)
//                //{
//                //    var config = rateConfig.FixedWindow;
//                //    options.AddFixedWindowLimiter(config.PolicyName, option =>
//                //    {
//                //        option.PermitLimit = config.PermitLimit;
//                //        option.QueueLimit = config.QueueLimit;
//                //        option.AutoReplenishment = true;
//                //        option.Window = TimeSpan.FromSeconds(config.WindowInSeconds);
//                //    });
//                //}

//                if (rateConfig.SlidingWindow != null)
//                {
//                    var config = rateConfig.SlidingWindow;
//                    options.AddSlidingWindowLimiter(config.PolicyName, option =>
//                    {
//                        option.QueueLimit = config.QueueLimit;
//                        option.AutoReplenishment = true;
//                        option.PermitLimit = config.PermitLimit;
//                        option.SegmentsPerWindow = config.Segments;
//                        option.Window = TimeSpan.FromSeconds(config.WindowInSeconds);
//                    });
//                }

//                //if (rateConfig.TokenBucket != null)
//                //{
//                //    var config = rateConfig.TokenBucket;
//                //    options.AddTokenBucketLimiter(config.PolicyName, option =>
//                //    {
//                //        option.QueueLimit = config.QueueLimit;
//                //        option.AutoReplenishment = true;
//                //        option.TokenLimit = config.TokenLimit;
//                //        option.TokensPerPeriod = config.TokensPerPeriod;
//                //        option.ReplenishmentPeriod = TimeSpan.FromSeconds(config.ReplenishmentPeriodInSeconds);
//                //    });
//                //}

//                //if (rateConfig.Concurrency != null)
//                //{
//                //    var config = rateConfig.Concurrency;
//                //    options.AddConcurrencyLimiter(config.PolicyName, option =>
//                //    {
//                //        option.QueueLimit = config.QueueLimit;
//                //        option.PermitLimit = config.PermitLimit;

//                //    });
//                //}

//                // 自定义拒绝响应
//                options.OnRejected = static (context, cancellationToken) =>
//                {
//                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
//                    context.HttpContext.Response.WriteAsync("请求过于频繁，请稍后再试。", cancellationToken);
//                    return ValueTask.CompletedTask;
//                };

//                options.GlobalLimiter = PartitionedRateLimiter.Create<Microsoft.AspNetCore.Http.HttpContext, string>(httpContext =>
//                {

//                    // 获取客户端IP（处理代理情况）
//                    //var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

//                    //// 如果有代理，从Header中获取真实IP
//                    //if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedIp))
//                    //{
//                    //    ipAddress = forwardedIp.FirstOrDefault();
//                    //}

//                    var ipAddress = "";

//                    return RateLimitPartition.GetSlidingWindowLimiter(
//                        partitionKey: ipAddress ?? "unknown-ip",
//                        factory: partition => new SlidingWindowRateLimiterOptions
//                        {
//                            PermitLimit = 10,
//                            Window = TimeSpan.FromSeconds(30),
//                            SegmentsPerWindow = 3
//                        });

//                });

//            });
//        }

//    }

//    /*
//     *
//     "RateLimitOptions": {
//    "FixedWindow": {
//      "PolicyName": "fixed-policy",
//      "PermitLimit": 15,
//      "WindowInSeconds": 10,
//      "QueueLimit": 3
//    },
//    "SlidingWindow": {
//      "PolicyName": "sliding-policy",
//      "PermitLimit": 30,
//      "WindowInSeconds": 60,
//      "Segments": 6,
//      "QueueLimit": 2
//    },
//    "TokenBucket": {
//      "PolicyName": "token-policy",
//      "TokenLimit": 100,
//      "TokensPerPeriod": 20,
//      "ReplenishmentPeriodInSeconds": 5,
//      "QueueLimit": 4
//    },
//    "Concurrency": {
//      "PolicyName": "concurrency-policy",
//      "PermitLimit": 8,
//      "QueueLimit": 3
//    }
//  }
//     *
//     */

//}
