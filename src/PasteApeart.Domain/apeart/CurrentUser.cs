using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteApeart
{
    /// <summary>
    /// 
    /// </summary>
    public class CurrentUser
    {
        //TODO 应该扩展ClaimsPrincipal 而不是重写一个 没意义！

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }


    }
}
