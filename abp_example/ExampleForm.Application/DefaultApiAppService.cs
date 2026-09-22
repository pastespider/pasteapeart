using Microsoft.AspNetCore.Mvc;
using PasteApeart.Application;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.Text;

namespace ExampleForm.Application
{
    /// <summary>
    /// 默认读取 这个很关键，实现了统一的新增 更新 详细 列表规范读取 相当于兜底的!
    ///</summary>
    [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
    public class DefaultApiAppService : ExampleFormAppService
    {

        private readonly IPasteDefaultAppService _appService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appService"></param>
        public DefaultApiAppService(IPasteDefaultAppService appService)
        {
            _appService = appService;
        }


        /// <summary>
        /// 获取新增格式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
        [Route("/api/app/{entityName}/readAddModel", Order = 10)]
        public async Task<VoloModelInfo> ReadAddModel([FromRoute] string entityName)
        {

            return await _appService.ReadAddModel(entityName);

        }

        /// <summary>
        /// 获取更新数据
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        [HttpGet]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
        [Route("/api/app/{entityName}/{id}/readUpdateModel", Order = 10)]
        public async Task<VoloModelInfo> ReadUpdateModel([FromRoute] string entityName, [FromRoute] string id)
        {
            return await _appService.ReadUpdateModel(entityName, id);

        }

        /// <summary>
        /// 获取详细
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/{entityName}/{id}/readDetailModel", Order = 10)]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
        public async Task<VoloModelInfo> ReadDetailModel([FromRoute] string entityName, [FromRoute] string id)
        {

            return await _appService.ReadDetailModel(entityName, id);

        }

        /// <summary>
        /// 获取列表格式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/{entityName}/readListModel", Order = 10)]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
        public async Task<VoloModelInfo> ReadListModel([FromRoute] string entityName, string searchName = "")
        {
            return await _appService.ReadListModel(entityName, searchName);
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="searchName"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        [HttpGet]
        [Route("/api/app/{entityName}/page", Order = 10)]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
        public async Task<dynamic> Page([FromRoute] string entityName, string searchName = "")
        {
            var query_string = base._httpContext.Request.QueryString.ToString();

            return await _appService.Page(entityName, query_string, searchName);

        }

        /// <summary>
        /// 通用创建方法
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        [HttpPost]
        [Route("/api/app/{entityName}/item", Order = 10)]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "add" })]
        public async Task<dynamic> CreateItemAsync([FromRoute] string entityName)
        {
            using var read = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
            var inputString = await read.ReadToEndAsync();
            return await _appService.CreateItemAsync(entityName, inputString);

        }

        /// <summary>
        /// 通用更新方法
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        [HttpPost]
        [Route("/api/app/{entityName}/updateItem", Order = 10)]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "edit" })]
        public async Task<object> UpdateItemAsync([FromRoute] string entityName)
        {
            using var read = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
            var inputString = await read.ReadToEndAsync();

            return await _appService.UpdateItemAsync(entityName, inputString);

        }


        /// <summary>
        /// 通用状态更新方法
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        [HttpPost]
        [Route("/api/app/{entityName}/updateState", Order = 10)]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "state" })]
        public async Task<string> UpdateState([FromRoute] string entityName, [FromBody] InputQueryUpdateState input)
        {
            return await _appService.UpdateState(entityName, input);


        }

        /// <summary>
        /// 通用删除 遵照ListDto的PasteDisable特性
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        [HttpPost]
        [Route("/api/app/{entityName}/{id}/remove", Order = 10)]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "del" })]
        public async Task<string> Remove([FromRoute] string entityName, [FromRoute] string id)
        {
            return await _appService.Remove(entityName, id);

        }



    }

}
