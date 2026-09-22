using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ExampleForm.Handler;
using Serilog;

//using Serilog.Sinks.RabbitMQ;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {

        /// <summary>
        /// 
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Main(string[] args)
        {
            try
            {
                Directory.SetCurrentDirectory(AppContext.BaseDirectory);

                Configuration = ExampleFormEntityFrameworkCoreModule.BuildConfiguration(args);
                var builder = CreateHostBuilder(args).Build();
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(Configuration)
                    .WriteTo.Sink(new SerilogCustomHandler(builder.Services), Serilog.Events.LogEventLevel.Error)
                    .Enrich.FromLogContext()
                    .CreateLogger();

                if (!System.IO.Directory.Exists("data")) { System.IO.Directory.CreateDirectory("data"); }

                var _model = Environment.GetEnvironmentVariable("RunConfig:Model");

                //if (!String.IsNullOrEmpty(_model))
                //{
                    Log.Information("Starting web host.");
                    builder.Run();
                //}
                //else
                //{
                //    Log.Information("Starting as Windows Service");
                //    //CreateHostServiceBuilder(args).Build().Run();
                //}
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(Configuration);
                    webBuilder.UseStartup<Startup>();
                })
            .UseSerilog()
            .UseAutofac();

        //	  <PackageReference Include="Microsoft.Extensions.Hosting.WindowsServices" Version="6.0.3" />
        //internal static Microsoft.Extensions.Hosting.IHostBuilder CreateHostServiceBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //    .UseWindowsService(s => { s.ServiceName = "ExampleForm"; })
        //    .ConfigureWebHostDefaults(webBuilder =>
        //    {
        //        //webBuilder.UseUrls("http://*:51666");//由外部指定 --urls=http://*:51666
        //        webBuilder.UseConfiguration(Configuration);
        //        webBuilder.UseStartup<Startup>();
        //    })
        //    .UseContentRoot(AppContext.BaseDirectory)
        //    .UseSerilog()
        //    .UseAutofac();
    }
}
