
"use strict";
/**
 * 
 */
var _classPath = "";

/**
 * 这个是否冲突了...
 */
var _id = "";

// /**
//  * 字段集合 迁移到form.js
//  */
// var _global_data_properties = null;//当前模型的属性列表

// /**
//  * 表的属性 迁移到form.js
//  */
// var _global_class_attributes = null;

/**
 * 是否需要载入richhtml
 */
var _global_need_loadrichhtml = false;
/**
 * 是否需要载入markdown
 */
var _global_need_loadmarkdown = false;

/**
 * 按需扩展，用于回传，比如extendUser
 */
//var _global_expend_object ={};

/**
 * 
 */
// const E = window.wangEditor;
var E = null;

/**
 * 
 */
var _config = {
    className: '',
    model: 'add',
    dynamic: true,
    numberTypes: ["Int32", "UInt32", "I32", "Int64", "UInt64", "Number"],
    show_tabs: false,//如果有分组，是否在最上方显示标签快选
    remove_empty: false,//字段空值的时候是否去除,比如{name:"",age:5}如果true,则返回{age:5}
    show_button: true,//是否显示提交按钮，默认显示
    single_line_length:128,//大于多少长度，是独占一行，否则是半行
    is_page_view:true,
    reload: true//执行完成后，是否重新加载父级页面 请使用okaction
};

var _map_values = new Map();

//图片集合 一般用于封面图的选择 主要是针对html rich
var _imageList = new Array();

/**
 * 从session中获取的对象信息 用于object的的时候
 */
var _readObject = null;

/**
 * 如果当前表单有outer则，标注最小高度应该大于600，以便承载index.html页面的选择
 */
var _has_outer = false;


/**
 * 
 */
$(window).ready(function () {

    //全局配置覆盖本地的
    if (typeof pasteform_custom_config != 'undefined') {
        // _config = pasteform_custom_config;
        _config = Object.assign({}, _config, pasteform_custom_config);
        // _config = PastedeepMerge(_config, pasteform_custom_config);
    }

    //参数配置覆盖本地的
    if (typeof parseSearch != 'undefined') {
        var searchObj = parseSearch(location.search);
        if (searchObj) {
            _config = Object.assign({}, _config, searchObj);
            // _config = PastedeepMerge(_config, searchObj);
            //PastedeepMerge
        }
    }

    var _path = _apigetquery("path");
    if (_path) {
        _classPath = _path;
        _config.className = _path;
    } else {
        console.error("参数错误，请携带参数path,注意大小写！");
    }

    //下方有歧义，主要是model是否也认localtion.search的qeury传递?
    var uidstr = _apigetquery("id");//基于主表，可能有不一样的类型 int long string guid 等
    if (uidstr != null) {
        _id = (uidstr);
        if (_id != '0') {
            _config.model = 'edit';
        } else {
            // _config.model = 'add';
            //默认值就是这个，不能二次覆盖了
        }
    }
    // else {
    // _config.model = 'add';
    // }
    // if (location.search.indexOf("dynamic=true") >= 0) {
    //     _config.dynamic = true;
    // }

    var _session = _apigetquery("session_key");
    if (_session) {
        var _read = sessionStorage.getItem(_session);
        if (_read) {
            try {
                _readObject = JSON.parse(_read);
                _global_expend_object = Object.assign({}, _global_expend_object, _readObject);
            } catch (err) {
                console.log(err);
            }
        }
    }

    // var _model = _apigetquery("model");
    var _model = _config.model;
    if (_model) {
        if (_model == 'select' || _model == 'object') {
            if (parent) {
                if (parent.FuncHiddenSureButtons) {
                    parent.FuncHiddenSureButtons();
                }
            }
        }
        if (_model == "sunobj") {
            $(".newform").css('padding', '10px 10px');
        }
    }

    //请使用okaction
    // var _unload = _apigetquery("unload");
    // if (_unload) {
    //     _config.reload = false;
    // }

    FuncFilexModel();

});

//配置字段信息，用于dynamic表单
var _setting_data_properties = null;

/**
 * 读取模型和默认值，值等
 */
function FuncFilexModel() {
    if (_id && _id != '0' && _id != 0) {
        _apiget(`${auto_path()}/${_id}/readUpdateModel${location.search}`, true, (c, o) => {
            if (c == 200) {
                loadHeader(o);
                _global_data_properties = new Array();
                //整合数据，整合字段信息用于显示UI
                if (o.data) {
                    if (o.properties) {
                        _setting_data_properties = o.properties;
                    } else {
                        _setting_data_properties = new Array();
                    }
                    dynamicBuildPropertyInfo(o.data, "", -1);
                } else {

                    if (o.properties) {
                        _global_data_properties = o.properties;
                        // LoadModelProperity(_global_data_properties);
                    }
                }
                if (o.title) {
                    document.title = "更新" + o.title;
                }
                LoadModelProperity(_global_data_properties);
            }
        });
    } else {
        _apiget(`${auto_path()}/readAddModel${location.search}`, true, (c, o) => {
            if (c == 200) {
                loadHeader(o);
                _global_data_properties = new Array();
                //整合数据，整合字段信息用于显示UI
                if (o.data) {
                    if (o.properties) {
                        _setting_data_properties = o.properties;
                    } else {
                        _setting_data_properties = new Array();
                    }
                    dynamicBuildPropertyInfo(o.data, "", -1);
                } else {

                    if (o.properties) {
                        //把存储值赋值给对象
                        if (_readObject) {
                            var _dynamicObject = flattenObject(_readObject);
                            o.properties.forEach(ii => {
                                if (!IsArrayProperty(ii.name)) {
                                    if (_readObject.hasOwnProperty(ii.name)) {
                                        ii.value = _readObject[ii.name];
                                    } else {
                                        if (_dynamicObject.hasOwnProperty(ii.name)) {
                                            ii.value = _dynamicObject[ii.name];
                                        }
                                    }
                                }
                            })
                        }
                        _global_data_properties = o.properties;
                        //基于从对象获取值显示到UI
                    }
                }
                if (o.title) {
                    document.title = "新增" + o.title;
                }
                LoadModelProperity(_global_data_properties);
            }
        });
    }
}
/**
 * 判断是否是数字的字段
 * @param {*} name 
 * @returns 
 */
function IsArrayProperty(name) {
    if (name) {
        if (name.indexOf("__") > 0) {
            var strs = name.split('__');
            for (const _str in strs) {
                if (isNaN(_str)) {
                    return true;
                }
            }
        }
    }
    return false;
}

/**
 * 把__分割的属性生成为对象
 * @param {*} flatObject 
 * @returns 
 */
function unflattenObject(flatObject) {
    const result = {};
    let isTopLevelArray = true; // 检查最外层是否全是数字键（表示数组）

    // 先检查所有一级键是否都是数字开头（如 "0__xxx"）
    for (const key in flatObject) {
        if (!/^\d+__/.test(key)) {
            isTopLevelArray = false;
            break;
        }
    }

    // 如果是数组，初始化 result 为数组
    if (isTopLevelArray) {
        const maxIndex = Math.max(
            ...Object.keys(flatObject).map(k => parseInt(k.split('__')[0]))
        );
        result = new Array(maxIndex + 1);
    }

    for (const flatKey in flatObject) {
        if (!flatObject.hasOwnProperty(flatKey)) continue;

        const keys = flatKey.split('__');
        let current = result;

        for (let i = 0; i < keys.length; i++) {
            const key = keys[i];
            const isLast = i === keys.length - 1;

            // 如果当前 key 是数字，且当前 current 是数组（或应初始化为数组）
            if (/^\d+$/.test(key)) {
                const index = parseInt(key);

                if (!Array.isArray(current)) {
                    // 如果 current 不是数组，但遇到数字 key，强制转为数组
                    const newArray = [];
                    Object.keys(current).forEach(k => {
                        newArray[k] = current[k];
                    });
                    current = newArray;
                    // 如果是顶层，直接替换 result
                    if (i === 0) result = current;
                }

                if (isLast) {
                    current[index] = flatObject[flatKey];
                } else {
                    if (!current[index]) {
                        current[index] = {};
                    }
                    current = current[index];
                }
            }
            // 普通字符串 key
            else {
                if (isLast) {
                    current[key] = flatObject[flatKey];
                } else {
                    if (!current[key]) {
                        current[key] = {};
                    }
                    current = current[key];
                }
            }
        }
    }

    return result;
}

/**
 * 把对象生成为__分割，这个一般用于赋值
 * @param {*} obj 
 * @param {*} parentKey 
 * @param {*} result 
 * @returns 
 */
function flattenObject(obj, parentKey = '', result = {}) {
    if (Array.isArray(obj)) {
        // 处理数组：生成 0__key, 1__key 等
        obj.forEach((item, index) => {
            const currentKey = parentKey ? `${parentKey}__${index}` : `${index}`;
            if (typeof item === 'object' && item !== null) {
                flattenObject(item, currentKey, result);
            } else {
                result[currentKey] = item;
            }
        });
    } else if (typeof obj === 'object' && obj !== null) {
        // 处理普通对象
        for (const key in obj) {
            if (!obj.hasOwnProperty(key)) continue;
            const currentKey = parentKey ? `${parentKey}__${key}` : key;
            const value = obj[key];
            if (typeof value === 'object' && value !== null) {
                flattenObject(value, currentKey, result);
            } else {
                result[currentKey] = value;
            }
        }
    } else {
        // 直接值（如字符串、数字等）
        if (parentKey) {
            result[parentKey] = obj;
        }
    }
    return result;
}

function isNumber(str) {
    return !isNaN(parseFloat(str)) && isFinite(str);
}

/**
 * 读取表单数据，处理过后得纯数据，可以用于测试等(不提交API)
 * @returns 
 */
function handlerReadFormData() {

    //表单验证
    if (!_beforeValidity("#paste-form")) {
        return null;
    }
    return readFormDataSkipValidity(true);

}

/**
 * 跳过验证，读取表单数据 特殊情况下使用
 * 一般给比如动态构建url的时候使用,需要传入false
 */
function readFormDataSkipValidity(boolValidity){
    var _info = $("#paste-form").parseForm();

    if(boolValidity){
        //表单验证的补充
        var _result = FuncHandlerParseFormValue(_info);
        if (!_result) {
            return null;
        }
    }
    if (_map_values) {
        for (let akey of _map_values.keys()) {
            if (akey) {
                if (!_info.hasOwnProperty(akey)) {
                    _info[akey] = _map_values.get(akey);
                }
            }
        }
    }
    if (_config.model == "object") {
        if (_global_expend_object) {
            for (let akey in _global_expend_object) {
                if (akey) {
                    if (!_info.hasOwnProperty(akey)) {
                        _info[akey] = _global_expend_object[akey];
                    }
                }
            }
        }
    }

    //和下面这个顺序不能高反了，因为动态的会校验是否存在这个对象
    //支持多层级的object info__name转化成info.name=xxx 注意是双下划线符号
    if (_config.dynamic) {
        _info = convertFlattenedToNested(_info);
    }

    //前面全部处理好了后，才要加这个！
    //针对group分组的特殊处理，删除没有值的分组，防止提交后后端用默认值填充了
    if (_global_class_attributes) {
        for (var _attribute of _global_class_attributes) {
            if (_attribute.name == "group") {
                if (_attribute.args4) {
                    //可能是directsun的模式
                    if (_attribute.args4 == "edit") {
                        if (_attribute.args1) {
                            var _group_name = _attribute.args1.split('__')[0];
                            if (!_info.hasOwnProperty(_group_name)) {
                                _info[_group_name] = null;//directsun模式，删除了内容,防止提交后后端用默认值填充了
                            }
                        }
                    }
                }
            }
        }
    }

    //关于非object的null等，需要后端默认值，比如默认为空

    return _info;
}

/**
 * 提交表单 提交表单的数据，最好前面加一层转化啥的
 */
async function funcSubmitForm() {
    // //表单验证
    var _info = handlerReadFormData();
    if (_info == null) { return; }

    if (_config.model == "sunobj") {
        return _info;
    }
    if (_config.model == "object") {
        window.SelectObject = _info;
        var handlered = false;
        // 如果当前窗口没有，且有父窗口且父窗口不是自己
        if (parent && parent !== window) {
            if (parent.handlerReadOpenValue && typeof parent.handlerReadOpenValue === 'function') {
                parent.handlerReadOpenValue(_info, _classPath);
                var index = parent.layer.getFrameIndex(window.name);
                parent.layer.close(index);
                handlered = true;
            }
            if (parent.FuncClickSure && typeof parent.FuncClickSure === 'function') {
                // 检查是否跨域（跨域时访问属性可能抛异常）
                parent.FuncClickSure();
                handlered = true;
            }
        }

        // 最终尝试顶层窗口
        if (!handlered) {
            if (window.top && window.top !== window) {
                var wtop = window.top;
                if (wtop.handlerReadOpenValue && typeof wtop.handlerReadOpenValue === 'function') {
                    wtop.handlerReadOpenValue(_info, _classPath);
                    var index = parent.layer.getFrameIndex(window.name);
                    parent.layer.close(index);
                    handlered = true;
                }
                if (wtop.FuncClickSure && typeof wtop.FuncClickSure === 'function') {
                    wtop.FuncClickSure();
                }
            }
        }
        //通知上级获取数据，并关闭自己... .. .
        if (!handlered) {
            layer.msg('父级需要实现函数 handlerReadOpenValue(obj,path)用来接收数据!');
        }
        return _info;
    }

    // console.log('line.755');
    // console.log(JSON.stringify(_info));

    if (_id && _id != '0' && _id != 0) {
        if (typeof apipostPromise == "function") {
            var read = await apipostPromise(`${auto_path()}/updateItem${location.search}`, _info);
            if (read.code == 200) {
                funcAfterAction(read.info, "更新成功");
            }
        } else {
            _apipost(`${auto_path()}/updateItem${location.search}`, true, JSON.stringify(_info), function (code, obj) {
                if (code == 200) {
                    funcAfterAction(obj, "更新成功");
                }
            });
        }
    } else {
        if (typeof apipostPromise == "function") {
            var read = await apipostPromise(`${auto_path()}/item${location.search}`, _info);
            if (read.code == 200) {
                funcAfterAction(read.info, "新增成功");
            }
        } else {
            _apipost(`${auto_path()}/item${location.search}`, true, JSON.stringify(_info), function (code, obj) {
                if (code == 200) {
                    funcAfterAction(obj, "新增成功");
                }
            });
        }
    }
}

/**
 * 动作之后
 * @param {*} obj 
 */
function funcAfterAction(obj, _msg) {
    // var _msg = '添加成功'; 这里有问题，如果ID就是string类型的...估计得引入一个特性... .. .
    if (typeof obj == 'string' && !isNumber(obj)) {
        _msg = obj;
    }

    if (typeof obj == "object") {
        // if()
        findAndCallHandler(obj);
    } else {
        // console.log(typeof (obj));
    }

    layer.msg(_msg, { time: 1500 }, function () {
        var okaction = funcFindAttribute(_global_class_attributes, "okaction");
        if (okaction) {
            if (okaction.args3) {
                const tourl = okaction.args3.replace("{{id}}", obj);
                if (okaction.args1 == "current") {
                    switch (okaction.args2) {
                        case "href":
                            {
                                location.href = tourl;
                            }
                            break;
                        case "replace":
                            {
                                location.replace(tourl);
                            }
                            break;
                        case "open":
                            {
                                window.open(tourl);
                            }
                            break;
                        case "reload":
                            {
                                location.reload();
                            }
                            break;
                        default:
                            break;
                    }
                } else {
                    switch (okaction.args2) {
                        case "href":
                            {
                                window.parent.location.href = tourl;
                            }
                            break;
                        case "replace":
                            {
                                window.parent.location.replace(tourl);
                            }
                            break;
                        case "open":
                            {
                                window.parent.window.open(tourl);
                            }
                            break;
                        case "reload":
                            {
                                window.parent.location.reload();
                            }
                            break;
                        default:
                            break;
                    }
                }
                // location.replace(okaction.args3.replace("{{id}}",obj));
            } else {
                if (okaction.args2 == "reload") {
                    if (okaction.args1 == "current") {
                        window.location.reload();
                    } else {
                        window.parent.location.reload();
                    }
                }
            }
        } else {
            var index = parent.layer.getFrameIndex(window.name); //获取窗口索引
            parent.layer.close(index);  // 关闭layer      
        }
    });

    //已经是根基模式了，如何获得上一页的信息...
    //handlerUpdateRow

}


//以下2个函数已经迁移到form.js中 26.08.08
// /**
//  * 从父级查找对应的页面，然后吧值更新到页面中，这里针对的是table的行数据新增或者变更！
//  * @param {} obj 
//  * @returns 
//  */
// function findAndCallHandler(obj) {

//     // console.log('准备查找表格，更新数据');

//     // 定义一个内部查找函数，专门负责在指定的窗口(window)里找 iframe 并调用
//     const searchInWindow = (targetWin) => {
//         // 1. 优先找 layer 弹窗 (层级通常最高)
//         // 注意：layer 的 iframe 类名通常是 layui-layer-iframe
//         var iframes = targetWin.document.querySelectorAll('.layui-layer-iframe');

//         // 2. 如果没找到，或者想兼容普通容器，再找 .show_iframe
//         if (iframes.length === 0) {
//             var containers = targetWin.document.querySelectorAll('.show_iframe');
//             if (containers.length > 0) {
//                 // 找到了容器，从容器里取 iframe
//                 // 这里假设一个容器里只有一个主要 iframe，或者取第一个
//                 iframes = containers[0].querySelectorAll('iframe');
//             }
//         }

//         // 3. 倒序遍历 (reverse)，确保最新打开的（显示在上面的）先被调用
//         // 这一步替代了复杂的 z-index 计算
//         var iframeList = Array.from(iframes).reverse();

//         for (let iframe of iframeList) {

//             try {
//                 if (iframe.contentWindow && typeof iframe.contentWindow.handlerUpdateRow === 'function') {
//                     // console.log('找到并调用 handlerUpdateRow');
//                     iframe.contentWindow.handlerUpdateRow(obj);
//                     return true;
//                 }
//             } catch (e) {
//                 // 跨域或错误忽略
//             }
//         }
//         return false;
//     };

//     // ================= 核心逻辑判断 =================

//     var find = false;
//     // 情况 A：当前页面本身就是一个 iframe (比如 Page 5)
//     // 它的父级 (Page 4) 才是我们要找的目标容器
//     if (window.top !== window.self) {
//         // 这里的逻辑是：我在 Page 5，我要去 Page 4 (parent) 里找其他的兄弟 iframe
//         // 因为 Page 4 可能同时打开了 Page 5 和 Page 6
//         find = searchInWindow(window.top);
//     }
//     // 情况 B：当前页面是顶层窗口 (比如 Page 6，或者是直接用 window.open 打开的独立窗口)
//     else {
//         // 这里的逻辑是：我就是顶层，我要在我自己 (window) 里找所有的 iframe
//         find = searchInWindow(window);
//     }
//     const rootWindow = window.top || window.parent;
//     if (!find) {
//         find = searchInWindow(rootWindow);
//     }
//     if (!find) {

//         const dialogIFrames = rootWindow.document.querySelectorAll(".layui-layer-iframe");
//         var dialogArray = Array.from(dialogIFrames).reverse();
//         for (let showIframe of dialogArray) {
//             var handler = callUpdateRow(showIframe, obj);
//             if (handler) {
//                 return true;
//             }
//         }

//         // findAndCallHandlerV2(obj);
//         const showIframes = rootWindow.document.querySelectorAll('.show_iframe');
//         for (let showIframe of showIframes) {
//             var handler = callUpdateRow(showIframe, obj);
//             if (handler) {
//                 return true;
//             }
//         }
//     }
// }


// /**
//  * 查找iframe并执行函数 handlerUpdateRow 当前这个版本即将删除，请使用非V2版本
//  * @param {*} showIframe 
//  * @returns 
//  */
// function callUpdateRow(showIframe, obj) {
//     try {
//         // 检查 .show_iframe 是否可见
//         const style = window.getComputedStyle(showIframe);
//         if (style.display !== 'none' && style.visibility !== 'hidden') {
//             // 在显示的 .show_iframe 中查找 iframe
//             const iframes = showIframe.querySelectorAll('iframe');
//             for (let iframe of iframes) {
//                 try {
//                     const iframeStyle = window.getComputedStyle(iframe);
//                     // 检查 iframe 是否可见且已加载
//                     if (iframeStyle.display !== 'none' && iframeStyle.visibility !== 'hidden' && iframe.contentWindow) {
//                         const iframeWindow = iframe.contentWindow;
//                         // 检查是否存在 handlerUpdateRow 函数
//                         if (typeof iframeWindow.handlerUpdateRow === 'function') {
//                             // 调用 handlerUpdateRow 并传入 obj
//                             iframeWindow.handlerUpdateRow(obj);
//                             // handlerUpdate = true;
//                             return true;
//                         }
//                     }
//                 } catch (error) {
//                     // 处理跨域限制等情况
//                     console.warn('无法访问 iframe 内容:', error);
//                 }
//             }
//         }
//     } catch (error) {
//         console.warn('无法访问 .show_iframe 元素:', error);
//     }
//     return false;
// }

/**
 * 删除一个组
 * @param {*} gcode 
 */
function handler_group_remove(gcode, event) {
    // $("#group_id_" + gcode).remove();
    $("#group_id_" + gcode).fadeOut(500, function () {
        $(this).remove(); // 动画完成后执行删除
    });
    var _k = -1;
    var removeProperties = new Array();
    for (let k = 0; k < _ui_data_list.length; k++) {
        let _g = _ui_data_list[k];
        if (_g.isgroup) {
            if (_g.gcode == gcode) {
                _k = k;
                if (_g.groups) {
                    for (let gitem of _g.groups) {
                        removeProperties.push(gitem.name);
                    }
                }
                break;
            }
        }
    }
    if (_k >= 0) {
        _ui_data_list.splice(_k, 1);
    }
    // 从 _global_data_properties 中移除相关属性
    if (removeProperties.length > 0) {
        // 创建一个新数组来保存不需要删除的属性
        var newProperties = [];

        for (var j = 0; j < _global_data_properties.length; j++) {
            var ditem = _global_data_properties[j];
            var shouldRemove = false;

            // 检查当前属性是否在要删除的列表中
            for (var p = 0; p < removeProperties.length; p++) {
                if (ditem.name === removeProperties[p]) {
                    shouldRemove = true;
                    break;
                }
            }

            // 如果不需要删除，就添加到新数组中
            if (!shouldRemove) {
                newProperties.push(ditem);
            }
        }

        // 用新数组替换原数组
        _global_data_properties = newProperties;
    }
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }
}

/**
 * 以当前组创建一个新的组
 * @param {*} gcode 
 */
function handler_group_add(gcode, event) {
    try {
        // var _add_html = $("#group_id_" + gcode).prop('outerHTML');
        // console.log('now group',gcode);

        let g_data_str = null;//string
        for (let _gdata of _ui_data_list) {
            if (_gdata.isgroup && _gdata.gcode == gcode) {
                g_data_str = JSON.stringify(_gdata);
                break;
            }
        }
        //如果子项有状态对象，就有问题了，比如富文本
        //获取到了后，需要替换关键字符。。。 。。 
        //如何计算出下一个group abx__13 abc__0__bd__2
        if (gcode.indexOf("__") >= 0) {
            var _last = gcode.lastIndexOf("__");
            var _head = gcode.substring(0, _last);
            var _next = parseInt(gcode.substring(_last + 2));
            if (_next != NaN) {
                //计算下一个id
                while (true) {
                    _next = _next + 1;
                    let _newgroup = _head + "__" + _next;
                    if (!_ui_data_list) {
                        break;
                    }
                    var _hit = false;
                    for (var ii of _ui_data_list) {
                        if (ii.gcode) {
                            if (ii.gcode == _newgroup) {
                                _hit = true;
                                break;
                            }
                        }
                    }
                    if (_hit) {
                        continue;
                    }
                    break;
                }

                //  console.log('new group '+_head+"__"+_next);

                let _new_group_code = `${_head}__${_next}`;
                if (g_data_str != null) {

                    var regex = new RegExp(`/${gcode}/g`);
                    g_data_str = g_data_str.replace(regex, _new_group_code);

                    let g_data_info = JSON.parse(g_data_str);
                    // g_data_info.groups = null;
                    g_data_info.gcode = _new_group_code;

                    var _reg = new RegExp(gcode, 'g');
                    // var _new_html = _add_html.replace(_reg, _new_group_code);
                    //这里要重置下，比如id这个属性得
                    if (g_data_info.groups) {
                        for (let aa of g_data_info.groups) {
                            aa.name = aa.name.replace(_reg, _new_group_code);
                            if (aa.name == `${g_data_info.gcode}__id`) {
                                if (aa.dataType == 'number') {
                                    aa.value = 0;
                                } else {
                                    //比如guid
                                }
                                if (aa.dataType == "fentoyuan") {
                                    aa.value = 0;
                                }
                                if (aa.dataType == "text") {
                                    aa.value = "";
                                }
                                //ui要如何变更... .. .
                                // break;
                            }
                            _global_data_properties.push(aa);
                        }
                    }
                    _ui_data_list.push(g_data_info);

                    let uilist = new Array();
                    uilist.push(g_data_info);
                    // var uiobj={
                    //     list:new Array()
                    // };
                    // uiobj.list.push(g_data_info);
                    // var ahtml=$("#template_group_item").html();
                    // var nhtml =template(ahtml,uiobj);

                    //进行多级模板渲染！
                    var _template_form = $("#template_group_item").html();
                    var _template_item = $("#template_tr_item").html();
                    var _tpl_form = template(_template_form);
                    var _tpl_item = template(_template_item);
                    // console.log('uilist',uilist);
                    //以下是新版本
                    var _ahtml = _tpl_form({ list: uilist, templatetr: _tpl_item, config: _config, tabs: [] });
                    // $(".paste-form-body").html(_ahtml);

                    $("#group_id_" + gcode).after(_ahtml);
                }
                // // _ui_data_list.push({ gcode: _new_group_code });//写入新的，防止重复
                // var _reg = new RegExp(gcode, 'g');
                // var _new_html = _add_html.replace(_reg, _new_group_code);

                // // 1. 将HTML字符串转为jQuery对象，并初始隐藏
                // var $newElement = $(_new_html).hide();
                // // 2. 插入到目标元素后面
                // $("#group_id_" + gcode).after($newElement);
                // // 3. 执行淡入动画
                // $newElement.fadeIn(500); // 时长可调
                layer.msg('已添加');
            }
        }
    } catch (err) {
        console.log(err);
        layer.msg('出现异常了，富文本等不支持分组，你可以改版成directsun模式');
    }
    // console.log(_add_html);
    event.preventDefault();
    event.stopPropagation();

}


/**
 * 使用图片选择器选择图片
 * 从元图片库选择图片
 * @param {*} elc 
 */
function funcDialogSelectImage(elc, url, query) {
    var dialogNum = $(elc).attr('data-num');
    var propertyName = $(elc).attr('data-name');
    // console.log($(elc).attr('data-name'));
    // console.log('name', propertyName, 'num', dialogNum);
    var _num = parseInt(dialogNum);
    var _name = propertyName;
    var lessNum = _num;
    if (_num > 1) {
        var haveNum = $(".drag_" + _name).find("img").length;
        lessNum = _num - haveNum;
        if (lessNum <= 0) {
            layer.msg("没有可选数量了，请先删除部分后再试");
            return;
        }
    }
    var fullUrl = `${url}${query}`;
    if (query) {
        if (query.indexOf("{{") >= 0) {
            //进行本地宏处理
            fullUrl = handlerBuildDynamicUrl(`${url}${query}`);
        }
    }
    var valField = null;
    var findProperty = funcFindPropertry(_name);
    if (findProperty != null) {
        var findAttribute = funcFindAttribute(findProperty.attributes, "imgexpend");
        //是否从这里获取 值字段？默认为url
        if (findAttribute != null) {
            if (findAttribute.args3) {
                valField = findAttribute.args3;
            }
        }
    }
    // funcOuterOpenSelect(`服务端图片选择器`, './images/index.html?type=imgs', lessNum, (obj) => {
    funcOuterOpenSelect(`服务端图片选择器`, fullUrl, lessNum, (obj) => {
        if (obj) {
            var objs = {
                data: new Array()
            };
            //这里模拟了上传图片得返回，因为当前是一个图片输入组件
            obj.forEach(oa => {
                if (typeof (oa) == 'object') {
                    // objs.data.push({ url: oa.url });
                    objs.data.push(obj);
                } else {
                    objs.data.push({ url: oa });
                }
            });
            if (valField && _num == 1) {
                var currentElc = $(`[name=${_name}]`);
                currentElc.parents(".formControls").find(".form-image-head").attr('src', obj.data[0].url);
                currentElc.parents(".formControls").find(".iconadd").hide();
                if (obj.data[0].hasOwnProperty(valField)) {
                    currentElc.val(obj.data[0][valField]);//特定字段，比如mediaid等
                } else {
                    currentElc.val(obj.data[0].url);
                }
                currentElc.parents(".formControls").find(".icon-close").show();
            } else {
                appendImageToUI(_num, _name, objs);
            }
            //是否是mediaid的模式
        }
    });
}