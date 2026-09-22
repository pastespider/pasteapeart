using System.Text;

namespace ExampleForm
{
    ///// <summary>
    ///// redis的链接信息
    ///// </summary>
    //public class RedisConfig
    //{

    //    /// <summary>
    //    /// 主链接
    //    /// </summary>
    //    public string MainConnection { get; set; }

    //    /// <summary>
    //    /// 前缀
    //    /// </summary>
    //    public string Prefix { get; set; }
    //}


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

    /// <summary>
    /// 
    /// </summary>
    public static class ObjectExtend
    {
        /// <summary>
        /// 转化成小写的md5
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        public static string ToMd5Lower(this string inputValue)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(inputValue));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// 从参数字符串获取参数列表
        /// </summary>
        /// <param name="_search"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToQuerys(this string _search)
        {
            if (!String.IsNullOrEmpty(_search))
            {
                if (_search.StartsWith("?"))
                {
                    _search = _search.Substring(1);
                }
                if (!String.IsNullOrEmpty(_search))
                {
                    var _dic = new Dictionary<string, string>();
                    var splits = _search.Split('&');
                    foreach (var _one in splits)
                    {
                        if (_one.Contains("="))
                        {
                            var ones = _one.Split('=');
                            var _key = ones[0];
                            var _val = ones[1];
                            if (!_dic.ContainsKey(_key))
                            {
                                _dic.Add(_key, _val);
                            }
                        }
                    }
                    return _dic;
                }
            }
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// 时间转时间戳s ok
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ToUnixTimeSeconds(this DateTime dt)
        {
            var dto = new DateTimeOffset(dt);
            return new DateTimeOffset(dt).ToUnixTimeSeconds();
        }

        /// <summary>
        /// 时间戳转换成时间
        /// </summary>
        /// <param name="timestamp">timestamp</param>
        /// <returns></returns>
        public static DateTime ToUtcDateFromMilliSecond(this long timestamp)
        {
            var date = TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return date.AddMilliseconds(timestamp);
        }

        /// <summary>
        /// 为登陆创建密钥
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="grades">使用|分割角色代码，示例role|user</param>
        /// <returns></returns>
        public static string TokenBuild(this int userid, string grades)
        {
            var nowtime = DateTime.Now.ToUnixTimeSeconds();
            // timestamp_userid_grades_temptoken
            var temptoken = $"{nowtime}_{userid}_{grades}_{PublicString.TokenKey}".ToMd5Lower();
            //Console.WriteLine("{0}-{1}-{2}-{3}", nowtime, userid, $"|{grades}|",temptoken);
            return $"{nowtime}_{userid}_{grades}_{temptoken}";
        }

        /// <summary>
        /// 检查密钥是否合法
        /// </summary>
        /// <param name="utoken"></param>
        /// <returns></returns>
        public static bool TokenCheck(this string utoken)
        {
            if (!String.IsNullOrEmpty(utoken))
            {
                var strs = utoken.Split('_');
                if (strs.Length == 4)
                {
                    if ($"{strs[0]}_{strs[1]}_{strs[2]}_{PublicString.TokenKey}".ToMd5Lower() == strs[3])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 从密钥中获取用户id
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static int CurrentUserId(this string token)
        {
            if (!String.IsNullOrEmpty(token))
            {
                var strs = token.Split('_');
                if (strs.Length >= 1)
                {
                    int.TryParse(strs[1],out var uid);
                    return uid;
                }
            }
            return 0;
        }

    }
}
