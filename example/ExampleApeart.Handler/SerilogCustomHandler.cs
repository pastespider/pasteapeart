using Microsoft.Extensions.DependencyInjection;
using PasteApeart;
using Serilog.Core;
using Serilog.Events;

namespace ExampleApeart.Handler
{
    public class SerilogCustomHandler : ILogEventSink
    {
        /// <summary>
        /// 
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public SerilogCustomHandler(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="logEvent"></param>
        public void Emit(LogEvent logEvent)
        {
            if (ServiceProvider != null)
            {
                var _channel = ServiceProvider.GetRequiredService<IChannelHelper>();
                //Console.WriteLine($"channel is null:{(_channel == null)}!");
                //这里进行比如异常推送等
            }
            //Console.WriteLine("--------- --  ----------- happend error wait to push!" + logEvent.MessageTemplate.ToString());
        }
    }
}
