using ExampleForm.Controllers;
using ExampleForm.usermodels;
using Microsoft.AspNetCore.Mvc;
using PasteApeart.Helper;
using System.Text;

namespace ExampleForm.HttpApi.Host.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("/api/app/[controller]/[action]")]
    public class TestController : IBaseController
    {


        //[HttpGet]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
        //public async Task<string> abc()
        //{
        //    await _dbContext.UserInfo.Where(x => true).Include(x => x.UserName).FirstOrDefaultAsync();
        //    return "";
        //}

        /// <summary>
        /// 获取所有枚举，或者某一个枚举数据的值
        /// </summary>
        /// <param name="enumName"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
        public async Task<string> ReadEnumInfo(string enumName = "")
        {
            var enums = typeof(RoleInfo).Assembly.GetTypes()
                .Where(x => x.IsEnum)
                .WhereIf(!string.IsNullOrEmpty(enumName), x => x.Name == enumName)
                .ToList();
            var sb = new StringBuilder("");
            if (enumName == "abc")
            {
                await Task.CompletedTask;
            }
            foreach (var en in enums)
            {
                sb.AppendLine(en.Name);

                var pros = BuilderHelper.GetAllEnumItemInfo(en, false);

                foreach (var aat in pros)
                {
                    sb.AppendLine($"{aat.value} {aat.name} {aat.field}");
                }
            }
            return sb.ToString();
        }

    }

}
