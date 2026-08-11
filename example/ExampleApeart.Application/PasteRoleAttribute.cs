using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PasteApeart;
using PasteApeart.Handler;
using ExampleApeart.Handler;
using System.Security.Claims;
using System.Text.Json;

namespace ExampleApeart
{
    /// <summary>
    /// 权限过滤 授权
    /// </summary>
    public class RoleAttribute : IAsyncAuthorizationFilter
    {

        /// <summary>
        /// 权限
        /// </summary>
        private string _role { set; get; }

        /// <summary>
        /// 角色
        /// </summary>
        private string _model { get; set; }

        /// <summary>
        /// 角色 单个
        /// </summary>
        private string _grade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private readonly CacheHandler _userHelper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="model">模块</param>
        /// <param name="role">权限</param>
        /// <param name="grade">角色</param>
        public RoleAttribute(
            CacheHandler cache,
            string model = "",
            string role = "",
            string grade = "")
        {
            _userHelper = cache;
            _model = model;
            _role = role;
            _grade = grade;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var needauth = true;
            foreach (var item in context.Filters)
            {
                if (item is RoleAttribute)
                {
                    needauth = (item == this);
                }
            }

            if (needauth)
            {
                var auth_code = 401;
                var _action = String.Empty;
                var _controller = String.Empty;

                foreach (var ii in context.RouteData.Values)
                {
                    // action
                    // controller 可以基于这个进行动态权限校验！
                    if (ii.Key == "action")
                    {
                        _action = ii.Value.ToString();
                    }
                    if (ii.Key == "controller")
                    {
                        _controller = ii.Value.ToString();
                    }
                }

                if (String.IsNullOrEmpty(_model))
                {
                    _model = _controller;
                }
                if (String.IsNullOrEmpty(_role))
                {
                    _role = _action;
                }
                var token = String.Empty;
                token = context.HttpContext.User.FindFirstValue("Token");
                Console.WriteLine($"当前密钥：{token}");
                if (!String.IsNullOrEmpty(token))
                {
                    var back = await _userHelper.HasRoleAsync(_model, _role, _grade, token);
                    if (!back.role)
                    {
                        auth_code = back.code;
                    }
                    else
                    {
                        auth_code = 200;
                    }
                }


                if (auth_code != 200)
                {
                    var info = new ExceptionModel();
                    info.code = $"{auth_code}";
                    info.success = false;
                    info.message = auth_code == 401 ? "当前密钥信息有误，请登陆后重试!" : $"权限不足，需要权限{_model}-{_role} !";
                    context.Result = new ContentResult
                    {
                        StatusCode = auth_code,
                        ContentType = "application/json;charset=utf-8",
                        Content = JsonSerializer.Serialize(info, JsonConfig.Options)
                        //Content = Newtonsoft.Json.JsonConvert.SerializeObject(info)
                    };
                }
            }
            //await Task.CompletedTask;
        }

    }

}
