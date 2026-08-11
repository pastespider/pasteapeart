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
        /// 
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<T> Items { get; set; }

        /// <summary>
        /// 数据模型
        /// </summary>
        public VoloModelInfo PasteModel { get; set; }

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


}
