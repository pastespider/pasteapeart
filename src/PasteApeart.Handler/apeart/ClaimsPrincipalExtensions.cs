using System.Security.Claims;

namespace PasteApeart.Handler
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pa"></param>
        /// <returns></returns>
        public static string CurrentToken(this ClaimsPrincipal pa)
        {
            return pa.FindFirstValue("Token");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pa"></param>
        /// <returns></returns>
        public static int CurrentUserId(this ClaimsPrincipal pa)
        {
            var useridStr = pa.FindFirstValue("UserId");
            if (!string.IsNullOrEmpty(useridStr))
            {
                int.TryParse(useridStr, out var uid);
                return uid;
            }
            return 0;
        }
    }
}
