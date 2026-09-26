using System.Text;

namespace PasteApeart.Handler.Extend
{
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

    }
}
