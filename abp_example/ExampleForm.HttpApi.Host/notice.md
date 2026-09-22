
# dynamicform 动态表单，基于json自动创建的form 一般用于编辑数据

## Dynamic
	read  data_key data table_code fields url:""
	post 

## DynamicTable 
	code(table_code)
	name
	desc
	Str空值是否去除
	Number空值去除
```JSON
	{
		"name":"",
		"age":3
	}
```
```JSON
{
	"age":3
}
```


## DynamicField
	
```
	        /// <summary>
        /// 字段名称 CreateDate
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 字段类型 System.DateTime
        /// </summary>
        public string DataType { get; set; } = "";

        /// <summary>
        /// 字段默认值 0001/1/1 0:00:00
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 字段中文 创建日期
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// 注释的后部分 空格之后的，一般用于描述
        /// </summary>
        public string Placeholder { get; set; } = "";

        /// <summary>
        /// XML文档注释内容
        /// </summary>
        public string Summary { get; set; } = "";

        /// <summary>
        /// 限定 最大长度
        /// </summary>
        public int Maxlength { get; set; } = 0;

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required { get; set; } = false;

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; } = 0;
```

	use_range
	max_val
	min_val
	default_value:默认值，当前数据为空的时候，启用新的数据！
	data_type:string number datetime
	required
	placeholder
	name
	xpath

	attributestr jsonstring


	转化成组??? array



	name
	user__name
	items__name
	items__


## 模式

	add
	edit
	view
	bind
	select
	json
	



## 文档说明
	
	框架介绍
		前言
		需求
		痛点
		理念
	案例若干
		前言
		案例一
		案例二
		案例三
		案例四
	注备开始
		前言
		下载
		组成
		约定
	特性介绍^
		特性合集
		image
		richtext
		text
		textarea
		... .. .

