

# ColumnDataType 特性属性说明

## image
当前特性可以作用于所有的Dto中，具体的样式由前端决定，支持单图和多图模式

|字段|类型|示例|说明|
|----|----|----|----|
|args1|数字|1|图片数量|
|args2|字符|cate|存放在什么位置，上传图片的时候会附带到参数type中 也会用于图片的样式？|
|args3|字符|60*60|图片是否需要压缩，压缩的宽高，不压缩的设置为0,比如60*0|
|args4|字符|small|small,normal,big表示图片的大小三个规格，默认normal，如果要返回格式，则由dataFrom决定|


## file
其实可以使用image的接口，他们2个的返回格式都是一样的，wangEditor的返回格式，主要是UI上不一样，毕竟文件没法预览
存放文件夹？路径？

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|/api/app/Upload/UpImage|表示上传的路径，默认是/api/app/Upload/UpFile,你也可以自己修改他,args1或者args3必填其中一个|
|args2|字符|file|datadir|
|args3|字符|global_upload_file(this);|onchange函数|
|args4|字符|待定|为后续扩展做准备|

## region
小程序中的地区选择，可以配置精确度，到区还是到县

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|region|表示地址选择的层级，可选region和sub-district|
|args2|字符|str|可用值str或者arr 表示返回的数据类型,str的时候用,隔开|

## outer 
表示一个值需要从外表获取，编辑的时候如何显示? 比如fatherId,extendRole,args1支持动态参数比如./index.html?path=userInfo&project_id={{:=item.projectId}}

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|cateInfo|外表的名称，对应模板的path,或者路径，路径一定附带了/字符示例./abc.html|
|args2|字符|extendCates|表示显示的数据，需要和下面2个配合，是一个当前的扩展，目标数组要配置hidden|
|args3|字符|id|获取返回对象的属性，一般为id|
|args4|字符|name|id的友好名称显示,这里指的是外表，比如cateId，需要打开catelist页面，选择后，返回cate,则name作为友好显示,id作为实际值,支持+拼接| 

## outers
outer的复数版本，表示可以从外部列表中选择多个，比如在创建账号的时候给他绑定多个角色，就用这个！

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|cateInfo|外表的名称，对应模板的path,或者路径，路径一定附带了/字符示例./abc.html|
|args2|字符|extendCates|表示显示的数据，需要和下面2个配合，是一个数组，目标数组要配置hidden|
|args3|字符|id|获取返回对象的属性，一般为id|
|args4|字符|name|id的友好名称显示,这里指的是外表，比如cateId，需要打开catelist页面，选择后，返回cate,则name作为友好显示,id作为实际值| 

## outerdisplay
ListDto中用于外表的显示，比如有字段cateInfoId,对应的ExtendCateInfo要标记为outerdisplay,args2配置为extendCateInfo?.name || '',否则会显示为[object object]

|字段|类型|示例|说明|
|----|----|----|:----|
|args1|字符|cateInfoId|表示这个字段的值，一般不显示|
|args2|字符|extendCateInfo?.name|表示显示的名称，友好名称，需要后端支持,在前端会处理成.display|
|args3|字符|name|表示显示的字段|

## display
在ListDto中用于显示某一个object类型的字段

|字段|类型|示例|说明|
|----|----|----|:----|
|args1|字符|name|显示当前字段的某一个子字段，默认为name|


## navigator 抛弃，请使用outer
表示这个值需要使用页面从另外一个列表中获取，这里表示小程序端的,建议使用outer outerdisplay

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|cate.name \|\| ''|表示显示cate.name或者空这个一般用于ListDto中|
|args2|字符|cateInfo|外表的名称，对应模板的path，可以为空|
|args3|字符|/pages/cate/index/?model=select|如果对应的表不用模板，则直接表示路径|

## hidden
表示隐藏这个字段，一般是主键ID，或者外表链接过来的会这配置，比如需要给cate添加子项，则添加由cate那边过来
这个也适用于ListDto

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|bind|如果不填表示隐藏，如果填写了表示页面的query中model=xxx的时候不隐藏,表示非xxx的时候隐藏，xxx的时候不隐藏|

## password
密码框模式

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|*|使用什么字符替换，有些情况下才支持|

## readyonly
表示这个字段是只读的，一般是编辑的时候生效

## richtext markdown
如果是字符串，没有设置maxlength，默认就会变更成richtext，也可以手动强制配置

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|500|配置高度,默认为500|
|args2|字符|txt|表示另外一个值存储在哪个字段，所以另外一个字段一般设置隐藏|
|args3|字符|/api/app/Upload/Image|图片上传的地址|

## textarea
如果是字符串，设置maxlength，且设置的值大于128，默认就会变更成textarea，也可以手动强制配置

|字段|类型|示例|说明|
|----|----|----|----|
|name|textarea|textarea|表示用TextArea来渲染这个内容|
|args1|字符|height:500px;|就是style里面的内容|

## text
对于一些长度过长的，会被判定为textarea，或者richtext的可以使用这个强制换行成text

|字段|类型|示例|说明|
|----|----|----|----|
|name|text|text|表示用Text来渲染这个内容|
|args1|字符|true|是否单行显示，类似singlerow|
|args2|字符|mbtn|就是外联样式名称，类似PasteClass|

## fentoyuan
这个是指后台单位为分，又需要前端进行元输入的，注意小数位数为2位小数，也就是分的格式为int

## selects select lselect
表示多选，这个表示的是页面上的多选，需要列表显示，然后是可以多选！关键在于最后读取数据的时候，需要判断是什么格式的！
select默认就只能选择一个

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|[{"name":"大","value":"1"},{"name":"小","value":"2"}]|表示单选的可选值，name是显示 value是值|
|args2|字符|,|如果值类型不是数组，则返回字符串，用这个字符拼接,也就是分割字符|
|args3|字符|-1,0|去除某些值的选项，比如去除-1表示的全部等,在enum的时候适用,配置的时候args1为空这个时候,从enum中读取|
|args4|字符|0|可选数量，不填或者0变表示不限|

## sort
表示排序，表示字段的顺序,一般表格比较会使用这个

|字段|类型|示例|说明|
|----|----|----|----|
|args1|数字|0|小的排在前面，默认为0|

## link
这个用于表格显示，一般表示用于显示外表的数据,这个将弃用，使用outerdisplay替换

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|extendCate.name|显示的外表链接，示例extendCate?.name \|\| ''表示显示cate.name或者空这个一般用于ListDto中|

## width
表示这个字段在表格得宽度，可以为*或者对应得数字,是表格得列的宽度的权重,这个适用于ListDto

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|60|表示这个列的宽度，可以为数字也可以是\*比如3\*这样|

## orderby
表示基于哪个字段进行排序,这个一般是ListDto表示表格中，可以基于哪个字段进行排序查询

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|id|表示使用id正序排序|
|args2|字符|id desc|表示使用倒叙排序|

## datalist
前端表示使用datalist作为选择数据源，前端需要把datalist的id赋值给datalistid，默认为字段name,这个规则适用于表单和QueryDto

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|[{"name":"正常","value":"1","selected":true}]|JSON的字符串，也可以为空|
|args2|字符|data_pro|表示调用哪个datalist数据，表示datalist的id,和args1互斥|
|args3|字符|read_pro_datalist()|表示需要使用eval执行哪个函数，一般和args2配合使用，和args1互斥|

## datetime
默认的yyyy-MM-dd HH:mm:ss的格式,关于提交的格式，如果是时间DateTime则为对应的yyyy-MM-dd HH:mm:ss的数据提交，如果为long则基于args2进行时间戳秒或者时间戳毫秒提交

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|yyyy-MM-dd HH:mm:ss|表示时间使用的格式|
|args2|字符|留空|留空表示按照默认格式提交,否则按照填写格式提交times(时间戳秒) timems(时间戳毫秒) yyyy-MM-dd(自定义格式)|


## daterange
主字段需要设置为daterange,其他字段需要设置hidden,在最后组合数据的时候，会基于参数生成对应的,应该要设置为可null格式

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|sdate|表示开始时间，最后会传送yyyy-MM-dd 00:00:00的格式数据|
|args2|字符|edate|表示结束时间，如果你选择2024-08-31,最后上送的会是2024-08-31 00:00:00|
|args3|字符|yyyy-MM-dd 00:00:00|表示时间的格式化，默认使用yyyy-MM-dd 00:00:00|
|args4|字符||time,times和留空，留空表示日期格式，和args3有关，times timems表示时间戳的秒和毫秒格式|

## disable
特殊限定，限定于class的，表示禁用这个模型的哪些功能，这个一般用于ListDto，因为这些功能都在列表页面

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|add|表示忽略新增，也就是不显示新增按钮|
|args2|字符|edit|表示忽略编辑，表示列表中没有编辑的这个选项,有些数据只能看，不需要新增和编辑|
|args3|字符|del|表示这个表没有删除，页面UI中不需要删除按钮|
|args4|字符|view|是否有详细，默认没有详细|

## menu
用于列表后面的菜单 比如\<a href="javascript:;" onclick="args2">\<i class="Hui-iconfont args3"></i>\args1</a>

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|编辑|按钮的名称|
|args2|字符|open_view(<%:=item.id%>);|onclick事件的代码|
|args3|字符|Hui-iconfont-menu|样式名称|
|args4|字符|box|默认不写，如果写box表示要方入到menubox中|

## ifmenu
基于条件模式的菜单，使用于Listdto中
示例: [ColumnDataType("ifmenu", "item.age==8", "<a href=\"javascript:;\" onclick=\"open_window(`222`,`./index.html?path=userInfo&goid={{:=item.id}}`)\">条件2</a>", "box")]
数据对象就是item表示当前这行数据

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|isReady == true|if的表达式中()内的内容|
|args2|字符|\<a hreaf="javascript:;" onclick="open({{:=id}})"></a>|这里填写条件后的菜单代码，注意引用需要使用{{}}|
|args3|字符|box|默认不写，如果写box表示要方入到menubox中|

## batch
用户表格数据中的批量操作，示例<input type="button" class="btn btntool" value="args1" onclick="args2">
25.04.11 args4和args1调整
|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|编辑|按钮的名称,可以为空，但是空的时候args3要配置，否则就是一个框了|
|args2|字符|func_batch_click(this);|onclick事件的代码|
|args3|字符|Hui-iconfont-menu|暂未实现，预留给图标用|
|args4|字符|title|按钮的title,鼠标放上去的显示信息|

## htmltemplate
已抛弃，使用下方的html替代,如果要显示自定义的表格，则使用这个，表示直接显示信息模板注意里面的带参使用{{:=item.name}}这样的形式

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|<div><span>{{:=item.name}}</span><span>{{:=item.desc}}</span></div>|需要在td中显示的html代码，支持模板的写法|

## html
如果要显示自定义的表格，则使用这个，表示直接显示信息模板注意里面的带参使用{{:=item.name}}这样的形式

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|<div><span>{{:=item.name}}</span><span>{{:=item.desc}}</span></div>|需要在td中显示的html代码，支持模板的写法|

## query
如果从query中获取到了数据，则对应的字段隐藏,注意不要配置hidden,因为查询的时候是从表单读取数据的，hidden的话是不写入到form中的，那样查询就没法获取query来的值了

|字段|类型|示例|说明|
|----|----|----|:----|
|args1|字符|cateid|表示使用url中的哪个参数读取值|
|args2|字符|hide|如果从query中获取到值后，是否隐藏这个项|

## linkquery
把当前页面的某一个或者多个query传递给下一个页面，多个之间用,隔开。在ListDto中，其实是传递给上方的搜索区域的！对于表单就是传递给表单了!接收需要使用query特性

|字段|类型|示例|说明|
|----|----|----|:----|
|args1|字符|需要传递的参数|多个之间用,隔开，比如serviceid,ftype|

## object
适用于表单页面，表示从另外一个表单新增数据，比如用户的收货地址，在表单的时候，打开一个新的表单，进行信息填写后，返回一个obj，这个时候是没有写入数据库的,所以在返回的时候需要显示
和outer有点像，不过回传的是一个object类型！如果是编辑的时候？需不需要上传到API表示编辑了?打开表单的时候会传递model=object这个参数过去，表示叫子表单不要做API入库操作
引入session_key作为信息传递，使用本地存储进行过手!

注意这个子模型也是需要建立对应的API的，不过不需要建立新增和编辑的接口，因为被上一级涵盖了！
|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|roleInfo|一般使用path也可以使用页面的路径|
|args2|字符|id|新增的时候无用，主要是编辑的时候，基于这个id和path去数据库查询新的数据|
|args3|字符|name|表示显示的是这个object的哪个字段，一般在编辑的时候可见|

## objects
object的复数版本，表示一个集合，比如一个会员有多个爱好，新增的时候，打开子表单，填写多个爱好的object信息体返回,在显示的时候，如果一个字段不足以显示???是否支持多个字段联动显示?

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|roleInfo|一般使用path也可以使用页面的路径|
|args2|字符|id|新增的时候无效，编辑的时候表示从数据库查询信息，也作为删除的key使用|
|args3|字符|name|表示显示的是这个object的哪个字段，一般在编辑的时候可见,支持+拼写多个字段，如果不存在的则会原样输出，比如http://+host_name+:+port|

## mark
字段详细说明，或者是文档，这里是做一个描点，需要使用公共函数自己实现具体的要求，一般的是项目下面的模块的某一个字段的说明，一个项目往往这个是一样的，然后就是哪个模块的哪个字段，所以有2个字段
会在title后面生成一个span class="tapmark" onclick="global_tap_mark(_modek,_code)";

|字段|类型|示例|说明|
|----|----|----|:----|
|args1|字符|product|一般作为模块使用，比如商品的这个模块,如果不填直接使用本页面的calssPath|
|args2|字符|code|模块下的一个字段的说明，比如code这个字段的用法，案例说明等,如果不填写则使用当前字段名称|

## template
作为有些表的特殊布局，就是自定义布局表格部分的内容，包括表头和表身2个部分,注意需要实现选择的功能，除非这个表用不到选择这个功能
文件存放于pasteform/template.html，就只有一个template.html页面，里面的都是模板代码

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|template_user_head|表格的表头部分的script的id|
|args2|字符|template_user_body|表格的表身部分的模板的script的id|

## style
针对一些特殊的设定，比如textarea，有些地方我们要设置高一些

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|height:500px;|就是style里面的内容|



## class
针对一些自定义的class，可以通过这个来配置

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|btngo|表示这个对象要添加的class|

## button
用于表单中，的按钮，用意是让用户点击后触发函数,global_form_button_click(this,className,name);
注意按钮的value为当前字段的value，比如当前页面的path=nginxRoute
如果这个特性标注在model上，比如adddto，则表示form表单的按钮的文字
```
        /// <summary>
        /// Http模板 表示使用Http模式的模板的内容
        /// </summary>
        [ColumnDataType("button")]
        public string Temp1 { get; set; } = "点击导入";
```
则上面会生成的页面内容为
```
Http模板：点击导入
```
点击导入点击后，会调用global_form_button_click(this,'nginxRoute','temp1');
注意由于返回的首单次会小写，所以是temp1而不是Temp1

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|点击新增|表示按钮显示的文字|
|args2|字符|global_func_btn(this)|表示要执行的函数，如果为空，则使用上面的模式|
|args3|字符|add|表示替换掉原本的新增按钮，用于ListDto中|

## unit
用于显示单位，比如MB，KG，个等，一般在Int Long Double等Number地方后显示,也可以作为表格中显示?

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|MB|表示这个对象的单位|

## splitarray
用于文本的拆分，比如int[] long[] string[]等的显示和编辑

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|,|表示使用什么来分割，分割后的类型看数据本身|
|args2|||预留|
|args3|||预留|
|args4|||预留|

## 待处理
1.列表中可编辑的对象 比如switch?
handler_switch_change(elc){
    // className columnName id state
}

2.外表选择的时候，是否支持多选模式?
多选模式的时候，如何加载已经选中的，已经选中的如何删除?难不成要搞一个选定区域?

3.Int32[] Int64[] String[]
这种数据的显示和最后更新

4.表单数据的提交前的数据格式的校验！

5.daterange如何设定
单个变成多个！

6.有些数据不需要新增，和编辑 ，虽然可以使用API控制，但是在前端有这么个按钮存在，也是很突兀的，所以得处理

7.管理员拥有多个角色，这个多个角色的显示和编辑问题！
selects 可以设定返回多个
list设定显示的方式 关键在于如何每次编辑的时候不是覆盖！还是写一个单个删除的接口?

8.enum类型的显示问题？其实就是enmu变种到attribute的select!!!

9.从角色绑定权限来说，角色列表，点击权限(给这个角色绑定权限),打开的权限列表是所有的，需要绑定的勾选，不要的取消勾选
model=bind 则有一些字段这个模式下才显示
hidden.args1表示某一个模式，如果为空表示都隐藏


### 待确认问题
1.必填，如果是int的类型的，是不是只能非0，还是不生效?

2.对于menu的支持，参数因该是{{}}控制的，而不是<%%>

3.enum类型的返回，是返回值，还是对应的名称?是否支持enum转select？

4.有时候表单只是为了新增一个数据，不入库那种！outform model=outform 返回组合的 ...待定

5.比如查看某一个列表--子项列表--子项添加
linkquery cateid,abcid

6.单位转化，比如后台是分，前端需要输入元 那么就是在输入的时候是元，显示是元 提交的时候是分！！！
fentoyuan 分转元 默认2位小数点

7.关于是否需要显示详情页面的问题，因为有时候表格数据是显示不全的，比如richtext，这个时候可能需要一个detail.html页面
如果要呈现，则需要对应的模型Dto的属性,生成器默认是去除的，则需要自己在Domain中添加/template/dto.html作为生成模型替换生成器默认的模型！
页面的呈现上和表单类型，把输入框等渲染成直接显示的即可！
然后再由ListDto中，添加菜单detail的支持！

8.在列表页面中，如果一个对象被标记为query,如果从参数中获取到了值，则这个查询项进行隐藏操作

9.如果列表页面的当前状态为model，则上方的搜索区域默认隐藏

10.关于引入字段说明的事情，比如一个表单100个字段，各个字段表示什么意思，一般的做法是点击对应的？然后弹出或者是跳转到某一个站点，显示这个介绍
mark由此而来

11.是否加入自模型，就是表格数据展示的样式，是否引入自定义的模板？？？
默认是使用内置的二次模板，如果是引用外部的化，则需要整个，也就是表头和表身都需要！
需要外部写一个html的template页面，然后使用Jquery的load加载进去！
在ListDto中，使用template进行标注，标注args1为表头样式的模板 args2为表身部分的内容

12.添加对自定义样式的支持 style class

13.添加对menubox的支持，针对很多按钮的情况

14.表单中需要特意操作的，添加函数支持 把表单的数据传递过去 由自定义函数决定执行结果!global_form_action(elc,className,name,location.search);
项目路由中的 载入模板 模板测试等！

15.搞一个占位的？有时候的排版等，需要空开一个位置

16.可以搞一个属性继承于基础属性，这样在输入的时候会好些，比如PasteMarkAttribute:ColumnDataTypeAttribute;

17.窗口的弹出大小问题，有些情况需要大的 有些情况小的 有些情况要动态的！

18.查询的select和表格的select冲突的问题！！！

19.表格的不同模式下，不同字段的显示问题！



### 新的需求

1.添加对全选模式的支持
    a.全部选择，反选，手动选择！
    b.多种按钮，至于上方！还是冒出来一行新的?
    c.批量操作的按钮在attributes中,率属于ListDto的Class中

    批量操作的具体实现需要在公共函数中自定义实现！！！

2.是否添加前缀，比如$ ￥等！

3.path支持路径写法，比如原来的写法roleInfo
    现在可以支持/api/task/roleInfo
    表示不使用默认的路径/api/app/roleInfo

4.引入公共函数库，之前已经有style.less->style.css作为公共样式库
    那么Js是否也需要，如果需要，那应该在pasteform文件夹之外，防止版本覆盖的问题，这个问题style.less应该也要遵守

5.index页面中，当model==bind的时候不再显示最后一列的操作，因为一般是空格的！！！

6.修改表格的外围border,解决滚动的时候表格上面和下面没有边框的问题

7.修复index页面中的select默认选中不生效的问题


## 24.10.22
    1.修复Enum在Domainn内的时候找不到的情况
    2.修复204返回的问题

## 24.10.25
    1.添加对object的支持，也就是字段object的类型的支持
    2.修复img[] 也就是多图的时候的问题
    

## 24.10.28
    1.?是否添加splitarray，表示字符串转数组，至于转int[],long[],string[] 看类型！
    2.object的编辑的时候的支持，由数据转sessionStrorge url+rand_db_key 页面打开后读取，渲染！！！
    3.添加对object的默认值的支持，包括UI的显示，和向外打开的数据呈现(object可以看成是另外一个对象，包含多个属性)

    
## 24.10.29.01
    1.小程序专用的，获取位置的 地区数据对象 层级 返回对象信息
    location
    name address latitude longitude
    返回的信息中，如果做地点的模糊 如何做脱敏? 额外加一个参数？

## location
使用wx.chooseLocation接口获取位置信息，包括经纬度等

|字段|类型|示例|说明|
|----|----|----|----|
|args1||name|名称字段，如果要编辑的话|
|args2||address|地址，如果用户要编辑的话|
|args3||latitude|经度字段，如果要编辑的话|
|args4||longitude|维度，如果需要编辑的话|


## 24.10.30.01
    1.image的属性和maxlength冲突?没有冲突，是因为value的null的问题判断导致破图

## 24.11.01.01
    1.引入scroe也就是raty评分，默认5颗星！

## score
评分，比如滑动或者点击的评分，目前配置是点击评分

|字段|类型|示例|说明|
|----|----|----|----|
|args1||1|预留字段，是否支持小数，默认不支持|
|args2||5|预留字段，后面会变更成最多多少星|
|args3||||
|args4||||

## 24.11.06.01
    wxhidden 同hidden 用于小程序或者小屏幕的隐藏，因为PC和移动端一般列表的字段不一
    wxdisplay

是否添加选择封面的字段，在文章处能用到

## cover
封面图，一般是从富文本或者md中获取,需要使用正则从字段中获取图片url
从哪个字段来？一般是Html而来
是一个集合 首图在最前面
新增 从html中获取
编辑 后端从html中获取 然后把首图写入到最前面
需要添加一个按钮，执行函数funcUpdateImages();表示从当前表单读取图片然后写入到备选

|字段|类型|示例|说明|
|--|--|--|--|
|args1|str|rhtml|对应从哪个html的字段而来|
|args2|str|covertImage|首图写入到哪个字段|

## wxdisplay
用于小程序的显示，和outerdisplay类似

|字段|类型|示例|说明|
|--|--|--|--|
|args1|str|extendUser|另外一个字段|
|args2|str|name|表示取另外一个字段的哪个属性|


是否需要auto接口 而不是item updateItem

## 24.11.10.01
    linkquery的实现思路有问题，应该是从这里打开的所有的url都要link下！！！
    1.一些特定的要如何处理？
    2.还是使用open_window的都经过二次加工一下！
    3.关于参数重复，是覆盖，还是使用原来的!


    1.创建成功，或者更新成功的提示??? response==string tomastMessage
    2.

    1.是否要引入prefix，比如$ ￥等，一般配合unit fentoyuan使用
    2.$6.78 ￥4.56   5.67元
    
## 24.11.12.01

    表添加模型
    已知的特性有linkquery batch
    现在加 button 表示form中的按钮的文字
    1.form中，如果返回的不是object类型，则按照返回的提示



## 24.11.13.01
    1.一种新的需求，比如选择不一样的BANNER位置，返回的图片不一样的规则
    2.则有选择某一个项的时候，字段赋值给url，然后重新回传页面？？？
    3.回传页面后，参数传递给get请求
    4.后端根据get的url修改对应的字段的特性？？？

    5.reload query参数名称 value值(当前值) 一般发生在select中!!!

## 24.11.14.01
    1.是否添加对lselect的支持 select默认为select(option)模式
    2.那么lselect就是ul li menu的横向模式
    3.某些字段的值是否隐藏，比如-1，0表示查询所有的！

## 24.11.16.01
    1.修复index.html表格页面中的搜索项中关于Guid?的回传null的问题！
    2.当页面query为model==object或者selet的时候，隐藏dialog下方的确定取消按钮，因为取消可以点上方的关闭按钮，而确定可以点页面中的选择或者确定，这样可以避免误操作
    3.参考1，修复view.html中Guid为空的时候，且!required的时候，删除这个对象，或者置为null
    4.index.html中，选择外表的时候，直接执行查询，免去再次点击查询按钮！，这个看看是否扩展到select的选择中
    5.添加对richtext的修正，args2为另外一个值的回传，比如richtext则回传html和txt,而txt存放于args2这个字段中
    6.添加对markdown的支持，参数参考richtext的
    7.对query进行调整，由特性决定有query值得时候，是否隐藏这个项,只要args2有值，则隐藏对应得项
    8.view.html中select等非string默认值选中的问题，转化成string再indexOf
    

## reload
在select上适用，比如位置使用select呈现，首页，个人中心，当这个选项变更的时候，我们希望对应的图片的规格变成，则有切换后，去读取reload特性
然后把值回传给API，API居于query参数，把默认值修改和对应的图片规格变更后下发,注意query的key是不能重复的

|字段|类型|示例|说明|
|--|--|--|--|
|args1|str|location|表示query的字段,值为当前选定的值,如果不填写表示当前字段|


## 24.11.17.01
    1.删除md附带的name,之前使用md的时候，会附带2个name
    2.outerdisplay不太适用于小程序等，所以需要有一个替代
    3.display 之间显示当前字段的某一个值，当前字段为object类型


## 24.11.18.01
    1.修复objects的返回值赋值错误的问题！
    2.添加unload参数
    3.添加button特性

## 24.11.27
    1.datalist,相当于可下拉可输入，之前以为没有使用到注释了，现在放开了

## 24.11.28
    1.修复表格页面中的prefix,fentoyuan,unit
    2.修复表格页面中outer的删除按钮右侧不一的问题
    3.修复表格页面，批量操作的按钮的样式问题
    4.添加对输入框回车查询的支持！

## 24.11.29
    1.修复Guid,DateTime非必填的提交问题

## 24.12.02
    1.添加index.html对lselect的支持
    2.修复URLSearchParams对array的支持的问题
    3.修改lselect选择数量超出的默认配置问题，默认为其他的全部取消并提醒

## 24.12.03
    1.修复lselect多选的问题，不会取消之前选中的
    2.修复lselect的默认选中为0的问题！
    3.修复lselect多个会被命中的问题，比如值为12，结果1，2，12都被命中
    
## 24.12.04
    1.group的引入?

## group

有时候字段太多，我们希望能否分组显示，这就需要使用group,如果第一个字段就是group，则表示最顶部使用瞄点模式
当前特性可以用于Class也可以用于Property,为了区分，封装后
PasteGroup用于Class,允许配置多个
PasteGroupItem用于Property,只允许单个，毕竟一个字段只能放于一个区块中
args3有变更 open hide hidden(open)(完全隐藏，可以显示)

|字段|类型|示例|说明|
|--|--|--|--|
|args1|str|gr1|分组依据,分组代码|
|args2|str|基础配置|分组名称|
|args3|str|hide|默认收缩还是展开,hide为收缩，其他为展开|
|args4|str|样式|分组的样式,view正常模式，或者空，edit表示可以+可以-的模式，必须是__多模式，表示有多项的|



## 24.12.12
    1.修复view.js的query的问题，需要在默认值之前进行读取，防止接口值权重大于query

## 24.12.14
    1.富文本，markdown的js和css载入修改成动态模式
    2.整理公共函数form.js,看看后续拆分view.js和index.js的代码
    3.markdown的粘贴图片的函数迁移到../lib/api.js中
    4.修改下方提交按钮的布局为fixed
    5.修改index搜索区域的规则 目前有隐藏 只显示一行 全部显示 默认选择模式的时候，新增按钮隐藏，搜索区域隐藏，正常模式下搜索项只显示一行，所以要把主要的搜索项目写前面去
    6.5的补充，搜索区域，如果搜索项大于一行，则中间位置会有一个展开的图标，点击后可以展开，这样给人的感觉会知道还有下一行

## 24.12.15
    1.修复查询的时候参数值为0被忽略的问题，因为可能后台对这个字段的默认值是非0，这样不传就有问题的！

## 24.12.16
    1.调整默认弹出的大小等
    
## 24.12.17

empty 占位但是不显示，用于改变布局，notice抛弃，这个作用不大！


## 24.12.23
    1.修正view.js中objects的args2.args3.args4错误的问题
    object objects outer outers 等都保持一致 args3表示key(排重字段) args4表示name(显示字段)
    2.针对view.js,对部分特性的另一半做hidden处理，比如outer,daterange等，防止dto中忘了做Hidden

## 24.12.26
    1.button添加，用于ListDto的时候可以修改表格页面的新增按钮！
    2.图片上传前压缩?
    
```javascript
    /**
     * 上传文件
     * @param {*} e 
     */
    this._onefileinput = async function (e) {
        // 这里要给他重置
        var _file = e.files[0];
        const reader = new FileReader();
        reader.onload = (e) => {
            const img = new Image();
            img.onload = () => {
                compressImage(img, 750, 0.6).then(async (compressedDataUrl) => {
                    var _back = compressedDataUrl;
                    var info = {
                        id: 0,
                        data: _back,
                        type: "img",
                        size: _file.size,
                        file_name: _file.name
                    };
                    var _read = await connection.invoke('File', info);
                    //执行上传
                    if (_read) {
                        if (_read.success) {
                            _appendChatToUI(_read.data, true);
                            handlerScrollToBottom();//=1000
                        }
                    }
                });
            };
            img.src = e.target.result;
        };
        reader.readAsDataURL(_file);
        //重置
        $(this).val = null;
    }

    /**
     * 压缩图片
     * @param {*} image 
     * @param {*} maxWidth 
     * @param {*} quality 
     * @returns 
     */
    async function compressImage(image, maxWidth, quality) {
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');

        let scale = 1;
        if (image.width > maxWidth) {
            scale = maxWidth / image.width;
        }

        canvas.width = image.width * scale;
        canvas.height = image.height * scale;

        ctx.drawImage(image, 0, 0, canvas.width, canvas.height);

        return new Promise((resolve, reject) => {
            try {
                const dataUrl = canvas.toDataURL('image/jpeg', quality);

                resolve(dataUrl);
            } catch (err) {
                reject(err);
            }
        });
    }

        /* 工具方法：dataURL(base64字符串)转换为Blob对象（二进制大对象） */
    //data:image/png;base64,iVBORw0KGgoAAAANSUhEUg......
    dataURLtoBlob = function dataURLtoBlob(dataurl) {
        var arr = dataurl.split(',');
        var mime = arr[0].match(/:(.*?);/)[1];// 结果： image/png
        // console.log("arr[0]====" + JSON.stringify(arr[0]));// "data:image/png;base64"
        // console.log("arr[0].match(/:(.*?);/)====" + arr[0].match(/:(.*?);/));// :image/png;,image/png
        // console.log("arr[0].match(/:(.*?);/)[1]====" + arr[0].match(/:(.*?);/)[1]);// image/png
        var bstr = atob(arr[1].replace(/\s/g, ''));
        var n = bstr.length;
        var u8arr = new Uint8Array(n);
        while (n--) {
            u8arr[n] = bstr.charCodeAt(n);
        }
        return new Blob([u8arr], { type: mime });//值，类型
    }
```

## 24.12.30
    1.view模式下支持动态参数，比如outer中的args1为./index.html?path=cateInfo&project_id={{:=item.project_id}}&vvid={{:=item.cateid}}
    如上，在实际跳转中会把{{:=item.project_id}}替换成当前对象的project_id的字段的值，同理{{:=item.cateid}}也一样的效果
    2.目前暂不合并js,继续处于测试周期内！

## 24.12.31
    1.index.js中添加对outer的支持，
    因为view中outer表示外选一个表的某一个数据，
    这里关系互通，
    当然outerdisplay,display可以继续使用,
    配置其中一个即可，
    不过得注意他们标注得字段不一样
    2.view中添加对readonly的支持，这个源于有一个需求，就是查看提交者提交的信息，然后写下新的结果，所以部分信息是不让修改的，所以引入了readonly这个特性！
    

## 25.1
    1.添加动态表单模块，dynamicform
        基于服务端，比如你要给一个非开发编辑json数据，
        由于数据本身没有其他附加信息
        可以通过读取dynamictable,dynamicfield的信息进行补全
        这个需求主要用于PasteSpdier的Windows.IIS版本而开发！
    2.index页面中添加对mark的支持，表格页面也支持哪里不懂点哪里了！
    3.调整Index页面的搜索区域的布局，添加class等，主要是之前的label更换为div!
    4.???是否考虑动态引入，比如动态引入js，动态引入css等！
    5.添加对详细得支持，之前要自己写menu，现在就在Func特性中配置，注意详细默认是关闭的，新增，编辑和删除则默认显示的

## 25.1.11
    1.修复objects的添加问题，字段显示排重的问题
    2.objects添加对编辑的支持
    3.修复text256还是半列宽度的问题
    4.修复objects的item删除的问题


## 25.1.16
    1.objects的排重问题，如果是多个key的？？？
    设定id+name 如果包含了+表示多个之间合并！！！
    objects的字段显示和字段排重支持+写法了
    
## 25.2.1
    1.open_mark_window关闭后的BUG的问题！，叫一个检查是否已经关闭
    2.添加point特性，表示要关注 .tagtrue tagnull tagfalse 应该是要标注tagtrue的样式
    

## 25.2.12
    1.index.html添加对location.search的支持，就是页面中会把当前页面的query传递给api，以便在特殊情况下使用
    2.view.html添加对location.search的支持，就是页面中会把当前页面的query传递给api，以便在特殊情况下使用

## 25.3.1
    1.form的label添加title应对title过长的问题，不然会显示不全
    2.表格中，外表的id是否需要点击后弹出detail？？？


## 25.3.9
    1.list中empty隐藏！！！
    2.文档整理
        基础类 maxlength required range  
        上传类 image file 
        选择类 select selects outer object objects region(小程序) outers datalist datetime daterange
        数据类 richtext markdown text textarea datetime
        显示类 outerdisplay display hidden empty  readonly password html width style class singlerow
        转化类 fentoyuan prefix unit 
        功能类 sort orderby button menu ifmenu batch query linkquery mark template switch splitarray point
    3.是否要添加一个iframe的支持？？？比如用于object的类型的！ 获取值由提交的时候，向iframe取值，由iframe内部做校验！这样可以实现多级表单模式！！！
        read_value 返回一个object类型的数据
    iframe url height style ???

    
## 25.03.12
    1.纠正datalist和其他的对齐(select selects lselect等)
    2.添加对sunform的支持，就是iframe的支持

### sunform
表示在当前页面填写下一级的内容，当前字段的类型应该为object或者其他，比如xxxDto等
对应读取的页面的model=sunobj，打开没有提交按钮！

|字段|类型|示例|说明|
|--|--|--|--|
|args1|str|url|如果没有/，则为./view.html?path=xxx&model=sunobj&data_key=yyyy|
|args2|str|400px|iframe的高度，宽度100%|
|args3|str|留空|备用|
|args4|str|留空|备用|



1.outer的时候，打开的页面的query 是否从当前query中自动补充，也就是不替换，新的没有的加入???  或者使用linkquery模式
2.之前是index才有linkquery,现在改版成都有，add update index都有！
3.由于多层级嵌套的问题，新版本改成子窗口在顶级窗口打开，这样大小就统一了！

1.多层级嵌套，要顶级操作的时候url有问题！！！
2.添加一个配置??? 还是基于当前url自己计算???
3.变更层级后，有一个问题，就是路径的相对问题，本次升级要注意！！！ 是否做一个兼容过渡??? ./开头的

4.关于选定，直接返回array或者object 这个取决于pf_num默认为单个，如果是多个？？？ 100手动 其他自动 默认为1 ？
5.outers支持多个选择了，需要点击最下方的确定按钮提交，如果是outer选择一个即会返回！,因为outers的参数都用完了，默认选中100个才会返回，这个后续看看如何兼容...
6.选择array的时候，返回后会替换掉之前选中了的，所以是全部更新，而之前是追加的模式！ 这里有问题如果数据有多个页...
7.outers 支持name进行+拼接了，这样可以防止某一个字段重复无法做排重，其实就是组合排重的概念！

1.有一个隐藏的BUG，就是列表表格页面，字段id必须存在，否则作为被选对象的时候，会有问题，这个看后续是否要指定一个key，不然有时候真没有id，还是api平凑???


### loaditem
这个是给outer之后读取信息用的，读取后修改某一项的内容,需求是比如填写了用户ID(outer),则下方列出他的角色列表(lselect),有权限的已经打勾了，这样可以基于不同用户下面的信息做个性化变动！
这个和reload有点像，reload是所有的都重新加载，而这个是更新某一个或者添加一个！

|字段|类型|示例|说明|
|--|--|--|--|
|args1|str|/api/app/xxx/yyy?id={{:=item.xxx}}|表示要读取某一个api地址，返回单个字段的信息，用于重新更新UI|
|args2|str|预留|预留|
|args3|str|预留|预留|
|args4|str|预留|预留|


sunform是否要支持动态url??? 因为渲染的时候已经生成url了。。。

## directsun

是否是直接子集，使用__进行拼接不同层级的对象，比如info__name info__age，当前特性只在服务端生效，标记了这个的往往query的dynamic=true
也就是上面的信息最后提交的为
```
info:{
    name:'xxx',
    age:'yyy'
}
```

1.dynamic改版成动态的，只有字段中有__则表示动态的！！！
2.修正dynamic转object不支持0__name的问题，就是直接array一级的情况！
3.添加区域单个对象更换的功能，就是上面的loaditem 目前仅仅支持outer特性之后！


1.有一个新的需求，就是分页不一定按照page来进行的，新的使用游标规则

## nextkey
这个用于分页的，游标模式的分页，默认为0或者空
和page,size类似，新增一个参数next_key,使用当前字段的名称和类型！
如果查询的字段名称叫next_key会直接带入这个分页模式中！
目前这个设定是请求和返回结果的这个字段名称要保持一致，类型也是！


1.修复api.js中关于select会自动转类型的问题
2.lselect的类型的问题... .. .
3.验证不通过的时候focus();



1.table中要依赖于id 如果没有id属性，则获取第一个对象作为id,遍历赋值给id
2.添加编辑模式 然后是读取模式，可以读取整行的数据，每一列用dataname=""表示当前的字段名称
3.是否单个单个单元格支持比如++ --的操作方式！


## edit
args1-args4可以都使用，表示当前模式，目前的模式有view,select,bind,这几个不分顺序，表示在这个模式下则为编辑模式！
这个特性是标注在listdto的字段中的

|字段|类型|示例|说明|
|--|--|--|--|
|args1|str|view|表示当前view模式的时候显示编辑|
|args2|str|select|预留|
|args3|str|bind|预留|
|args4|str|xxx|预留|

## iskey
表示当前字段是主key,会被当成id来使用，也就是唯一！你可以隐藏他！
有些表格listdto数据没有id,则需要弄一个出来，默认会选择第一个，如果你要指定可以给某一个字段标注特性iskey

1.index.html的规则调整 当有批量模式的时候，可选框之前是和id公用，新的独立出来！
2.td添加dataname datatype信息 表示当前字段的数据格式
3.添加getrowselectdata读取选中行的数据，最后形成一个array和2有关系
4.由于1的问题，之前的选择选中的id的集合的代码做了变更
5.调整批量操作按钮和功能按钮的样式，目前的按钮样式是整体性质的
6.由于可编辑的载入，调整了prefix,unit的实现方式，就是呈现不一样！
7.目前还有一个问题，就是table的td中的input的宽度的问题，咋个情况...是被TD提升了，本来的想法是保持最小多宽，然后设定padding 这样随着内容可以扩大的...


## switch
用于表格中显示switch，否则显示true/false,默认点击switch后会提交一个请求给对应的api，你可以控制他，也可以用自己的args3函数来处理这个事宜！
args4:25.04.10更新

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|view|当model==args1的时候才生效，如果全部要生效则标记为空|
|args2|字符|confirm|表示提交前是否要confirm确认，默认不需要|
|args3|字符|handlerStatus|表示要执行的函数名称，注意只有一个参数elc,参考handlerSwitchChange|
|args4|字符|local|本地模式，也就是不触发函数,不填默认远端模式remote,即点击后会执行函数等|


## loadlib
用于给当前页面加载lib用，比如要载入js css等,这个特性是给class标记的，表单和table都可以生效

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|../lib/pro.js|表示载入当前上一个文件夹的lib下的pro.js文件,也可以载入css要看尾巴！|
|args2|字符|func_name|如果是js结尾的，这个可以填写，表示载入后要执行的哪个函数,注意这个函数没有参数|




## 03.30
    新的思路
    1.表单中比如lselect选中某一个的适合，是否可以配置哪些字段显示或者隐藏，也就是reload的变种，这种会有很多场景
    2.

## changeui
改变UI，reload的变种，reload需要api配合，这个是本地的，比如某一个选项选中1，则下面的4.5字段隐藏，否则45显示，是否支持组模式等???
如果选中命中args1，则args2隐藏，如果没命中，则args2显示
可以配置多个，比如有一个lselect有a,b,c对应ABC模块，如果默认值为a，则应该配置B,C隐藏,然后配置3个changeui,a,B b,B 和c,C
这样如果选中a,会走3遍，因为都没命中，也就是B,C都隐藏
如果选中B,也会走3遍，最后变成B显示，A,C隐藏了

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符串|1|当前字段的值,多个之间使用,隔开，比如1，2，3，4选项，选中2或4都可以执行以下逻辑，则2,4|
|args2|字符串|name,age,info_(.*)_name|表示命中后哪些字段显示，否则隐藏,如果是组则group:开头，比如group:gc_01,如果是正则则regexp:开头,args3同理|
|args3|字符串|备用|备用|
|args4|字符串|备用|备用|

比如说1|2|3
如果选中2，则a2显示，也就是1，3的时候a2隐藏
所以有命中和没命中，命中了，则隐藏某些，没命中则显示???

还有一个问题group是否支持hidden模式！
比如directsun模式下，是否可以配置hidden信息

1.修复组别添加删除的问题，之前不完整，导致新加的changeui没有匹配到新加的group

## 03.31
    1.之前的menu,ifmenu不是a标签么，这样会有一个问题，就是没办法传入当前this，所以本次添加一个button！为了和之前的功能button区分，所以命名为tap iftap



## tap
用于列表后面的菜单 比如<input type="button" value="args1" onclick="args2" class="args3">

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|编辑|按钮的名称|
|args2|字符|open_view(<%:=item.id%>);|onclick事件的代码|
|args3|字符|Hui-iconfont-menu|样式名称|
|args4|字符|box|默认不写，如果写box表示要方入到menubox中|


## 25.04
    1.添加ui chartx charty chartseries特性 都是关于图表的
    2.默认dataType的赋值问题，之前直接把特性赋值过去了，这样是有问题的
    3.添加autoUrl,防止比如出现https://www.yyy.com/page/pasteform/pasteform/view.html的问题，就是剔除重复的，后续再看看是否要规定/page/这个唯一唯一
    4.默认图表样式的问题，默认隐藏
    5.query和attribute冲突的时候的问题

    1.changeui有一个未完待续的问题，就是编辑的时候，如果和配置的不符... 需要在解析数据的时候，对对应的property进行干涉，这样渲染的ui才是正确的。。。
    ../lib/api.js有变更
    ```js
                //如果这个UI是主动被隐藏的，则不校验整个规则 但是这个有一个问题，后端会校验...
            var _hidden =$(`[name=${namestr}]`).closest('.hidden');
            if(_hidden){
                return true;
            }
    ```
    输入验证，如果是主动隐藏的，代码层级逻辑隐藏的，则不需要校验，后端也要对应的注意下！
    2.原本hidden的会因为changeui给显示出来,也就是如何本来就要隐藏的，还是changeui引发的隐藏的... .. .,引入了hide这个是始终隐藏的，也适用于id这个字段
    3.directsun，是否引入继承模式，继承特性集合，如果内部有的，名称冲突的则有权重

    1.cover的问题修复，排重，然后是直接读取选中的！
    2.?.之前的changeui的多值问题还没判断
    3.?.cover的required的问题，然后是如何自动的问题！目前是采用读取的方式
    4.changeui的时候，如果是markdown则执行resize,
    5.表单中的按钮支持多按钮一起的模式！

    1.outer如果在表单中有value 而没有对应的对象，则直接显示value
    2.detail添加对sunform的支持，添加对images的支持显示，同时过滤掉null的对象
    3.detail不支持组的问题的修复，添加对应的样式

    1.directsun中的outer outers等，需要是同级别的！！！,这个之前已经实现了，现在排查一遍即可
    2.?.目前还有一个问题，自动填充的问题，有些浏览器有点鬼啊，不遵守协议

    1.针对各种的?类型可能要做一个...专项！！！

    1.要加一个时间戳的时间选定 秒 还是毫秒 格式化

## datetime
默认的yyyy-MM-dd HH:mm:ss的格式 新增args2支持！

|字段|类型|示例|说明|
|----|----|----|----|
|args1|字符|yyyy-MM-dd HH:mm:ss|表示时间使用的格式|
|args2|字符|留空|留空表示按照默认格式提交,否则按照填写格式提交times(时间戳秒) timems(时间戳毫秒) yyyy-MM-dd(自定义格式)|

    1.修复上面的time times 的问题
    2.引入nullable 如果值为空则不回传 比如 int? long? datetime?等
    3.目前还有一个问题，就是值更新的问题，比如timestamp=>yyyyMMdd的问题！
    4.3的问题应该修复了，注意dto中的值要和配置的一致，有值的时候，否则显示的时候会有问题，因为格式和数据不匹配!

    1.daterange是否也要支持时间戳模式 那就有datetime timestamp timestamps的模式了

## daterange
主字段需要设置为daterange,其他字段需要设置hidden,在最后组合数据的时候，会基于参数生成对应的,应该要设置为可null格式

|字段|类型|示例|说明|
|----|----|----|----|
|name|str|daterange||
|args1|字符|sdate|表示开始时间，最后会传送yyyy-MM-dd 00:00:00的格式数据|
|args2|字符|edate|表示结束时间，如果你选择2024-08-31,最后上送的会是2024-08-31 00:00:00|
|args3|字符|yyyy-MM-dd 00:00:00|表示时间的格式化，默认使用yyyy-MM-dd 00:00:00|
|args4|字符||time,times和留空，留空表示日期格式，和args3有关，times timems表示时间戳的秒和毫秒格式|

    1.基于上面的问题，daterange也进行升级，支持long的格式！


## minbtn
add.25.04.10
用于表格中的输入框后面的小按钮，注意当前字段必须是edit模式的时候，

|字段|类型|示例|说明|
|----|----|----|----|
|name|str|minbtn||
|args1|字符|点击新增|表示按钮显示的文字|
|args2|字符|global_func_btn(this)|表示要执行的函数，如果为空，则为默认函数func_min_btn_click(this,'{{:=item.id}}','"+item.name+"')"|
|args3|字符|minbtn|表示要用哪个class样式|
|args4|字符|备用|备用|

    1.修复获取行数据的checkbox的数据的问题
    2.???关于array的query传递和读取的问题，这个应该有问题，需要测试下,比如user_ids =[1,2,3] 在query中应该是user_ids=1&user_ids=2&user_ids=3 ...
    3.index.html中关于每页大小可选配的支持！pagesize 在class的attribute中，多个之间使用,隔开，第一个是默认值！
    4.批量菜单等可能过多，需要支持title,这样就可以使用图标加title来布局更多,icon是支持多个的，其实就是class 


    1.主键key估计要引入PasteKey用于在创建或者更新的时候，在表单中隐藏这个对象？？？
    2.directsun啥时候有+ - ？

    1.其实有些请求可以本地缓存，我说的是前端(管理端)部分，比如获取addModel和获得listModel以及queryModel的接口数据！可以使用session本地存储！

    
    
    1.classA:ClassB,ClassA中有一个 public new string Name{get;set;} 如果ClassB中的类型不是string,会有问题，这个问题确实存在！！！
    2.1中如果new的类型一致，则不会有这个问题！这个可能要用覆盖来搞定，覆盖的话有一个问题，如果是排序呢... 会不会干扰

## query 强化
    1.关于query的强化，一个是object.array，一个是基础array
    2.啥类型基于当前字段的类型
    3.如果是object的应该是从session中读取，然后赋值
    3.对于普通的array，基础类型的array应该从query中读取！！！
    4.一个是读取，那么另外一个就是写入的问题，要基于query的特性写入
    5.所以object的query是针对本地的，而其他的就是兼容本地和服务端了，因为服务端可能要用
    6.要兼容旧版本的！

我的需求是这样的
```
    fielda
    fieldb
    items
        object1
            field1
            filed2
        object2
            field1
            field2
```
所以说上面的其实是有问题的，因为objects如何显示在表单中???

那么逻辑应该是这一段的
如果items是选择的，就是表格编辑模式
则配置
    批量模式，勾选后
    打开一个object模式的表单
    提交后，在function中和items组合，然后提交！！！
搞定，完结！


    1.分页控件更换，更换成可以修改的，主要是为了引入页大小，就是每页多少条数据!
    2.关于时间戳在UI中的显示问题！

    1.选项selected的问题，不再由数据模型设定，由字段值决定！

## 阶段性修复
    1.outer的返回，args4中附带+关键字的时候的解析逻辑修复
    2.image的值如果为null，是字符串null的时候，进行null赋值，表示没有，而不是地址null，也就是编辑的时候的破图！！！
    3.由2引申出一个新的问题，image的读取，是不是读取为null了？
    4.修复多按钮改版引发的名称丢失的问题！

    1.markdown richtext添加条件判断，只有当UI有显示的时候才读取值，否则不读取！
    2.发现markdown和richtext切换的时候有问题，是1的问题，被双重赋值了！

    1.view中的style hidden empty有问题啊 会冲突，js端应该要先组合，UI中统一渲染才对！！！
    2.echarts采用动态引入，估计后面的还有些也会这样引入...

## 25.06
    1.添加代码，阻止默认提交，之前一直没注意，就是输入框的回车，会触发form的默认提交
    2.关于表单中的图片的大小限定
    3.关于lselect的宽度修改，允许大一些，保证每一个选项是80px宽度的
    4.关于index中的图片，默认转化以,分割的图片显示，也就是默认是多图模式显示，这样可以兼容单图和多图的情况
    5.针对4，如果是string[]的情况... 是否需要先转化下?

    1.修复index中的select的问题，其实就是前后端都会实现一个扩展xxx_expend_select_values,只是一个是后端来实现，一个是前端来实现，殊途同归！
    
## 25.07
    1.是否添加一个需求，比如新增完成后，跳转到详细。。。 或者特定页面，那么API要返回对应的ID
    2.更新同理，因为有些时候不一定是管理端再使用！
    3.基于上面的，包括是否刷新页面等！

    1.之前model缓存没有考虑关于model=bind model=view的情况，现在给他拆分开来
    2.目前有一个新的需求是。。。 我用通用AppService的，发现一个字段转数据库字段的问题，比如你用Dto中为status_int在数据库为Status这就需要申明了，特别是在查询上
    3.因为表格页面的查询区域和下面的表格区域，有些东西是共享的，会导致穿透，这个时候就希望不一样的字段了
    4.2.3可能会归纳为FieldName("UserId") 表示当前对应的是哪个字段名称，比如user_id上面标记[PasteFildName("UserId")],对于UI没啥影响，主要是后端自己

    1.比如添加一个OkAction表示成功后执行的内容

### OkAction
表示成功后的动作，比如新增表单之后，比如编辑之后,只能有一个特性

|字段|类型|示例|说明|
|--|--|--|--|
|name|str|okaction|表示当前特性的名称|
|args1|str|current|current/parent|
|args2|str|href|href/close/href/replace/open|
|args3|str|url|可以包含{{id}}宏，替换返回值即可|

//上面的有歧义，一个是父级的 一个是当前的 href还是replace



## 25.08.001
    1.image特性，如果你再加一个unit特性，变成图片选择器了。。。，需要实现云图逻辑
    2.image的选择数量纠正，image单图的时候删除的操作支持了！
    3.之前的清除按钮这么难写，那就改成icon模式

    ?.考虑是否需要引入 统计 合计模块!



## 25.09.001
    1.基于chart的特性，我想引入一个新的模式
        a.访问中querystring附带request=one表示合并模式
        b.只有请求page==1的时候，会返回新的格式
            totalCount
            items
            pasteModel:数据格式信息
            pasteTotal:合计信息，基于查询统计，如果是页面的，直接标注sum特性即可
        
以下是后端案例代码，其实就是动态创建数据模型，重点看readModel
然后是返回数据的时候，在page=1的时候进行pasteModel赋值
```csharp
        /// <summary>
        /// 读取ListDto的数据模型
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
            model.Properties.Add(new VoloModelProperty { Title = "周三", Name = "day3", DataType = "Int32", });
            model.Properties.Add(new VoloModelProperty { Title = "周四", Name = "day4", DataType = "Int32", });
            model.Properties.Add(new VoloModelProperty { Title = "周五", Name = "day5", DataType = "Int32", });
            model.Properties.Add(new VoloModelProperty { Title = "周六", Name = "day6", DataType = "Int32", });
            model.Properties.Add(new VoloModelProperty { Title = "周日", Name = "day7", DataType = "Int32", });

            return model;
        }

        /// <summary>
        /// 获取 测试单次返回模式，queryString中需要参数和值request=one
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

```



## 25.09.002
    1.调整view的class，自定义class的范围，直接到项的最外围，这样可以自定义，比如一行3个数据等！

## 25.10.01
    1.修复loadlib template的问题，添加异步支持，主要是为了先让loadlib执行完成，再执行template，因为可能需要上一步的结果
    2.loadlib添加对.html的支持，主要是为了载入template

## 25.10.02
    1.是否添加对列的排序的支持，支持本地存储，这个和后端无关
    2.由上面的需求，是否添加一个特性config，表示修改页面的config的信息，应该是一个obj的jsonString,Parse之后，需要覆盖本地的配置

## 25.10.03
    1.组别添加有一个BUG，就是特性不会跟随，比如第一个组别中有一个售价，里面有fentoyuan特性，则添加添加组后，新的组不会有这个，所以要做一个字段跟随
    2.顺带的，那删除肯定有问题，至于编辑，不涉及
    3.经过查询，新增只是更新了UI，没有更新对应的数据信息，对应的删除也有这个问题，一并更新!
    4.如果之前有数据，新的查询没有数据，tbody没有处理，需要更新这个bug
    
    
## 25.11.001
    1.有一个毛病，我不喜欢写更新日志！！！！
    2.修复动态表的问题，就是给你一串Json值，你要编辑他的这个功能，之前其实实现了，是后面更新给覆盖了！！！
    3.添加对PasteLink的支持，
    4.修复之前引入的，编辑，新增后在table写入数据，如果是dialog的ta'b'l会有问题，会更新到最顶级的页面中，这个问题现在修复了
    5.添加select模式下的多选，这个多选指跨页多选，也就是多选模式下，如果你选择了某一项，翻页的时候这个不会清空，这你就可以跨越多选了！


## 25.11.002
    1.imgexpend args1:mediaid args2:url(默认value就是url) args3:field字段，针对val的，图片预览的一直都是url
    这种情况需要服务端处理后返回！
    2.image的value添加null判断
    3.image的placeholder修改放在title上面，不再用span显示，会站地方，很变扭
    4.接口端，说明应该要做一次"''的替换工作？

## 25.11.003
    1.添加对tosize的完善，加入了函数parseFileSize和formatFileSize,就是KB,B,MB,GB,TB等互转


## 25.12.01
    1.去除“”空值去除字段的问题，就是比如你配置name的值为“”，最后提交的时候没有这个对象！
    2.上面的问题，可能要引入一个配置问题!

## 25.12.08
    1.model=select的时候，如果没有数据，是否直接调用新增，新增一个
    2.新增后，是否选中... .. .

    1.model=bind的时候extendBind需要在最后一位！！！

## 25.12.11
    1.修复多层级后的display的问题，特别是在更新的时候，就是UserId显示ExtendUser.name的时候，之前会显示的是父级的，目前修复了!

## 25.12.17
    1.修复select模式下table丢失了header的bug
    
## 25.12.23
    1.针对outer的自定义路径，新版本支持写法有3
        a: userInfo 表示./pasteform/index.html?path=userInfo
        b: ./pasteform/index.html?path=userInfo&level=2 表示直接全路径，一般是为了引入特殊参数，比如这里的level=2
        c: userInfo&level=2 表示./pasteform/index.html?path=userInfo&level=2
        总结，其实还支持宏，比如
        userInfo&level={{:=item.level}} 表示从当前的信息中获取表单项level的值带入！
    2.就是绑定的时候，来一个批量操作，估计先用循环点击的方式，批量提交的话得API配合，可以自定义实现！

## 25.12.29
    1.新增，编辑后，支持array更新到表格，之前是一行，现在可以多行了handlerUpdateRow

## 25.12.30
    1.加载下一页的时候，有些东西归位，比如左边的全选，右边的全选!
    2.修复关于outer string 在Index.html中，默认为null的时候，会带上null字符串进行查询的问题!
    3.修改index的unit的排版问题，不知道会不会有问题，目前测试是正常
    4.如果table td itd 下面有p标签，则这个时候要他自动换行才符合需求，这种一般表示html内容了！

## 25.12.31
    1._map_values之前是不是漏了啥，提交的时候没有提交到服务端？object objects的特性好久没用了
    2._global_expend_object应对比如extendUser的提交，这个有些地方需要使用，感觉没啥必要提交的！限定一下，model=object再返回
    3.扩展信息可以从session_key中获取，也就是比如_global_expend_object
    4.对于一些全局遍历迁移到form.js中，比如_global_class_attributes _global_data_properties
    5.修改样式规则，pasteform.css放后面，用于三方重写覆盖基础样式!

    1.想引入一个时间快选，比如今日，昨日，本周，上周，本月，上月这样的... .. . 感觉直接设计到daterange好，按照配置是否显示... .. .
    2.表格的编辑，详细，删除，添加id的判断，有些时候没有id，或者id不符合，则不需要这个！ 比如后端动态生成的，没有Id的时候
    3.api的提交和请求改成promise的模式，减少重复操作的问题!

## 26.1.1
    1.修复之前的promise的问题，就是没更新table的行
    2.修复enum的lselect的值的类型的问题，比如不会回传“1”而是基于lselect的value的类型返回，比如1

## 26.1.5
    1.修复autoUrl的问题，主要是修正./的问题，之前能用也没注意看！后面才注意到，有./ ../等情况的处理
    2.open_window添加自动追加model=dialog的情况，主要是针对index.html的情况，比如查看商品列表，查看货品列表，从商品列表查看货品列表，这个时候的货品列表我们就希望不看到商品信息，因为都是同一个，所以只要不看的配置[PasteHidden("view")]即可，因为默认是view模式，而本次更新弹出的变成dialog模式，所以就隐藏了！
    
## 26.1.6
    1.上面说得不行，要自己添加，不能自动！model=dialog不自动追加，需要的自己手动！
    2.index.html的btnadd，默认不隐藏，以前在model=select/object的时候隐藏是为了避免套娃，后面改成top模式后，可以把这个去掉!
    3.???edit模式下，是否支持outer???，点击后，弹出，然后回传写入... 理论上是可行的...

## 26.1.8
    1.FuncClickSure函数重写，点击最后一层的确定按钮，防止多层的时候干扰!
    2.关于view.html的model=object模式，添加父级接收的支持！handlerReadOpenValue 注意看iframe的模式!，保留历史功能兼容

## 26.1.10
    1.关于[PasteOuter]打开选择表，如果没有数据，则新增后，会更新到table,结果点击无效的问题，其实就是没同步到global_page_data中
    2.关于选定的，不删除，则global_page_data也需要保留
    3.关于select模式下，切页保留global_page_data数据，也就是选中了的保留，引入了__selected字段，表示是否选中！！！

## 26.1.13
    1.view中，outer设置readonly的时候，不需要清空按钮，也不需要点击事件,表示只读！！！
    2.detail添加对html的支持，form.js添加了handlerBuildDynamicData函数，主要用于处理宏{{:=item.name}}这样的替换问题

## 26.1.16
    1.关于table的outer，支持edit模式，有清空功能
    2.关于行数据，id作为数据描点，其他字段允许基于global_page_data进行匹配后补充，比如你有一个字段tableId，不想在table中显示,getSelectedRowsData();的时候，将会追加这个字段值

## 26.1.20
    1.修复之前的表格最后一列的宽度的问题，主要是jquery的选择器的问题，选择到了其他的th


## 26.03.001
    1.修复index.html中得edit模式下select lslect不适配编辑模式得问题


## 26.03.002
    1.表格拖拽顺序那个放前面,不然和排序，mark等冲突了
    2.show_style回传选中的，这样后端就知道用户当前选择的是那个了
    3.支持传参，比如size等，这个其实要看和后端查询的时候要啥！注意page不适用，因为后端在page==1的时候做了额外的查询
    4.mark支持悬停模式和访问模式，这个要看配置[PasteMark]，PasteDocument要更新到最新版本

## 26.03.03
    1.修复echarts的y标轴没有刻度和数值的问题！！！

## 26.04.02.01
    1.privateHandlerPageItemData调整到table或者chart之前，这样就能处理比如fentoyuan等的问题

## 26.04.03.01
    1.修复动态载入echarts.min.js的问题，一个是竞争的问题，一个是要确保加载完成，而不是引入就行
    2.之前有一个版本升级了，支持outer多个参数返回，默认比如 id name 表示id是值要给字段，当前字段，现在支持这样写，比如id,userid name
    表示把当前的Outer选择的对象，id赋值给当前输入框作为value name复制给当前输入框作为友好显示 userid赋值给userid这个字段，且这个字段要配置隐藏！

## 26.04.05.01
    1.修复Directsun的时候，没办法select的问题

## 26.04.06.01
    1.添加函数handlerMicroString 主要是针对当前表单值替换和当前location.search，比如[PasteButton]现在支持宏了，比如你可以这样写
    [PasteButton('一键排序','func_auto_sort(this,\'{{:=item.obj_id}}\')')]这个时候如果你得表单有obj_id或者url有，就会被替换，注意表单得优先级更高!

## 26.04.08.01
    1.index.html的默认输入框移除handler_keyup，解决不能copy的问题！


## 26.04.12.01
    1.textarea可以附带style,和[PasteStyle]的分割开来，各自管各自的

## 26.04.15.01
    1.调整findAndCallHandler，主要针对多个页面打开后，的form表单提交后更新table行的问题！

## 26.04.18.01
    
```    
    if(_config.model=="add"){
        //加载query如果有 只有新增的时候需要，编辑模式不需要！！！
        LoadQuery(properties);
    }
```
上面问题修复的是更新的时候，query影响了数据本身，这不是允许的，或者说这不是预期要的，只有add的时候才要这样做!

## 26.04.25
    关于directsun的，UI删除后，如果后端有默认值，会使用默认值的问题，添加了基于group判断，如果没有则设置为null,需要在dynamic之后!
    比如args__0这样的object对象组

## 26.05.01
    1.优化表格的orderby,不再使用字体，改成path实现!

## 26.05.04
    1.修复enum[]的显示问题，就是有多个的时候!
    

## 26.05.07
    1.添加对Double Decimal Float的支持，添加特性step标注UI的小数位数
    2.select的提交值是string的问题，应该跟随来源的信息，比如number类型!主要发生在enum格式上

## 26.05.08
    1.menu整改 args1支持name title模式

## 26.05.12
    1.修复表格对html的支持，也就是支持写宏等，注意html如果args1为空，则读取value

## 26.05.30
    1.菜单盒子的逻辑修改了，使用visibility而不是之前的disply,然后是菜单变成横向的，防止在末尾的时候被遮盖问题!
    2.删除的提问修改，之前是修改状态!

## 26.06.13
    1.index.html添加配置freeze表示表格是否启动冻结模式，默认启动
    2.index.html参数载入顺序问题，先query，然后是基于这个判断其他的

## 26.06.20
    1.修复入参 query到form的隐藏的问题

## 26.07.01
    1.index.html 搜索区域，回车查询
    2.添加对unit的显示 prefix的话有点问题

### trclass
表示为数据行 tr标注一个class
1.作用于Class 
比如
```
[PasteTrclass("trclassname")]
public class UserInfoListDto{
    xxxxx

    [PasteHidden]
    public string trclassname=> "levelx";
}
```

## 26.07.02
    1.model==select模式下改版了
    2.选择为switch 而不是按钮
    3.支持导入，需要使用参数__selected，允许多个，比如__selected=1&__selected=2

## 26.07.09
    1.open_window添加第五个可选参数 endcallback 表示完成后回调
    使用场景，比如你有一个列表，用户列表，其中有一个字段显示他有几个角色，点击打开绑定角色，完成的时候，你可以读取这个用户的ListDto这样就可以立马更新绑定数量了，所见即所得！
    2.lselect多选和单选，添加样式描点 lsingle lbatch 和ulitems同级别 用于区分样式
    3../lib/img_dray.js  ./lib/star_score.js 把图片拖拽排序和打分的独立出来，后续考虑是使用PasteLoadlib还是系统自动引入...
    4.基于3，后续考虑很多特殊的东西 都要用外载模式，比如之前的md richtext就是外载 自动模式！

## 26.07.10
    1.修复PasteFile的默认参数args1的问题

## 26.07.11
    1.优化金额精度转化的问题！

## 26.08.01
    1.loaditem添加对输入框，lselect select 等的支持！
    2.引入readFormDataSkipValidity函数，表示读取当前表单数据，不进行验证，特殊的情况下使用，比如你要获悉当前表单的结果的时候
    3.修复 handlerMicroString handlerBuildDynamicData(后续用handlerMicroString) handlerBuildDynamicUrl 基于当前表单，然后
    4.添加对数组的支持，比如ids=1&ids=2&ids=3这样传递
    
## 26.08.02
    1.findAndCallHandler变更，允许传入urlKey表示对应页面的URL的关键字，用于追踪
    2.findAndCallHandler添加对当前窗口和父级窗口的支持，主要是不一定是iframe
    3.extendBind添加对findAndCallHandler的支持，这个需要API端返回对应的ListDto，当前或者父级都可，看需求,单单state的话不需要，200就会变更反状态
    4.daterange之前的比如2026-03-01 至 2026-06-01 返回的数据不好含06-01 一般的我们希望包含，你可以在配置中进行配置index.js中的config.next_end_date
    5.findAndCallHandler迁移到form.js中了，之前是只在view.js中
    
## 26.08.03
    1.修复关于多层打开后select_num的叠加问题，由数字转数组了
    

## 26.08.04
    1.添加对thclass的支持，主要用于表格头的大小拉升
    2.handlerUpdateRow添加返回值，涉及格式校验，所以false的时候应该继续往下找
    
## 26.09.01
    1.由于特性的读取的顺序有时候会变，引入了tempConfigButtonAdd,去报特性顺序无关的修改add按钮
    2.基于1的问题，后续将引入特性顺序Order的支持！仅仅作用于Class的特性生效

    1.pasteTotal没返回的时候，下方清空
    2.menuhtml，设置globalMenuCount++; 不然菜单列就被可能吃掉了

## 26.09.02
    1.本页数据和综合的问题，读取数据后，重置
    2.综合数据权重大于本页

## 26.09.03
    1.表格搜索区域，添加对fentoyuan的支持
    
## 26.09.05
    1.selects改版，之前是一个区域，选择，新版本改成一个浮层，显示的地方是(x) name1 name2 这样可以直接看到选择了哪几个！！！
    可以说就是lselect的改版，支持更多的多选！！！

## 26.09.06
    1.合并index.js和form.js的部分代码，也就是代码复用！
    2.修复表格数据行更新逻辑，先从顶级-顶级工作台 父级-父级工作台 自己 按照这个顺序，然后引入了数据格式，需要大于60%(update_row_min_match_rate),支持配置

## 26.09.07
    1.约定Enum开头的枚举，是number类型，这个放生在比如EnumRoleType[] types 默认值为Null 这个时候没办法推测是啥类型，估计后续要引入一个特性
    2.比如你传参role_type=5,如果不按照1来，可以能会推测出这个是字符串！，我们知道Enum一般是int类型，所以做一个约定！
    3.引入特性[PasteValuetype],最好是使用这个，如果你得枚举命名不合规的话

## 26.09.08
    1.修复之前的[]的selects的query问题，单个的时候直接显示，不显示数量(1) 多个的时候才显示！
    2.datetime,如果已经有daterange的时候，则不进行datetime的处理，其实PasteDaterange和PasteDate是互斥的！

## 26.09.09
    1.调整页面的页面区域的样式border-raduis:4px;
    2.修复1中的宽度问题，不知道啥时候设置的margin-right:100px;
    
    
## 26.09.10
    1.index.js引入_config.auto_menu_width 页面载入后，是否自动伸缩菜单区域的宽度