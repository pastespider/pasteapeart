namespace PasteApeart.Helper
{
    /// <summary>
    /// 前端用数据类型 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class ColumnDataTypeAttribute : Attribute
    {
        /// <summary>
        /// image region navigator select selects dateplan datetime等
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Args1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Args2 { get; set; }

        /// <summary>
        /// 参数3
        /// </summary>
        public string Args3 { get; set; }

        /// <summary>
        /// 参数4
        /// </summary>
        public string Args4 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 服务端特性 是的话不需要返回给前端
        /// </summary>
        public bool IsServerAttribute { get; set; } = false;

        /// <summary>
        /// 排序 用于特性排序，从小大大
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 按照要求文档填写参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args1"></param>
        /// <param name="args2"></param>
        /// <param name="args3"></param>
        /// <param name="args4"></param>
        /// <param name="ErrorMessage"></param>
        public ColumnDataTypeAttribute(string name = null, string args1 = null, string args2 = null, string args3 = null, string args4 = null, string ErrorMessage = null)
        {
            this.Name = name;
            this.Args3 = args3;
            this.Args1 = args1;
            this.Args2 = args2;
            this.Args4 = args4;
            this.ErrorMessage = ErrorMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}-{3}-{4}", Name, Args1, Args2, Args3, Args4);
        }


    }


    /// <summary>
    /// 权限按钮
    /// </summary>
    public abstract class BtnCodeAttribute : ColumnDataTypeAttribute
    {

        /// <summary>
        /// btnCode权限按钮的代码
        /// </summary>
        public string Code { get; set; }

    }

}
