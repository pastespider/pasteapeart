using ExampleForm.Application.Contracts;
using ExampleForm.Controllers;
using ExampleForm.usermodels;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler;
using PasteApeart.Handler.common;
using PasteApeart.Helper;
using System.Reflection;

namespace ExampleForm.HttpApi.Host.Controllers
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
                throw new PasteArgsException(nameof(model_name));// PasteCodeException("");
            }
            var _model = _queryHandler.ReadAddModel(model_name);

            var findName = _model.Properties.FirstOrDefault(x=>x.Name=="eventName");
            if (findName != null)
            {
                findName.Value = model_name;
            }

            return _model;
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
        public async Task<string> CreateItemAsync([FromServices] IEventActionHandler handler)
        {
            var bodyStr = String.Empty;
            using var sb = new StreamReader(Request.Body);
            bodyStr = await sb.ReadToEndAsync();
            await handler.ActionMessageAsync(bodyStr, typeof(UserInfoDto).Assembly.GetTypes());
            return "执行完成";

        }

    }

}
