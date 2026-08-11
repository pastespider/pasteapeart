using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PasteApeart
{
    /// <summary>
    /// 让 MVC 能发现 AppService
    /// </summary>
    public class AppServiceControllerFeatureProvider : ControllerFeatureProvider
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        protected override bool IsController(TypeInfo typeInfo)
        {
            // 如果是抽象类或接口，跳过
            if (!typeInfo.IsClass || typeInfo.IsAbstract || typeInfo.IsInterface)
                return false;

            // 实现了 IAppService 接口的类，都当作 Controller
            if (typeof(IAppService).IsAssignableFrom(typeInfo))
                return true;

            // 默认规则：以 Controller 结尾
            return typeInfo.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase);
        }
    }
}
