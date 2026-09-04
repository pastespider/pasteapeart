using PasteApeart.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteApeart.Handler
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="EventModel"></typeparam>
    public interface IEventHandler<EventModel> where EventModel : IEventModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task HandleEventAsync(EventModel input);
    }
}
