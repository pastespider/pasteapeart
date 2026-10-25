using Microsoft.Extensions.DependencyInjection;
using PasteApeart;
using PasteApeart.Handler;
using Serilog.Core;
using Serilog.Events;

namespace ExampleForm.Handler
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
                Console.WriteLine($"channel is null:{(_channel == null)}!");
            }
            Console.WriteLine("--------- --  ----------- happend error wait to push!" + logEvent.MessageTemplate.ToString());
        }
    }
}
