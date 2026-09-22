using Volo.Abp.AspNetCore.Mvc;

namespace ExampleForm.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class IBaseController : AbpController
    {
        /// <summary>
        /// 
        /// </summary>
        protected IExampleFormDbContext _dbContext => LazyServiceProvider.LazyGetRequiredService<IExampleFormDbContext>();


        /// <summary>
        /// 获取登录者UID
        /// </summary>
        /// <returns></returns>
        protected int CurrentUserId()
        {
            if (base.HttpContext.Request.Headers.ContainsKey(PublicString.TokenHeadName))
            {
                var token = base.HttpContext.Request.Headers[PublicString.TokenHeadName].ToString();
                if (token.TokenCheck())
                {
                    var strs = token.Split('_');
                    int.TryParse(strs[1], out var userid);
                    return userid;
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取登录者UID
        /// </summary>
        /// <returns></returns>
        protected string CurrentUserToken()
        {
            //time_uid_guid
            if (base.HttpContext.Request.Headers.ContainsKey(PublicString.TokenHeadName))
            {
                var token = base.HttpContext.Request.Headers[PublicString.TokenHeadName].ToString();
                if (token.TokenCheck())
                {
                    return token;
                }
            }
            //临时测试，后续改成0
            return "";
        }


    }
}
