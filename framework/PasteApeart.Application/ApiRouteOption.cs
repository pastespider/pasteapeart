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
    public class ApiRouteOption
    {
        /// <summary>
        /// 默认为api/app
        /// </summary>
        public string RootApi { get; set; } = "api/app";

        /// <summary>
        /// Action命名格式CamelCase/KebabCase
        /// </summary>
        public string RouteModel { get; set; } = "CamelCase";


        /// <summary>
        /// AppService文件结尾 默认AppService
        /// </summary>
        public string ServiceEnd { get; set; } = "AppService";

    }
}
