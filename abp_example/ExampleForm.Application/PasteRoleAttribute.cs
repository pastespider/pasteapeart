using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExampleForm
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
            string role = "", string grade = "")
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
                if (context.HttpContext.Request.Headers.TryGetValue(PublicString.TokenHeadName, out var vals))
                {
                    foreach (var htoken in vals)
                    {
                        if (htoken.TokenCheck())
                        {
                            token = htoken;
                            break;
                        }
                    }
                }
                if (String.IsNullOrEmpty(token))
                {
                    if (context.HttpContext.Request.Cookies.TryGetValue(PublicString.TokenHeadName, out var tok))
                    {
                        if (tok.TokenCheck())
                        {
                            token = tok;
                        }
                    }
                }
                //var token = String.Empty;
                //if (context.HttpContext.Request.Headers.TryGetValue(PublicString.TokenHeadName, out var vals))
                //{
                //    token = vals.FirstOrDefault();
                //}
                //if (!String.IsNullOrEmpty(token))
                //{
                //    token = token.Replace("Bearer ", "");
                //}
                //这里可以做token的安全校验
                if (!String.IsNullOrEmpty(token))
                {
                    //其实上面可以考虑只读取User即可，中间件校验了
                    //var claims = new[] { new Claim("FilterSiteId", "true"), new Claim(AbpClaimTypes.UserName,token),new Claim(AbpClaimTypes.SurName,token) };
                    //var identity = new ClaimsIdentity(claims, "Custom");//Simple
                    //context.HttpContext.User = new ClaimsPrincipal(identity);
                    //这里可以修改下，如果model和role都是空的，则可以不执行TokenCheck 这样可以基于controller和action做游客可访问的配置
                    //这里可以做一些判断，比如get不进行redis二次校验等
                    auth_code = 200;
                    if (auth_code < 200)
                    {
                        await Task.Delay(10);
                    }
                    //TODO:测试阶段，不需要验证
                    //var back = await _userHelper.HasRoleAsync(_model, _role, _grade, token);
                    //if (!back.role)
                    //{
                    //    auth_code = back.code;
                    //}
                    //else
                    //{
                    //    auth_code = 200;
                    //}

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
                        Content = Newtonsoft.Json.JsonConvert.SerializeObject(info)
                    };
                }
            }
            //await Task.CompletedTask;
        }

    }

}
