namespace PasteApeart.Helper
{

    /*
     * 宏的约定
     * 写法{{:=item.xxx}},比如你可以写打开页面./pasteform/index.html?path=userInfo&role_id={{:=item.id}}
     * 1.一定是上面的格式，也就是{{:=item.}}是固定的，有多方面的考虑，主要是始应table的tr表示当行数据，当条数据
     * 2.取值从form先，如果没有获取到则从query获取
     * 3.如果是table的表格数据，先从当行获取,也只会从当行获取
     */

    /// <summary>
    /// 书签特性 mark
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteMarkAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 书签特性 会在表格头或者表单的名称后面附带一个?图标，点击后直接转到指定位置查看说明，案例等，就是在线文档，需要项目PasteDocument配合！
        /// </summary>
        /// <param name="_model">书签模块,示例product,如果为空表示当前模块</param>
        /// <param name="_value">书签项,示例code,如果为空表示当前字段</param>
        /// <param name="args3">如果这里写popup,那就是PasteMarkPopupAttribute了</param>
        /// <param name="args4">备用</param>
        public PasteMarkAttribute(string _model = null, string _value = null, string args3 = null, string args4 = null)
        {
            base.Name = "mark";
            base.Args1 = _model;
            base.Args2 = _value;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 书签特性 mark的popup形式,就是点击后不跳转到新的页面，就在当前页面对应的位置弹出popup显示说明
    /// 需要到 ../lib/api.js配置global_tap_mark的apiRoute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteMarkPopupAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 书签特性 mark的popup形式,就是点击后不跳转到新的页面，就在当前页面对应的位置弹出popup显示说明
        /// 需要到 ../lib/api.js配置global_tap_mark的apiRoute
        /// </summary>
        /// <param name="_model">书签模块,示例product,如果为空表示当前模块</param>
        /// <param name="_value">书签项,示例code,如果为空表示当前字段</param>
        /// <param name="args3">备用</param>
        /// <param name="args4">备用</param>
        public PasteMarkPopupAttribute(string _model = null, string _value = null, string args3 = "popup", string args4 = null)
        {
            base.Name = "mark";
            base.Args1 = _model;
            base.Args2 = _value;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// Html富文本 richtext
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteRichTextAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// Html富文本 按需加载，会载入富文本，注意这个载入是延迟的，支持获取纯txt内容
        /// </summary>
        /// <param name="_height">UI高度设置</param>
        /// <param name="_txt_save_column">对应的txt提交在哪个字段，对应字段一般设置为hidden</param>
        /// <param name="_image_upload_path">上传图片的api地址</param>
        /// <param name="args4">备用</param>
        public PasteRichTextAttribute(int _height = 0, string _txt_save_column = null, string _image_upload_path = null, string args4 = null)
        {
            base.Name = "richtext";
            if (_height != 0)
            {
                base.Args1 = $"{_height}";
            }
            base.Args2 = _txt_save_column;
            base.Args3 = _image_upload_path;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 字段映射,字段关联，一般用于基于哪个字段查询，所以这个应该标在InputQueryXXXX的字段上，表示实际是哪个字段，保持和Dto一致，比如大写 比如查询user_id映射字段为UserId
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteFieldAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 字段映射,字段关联，一般用于基于哪个字段查询，所以这个应该标在InputQueryXXXX的字段上，表示实际是哪个字段，保持和Dto一致，比如大写 比如查询user_id映射字段为UserId
        /// </summary>
        /// <param name="field">UserId</param>
        /// <param name="operatorName"><![CDATA[查询运算符，注意IN查询用=,like用()示例=/>=/<=/()/>/</!=等]]></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteFieldAttribute(string field, string operatorName = "=", string args3 = null, string args4 = null)
        {
            base.Name = "field";
            base.Args1 = field;
            switch (operatorName)
            {
                case "==":
                case "=": { base.Args2 = "Equals"; } break;
                case "!=": { base.Args2 = "NotEquals"; } break;
                case ">": { base.Args2 = "GreaterThan"; } break;
                case "<": { base.Args2 = "LessThan"; } break;
                case ">=": { base.Args2 = "GreaterThanOrEqual"; } break;
                case "<=": { base.Args2 = "LessThanOrEqual"; } break;
                case "()": { base.Args2 = "Contains"; } break;
                default: { base.Args2 = "Equals"; } break;
            }
            //base.Args2 = operatorName;
            base.Args3 = args3;
            base.Args4 = args4;
            base.IsServerAttribute = true;
        }
    }

    /// <summary>
    /// 排重键，表示这个字段唯一，标记于Entity中，用于更新写入的时候排重判断,你建不建索引是另外一码事，多个之间默认是Or关系，如果要and需要标注PasteBatchMutex
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteMutexAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 服务端特性
        /// 排重键，表示这个字段唯一，标记于Entity中，用于更新写入的时候排重判断,你建不建索引是另外一码事
        /// 如果是多个字段组合排重需要在Entity的Class上标注PasteBatchMutex
        /// </summary>
        /// <param name="args1"></param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteMutexAttribute(string args1 = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "mutex";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
            base.IsServerAttribute = true;
        }
    }

    /// <summary>
    /// 表示新增或者更新的时候，多个字段的关系是And关系，如果是Or关系，则删除当前特性即可
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PasteBatchMutexAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 服务端特性
        /// 排重特性 表示多个字段排重，对应字段上需要标注PasteMutex 
        /// 表示新增或者更新的时候，多个字段的关系是And关系，如果是Or关系，则删除当前特性即可
        /// </summary>
        /// <param name="args1"></param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteBatchMutexAttribute(string args1 = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "batchmutex";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
            base.IsServerAttribute = true;
        }
    }

    /// <summary>
    /// 服务端特性
    /// 外表绑定
    /// 比如为角色分配权限(当前为Role表) 
    /// args1:GradeBindRole 
    /// args2:RoleId 
    /// args3:GradeId 
    /// args4:grade_id(query)
    /// 在InputQueryXXX绑定表示查询扩展，只允许一个
    /// 在ListDto上绑定表示更新绑定数据，允许多个
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PasteExtendBindAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 服务端特性 
        /// 扩展绑定 比如为角色分配权限,当前是权限列表，引入角色id
        /// </summary>
        /// <param name="args1">GradeBindRole(EntityName)</param>
        /// <param name="args2">RoleId(id),表示当前表，比如RoleInfo,那就是ExtendBind也在这个表</param>
        /// <param name="args3">GradeId</param>
        /// <param name="args4">grade_id(来源于参数)</param>
        public PasteExtendBindAttribute(string args1, string args2, string args3, string args4)
        {
            base.Name = "extendbind";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
            base.IsServerAttribute = true;
        }
    }

    /// <summary>
    /// Markdown markdown
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteMarkDownAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// Markdown markdown
        /// </summary>
        /// <param name="_height">UI高度设置</param>
        /// <param name="_html_save_column">对应的html提交在哪个字段，对应字段一般设置为hidden</param>
        /// <param name="_image_upload_path">上传图片的api地址</param>
        /// <param name="args4">备用</param>
        public PasteMarkDownAttribute(int _height = 0, string _html_save_column = null, string _image_upload_path = null, string args4 = null)
        {
            base.Name = "markdown";
            if (_height != 0)
            {
                base.Args1 = $"{_height}";
            }
            base.Args2 = _html_save_column;
            base.Args3 = _image_upload_path;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 日期区间 时间区间，最终要转化成yyyy-MM-dd HH:mm:ss的格式提交，至于前端的显示格式由PasteDateAttribute决定
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteDaterangeAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 日期区间 daterange
        /// </summary>
        /// <param name="_sdate">开始日期字段 默认为sdate</param>
        /// <param name="_edate">结束日期字段 默认为edate</param>
        /// <param name="args3">输入的格式，默认为yyyy-MM-dd HH:mm:ss 可以理解为UI格式</param>
        /// <param name="out_format">输出格式，默认为时间格式不填，你可以填写times时间戳秒和timems时间戳毫秒格式</param>
        public PasteDaterangeAttribute(string _sdate = "sdate", string _edate = "edate", string args3 = null, string out_format = null)
        {
            base.Name = "daterange";
            base.Args1 = _sdate;
            base.Args2 = _edate;
            base.Args3 = args3;
            base.Args4 = out_format;
        }
    }


    /// <summary>
    /// 服务端特性 查询范围 searchrange 有别于时间区间，这个一般用于表示数值类型，比如80-90分成绩，小的包含，大的不包含
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteSearchRangeAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 服务端特性 查询范围 searchrange 有别于时间区间，这个一般用于表示数值类型，比如80-90分成绩，小的包含，大的不包含
        /// </summary>
        /// <param name="_sdate">开始(包含) 默认为minLimit</param>
        /// <param name="_edate">结束(不包含) 默认为maxLimit</param>
        /// <param name="step">表示输入粒度，比如1还是0.01等</param>
        /// <param name="args4"></param>
        public PasteSearchRangeAttribute(string _sdate = "minLimit", string _edate = "maxLimit", string step = "1", string args4 = null)
        {
            base.Name = "searchrange";
            base.Args1 = _sdate;
            base.Args2 = _edate;
            base.Args3 = step;
            base.Args4 = args4;
            base.IsServerAttribute = true;
        }
    }

    /// <summary>
    /// 时间输入
    /// 使用时间输入框进行时间输入,注意提交到后端的格式是严格的yyyy-MM-dd HH:mm:ss
    /// 如果当前对应的字段类型是String，则是格式化的字符串提交，比如你可以设定yyyy-MM-dd
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteDateAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 时间输入 datetime
        /// </summary>
        /// <param name="in_format">输入的格式，默认为yyyy-MM-dd HH:mm:ss 可以理解为UI格式</param>
        /// <param name="out_format">输出的格式times(时间戳秒) timems(时间戳毫秒) 其他(比如yyyy-MM-dd) 可以理解为后端格式</param>
        /// <param name="args3">备用</param>
        /// <param name="args4">备用</param>
        public PasteDateAttribute(string in_format = null, string out_format = null, string args3 = null, string args4 = null)
        {
            base.Name = "datetime";
            base.Args1 = in_format;
            base.Args2 = out_format;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }


    /// <summary>
    /// 使用执行的class表示设定样式，默认赋值样式名称fleft,内置的，表示文字靠左对齐
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteClassAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 使用某一个样式
        /// </summary>
        /// <param name="className">样式名称，默认为fleft</param>
        /// <param name="args2">备用</param>
        /// <param name="args3">备用</param>
        /// <param name="args4">备用</param>
        public PasteClassAttribute(string className = "fleft", string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "class";
            base.Args1 = className;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 针对表格头设计的样式，默认hresize表示列拖拽调整大小
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteThclassAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 针对表格头设计的样式，默认hresize表示列拖拽调整大小
        /// </summary>
        /// <param name="className">样式名称，默认为 hresize</param>
        /// <param name="args2">备用</param>
        /// <param name="args3">备用</param>
        /// <param name="args4">备用</param>
        public PasteThclassAttribute(string className = "hresize", string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "thclass";
            base.Args1 = className;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 使用Switch开关来实现bool类型的显示和操作，替换默认的checkbox
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteSwitchAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// switch显示 和可交互
        /// </summary>
        /// <param name="onlyModel">model=xxxx的时候生效，留空为全部生效</param>
        /// <param name="func">函数名称，没有参数，注意看文档,一般用于改变前提问</param>
        /// <param name="confirm">如果有值，提交前会confirm确认</param>
        /// <param name="remote">list中点击后是否提交api，默认提交，false的时候不提交，一般用于table的edit</param>
        public PasteSwitchAttribute(string onlyModel = "view", bool confirm = false, string func = null, bool remote = true)
        {
            base.Name = "switch";
            base.Args1 = onlyModel;
            base.Args2 = confirm ? "confirm" : "";
            base.Args3 = func;
            base.Args4 = !remote ? "local" : "remote";//默认为空或者remote
        }
    }

    /// <summary>
    /// 同hidden 表示在UI中隐藏这个对象 只是UI隐藏，提交的时候会提交，可以看成是readonly的隐藏版
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteHiddenAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 表示是否隐藏
        /// </summary>
        /// <param name="_unhidden_model">表示某一个模式不隐藏，如果为空表示所有模式都隐藏,在前端输入_config.model查看当前model是啥,支持,分割多个，比如view,bind</param>
        /// <param name="args2">备用</param>
        /// <param name="args3">备用</param>
        /// <param name="args4">备用</param>
        public PasteHiddenAttribute(string _unhidden_model = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "hidden";
            base.Args1 = _unhidden_model;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 按需显示 PasteHidden的权限版本，表示有对应的btnCode的才显示
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteShowAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// btnCode的值！
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 表示是否隐藏
        /// </summary>
        /// <param name="unhiddenModel">不隐藏的model,支持用,分割多个</param>
        /// <param name="args2">备用</param>
        /// <param name="args3">备用</param>
        /// <param name="args4">备用</param>
        /// <param name="btnCode">权限代码 有权限的时候不下发hidden特性</param>
        public PasteShowAttribute(string unhiddenModel = null, string args2 = null, string args3 = null, string args4 = null, string btnCode = null)
        {
            base.Name = "hidden";
            base.Args1 = unhiddenModel;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
            this.Code = btnCode;
        }
    }

    /// <summary>
    /// 完全隐藏 有点像jsonignore,不过这个会把数据返回给UI，但是不会回传，也就是回传是api端的默认值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteHideAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 表示隐藏 完全隐藏，无法被唤醒的那种(ui无数据)，而hidden是有可能被唤醒的(ui有数据)
        /// </summary>
        /// <param name="args1">备用</param>
        /// <param name="args2">备用</param>
        /// <param name="args3">备用</param>
        /// <param name="args4">备用</param>
        public PasteHideAttribute(string args1 = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "hide";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 配置菜单或者是条件菜单
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteMenuAttribute : BtnCodeAttribute
    {

        /// <summary>
        /// 是否在盒子内
        /// </summary>
        public bool InBox { get; set; }


        /// <summary>
        /// 菜单A标签模式
        /// </summary>
        /// <param name="_name">名称</param>
        /// <param name="_script">执行某一个函数</param>
        /// <param name="_iconfont">启用的图标H其实就是class</param>
        /// <param name="_box">是否放置于按钮盒子中</param>
        /// <param name="btnCode">按钮代码</param>
        public PasteMenuAttribute(
            string _name,
            string _script,
            string _iconfont = null,
            bool _box = false,
            string btnCode = null)
        {
            base.Name = "menu";
            base.Args1 = _name;
            base.Args2 = _script;
            base.Args3 = _iconfont;
            if (_box)
            {
                base.Args4 = "box";
            }
            this.Code = btnCode;
            if (InBox)
            {
                base.Args4 = "box";
            }
        }

        /// <summary>
        /// 直接按钮 前端会转化成ifmenu,恒成立的
        /// </summary>
        /// <param name="name">菜单名称</param>
        /// <param name="func">执行函数，示例:func_read_abc({{:=item.id}});</param>
        /// <param name="iconFont">示例：Hui-iconfont-home</param>
        /// <param name="_box"></param>
        /// <param name="className">样式名称</param>
        /// <param name="args4">备用</param>
        /// <param name="btnCode">权限码</param>
        public PasteMenuAttribute(
            string name,
            string func,
            string className,
            string iconFont = null,
            bool _box = false,
            string args4 = null,
            string btnCode = null)
        {
            base.Name = "ifmenu";
            base.Args1 = "1==1";
            base.Args2 = $"<a href=\"javascript:;\" {((!String.IsNullOrEmpty(className)) ? $"class=\"{className}\"" : "")} onclick=\"{func}\">{((!String.IsNullOrEmpty(iconFont)) ? $"<i class=\"Hui-iconfont {iconFont}\">{name}</i>" : $"{name}")}</a>";
            if (_box || InBox)
            {
                //ifmenu是args3
                base.Args3 = "box";
            }
            base.Args4 = args4;
            this.Code = btnCode;
        }
    }

    /// <summary>
    /// HTML模式的菜单，直接用当前字段的值，为空或者有效值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteMenuhtmlAttribute : BtnCodeAttribute
    {
        /// <summary>
        /// 是否在盒子内
        /// </summary>
        public bool InBox { get; set; }

        /// <summary>
        /// HTML模式的菜单，直接用当前字段的值，为空或者有效值
        /// </summary>
        public PasteMenuhtmlAttribute(string btnCode = null)
        {
            base.Name = "menuhtml";
            if (!string.IsNullOrEmpty(btnCode))
            {
                base.Code = btnCode;
            }
            if (InBox)
            {
                base.Args4 = "box";
            }
        }
    }


    /// <summary>
    /// 基于表达式配置菜单 配置菜单或者是条件菜单
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PasteIfMenuAttribute : BtnCodeAttribute
    {

        /// <summary>
        /// 是否在盒子内
        /// </summary>
        public bool InBox { get; set; }


        /// <summary>
        /// 条件按钮
        /// </summary>
        /// <param name="_expresion"><![CDATA[item.isReady && item.stype==1]]></param>
        /// <param name="_insertHtml"><![CDATA[<a href=\"javascript:;\" title=\"如果之前创建失败，修改信息后，可以重置，重置后可以重新创建\" onclick=\"global_store_reset_state({{:=item.id}});\"><i class=\"Hui-iconfont Hui-iconfont-xiangpicha\">重置状态</i></a>]]></param>
        /// <param name="_box">是否在按钮盒子中</param>
        /// <param name="args4">备用</param>
        /// <param name="btnCode">按钮代码</param>
        public PasteIfMenuAttribute(
            string _expresion,
            string _insertHtml,
            bool _box = false,
            string args4 = null,
            string btnCode = null)
        {
            base.Name = "ifmenu";
            base.Args1 = _expresion;
            base.Args2 = _insertHtml;
            if (_box || InBox)
            {
                base.Args3 = "box";
            }
            base.Args4 = args4;
            this.Code = btnCode;
        }

        /// <summary>
        /// 条件模式的直接按钮
        /// </summary>
        /// <param name="expiresion"><![CDATA[item.isReady && item.stype==1]]></param>
        /// <param name="name">菜单名称</param>
        /// <param name="func">执行函数，示例:func_read_abc({{:=item.id}});</param>
        /// <param name="iconFont">示例：Hui-iconfont-home</param>
        /// <param name="_box"></param>
        /// <param name="className">样式名称</param>
        /// <param name="args4">备用</param>
        /// <param name="btnCode">权限码</param>
        public PasteIfMenuAttribute(
            string expiresion,
            string name,
            string func,
            string iconFont = null,
            bool _box = false,
            string className = null,
            string args4 = null,
            string btnCode = null)
        {
            base.Name = "ifmenu";
            base.Args1 = expiresion;
            base.Args2 = $"<a href=\"javascript:;\" {((!String.IsNullOrEmpty(className)) ? $"class=\"{className}\"" : "")} onclick=\"{func}\">{((!String.IsNullOrEmpty(iconFont)) ? $"<i class=\"Hui-iconfont {iconFont}\">{name}</i>" : $"{name}")}</a>";
            if (_box || InBox)
            {
                base.Args3 = "box";
            }
            base.Args4 = args4;
            this.Code = btnCode;
        }
    }



    /// <summary>
    /// 配置菜单或者是条件菜单
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    [Obsolete("Use PasteTdMenu")]
    public class PasteTapAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 菜单按钮
        /// </summary>
        /// <param name="name">按钮的名称</param>
        /// <param name="func_script">执行某一个函数</param>
        /// <param name="class_name">启用的图标H其实就是class</param>
        /// <param name="_box">是否放置于按钮盒子中</param>
        [Obsolete("Use PasteTdMenu")]
        public PasteTapAttribute(string name, string func_script, string class_name = null, bool _box = false)
        {
            base.Name = "tap";
            base.Args1 = name;
            base.Args2 = func_script;
            base.Args3 = class_name;
            if (_box)
            {
                base.Args4 = "box";
            }
        }
    }

    /// <summary>
    /// 批量操作 作用于ListDto中，在表格上左上方显示，左侧会有可选的单选框，表示批量操作
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PasteBatchAttribute : BtnCodeAttribute
    {

        /// <summary>
        /// 批量操作按钮 使用FuncReadBitchCheckedNumber或者FuncReadBitchCheckedString可以读取选中的值
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_script"></param>
        /// <param name="_iconfont"></param>
        /// <param name="title">标题，鼠标放上去的标题</param>
        /// <param name="btnCode">按钮代码</param>
        public PasteBatchAttribute(string _name, string _script, string _iconfont = null, string title = null, string btnCode = null)
        {
            base.Name = "batch";
            base.Args1 = _name;
            base.Args2 = _script;
            base.Args3 = _iconfont;
            base.Args4 = title;
            this.Code = btnCode;
        }
    }

    /// <summary>
    /// 链接参数 作用于Listdto，表示那些参数会传递给表单，比如user_id,打开表格页面，如果没有，用于要新增的时候，会把这个user_id传递给打开的页面
    /// 然后你在对应的AddDto中引入PasteQuery就可以表示给表单默认填充了，所以一般和PasteQuery结合使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PasteLinkQueryAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 需要传递某一个参数给下一级 一般放置于ListDto中，表示把这个参数传递给比如新增表单
        /// </summary>
        /// <param name="_querys">参数名称，示例vname，多个之间使用,隔开</param>
        /// <param name="_args2"></param>
        /// <param name="_args3"></param>
        /// <param name="_args4"></param>
        public PasteLinkQueryAttribute(string _querys, string _args2 = null, string _args3 = null, string _args4 = null)
        {
            base.Name = "linkquery";
            base.Args1 = _querys;
            base.Args2 = _args2;
            base.Args3 = _args3;
            base.Args4 = _args4;
        }
    }

    /// <summary>
    /// 服务端特性 
    /// 链接外表 
    /// 比如UserGrade中的UserId就是链接了UserInfo的Id 标记于Entity中
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteLinkAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 服务端特性 链接外表 比如UserGrade中的UserId就是链接了UserInfo的Id 标记于Entity中
        /// </summary>
        /// <param name="tableName">表名，示例UserInfo</param>
        /// <param name="key"></param>
        /// <param name="_args3"></param>
        /// <param name="_args4"></param>
        public PasteLinkAttribute(string tableName, string key = "Id", string _args3 = null, string _args4 = null)
        {
            base.Name = "link";
            base.Args1 = tableName;
            base.Args2 = key;
            base.Args3 = _args3;
            base.Args4 = _args4;
            base.IsServerAttribute = true;
        }
    }

    /// <summary>
    /// 服务端特性 类似PasteLink这里表示Include表 标记于Entity中
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteIncludeAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 服务端特性 类似PasteLink这里表示Include表 标记于Entity中
        /// </summary>
        /// <param name="tableName">表名，示例UserInfo</param>
        /// <param name="key"></param>
        /// <param name="_args3"></param>
        /// <param name="_args4"></param>
        public PasteIncludeAttribute(string tableName, string key = "Id", string _args3 = null, string _args4 = null)
        {
            base.Name = "include";
            base.Args1 = tableName;
            base.Args2 = key;
            base.Args3 = _args3;
            base.Args4 = _args4;
            base.IsServerAttribute = true;
        }

        /// <summary>
        /// 服务端特性 类似PasteLink这里表示Include表 标记于Entity中
        /// </summary>
        public PasteIncludeAttribute()
        {
            base.Name = "include";
            base.Args1 = null;
            base.Args2 = null;
            base.Args3 = null;
            base.Args4 = null;
            base.IsServerAttribute = true;
        }
    }

    /// <summary>
    /// 读取参数 从页面的querystring中读取数据，可以用于表格页面的搜索区域和表单，注意表单只作用于新建！
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PasteQueryAttribute : ColumnDataTypeAttribute
    {


        /// <summary>
        /// 读取query中的某一个值 前端特性，往往和PasteQueryLink合并使用
        /// </summary>
        /// <param name="_query">某一个参数比如id</param>
        /// <param name="_hide">获取数据后，如果留空表示不隐藏，不留空表示隐藏，建议填写hide</param>
        /// <param name="_args3"></param>
        /// <param name="_args4"></param>
        public PasteQueryAttribute(string _query, string _hide = null, string _args3 = null, string _args4 = null)
        {
            base.Name = "query";
            base.Args1 = _query;
            base.Args2 = _hide;
            base.Args3 = _args3;
            base.Args4 = _args4;
        }

        /// <summary>
        /// 读取query中的某一个值 前端特性，往往和PasteQueryLink合并使用
        /// </summary>
        /// <param name="_query">某一个参数比如id</param>
        /// <param name="hide">获取到数据后是否隐藏</param>
        /// <param name="_args3"></param>
        /// <param name="_args4"></param>
        public PasteQueryAttribute(string _query, bool hide = false, string _args3 = null, string _args4 = null)
        {
            base.Name = "query";
            base.Args1 = _query;
            base.Args2 = hide ? "hide" : "";
            base.Args3 = _args3;
            base.Args4 = _args4;
        }
    }

    /// <summary>
    /// 服务端特性 
    /// 创建字段查询项
    /// 比如用户表，你在Name和Desc都打上这个特性，那么在生成的表格页面中，搜索区域就会多2个输入框，对应Name的检索和Desc的检索,默认是==查询
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteFieldQueryAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 服务端特性 创建字段查询项 比如你在Name上标记，则对应的InputQueryXXX会添加这个Name查询项
        /// </summary>
        /// <param name="args1"></param>
        /// <param name="args2"></param>
        /// <param name="_args3"></param>
        /// <param name="_args4"></param>
        public PasteFieldQueryAttribute(string args1 = null, string args2 = null, string _args3 = null, string _args4 = null)
        {
            base.Name = "filedquery";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = _args3;
            base.Args4 = _args4;
            base.IsServerAttribute = true;
        }
    }

    /// <summary>
    /// 服务端特性 过滤参数 默认word关键字检索的对象，标注在Entity中，需要为string类型的字段 标记于Entity
    /// 模糊查询，查询哪些字段，比如你在Name和Desc都标记，那么页面的关键字查询就会在Name或者Desc字段上基于输入关键字模糊查询
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteSearchAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 服务端特性 标记当前字段要进行搜索 word的字段匹配 标记于Entity
        /// </summary>
        /// <param name="args1"></param>
        /// <param name="args2"></param>
        /// <param name="_args3"></param>
        /// <param name="_args4"></param>
        public PasteSearchAttribute(string args1 = null, string args2 = null, string _args3 = null, string _args4 = null)
        {
            base.Name = "search";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = _args3;
            base.Args4 = _args4;
            base.IsServerAttribute = true;
        }
    }

    /// <summary>
    /// 模板标注 标注使用哪个模板 用于ListDto
    /// 有些页面你要个性化，就可以使用这个引入外部的模板，注意要先使用PasteLoadlib引入html(模板页面)
    /// 然后这里标注用哪个id的模板，这样你可以一个项目设定一个模板页面复用就行了
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteTemplateAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 模板标注 标注使用哪个模板 用于ListDto
        /// </summary>
        /// <param name="headTemplateId">表格头部的模板ID，示例：template_table_head</param>
        /// <param name="bodyTemplateId">表格内容的模板ID，示例：template_table_body</param>
        /// <param name="_args3"></param>
        /// <param name="_args4"></param>
        public PasteTemplateAttribute(string headTemplateId, string bodyTemplateId, string _args3 = null, string _args4 = null)
        {
            base.Name = "template";
            base.Args1 = headTemplateId;
            base.Args2 = bodyTemplateId;
            base.Args3 = _args3;
            base.Args4 = _args4;
        }
    }

    /// <summary>
    /// 标记为textarea 默认textarea是单独一行显示的
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteTextAreaAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 表示使用TextArea来渲染这个内容
        /// </summary>
        /// <param name="args1">表示自定义样式，示例:height:500px;</param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteTextAreaAttribute(string args1 = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "textarea";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 标记为text 主要在于字段没有标注MaxLength或者MaxLength过大，因为默认过大会使用比如富文本或者TextArea进行渲染
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteTextAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 使用Input text来渲染
        /// </summary>
        /// <param name="args1">是否单行独占，示例:true,同理于PasteSingleRow</param>
        /// <param name="args2">表示外样式，示例:menubtn</param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteTextAttribute(string args1 = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "text";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 使用Title 貌似没用了使用了Notice替代
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteTitleAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 使用Title
        /// </summary>
        /// <param name="titleStr">title的内容，主要是为了按钮等特殊的组件用</param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteTitleAttribute(string titleStr, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "title";
            base.Args1 = titleStr;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 为tr写入html代码 可以使用宏 有些时候表格需要为行限定样式，则你需要指定一个字段用于当前行的class名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PasteTrclassAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 为表格的tr标记class，可以使用宏
        /// </summary>
        /// <param name="classStr">字段的名称，示例trclass_name 表示从数据中的trclass_name读取值作为样式名称</param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteTrclassAttribute(string classStr, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "trclass";
            base.Args1 = classStr;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 为tr写入html代码 不再使用默认样式，直接渲染整行tr
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteTrhtmlAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 为tr写入html代码 可以使用宏
        /// </summary>
        /// <param name="htmlStr">示例data-level="{{:=item.level}}"</param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteTrhtmlAttribute(string htmlStr, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "trhtml";
            base.Args1 = htmlStr;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 标记为独占一行 比如PasteLselect默认是半行的，这样标注就会一整行显示了
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteSingleRowAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 标记为独占一行
        /// </summary>
        public PasteSingleRowAttribute(string args1 = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "singlerow";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 标记为半行
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteHalfRowAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 标记为半行 主要针对maxlength大于64的，要半行显示，不然会切换到单行
        /// </summary>
        public PasteHalfRowAttribute(string args1 = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "halfrow";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 服务端特性 
    /// 脱敏处理 
    /// 需要Class先标记，表示这个对象有字段要脱敏 然后字段标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class PasteHalfHideAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 自定义脱敏规则：起始保留长度
        /// </summary>
        public int StartLength { get; set; } = 3;

        /// <summary>
        /// 自定义脱敏规则：末尾保留长度
        /// </summary>
        public int EndLength { get; set; } = 4;

        /// <summary>
        /// 自定义脱敏字符
        /// </summary>
        public char MaskChar { get; set; } = '*';

        /// <summary>
        /// 创建新对象，还是在原值上修改，顶级Class上有效，字段上无效
        /// </summary>
        public bool CreateNewObj { get; set; } = false;

        /// <summary>
        /// 服务端特性 脱敏处理 需要Class先标记，表示这个对象有字段要脱敏 然后字段标记
        /// </summary>
        /// <param name="startLen">开头保持位数</param>
        /// <param name="endLen">尾巴保持位数</param>
        /// <param name="hideChar">默认字段的脱敏字符</param>
        /// <param name="newObj">新对象上处理，否则为原对象上变更</param>
        public PasteHalfHideAttribute(int startLen = 3, int endLen = 4, char hideChar = '*', bool newObj = false)
        {
            base.Name = "halfhide";
            this.StartLength = startLen;
            this.EndLength = endLen;
            this.MaskChar = hideChar;
            this.CreateNewObj = newObj;
            base.IsServerAttribute = true;
        }
    }

    /// <summary>
    /// 用于表格中，显示另外一个字段的某一个值的表达式
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    [Obsolete("建议改用Display")]
    public class PasteOuterDisplayAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 显示某一个 可以使用html 或者 display代替
        /// </summary>
        /// <param name="_expression">示例<![CDATA[extendService?.name || '']]></param>
        /// <param name="args3">备用</param>
        /// <param name="args4">备用</param>
        [Obsolete("建议改用Display")]
        public PasteOuterDisplayAttribute(string _expression, string args3 = null, string args4 = null)
        {
            base.Name = "outerdisplay";
            base.Args2 = _expression;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 禁用 新增 编辑 删除 功能 
    /// 禁用的有值表示近用，没值表示可用，详细反之
    /// 其实还和前端的默认配置有关
    /// 用于ListDto用于给UI使用，用于Entity用于DefaultAppService
    /// </summary>
    [Obsolete("由于容易混淆，建议改用PasteEnable特性")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PasteDisableAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disableAdd">禁用新增</param>
        /// <param name="disableEdit">禁用编辑</param>
        /// <param name="disableRemove">禁用删除</param>
        [Obsolete("由于容易混淆，建议改用PasteEnable特性")]
        public PasteDisableAttribute(bool disableAdd = false, bool disableEdit = false, bool disableRemove = false)
        {
            base.Name = "disable";
            if (disableAdd)
            {
                //禁用新增
                base.Args1 = "add";
            }
            else
            {
                //不禁用新增
                base.Args1 = "false";
            }
            if (disableEdit)
            {
                base.Args2 = "edit";
            }
            else
            {
                base.Args2 = "false";
            }
            if (disableRemove)
            {
                base.Args3 = "del";
            }
            else
            {
                base.Args3 = "false";
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disableAdd">禁用新增</param>
        /// <param name="disableEdit">禁用编辑</param>
        /// <param name="disableRemove">禁用删除</param>
        /// <param name="showDetail">是否有详细</param>
        [Obsolete("由于容易混淆，建议改用PasteEnable特性")]
        public PasteDisableAttribute(bool disableAdd, bool disableEdit, bool disableRemove, bool showDetail)
        {
            base.Name = "disable";
            if (disableAdd)
            {
                base.Args1 = "add";
            }
            else
            {
                base.Args1 = "false";
            }
            if (disableEdit)
            {
                base.Args2 = "edit";
            }
            else
            {
                base.Args2 = "false";
            }
            if (disableRemove)
            {
                base.Args3 = "del";
            }
            else
            {
                base.Args3 = "false";
            }
            if (showDetail)
            {
                base.Args4 = "detail";
            }
            else
            {
                base.Args4 = "false";
            }
        }
    }

    /// <summary>
    /// 是否启用CRUD功能 PasteDisable的反向配置
    /// 没有值的时候表示使用UI的默认配置
    /// </summary>
    public class PasteEnableAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 是否启用CRUD功能 PasteDisable的反向配置
        /// 1启用这个功能
        /// 0不启用这个功能
        /// 其他，忽略，使用前端的默认配置
        /// </summary>
        /// <param name="add">1启用 0禁用 2忽略</param>
        /// <param name="edit">1启用 0禁用 2忽略</param>
        /// <param name="del">1启用 0禁用 2忽略</param>
        /// <param name="detail">1启用 0禁用 2忽略</param>
        public PasteEnableAttribute(int add, int edit, int del, int detail)
        {
            base.Name = "enable";
            if (add == 1 || add == 0)
            {
                base.Args1 = add == 1 ? "true" : "false";
            }
            if (edit == 1 || edit == 0)
            {
                base.Args2 = edit == 1 ? "true" : "false";
            }
            if (del == 1 || del == 0)
            {
                base.Args3 = del == 1 ? "true" : "false";
            }
            if (detail == 1 || detail == 0)
            {
                base.Args4 = detail == 1 ? "true" : "false";
            }
        }

        /// <summary>
        /// 是否启用CRUD功能 PasteDisable的反向配置
        /// 值只能是true/false/(留空)
        /// </summary>
        /// <param name="add">true启用 false禁用 留空忽略</param>
        /// <param name="edit">true启用 false禁用 留空忽略</param>
        /// <param name="del">true启用 false禁用 留空忽略</param>
        /// <param name="detail">true启用 false禁用 留空忽略</param>
        public PasteEnableAttribute(string add, string edit, string del, string detail)
        {
            base.Name = "enable";
            base.Args1 = add;
            base.Args2 = edit;
            base.Args3 = del;
            base.Args4 = detail;
        }

    }


    /// <summary>
    /// 服务端特性 标记于Entity用于标记权限 PasteDisable是UI 这个也标记于ListDto注意要转化给为disable
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PasteAuthAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 方法 示例*/add,update/list,detail,add
        /// </summary>
        public string Method { get; set; } = "*";

        /// <summary>
        /// 是否有权限
        /// </summary>
        public bool Auth { get; set; } = true;

        /// <summary>
        /// 模块
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 服务端特性 当前是总开关，表示所有的，包括root，比如del false表示root也没有删除权限
        /// </summary>
        /// <param name="method">哪些动作，示例:*/add,list等，多个之间使用,隔开</param>
        /// <param name="auth">是否有权限</param>
        public PasteAuthAttribute(string method = "*", bool auth = true)
        {
            base.Name = "auth";
            Method = method;
            Auth = auth;
            base.IsServerAttribute = true;
            //*/add/update/detail/list true 表示都可以使用
            // */add/update/detail/list false 表示都禁止

            //* false 表示root也禁止！！！
            // * true 默认值 表示都有 可以 CRUD
            // * root-root true 表示root才可以CRUD
            // detail,list data-view true 表示detail/list需要权限data-view 才可以使用 否则就是禁止！
        }

        /// <summary>
        /// 示例1：add data-view true
        /// 表示有data-view的有add的权限
        /// 至于update权限，就是默认权限，也就是true
        /// 示例2：del root-root false
        /// 表示都没有删除权限，其他采用默认
        /// </summary>
        /// <param name="method"></param>
        /// <param name="model"></param>
        /// <param name="role"></param>
        public PasteAuthAttribute(string method, string model, string role)
        {
            base.Name = "auth";
            Method = method;
            Model = model;
            Role = role;
            Auth = true;
            base.IsServerAttribute = true;
            //*/add/update/detail/list true 表示都可以使用
            // */add/update/detail/list false 表示都禁止

            //* false 表示root也禁止！！！
            // * true 默认值 表示都有 可以 CRUD
            // * root-root true 表示root才可以CRUD
            // detail,list data-view true 表示detail/list需要权限data-view 才可以使用 否则就是禁止！
        }

        /*
         * detail,list data-view true
         * 表示需要有data-view权限才可以执行detail list权限
         * add:没标记，默认可以执行
         * 
         * add,update root-root true
         * add:标记了，如果当前不是root则不能执行
         * 综上所述，如果不是root 则add update不能执行
         */

        /*
         * * -  false
         * 表示禁用所有，包括root
         * 
         * del root-root true 表示只有root可以执行删除操作
         * del data-view true 表示只有data可以执行删除操作，root一般覆盖了，所以root也是可以的
         */
    }


    ///// <summary>
    ///// 增删改查按钮显示
    ///// </summary>
    //[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    //public class PasteFuncAttribute : ColumnDataTypeAttribute
    //{
    //    /// <summary>
    //    /// 增删改查按钮显示 注意默认是可增，删，改，详细是默认不显示的
    //    /// ui中的名称为disable
    //    /// </summary>
    //    /// <param name="_unadd">禁用新增</param>
    //    /// <param name="_unedit">禁用编辑</param>
    //    /// <param name="_undel">禁用删除</param>
    //    /// <param name="_detail">显示详细</param>
    //    [Obsolete("使用PasteEnable,当前这个即将删除")]
    //    public PasteFuncAttribute(bool _unadd = false, bool _unedit = false, bool _undel = false, bool _detail = false)
    //    {
    //        base.Name = "disable";
    //        if (_unadd)
    //        {
    //            base.Args1 = "add";
    //        }
    //        if (_unedit)
    //        {
    //            base.Args2 = "edit";
    //        }
    //        if (_undel)
    //        {
    //            base.Args3 = "del";
    //        }
    //        if (_detail)
    //        {
    //            base.Args4 = "detail";
    //        }
    //    }
    //}


    /// <summary>
    /// 单位特性 为计量添加单位
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteUnitAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// Unit的特性
        /// </summary>
        /// <param name="_unit">示例元</param>
        /// <param name="args2">备用</param>
        /// <param name="args3">备用</param>
        /// <param name="args4">备用</param>
        public PasteUnitAttribute(string _unit, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "unit";
            base.Args1 = _unit;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 前缀特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PastePrefixAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// Prefix的特性 如果有FenToYuan，要在FenToYuan之后
        /// </summary>
        /// <param name="_prefix">示例￥</param>
        /// <param name="args2">备用</param>
        /// <param name="args3">备用</param>
        /// <param name="args4">备用</param>
        public PastePrefixAttribute(string _prefix, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "prefix";
            base.Args1 = _prefix;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 留空特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteEmptyAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 留空 表示这里显示空 和Hidden不一样，这个占位置
        /// </summary>
        public PasteEmptyAttribute(string args1 = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "empty";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 获取定位信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteLocationAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 获取定位信息 需要坐标和地址信息,详细地址放当前字段，对应wx.chooseLocation
        /// </summary>
        /// <param name="City">城市 放哪个字段</param>
        /// <param name="Longitude">经度放哪个字段，是double类型</param>
        /// <param name="Latitude">纬度放哪个字段，是double类型</param>
        public PasteLocationAttribute(string City = null, string Longitude = "longitude", string Latitude = "latitude")
        {
            base.Name = "location";
            base.Args2 = City;
            base.Args3 = Longitude;
            base.Args4 = Latitude;
        }
    }

    /// <summary>
    /// 获取定位信息 只要坐标信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteGpsAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 获取定位信息 只要坐标信息,对应wx.getLocation
        /// </summary>
        /// <param name="Longitude">经度放哪个字段，是double类型</param>
        /// <param name="Latitude">纬度放哪个字段，是double类型</param>
        public PasteGpsAttribute(string Longitude = "longitude", string Latitude = "latitude")
        {
            base.Name = "gps";
            base.Args3 = Longitude;
            base.Args4 = Latitude;
        }
    }

    /// <summary>
    /// Html特性 如果是枚举类型得，对象是友好名称，如果要获取原值需要添加一个自读get出来！
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteHtmlAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// Html特性 一般用于ListDto中，表示用渲染html，注意比如当前的id,写法为{{:=item.id}}
        /// </summary>
        /// <param name="_html_code">示例：<![CDATA[<div class="idesc">{{:=item.name}}</div>]]></param>
        public PasteHtmlAttribute(string _html_code)
        {
            base.Name = "html";
            base.Args1 = _html_code;
        }
    }



    /// <summary>
    /// Select的特性 或者是Enum的枚举
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteSelectAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// Select的特性 类似于lselect selects
        /// </summary>
        /// <param name="args2"></param>
        /// <param name="args4"></param>
        /// <param name="_selectvalues">示例[{name,value,selected}],选项的Jsonstring,如果是enum类型，这里可以为空</param>
        /// <param name="_ignores">表示要去掉哪些值，在enum的时候适用，比如去掉-1值的这个选项，因为-1我们一般用于查询的时候表示所有都查询使用,多个之间使用,隔开</param>
        public PasteSelectAttribute(string _selectvalues = null, string _ignores = null, string args2 = null, string args4 = null)
        {
            base.Name = "select";
            base.Args1 = _selectvalues;
            base.Args3 = _ignores;
            base.Args2 = args2;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 标记表单提交成功后的后续动作 okaction
    /// 特殊场景下使用 比如新增后，刷新父级页面(父级页面刷新了，当前页面也会自动关闭)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PasteOkactionAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 标记表单提交后的动作 okaction
        /// </summary>
        /// <param name="page">哪个页面 可选值parent,current</param>
        /// <param name="action">执行动作,可选值href,close,replace,open,reload</param>
        /// <param name="url">如果是跳转，则表示url,可是使用{{id}}表示带入当前返回值</param>
        /// <param name="args4"></param>
        public PasteOkactionAttribute(string page = "parent", string action = null, string url = null, string args4 = null)
        {
            base.Name = "okaction";
            base.Args1 = page;
            base.Args2 = action;
            base.Args3 = url;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 以datalist的形式实现输入框，这里主要有一个问题，显示名称和值的冲突问题，目前采用的是名称(值)的形式
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteDatalistAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// Datalist特性
        /// </summary>
        /// <param name="_selectvalues">示例[{name,value,selected}],选项的Jsonstring,如果是enum类型，这里可以为空</param>
        /// <param name="_ignores">表示要去掉哪些值，在enum的时候适用，比如去掉-1值的这个选项，因为-1我们一般用于查询的时候表示所有都查询使用,多个之间使用,隔开</param>
        public PasteDatalistAttribute(string _selectvalues = null, string _ignores = null)
        {
            base.Name = "datalist";
            base.Args1 = _selectvalues;
            base.Args3 = _ignores;
        }
    }


    /// <summary>
    /// 横向平铺选择 可以理解成select的ul,li模式,支持单选多选等
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteLselectAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// Select的特性 类似于lselect selects
        /// </summary>
        /// <param name="_selectvalues">示例[{name,value,selected}],选项的Jsonstring,如果是enum类型，这里可以为空</param>
        /// <param name="_ignores">表示要去掉哪些值，在enum的时候适用，比如去掉-1值的这个选项，因为-1我们一般用于查询的时候表示所有都查询使用,多个之间使用,隔开</param>
        /// <param name="_num">允许选择多个，如果是0则不限</param>
        public PasteLselectAttribute(string _selectvalues, string _ignores, int _num = 0)
        {
            base.Name = "lselect";
            base.Args1 = _selectvalues;
            base.Args3 = _ignores;
            if (_num > 0)
            {
                base.Args4 = _num.ToString();
            }
        }

        /// <summary>
        /// 使用枚举类型，快速构建
        /// </summary>
        /// <param name="num">目标选择个数</param>
        public PasteLselectAttribute(int num = 1)
        {
            base.Name = "lselect";
            base.Args1 = null;
            base.Args3 = null;
            if (num > 0)
            {
                base.Args4 = num.ToString();
            }
        }
    }

    /// <summary>
    /// Select的特性 或者是Enum的枚举
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteSelectsAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// Select的特性 类似于lselect selects
        /// </summary>
        /// <param name="_selectvalues">示例[{name,value,selected}],选项的Jsonstring,如果是enum类型，这里可以为空</param>
        /// <param name="_ignores">表示要去掉哪些值，在enum的时候适用，比如去掉-1值的这个选项，因为-1我们一般用于查询的时候表示所有都查询使用,多个之间使用,隔开</param>
        /// <param name="_num">允许选择多个，如果是0则不限</param>
        /// <param name="_splitchar">返回值用什么分割</param>
        public PasteSelectsAttribute(string _selectvalues = null, string _splitchar = ",", string _ignores = null, int _num = 0)
        {
            base.Name = "selects";
            base.Args1 = _selectvalues;
            base.Args2 = _splitchar;
            base.Args3 = _ignores;
            if (_num > 0)
            {
                base.Args4 = _num.ToString();
            }
        }
    }

    /// <summary>
    /// 表示忽略这个字段，和JsonIgnore有点区别，JsonIgnore包含这个，
    /// 如果标记这个的话，后端还是可以接收json的，只是我返回给前端的时候不会有这个字段的信息
    /// 所以基于实际情况使用吧！
    /// 如果标记于Class上，表示这个Class不进行比如Desc等的处理(这里说的是PasteBuilder工具)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class PasteIgnoreAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 表示忽略这个字段，和JsonIgnore有点区别，JsonIgnore包含这个，
        /// 如果标记这个的话，后端还是可以接收json的，只是我返回给前端的时候不会有这个字段的信息
        /// 所以基于实际情况使用吧！
        /// 如果标记于Class上，表示这个Class不进行比如Desc等的处理(这里说的是PasteBuilder工具)
        /// </summary>
        /// <param name="args1"></param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteIgnoreAttribute(string args1 = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "ignore";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
            base.IsServerAttribute = true;
        }
    }


    /// <summary>
    /// Image的特性 加一个PasteImageselectAttribute则表示从图库选择
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteImageAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// Image的特性 加一个PasteImageselectAttribute则表示从图库选择
        /// </summary>
        /// <param name="_num">可以传多少张默认1</param>
        /// <param name="_path">存放的路径示例icon</param>
        /// <param name="_size">转化的大小,示例60*60,1920*0</param>
        /// <param name="className">样式名称，默认为空</param>
        public PasteImageAttribute(int _num = 1, string _path = null, string _size = null, string className = null)
        {
            base.Name = "image";
            base.Args1 = _num.ToString();
            base.Args2 = _path;
            base.Args3 = _size;
            base.Args4 = className;
        }
    }

    /// <summary>
    /// simg 图片选择库选择 simg 需要配合PasteLoablib需要引入./lib/img_drag.js
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteImageSelectAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// simg 使用图片选择器选择 simg
        /// </summary>
        /// <param name="args1">路径，选择页面得路径，默认是./images/index.html</param>
        /// <param name="args2"><![CDATA[参数，支持宏，示例?type=head&cid={{:=item.id}}]]></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteImageSelectAttribute(string args1 = "./images/index.html", string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "simg";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// imgexpend 针对mediaid和url的情况 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    [Obsolete("使用PasteImage和服务端自行处理，不需要前端处理")]
    public class PasteImageExpendAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// imgexpend 针对mediaid和url的情况
        /// </summary>
        /// <param name="mediaId">表示表单中的图片的实际值</param>
        /// <param name="url">可以不填写，默认value就是url,表示图片的预览地址，一般用相对地址就行</param>
        /// <param name="field">值字段，默认为url,表示mediaid的字段名称，需要上传返回值支持</param>
        /// <param name="args4"></param>
        [Obsolete("使用PasteImage和服务端自行处理，不需要前端处理")]
        public PasteImageExpendAttribute(string mediaId = null, string url = null, string field = "url", string args4 = null)
        {
            base.Name = "imgexpend";
            base.Args1 = mediaId;
            base.Args2 = url;
            base.Args3 = field;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// outer 外表选择 比如用户绑定角色，这个时候我们希望userid的输入，就是打开用户列表选择一个用户，然后返回userid和昵称写入对应的输入框
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteOuterAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// outer 链接外表显示和输入用
        /// </summary>
        /// <param name="path"><![CDATA[外表名称，示例serviceInfo,也可以是路径(路径注意model=select表示选择模式)，示例：./pasteform/index.html?path=xxxx&model=select&is_top=1，支持宏模式]]></param>
        /// <param name="extendName">显示的时候的表达式，示例extendService,这个时候一般用于UpdateDto的时候，因为一个字段表示数据userId，一个字段表示数据extendUser</param>
        /// <param name="_keyColumn">回传的列的名称，示例id,多个支持用,隔开，第一个是主要的，后面的会寻找对应字段进行赋值</param>
        /// <param name="_showColumn">显示的列表的名称，示例name,新版本应该支持+拼接，如果中间的不是有效属性则原样输出</param>
        public PasteOuterAttribute(string path, string extendName = null, string _keyColumn = "id", string _showColumn = "name")
        {
            base.Name = "outer";
            base.Args1 = path;
            base.Args2 = extendName;
            base.Args3 = _keyColumn;
            base.Args4 = _showColumn;
        }
    }

    /// <summary>
    /// short 服务端特性 为字段扩展外表
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteShortAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// short 服务端特性 为字段扩写外表 需要标注在外表Key，比如UserId
        /// </summary>
        /// <param name="table_name">表名，示例:UserInfo,作为_dbContext的对象</param>
        /// <param name="outer_field">某一个字段，外键值字段，示例:ExtendUser</param>
        /// <param name="expression">扩展表达式,示例：ToUserShort()</param>
        /// <param name="args4">预留字段</param>
        public PasteShortAttribute(string table_name, string outer_field, string expression, string args4 = null)
        {
            base.Name = "short";
            base.Args1 = table_name;
            base.Args2 = outer_field;
            base.Args3 = expression;
            base.Args4 = args4;
            base.IsServerAttribute = true;
        }
    }

    /// <summary>
    /// shortmodel 服务端特性 为当前Model创建一个ShortModel
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PasteShortModelAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// shortmodel 服务端特性 生成ShortModel 按照规则名称自动生成Info会被替换空，默认主键Id,默认显示Name
        /// </summary>
        public PasteShortModelAttribute()
        {
            base.Name = "shortmodel";
            base.Args1 = null;
            base.Args2 = null;
            base.Args3 = "Id";
            base.Args4 = "Name";
            base.IsServerAttribute = true;
        }

        /// <summary>
        /// 表示为当前Model创建一个ShortModel
        /// </summary>
        /// <param name="field_id">主键字段，示例：Id</param>
        /// <param name="func_name">转换函数 示例：ToUserShort,注意没有后面的括号</param>
        /// <param name="field_name">显示字段，示例：Name</param>
        /// <param name="short_name">Model名称，示例：UserShort</param>
        public PasteShortModelAttribute(string short_name, string func_name, string field_id = "Id", string field_name = "Name")
        {
            base.Name = "shortmodel";
            base.Args1 = short_name;
            base.Args2 = func_name;
            base.Args3 = field_id;
            base.Args4 = field_name;
            base.IsServerAttribute = true;
        }
    }


    /// <summary>
    /// 必填限制 这个和官方的有区别的，官方的包含这个，这个的是前端进行必填校验，而后端不校验！可以和Changeui配合使用，可以实现按需必填！
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteRequiredAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// Required 如果用原生的，有些数据类型是不支持的，如果是int表示非0注意！
        /// </summary>
        /// <param name="modelName">不填写默认全模式Required,填写表示只有这个模式配置</param>
        public PasteRequiredAttribute(string modelName = null)
        {
            base.Name = "required";
            base.Args1 = modelName;
        }
    }

    /// <summary>
    /// 字段排序 默认为0不会排序，小于0得先排序，然后是大于0的，前端返回在sort字段中
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteSortAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 字段排序 默认为0，小的排前面
        /// </summary>
        public PasteSortAttribute(int sort = 0)
        {
            base.Name = "sort";
            base.Args1 = sort.ToString();
        }
    }


    /// <summary>
    /// score 评分 5星评分 需要和PasteLoablib引入./lib/star_score.js
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteScoreAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// score 评分
        /// </summary>
        /// <param name="score">最大多少分</param>
        public PasteScoreAttribute(int score = 5)
        {
            base.Name = "score";
            base.Args1 = score.ToString();
        }
    }

    /// <summary>
    /// 字段统计 用于Sum计算，可以用于UI的当前页，和后续的后端的基于查询统计
    /// 用于表格中某一列的值的统计，体现在表格页面就是某一列当前页面的合计是多少，支持多个字段都标记
    /// 如果是基于查询的，你得返回PasteTotal，也就是返回类型改成PastePagedto
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteSumAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 字段统计 用于Sum计算，可以用于UI的当前页，和后续的后端的基于查询统计
        /// </summary>
        /// <param name="args1"></param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteSumAttribute(string args1 = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "sum";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 要点字段 打重点，注意点，非必填
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PastePointAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 要点字段 打重点，注意点，非必填
        /// </summary>
        public PastePointAttribute(string args1 = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "point";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 只读模式 一般体现在表单中，既要给用户看，又不要用户修改，注意需要后端自己校验用户是否修改了！
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteReadOnlyAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 只读模式 更新的时候需要自行处理，否则客户端F12后还是可以修改的
        /// </summary>
        public PasteReadOnlyAttribute()
        {
            base.Name = "readonly";
        }
    }


    /// <summary>
    /// 外表选择 类似PasteOuter，不过这个是多选模式，选择多个
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteOutersAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 链接外表显示和输入用
        /// </summary>
        /// <param name="path">外表名称，示例serviceInfo也可以是路径，比如/api/app/bac/readListModel</param>
        /// <param name="extendName">显示的时候的表达式，示例extendService</param>
        /// <param name="_keyColumn">回传的列的名称，示例id</param>
        /// <param name="_showColumn">显示的列表的名称，示例name,新版本应该支持+拼接，如果中间的不是有效属性则原样输出</param>
        public PasteOutersAttribute(string path, string extendName = null, string _keyColumn = "id", string _showColumn = "name")
        {
            base.Name = "outers";
            base.Args1 = path;
            base.Args2 = extendName;
            base.Args3 = _keyColumn;
            base.Args4 = _showColumn;
        }
    }

    /// <summary>
    /// 创建对象 可以理解成扩展信息，有自己的表单信息的，比如新增用户的时候，你要给他一个配置表信息，则用这个会输入配置的object信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteObjectAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 通过一个模型，使用表单进行对象输入
        /// </summary>
        /// <param name="path">外表名称，示例serviceInfo</param>
        /// <param name="extendName">这个留空</param>
        /// <param name="_keyColumn">回传的列的名称，示例id</param>
        /// <param name="_showColumn">显示的列表的名称，示例name,新版本应该支持+拼接，如果中间的不是有效属性则原样输出</param>
        public PasteObjectAttribute(string path, string extendName = null, string _keyColumn = "id", string _showColumn = "name")
        {
            base.Name = "object";
            base.Args1 = path;
            base.Args2 = extendName;
            base.Args3 = _keyColumn;
            base.Args4 = _showColumn;
        }
    }

    /// <summary>
    /// 创建对象 PasteObject的多个模式，表示返回多个
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteObjectsAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 通过一个模型，使用表单进行对象输入
        /// </summary>
        /// <param name="path">外表名称，示例serviceInfo</param>
        /// <param name="extendName">这个留空</param>
        /// <param name="_keyColumn">回传的列的名称，示例id,这个ID比较特殊，支持+拼接，也就是多字段排重</param>
        /// <param name="_showColumn">显示的列表的名称，示例name,支持+拼接，比如https://+host+:+port</param>
        public PasteObjectsAttribute(string path, string extendName = null, string _keyColumn = "id", string _showColumn = "name")
        {
            base.Name = "objects";
            base.Args1 = path;
            base.Args2 = extendName;
            base.Args3 = _keyColumn;
            base.Args4 = _showColumn;
        }
    }

    /// <summary>
    /// ListDto中用于排序 是查询的那个排序，目前只支持ACS和DESC，比如UserId，那么你就是输入UserId 和UserId desc
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteOrderbyAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 排序 不写表示当前字段的默认值，小写开头(ui要判断样式) 比如nickName和nickName desc
        /// </summary>
        /// <param name="acsName">升序 示例:nickName,不写默认为当前字段升序</param>
        /// <param name="descName">降序 示例:nickName desc，不写为当前字段降序</param>
        public PasteOrderbyAttribute(string acsName = null, string descName = null)
        {
            base.Name = "orderby";
            base.Args1 = acsName;
            base.Args2 = descName;
        }
    }

    /// <summary>
    /// 按钮 注意Property上只能单个 Class上可以多个，表示功能按钮
    /// 如果在Class上，需要做权限按钮，需要配合Order使用，先禁用这个功能，然后写入这个按钮
    /// 也就是PasteEnable中，比如配置add为0，然后配置PasteButton中的code为add
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class PasteButtonAttribute : BtnCodeAttribute
    {

        /// <summary>
        /// 显示为按钮 表单的提交按钮的文本 后续LIST中的搜索区域也将支持
        /// </summary>
        /// <param name="_name">按钮名称(Class的时候为最下方的提交按钮的文本),示例:保存</param>
        /// <param name="_func">点击后执行的函数(Property的时候),示例:global_tap_regex(this);,支持宏</param>
        /// <param name="_code">add表示替换掉表格页面的新增按钮,如果配置了add,且为btnCode模式，则PasteEnable需要在之前</param>
        /// <param name="title">标题，鼠标放上去的标题</param>
        /// <param name="btnCode">按钮权限的代码</param>
        public PasteButtonAttribute(
            string _name,
            string _func,
            string _code = null,
            string title = null,
            string btnCode = null)
        {
            base.Name = "button";
            base.Args1 = _name;
            base.Args2 = _func;
            base.Args3 = _code;
            base.Args4 = title;
            this.Code = btnCode;
        }
    }

    /// <summary>
    /// 编辑模式后的小按钮
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteMinbtnAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 编辑模式后的小按钮 估计用html模式更可靠
        /// </summary>
        /// <param name="name">按钮名称示例:保存</param>
        /// <param name="func">自定义函数，留空表示func_min_btn_click(this,id,name)</param>
        /// <param name="class_name">样式名称</param>
        /// <param name="_args4">自定义参数</param>
        public PasteMinbtnAttribute(string name, string func = null, string class_name = null, string _args4 = null)
        {
            base.Name = "minbtn";
            base.Args1 = name;
            base.Args2 = func;
            base.Args3 = class_name;
            base.Args4 = _args4;
        }
    }

    /// <summary>
    /// 导出忽略 导出的时候，忽略这个字段 一般用于ListDto中的字段中 ，导出约定，菜单区域不导出，然后是这个特性的不导出
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteOutignoreAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 导出忽略 导出的时候，忽略这个字段 一般用于ListDto中的字段中 ，导出约定，菜单区域不导出，然后是这个特性的不导出
        /// 导出约定 没有名称的，菜单区域的 配置了当前特性的 都不导出
        /// </summary>
        public PasteOutignoreAttribute()
        {
            base.Name = "outignore";
        }
    }


    /// <summary>
    /// 样式内容
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteStyleAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 样式的内容 直接写样式，比如height:300px;
        /// </summary>
        /// <param name="_style">height:300px;</param>
        public PasteStyleAttribute(string _style)
        {
            base.Name = "style";
            base.Args1 = _style;
        }
    }

    /// <summary>
    /// 输入为数组 用于文本输入后的拆分，比如int[] long[] string[]等的显示和编辑
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteSplitAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 文本输入拆分为数组splitarray 一般用于要使用输入框输入数组的时候使用！
        /// </summary>
        /// <param name="_split_char"></param>
        public PasteSplitAttribute(string _split_char = ",")
        {
            base.Name = "splitarray";
            base.Args1 = _split_char;
        }
    }

    /// <summary>
    /// 密码框模式
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PastePasswordAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 密码框模式
        /// </summary>
        /// <param name="pass_char"></param>
        public PastePasswordAttribute(string pass_char = "*")
        {
            base.Name = "password";
            base.Args1 = pass_char;
        }
    }

    /// <summary>
    /// 上传文件 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteFileAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 上传文件 默认上传于/api/app/Upload/UpFile
        /// </summary>
        /// <param name="_path">上传路径，默认值在前端,示例:/api/app/Upload/UpFile</param>
        /// <param name="_dir">上传的参数type一般用于存放哪个位置使用</param>
        /// <param name="_func">或者走自己的上传逻辑，一个函数示例global_file_upload(this);</param>
        /// <param name="filter">选择文件过滤，默认为*.*表示所有</param>
        public PasteFileAttribute(string _path = null, string _dir = "file", string _func = null, string filter = "*.*")
        {
            base.Name = "file";
            base.Args1 = _path;
            base.Args2 = _dir;
            base.Args3 = _func;
            base.Args4 = filter;
        }
    }

    /// <summary>
    /// 显示字段 当前字段是一个对象，显示这个对象中的某一个字段，比如用户简要信息，显示昵称，你总不能UserId显示出来，那样体验很差
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteDisplayAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 显示字段 一般用于ListDto 当前为object类型，显示当前字段对象的某一个字段
        /// list detail
        /// </summary>
        /// <param name="_column">显示对象的哪个字段，默认为name,注意是前端的，所以开头小写</param>
        public PasteDisplayAttribute(string _column = "name")
        {
            base.Name = "display";
            base.Args1 = _column;
        }
    }

    /// <summary>
    /// 分转元 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteFenToYuanAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 前端进行转化 后端单位是分，前端单位为元，进行换算 注意要在prefix和unit之前标注这个特性
        /// list detail
        /// </summary>
        public PasteFenToYuanAttribute()
        {
            base.Name = "fentoyuan";
        }
    }

    /// <summary>
    /// 米转千米 m转KM 用于距离显示 tokm 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteToKmAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 米转千米 m转KM 用于距离显示 tokm 
        /// </summary>
        /// <param name="prefix">前缀，如果空表示直接转化，≈表示约值</param>
        /// <param name="unit">单位 km/千米/公里</param>
        /// <param name="digit">保留小数位数</param>
        /// <param name="args4"></param>
        public PasteToKmAttribute(string prefix = "≈", string unit = "KM", int digit = 2, string args4 = null)
        {
            base.Name = "tokm";
            Args1 = prefix;
            Args2 = unit;
            Args3 = digit.ToString();
            Args4 = args4;
        }
    }

    /// <summary>
    /// 字节转大单位 比如byte 转MB
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteToSizeAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// 字节转大单位 比如byte 转MB
        /// </summary>
        /// <param name="prefix">当前单位 b kb mb gb 全部小写</param>
        /// <param name="unit">单位 目标单位 KB MB GB 会显示最靠近得单位，比如设定GB，但是数据只有MB，则显示MB 如果到PB，则继续显示GB</param>
        /// <param name="digit">保留小数位数</param>
        /// <param name="args4"></param>
        public PasteToSizeAttribute(string prefix = "b", string unit = "GB", int digit = 2, string args4 = null)
        {
            base.Name = "tosize";
            Args1 = prefix;
            Args2 = unit;
            Args3 = digit.ToString();
            Args4 = args4;
        }
    }


    /// <summary>
    /// 分组 用于Class 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PasteGroupAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 分组
        /// 如果和PasteDirectSun一起用，如果是数组，code的是xxxx__0这样的格式，这样才能基于当前新增一个组
        /// </summary>
        /// <param name="_code">代码，唯一，示例:gr1</param>
        /// <param name="_name">名称，示例:基础配置</param>
        /// <param name="_open">是否展开,默认展开</param>
        /// <param name="args4">留空，如果是edit表示可以基于当前版本创建一个新的，需要为数组模式</param>
        public PasteGroupAttribute(string _code, string _name, bool _open = true, string args4 = null)
        {
            base.Name = "group";
            base.Args1 = _code;
            base.Args2 = _name;
            base.Args3 = !_open ? "hide" : "open";
            base.Args4 = args4;
        }
        /// <summary>
        /// Class不写的时候 要用这种，命中第一个作为组
        /// 如果和PasteDirectSun一起用，如果是数组，code的是xxxx__0这样的格式，这样才能基于当前新增一个组
        /// </summary>
        /// <param name="_code"></param>
        /// <param name="_name"></param>
        /// <param name="state">open close hidden</param>
        /// <param name="args4">留空，如果是edit表示可以基于当前版本创建一个新的，需要为数组模式</param>
        public PasteGroupAttribute(string _code, string _name, string state, string args4 = null)
        {
            base.Name = "group";
            base.Args1 = _code;
            base.Args2 = _name;
            base.Args3 = state;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 分组 用于Property 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteGroupItemAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 分组 Class写的时候可以用这个
        /// 如果和PasteDirectSun一起用，如果是数组，code的是xxxx__0这样的格式，这样才能基于当前新增一个组
        /// </summary>
        /// <param name="_code">代码,需要和Class中的特性呼应，示例:gr1</param>
        public PasteGroupItemAttribute(string _code)
        {
            base.Name = "group";
            base.Args1 = _code;
        }
        /// <summary>
        /// Class不写的时候 要用这种，命中第一个作为组
        /// 如果和PasteDirectSun一起用，如果是数组，code的是xxxx__0这样的格式，这样才能基于当前新增一个组
        /// </summary>
        /// <param name="_code"></param>
        /// <param name="_name"></param>
        /// <param name="_open"></param>
        /// <param name="args4">如果是edit表示可以基于当前版本创建一个新的，需要为数组模式</param>
        public PasteGroupItemAttribute(string _code, string _name, bool _open = true, string args4 = null)
        {
            base.Name = "group";
            base.Args1 = _code;
            base.Args2 = _name;
            base.Args3 = !_open ? "hide" : "open";
            base.Args4 = args4;
        }
        /// <summary>
        /// Class不写的时候 要用这种，命中第一个作为组
        /// 如果和PasteDirectSun一起用，如果是数组，code的是xxxx__0这样的格式，这样才能基于当前新增一个组
        /// </summary>
        /// <param name="_code"></param>
        /// <param name="_name"></param>
        /// <param name="state">open close hidden</param>
        /// <param name="args4">如果是edit表示可以基于当前版本创建一个新的，需要为数组模式</param>
        public PasteGroupItemAttribute(string _code, string _name, string state, string args4 = null)
        {
            base.Name = "group";
            base.Args1 = _code;
            base.Args2 = _name;
            base.Args3 = state;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 以iframe的模式编辑子内容 最后会以object的形式放回当前字段中 页面需要实现对应函数window.funcSubmitForm，可以参考sku板块的案例
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteSunformAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// iframe模式的子对象
        /// </summary>
        /// <param name="path"><![CDATA[外表名称，示例serviceInfo也可以是./view.html?path=xxx&model=sunobj,支持{{:=item.id}}动态传参，不过得是默认值或者是query来得，否则可能会是空值]]></param>
        /// <param name="height">高度，默认为300</param>
        /// <param name="class_name">css的样式名称</param>
        /// <param name="args4"></param>
        public PasteSunformAttribute(string path, int height = 300, string class_name = null, string args4 = null)
        {
            base.Name = "sunform";
            base.Args1 = path;
            base.Args2 = height.ToString();
            base.Args3 = class_name;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 重载单项 这个特殊情况下才使用，需要注意服务端要返回当个表单的对象，比如你选择某一个类，你可以在服务端读取对应信息，返回到某一个表单项中
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteLoaditemAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 当前表示outer选择后，重新载入某一项，项由API返回
        /// </summary>
        /// <param name="api"><![CDATA[示例:/api/app/xxx/yyy?id={{:=item.xxx}}]]></param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteLoaditemAttribute(string api = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "loaditem";
            base.Args1 = api;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 多级分拆 注意如果不注释，则不分组，也就是Description特性,比如订阅消息有多个字段，你可以把字段配置成一个object然后配置这个，就可以在一个页面直接编辑了
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteDirectsunAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 表示当前字段按照__分割层级关系，动态表单支持！
        /// </summary>
        /// <param name="args1"></param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteDirectsunAttribute(string args1 = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "directsun";
            base.Args1 = args1;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 游标分页 如果是非page.size分页的，那就是游标分页，用这个标记，就会在请求的时候，返回nextkey获取下一页数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteNextkeyAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 仅支持表格页面的搜索项的字段特性，表示按照游标分页
        /// </summary>
        /// <param name="field">返回的字段，默认就是当前，也可以配置不一样，这里的返回是指查询后的数据，默认是这样的格式.next_key下一页游标.items表示数据</param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteNextkeyAttribute(string field = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "next_key";
            base.Args1 = field;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 编辑模式 针对表格页面，表示某些列直接就是编辑模式，提交你要自己实现，可以调用getSelectedRowsData获取当前值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteEditAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 表示表格中的内容是否是可编辑模式，直接就是可操作的select input chekbox等,args1-args4表示哪个模式下为可编辑，可以任意填写
        /// </summary>
        /// <param name="args1">默认view</param>
        /// <param name="placeholder">placeholder注释内容</param>
        /// <param name="className">className 样式名称</param>
        /// <param name="args4"></param>
        public PasteEditAttribute(string args1 = "view", string placeholder = null, string className = null, string args4 = null)
        {
            base.Name = "edit";
            base.Args1 = args1;
            base.Args2 = placeholder;
            base.Args3 = className;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 载入资源 提前载入某些资源，可以是css/js/html等，一般用于自定义实现一些功能！
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PasteLoadlibAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 载入js或者css等资源
        /// </summary>
        /// <param name="src">载入的文件名，示例/lib/xxx.js或者https://www.xxx.yyy.com/lib/exx.css等</param>
        /// <param name="func">载入后执行哪个函数，注意函数没有参数，示例handlerStart</param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteLoadlibAttribute(string src, string func = null, string args3 = null, string args4 = null)
        {
            base.Name = "loadlib";
            base.Args1 = src;
            base.Args2 = func;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }


    /// <summary>
    /// 选中后，用于隐藏或者显示其他项,比如案例中的，权限列表，如果是权限则只需要模块和权限，如果是菜单则只显示路径，就是这个实现的！
    /// 配置对象可以是PasteSelect PasteLselect,一般用于PasteLselect比较合理！
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PasteChangeuiAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 选中后，用于隐藏或者显示其他项
        /// </summary>
        /// <param name="val">选中的值，比如选中的是1,多个之间使用,隔开，比如1,3</param>
        /// <param name="shows">命中要显示哪些字段，没命中则隐藏这些字段，比如name,多个之间使用,隔开，regex:开头表示正则模式,group:regex:表示组别的正则，group:表示组别</param>
        /// <param name="args3">备用</param>
        /// <param name="args4">备用</param>
        public PasteChangeuiAttribute(string val, string shows = null, string args3 = null, string args4 = null)
        {
            base.Name = "changeui";
            base.Args1 = val;
            base.Args2 = shows;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 页面配置 前端页面默认是支持query模式的，这个可以说是一个扩展吧，query模式侧重前端控制，这个侧重后端控制
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PasteConfigAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 页面配置 修改页面得config配置
        /// </summary>
        /// <param name="jsonString">传入的是配置得jsonString,示例{"drag_order_column":true}</param>
        /// <param name="args2">预留：key</param>
        /// <param name="args3">备用：val</param>
        /// <param name="args4">备用：datatype</param>
        public PasteConfigAttribute(string jsonString, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "config";
            base.Args1 = jsonString;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 配置默认以图表显示还是表格显示，还是2个都可以显示 图表echarts显示数据还是表格显示数据 相关的是PasteChart...
    /// </summary>
    [Obsolete("Use PasteChartui")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PasteUIAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 以什么方式显示数据 这个用于ListDto 谁写前面就默认用哪个显示，也可以使用query中的show_type来传值
        /// </summary>
        /// <param name="first">默认为table/chart表格形式</param>
        /// <param name="sedond">可选方式，不能2个都一样，单个显示的时候这个参数留空 chart</param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        [Obsolete("Use PasteChartui")]
        public PasteUIAttribute(string first = "table", string sedond = null, string args3 = null, string args4 = null)
        {
            base.Name = "ui";
            base.Args1 = first;
            base.Args2 = sedond;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 配置默认以图表显示还是表格显示，还是2个都可以显示 图表echarts显示数据还是表格显示数据 相关的是PasteChart...
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PasteChartuiAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 以什么方式显示数据 这个用于ListDto 谁写前面就默认用哪个显示，也可以使用query中的show_type来传值
        /// </summary>
        /// <param name="first">默认为table/chart表格形式</param>
        /// <param name="sedond">可选方式，不能2个都一样，单个显示的时候这个参数留空 chart</param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteChartuiAttribute(string first = "table", string sedond = null, string args3 = null, string args4 = null)
        {
            base.Name = "chartui";
            base.Args1 = first;
            base.Args2 = sedond;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 值类型，特别用于前端要推测类型的时候，比如EnumRoleType[] 这个时候没办法从value推测，因为可能为null，就需要这个辅助
    /// 可选值有 number string 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteValuetypeAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 值类型，特别用于前端要推测类型的时候，比如EnumRoleType[] 这个时候没办法从value推测，因为可能为null，就需要这个辅助
        /// </summary>
        /// <param name="valueType">值类型，是单个的，可选number/string/boolean</param>
        public PasteValuetypeAttribute(string valueType = "number")
        {
            base.Name = "valuetype";
            base.Args1 = valueType;
        }
    }

    /// <summary>
    /// 图表显示的时候的X轴，就是横轴，一般是时间轴
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteChartxAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 图表显示的时候的X轴，就是横轴，一般是时间轴
        /// </summary>
        /// <param name="title">标题，不填则用当前字段的字段标题</param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        public PasteChartxAttribute(string title = null, string args2 = null, string args3 = null, string args4 = null)
        {
            base.Name = "chartx";
            base.Args1 = title;
            base.Args2 = args2;
            base.Args3 = args3;
            base.Args4 = args4;
        }
    }

    /// <summary>
    /// 图表显示的时候的Y轴,左右数据标柱那个，示例单量，金额
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PasteChartyAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 图表显示的时候的Y轴,比如数量，比如金额，一个Y轴可以给多个数据使用,注意yzinde的顺序就是打特性的顺序
        /// </summary>
        /// <param name="name">显示名称，比如数量，还是金额，不填直接读取本字段的</param>
        /// <param name="position">左边left 右边right</param>
        /// <param name="offset">偏移位置</param>
        /// <param name="formatter"><![CDATA[数据格式,比如{value}个]]></param>
        public PasteChartyAttribute(string name = null, string position = "left", int offset = 0, string formatter = null)
        {
            base.Name = "charty";
            base.Args1 = name;
            base.Args2 = position.ToString();
            base.Args3 = offset.ToString();
            base.Args4 = formatter;
        }
    }

    /// <summary>
    /// 图表显示的时候的Y轴，就是数据轴，比如雨水量，成绩，成交量等
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasteChartseriesAttribute : ColumnDataTypeAttribute
    {
        /// <summary>
        /// 图表显示的时候的数据项，比如雨水量，成绩，成交量等
        /// </summary>
        /// <param name="type">显示类型 line线段模式 bar柱子模式</param>
        /// <param name="yindex">用哪个Y轴数据比如一边是数量，一边是金额</param>
        /// <param name="title">名称，如果不填则读取字段的</param>
        /// <param name="args4"></param>
        public PasteChartseriesAttribute(string type = "line", int yindex = 0, string title = null, string args4 = null)
        {
            base.Name = "chartseries";
            base.Args1 = type;
            base.Args2 = yindex.ToString();
            base.Args3 = title;
            base.Args4 = args4;
        }
    }
}
