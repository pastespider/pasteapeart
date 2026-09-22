using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasteApeart;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler;
using ExampleApeart.Controllers;
using ExampleApeart.usermodels;
using System.ComponentModel.DataAnnotations;
using PasteApeart.Handler.redis;
using PasteApeart.Handler.Extensions;

namespace ExampleApeart.HttpApi.Host.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("/api/app/[controller]/[action]")]
    public class UserController : IBaseController
    {

        private IAppCache _appCache => LazyServiceProvider.LazyGetRequiredService<IAppCache>();

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        public async Task<FileContentResult> Image(string guid)
        {
            try
            {
                if (String.IsNullOrEmpty(guid))
                {
                    throw new Exception("验证码代码错误，guid不能为空!");
                }

                if (_appCache == null)
                {
                    Console.WriteLine("IAppCache  is null");
                }

                //进行特殊符号的替换工作
                if (!new System.Text.RegularExpressions.Regex("[0-9,a-z,A-Z]{16}").Match(guid).Success)
                {
                    throw new Exception("guid的位数不足，应为16位随机数，不能包含特殊符号，需要为字母和数字的组合");
                }
                if (await _appCache.ExistsAsync($"{PublicString.CacheImageHead}{guid}")) { throw new Exception("guid不能重复使用！"); }
                //判断guid是否存在
                string code = ImageHelper.CreateVerifyCode(ImageHelper.VerifyCodeType.NumberVerifyCode);
                await _appCache.SetAsync($"{PublicString.CacheImageHead}{guid}", code, 300);
                byte[] codeImage = ImageHelper.CreateByteByImgVerifyCode(code, 80, 36);
                return File(codeImage, @"image/jpeg");
            }
            catch (Exception exl)
            {
                Logger.LogError(exl.ToString());
                throw new Exception(exl.Message);
            }
        }


        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<dynamic> Login(InputLogin input)
        {

            var codeval = await _appCache.GetAsync($"{PublicString.CacheImageHead}{input.guid}");
            if (string.IsNullOrEmpty(codeval))
            {
                throw new PasteCodeException("图形验证码超时，请重新刷新！");
            }
            if (codeval != input.code)
            {
                await _appCache.DeleteAsync($"{PublicString.CacheImageHead}{input.guid}");
                throw new PasteCodeException("图形验证码错误，请重新输入！");
            }
            else
            {
                await _appCache.DeleteAsync($"{PublicString.CacheImageHead}{input.guid}");
            }

            var user = await _dbContext.UserInfo.Where(x => x.Email == input.Email).AsNoTracking().FirstOrDefaultAsync();
            if (user == null || user == default)
            {
                throw new PasteCodeException("不存在的邮箱账号，请重试！");
            }

            if (!user.IsEnable)
            {
                throw new PasteCodeException("账号被封禁，请联系管理员");
            }

            if (user.PassWord != input.Password.ToMd5Lower())
            {
                throw new PasteCodeException("密码错误，请重新输入");
            }



            var grade_ids = await _dbContext.UserGrade.Where(x => x.UserId == user.Id).Select(x => x.GradeId).ToArrayAsync();

            if (grade_ids == null || grade_ids.Length == 0)
            {
                throw new PasteCodeException("当前账号没有分配角色，无法继续操作！");
            }

            //查询是否有超级角色

            var _is_root = false;

            var grades = await _dbContext.GradeInfo.Where(x => x.IsEnable && grade_ids.Contains(x.Id)).AsNoTracking().ToArrayAsync();
            if (grades != null && grades.Length > 0)
            {
                foreach (var item in grades)
                {
                    if (item.Code == "root")
                    {
                        _is_root = true;
                        break;
                    }
                }
            }

            var _query_roleid = _dbContext.GradeRole.Where(x => grade_ids.Contains(x.GradeId)).Select(x => x.RoleId);

            //通过二次缓存，可以读取权限列表信息
            var roles = await _dbContext.RoleInfo.Where(x => x.IsEnable && x.RoleType == 0)
                .WhereIf(!_is_root, x => _query_roleid.Contains(x.Id))
                .WhereIf(_is_root, x => true)
                .AsNoTracking()
                .ToListAsync();
            if (roles != null && roles.Count > 0)
            {
                //var rolesdto = ObjectMapper.Map<List<RoleInfo>, List<RoleAuthDto>>(roles);
                var rolesdto = roles.Adapt<List<RoleAuthDto>>();
                var gradestrings = "";
                //var grades = await _dbContext.GradeInfo.Where(x => grade_ids.Contains(x.Id)).Select(x => x.Code).ToArrayAsync();
                if (grades != null && grades.Length > 0)
                {
                    //非分隔符只有_，这里引入大写字母V分割角色，为了适配双击选中token和urlencode
                    //新增角色的时候，会进行小写转换，这样就不会冲突了
                    gradestrings = string.Join('V', grades.Select(x => x.Code).ToArray());
                }
                var token = user.Id.TokenBuild(gradestrings);
                //缓冲72小时
                await _appCache.SetObjectAsync<List<RoleAuthDto>>(token.CurrentTokenCacheKey(), rolesdto, PublicString.TokenTimeOut);

                //按需配置 
                //base.Response.Cookies.Append(PublicString.TokenHeadName, token, new Microsoft.AspNetCore.Http.CookieOptions() { Path = "/", Expires = DateTimeOffset.Now.AddSeconds(PublicString.TokenTimeOut), Secure = true });
                base.Response.Cookies.Append(PublicString.TokenHeadName, token, new Microsoft.AspNetCore.Http.CookieOptions() { Path = "/", Expires = DateTimeOffset.Now.AddSeconds(PublicString.TokenTimeOut), Secure = false });
                return new
                {
                    name = user.Name,
                    token = token
                };
            }
            else
            {
                throw new PasteCodeException("未为当前账号分配权限，无法进行登陆操作，请联系管理员分配权限后再试！");
            }

        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> LoginOut()
        {
            if (base.HttpContext.Request.Headers.ContainsKey(PublicString.TokenHeadName))
            {
                var token = base.HttpContext.Request.Headers[PublicString.TokenHeadName].ToString();
                await _appCache.DeleteAsync(token);
            }
            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        [HttpGet]
        [TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "data", "view" })]
        public async Task<UserInfoListDto> ReadInfo()
        {
            var userid = CurrentUserId();
            var user = await _dbContext.UserInfo.Where(x => x.Id == userid).AsNoTracking().FirstOrDefaultAsync();
            if (user != null && user != default)
            {
                //return ObjectMapper.Map<UserInfo, UserInfoListDto>(user);
                return user.Adapt<UserInfoListDto>();
            }
            else
            {
                Console.WriteLine($"当前用户 UserId:{userid}");
                throw new PasteCodeException("没有这个用户信息");
            }
        }

        ///// <summary>
        ///// 获取所有枚举，或者某一个枚举数据的值
        ///// 针对文档返回的枚举各种作妖，这里直接这样操作
        ///// </summary>
        ///// <param name="enumName"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[TypeFilter(typeof(RoleAttribute), Arguments = new object[] { "root", "root" })]
        //public async Task<string> ReadEnumInfo(string enumName = "")
        //{
        //    //默认枚举都放在Domain的程序上
        //    var enums = typeof(RoleInfo).Assembly.GetTypes()
        //        .Where(x => x.IsEnum)
        //        .WhereIf(!string.IsNullOrEmpty(enumName), x => x.Name == enumName)
        //        .ToList();

        //    var sb = new StringBuilder("");

        //    if (enumName == "abc")
        //    {
        //        await Task.CompletedTask;
        //    }
        //    foreach (var en in enums)
        //    {
        //        sb.AppendLine(en.Name);
        //        //非XML模式，如果是XML模式要调用ReadEnumProperty
        //        var pros = BuilderHelper.GetAllEnumItemInfo(en, false);
        //        foreach (var aat in pros)
        //        {
        //            sb.AppendLine($"{aat.value} {aat.name} {aat.field}");
        //        }
        //    }
        //    return sb.ToString();
        //}

        /// <summary>
        /// 结果缓存 需要添加app.UseResponseCaching();
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public string Now()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

    }

    /// <summary>
    /// 登录的模型
    /// </summary>
    public class InputLogin
    {

        /// <summary>
        /// 邮箱或者手机号码
        /// </summary>
        //[RegularExpression("[0-9,a-z]{3,16}@[a-z,0-9]{2,12}.[a-z]{2,5}", ErrorMessage = "账号格式有误xxx(3-16)@xxx(2-12).xxx(2-5)")]
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [RegularExpression("[0-9,a-z,A-Z]{3,32}", ErrorMessage = "密码的有效位数为3~32位数的字母数字组合")]
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 图形验证码代码
        /// </summary>
        [RegularExpression("[0-9]{4,6}", ErrorMessage = "图形验证码的值为4-6位数的数字")]
        [Required]
        public string code { get; set; }

        /// <summary>
        /// 图形验证码串号
        /// </summary>
        [RegularExpression("[0-9,a-z,A-Z]{16}", ErrorMessage = "串号为一组16位数的字母或者数字的组合")]
        [Required]
        public string guid { get; set; }
    }

}
