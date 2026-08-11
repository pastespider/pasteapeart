using System.Text;

namespace PasteApeart
{

    /// <summary>
    /// 
    /// </summary>
    public class PublicString
    {
        /// <summary>
        /// 图形二维码的前缀 cache:image:
        /// </summary>
        public const string CacheImageHead = "cache:image:";

        /// <summary>
        /// user.token密钥 这个在使用模板创建的时候会随机掉
        /// </summary>
        public const string TokenKey = "b118f5eb-596d-4ed7-bfc4-21618327142e";

        /// <summary>
        /// 密钥名称
        /// </summary>
        public const string TokenHeadName = "token";

        /// <summary>
        /// 角色缓存的权限列表 cache:item:grade:{gradecode}
        /// </summary>
        public const string CacheGradeInfoHead = "cache:item:grade:{0}";

        /// <summary>
        /// 密钥过期时间
        /// </summary>
        public const int TokenTimeOut = 3600 * 24 * 7;

        /// <summary>
        /// report:hash:user yyyyMMdd_userid_type value
        /// </summary>
        public const string CacheReportUserKey = "report:hash:user";

        /// <summary>
        /// report:hash:user yyyyMMdd_projectid_type value
        /// </summary>
        public const string CacheReportProjectKey = "report:hash:project";

        /// <summary>
        /// 我的下级账号列表 cache:next:user:{0}
        /// </summary>
        public const string CacheMyNextUserList = "cache:next:user:{0}";

        /// <summary>
        /// 
        /// </summary>
        public const string _cacheheadproject = "cache:item:project:";

        /// <summary>
        /// 
        /// </summary>
        public const string _cacheheadorder = "cache:item:order:";

        /// <summary>
        /// 
        /// </summary>
        public const string _cacheheadpart = "cache:item:part:";

        /// <summary>
        /// 
        /// </summary>
        public const string _cacheheaduser = "cache:item:user:";
    }


}
