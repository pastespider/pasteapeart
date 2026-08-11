//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ActionConstraints;
//using Microsoft.AspNetCore.Mvc.ApplicationModels;
//using Microsoft.AspNetCore.Mvc.ModelBinding;

//namespace PasteApeart
//{

//    /// <summary>
//    /// 配置路由得 把AppService注入到Controller中
//    /// </summary>
//    [Obsolete("Use AppServiceAutoConvention")]
//    public class ApeartAppServiceAutoConvention : IApplicationModelConvention
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="application"></param>
//        public void Apply(ApplicationModel application)
//        {
//            var appServices = application.Controllers
//                .Where(c => c.ControllerType.Name.EndsWith("AppService"))
//                .ToList();

//            foreach (var controller in appServices)
//            {
//                // 获取控制器名称
//                var controllerName = controller.ControllerType.Name;
//                if (controllerName.EndsWith("AppService"))
//                {
//                    controllerName = controllerName[..^10];
//                }

//                controller.ControllerName = controllerName;

//                // 检查 Controller 级别是否已有手动配置的路由
//                var hasManualControllerRoute = controller.Selectors.Any(s =>
//                    s.AttributeRouteModel != null && s.AttributeRouteModel.Template != null);

//                // 如果没有手动配置，才设置默认路由
//                if (!hasManualControllerRoute)
//                {
//                    var route = $"api/app/{ToCamelCase(controllerName)}";
//                    controller.Selectors[0].AttributeRouteModel = new AttributeRouteModel(
//                        new RouteAttribute(route));
//                }

//                // 配置每个 Action
//                foreach (var action in controller.Actions)
//                {
//                    // 检查是否已有手动配置的 HTTP 动词
//                    var hasManualHttpMethod = action.Selectors.Any(s =>
//                        s.ActionConstraints != null && s.ActionConstraints.Any());

//                    // 设置 HTTP 动词（如果没手动配置）
//                    if (!hasManualHttpMethod)
//                    {
//                        var httpMethod = GetHttpMethod(action.ActionName);
//                        if (httpMethod != null)
//                        {
//                            action.Selectors[0].ActionConstraints.Add(new HttpMethodActionConstraint(new[] { httpMethod }));
//                        }
//                    }

//                    // ⭐ 关键：为参数设置绑定源
//                    foreach (var parameter in action.Parameters)
//                    {
//                        // 检查是否已手动配置了绑定源
//                        if (parameter.BindingInfo != null && parameter.BindingInfo.BindingSource != null)
//                        {
//                            continue; // 已有配置，跳过
//                        }

//                        // 判断参数类型
//                        var paramType = parameter.ParameterType;

//                        // 如果是复杂类型（非简单类型）且不是从路由来的
//                        if (!IsSimpleType(paramType) && !IsRouteParameter(action, parameter.ParameterName))
//                        {
//                            // 设置 [FromBody] 绑定
//                            parameter.BindingInfo = new BindingInfo
//                            {
//                                BindingSource = BindingSource.Body
//                            };
//                        }
//                        // 如果参数名是 id，且是简单类型，设置为 FromRoute
//                        else if (parameter.ParameterName.Equals("id", StringComparison.OrdinalIgnoreCase))
//                        {
//                            parameter.BindingInfo = new BindingInfo
//                            {
//                                BindingSource = BindingSource.Path
//                            };
//                        }
//                        // 其他简单类型使用 FromQuery
//                        else if (IsSimpleType(paramType))
//                        {
//                            parameter.BindingInfo = new BindingInfo
//                            {
//                                BindingSource = BindingSource.Query
//                            };
//                        }
//                    }

//                    // 检查 Action 是否已有手动配置的路由
//                    var hasManualActionRoute = action.Selectors.Any(s =>
//                        s.AttributeRouteModel != null && s.AttributeRouteModel.Template != null);

//                    if (hasManualActionRoute)
//                    {
//                        continue;
//                    }

//                    // 设置 Action 路由
//                    var actionName = action.ActionName.Replace("Async", "");
//                    var routeTemplate = GetActionRouteTemplate(actionName, action.Parameters);
//                    if (!string.IsNullOrEmpty(routeTemplate))
//                    {
//                        action.Selectors[0].AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(routeTemplate));
//                    }
//                }
//            }
//        }


//        // 辅助方法：判断是否为简单类型
//        private bool IsSimpleType(Type type)
//        {
//            return type.IsPrimitive ||
//                   type == typeof(string) ||
//                   type == typeof(decimal) ||
//                   type == typeof(DateTime) ||
//                   type == typeof(DateTimeOffset) ||
//                   type == typeof(Guid) ||
//                   type == typeof(int?) ||
//                   type == typeof(long?) ||
//                   type == typeof(decimal?) ||
//                   type == typeof(DateTime?) ||
//                   type == typeof(Guid?) ||
//                   type.IsEnum;
//        }

//        // 辅助方法：判断参数是否从路由获取
//        private bool IsRouteParameter(ActionModel action, string parameterName)
//        {
//            // 检查路由模板中是否包含该参数
//            foreach (var selector in action.Selectors)
//            {
//                if (selector.AttributeRouteModel?.Template?.Contains($"{{{parameterName}}}") == true)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        // 辅助方法：获取 HTTP 方法
//        private string GetHttpMethod(string actionName)
//        {
//            if (actionName.StartsWith("Get"))
//                return "GET";
//            if (actionName.StartsWith("Create"))
//                return "POST";
//            if (actionName.StartsWith("Update"))
//                return "PUT";
//            if (actionName.StartsWith("Delete"))
//                return "DELETE";
//            return null;
//        }

//        // 辅助方法：获取 Action 路由模板
//        private string GetActionRouteTemplate(string actionName, IList<ParameterModel> parameters)
//        {
//            if (actionName == "Get" && parameters.Any(p => p.ParameterName == "id"))
//            {
//                return "{id}";
//            }

//            if (actionName != "Get" && actionName != "Create" &&
//                actionName != "Update" && actionName != "Delete")
//            {
//                var actionRoute = ToCamelCase(actionName);
//                if (parameters.Any(p => p.ParameterName == "id"))
//                {
//                    actionRoute = $"{{id}}/{actionRoute}";
//                }
//                return actionRoute;
//            }

//            return null;
//        }


//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        private string ToKebabCase(string input)
//        {
//            if (string.IsNullOrEmpty(input)) return "";
//            // 去掉常见前缀
//            //input = input.Replace("Get", "").Replace("Create", "").Replace("Update", "").Replace("Delete", "");
//            if (input.StartsWith("Create"))
//            {
//                input = input.Replace("Create", "");
//            }
//            return string.Concat(input.Select((c, i) =>
//                i > 0 && char.IsUpper(c) ? "-" + c.ToString() : c.ToString())).ToLower();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="input"></param>
//        /// <returns></returns>
//        private string ToCamelCase(string input)
//        {
//            if (string.IsNullOrEmpty(input)) return "";

//            //if (input == "UpdateItem") { return "updateItem"; }

//            if (input.StartsWith("Create"))
//            {
//                input = input.Replace("Create", "");
//            }

//            // 去掉常见前缀
//            //input = input.Replace("Get", "").Replace("Create", "").Replace("Update", "").Replace("Delete", "");

//            if (string.IsNullOrEmpty(input)) return "";

//            // 首字母小写，其余保持不变
//            return char.ToLowerInvariant(input[0]) + input.Substring(1);
//        }

//    }

//}
