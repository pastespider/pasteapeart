using Microsoft.AspNetCore.Mvc;
using PasteForm.Helper;
using System.Dynamic;

namespace PasteApeart.usermodels
{
    /// <summary>
    ///用于动态渲染 什么时候最合适？ 比如多格式的图标模式
    ///</summary>
    public class SingleAppService : PasteApeartAppService
    {


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public VoloModelInfo ReadListModel()
        {
            return readModel();
        }


        /// <summary>
        /// 读取数据模型
        /// 你也可以从实际Model中读取，然后调用动态删除，把不需要的删除即可
        /// </summary>
        /// <returns></returns>
        private VoloModelInfo readModel()
        {

            var model = new VoloModelInfo
            {
                Title = "动态表格",
                Desc = "基于Page模式，二合一返回数据信息",
                KeyType = "int",
                Properties = new List<VoloModelProperty>(),
            };

            var _query_model = BuilderHelper.DynamicReadModelProperty(new InputQueryGradeInfo());
            if (_query_model != null)
            {
                model.QueryProperties = _query_model.Properties;
            }
            model.Properties.Add(new VoloModelProperty { Title = "标识", Name = "id", DataType = "Int32", });
            model.Properties.Add(new VoloModelProperty { Title = "周一", Name = "day1", DataType = "Int32", });
            model.Properties.Add(new VoloModelProperty { Title = "周二", Name = "day2", DataType = "Int32", });
            model.Properties.Add(new VoloModelProperty { Title = "周三", Name = "day3", DataType = "Int32", Attributes = new List<VoloModelAttribute> { new VoloModelAttribute { Name = "sum" } } });
            model.Properties.Add(new VoloModelProperty { Title = "周四", Name = "day4", DataType = "Int32", });
            model.Properties.Add(new VoloModelProperty { Title = "周五", Name = "day5", DataType = "Int32", Attributes = new List<VoloModelAttribute> { new VoloModelAttribute { Name = "sum" } } });
            model.Properties.Add(new VoloModelProperty { Title = "周六", Name = "day6", DataType = "Int32", });
            model.Properties.Add(new VoloModelProperty { Title = "周日", Name = "day7", DataType = "Int32", });

            return model;
        }

        /// <summary>
        /// 获取 测试单次返回模式，queryString中需要参数和值request=one
        /// 返回的数据要和上面的格式一致
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic Page([FromQuery] InputQueryGradeInfo input)
        {

            var list = new List<ExpandoObject>();

            var _random = new Random();

            for (var k = 0; k < input.size; k++)
            {
                dynamic da = new ExpandoObject();
                da.id = (input.page + k);
                da.day1 = _random.Next(1, 100);
                da.day2 = _random.Next(1, 100);
                da.day3 = _random.Next(1, 100);
                da.day4 = _random.Next(1, 100);
                da.day5 = _random.Next(1, 100);
                da.day6 = _random.Next(1, 100);
                da.day7 = _random.Next(1, 100);
                list.Add(da);
            }

            dynamic pageDto = new ExpandoObject();

            if (input.page == 1)
            {
                pageDto.pasteModel = readModel();
                pageDto.totalCount = 3 * input.size;
            }
            pageDto.items = list;

            return pageDto;

        }

    }
}
