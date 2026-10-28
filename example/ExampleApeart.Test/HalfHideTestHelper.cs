using PasteApeart.Helper;
using System.Text.Json;
using Xunit;

namespace ExampleApeart.Test
{
    /// <summary>
    /// 测试脱敏处理
    /// </summary>
    public class HalfHideTestHelper
    {
        private readonly ITestOutputHelper _output;
        public HalfHideTestHelper(ITestOutputHelper output) { _output = output; }

        /// <summary>
        /// 标记新对象单个
        /// </summary>
        [Fact]
        public void NewSingle()
        {
            var model = new HalfNewModel { Name = "1345678901", Age = 5, Info = new HalfNewIncludeModel { Email = "buijhu@admin.com", Desc = "bhuijk" } };
            var result = HalfHideHelper.PasteMaskData(model, typeof(HalfNewModel));
            _output.WriteLine("原对象:" + JsonSerializer.Serialize(model));
            _output.WriteLine("新对象:" + JsonSerializer.Serialize(result));
        }

        /// <summary>
        /// 标记新对象的集合
        /// </summary>
        [Fact]
        public void NewList()
        {
            var list = new List<HalfNewModel>();
            var model = new HalfNewModel { Name = "1345678901", Age = 5 };
            list.Add(model);
            var model2 = new HalfNewModel { Name = "40987654567765", Age = 15, Info = new HalfNewIncludeModel { Email = "cvbnkjhgfdty@qq.com", Desc = "list.info" } };
            list.Add(model2);

            var result = HalfHideHelper.PasteMaskData(list, typeof(HalfNewModel));

            _output.WriteLine("原对象:" + JsonSerializer.Serialize(list));
            _output.WriteLine("新对象:" + JsonSerializer.Serialize(result));

        }

        /// <summary>
        /// 标记单个
        /// </summary>
        [Fact]
        public void Single()
        {
            var model = new HalfModel { Name = "1345678901", Age = 5, Info = new HalfIncludeModel { Email = "buijhu@admin.com", Desc = "bhuijk" } };
            var result = HalfHideHelper.PasteMaskData(model, typeof(HalfModel));
            _output.WriteLine("原对象:" + JsonSerializer.Serialize(model));
            _output.WriteLine("新对象:" + JsonSerializer.Serialize(result));
        }

        /// <summary>
        /// 标记集合
        /// </summary>
        [Fact]
        public void List()
        {
            var list = new List<HalfModel>();
            var model = new HalfModel { Name = "1345678901", Age = 5 };
            list.Add(model);
            var model2 = new HalfModel { Name = "40987654567765", Age = 15, Info = new HalfIncludeModel { Email = "cvbnkjhgfdty@qq.com", Desc = "list.info" } };
            list.Add(model2);

            var result = HalfHideHelper.PasteMaskData(list, typeof(HalfModel));

            _output.WriteLine("原对象:" + JsonSerializer.Serialize(list));
            _output.WriteLine("新对象:" + JsonSerializer.Serialize(result));

        }

    }

    /// <summary>
    /// 
    /// </summary>
    [PasteHalfHide(4, 4, '*', true)]
    public class HalfNewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [PasteHalfHide]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [PasteHalfHide]
        public HalfNewIncludeModel Info { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HalfExpendModel Model { get; set; } = new HalfExpendModel();
    }

    /// <summary>
    /// 
    /// </summary>
    public class HalfExpendModel
    {

        /// <summary>
        /// 
        /// </summary>
        public int High { get; set; } = 100;

        /// <summary>
        /// 
        /// </summary>
        public int Width { get; set; } = 1920;

    }


    /// <summary>
    /// 
    /// </summary>
    public class HalfNewIncludeModel
    {
        /// <summary>
        /// 
        /// </summary>
        [PasteHalfHide(4, 5, '.')]
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Desc { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [PasteHalfHide(3, 4, '*', false)]
    public class HalfModel
    {
        /// <summary>
        /// 
        /// </summary>
        [PasteHalfHide]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 关键字段要标记，否则不会继续下探
        /// </summary>
        [PasteHalfHide]
        public HalfIncludeModel Info { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HalfExpendModel Model { get; set; } = new HalfExpendModel();
    }

    /// <summary>
    /// 
    /// </summary>
    public class HalfIncludeModel
    {
        /// <summary>
        /// 
        /// </summary>
        [PasteHalfHide(4, 5, '#')]
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Desc { get; set; }
    }
}
