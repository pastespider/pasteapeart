using PasteApeart.Helper;
//using PasteApeart.Helper
//using Volo.Abp.AutoMapper;

namespace PasteApeart
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PasteResultDto<T>
    {

        /// <summary>
        /// 
        /// </summary>
        public PasteResultDto()
        {

        }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IReadOnlyList<T> Items { get; set; }

        /// <summary>
        /// 数据模型 适用于request=one page==1的返回
        /// </summary>
        public VoloModelInfo PasteModel { get; set; }

        /// <summary>
        /// 综合统计 适用于基于查询的数据总量，PasteSum是当前页的，这个是总的
        /// </summary>
        public List<PasteSelectModel> PasteTotal { get; set; }

        /// <summary>
        /// 设定类型
        /// </summary>
        /// <param name="listObj">new XXXListDto()</param>
        /// <param name="queryObj">new InputQueryXXX()</param>
        /// <returns></returns>
        public VoloModelInfo BuildModel(object listObj, object queryObj)
        {
            var _model = BuilderHelper.AttributeReadModelProperty(listObj, listObj.GetType(), true);
            var _query_model = BuilderHelper.AttributeReadModelProperty(queryObj, queryObj.GetType());
            if (_query_model != null)
            {
                _model.QueryProperties = _query_model.Properties;
            }
            PasteModel = _model;
            return _model;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class PasteSelectModel
    {

        /// <summary>
        /// 名称 示例:销量
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 值 示例:￥35,000.00
        /// </summary>
        public string value { get; set; }
    }
}
