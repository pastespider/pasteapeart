var pageobject = null;
// var isselect = false;
var _classPath = "";
/**
 * 表格的数据
 */
var global_page_data = new Array();

/**
 * 当前页大小
 */
var _pageSize = 20;

/**
 * 表头的模板 template.args1
 */
var _template_head_html = null;

/**
 * 数据表格的模板 template.args2
 */
var _template_body_html = null;

// /**
//  * 查询的属性集合 迁移到form.js
//  */
// var _global_data_properties = null;

/**
 * ListDto的属性集合
 */
var _global_list_properties = null;

// /**
//  * 当前主表的属性列表 迁移到form.js
//  */
// var _global_class_attributes = null;

/**
 * 配置信息，配置这个模型名称
 */
var _config = {
    className: '',//需要和路径的一致，表示后端的对象
    model: 'view',//当前模式view bind select
    menubox: false,//是否有按钮盒子
    enable_view: false,//默认不显示详细
    disable_del: true,//默认禁止删除
    next: null,//nextkey模式下适用，内部自动赋值，需要配置PasteNextkey特性
    root_path: "api/app",//后端根地址
    show_type: 'table',//table表格显示chart图表显示
    ui_model: false,//是否被激活为table或者chart模式
    exchange_id: "id",//主键字段，默认为id
    show_key: true,//是否显示默认的id这个字段，也就是表的主键，有时候不想显示
    // single_select: true,//选择模式下，是否是单选模式，默认是单选模式 弃用，已经使用数量模式了，默认1
    select_num: 1,//选中多少个后确定
    has_menu: true,//是否有操作按钮，默认为true
    page_size: [20, 50, 100, 200],//分页的大小
    // page:1,//当前默认第几页，主要是为了传参覆盖 这个不能用，因为后端是第一页才会获取总量，统计等信息
    // page_default_size: 20,//默认的分页大小使用_pageSize
    drag_order_column: false,//拖拽修改列顺序
    empty_create: false,//查询没有数据的时候是否打开创建
    expend_search_box: true,//加载后是否展开搜索区域，这个主要针对默认搜索区域关闭的情况，比如select/bind模式下
    freeze: true,//是否冻结表头
    trclass: null,//表格中trclass的字段名称，实际中这个值中去获取
    batch: false//是否启用批量操作
};

/**
 * 关于多选的问题！
 */


//_config.next.key _config.next.type _config.next.map
/**
 * 排序字段 需要查询对象有orderby字段
 */
var _orderby = "id";

/**
 * 初始化事件数据等
 */
$(window).ready(function () {
    if (typeof pasteform_custom_config != 'undefined') {
        // _config = pasteform_custom_config; //这个变成全替换了，改成下面的覆盖写法
        _config = Object.assign({}, _config, pasteform_custom_config);
    }

    if (typeof parseSearch != 'undefined') {
        var searchObj = parseSearch(location.search);
        if (searchObj) {
            _config = Object.assign({}, _config, searchObj);
        }
    }

    //判断是否是选择模式，就是其他表单的外表
    // var _model = _apigetquery("model");
    var _model = _config.model;
    // if (_model != null) {
    // _config.model = _model;
    if (_model == 'select' || _model == 'object') {
        //warning 后续用select_num
        if (_config.pf_num) {
            _config.select_num = _config.pf_num;
        }
        // _config.expend_search_box = false;
    }
    if (_model == "bind") {
        // _config.expend_search_box = false;
    }
    // }
    // var _show_type = _apigetquery('show_type');
    var _show_type = _model.show_type;
    if (_show_type != null) {
        if (_show_type == 'table') {
            _config.ui_model = true;
            _config.show_type = 'table';
        }
        //这样写，防止参数被其他东西占用了
        if (_show_type == 'chart') {
            _config.ui_model = true;
            handlerChangeShowType(true);
        }
    }


    // var _path = _apigetquery("path");
    var _path = _config.path;
    if (_path != null) {
        _classPath = _path;
        _config.className = _classPath;
    }

    //localStorage.setItem(`${location.pathname}?path=${_classPath}`,pageSize);
    //切换排序 查询基于哪个排序基于哪个
    $(".table").on('click', '.ordersell', function () {
        _orderby = $(this).attr('dataval');
        $(".table").find(".ordersell").removeClass('ordered');
        $(this).addClass('ordered');
        readPageData(1);//直接基于这个查询第一页
    })
    if (_config.expend_search_box) {
        $(".searcharea").fadeIn();
    } else {
        $(".searcharea").fadeOut();
    }

    //如果有传递进来size大小，则进行UI变更
    if (_config.size) {
        if (_config.size != _pageSize) {
            _pageSize = parseInt(_config.size);
            $(".pagination-select").val(_config.size);
        }
    } else {
        var pageSize = localStorage.getItem(`pagesize:${_classPath}`);
        if (pageSize) {
            _pageSize = parseInt(pageSize);
        }
    }

    //取消表格冻结
    if (!_config.freeze) {
        $(".table").removeClass("tb_freeze");
    }

    addImgTapListenter();

    QueryModel();
});

/**
 * 绑定，预览图片
 */
function addImgTapListenter() {
    $(".ppbody").on('click', 'img', function () {
        var currentUrl = $(this).attr('src');
        var images = [];
        var currentIndex = 0;
        let zindex = 0;
        $('.ppbody img').each(function () {
            let src = $(this).attr('src');
            images.push({
                src: src,
                alt: $(this).attr('alt')
            });
            if (src == currentUrl) {
                currentIndex = zindex;
            }
            zindex++;
        });

        // 调用layer.photo方法
        layer.photos({
            photos: {
                "title": "图片预览", // 相册标题
                "id": 123, // 相册id
                "start": currentIndex, // 初始显示的图片索引
                "data": images.map(function (img, i) {
                    return {
                        "alt": img.alt,
                        "pid": i, // 图片id
                        "src": img.src,
                        "thumb": img.src
                    }
                })
            },
            tab: function (pic, layero) {
                console.log(pic); // 当前图片的一些信息
            }
        });
    });
}

/**
 * 显示为chart模式
 */
function handlerChangeShowType(ischart, elc) {
    if (ischart) {
        _config.show_type = 'chart';
        $(".data_echart").show();
        $(".table").hide();
        $(".batcharea").hide();
    } else {
        _config.show_type = 'table';
        $(".data_echart").hide();
        $(".table").show();
        $(".batcharea").show();
    }
    if (elc) {
        if (!$(elc).hasClass('selected')) {
            $(elc).addClass('selected').siblings().removeClass('selected');
        }
        readPageData(1);
    } else {
        if (ischart) {
            $('.show_type i').eq(0).addClass('selected').siblings().removeClass('selected');
        } else {
            $('.show_type i').eq(1).addClass('selected').siblings().removeClass('selected');
        }
    }
}

/**
 * 批量模式的全选和非全选
 * @param {*} elc 
 */
function tap_batch_check_change(elc) {
    var _val = $(elc).prop('checked');
    let all = $(".table .trbatch")
    for (let i = 0; i < all.length; i++) {
        $(all[i]).prop("checked", _val)
    }
}

/**
 * 以number的模式返回选中的对象的id
 * @returns 
 */
function FuncReadBitchCheckedNumber() {
    let all = $(".table .trbatch")
    let idarr = []
    for (let i = 0; i < all.length; i++) {
        if ($(all[i]).prop("checked")) {
            idarr.push(Number($(all[i]).closest('tr').attr("dataid")));
        }
    }
    return idarr;
}

/**
 * 以String的模式返回选中的对象的id
 * @returns 
 */
function FuncReadBitchCheckedString() {
    let all = $(".table .trbatch")
    let idarr = []
    for (let i = 0; i < all.length; i++) {
        if ($(all[i]).prop("checked")) {
            idarr.push($(all[i]).closest('tr').attr("dataid"));
        }
    }
    return idarr;
}

/**
 * 处理回传的数据，比如把int显示为Title显示等
 * @param {*} datas 
 */
function privateHandlerPageItemData(datas) {

    //初始化合计数据
    if (globalSumMap) {
        for (let aa of globalSumMap.keys()) {
            globalSumMap.set(aa, 0);
        }
    }

    //首次加载数据前 进行默认选中项的填充
    if (_first_page_data) {
        var selectArray = readQueryFromSearch("__selected");
        if (selectArray && selectArray.length > 0) {
            datas.forEach(item => {
                if (item.id) {
                    if (selectArray.indexOf(item.id.toString()) >= 0) {
                        item.__selected = true;
                        _first_page_data = false;
                    }
                }
            });
        }
    }

    datas.forEach(item => {
        if (gloablSelectMap) {
            for (var _k of Object.keys(item)) {
                //编辑模式的时候不要转化值
                if (gloablSelectMap.has(_k)) {
                    let _property = findPropertyByName(_global_list_properties, _k);
                    if (_property && _property.edit) {
                        continue;
                    } else {
                    }
                    var _val = item[_k];
                    item[_k] = gloablSelectMap.get(_k).get(_val);
                }
            }
        }
        for (let makey of globalSumMap.keys()) {
            if (item.hasOwnProperty(makey)) {
                globalSumMap.set(makey, globalSumMap.get(makey) + item[makey]);
            }
        }



        // var select_map = new Map();
        for (var _key in item) {
            var _val = item[_key];
            // if (_val == null) {
            // item[_key] = '';//这里有问题 要判断是不是string
            // }
            var _property = funcFindListPropertry(_key);
            if (_property) {
                if (_property.dataType == "String") {
                    if (_val == null) {
                        item[_key] = '';
                    }
                }
                if (_property.attributes) {
                    //这里可能阻断了... .. .
                    // if (_val) {
                    for (var _attribute of _property.attributes) {
                        _val = item[_key];
                        switch (_attribute.name) {
                            case "fentoyuan":
                                {
                                    item[_key] = (Number(_val) / 100).toFixed(2);
                                }
                                break;
                            case "tosize":
                                {
                                    item[_key] = formatFileSize(Number(_val));
                                }
                                break;
                            case "datetime":
                                {
                                    var _format = "yyyy-MM-dd HH:mm:ss";
                                    if (_attribute.args1) {
                                        _format = _attribute.args1;
                                    }
                                    if (_val == 'null' || _val == null) {
                                        item[_key] = '';
                                    }
                                    if (_val) {
                                        if (typeof _val == "number") {
                                            if (_val.length == 10) {
                                                _val = _val * 1000;
                                            }
                                            var _date = new Date(_val);
                                            item[_key] = _date.Format(_format);
                                        } else {
                                            var _date = new Date(_val);
                                            item[_key] = _date.Format(_format);
                                        }
                                    } else {
                                        item[_key] = "";
                                    }
                                }
                                break;
                            case "select":
                                {
                                    var select_key = `${_key}_expend_select_values`;
                                    if (!item.hasOwnProperty(select_key)) {
                                        // select_map.set(select_key, _property.selects);
                                        var selects = gloablSelectVals.get(_key);
                                        if (selects) {
                                            item[select_key] = selects;
                                        }
                                    }
                                    //这里有问题，应该从另外一个地方获取... .. .
                                    // console.log("_property.selects is",_property.selects);
                                    // //gloablSelectMap 从这里获取... .. .
                                    // if(gloablSelectMap){
                                    //     var rr = gloablSelectMap.get(_key);
                                    //     if(rr){
                                    //         //组合成一个selects
                                    //     }
                                    // }
                                }
                                break;
                        }
                    }
                    // }
                }
            }
        }
        // for (let k of select_map.keys()) {
        //     item[k] = select_map.get(k);
        // }
    });
}

/**
 * 基于历史选择 进行数据叠加
 * @param {*} datas 
 */
function auto_cache_history_selected(datas) {
    // 修复后的代码 - 只处理有id的有效数据
    var selectedItems = global_page_data.filter(x => x.__selected && x.id != null);
    if (selectedItems.length > 0) {
        // 1. 构建历史数据映射（只包含有id的）
        var historyMap = new Map();
        selectedItems.forEach(item => {
            var key = item.id.toString();
            historyMap.set(key, item);
        });
        // 2. 合并新数据（datas），只处理有id的
        var resultMap = new Map(); // 先复制历史数据
        datas.forEach(item => {
            if (item.id != null) {
                var key = item.id.toString();
                // 使用新数据覆盖，并设置 __selected = true
                if (historyMap.has(key)) {
                    item.__selected = true;
                    historyMap.delete(key); // 删除历史中已存在的项，避免重复
                    // resultMap.set(key, item);
                }
                resultMap.set(key, item); // 添加新数据到结果映射
            }
            // 没有id的新数据直接忽略
        });
        // 3. 组装最终数据：保持历史顺序 + 新增的
        var finalData = [];
        // 3.1 先按历史顺序添加
        historyMap.forEach((item, key) => {
            finalData.push(item);
        });
        // 3.2 再添加新增的（历史中没有的）
        resultMap.forEach((item, key) => {
            finalData.push(item);
        });
        if (finalData.length > 0) {
            global_page_data = finalData;
        } else {
            global_page_data = datas;
        }
    } else {
        // 没有历史选中数据时，只保留有id的新数据
        global_page_data = datas;
    }
}

/**
 * 显示页面数据到UI
 */
function _showpagedata(datas) {
    // global_page_data = datas;

    // //初始化合计数据
    // if (globalSumMap) {
    //     for (let aa of globalSumMap.keys()) {
    //         globalSumMap.set(aa, 0);
    //     }
    // }

    //外部处理
    // privateHandlerPageItemData(datas);

    $(".table").find("tbody").html('');

    if (_config.model == "select") {
        //第一页如果从query中载入呢... .. .
        // 这里需要处理历史选中数据的叠加 最终赋值给global_page_data;
        auto_cache_history_selected(datas);
    } else {

        global_page_data = datas;
    }
    // console.log(_template_body_html);
    if (_template_body_html != null) {

        //版本二
        var tplBody = $("#template_body_tbody").html();
        var tplLine = template(_template_body_html);
        // global_body_template = tplLine;
        var _tpl_item = template(tplBody);
        var _ahtml = _tpl_item({ list: global_page_data, templatetr: tplLine, config: _config });
        //应对select多选的情况，切换页码后，之前选定的不会删除
        if (_config.model == "select") {
            // 如果有tr则追加，没有则直接赋值
            if ($("table tbody tr").length > 0) {
                $(".table").find("tbody").append(_ahtml);
            } else {
                $(".table").find("tbody").html(_ahtml);
            }
        } else {
            $(".table").find("tbody").html(_ahtml);
        }
    }

    //显示合计数据到UI
    // console.log(globalSumMap);
    hanlderDisplaySumData();

    if (_config.drag_order_column) {
        if (typeof ColumnDragSorter != "undefined") {
            if (global_table_column_order_handler == null) {
                global_table_column_order_handler = new ColumnDragSorter('data-table');
                $(".btn_clear_order_column").fadeIn();
            } else {
                global_table_column_order_handler.rebind();
                console.log('1111执行重新加载顺序... .. .');
            }
        }
    }
}

//当前页面，用于排序的handler
var global_table_column_order_handler = null;

/**
 * 显示合计数据到UI
 */
function hanlderDisplaySumData() {
    if (globalSumMap) {
        if (globalSumMap.size > 0) {
            let ahtml = '当页合计:';
            for (let mapkey of globalSumMap.keys()) {
                var pro = findPropertyByName(_global_list_properties, mapkey);
                if (pro) {
                    if (pro.attributes) {
                        if (pro.attributes.some(x => x.name == "fentoyuan")) {
                            ahtml += `<span class="name">${pro.title}</span><span>${(globalSumMap.get(mapkey) / 100).toFixed(2)}</span>`;
                            continue;
                        }
                    }
                    ahtml += `<span class="name">${pro.title}</span><span>${globalSumMap.get(mapkey)}</span>`;
                } else {
                    console.log('没有找到', _global_list_properties);
                }
            }
            $(".totalsum").html(ahtml);
        }
    }
}

/**
 * 搜索区域的高度是否自适应
 */
// var _search_area_auto_height = true;
/**
 * 切换搜索区域的高度
 * @param {*} elc 
 */
function func_auto_searcharea(elc) {
    if (!$(".searcharea").is(":visible")) {
        $(".searcharea").fadeIn();
    } else {

        if ($(".pasteform").hasClass("oneline")) {
            $(".pasteform").removeClass("oneline");
            $(".tap_more .Hui-iconfont-arrow3-bottom").removeClass("Hui-iconfont-arrow3-bottom").addClass("Hui-iconfont-arrow3-top");
        } else {
            $(".pasteform").addClass("oneline");

            $(".tap_more .Hui-iconfont-arrow3-top").removeClass("Hui-iconfont-arrow3-top").addClass("Hui-iconfont-arrow3-bottom");
        }
    }
}

/**
 * 点击新增
 */
function tap_add_form(elc) {
    //检查是否需要传递参数下去
    var _linkquery = handlerBuildLinkQueryUrl();
    open_window('新增', `${handlerPasteFormPath()}view.html?path=${_classPath}${_linkquery}`, `1000px`, '90%');
}

/**
 * 新增
 * @param {*} elc 
 */
function tap_add_item(elc) {
    tap_add_form(elc);
}

/**
 * 点击编辑
 */
function tap_view_item(elc, id) {
    var _id = $(elc).attr('dataid');
    if (!_id) {
        _id = id;
    }
    if (!_id) {
        layer.msg("没有获取到有效的值，获取是调用方式不符！");
    }
    var _linkquery = handlerBuildLinkQueryUrl();
    open_window('编辑', `${handlerPasteFormPath()}view.html?path=${_classPath}&id=${_id}${_linkquery}`, `1000px`, '90%');
}

/**
 * 点击详细
 */
function tap_detail_item(elc, id) {
    var _id = $(elc).attr('dataid');
    if (!_id) {
        _id = id;
    }
    if (!_id) {
        layer.msg("没有获取到有效的值，获取是调用方式不符！");
    }
    var _linkquery = handlerBuildLinkQueryUrl();
    open_window('查看', `${handlerPasteFormPath()}detail.html?path=${_classPath}&id=${_id}${_linkquery}`, `1000px`, '90%');
}

/**
 * 点击删除
 * @param {*} elc 
 * @param {*} id 
 */
function tap_del_item(elc, id) {
    handler_tap_del(elc, id);
}

/**
 * 点击删除
 * @param {*} elc 
 * @param {*} id 
 * @returns 
 */
function handler_tap_del(elc, id) {
    var _id = $(elc).attr('dataid');
    if (!_id) {
        _id = id;
    }
    if (!_id) {
        layer.msg("没有获取到有效的操作对象，无法继续执行");
        return;
    }
    // 存储当前活动按钮索引
    let activeBtnIndex = 0;

    // 创建layer确认框
    layer.confirm('确定删除当前数据么，删除后不可恢复，确定删除?', {
        btn: ["确定", "取消"],
        // 添加键盘事件处理
        success: function (layero) {
            // 获取按钮
            const buttons = $(layero).find('.layui-layer-btn').children();

            // 设置初始焦点
            $(buttons[activeBtnIndex]).addClass('layui-layer-btn-focus');

            // 键盘事件处理函数
            const handleKeyPress = function (e) {
                // 左右箭头切换按钮
                if (e.key === 'ArrowLeft' || e.key === 'ArrowRight') {
                    e.preventDefault();

                    // 移除当前焦点
                    $(buttons[activeBtnIndex]).removeClass('layui-layer-btn-focus');

                    // 计算新索引
                    if (e.key === 'ArrowLeft') {
                        activeBtnIndex = activeBtnIndex > 0 ? activeBtnIndex - 1 : buttons.length - 1;
                    } else {
                        activeBtnIndex = activeBtnIndex < buttons.length - 1 ? activeBtnIndex + 1 : 0;
                    }

                    // 设置新焦点
                    $(buttons[activeBtnIndex]).addClass('layui-layer-btn-focus');

                    // 回车键触发当前焦点按钮
                } else if (e.key === 'Enter') {
                    e.preventDefault();
                    $(buttons[activeBtnIndex]).trigger('click');

                    // ESC键触发取消（第二个按钮）
                } else if (e.key === 'Escape') {
                    e.preventDefault();
                    $(buttons[1]).trigger('click');
                }
            };

            // 绑定键盘事件
            $(document).on('keydown.layerConfirm', handleKeyPress);

            // 为每个按钮添加焦点切换事件
            buttons.each(function (index) {
                $(this).on('mouseenter focus', function () {
                    buttons.removeClass('layui-layer-btn-focus');
                    $(this).addClass('layui-layer-btn-focus');
                    activeBtnIndex = index;
                });
            });

            // 对话框关闭时清理事件
            $(layero).data('closeCallback', function () {
                $(document).off('keydown.layerConfirm');
            });
        }
    },
        function (index) { // 确定按钮的回调
            _apipost(`${auto_path()}/${_id}/remove`, true, '', (c, o) => {
                layer.close(index);
                if (c == 200) {
                    $(elc).html('已删除');
                }
                // 清理键盘事件
                $(document).off('keydown.layerConfirm');
            });
        },
        function (index) { // 取消按钮的回调
            layer.close(index);
            // 清理键盘事件
            $(document).off('keydown.layerConfirm');
        });
}

/**
 * query model=select 的时候作为选择一个对象使用，会使用postMessage向上一个页面推送消息
 * 这里是否需要修改成支持多选模式?
 * 即将作废 使用下面的switch模式
 */
function tap_select_item(elc) {
    var targetId = $(elc).attr('dataid');
    var findSelect = null;
    for (var item of global_page_data) {
        if (item.id.toString() == targetId) {
            findSelect = item;
            break;
        }
    }
    if ($(elc).hasClass('selected')) {
        $(elc).removeClass('selected');
        if (findSelect) {
            findSelect.__selected = false;
        }
    } else {
        $(elc).addClass('selected');
        if (findSelect) {
            findSelect.__selected = true;
        }
    }
    read_select_values();
}

/**
 * 选择模式下 选择
 * @param {*} elc 
 */
function tap_select_switch_item(elc) {
    var targetId = $(elc).attr('dataid');
    var findSelect = null;
    for (var item of global_page_data) {
        if (item.id.toString() == targetId) {
            findSelect = item;
            break;
        }
    }
    if ($(elc).prop('checked')) {
        if (findSelect) {
            findSelect.__selected = true;
        }
    } else {
        if (findSelect) {
            findSelect.__selected = false;
        }
    }
    read_select_values();
}

/**
 * 通知上级点击确定
 */
function sure_select_close_page() {
    // if (find != null && _config.single_select) {
    var _top = window.top;
    if (_top) {
        if (_top.FuncClickSure) {
            _top.FuncClickSure();
        }
    }
}

/**
 * 读取选定的值
 */
function read_select_values() {
    var datas = global_page_data.filter(x => x.__selected);
    if (datas && datas.length > 0) {
        if (_config.select_num == 1) {
            window.SelectObject = datas[0];
            sure_select_close_page();
        } else {
            window.SelectObject = datas;
            if (_config.select_num == datas.length) {
                sure_select_close_page();
            }
        }
    } else {
        window.SelectObject = null;
    }
    // var items = $(".table").find('.selected');
    // if (items && items.length > 0) {
    //     var ids = new Array();
    //     for (var k = 0; k < items.length; k++) {
    //         var _elc = items[k];
    //         ids.push($(_elc).attr('dataid'));
    //     }
    //     var datas = new Array();
    //     if (global_page_data.length > 0) {
    //         for (var _id of ids) {
    //             for (var one of global_page_data) {
    //                 if (one.id.toString() == _id) {
    //                     datas.push(one);
    //                 }
    //             }
    //         }
    //         if (datas.length > 0) {
    //             if (_config.select_num == 1) {
    //                 window.SelectObject = datas[0];
    //                 sure_select_close_page();
    //             } else {
    //                 window.SelectObject = datas;
    //                 if (_config.select_num == datas.length) {
    //                     sure_select_close_page();
    //                 }
    //             }
    //         }
    //     }
    // } else {
    //     window.SelectObject = null;
    // }
}

/**
 * 获取选中行的数据，是一个[object]格式 基于可编辑的表格实现，需要edit特性配合
 * @returns 
 */
function getSelectedRowsData() {
    const selectedRows = [];
    // 遍历所有选中的复选框所在的行
    $('.table .trbatch:checked').each(function () {
        // const row = $(this).closest('tr');
        const rowData = handlerReadRowData(this)
        selectedRows.push(rowData);
    });
    // console.log(selectedRows);
    return selectedRows;
}

/**
 * 读取当前行数据
 * @param {*} elc 
 * @returns 
 */
function handlerReadRowData(elc) {
    const row = $(elc).closest('tr');
    const rowData = {};
    // 遍历行中的每个 td 元素
    row.find('td[dataname]').each(function () {
        const dataName = $(this).attr('dataname');

        if (dataName) {
            if (dataName.indexOf(".") > 0) {
                return true;//这里我要跳出
            }
        }

        const dataType = $(this).attr('datatype');
        const valType = $(this).attr('valtype');
        let value;
        const input = $(this).find('input:not([type="checkbox"])');
        const select = $(this).find('select');
        const cbox = $(this).find('input[type="checkbox"]');
        if (input.length > 0) {
            value = input.val();
        } else if (select.length > 0) {
            value = select.val();
        } else if (cbox.length > 0) {
            value = $(cbox).is(':checked') ? 'on' : 'off';
        } else {
            value = $(this).text().trim();
        }

        if (dataType == "outer") {
            if ($(this).find("input[type=text]").length > 0) {
                value = $(this).find("input[type=text]").eq(0).val();
            }
        }

        switch (dataType) {
            case "Int32":
            case "Int64":
            case "UInt32":
            case "UInt64":
            case "number":
            case "Double":
                {
                    if (value) {
                        value = Number(value);
                    }
                }
                break;
            case "fentoyuan":
                {
                    value = Number(value) * 100;
                }
                break;
            case "Boolean":
            case "bit":
            case "bool":
            case "switch":
                {
                    if (value === 'on' || value === '1' || value === 'checked') {
                        value = true;
                    } else {
                        value = false;
                    }
                }
                break;
            case "Double":
                {
                    value = parseFloat(value);
                }
                break;
            default:
                {
                    switch (valType) {
                        case "number":
                        case "int":
                            {
                                if (value) {
                                    value = Number(value);
                                }
                            }
                            break;
                    }
                }
                break;
        }
        //是否还有其他需要解析的
        rowData[dataName] = value;
    });

    if (rowData.id) {
        if (global_page_data) {
            for (let item of global_page_data) {
                if (item.id == rowData.id) {
                    for (let afield in item) {
                        if (afield.indexOf(".") == -1) {
                            if (!rowData.hasOwnProperty(afield)) {
                                rowData[afield] = item[afield];
                            }
                        }
                    }
                    break;
                }
            }
        }
    }

    return rowData;
    //可以考虑和list的item进行组合，进行完整数据回传，比如基于id来判断，如果id无效，则不继续了... .. .
}

/**
 * 查询模型信息
 */
function QueryModel() {
    var request_str = _apigetquery("request");
    if (request_str != null && request_str == 'one') {
        readPageData(1);//request=one模式，直接读取第一页，在第一页中返回对应的数据
    } else {
        //读取数据模板
        _apiget(`${auto_path()}/readListModel${location.search}`, true, (c, o) => {
            if (c == 200) {
                handlerModelFormatConfig(o, false);
            }
        });
    }
}

/**
 * 表格edit模式下的清理
 * @param {*} elc 
 */
function handlerEditClean(elc) {
    //未完待续
    $(elc).parent().find("input").val('');
    var elcTr = $(elc).closest('tr');
    var elcTd = $(elc).closest('td');
    var dataName = elcTd.attr("dataname");
    var dataId = elcTr.attr('dataid');
    console.log('将要删除 id', dataId, " name", dataName);
    if (dataId && dataName) {
        if (global_page_data) {
            for (let item of global_page_data) {
                if (item.id == dataId) {
                    delete item[dataName];
                    break;
                }
            }
        }
    }
}

/**
 * 把数据格式转化成UI信息或者是配置
 * @param {*} obj 
 */
async function handlerModelFormatConfig(obj, pageData) {
    if (obj.title) {
        $(".ppbody .st").find(".sn").html(obj.title);
        this.document.title = obj.title;
    }
    if (obj.desc) {
        $(".ppbody .st").find(".idesc").html(obj.desc);
    }
    //class模型的属性列表
    await handlerQueryClassAttribute(obj);

    _global_list_properties = obj.properties;

    //如果没有主键id
    if (obj.properties) {
        const hasIdProperty = findExistsPropertyByName(_global_list_properties, 'id');
        if (!hasIdProperty) {
            _config.exchange_id = _global_list_properties[0]?.name;
            const keyPropertyName = findKeyAttributeProperty(_global_list_properties);
            if (keyPropertyName) {
                _config.exchange_id = keyPropertyName;
            }
        }
        //隐藏主键在表格中
        if (!_config.show_key) {
            if (_config.exchange_id) {
                var find = findPropertyByName(_global_list_properties, _config.exchange_id);
                if (find) {
                    find.hidden = true;
                }
            }
        }
    }

    //模型处理，如何显示外表 比如cate.name 
    handlerModelFieldcolumnProperty(obj.properties);

    if (_config.model == "bind") {
        //确保最后一项extendBind在最后面
        obj.properties = moveExtendBindToEndInPlace(obj.properties);
        _config.has_menu = false;
    }

    if (_config.has_menu) {
        if (!_config.enable_view && _config.disable_del && _config.disable_add && _config.disable_edit) {
            //如果按钮个数为0 ，则禁用
            if (globalMenuCount == 0) {
                _config.has_menu = false;
            }
        }
    }
    if (_config.model == "select") {
        _config.has_menu = true;
    }
    //禁用新增 禁用详细 禁用编辑 且没有menu ifmenu ... .. .

    if (_template_head_html == null) {
        _template_head_html = $("#template_header").html();
        _template_head_html = removeEmptyLinesRegex(_template_head_html);
    }
    var _modelhtml = template(_template_head_html, { list: obj.properties, config: _config });
    _modelhtml = removeEmptyLinesRegex(_modelhtml);

    if (_config.batch) {
        if (_config.batchs.length > 0) {
            var _batch_template = $("#template_batch").html();
            var _batch_html = template(_batch_template, _config);
            $(".batcharea").html(_batch_html);
        }
    }

    //一级模型 转化成 二级模型
    if (_template_body_html == null) {

        //单个tbody版本 旧的版本，未拆分 版本一
        // var _template_body = $("#template_body").html();
        // var _bodyhtml = template(_template_body, { list: o.properties, config: _config });
        // _template_body_html = _bodyhtml.replace(/{{/g, '<%').replace(/}}/g, '%>');

        //新的版本，拆分了，注意写入的时候，需要读取模板 版本二
        var _template_body = $("#template_body_line").html();
        var _bodyhtml = template(_template_body, { list: obj.properties, config: _config });
        _template_body_html = _bodyhtml.replace(/{{/g, '<%').replace(/}}/g, '%>');
        _template_body_html = removeEmptyLinesRegex(_template_body_html);
    }
    $(".table").find("thead").html(_modelhtml);
    // console.log('向thead赋值',_modelhtml);
    //处理查询项 理论上至少有一个查询项？如果没有呢...
    if (obj.queryProperties) {
        _global_data_properties = obj.queryProperties;
        //载入query的值
        LoadQuery(obj.queryProperties);
        //校验格式等
        HandlerQueryItem(obj.queryProperties, pageData);
    } else {
        if (!pageData) {
            readPageData(1);
        } else {
            handlerDisplayDataToUi(pageData);
        }
    }
    //读取数据
}

/**
 * 
 * @param {*} arr 
 * @returns 
 */
function moveExtendBindToEndInPlace(arr) {

    if (!Array.isArray(arr) || arr.length === 0) {
        return arr;
    }

    // 先收集所有 extendBind 元素
    const extendBindItems = [];
    const otherItems = [];

    for (let i = 0; i < arr.length; i++) {
        if (arr[i] && arr[i].name === "extendBind") {
            extendBindItems.push(arr[i]);
        } else {
            otherItems.push(arr[i]);
        }
    }

    // 如果不存在 extendBind，直接返回
    if (extendBindItems.length === 0) {
        return arr;
    }

    // 清空原数组并重新填充
    arr.length = 0;
    arr.push(...otherItems, ...extendBindItems);

    return arr;
}


/**
 * 使用正则表达式剔除空行（性能更好）
 * @param {string} str - 输入字符串
 * @param {boolean} [trimLines=true] - 是否同时去除每行首尾空格
 * @returns {string} 处理后的字符串
 */
function removeEmptyLinesRegex(str, trimLines = true) {
    if (!str || typeof str !== 'string') {
        return str || '';
    }

    let result = str;

    if (trimLines) {
        // 先去除每行首尾空格，再去除空行
        result = result.replace(/^[ \t]+|[ \t]+$/gm, '');
    }

    // 去除空行（包括只有空格的行）
    return result.replace(/^\s*[\r\n]/gm, '');
}

/**
 * 兼容旧版本
 * @param {*} page 
 */
function _readpagedata(page) {
    readPageData(page);
}

/**
 * 处理类型的特性 比如按钮，批量操作等等
 * @param {*} o 
 */
async function handlerQueryClassAttribute(o) {
    if (o.attributes) {
        _global_class_attributes = o.attributes;
        var btns = new Array();
        for (var _attribute of o.attributes) {


            // o.attributes.forEach(_attribute => {

            switch (_attribute.name) {
                case "disable":
                    {
                        //是否禁用新增 编辑 删除功能 由于有默认配置问题，下面的要修改了
                        if (_attribute.args1 && (_attribute.args1 == "add" || _attribute.args1 == "true")) {
                            _config.disable_add = true;
                        }
                        if (_attribute.args1 == "false") {
                            _config.disable_add = false;
                        }
                        if (_config.disable_add) {
                            $(".btnadd").hide();
                        }
                        if (_attribute.args2 && (_attribute.args2 == "edit" || _attribute.args2 == "true")) {
                            _config.disable_edit = true;
                        }
                        if (_attribute.args2 == "false") {
                            _config.disable_edit = false;
                        }
                        if (_attribute.args3 && (_attribute.args3 == "del" || _attribute.args3 == "true")) {
                            _config.disable_del = true;
                        }
                        if (_attribute.args3 == "false") {
                            _config.disable_del = false;
                        }
                        if (_attribute.args4 && (_attribute.args4 == "detail" || _attribute.args4 == "true")) {
                            _config.enable_view = true;
                        }
                        if (_attribute.args4 == "false") {
                            _config.enable_view = false;
                        }
                    }
                    break;
                case "enable":
                    {
                        if (_attribute.args1) {
                            if (_attribute.args1 == "true") {
                                _config.disable_add = false;
                            } else {
                                _config.disable_add = true;
                            }
                        }
                        if (_config.disable_add) {
                            $(".btnadd").hide();
                        }
                        if (_attribute.args2) {
                            if (_attribute.args2 == "true") {
                                _config.disable_edit = false;
                            } else {
                                _config.disable_edit = true;
                            }
                        }
                        if (_attribute.args3) {
                            if (_attribute.args3 == "true") {
                                _config.disable_del = false;
                            } else {
                                _config.disable_del = true;
                            }
                        }
                        if (_attribute.args4) {
                            if (_attribute.args4 == "true") {
                                _config.enable_view = true;
                            } else {
                                _config.enable_view = false;
                            }
                        }
                    }
                    break;
                case "template":
                    {
                        //自定义页面表头和表格的模板 之前需要载入模板页面
                        if (_attribute.args1) {
                            _template_head_html = $(`#${_attribute.args1}`).html();
                        }
                        if (_attribute.args2) {
                            _template_body_html = $(`#${_attribute.args2}`).html();
                        }
                    }
                    break;
                case "batch":
                    {
                        if (_config.model == "view") {
                            //是否启用批量操作
                            _config.batch = true;
                            if (!_config.batchs) {
                                _config.batchs = new Array();
                            }
                            let btn = {
                                name: "",
                                func: _attribute.args2,
                                icon: "",
                                title: ""
                            };
                            if (_attribute.args1) {
                                btn.name = _attribute.args1;
                            }
                            if (_attribute.args3) {
                                btn.icon = _attribute.args3;
                            }
                            if (_attribute.args4) {
                                btn.title = _attribute.args4;
                            }
                            _config.batchs.push(btn);
                        }
                    }
                    break;
                case "button":
                    {
                        if (_attribute.args3 == 'add') {
                            if (_attribute.args1) {
                                $(".btnadd").val(_attribute.args1);
                            }
                            if (_attribute.args2) {
                                //这里得让他支持宏
                                //handlerBuildDynamicUrl
                                if (_attribute.args2.indexOf("{{") >= 0) {
                                    if (typeof handlerMicroString === "function") {
                                        _attribute.args2 = handlerMicroString(_attribute.args2);
                                    }
                                }
                                $(".btnadd").attr('onclick', _attribute.args2);
                            }
                            //如果是隐藏的则需要显示
                            _config.disable_add = false;
                            if (!$(".btnadd").is(":visible")) {
                                $(".btnadd").show();
                            }
                        } else {
                            btns.push(_attribute);
                        }
                    }
                    break;
                case "loadlib":
                    {
                        //载入资源
                        // loadLibFile(_attribute.args1, _attribute.args2);
                        await loadResource(_attribute.args1, _attribute.args2);
                    }
                    break;
                case "trclass":
                    {
                        if (_attribute.args1) {
                            _config.trclass = _attribute.args1;
                        }
                    }
                    break;
                case "charty":
                    {
                        let _info = {
                            type: 'value',
                            name: "数量",
                            position: 'left',
                            offset: 0,
                            show: true,
                            axisLine: {
                                show: true        // ← 关键：显示轴线
                            },
                            axisTick: {
                                show: true        // ← 显示刻度线
                            },
                            axisLabel: {
                                formatter: null,
                                show: true
                            }
                        };
                        if (_attribute.args1) {
                            _info.name = _attribute.args1;
                        }
                        if (_attribute.args2) {
                            _info.position = _attribute.args2;
                        }
                        if (_attribute.args3) {
                            let _off = Number(_attribute.args3);
                            if (!isNaN(_off)) {
                                _info.offset = _off;
                            }
                        }
                        if (_attribute.args4) {
                            _info.axisLabel.formatter = _attribute.args4
                        }
                        chart_data_yaxis.push(_info);
                    }
                    break;
                case "ui":
                    {
                        if (_attribute.args2) {
                            $(".show_type").show();
                        }
                        //防止query的配置被覆盖
                        if (!_config.ui_model) {
                            if (_attribute.args1 === 'chart') {
                                handlerChangeShowType(true);
                            } else {
                                handlerChangeShowType(true);
                            }
                        }
                        _config.ui_model = true;
                    }
                    break;
                case "config":
                    {
                        try {
                            var info = JSON.parse(_attribute.args1);
                            _config = Object.assign({}, info, _config);
                        } catch (err) {
                            console.error(err);
                        }
                    }
                    break;
            }
            // });
        }

        if (btns && btns.length > 0) {
            if (_config.model == "view") {
                var _ahtml = '<div class="buttons">';
                btns.forEach(_btn => {
                    _ahtml += `<input type="button" class="btn btnstyle" value="${_btn.args1}" onclick="${_btn.args2}" />`;
                    //这里要改版 有title 等
                });
                _ahtml += '</div>';
                $(".func_btn_area").html(_ahtml);
            }
        }
    }

    //引入table的列的拖拽排序的组件
    if (_config.drag_order_column) {
        await loadResource('./lib/ordercol/lib.js', '');
    }
    //多图是否自动引入 ./lib/img_drag.js呢？
    //同理打分的星星 是否也自动引入
}

/**
 * 从表格字段中找字段 返回true false
 * @param {*} properties 
 * @param {*} targetName 
 * @returns 
 */
function findExistsPropertyByName(properties, targetName) {
    //这里后续要做优化 做成map的形式，或者是用一些更高效的查找方式，现在先这样简单实现
    return properties.some(property => property.name === targetName);
}

/**
 * 查找某一个名称的字段信息
 * @param {*} properties 
 * @param {*} propertyName 
 * @returns 
 */
function findPropertyByName(properties, propertyName) {
    //这里后续要做优化 做成map的形式，或者是用一些更高效的查找方式，现在先这样简单实现
    for (const property of properties) {
        if (property.name == propertyName) {
            return property;
        }
    }
    return false;
}

/**
 * 从表格信息中找找看看是否有主键列
 * @param {*} properties 
 * @returns 
 */
function findKeyAttributeProperty(properties) {
    //这里后续要做优化 做成map的形式，或者是用一些更高效的查找方式，现在先这样简单实现
    for (const property of properties) {
        if (property.attributes && property.attributes.some(attr => attr.name === "iskey")) {
            return property.name;
        }
    }
    return null;
}



/**
 * 处理列表中表格中的字段的属性信息
 * @param {*} properties 
 */
function handlerModelFieldcolumnProperty(properties) {
    if (properties) {
        properties.forEach(item => {
            // item.className=_classPath;
            item.edit = false;//是否是编辑模式
            item.valueType = "string";
            if (item.value != null) {
                item.valueType = typeof (item.value);
            }
            if (item.attributes) {
                item.attributes.forEach(_attribute => {
                    switch (_attribute.name) {
                        case "html":
                            {
                                if(item.dataType!="menu"){
                                    item.dataType = "html";
                                }
                                if (_attribute.args1) {
                                    item.html = _attribute.args1;
                                } else {
                                    item.html = item.value;
                                }
                            }
                            break;
                        case "menuhtml":
                            {
                                item.dataType = "menuhtml";
                                item.hidden = true;
                            }
                            break;
                        case "htmltemplate":
                            {
                                //后续这个删除，用上面的html替代
                                item.html = _attribute.args1;
                            }
                            break;
                        case "ifmenu":
                            {
                                if (_attribute.args1) {
                                    globalMenuCount++;
                                    item.ifmenu = {
                                        expression: _attribute.args1,
                                        value: _attribute.args2
                                    };
                                    item.dataType = "ifmenu";
                                    item.hidden = true;
                                    if (_attribute.args3 == 'box') {
                                        item.dataType = "ifmenubox";
                                        if (!_config.menubox) {
                                            _config.menubox = true;
                                        }
                                    }
                                }
                            }
                            break;
                        case "menu":
                            {
                                item.dataType = "menu";
                                item.hidden = true;
                                globalMenuCount++;
                                item.menu = {
                                    title: "",
                                };
                                if (_attribute.args1) {
                                    item.menu.name = _attribute.args1;
                                    if (item.menu.name.indexOf(" ") >= 0) {
                                        let strs = item.menu.name.split(" ");
                                        item.menu.name = strs[0].replace(/_*_/g, ' ');
                                        item.menu.title = strs[1].replace(/_*_/g, ' ');
                                    }
                                }
                                if (_attribute.args2) {
                                    item.menu.func = _attribute.args2;
                                }
                                if (_attribute.args3) {
                                    item.menu.icon = _attribute.args3;
                                }
                                if (_attribute.args4) {
                                    if (_attribute.args4 == 'box') {
                                        item.dataType = 'menubox';
                                        if (!_config.menubox) {
                                            _config.menubox = true;
                                        }
                                    }
                                }
                            }
                            break;
                        case "tap":
                            {
                                item.dataType = "tap";
                                item.hidden = true;
                                globalMenuCount++;
                                if (_attribute.args4) {
                                    if (_attribute.args4 == 'box') {
                                        item.dataType = 'tapbox';
                                        if (!_config.menubox) {
                                            _config.menubox = true;
                                        }
                                    }
                                }
                            }
                            break;
                        case "outerdisplay":
                            {
                                item.name = _attribute.args2;
                            }
                            break;
                        case "display":
                            {
                                if (_attribute.args1) {
                                    item.name = `${item.name}?.${_attribute.args1}`;
                                    item.display = item.name;
                                } else {
                                    item.name = `${item.name}?.name`;
                                }
                            }
                            break;
                        case "outer":
                            {
                                if (item.value != null) {
                                    item.dataFrom = typeof (item.value);
                                    item.valType = item.dataFrom;
                                } else {
                                    item.valType = "string";
                                }
                                if (!item.display) {
                                    item.display = item.name;
                                }
                                item.dataType = "outer";
                                if (_attribute.args2) {
                                    var toField = "name";
                                    if (_attribute.args4) {
                                        toField = _attribute.args4;
                                    }
                                    item.display = `${_attribute.args2}?.${toField}`;
                                }
                                //这个规则有点问题啊，outer的时候，当前显示显示字段，扩展信息不显示。。。 改成扩展显示，当前看看是否显示，感觉更合适
                                // if (_attribute.args2) {
                                //     if (_attribute.args4) {
                                //         item.name = `${_attribute.args2}?.${_attribute.args4}`;
                                //     } else {
                                //         item.name = `${_attribute.args2}?.name`;
                                //     }
                                //     var _property = funcFindListPropertry(_attribute.args2);
                                //     if (_property) {
                                //         _property.hidden = true;
                                //     }
                                // }
                                //扩展字段，标记display来显示到UI
                            }
                            break;
                        case "select":
                            {
                                try {
                                    item.dataType = 'select';
                                    item.selects = JSON.parse(_attribute.args1);
                                    var _map = new Map();
                                    item.selects.forEach(ii => {
                                        _map.set(ii.value, ii.name);
                                    });
                                    gloablSelectMap.set(item.name, _map);
                                    gloablSelectVals.set(item.name, item.selects);
                                } catch (err) {
                                    console.log(item.name, "属性 select 的值 args1 配置错误", err);
                                }
                            }
                            break;
                        case "lselect":
                            {
                                try {
                                    item.dataType = 'lselect';
                                    item.selects = JSON.parse(_attribute.args1);
                                    item.selects_max = "lsingle";
                                    if (_attribute.args4) {
                                        // console.log('lselect args4', _attribute.args4);
                                        if (_attribute.args4 != "1") {
                                            item.selects_max = "lbatch";
                                        }
                                    }
                                    var _map = new Map();
                                    item.selects.forEach(ii => {
                                        _map.set(ii.value, ii.name);
                                    });
                                    gloablSelectMap.set(item.name, _map);
                                    gloablSelectVals.set(item.name, item.selects);
                                } catch (err) {
                                    console.log(item.name, "属性 select 的值 args1 配置错误", err);
                                }
                            }
                            break;
                        case "hidden":
                            {
                                if (_attribute.args1) {
                                    if (_config.model != _attribute.args1) {
                                        item.hidden = true;
                                    }
                                } else {
                                    item.hidden = true;
                                }
                            }
                            break;
                        case "empty":
                            {
                                item.hidden = true;
                            }
                            break;
                        case "class":
                            {
                                item.class = _attribute.args1;
                            }
                            break;
                        case "switch": {

                            item.switch_func = "undefined";
                            item.switch_confirm = false;
                            if (_attribute.args1) {
                                if (_config.model == _attribute.args1) {
                                    item.dataType = _attribute.name;
                                }
                            } else {
                                item.dataType = _attribute.name;
                            }
                            if (_attribute.args2) {
                                item.switch_confirm = true;
                            }
                            if (_attribute.args3) {
                                item.switch_func = _attribute.args3;
                            }
                            if (_attribute.args4) {
                                if (_attribute.args4 == "local") {
                                    item.switch_local = true;
                                }
                            }
                        }
                            break;
                        case "mark":
                            {
                                var _mark = {
                                    model: _classPath,
                                    value: item.name
                                };
                                if (_attribute.args1) {
                                    _mark.model = _attribute.args1;
                                }
                                if (_attribute.args2) {
                                    _mark.value = _attribute.args2;
                                }
                                if (_attribute.args3) {
                                    _mark.style = _attribute.args3;
                                }
                                item.mark = _mark;
                            }
                            break;
                        case "edit":
                            {
                                if (_attribute.args1) {
                                    if (_attribute.args1 == _config.model) {
                                        item.edit = true;
                                    }
                                }
                                if (_attribute.args2) {
                                    if (_attribute.args2 == _config.model) {
                                        item.edit = true;
                                    }
                                }
                                if (_attribute.args3) {
                                    if (_attribute.args3 == _config.model) {
                                        item.edit = true;
                                    }
                                }
                                if (_attribute.args4) {
                                    if (_attribute.args4 == _config.model) {
                                        item.edit = true;
                                    }
                                }
                            }
                            break;
                        case "prefix":
                            {
                                // item[_key] = `${_attribute.args1}${_val}`;
                                item.prefix = _attribute.args1;
                            }
                            break;
                        case "orderby":
                            {
                                if (_attribute.args1 && _attribute.args2) {
                                    item.orderby = {
                                        asc: _attribute.args1,
                                        desc: _attribute.args2
                                    };
                                }
                            }
                            break;
                        case "unit":
                            {
                                // item[_key] = `${_val}${_attribute.args1}`;
                                item.unit = _attribute.args1;
                            }
                            break;
                        case "fentoyuan":
                            {
                                item.dataType = "fentoyuan";
                            }
                            break;
                        case "image":
                            {
                                item.dataType = _attribute.name;
                            }
                            break;
                        case "minbtn":
                            {
                                item.minbtn = {
                                    name: _attribute.args1,
                                    func: "func_min_btn_click(this,'{{:=item.id}}','" + item.name + "')",
                                    class: ""
                                };
                                if (_attribute.args2) {
                                    item.minbtn.func = _attribute.args2;
                                }
                                if (_attribute.args3) {
                                    item.minbtn.class = _attribute.args3;
                                }
                            }
                            break;
                        case "sum":
                            {
                                //合计数据列
                                globalSumMap.set(item.name, 0);
                            }
                            break;
                        default:
                            {
                                // item.dataType = _attribute.name;
                            }
                            break;
                    }
                });
            }
        });
    }
}

/**
 * 合计列 用于叠加对应列的数据，最后显示在下发行中
 */
var globalSumMap = new Map();
/**
 * 关于Select Lselect的信息对，用于UI显示友好名称使用
 */
var gloablSelectMap = new Map();

/**
 * 用于承载selct lselct得selects，主要用于edit模式
 */
var gloablSelectVals = new Map();

/**
 * 按钮总数，用于右侧操作区域的按钮总数
 */
var globalMenuCount = 0;

/**
 * 从服务端获取查询项，检查格式，并把查询项写入到searcharea对应的UI中
 * 里面可能会对页面进行数据请求，比如获取第一页数据
 * @param {*} listproperties 
 * @param {*} pageData request=one模式下的页面数据
 */
function HandlerQueryItem(listproperties, pageData) {
    var _temp = new Array();
    listproperties.forEach(item => {
        item.dataFrom = item.dataType;

        if (item.name == "show_type") {
            console.log('注意 show_type是表格或者图标显示的参数，当前查询会受干扰');
        }
        if (item.name == "path") {
            console.log('注意 path 是表格数据源标识，当前查询会受干扰');
        }
        if (item.name == "model") {
            console.log('注意 model是当前页面的显示类型，当前查询会受干扰');
        }
        // if (item.name == "request") {
        //     console.log('注意 request是当前页面的请求类型，当前查询会受干扰');
        // }

        if (item.attributes) {
            item.attributes.forEach(_attribute => {
                switch (_attribute.name) {
                    case "select":
                        {
                            try {
                                item.dataType = 'select';
                                item.selects = JSON.parse(_attribute.args1);
                                for (let sitem of item.selects) {
                                    sitem.selected = false;//改版成值来命中
                                }
                                var _map = new Map();
                                item.selects.forEach(ii => {
                                    _map.set(ii.value, ii.name);
                                });
                                // gloablSelectMap.set(item.name, _map);    //搜索项不需要这个，不然会导致联动，也就是2变得名称不能一致
                            } catch (err) {
                                console.log(item.name, "属性 select 的值 args1 配置错误", err);
                            }
                        }
                        break;
                    case "datalist":
                        {
                            item.dataType = 'datalist';
                            item.datalistid = item.name;
                            if (_attribute.args1) {
                                try {
                                    item.selects = JSON.parse(_attribute.args1);
                                    for (let sitem of item.selects) {
                                        sitem.selected = false;//改版成值来命中
                                    }
                                } catch (err) {
                                    console.log(item.name, "属性 datalist 的值 args1 配置错误", err);
                                }
                            } else {
                                item.datalistid = _attribute.args2;
                                if (_attribute.args3) {
                                    try {
                                        eval(_attribute.args3);
                                    } catch (err) {
                                    }
                                }
                            }
                        }
                        break;
                    case "hidden":
                        {
                            item.hidden = true;
                            if (_attribute.args1) {
                                if (_config.model == _attribute.args1) {
                                    item.hidden = false;
                                }
                            }
                        }
                        break;
                    case "mark":
                        {
                            var _mark = {
                                model: _classPath,
                                value: item.name
                            };
                            if (_attribute.args1) {
                                _mark.model = _attribute.args1;
                            }
                            if (_attribute.args2) {
                                _mark.value = _attribute.args2;
                            }
                            item.mark = _mark;
                        }
                        break;
                    case "step":
                        {
                            if (_attribute.args1) {
                                item.step = _attribute.args1;
                            }
                        }
                        break;
                    case "daterange":
                        {
                            item.dataType = "daterange";
                            if (_attribute.args2) {
                                var _target = funcFindExpendPropertry(_attribute.args2, item.name);
                                if (_target) {
                                    _target.hidden = true;
                                }
                            }
                            //2个时间合并到一个去 注意要和js的保持一致 比如yyyy-MM-dd  至  yyyy-MM-dd
                            var _start = "";
                            var _end = "";
                            if (_attribute.args1 == item.name) {
                                _start = item.value;
                            } else {
                                if (_attribute.args1) {
                                    var _arg1_obj = funcFindExpendPropertry(_attribute.args2, item.name);
                                    if (_arg1_obj) {
                                        if (_arg1_obj.value) {
                                            _start = _arg1_obj.value;//默认为yyyy-MM-dd HH:mm:ss格式
                                            //时间可能是时间戳格式
                                            if (_attribute.args4) {
                                                switch (_attribute.args4) {
                                                    case "times":
                                                        {
                                                            _start = new Date(_start * 1000).Format('yyyy-MM-dd HH:mm:ss');
                                                        }
                                                        break;
                                                    case "timems":
                                                        {
                                                            _start = new Date(_start * 1000).Format('yyyy-MM-dd HH:mm:ss');
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (_start && _attribute.args2) {
                                var _arg2_obj = funcFindExpendPropertry(_attribute.args2, item.name);
                                if (_arg2_obj) {
                                    if (_arg2_obj.value) {
                                        _end = _arg2_obj.value;
                                        if (_attribute.args4) {
                                            switch (_attribute.args4) {
                                                case "times":
                                                    {
                                                        _end = new Date(_end * 1000).Format('yyyy-MM-dd HH:mm:ss');
                                                    }
                                                    break;
                                                case "timems":
                                                    {
                                                        _end = new Date(_end * 1000).Format('yyyy-MM-dd HH:mm:ss');
                                                    }
                                                    break;
                                            }
                                        }
                                        var _fulldate = `${new Date(_start).Format('yyyy-MM-dd')}  至  ${new Date(_end).Format('yyyy-MM-dd')}`;
                                        item.value = _fulldate;
                                    }
                                }
                            }
                        }
                        break;
                    case "searchrange":
                        {
                            var step = "1";
                            if (_attribute.args3) {
                                step = _attribute.args3;
                            }
                            item.dataType = "searchrange";
                            if (_attribute.args2) {
                                var _target = funcFindExpendPropertry(_attribute.args2, item.name);
                                if (_target) {
                                    _target.hidden = true;
                                }
                            }
                            var range = {
                                low_name: _attribute.args1,
                                step: step,
                                placeholder: item.placeholder,
                                high_name: _attribute.args2
                            };
                            item.range = range;
                            //感觉这个要做好，还缺好多，比如输入值区间等
                        }
                        break;
                    case "outer":
                        {
                            item.dataType = 'outer';
                        }
                        break;
                    case "selects":
                    case "lselect":
                        {
                            try {
                                item.dataType = _attribute.name;
                                item.selects = JSON.parse(_attribute.args1);
                                item.selects_max = "lsingle";
                                if (_attribute.args4) {
                                    if (_attribute.args4 != "1") {
                                        item.selects_max = "lbatch";
                                    }
                                }
                                for (let sitem of item.selects) {
                                    sitem.selected = false;//改版成值来命中
                                }
                                if (item.value || item.value == 0) {

                                    var _typeof = typeof (item.value);
                                    switch (_typeof) {
                                        case "string":
                                            {
                                                var _splitchar = ',';
                                                if (_attribute.args2) {
                                                    _splitchar = _attribute.args2;
                                                }
                                                var strs = item.value.split(_splitchar);
                                                item.selects.forEach(ii => {
                                                    if (strs.indexOf(ii.value) >= 0) {
                                                        ii.selected = true;
                                                    }
                                                });
                                            }
                                            break;
                                        case "number":
                                            {
                                                var _vals = item.value;
                                                item.selects.forEach(ii => {
                                                    if (_vals == (ii.value)) {
                                                        ii.selected = true;
                                                    }
                                                });
                                            }
                                            break;
                                        case "object":
                                            {
                                                if (Array.isArray(item.value)) {
                                                    item.selects.forEach(ii => {
                                                        if (item.value.indexOf(ii.value) >= 0) {
                                                            ii.selected = true;
                                                        }
                                                    });
                                                } else {
                                                    var _vals = item.value.toString();
                                                    item.selects.forEach(ii => {
                                                        if (_vals.indexOf(ii.value.toString()) >= 0) {
                                                            ii.selected = true;
                                                        }
                                                    });
                                                }
                                            }
                                            break;
                                    }

                                }
                            } catch (err) {
                                console.log(item.name, "属性 select 的值 args1 配置错误", err);
                            }
                        }
                        break;
                    case "next_key":
                        {
                            _config.next.key = item.name;
                            if (item.dataType == "Int32" || item.dataType == "Int64" || item.dataType == "UInt32" || item.dataType == "UInt64") {
                                _config.next.type = "number";
                            } else {
                                _config.next.type = "string";
                            }
                            if (_attribute.args1) {
                                _config.next.key = _attribute.args1;
                            }
                            item.hidden = true;
                        }
                        break;
                    case "splitarray":
                        {
                            // item.dataType="String";
                            item.dataType = "splitarray";
                            if (item.value) {
                                if (_attribute.args1) {
                                    item.value = item.value.join(_attribute.args1);//用其他字符分割
                                } else {
                                    item.value = item.value.toString();//使用,分割
                                }
                            } else {
                                item.value = "";
                            }

                        }
                        break;
                    case "class":
                        {
                            if (_attribute.args1) {
                                item.class = _attribute.args1;
                            }
                        }
                        break;
                    default:
                        {
                            // console.log('属性', _attribute.name, '未处理');
                        }
                        break;
                }
            });
        }

        if (item.name == 'orderby') {
            if (item.value) {
                _orderby = item.value;
            }
            item.hidden = true;
        }

        if (item.dataFrom == "Guid") {
            if (item.value == "00000000-0000-0000-0000-000000000000") {
                item.value = "";
                if (item.display == "00000000-0000-0000-0000-000000000000") {
                    item.display = "";
                }
            }
        }

        if (item.name == "next_key") {
            _config.next.key = item.name;
            if (item.dataType == "Int32" || item.dataType == "Int64" || item.dataType == "UInt32" || item.dataType == "UInt64") {
                _config.next.type = "number";
            } else {
                _config.next.type = "string";
            }
            item.hidden = true;
        }

        //过滤掉默认存在的 如果后端要修改size？
        if (item.name != "page" && item.name != "size") {
            if (item.dataType == "DateTime") {
                if (!item.format) {
                    item.format = "yyyy-MM-dd HH:mm:ss";
                }
                item.required = true;
            }
            if (item.dataType == "String") {
                if (!item.maxlength) {
                    item.maxlength = 64;
                }
            }
            //外表选择?
            _temp.push(item);//hidden的也需要显示在UI，所以后端需要给默认值
        } else {
            if (item.name == "size") {
                if (item.value) {
                    var tagNum = parseInt(item.value);
                    if (tagNum != _pageSize) {
                        _pageSize = tagNum;
                        localStorage.setItem(`pagesize:${_classPath}`, tagNum);
                    }
                }
            }
            item.hidden = true;
        }
        //初始化daterange
    });
    if (_temp.length > 0) {

        var buildQueryForm = false;
        //防止重复渲染搜索项
        if ($(".searchbox").text().length == 0) {
            var _template_query = $("#template_search").html();
            var _ahtml = template(_template_query, { list: _temp, config: _config });
            $(".searchbox").append(_ahtml);
            buildQueryForm = true;
        }

        // setTimeout(function () {
        if (chart_table) {
            chart_table.resize();
        }
        if (buildQueryForm) {
            InitUiData();
        }
        if ($(".searchbox").height() > 60) {
            $(".tap_more").fadeIn();
        }

        // if (!pageData) {
        //     readPageData(1);
        // } else {
        //     handlerDisplayDataToUi(pageData);
        // }
        // }, 10);
    }
    // console.log(listproperties);
    //主要为了上方的UI部分加载完成
    setTimeout(() => {
        if (!pageData) {
            readPageData(1);
        } else {
            handlerDisplayDataToUi(pageData);
        }
    }, 10);
}

/**
 * 数据载入后，初始化一些绑定等操作
 */
function InitUiData() {
    if (_global_data_properties) {
        _global_data_properties.forEach(item => {
            if (item.dataType == 'daterange') {
                HandlerInitDateRange(item.name, item.value);
            }
        });

        if ($(".ulitems").length > 0) {
            /**
     * selects lselect 下面逻辑有问题！
     */
            $(".ulitems").on('click', 'li', function () {
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                } else {
                    $(this).addClass('selected');
                }
                var _name = $(this).parents(".ulitems").attr('name');
                if (_name) {
                    var _pro = funcFindPropertry(_name);
                    if (_pro) {
                        var _maxnum = 0;
                        var _attribute = funcFindAttribute(_pro.attributes, "selects");
                        if (_attribute) {
                            if (_attribute.args4) {
                                _maxnum = Number(_attribute.args4);
                            }
                        } else {
                            _attribute = funcFindAttribute(_pro.attributes, "lselect");
                            if (_attribute) {
                                if (_attribute.args4) {
                                    _maxnum = Number(_attribute.args4);
                                }
                            }
                        }
                        if (_maxnum > 0) {
                            if (_maxnum == 1) {
                                $(this).siblings().removeClass('selected');
                            }
                            var _selnum = $(this).parents(".ulitems").find(".selected").length;
                            if (_selnum > _maxnum) {
                                // $(this).siblings().removeClass('selected');
                                $(this).removeClass('selected');
                                if (_maxnum > 1) {
                                    layer.msg(`限定最多选择${_maxnum}项`);
                                }
                            }
                            if (_maxnum == 1 && _selnum > 0) {
                                readPageData(1);
                            }
                        }
                    }
                }
            });
        }
    }
}





/**
 * 提交搜索前数据校验
 * @param {*} _info 
 */
function handlerInputValue(_info) {
    if (_global_data_properties) {
        _global_data_properties.forEach(item => {
            switch (item.dataType) {
                case "daterange":
                    {
                        if (item.attributes) {
                            for (let _attribute of item.attributes) {
                                if (_attribute.name == 'daterange') {
                                    var _value = _info[item.name];//获取当前选中的值
                                    if (_value) {
                                        var splits = _value.split('  ');
                                        if (splits.length == 3) {
                                            let _start = `${splits[0]} 00:00:00`;
                                            let _end = `${splits[2]} 00:00:00`;
                                            if (_attribute.args1) {
                                                // _info[_attribute.args1] = _start;
                                                setValueToSameLevel(item.name, _info, _attribute.args1, _start);
                                            }
                                            if (_attribute.args2) {
                                                // _info[_attribute.args2] = _end;
                                                setValueToSameLevel(item.name, _info, _attribute.args2, _end);
                                            }

                                            if (_attribute.args4) {
                                                switch (_attribute.args4) {
                                                    case "times":
                                                        {
                                                            let _times = new Date(_start).getTime() / 1000;
                                                            let _timee = new Date(_end).getTime() / 1000;
                                                            // _info[_attribute.args1] = _times;
                                                            // _info[_attribute.args2] = _timee;
                                                            setValueToSameLevel(item.name, _info, _attribute.args1, _times);
                                                            setValueToSameLevel(item.name, _info, _attribute.args2, _timee);
                                                        }
                                                        break;
                                                    case "timems":
                                                        {
                                                            let _times = new Date(_start).getTime();
                                                            let _timee = new Date(_end).getTime();
                                                            // _info[_attribute.args1] = _times;
                                                            // _info[_attribute.args2] = _timee;
                                                            setValueToSameLevel(item.name, _info, _attribute.args1, _times);
                                                            setValueToSameLevel(item.name, _info, _attribute.args2, _timee);
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case "Guid":
                case "DateTime":
                    {
                        if (!item.required) {
                            if (_info[item.name] == "null" || _info[item.name] == '') {
                                delete _info[item.name];
                            }
                        }
                    }
                    break;
                case "selects":
                case "lselect":
                    {
                        //基于结果进行值拼接！
                        var _elcs = $(`[name=${item.name}]`).find('.selected');
                        if (_elcs && _elcs.length > 0) {
                            var _array = new Array();
                            for (var k = 0; k < _elcs.length; k++) {
                                _array.push($(_elcs[k]).attr('value'));
                            }
                            if (item.dataFrom.indexOf("[]") >= 0) {
                                if (item.dataFrom == "Int32[]" || item.dataFrom == "Int64[]" || item.dataFrom == "UInt32[]" || item.dataFrom == "UInt64[]") {
                                    var _newarray = new Array();
                                    _array.forEach(_ary => {
                                        _newarray.push(Number(_ary));
                                    });
                                    _info[item.name] = _newarray;
                                } else {
                                    _info[item.name] = _array;
                                }
                            } else {
                                //string

                                if (item.dataFrom == 'Int32' || item.dataFrom == 'Int64' || item.dataFrom == 'UInt32' || item.dataFrom == 'UInt64') {
                                    _info[item.name] = Number(_array[0]);
                                } else {
                                    var _splitchar = ',';
                                    if (item.attributes) {
                                        for (var _attribute of item.attributes) {
                                            if (_attribute.name == item.dataType) {
                                                if (_attribute.args2) {
                                                    _splitchar = _attribute.args2;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    _info[item.name] = _array.join(_splitchar);
                                }
                            }
                        }
                    }
                    break;
                case "splitarray":
                    {
                        var _split_char = ',';//默认值使用逗号隔开
                        var _split = funcFindAttribute(item.attributes, "splitarray");
                        if (_split) {
                            if (_split.args1) {
                                _split_char = _split.args1;
                            }
                        }
                        if (_info.hasOwnProperty(item.name)) {
                            var _val = _info[item.name];
                            if (_val) {
                                _val = _val.replace(/，/g, _split_char);
                                if (item.dataFrom == "Int32[]" || item.dataFrom == "Int64[]" || item.dataFrom == "UInt32[]" || item.dataFrom == "UInt64[]") {
                                    var strs = _val.split(_split_char);
                                    var arr = new Array();
                                    for (var ia of strs) {
                                        arr.push(Number(ia));
                                    }
                                    _info[item.name] = arr;
                                } else {
                                    _info[item.name] = _val.split(_split_char);
                                }
                            } else {
                                delete _info[item.name];
                            }

                        }
                    }
                    break;
            }
            if (typeof (item.value) == 'number') {
                if (!_info.hasOwnProperty(item.name)) {
                    _info[item.name] = 0;
                }
            }

            if (item.nullable) {
                var _value = null;
                if (_info.hasOwnProperty(item.name)) {
                    _value = _info[item.name];
                }
                //null的问题应该是API处理
                if (!_value) {
                    delete _info[item.name];
                }
                switch (item.dataFrom) {
                    case "DateTime":
                    case "Guid":
                        {
                            if (_info[item.name] == "null" || _info[item.name] == '') {
                                delete _info[item.name];
                            }
                        }
                        break;
                }
            }
            //int类型的0可能被忽略，这样后台会命中比如默认值-1的情况

        });
    } else {
        //request=one模式下，可能会这样，则需要从query中拼接数据

    }
}


/**
 * 查找这个属性
 */
function funcFindListPropertry(_name) {
    if (_global_list_properties) {
        for (var one of _global_list_properties) {
            if (one.name == _name) {
                return one;
            }
        }
    }
    return null;
}



/**
 * 打开选择一个外表的值 这个有问题啊应该在form.js才对
 */
function funcLinkOuterOpenSelect(_title, _className, num, callback) {
    var _url = _className;
    if (_url.indexOf('/') == -1) {
        _url = `${handlerPasteFormPath()}index.html?model=select&path=${_className}`;
    }
    _url = handlerBuildDynamicUrl(_url);
    open_select(_title, _url + '&pf_num=' + num, "", "", callback);
}

/**
 * 
 * @param {*} elc 
 */
function open_menu_box(elc) {
    //隐藏其他
    $(".menubox").css('visibility', 'hidden');
    var _width = $(elc).parent().width();
    var _menubox = $(elc).parent().find(".menubox");
    if (_menubox) {
        $(_menubox).css('right', (_width + 20));
        //显示当前这个
        $(_menubox).css('visibility', 'visible');
    }
}

/**
 * 删除输入框的值
 * @param {*} elc 
 */
function handlerSearchClean(elc) {
    var _parent = $(elc).parents('.sitem');
    if (_parent) {
        _parent.find('input,textarea').val('');
    }
}

/**
 * 
 * @param {*} elc 
 */
function handlerSearchCleanOuterInput(elc) {
    var _parent = $(elc).parents('.outer');
    if (_parent) {
        _parent.find('input,textarea').val('');
    }
}

/**
 * 删除输入框的值
 * @param {*} elc 
 */
function handlerCleanOuterInput(elc) {
    var _parent = $(elc).parents('.outer');
    if (_parent) {
        _parent.find('input,textarea').val('');
    }
}



/**
 * 输入框回车发送
 * @param {*} event 
 */
function handler_keyup(event) {
    if (event.key == 'Enter') {
        readPageData(1);
    }
}

/**
 * 下拉后执行读取数据
 * @param {*} e 
 */
function handler_select_change(e) {
    readPageData(1);
}

/**
 * 表格中的switch的开关触发
 * @param {*} elc 
 */
function handlerSwitchChange(elc, confirm, func_name) {
    // dataid dataname
    if (func_name && func_name != "undefined") {
        const func_direct = window[func_name];
        if (typeof func_direct == 'function') {
            func_direct(elc);
        }
    } else {
        var _value = $(elc).prop('checked');//true/false
        if (confirm == 'true') {
            layer.confirm('当前操作会修改数据，是否继续？', { btn: ["确定", "取消"] }, function (index) {
                layer.close(index);
                handlerSwitchUpload(elc);
            }, function () {
                $(elc).prop('checked', !_value);
                return;
            });
        } else {
            handlerSwitchUpload(elc);
        }
    }
}

/**
 * 处理点击switch的提交
 * @param {*} elc 
 */
function handlerSwitchUpload(elc) {
    var _dataid = $(elc).attr('dataid');// 1或者guid等主键
    var _dataname = $(elc).attr('dataname');//isEnable
    var _value = $(elc).prop('checked');//true/false
    var _info = {
        id: _dataid,
        name: _dataname,
        state: _value,
        search: location.search
    };
    //由后端基于这个参数自己决定如何处理
    _apipost(`${auto_path()}/updateState`, true, JSON.stringify(_info), (c, o) => {
        if (c != 200) {
            $(elc).prop('checked', !_value);
        }
    });
}

/**
 * location.search转化成一个object
 * @param {*} queryString 
 * @returns 
 */
function parseQueryString(queryString) {
    // 去掉开头的 '?'
    if (queryString.startsWith('?')) {
        queryString = queryString.slice(1);
    }

    // 分割成键值对
    const pairs = queryString.split('&');

    // 创建一个空对象来存储结果
    const result = {};

    // 遍历每个键值对
    pairs.forEach(pair => {
        const [key, value] = pair.split('=');
        // 解码键和值
        const decodedKey = decodeURIComponent(key);
        const decodedValue = decodeURIComponent(value || ''); // 处理没有值的情况
        // 如果键已经存在，转换为数组
        if (result.hasOwnProperty(decodedKey)) {
            if (Array.isArray(result[decodedKey])) {
                result[decodedKey].push(decodedValue);
            } else {
                result[decodedKey] = [result[decodedKey], decodedValue];
            }
        } else {
            result[decodedKey] = decodedValue;
        }
    });

    return result;
}

/**
 * 绑定得时候 点击上方执行批量操作，注意延迟
 * @param {*} elc 
 */
/**
 * 批量操作开关处理函数
 * @param {*} elc 触发批量操作的checkbox元素
 */
function handler_batch_switch(elc) {
    var batchVal = $(elc).prop('checked'); // true/false
    var elements = $('[dataname="extendBind"]');

    if (elements.length === 0) {
        console.log('未找到权限元素');
        return;
    }

    var loadid = layer.load();
    // 将jQuery对象转换为数组以便使用shift()
    var elementsArray = elements.toArray();
    var processedCount = 0;

    var batchHandler = function () {
        if (elementsArray.length === 0) {
            // 所有元素处理完毕
            layer.close(loadid);
            console.log(`批量操作完成，共处理 ${processedCount} 个`);
            return;
        }

        // 取出第一个元素（并从数组中移除）
        var currentElement = elementsArray.shift();
        var $currentElement = $(currentElement);

        // 只有状态不同时才触发点击
        if ($currentElement.prop('checked') !== batchVal) {
            $currentElement.click();
            processedCount++;
        } else {

        }
        // 继续处理下一个，延迟300ms
        setTimeout(batchHandler, 10);
    };

    // 开始批量处理
    batchHandler();
}

// // 使用示例
// const queryObject = parseQueryString(location.search);
// console.log(queryObject);

/**
 * 处理获取到的数据
 */
async function readPageData(page) {
    if (page == 1) {
        $("#pagenumarea").empty();
    }

    //请求页码的时候重置一些东西
    if (_config.model == "bind") {
        $(".batch-bind-switch").prop("checked", false);
    }
    if (_config.model == "view") {
        $(".batch-select-switch").prop("checked", false);
    }

    var infoSearch = {};
    //载入当前页面的queryString内容 这里有问题 应该是要先获取search的然后获取表单的
    if (location.search) {
        var _querys = parseQueryString(location.search);
        for (var _key in _querys) {
            infoSearch[_key] = _querys[_key];
        }
    }

    var _info = $("#formsearch").parseForm();
    //这里要做输入结果的校验，转化等
    handlerInputValue(_info);

    //这里做合并 form表单的优先级更高，覆盖掉queryString的参数，queryString主要是为了携带一些固定参数或者特殊参数用的，form表单才是用户每次查询时输入的参数
    _info = Object.assign({}, infoSearch, _info);

    if (_config.next != null) {
        if (_config.next.key) {
            if (_config.next.map) {
                var _read = _config.next.map.get(page);
                if (_read) {
                    _info[_config.next.key] = _read;
                }
            }
        }
    }

    //这里错误了，url会启动一个默认值
    // if (!_info.hasOwnProperty('show_type')) {
    //     _info.show_type = _config.show_type;
    // }


    //标识当前的最新选择，而不是上面的固定用来源
    _info.show_type = _config.show_type;//强制传递显示类型，防止后端不传导致的显示问题
    _info.page = page;
    _info.size = _pageSize;
    _info.orderby = _orderby;

    if (typeof apigetPromise == "function") {
        var read = await apigetPromise(`${auto_path()}/page`, _info);

        afterReadPageData(read.code, read.info, page);

    } else {
        _apiquery(`${auto_path()}/page`, true, _info, function (code, obj) {

            afterReadPageData(code, obj, page);

        });
    }

}

/**
 * 处理页面获取数据后
 * @param {*} code 
 * @param {*} obj 
 * @param {*} page 
 */

function afterReadPageData(code, obj, page) {
    var _show_empty = false;
    if (code == 200) {

        if (obj.items) {
            if (obj.items.length == 0) {
                _show_empty = true;
            }
        }

        //后续要用，表示当前是第几页数据
        obj.page = page;

        //如果是有模型，且是第一页，这样可以防止重复渲染模型 如果是chart模式... ..  是否有渲染过模型???...
        if (obj.pasteModel && page == 1) {
            //有数据格式 ...
            if (_config.show_type == 'chart') {
                //先要进行数据格式整理
                handlerResetChartData();
            }
            //重置模板 可能当前接口返回的是多模板模式
            //变成每次都重置模板，后端如果不想每次都返回，可以在接口不变的情况下，增加一个参数来控制是否需要重置模板，这样可以兼容不同的需求
            _template_body_html = null;
            _template_head_html = null;
            //后面才是显示数据
            handlerModelFormatConfig(obj.pasteModel, obj);
        } else {
            handlerDisplayDataToUi(obj);
        }

        //渲染总计 pasteTotal [name value ] 都是字符串格式
        if (obj.pasteTotal) {

            if (obj.pasteTotal.length > 0) {
                //组合成信息到ui
                var ahtml = '综合：';
                obj.pasteTotal.forEach(va => {
                    ahtml += ` ${va.name}:${va.value}   `;
                });
                $(".totalsum").html(ahtml);
            }
        }

        //判断是不是游标获取模式，游标模式不走页码，只走游标和页大小
        if (_config.next != null) {
            if (_config.next.key) {
                if (obj.hasOwnProperty(_config.next.key)) {
                    if (!_config.next.map) {
                        var _map = new Map();
                        _map.set((page + 1), obj[_config.next.key]);
                        _config.next.map = _map;
                    } else {
                        var _map = _config.next.map;
                        _map.set((page + 1), obj[_config.next.key]);
                    }
                }
            }
        }

        if (page == 1) {
            if (typeof toolResizeTapMenuWidth != 'undefined') {
                toolResizeTapMenuWidth();
            }
        }

    } else {
        if (code == 204) {
            //没有查询到数据
            _show_empty = true;
            //涉及到按钮权限，不能直接弹出新增，可能没权限
            if (page == 1) {
                if (_config.model == 'select') {
                    if (_config.empty_create) {
                        //是否显示了 未完待续
                        if ($(".btnadd").is(":visible")) {
                            tap_add_form($(".btnadd"));
                        }
                    }
                }
            }
        }

    }
    if (_show_empty) {
        // global_page_data = new Array(); //不需要直接重置 等待下一个，可能会把上一次选定给清空了
        $("#pagenumarea").empty();
        if (_config.model == "select") {
            //select模式下，如果已经有选择了的，则不进行清空
            // $('.table tbody tr').filter(function () {
            //     return $(this).find('td a.selected').length === 0;
            // }).remove();
            //未完待续 global_page_data也需要处理... .. . 按需保留和追加datas... .. .
            var lessArray = global_page_data.filter(x => x.__selected);
            global_page_data = lessArray;
            //这里有问题 下一页空 结果之前有选定... .. .
        } else {
            var _html = `<div class="nodata">
            <div class="nobody">
                <span>没有数据</span>
                <i class="Hui-iconfont Hui-iconfont-search"></i>
            </div>
        </div>`;
            $(".table").find("tbody").html(_html);
        }
    }
}

/**
 * 是否是页面打开后的 第一次页面展示
 */
var _first_page_data = true;
/**
 * 显示数据到UI
 * @param {*} obj 
 * @returns 
 */
function handlerDisplayDataToUi(obj) {

    var page = 1;
    if (obj.page) {
        page = obj.page;
    }

    if (obj.items.length > 0) {
        var datas = obj.items;
        global_page_data = datas;
        //补充主key的数据
        if (_config.exchange_id != 'id') {
            datas.forEach(element => {
                if (element.hasOwnProperty(_config.exchange_id)) {
                    element["id"] = element[_config.exchange_id];
                }
            });
        }

        privateHandlerPageItemData(datas);

        //判断类型
        if (_config.show_type == 'table') {
            _showpagedata(datas);
        } else {
            if (echarts_loaded) {
                showPageChart(datas);
            } else {
                loadEchartsLib().then(() => {
                    showPageChart(datas);
                }).catch(() => {
                    console.error('加载图表组件失败！');
                });
            }
        }
        if (pageobject == null) {
            pageobject = $('#pagenumarea').pagination({
                current: page,  // 默认为1
                pageSize: _pageSize,  // 默认为10
                sizeArray: _config.page_size,//可以切换每页多少
                total: obj.totalCount,  // 默认0
                callback: function (pageIndex, pagination, pageSize) {
                    if (_pageSize != pageSize) {
                        //保存到缓存中... .. .
                        localStorage.setItem(`pagesize:${_classPath}`, pageSize);
                    }
                    _pageSize = pageSize;
                    readPageData(pageIndex);
                }
            })
        } else {
            if (page == 1) {
                pageobject.setTotal(obj.totalCount);
            }
        }
        // _first_page_data = false;
        return false;
    } else {
        // _show_empty = true;
        return true;
    }
}

/**
 * 更新某一行的数据，一般用于比如你点击某一行编辑，弹出表单页面，修改完成后，调用这个函数，用于修改对应行的数据!
 * @param {*} obj 
 */
function handlerUpdateRow(input) {
    // let objs = new Array();
    // if (Array.isArray(input)) {
    //     for (let oba of input) {
    //         if (_config.exchange_id != 'id') {
    //             if (oba.hasOwnProperty(_config.exchange_id)) {
    //                 oba["id"] = oba[_config.exchange_id];
    //             }
    //         }
    //         objs.push(oba);
    //     }
    // } else {
    //     if (_config.exchange_id != 'id') {
    //         if (input.hasOwnProperty(_config.exchange_id)) {
    //             input["id"] = input[_config.exchange_id];
    //         }
    //     }
    //     objs.push(input);
    // }
    // //更新列得对应显示等
    // privateHandlerPageItemData(objs);
    // //更新到绑定数据
    // handlerAutoUpdateGlobalPageData(objs);
    var objs = handlerInsertOrUpdateRowData(input);
    //更新到UI
    for (let obj of objs) {
        // var ahtml = template(_template_body_html, { item: obj, config: _config });
        const tempContainer = document.createElement('tbody');
        // tempContainer.innerHTML = ahtml;
        tempContainer.innerHTML = obj.__html;
        const newTr = tempContainer.querySelector('tr');
        if (obj.id) {
            const existingTr = document.querySelector(`tr[dataid="${obj.id}"]`);
            if (existingTr) {
                if (existingTr.classList.contains('hitline')) {
                    newTr.classList.add('hitline');
                }
                existingTr.parentNode.replaceChild(newTr, existingTr);
            } else {
                newTr.classList.add('hitline');//写入激活样式，这样用户可以发现新数据
                // console.log('没有基于dataid='+obj.id+'找到需要替换的行');
                const table = document.querySelector('table');
                if (table) {
                    const tbody = table.querySelector('tbody');
                    // 插入到 tbody 的最前面
                    if (tbody.firstChild) {
                        tbody.insertBefore(newTr, tbody.firstChild);
                    } else {
                        tbody.appendChild(newTr);
                    }
                    // console.log(`未找到 dataid="${obj.id}" 的 tr，已添加到表格开头`);
                } else {
                    console.error('未找到表格元素');
                }
            }
        }
    }
}

/**
 * 处理更新或者写入行数据，最终会把html写入到__html字段中
 * @param {*} input 
 * @returns 
 */
function handlerInsertOrUpdateRowData(input) {
    let objs = new Array();
    if (Array.isArray(input)) {
        for (let oba of input) {
            if (_config.exchange_id != 'id') {
                if (oba.hasOwnProperty(_config.exchange_id)) {
                    oba["id"] = oba[_config.exchange_id];
                }
            }
            objs.push(oba);
        }
    } else {
        if (_config.exchange_id != 'id') {
            if (input.hasOwnProperty(_config.exchange_id)) {
                input["id"] = input[_config.exchange_id];
            }
        }
        objs.push(input);
    }

    //更新列得对应显示等
    privateHandlerPageItemData(objs);
    //更新到绑定数据
    handlerAutoUpdateGlobalPageData(objs);
    //更新到UI
    for (let obj of objs) {
        var ahtml = template(_template_body_html, { item: obj, config: _config });
        obj.__html = ahtml; // 将生成的 HTML 存储在对象中，以便后续使用
    }
    return objs; // 返回更新后的对象数组
}


/**
 * 自动基于Id进行更新还是填充
 * 这里影响的是global_page_data 没有影响UI
 * @param {*} objs 
 * @returns 
 */
function handlerAutoUpdateGlobalPageData(objs) {
    if (global_page_data.length == 0) {
        global_page_data = objs;
        return;
    }
    var gloMap = new Map();
    for (var item of global_page_data) {
        gloMap.set(item.id, item);
    }
    for (let obj of objs) {
        gloMap.set(obj.id, obj);
    }
    var newArray = new Array();
    gloMap.forEach((data, key) => {
        newArray.push(data);
    });
    global_page_data = newArray;
}

let echarts_loaded = false;
let loadingPromise = null;  // 缓存正在加载的 Promise

async function loadEchartsLib() {
    if (echarts_loaded) {
        return Promise.resolve();  // 已加载完成，直接返回
    }

    if (loadingPromise) {
        return loadingPromise;  // 正在加载中，返回同一个 Promise
    }

    loadingPromise = new Promise((resolve, reject) => {
        var _script = document.createElement("script");
        _script.src = "../lib/echarts.min.js";
        _script.type = "text/javascript";
        _script.onload = function () {
            echarts_loaded = true;
            loadingPromise = null;  // 清空缓存
            resolve();
        };
        _script.onerror = function () {
            loadingPromise = null;  // 清空缓存
            reject(new Error("Failed to load echarts.min.js"));
        };
        document.head.appendChild(_script);
    });

    return loadingPromise;
}

// var echarts_loaded = false;
// /**
//  * 载入echarts组件 按需载入
//  */
// async function loadEchartsLib() {
//     if (!echarts_loaded) {
//         await new Promise((resolve, reject) => {
//             var _script = document.createElement("script");
//             _script.src = "../lib/echarts.min.js";
//             _script.type = "text/javascript";
//             _script.onload = function () {
//                 resolve();
//             }
//             _script.onerror = function () {
//                 reject();
//             }
//             document.head.appendChild(_script);
//         });
//         echarts_loaded = true;
//     } else {
//         return new Promise((resolve) => {
//             resolve();
//         });
//     }
// }

/**
 * 图表的数据轴信息 比如注册量 PV PU等标题
 */
var chart_data_heads = new Array();
/**
 * 数据Y轴 比如数量 金额 单量
 */
var chart_data_yaxis = new Array();
/**
 * 数据轴 比如成交量 退款量 成交金额 退款金额当数据
 */
var chart_data_series = new Array();
/**
 * 数据X轴是哪个 一般是时间
 */
var chart_data_xaxis = null;
/**
 * 图表对象
 */
var chart_table = null;

/**
 * 初始化echarts数据
 */
function handlerResetChartData() {
    /**
 * 图表的数据轴信息 比如注册量 PV PU等标题
 */
    chart_data_heads = new Array();
    /**
     * 数据Y轴 比如数量 金额 单量
     */
    chart_data_yaxis = new Array();
    /**
     * 数据轴 比如成交量 退款量 成交金额 退款金额当数据
     */
    chart_data_series = new Array();
    /**
     * 数据X轴是哪个 一般是时间
     */
    chart_data_xaxis = null;
    // /**
    //  * 图表对象
    //  */
    // chart_table = null;
    // $("#data_echart").empty();
    // console.log('初始化echarts');
    if (chart_table) {
        chart_table.clear();
    }
}

/**
 * 图表形式显示数据
 * @param {*} datas 
 */
function showPageChart(datas) {

    if (!chart_data_yaxis || chart_data_yaxis.length == 0) {
        //获得 单量 金额 等左右数据柱
        if (_global_class_attributes) {
            for (let _ymodel of _global_class_attributes) {
                if (_ymodel.name == 'charty') {
                    let _info = {
                        type: 'value',
                        name: "数量",
                        position: 'left',
                        offset: 0,
                        axisLabel: {
                            formatter: ''
                        }
                    };
                    if (_ymodel.args1) {
                        _info.name = _ymodel.args1;
                    }
                    if (_ymodel.args2) {
                        _info.position = _ymodel.args2;
                    }
                    if (_ymodel.args3) {
                        let _off = Number(_ymodel.args3);
                        if (!isNaN(_off)) {
                            _info.offset = _off;
                        }
                    }
                    if (_ymodel.args4) {
                        _info.axisLabel.formatter = _ymodel.args4
                    }
                    chart_data_yaxis.push(_info);
                }
            }
        } else {
            console.error('图表chart模式，需要在ListDto的Class中标注特性charty作为左右数据柱信息!');
        }
    }

    if (!chart_data_heads || chart_data_heads.length == 0) {
        chart_data_heads = new Array();
        for (let _property of _global_list_properties) {
            if (_property.attributes) {
                var _attribute = funcFindAttribute(_property.attributes, "chartseries");
                if (_attribute) {
                    chart_data_heads.push(_property.title);
                }
                if (chart_data_xaxis == null) {
                    var _attrcharty = funcFindAttribute(_property.attributes, "chartx");
                    if (_attrcharty) {
                        chart_data_xaxis = _property.name;
                    }
                }
            }
        }
    }

    chart_data_series = new Array();
    for (let _property of _global_list_properties) {
        if (_property.attributes) {
            var _attribute = funcFindAttribute(_property.attributes, "chartseries");
            if (_attribute) {
                // chart_data_heads.push(_property.title);
                let _info = {
                    extend_name: _property.name,
                    extend_type: 'line',
                    extend_index: 0,
                    extend_title: _property.title,
                    extend_datas: new Array()
                };
                if (_attribute.args1) {
                    _info.extend_type = _attribute.args1;
                }
                if (_attribute.args2) {
                    _info.extend_index = Number(_attribute.args2);
                    if (isNaN(_info.extend_index)) {
                        _info.extend_index = 0;
                    }
                }
                if (_attribute.args3) {
                    _info.extend_title = _attribute.args3;
                }
                chart_data_series.push(_info);
            }
        }
    }

    var option = {
        title: {
            text: ''
        },
        tooltip: {
            trigger: 'axis'
        },
        toolbox: {
            show: true
        },
        grid: {
            left: '10%',
            right: '10%',
            top: '10%',
            bottom: '10%',
            containLabel: true
        },
        legend: {
            data: chart_data_heads,
            top: '20'
        },
        xAxis: {
            data: []
        },
        yAxis: chart_data_yaxis,
        series: []
    };


    let xdatas = new Array();

    for (let item of datas) {
        //看看是否需要格式转化
        if (item.hasOwnProperty(chart_data_xaxis)) {
            xdatas.push(item[chart_data_xaxis]);
        }
        for (let dy of chart_data_series) {
            dy.extend_datas.push(item[dy.extend_name]);
        }
    }

    option.xAxis.data = xdatas;

    chart_data_series.forEach(yy => {
        let seri = {
            smooth: true,
            name: '',
            type: 'line',
            data: new Array(),
            yAxisIndex: 0,
            showSymbol: true
        };
        seri.data = yy.extend_datas;
        seri.name = yy.extend_title;
        seri.type = yy.extend_type;
        seri.yAxisIndex = yy.extend_index;
        option.series.push(seri);
    });
    if (!chart_table) {
        //初始化echarts实例
        chart_table = echarts.init(document.getElementById('data_echart'));
        //使用制定的配置项和数据显示图表
        chart_table.setOption(option);
        // 监听窗口大小变化事件
        window.addEventListener('resize', function () {
            chart_table.resize();//多次执行这里会不会重复?
        });
    } else {
        //使用制定的配置项和数据显示图表
        chart_table.setOption(option);
    }
}

