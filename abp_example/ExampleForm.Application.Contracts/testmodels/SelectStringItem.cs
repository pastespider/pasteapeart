using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleForm.Application.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectStringItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool selected { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SelectIntItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool selected { get; set; }
    }
}
