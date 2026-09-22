using ExampleApeart.Controllers;
using ExampleApeart.usermodels;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasteApeart.Handler;
using PasteApeart.Helper;
using System.Text;

namespace ExampleApeart.HttpApi.Host.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
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
                .PasteWhereIf(!string.IsNullOrEmpty(enumName), x => x.Name == enumName)
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

        /// <summary>
        /// 测试时间 格式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public InputDate InputDate(InputDate input)
        {
            if (input.edate != null)
            {
                input.edate = input.edate.Value.AddDays(3);
            }

            input.sdate = input.sdate.AddMonths(1);

            return input;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
        public async Task<string> TestAdaptAsync()
        {
            var first = await _dbContext.UserInfo.OrderBy(x => x.Id).AsNoTracking().FirstOrDefaultAsync();

            var newone = new UserInfo { };
            first.Adapt(newone);
            //注意ID会带出来.... ... 
            return $"Id{newone.Id} UserName{newone.Name}";
        }



    }

    /// <summary>
    /// 
    /// </summary>
    public class InputDate
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime sdate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? edate { get; set; }
    }

}
