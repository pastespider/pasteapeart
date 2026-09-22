using Microsoft.AspNetCore.Mvc;
using ExampleForm.Application.Contracts;
using ExampleForm.Application.testmodels;
using PasteApeart.Helper;
using ExampleForm.usermodels;
using System.Reflection;
using PasteApeart.Application.Contracts;

namespace ExampleForm.testmodels
{
    /// <summary>
    ///
    ///</summary>
    public class TestAppService : ExampleFormAppService
    {

        /// <summary>
        ///
        ///</summary>
        public TestAppService() : base()
        {

        }


        /// <summary>
        /// 读取AddDto的数据模型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/auto/model")]
        public VoloModelInfo ReadAddModel20([FromQuery] string module)
        {
            if (String.IsNullOrEmpty(module))
            {
                throw new PasteCodeException("参数module必填！");
            }
            //下面的代码要做缓存 最好要做限定 比如NameSpace的限定，这样会安全些
            var _path = "ExampleForm.Application.Contracts.dll";
            Type _target = null;
            if (System.IO.File.Exists(_path))
            {
                var assembly = Assembly.LoadFrom(_path);
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract)
                    {
                        if (type?.Namespace == "ExampleForm.Application.testmodels")
                        {
                            if (type.Name == module)
                            {
                                _target = type;
                                break;
                            }
                        }
                    }
                }
            }
            if (_target != null)
            {
                var _model = BuilderHelper.ReadTypeProperty(_target);
                return _model;
            }
            else
            {
                throw new PasteCodeException("没有找到这个类型" + module);
            }
        }

        /// <summary>
        /// select lselect selects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/select/readAddModel")]
        public VoloModelInfo ReadAddModel1()
        {
            var _model = BuilderHelper.AttributeReadModelProperty<SelectDto>(new SelectDto(), true);
            return _model;
        }

        /// <summary>
        /// date time datetime dateplan
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/date/readAddModel")]
        public VoloModelInfo ReadAddModel2()
        {
            var _model = BuilderHelper.AttributeReadModelProperty<DateTimeDto>(new DateTimeDto());
            return _model;
        }

        /// <summary>
        /// date time datetime dateplan
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/date/1/readDetailModel")]
        public VoloModelInfo ReadAddModel12()
        {
            var _model = BuilderHelper.DynamicReadModelProperty<DateTimeDto>(new DateTimeDto());
            return _model;
        }

        /// <summary>
        /// text textarea html md 等
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/text/readAddModel")]
        public VoloModelInfo ReadAddModel3()
        {
            var _model = BuilderHelper.AttributeReadModelProperty<StringDto>(new StringDto());
            return _model;
        }

        /// <summary>
        /// outer outers object objects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/obj/readAddModel")]
        public VoloModelInfo ReadAddModel4()
        {
            var _model = BuilderHelper.AttributeReadModelProperty<ObjectDto>(new ObjectDto());

            ////排序 自行处理
            //_model.Properties.Sort((p1,p2)=>p1.Sort.CompareTo(p2.Sort));
            ////排序自行处理
            //_model.Properties = _model.Properties.OrderBy(x => x.Sort).ThenBy(x => x.Name).ToList();

            return _model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/obj/item")]
        public VoloModelProperty Item()
        {
            var _property = new VoloModelProperty
            {
                Name = "name",
                Title = "abc",
                Value = base._httpContext.Request.QueryString.Value.ToString(),
                Summary = "修改后的信息",
                DataType = "String",
                Maxlength = 16
            };
            _property.Attributes = new System.Collections.Generic.List<VoloModelAttribute>
            {
                new VoloModelAttribute
                {
                    Name="sunform",
                     Args1="./pasteform/view.html?path=mod1&vid=112233"
                }
            };
            return _property;
        }

        /// <summary>
        /// object的外援
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/mod1/readAddModel")]
        public VoloModelInfo ReadAddModel5()
        {
            //ObjectMapper.Map<UserInfo, UserInfoDto>(null);
            var _model = BuilderHelper.AttributeReadModelProperty<Model1>(new Model1());
            return _model;
        }

        /// <summary>
        /// object的外援
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/mod2/readAddModel")]
        public VoloModelInfo ReadAddModel6()
        {
            var _model = BuilderHelper.AttributeReadModelProperty<Model2>(new Model2());
            return _model;
        }

        /// <summary>
        /// image测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/img/readAddModel")]
        public VoloModelInfo ReadAddModel7()
        {
            var _model = BuilderHelper.AttributeReadModelProperty<ImageModel>(new ImageModel());
            return _model;
        }

        /// <summary>
        /// file测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/file/readAddModel")]
        public VoloModelInfo ReadAddModel8()
        {
            var _model = BuilderHelper.AttributeReadModelProperty<FileModel>(new FileModel());
            return _model;
        }

        /// <summary>
        /// file测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/app/num/readAddModel")]
        public VoloModelInfo ReadAddModel9()
        {
            var _model = BuilderHelper.AttributeReadModelProperty<DataFormat>(new DataFormat());
            return _model;
        }

    }
}
