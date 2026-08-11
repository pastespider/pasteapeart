    
## 在当前文件夹下可以创建对应的模板文件

    dto.txt
    server.txt
    
## 模板写法资料
    //http://dotliquidmarkup.org/
    //https://shopify.github.io/liquid/tags/template/

    //两个单词的时候应该改成小写中间加_ 比如DirName 应为 dir_name

## ClassModel
```csharp
    /// <summary>
    /// 对象信息
    /// </summary>
    public class ClassModel : Drop
    {
        /// <summary>
        /// 命名空间，项目名称
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 首字母小写
        /// </summary>
        public string lowerName { get {
                return CommonHelper.FirstCharToLower(Name);
            } 
        }
        /// <summary>
        /// 模型中文名称
        /// </summary>
        public string CnName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string DirName { get; set; }

        /// <summary>
        /// 附带的属性
        /// </summary>
        public List<ClassProperty> ClassPropertys { get; set; }

                /// <summary>
        /// 附带的属性
        /// </summary>
        public List<ClassProperty> ClassPropertys { get; set; }

        /// <summary>
        /// AddDto附带的属性
        /// </summary>
        public List<ClassProperty> AddPropertys { get; set; }

        /// <summary>
        /// UpdateDto附带的属性
        /// </summary>
        public List<ClassProperty> UpdatePropertys { get; set; }

        /// <summary>
        /// Dto附带的属性
        /// </summary>
        public List<ClassProperty> DetailPropertys { get; set; }

        /// <summary>
        /// ListDto附带的属性
        /// </summary>
        public List<ClassProperty> ListPropertys { get; set; }

        /// <summary>
        /// 在Update中不在AddDto中的
        /// </summary>
        public List<ClassProperty> UpdateExceptAddPropertys { get; set; }

        /// <summary>
        /// 在DetailDto中不在AddDto中的
        /// </summary>
        public List<ClassProperty> DetailExceptAddPropertys { get; set; }

        /// <summary>
        /// 在ListDto中不在AddDto中的
        /// </summary>
        public List<ClassProperty> ListExceptAddPropertys { get; set; }

        /// <summary>
        /// uint long int
        /// </summary>
        public string EntityType { get; set; } = "int";

        /// <summary>
        /// 显示的注释 注释summary里面的内容
        /// </summary>
        public string DisplayStr { get; set; }

        /// <summary>
        /// 作废，使用CnName替代
        /// </summary>
        public string DisplayShort { get; set; }

        /// <summary>
        /// 是否是读写分离模式
        /// </summary>
        public bool IsWriteReadModel { get; set; } = false;

    }
```

## ClassProperty
```csharp
    /// <summary>
    /// 对象属性
    /// </summary>
    public class ClassProperty : Drop
    {
        /// <summary>
        /// 属性类型 可能带了Dto
        /// </summary>
        public string PropertyType { get; set; }

        /// <summary>
        /// 上面这个的基类，不带Dto，干净的个体
        /// </summary>
        public string PropertyBase { get; set; }

        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 首字母小写
        /// </summary>
        public string lowerName
        {
            get
            {
                return CommonHelper.FirstCharToLower(Name);
            }
        }

        /// <summary>
        /// 属性中文名称
        /// </summary>
        public string CnName { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required { get; set; } = false;

        /// <summary>
        /// 最大长度 -1表示不启用MaxLength
        /// </summary>
        public int MaxLength { get; set; } = -1;

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; } = "";

        /// <summary>
        /// 属性特性
        /// </summary>
        public List<ClassAttribute> ClassAttributes { get; set; }

        /// <summary>
        /// 是否是其他的模型
        /// </summary>
        public bool boolOtherModel { get; set; }

        /// <summary>
        /// 注释是啥
        /// </summary>
        public string PropertyDisplayStr { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string DisplayStr { get { return PropertyDisplayStr; } }

        /// <summary>
        /// 是否是枚举类型
        /// </summary>
        public bool IsEnumClass { get; set; } = false;
    }
```

## ClassAttribute
```csharp
    /// <summary>
    /// 属性特性
    /// </summary>
    public class ClassAttribute:Drop
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 属性值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 属性组合字符串
        /// </summary>
        public string NameValue { get; set; }
    }
```

## TemplateModel

```csharp
    /// <summary>
    /// 自定义模板信息
    /// </summary>
    public class TemplateModel
    {
        /// <summary>
        /// 服务页面
        /// </summary>
        public string serverbody { get; set; }

        /// <summary>
        /// 列表首页
        /// </summary>
        public string indexbody { get; set; }

        /// <summary>
        /// 查看页面
        /// </summary>
        public string viewbody { get; set; }

        /// <summary>
        /// 编辑页面
        /// </summary>
        public string editbody { get; set; }

        /// <summary>
        /// 新增页面
        /// </summary>
        public string addbody { get; set; }

        /// <summary>
        /// Vue的首页
        /// </summary>
        public string vueindex { get; set; }

        /// <summary>
        /// Vue的浏览页
        /// </summary>
        public string vueview { get; set; }

        /// <summary>
        /// Vue的浏览页
        /// </summary>
        public string vueadd { get; set; }

        /// <summary>
        /// Vue的浏览页
        /// </summary>
        public string vueedit { get; set; }
    }
```