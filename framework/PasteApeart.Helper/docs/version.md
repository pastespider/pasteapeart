## 26.05.02
    1.修复PageOrderby的字段大小写问题
    2.添加PasteTitleAttribute，UI暂未实现
    3.PasteMenu的名称支持 name title的写法，空格隔开！！！

## 26.04.11
    1.修复PasteFieldAttribute的args2的问题，之前的直接被覆盖了，导致非=的判断有问题等！

## 26.03.03
    1.字段支持官方的，注意是官方的 JsonIgnoreAttribute JsonPropertyNameAttribute 如果你不是使用官方的，则你需要标注一下了
    2.添加特性 PasteIgnoreAttribute 官方的JsonIgnoreAttribute包含PasteIgnoreAttribute，如果标记了PasteIgnore则当前字段不返回信息给管理端，但是后端还是可以接收这个json的
    3.PasteIgnoreAttribute如果作用域Class，则再PasteBuilder的时候，不会对这个字段或者Class进行处理！
    4.PasteBuilder需要是版本26.04.01及以上

## 26.01.12
    1.名称也支持空格了，需要用_**_替换，主要用于哪里？比如enum的选择，你可以写这样的<i_**_style="color:red;">错误</i>
    2.优化权限的过滤器，继承于BtnCodeAttribute，这样统一处理，不过PasteShow还是最靓的仔，他比较特殊是反着来的！

## 26.1.11
    1.添加PasteShowAttribute表示控制显示还是隐藏，其实就是PasteHiddenAttribute的权限版本，如果有权限则不下发hidden，如果没权限，则下发hidden表示隐藏了，具体的还要看unhiddenmodel参数
    2.changeui,如果有多重链接，会咋样？？？感觉是修改了某一个项后，要去读这个项的值和是否有changeui，这个工作交给前端处理...

## 26.1.006
    1.IfMenu Menu优化，特别是IfMenu支持直接button类似的模式，不用自己写html代码，前端不变！
    2.Menu有重载，支持class模式了，前端是ifmenu模式，不过表达式恒成立而已！
    3.按钮判断支持path=* code=*的模式！

## 26.1.005
    1.添加对通用DefaultAppService对buttons的支持！

## 26.1.004
    1.针对之前的，添加对*的支持，比如Path=* 或者Code为* 这样就可以统配了

## 26.1.003
    1.上面考虑太多了，由于Path就是对应的，比如UserInfoListDto，所有不需要加action ... .. .

## 26.1.002
    1.上面的，添加限定，只有配置了btnCode且传入的IPasteButtonRole不为null才执行过滤！

## 26.1.001
    1.PasteMenu PasteIfMenu PasteButton PasteBatch添加对btnCode的支持，路径就是当前的XXXDto,Code就是这些特性的btnCode参数，这个参数不返回UI
    2.给按钮权限的时候，需要继承于IPasteButtonRole,里面有Path(当前Dto,示例UserInfoListDto) Code(自定义，用于匹配)两个参数

## 25.12.001
    1.PasteOrderby PasteOrder 添加判断，之前是AI提供的代码，没有仔细检查，就是OrderBy的时候 默认ACS没有检查到！
    2.处理多属性继承，重写的问题 比如 public new string Id{get;set;}

## 25.11.003
    1.针对有些情况，需要mediaid的图片情况，编辑的时候我们希望是可以查看预览，而提交要求是mediaid的情况，可以使用PasteImageExpend特性!
        对应的UpImage的返回值也要修改，也就是你上传到服务端，服务端还要上传到三方，然后写入比如media字段中！
        读取数据的时候，要进行处理，把mediaid转化成url给图片字段赋值，同时需要把mediaid赋值给PasteImageExpendAttribute这个特性
    PasteImage:表示这个字段是上传图片的
    PasteImageSelect表示可以使用图片库
    PasteExpend表示这个图片一般是Mediaid的模式，值保存的是媒体ID，而不是Url，服务端需要特别处理!

## 25.11.02
    1.添加simg也就是PasteImageselectAttribute用于标注这个图片可以从图片库选择,旧版本是Unit标注，新的引入了更加灵活得页面地址和参数等

## 25.11.01
    1.添加PasteLink和PasteInclude，两个都是服务端特性，用于变更的外键检查，比如UserGrade写入或者更新的时候你得检查UserId,GradeId是否合法!

## 25.10.02
    1.修复10.01版本中对PasteAuth支持不足的问题，现在可以支持多个PasteAuth了，注意是非权限关联的！
    2.当前版本支持多个PasteAuth，如果PasteAuth是配置了权限的话，需要手动处理PasteEnable!


## 25.10.01
    1.PasteAuth的false模式，支持自动转化，注意是转化成enable模式，如果是其他PasteAuth模式，需要自行处理!
    

## 25.09.04
    1.考虑添加Tolong(距离转化，米转KM) Tosize(字节转化字节转KB MB GB TB DB等)

## 25.09.04
    1.Description可以设置为空，表示故意留空！
    2.预先添加PasteHalfRow,表示半行显示，其实就是字段超出64后，不做单行标记处理
    3.PasteFieldQuery表示自建检索项
    

## 25.09.02
    1.修复enable PasteEnableAttribute的逻辑问题，这个问题导致新增编辑详细不听使唤的问题
    2.修改部分特性的说明，比如PasteOuterAttribute的args1

## 25.08.06
    1.directsun和sort一起使用的时候，sort时效的问题！
    2.上面引入了一个新的问题，那就是内部的排序... .. .
    3.注意内部会++排序
    4.忘记了，srot比较特殊，不再从特性中获取

## 25.08.04
    1.修改PasteAuth
    2.添加Enable针对Disable的反配置容易混淆的问题
    3.针对PasteAuth添加对应的帮助函数，用于判断是否有权限等

## 25.08.02
    1.添加标记，是否是服务端使用的特性，如果是则不返回给前端
    2.添加对accept的支持，也就是file特性PasteFile
    

## 25.08.001
    1.添加PasteBatchMutexAttribute表示Mutex是And关系，默认是Or关系

## 25.07.17
    1.text textarea添加对style/class/singlerow的支持
    2.由于前端配置信息的引入，disable添加反向标记，比如配置启用，这里配置禁用，以特性标注为准
    3.对应的添加了一些注释说明

## 25.07.16
    1.默认字段的预留注释，尽量减少代码显示,比如name id createDate userId desc mark等
    

## 25.07.15
    1.添加PasteAuth,用于DefaultAppService的授权判断，支持多个
    2.PasteDisable用于ListDto和Entity,一个是给UI，一个是给DefaultAppService

## 25.07.10
    1.PasteExtendBindAttribute重新定义
        在InputQuery的时候表示读取的时候使用，也就是读取绑定状态
        在ListDot中的ExtendBind上表示写入的时候,也就是更新绑定
    2.PasteFileAttribute添加filter过滤字符串，默认*.*


## 25.07.09
    1.添加对PasteExtendBindAttribute的支持 用于扩展绑定，比如角色绑定权限，用户绑定角色使用! 要绑定啥就在啥的Query和ListDto中标记！
    2.添加对PasteMutexAttribute的支持 用于写入，更新排重
    3.?绑定是否可以多个... .. . 理论上修改加判断后是可以的! 关键在查询和检索上

## 25.07.06
    1.如果没有Description特性，或者是没有注释(XML模式),则Name赋值给Title,这样防止空...
    2.如果是故意留空呢？？？这个留给后续考虑，估计得引入新的特性了
    3.修复Class的Title的问题
    4.添加okaction特性，管理端下一个版本支持！

## 25.07.03
    1.修复Class获取Desc引发异常的问题!

## 25.07.01
    1.重大更新！重大更新！
    2.添加配置 create_appservice 表示是否创建AppService,因为你可以使用默认的DefaultAppService
    3.DefaultAppService的内容如下，目前还有一些问题
    
    1.目前没有考虑Include的情况
    2.目前的多表查询是使用的分步查询，因为GroupJoin有一个问题，Queryable和DbSet分不清

    1.新增字段PasteFieldAttribute,表示当前字段要基于哪个字段(args1)查询，支持运算符(args2)
    2.支持时间等查询，包含，大小，不等等!

    1.25.07.02之后版本，支持非XML模式，也就是你得使用DescriptionAttribute这个特性，也是支持Enum的，写法一样，空格区分
    2.为了兼容旧版本，新的要调用AttributeReadModelProperty(非XML模式)


## 25.06
    1.进一步缓存XML文档，提高读取效率

## 25.05
    1.?字段覆盖的问题，因为有继承Class的时候，如果类型不一致，会导致2个字段！,所以是否需要排重？感觉自行调整Dto更划算！
    2.添加PasteSearch PasteShort等特性，主要用于生成代码，特别是外表的代码

## 25.04
    1.directsun子对象模式的时候添加对Hidden的支持，也就是默认隐藏，UI是有对应代码的
    2.缓存xml信息，读取当前文件夹(执行程序同级)下的xml文件，作为字段注释的数据源，提升第二次以上的读取速度
    3.添加charty chartx ui chartseries特性，用于表格数据的图表显示用
    4.添加hide,和hidden有点不一样，hidden可能被显示，hide不会有ui,hidden相当于readonly的隐藏款
    5.修复select lselect selects关于ignore的问题，之前的判断有问题,其实就是枚举类型的名称和值的问题！
    6.排序规则调整，也就是sort特性，拆分3块，小于0的，大于0的 为0的 按照升序排序，小于0的在前面，为0的保持 大于0的在后面

    7.添加对minbtn的支持，这个用于table的edit的输入框后面的小按钮
    8.新增switch在table模式下的功能，args4，默认为远端模式，也就是点击后会执行事件，设置为false后，不会执行事件表示本地模式，一般用于table.edit模式下！
    9.优化enum的取值
    10.优化xml的缓存等
    11.添加nullable的支持，如果没有值，则不提交，表示null

    12.nextkey修改成next_key,然后是启用args1，表示字段，主要用于查询的字段和返回数据的next_key字段不一致的问题！

## 25.03.30
    1.修复loadlib的适用范围问题，应该是class之前是property
    2.添加changeui字段 ，表示当前字段选中为某一个值的时候显示某些字段或者组，或者隐藏某些字段或者组
    3.group特性，3参数调整，多一个重载，open close hidden hidden表示隐藏，可以显示出来，其他2个表示展开还是收缩，意义不一样
    3.如果是directsun,可以结合hidden这样就可以创建隐藏的组了，主要是用于changeui特性！
    4.上一个版本写错了，应该是如果命中则显示这些字段，如果没命中则隐藏，这样如果是多模式，直接全部隐藏，然后按照默认值来配置！
    //不对 不对 1，3，5 如果是1，则隐藏35 如果是2，则隐藏15

## 25.03.28
    1.添加loadlib
    2.switch扩展args2,args3
    3.添加edit表示是否是编辑模式


## 25.03.22
    1.添加directsun
    2.添加nextkey
    3.添加loaditem
    4.子对象的null值问题,之前的有问题，传递的是父级的值
    5.支持list<object>等，默认给与默认组，需要有文档注释作为组名
    6.修复dynamic中enum没有添加的问题
    7.list的group添加edit模式，可以基于当前新增或者删除某一项

## 25.03.17
    1.处理这种问题EnumAction[] actions 这样的array枚举类型
    2.如果是DynamicReadTypeProperty/DynamicReadModelProperty则字段是object类型的会以__分割返回，注意数组不会返回
    3.sunform添加class表示使用哪个样式，多一个选择！

## 25.03.16
    1.取消sort的排序，只做标记，需要由API来排序，因为有些Model有多层级关系，直接排序会有问题,和意向不一定符合
    2.添加对sunform的支持，以一个iframe的形式嵌套子对象
    3.添加对datalist的支持，这里还有一个问题，就是UI中友好显示的问题，不过这个问题是UI侧考虑的
    4.selects,lselect,select,datalist的忽略列表，支持非int的数据类型，之前只支持Int的，也就是现在string也支持了
    5.添加对Type的支持，也就是你可以做动态读取了，请做好校验！！！(比如传递字符串"UserInfo",根据查询获得结构体和默认值)

## 25.2.15
    1.添加point特性
    2.部分特性补充字段，用于便于二开
    3.修复注释中的_*_不会替换为空格的问题
    4.调整部分特性的说明等

## 25.1.18
    1.在注释中_*_会被替换成空格显示

## 25.1.6
    1.修复部分单词错误的问题
    2.添加部分特性的重载，适用于不同使用环境
    3.部分添加缺省参数，便于二开也就是补全args1,args2,args3,args4等

## 25.1.1
    1.添加sort的支持，用于排序，小的排前面，默认为0
    2.添加readonly,sort,required

## 24.12.1
    1.添加特性Empty Objects Html等
    2.修改Outer的注释，主要是第一个参数其实是支持path的

## 24.11.9
    1.添加分组特性group,可以作用于class和字段
    2.修复之前的部分特性的说明

## 24.11.8
    1.添加新的特性File
    2.Password
    3.Split
    4.style
    5.button等
    6.注释掉文档找不到的Console打印
    7.修改dto的继承深度最大为5
    8.注意Dto或者Enum要存放在Domain,或者Applciation.Contracts子项目中

## 24.11.7
    1.注释文档找不到的打印
    2.添加orderby等特性
    3.修复outer特性的第二个参数错误的问题

## 24.11.16
    1.PasteLinkQuery的支持

## 24.11.15

    1.修复enum模式下的select lselect slects的ignore的配置问题，因为是obj类型，所以要用(int),也就是enmu只适用于int
    
