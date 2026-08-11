namespace PasteApeart.Helper
{
    /// <summary>
    /// 权限配置
    /// </summary>
    public interface IPasteAuthRole
    {
        /// <summary>
        /// 模块
        /// </summary>
        string Model { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        string Role { get; set; }
    }

    /// <summary>
    /// 按钮权限配置
    /// </summary>
    public interface IPasteButtonRole
    {
        /// <summary>
        /// 代码 示例batch_auth 表示哪个按钮
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 路径 示例:UserInfo.add,如果是UserInfo.*表示所有
        /// </summary>
        public string Path { get; set; }
    }
}
