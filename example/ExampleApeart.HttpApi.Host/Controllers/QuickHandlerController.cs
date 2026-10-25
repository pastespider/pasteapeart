using ExampleApeart.Controllers;
using ExampleApeart.usermodels;
using Microsoft.AspNetCore.Mvc;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler;
using PasteApeart.Handler.common;
using PasteApeart.Helper;
using System.Reflection;

namespace ExampleApeart.HttpApi.Host.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("/api/app/quickHandler")]
    [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
    public class QuickHandlerController : IBaseController
    {

        private readonly IEventQuickActionHandler _queryHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventActionHandler"></param>
        public QuickHandlerController(IEventQuickActionHandler eventActionHandler)
        {
            _queryHandler = eventActionHandler;
        }

        private static readonly string[] IgnoreProperty = new string[] { "mutexVal", "mutexKey" };

        /// <summary>
        /// 读取AddDto的数据模型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("readAddModel")]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
        public VoloModelInfo ReadAddModel([FromQuery] string model_name)
        {
            if (string.IsNullOrEmpty(model_name))
            {
                throw new PasteArgsException(nameof(model_name));
            }
            var model = _queryHandler.ReadAddModel(model_name);
            if (model.Attributes == null)
            {
                model.Attributes = new List<VoloModelAttribute>();
            }
            if (!model.Attributes.Any(x => x.Name == "okaction"))
            {
                model.Attributes.Add(new VoloModelAttribute { Name = "okaction", Args1 = "current", Args2 = "reload" });
            }
            if (model.Properties?.Any() == true)
            {

                //倒序
                model.Properties.Reverse();

                foreach (var item in model.Properties)
                {

                    switch (item.Name)
                    {
                        case "eventName":
                            {
                                item.Title = "事件模型";
                                if (item.Attributes == null)
                                {
                                    item.Attributes = new System.Collections.Generic.List<VoloModelAttribute>();
                                }
                                item.Attributes.Add(new VoloModelAttribute { Name = "hidden" });
                            }
                            break;
                        case "expireSecond":
                            {
                                item.Title = "延迟秒数";
                                item.Placeholder = "延迟X秒后执行这个事件，默认为立即执行";
                            }
                            break;
                        case "maxRetryTime":
                            {
                                item.Title = "最大重试";
                                item.Placeholder = "消息异常后，最大重试次数，会基于重试模型，自动延期";
                            }
                            break;
                        case "isSubscribe":
                            {
                                item.Title = "订阅模式";
                                item.Placeholder = "广播消息";
                            }
                            break;
                        case "retryModel":
                            {
                                item.Title = "重试模型";
                                item.Placeholder = "基于哪个重试模型，主要影响的是重试的间隔";
                            }
                            break;
                    }
                }

                model.Properties.RemoveAll(x => IgnoreProperty.Contains(x.Name) || !x.CanWrite);
            }
            return model;
        }


        /// <summary>
        /// 读取ListDto的数据模型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("readListModel")]
        public VoloModelInfo ReadListModel()
        {
            var _model = BuilderHelper.AttributeReadModelProperty<QuickHandlerPropertyInfo>(new QuickHandlerPropertyInfo());
            var _query_model = BuilderHelper.AttributeReadModelProperty(new InputSearchBase());
            if (_query_model != null)
            {
                _model.QueryProperties = _query_model.Properties;
            }
            return _model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("page")]
        public async Task<PastePagedResult<QuickHandlerPropertyInfo>> Page([FromQuery] InputSearchBase input)
        {
            return await _queryHandler.PageAsync(input);
            //var reads = await _queryHandler.PageAsync(input);

            //if (reads == null)
            //{
            //    return null;
            //}
            //var pageDto = new PastePagedResult<QuickHandlerModelDto>();
            //if (reads.Items?.Any() == true)
            //{
            //    pageDto.Items = reads.Items.Adapt<List<QuickHandlerModelDto>>();
            //}
            //pageDto.TotalCount = reads.TotalCount;
            //return pageDto;
        }

        /// <summary>
        /// 执行一个
        ///</summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("item")]
        public async Task<dynamic> CreateItemAsync([FromServices]IEventActionHandler handler)
        {
            var bodyStr = String.Empty;
            using var sb = new StreamReader(Request.Body);
            bodyStr = await sb.ReadToEndAsync();
            //await handler.ActionMessageAsync(bodyStr, typeof(UserInfoDto).Assembly.GetTypes());

            //可以考虑基于PasteAuth校验权限... .. .
            if (string.IsNullOrEmpty(bodyStr))
            {
                throw new PasteCodeException("传入的内容不能为空");
            }

            var result = await handler.ActionResultAsync(bodyStr, typeof(UserInfoDto).Assembly.GetTypes());
            if (result != null)
            {
                return result;
            }
            //后续可以调用有返回值的版本！！！
            return "执行完成 如果需要返回值请给Result赋值";

        }


        //private readonly IEventQuickActionHandler _queryHandler;

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="eventActionHandler"></param>
        //public QuickHandlerController(IEventQuickActionHandler eventActionHandler)
        //{
        //    _queryHandler = eventActionHandler;
        //}

        ///// <summary>
        ///// 读取AddDto的数据模型
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("readAddModel")]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
        //public VoloModelInfo ReadAddModel([FromQuery] string model_name)
        //{
        //    if (string.IsNullOrEmpty(model_name))
        //    {
        //        throw new PasteArgsException(nameof(model_name));// PasteCodeException("");
        //    }
        //    return _queryHandler.ReadAddModel(model_name);
        //}


        ///// <summary>
        ///// 读取ListDto的数据模型
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("readListModel")]
        //public VoloModelInfo ReadListModel()
        //{
        //    var _model = BuilderHelper.AttributeReadModelProperty<QuickHandlerPropertyInfo>(new QuickHandlerPropertyInfo());
        //    var _query_model = BuilderHelper.AttributeReadModelProperty(new InputSearchBase());
        //    if (_query_model != null)
        //    {
        //        _model.QueryProperties = _query_model.Properties;
        //    }
        //    return _model;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("page")]
        //public async Task<PastePagedResult<QuickHandlerPropertyInfo>> Page([FromQuery] InputSearchBase input)
        //{
        //    return await _queryHandler.PageAsync(input);
        //}

        ///// <summary>
        ///// 执行一个
        /////</summary>
        ///// <param name="handler"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("item")]
        //public async Task<string> CreateItemAsync([FromServices] IEventActionHandler handler)
        //{
        //    var bodyStr = String.Empty;
        //    using var sb = new StreamReader(Request.Body);
        //    bodyStr = await sb.ReadToEndAsync();
        //    await handler.ActionMessageAsync(bodyStr, typeof(UserInfoDto).Assembly.GetTypes());
        //    return "执行完成";

        //}

    }

}
