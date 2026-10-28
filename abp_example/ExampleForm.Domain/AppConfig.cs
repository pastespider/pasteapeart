using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfig
    {

        /// <summary>
        /// 
        /// </summary>
        public bool RabbitQueue { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public string RootAccount { get; set; } = "admin@admin.com";

        /// <summary>
        /// 
        /// </summary>
        public string RootPassword { get; set; } = "123456";
    }
}
