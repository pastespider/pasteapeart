namespace PasteApeart.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtendHelper
    {

        #region 关于权限过滤的查询

        /// <summary>
        /// 有权限配置转化成UI的disable特性
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="attrs"></param>
        /// <returns></returns>
        [Obsolete("use enable ToEnableAttribute")]
        public static VoloModelAttribute ToDisableAttribute(IEnumerable<IPasteAuthRole> roles, List<PasteAuthAttribute> attrs)
        {
            if (attrs?.Any() == true)
            {
                var volo = new VoloModelAttribute { Name = "disable", Args1 = "false", Args2 = "false", Args3 = "false" };
                foreach (var item in attrs)
                {
                    if (String.IsNullOrEmpty(item.Method))
                    {
                        continue;
                    }
                    if (String.IsNullOrEmpty(item.Model) || string.IsNullOrEmpty(item.Role))
                    {
                        if (item.Method == "*")
                        {
                            if (!item.Auth)
                            {
                                volo.Args1 = "true";
                                volo.Args2 = "true";
                                volo.Args3 = "true";
                                volo.Args4 = "";
                            }
                            else
                            {
                                volo.Args1 = "false";
                                volo.Args2 = "false";
                                volo.Args3 = "false";
                                volo.Args4 = "";
                            }
                        }
                        else
                        {
                            foreach (var ii in item.Method.Split(','))
                            {
                                switch (ii)
                                {
                                    case "add":
                                        {
                                            volo.Args1 = !item.Auth ? "true" : "false";
                                        }
                                        break;
                                    case "update":
                                        {
                                            volo.Args2 = !item.Auth ? "true" : "false";
                                        }
                                        break;
                                    case "del":
                                        {
                                            volo.Args3 = !item.Auth ? "true" : "false";
                                        }
                                        break;
                                    case "detail":
                                        {
                                            volo.Args4 = item.Auth ? "true" : "false";
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //判断权限
                        var hasRole = roles.Where(x => (x.Model == "root" && x.Role == "root") || (x.Model == item.Model && x.Role == item.Role)).Any();
                        if (item.Method == "*")
                        {
                            if (!hasRole)
                            {
                                volo.Args1 = "true";
                                volo.Args2 = "true";
                                volo.Args3 = "true";
                                volo.Args4 = "";
                            }
                            else
                            {
                                volo.Args1 = "false";
                                volo.Args2 = "false";
                                volo.Args3 = "false";
                                volo.Args4 = "";
                            }
                        }
                        else
                        {
                            foreach (var ii in item.Method.Split(','))
                            {
                                switch (ii)
                                {
                                    case "add":
                                        {
                                            volo.Args1 = !hasRole ? "true" : "false";
                                        }
                                        break;
                                    case "update":
                                        {
                                            volo.Args2 = !hasRole ? "true" : "false";
                                        }
                                        break;
                                    case "del":
                                        {
                                            volo.Args3 = !hasRole ? "true" : "false";
                                        }
                                        break;
                                    case "detail":
                                        {
                                            volo.Args4 = hasRole ? "true" : "false";
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                return volo;
            }
            return null;
        }

        /// <summary>
        /// 有权限配置转化成UI的enable特性
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="attrs"></param>
        /// <returns></returns>
        public static VoloModelAttribute ToEnableAttribute(IEnumerable<IPasteAuthRole> roles, List<PasteAuthAttribute> attrs)
        {
            if (attrs?.Any() == true)
            {
                var volo = new VoloModelAttribute { Name = "enable", Args1 = "", Args2 = "", Args3 = "" };
                foreach (var item in attrs)
                {
                    if (String.IsNullOrEmpty(item.Method))
                    {
                        continue;
                    }
                    if (String.IsNullOrEmpty(item.Model) || string.IsNullOrEmpty(item.Role))
                    {
                        if (item.Method == "*")
                        {
                            if (!item.Auth)
                            {
                                volo.Args1 = "false";
                                volo.Args2 = "false";
                                volo.Args3 = "false";
                                volo.Args4 = "false";
                            }
                            else
                            {
                                volo.Args1 = "true";
                                volo.Args2 = "true";
                                volo.Args3 = "true";
                                volo.Args4 = "true";
                            }
                        }
                        else
                        {
                            foreach (var ii in item.Method.Split(','))
                            {
                                switch (ii)
                                {
                                    case "add":
                                        {
                                            volo.Args1 = item.Auth ? "true" : "false";
                                        }
                                        break;
                                    case "update":
                                        {
                                            volo.Args2 = item.Auth ? "true" : "false";
                                        }
                                        break;
                                    case "del":
                                        {
                                            volo.Args3 = item.Auth ? "true" : "false";
                                        }
                                        break;
                                    case "detail":
                                        {
                                            volo.Args4 = item.Auth ? "true" : "false";
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        var hasRole = false;
                        //判断权限
                        if (roles != null)
                        {
                            hasRole = roles.Where(x => (x.Model == "root" && x.Role == "root") || (x.Model == item.Model && x.Role == item.Role)).Any();
                        }
                        if (item.Method == "*")
                        {
                            if (hasRole)
                            {
                                volo.Args1 = "true";
                                volo.Args2 = "true";
                                volo.Args3 = "true";
                                volo.Args4 = "true";
                            }
                            else
                            {
                                volo.Args1 = "false";
                                volo.Args2 = "false";
                                volo.Args3 = "false";
                                volo.Args4 = "false";
                            }
                        }
                        else
                        {
                            foreach (var ii in item.Method.Split(','))
                            {
                                switch (ii)
                                {
                                    case "add":
                                        {
                                            volo.Args1 = hasRole ? "true" : "false";
                                        }
                                        break;
                                    case "update":
                                        {
                                            volo.Args2 = hasRole ? "true" : "false";
                                        }
                                        break;
                                    case "del":
                                        {
                                            volo.Args3 = hasRole ? "true" : "false";
                                        }
                                        break;
                                    case "detail":
                                        {
                                            volo.Args4 = hasRole ? "true" : "false";
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                return volo;
            }
            return null;
        }

        /// <summary>
        /// 是否有当前Action的执行权限
        /// </summary>
        /// <param name="action">add/update/del/detail</param>
        /// <param name="roles"></param>
        /// <param name="attrs"></param>
        /// <returns></returns>
        public static (bool auth, string msg) HasRoleFromPasteAuthConfigAttribute(string action, IEnumerable<IPasteAuthRole> roles, List<PasteAuthAttribute> attrs)
        {
            if (attrs?.Any() == true && roles?.Any() == true)
            {
                foreach (var item in attrs)
                {
                    if (String.IsNullOrEmpty(item.Method))
                    {
                        continue;
                    }
                    if (String.IsNullOrEmpty(item.Model) || string.IsNullOrEmpty(item.Role))
                    {
                        if (item.Method.Contains("*") || item.Method.Contains(action))
                        {
                            var auth = roles.Where(x => (x.Model == "root" && x.Role == "root") || (x.Model == item.Model && x.Role == item.Role)).Any();
                            return (auth, $"权限不足，无法继续操作，需要权限 {item.Model}-{item.Role} !");
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        //return item.Auth;
                        return (item.Auth, $"权限不足，无法继续操作，没有配置当前接口 !");
                    }
                }
            }
            return (true, "");
        }

        /// <summary>
        /// 补充enable特性
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="attrs"></param>
        /// <param name="model"></param>
        public static void AutoEnableAttribute(IEnumerable<IPasteAuthRole> roles, List<PasteAuthAttribute> attrs, VoloModelInfo model)
        {
            if (model.Attributes == null) { model.Attributes = new List<VoloModelAttribute>(); }
            if (!model.Attributes.Any(x => x.Name == "enable"))
            {
                var read = ToEnableAttribute(roles, attrs);
                if (read != null)
                {
                    model.Attributes.Add(read);
                }
            }
        }

        /// <summary>
        /// 补充disable特性
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="attrs"></param>
        /// <param name="model"></param>
        public static void AutoDisableAttribute(IEnumerable<IPasteAuthRole> roles, List<PasteAuthAttribute> attrs, VoloModelInfo model)
        {
            if (model.Attributes == null) { model.Attributes = new List<VoloModelAttribute>(); }
            if (!model.Attributes.Any(x => x.Name == "disable"))
            {
                var read = ToEnableAttribute(roles, attrs);
                if (read != null)
                {
                    model.Attributes.Add(read);
                }
            }
        }

        #endregion

    }
}
