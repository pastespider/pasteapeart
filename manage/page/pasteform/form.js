"use strict";

var _classPath = "";
/**
 * 用于view承载extend扩展信息，比如extendUser特殊地方需要使用
 */
var _global_expend_object = {};

/**
 * 字段集合 或者index.html中的查询字段
 */
var _global_data_properties = null;//当前模型的属性列表

/**
 * 表的属性
 */
var _global_class_attributes = null;

/**
 * 组合路径
 * @returns 
 */
function auto_path() {
    if (_classPath.indexOf("/") >= 0) {
        return _classPath;
    } else {
        return `/${readRootPath()}/${_classPath}`;
    }
}

/**
 * 获取根路径 示例/api/app
 * @returns 
 */
function auto_root_path() {
    if (_classPath.indexOf("/") >= 0) {
        return _classPath.substring(0, _classPath.lastIndexOf("/"));
    } else {
        return `/${readRootPath()}`;
    }
}

/**
 * 为子页面调用使用，点击layer.open的确定按钮 子页面点击选择 即关闭页面
 */
function FuncClickSure() {

    // $(".layui-layer-btn0").trigger('click');
    // console.log(location.href, "trigger.click");
    // var lays = document.getElementsByClassName("layui-layer-shade");
    // if (lays) {
    //     for (var _lay of lays) {
    //         $(_lay).trigger('click');
    //     }
    // }

    // 找到所有包含 btn0 的弹层，按 z-index 降序排列，取第一个
    $('.layui-layer').has('.layui-layer-btn0').sort(function (a, b) {
        return $(b).css('z-index') - $(a).css('z-index');
    }).first().find('.layui-layer-btn0').trigger('click');

}

/**
 * 隐藏dialog下方的确定取消按钮条
 */
function FuncHiddenSureButtons() {
    // console.log('layer-btn hide ',location.href);
    $(".layui-layer-btn").hide();
    console.log(location.href, "FuncHiddenSureButtons");
}

/**
 * 2026.01.05 修正 ./ ../等的问题！
 * @param {*} path 
 * @returns 
 */
function autoUrl(path) {
    // 获取当前页面的协议、域名和路径
    const protocol = window.location.protocol;
    const host = window.location.host;
    const currentPath = window.location.pathname;
    const baseUrl = protocol + '//' + host;

    let fullUrl;

    if (path.startsWith('/')) {
        // 如果 path 是绝对路径，直接拼接协议和域名
        fullUrl = baseUrl + path;
    } else {
        // 如果 path 是相对路径，先使用URL API解析
        const currentDir = currentPath.substring(0, currentPath.lastIndexOf('/') + 1);
        fullUrl = new URL(path, baseUrl + currentDir).href;
    }

    // 处理多个 pasteform 文件夹路径
    const urlObj = new URL(fullUrl);
    const parts = urlObj.pathname.split('/').filter(p => p);

    const newParts = [];
    let pasteformFound = false;

    for (const part of parts) {
        if (part === 'pasteform') {
            if (!pasteformFound) {
                newParts.push(part);
                pasteformFound = true;
            }
        } else {
            newParts.push(part);
        }
    }

    urlObj.pathname = '/' + newParts.join('/');
    return urlObj.href;
}

/**
 * 打开一个窗口
 * @param {*} title 
 * @param {*} url 
 * @param {*} _w 设定的宽度，可以不设置，默认为90%，可以设置为xxxpx
 * @param {*} _h 设定的高度，可以不设置，默认为90%,可以设置为xxxpx
 */
function open_window(title, url, _w, _h, endcallback) {
    if (url.indexOf('pasteform') >= 0) {
        url = autoUrl(url);
    }
    if (!_w) {
        _w = '1200px';
    }
    if (!_h) {
        _h = '800px';
    }
    var _th = '100%';
    var _tw = '100%';
    if (_w) {
        var maxHeight = $(window).width();
        if (_w.indexOf('px') >= 0) {
            var _current_w = Number(_w.replace('px', ''));
            if (_current_w <= maxHeight) {
                _tw = _w;
            } else {
                console.log('target.width', _w, "max.width", maxHeight);
            }
        }
        if (_w.indexOf("%") > 0) {
            _tw = _w;
        }
    }
    if (_h) {
        var maxHeight = $(window).height();
        if (_h.indexOf('px') >= 0) {
            var _current_h = Number(_h.replace('px', ''));
            if (_current_h <= maxHeight) {
                _th = _h;
            }
        }
        if (_h.indexOf("%") > 0) {
            _th = _h;
        }
    }
    window.top.layer.open({
        type: 2,
        area: [_tw, _th],
        fix: false, //不固定
        maxmin: true,
        resize: true,
        shadeClose: true,
        shade: 0.8,
        title: title,
        content: url,
        success: function (layero, index) {
            // if(has_reset_size){
            //     setTimeout(()=>{
            //         layero.css('opacity', '1');
            //         console.log('延迟设置为1透明度');
            //     },1000);
            // }
            //TODO 后续调整为，页面注备就绪后再显示 skin不行是显示后追加的样式 而不是一开始... .. .
        },
        end: function () {
            if (endcallback) {
                endcallback();
            }
        }
    });

    //如果当前页面有回调函数，则先进行0透明 后面再显示
    // if (window.top.set_dialog_height) {
    //     window.top.set_dialog_height(_height, _index);
    // } else {
    //     console.log('没有在父级找到函数set_dialog_height');
    // }
}

/**
 * 打开，从右侧打开到左侧
 * @param {*} title 
 * @param {*} url 
 * @param {*} _w 
 * @param {*} _h 
 * @param {*} endcallback 
 */
function open_left(title, url, _w, _h, endcallback) {
    if (url.indexOf('pasteform') >= 0) {
        url = autoUrl(url);
    }
    if (!_w) {
        _w = '50%';
    }
    if (!_h) {
        _h = '100%';
    }
    var _th = '100%';
    var _tw = '50%';
    if (_w) {
        var maxHeight = $(window).width();
        if (_w.indexOf('px') >= 0) {
            var _current_w = Number(_w.replace('px', ''));
            if (_current_w <= maxHeight) {
                _tw = _w;
            } else {
                console.log('target.width', _w, "max.width", maxHeight);
            }
        }
        if (_w.indexOf("%") > 0) {
            _tw = _w;
        }
    }
    if (_h) {
        var maxHeight = $(window).height();
        if (_h.indexOf('px') >= 0) {
            var _current_h = Number(_h.replace('px', ''));
            if (_current_h <= maxHeight) {
                _th = _h;
            }
        }
        if (_h.indexOf("%") > 0) {
            _th = _h;
        }
    }
    window.layer.open({
        type: 2,
        area: [_tw, _th],
        fix: false, //不固定
        maxmin: false,
        resize: false,
        shadeClose: true,
        shade: 0.2,
        offset: 'r',
        title: title,
        anim: 'slideLeft',
        skin: 'layui-layer-win10',
        content: url,
        success: function (layero, index) {
        },
        end: function () {
            if (endcallback) {
                endcallback();
            }
        }
    });

    //如果当前页面有回调函数，则先进行0透明 后面再显示
    // if (window.top.set_dialog_height) {
    //     window.top.set_dialog_height(_height, _index);
    // } else {
    //     console.log('没有在父级找到函数set_dialog_height');
    // }
}

//可以再来一个 右侧往左弹出的，用于特殊地方！

/**
 * 打开一个对话框,并等待选择结果
 * @param {*} _title 标题
 * @param {*} _url url
 * @param {*} _w 宽度 xxxpx/xxx%
 * @param {*} _h 高度
 * @param {*} callback 回调
 */
function open_select(_title, _url, _w, _h, callback) {

    //解析linkquery特性
    // var _linkquery = handlerBuildLinkQueryUrl();
    // if(_linkquery){
    //     if(_url.indexOf("?")>=0){
    //         _url = _url+_linkquery;
    //     }else{
    //         _url = '?'+_linkquery.substring(1);
    //     }
    // }
    // console.log( "line.218",_url);
    //补充linkquery
    _url = handlerAutoQueryIncludeLinkQuery(_url);
    // console.log( "line.221",_url);
    //补充表单{{:=item.xxx}}动态参数
    _url = handlerBuildDynamicUrl(_url);
    // console.log( "line.224",_url);

    if (!_w) {
        _w = '80%';
    }
    if (!_h) {
        _h = '90%';
    }
    var _th = '100%';
    var _tw = '100%';

    if (_w) {
        var _max_height = $(window).width();
        if (_w.indexOf('px') >= 0) {
            var _current_w = Number(_w.replace('px', ''));
            if (_current_w < _max_height) {
                _tw = _w;
            }
        }
        if (_w.indexOf("%") > 0) {
            _tw = _w;
        }
    }
    if (_h) {
        var _max_height = $(window).height();
        if (_h.indexOf('px') >= 0) {
            var _current_h = Number(_h.replace('px', ''));
            if (_current_h < _max_height) {
                _th = _h;
            }
        }
        if (_h.indexOf("%") > 0) {
            _th = _h;
        }
    }

    window.top.layer.open({
        type: 2,
        area: [_tw, _th],
        fixed: true,
        maxmin: false,
        resize: false,
        scrollbar: false,
        shade: 0.6,
        shadeClose: true,
        title: _title,
        content: _url,
        btn: ['确定', '取消'],
        success: function (layero, index) {
        },
        yes: function (index, layero) {
            // console.log('yes:', _url);
            var _select = $(layero).find("iframe")[0].contentWindow.SelectObject;
            window.top.layer.close(index);
            // console.log('selected:', _select);
            callback(_select);
        },
        end: function () {
            console.log('open_select.end', _url);
        }
    });
}

/**
 * 子窗口穿透过来，重新计算高度
 * @param {*} _height 
 * @param {*} _index 
 */
function set_dialog_height(_height, _index) {
    var _max_height = $(window).height();
    if (_height < _max_height * 0.9) {
        $(`#layui-layer${_index}`).css('height', _height);
        $(`#layui-layer${_index}`).css('top', (_max_height - _height) / 2);
        $(`#layui-layer-iframe${_index}`).css('height', (_height - 40));
    } else {
        var ilayerHeight = $(`#layui-layer${_index}`).height();
        $(`#layui-layer-iframe${_index}`).css('height', (ilayerHeight - 40));
    }
}

/**
 * 查找某一个属性
 * _global_data_properties
 * 如果要查找_global_list_properties 则使用 funcQueryPropertry
 * @param {*} _name 
 * @returns 
 */
function funcFindPropertry(_name) {
    if (_global_data_properties) {
        for (var one of _global_data_properties) {
            if (one.name == _name) {
                return one;
            }
        }
    }
    return null;
}

/**
 * 查找这个属性
 * _global_list_properties
 * @param {*} _name 
 * @returns 
 */
function funcQueryPropertry(_name) {
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
 * 查找同级的另外一个字段，有可能有__ _global_data_properties
 * @param {*} _name extendUser
 * @param {*} _from userId 这个可能为 user__userId
 * @returns 
 */
function funcFindExpendPropertry(_name, _from) {
    var _new_name = false;
    if (_from) {
        if (_from.indexOf("__") > 0) {
            var _la = _from.lastIndexOf("__");
            var _head = _from.substring(0, _la);
            _new_name = _head + "__" + _name;
        }
    }
    if (_global_data_properties) {
        //是否要进行忽略大小写的查找对比？
        for (var one of _global_data_properties) {
            if (one.name == _name || (_new_name && one.name == _new_name)) {
                return one;
            }
        }
    }
    //_readObject是session_key暂存的 需要转化成一个voloModelInfo
    if (_global_expend_object) {
        if (_global_expend_object.hasOwnProperty(_name)) {
            var info = {
                name: _name,
                hidden: true,
                value: _global_expend_object[_name]
            };
            return info;
        }
        //应对多层级的问题
        if (_new_name) {
            if (_global_expend_object.hasOwnProperty(_new_name)) {
                var info = {
                    name: _new_name,
                    hidden: true,
                    value: _global_expend_object[_new_name]
                };
                return info;
            }
        }
    }
    return null;
}

/**
 * 从某一个字段中查找某一个属性
 * @param {*} attributes 
 * @param {*} _name 
 * @returns 
 */
function funcFindAttribute(attributes, _name) {
    if (attributes) {
        for (var one of attributes) {
            if (one.name == _name) {
                return one;
            }
        }
    }
    return null;
}

/**
 * 打开一个表的列表，然后进行选择！
 * 后续用 funcLinkOuterOpenSelect
 * @param {*} title 
 * @param {*} className userInfo或者userInfo&level=1或者./pasteform/index.html?path=userInfo&level=1
 * @param {*} num 
 * @param {*} callback 
 */
function funcOuterOpenSelect(title, className, num, callback) {
    var _url = className;
    if (_url.indexOf('/') == -1) {
        _url = `${handlerPasteFormPath()}index.html?model=select&path=${className}`;
    } else {
        _url = handlerBuildDynamicUrl(className);
    }
    if (_url.indexOf("/") == -1) {
        _url = `${handlerPasteFormPath()}index.html?path=${_url}`;
    }
    open_select(title, _url + '&select_num=' + num, "", "", callback);
}

/**
 * 获取pasteform的根目录 
 * 返回格式 /page/pasteform/
 * 也有可能是 比如/manage/pasteform/
 * 注意pasteform的框架的文件夹必须是这个名字才行！
 * 如果没有，则返回空
 */
function handlerPasteFormPath() {
    var _path = location.pathname;
    var _path_index = _path.indexOf("/pasteform/");
    if (_path_index >= 0) {
        return _path.substring(0, _path_index) + "/pasteform/";
    }
    return "";
}

/**
 * 打开对话框，基于sessionStorage存储读取对象信息并显示到表单中等待回传
 * @param {*} _title 
 * @param {*} _className 
 * @param {*} _session_key 
 * @param {*} callback 
 */
function funcObjectOpenSelect(_title, _className, _session_key, callback) {
    var _url = _className;
    if (_url.indexOf('/') == -1) {
        _url = `${handlerPasteFormPath()}view.html?model=object&path=${_className}&session_key=${_session_key}`;
    } else {
        _url = handlerBuildDynamicUrl(_className);
    }
    open_select(_title, _url, "", "", callback);
}

/**
* 注入时间区间 注意这个daterange库是改版过的，就是在库里面已经为这个输入框赋值了
* @param {*} elcid 
*/
function HandlerInitDateRange(elcid, _value) {
    var _start = '';
    var _end = '';
    if (_value) {
        var splits = _value.split(' ');
        _start = splits[0];
        var sizes = splits.length;
        _end = splits[sizes - 1];
    }
    if (typeof (pickerDateRange) != 'undefined') {
        if ($(`#${elcid}`).length > 0) {
            new pickerDateRange(elcid, {
                isTodayValid: true,
                defaultText: '  至  ',
                startDate: _start,
                endDate: _end,
                success: function (obj) { },
                clear: function () { }
            });
        } else {

        }
    } else {
        console.log('没有找到模块pickerDateRange 注入daterange失败！');
    }
}

// 判断字符串是否以.js结尾的函数，兼容旧浏览器
function isJsFile(str, suffix) {
    // const suffix = '.js';
    return str.slice(-suffix.length) === suffix;
}

/**
 * 载入js或者css等 当前残缺的版本 请使用 loadResource
 * @param {*} src 
 * @param {*} func 
 */
function loadLibFile(src, func) {
    if (src) {
        if (isJsFile(src, ".js")) {
            const script = document.createElement('script');
            script.src = src;
            script.onload = () => {
                if (func) {
                    const functionToExecute = window[func];
                    // 检查函数是否存在且为函数类型
                    if (typeof functionToExecute === 'function') {
                        // 执行函数
                        functionToExecute();
                    } else {
                        console.error(`名为 ${func} 的函数不存在或者不是一个有效的函数。`);
                    }
                }
            };
            script.onerror = () => {
                console.error(`Failed to load script: ${src}`);
            };
            document.head.appendChild(script);
        }
        if (isJsFile(src, ".css")) {
            const link = document.createElement('link');
            link.rel = 'stylesheet';
            link.href = src;
            link.onload = () => {
                if (func) {
                    const functionToExecute = window[func];
                    // 检查函数是否存在且为函数类型
                    if (typeof functionToExecute === 'function') {
                        // 执行函数
                        functionToExecute();
                    } else {
                        console.error(`名为 ${func} 的函数不存在或者不是一个有效的函数。`);
                    }
                }
            };
            link.onerror = () => {
                console.error(`Failed to load CSS: ${src}`);
            };
            document.head.appendChild(link);
        }
        //载入模板数据
        if (isJsFile(src, ".html")) {
            const link = document.createElement('link');
            link.rel = 'stylesheet';
            link.href = src;
            link.onload = () => {
                if (func) {
                    const functionToExecute = window[func];
                    // 检查函数是否存在且为函数类型
                    if (typeof functionToExecute === 'function') {
                        // 执行函数
                        functionToExecute();
                    } else {
                        console.error(`名为 ${func} 的函数不存在或者不是一个有效的函数。`);
                    }
                }
            };
            link.onerror = () => {
                console.error(`Failed to load CSS: ${src}`);
            };
            document.head.appendChild(link);
        }
    }
}


/**
 * 异步加载资源的函数
 * @param {*} src 
 * @param {*} func 
 * @returns 
 */
async function loadResource(src, func) {
    try {
        if (isJsFile(src, ".js")) {
            await loadScript(src);
        }
        else if (isJsFile(src, ".css")) {
            await loadCSS(src);
        }
        else if (isJsFile(src, ".html")) {
            await loadHTML(src);
        }
        // 资源加载完成后执行回调函数
        if (func) {
            const functionToExecute = window[func];
            if (typeof functionToExecute === 'function') {
                functionToExecute();
            } else {
                console.error(`名为 ${func} 的函数不存在或者不是一个有效的函数。`);
            }
        }
        return true; // 加载成功       
    } catch (error) {
        console.error(`Failed to load resource: ${src}`, error);
        return false; // 加载失败
    }
}

// 加载JS文件
function loadScript(src) {
    return new Promise((resolve, reject) => {
        const script = document.createElement('script');
        script.src = src;
        script.onload = () => resolve();
        script.onerror = () => reject(new Error(`Failed to load script: ${src}`));
        document.head.appendChild(script);
    });
}

// 加载CSS文件
function loadCSS(src) {
    return new Promise((resolve, reject) => {
        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = src;
        link.onload = () => resolve();
        link.onerror = () => reject(new Error(`Failed to load CSS: ${src}`));
        document.head.appendChild(link);
    });
}

// 加载HTML模板 - 正确的方式
function loadHTML(src) {
    return new Promise((resolve, reject) => {
        fetch(src)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.text();
            })
            .then(html => {
                // 创建一个临时容器来存放HTML内容
                const container = document.createElement('div');
                container.style.display = 'none'; // 隐藏容器
                container.innerHTML = html;
                document.body.appendChild(container);

                // 将容器存储在全局变量中以便后续访问
                if (!window.loadedTemplates) {
                    window.loadedTemplates = {};
                }
                window.loadedTemplates[src] = container;

                resolve(container);
            })
            .catch(error => {
                reject(new Error(`Failed to load HTML: ${src} - ${error.message}`));
            });
    });
}

/**
 * 获取范围内的随机值
 * @param {*} n 
 * @param {*} m 
 */
function randomValue(n, m) {
    return Math.floor(Math.random() * (m - n + 1)) + n;
}

//组列表
var _ui_data_list = new Array();

var BtnExpend = null;
/**
 * 缓存，哪些changeui特性
 */
var _changeui_list = new Array();//attribute val
/**
* 读取数据的模型 处理数据的模型
* @param {*} properties 
*/
async function LoadModelProperity(properties) {

    if (_config.model == "add" || _config.model == "list") {
        //加载query如果有 只有新增的时候需要，编辑模式不需要！！！
        LoadQuery(properties);
    }

    handlerExchangeDataTypeToUIType(properties);

    //如果要加载markdown
    if (_global_need_loadmarkdown) {
        await new Promise((resolve, reject) => {
            var _script = document.createElement("script");
            _script.src = "../lib/editor.md/editormd.min.js";
            _script.type = "text/javascript";
            _script.onload = function () {
                resolve();
            }
            _script.onerror = function () {
                reject();
            }
            document.head.appendChild(_script);
        });
        //../lib/editor.md/css/editormd.min.css
        const _style = document.createElement('link');
        _style.type = 'text/css';
        _style.rel = "stylesheet";
        _style.href = "../lib/editor.md/css/editormd.min.css";
        document.head.appendChild(_style);

        var _paste = document.createElement("script");
        _paste.src = "../lib/editor.md/md.paste.js";
        _paste.type = "text/javascript";
        document.head.appendChild(_paste);
    }
    //如果要加载富文本
    if (_global_need_loadrichhtml) {
        await new Promise((resolve, reject) => {
            var _script = document.createElement("script");
            _script.src = "../lib/wangeditor/wangEditor.min.js";
            _script.type = "text/javascript";
            _script.onload = function () {
                resolve();
            }
            _script.onerror = function () {
                reject();
            }
            document.head.appendChild(_script);
        });
        E = window.wangEditor;
    }

    //剔除hide的项目
    _global_data_properties = _global_data_properties.filter(item => !item.hide);

    //处理group模式
    for (var k = 0; k < properties.length; k++) {
        var item = properties[k];
        if (item.attributes) {
            var _group = funcFindAttribute(item.attributes, "group");
            if (_group) {
                handlerTakeGroupInfo(_group.args1, _ui_data_list, item);
            } else {
                _ui_data_list.push(item);
            }
        } else {
            _ui_data_list.push(item);
        }
        if (item.value) {
            if (item.value != null) {
                if (typeof (item.value) == "object") {
                    _global_expend_object[item.name] = item.value;
                }
            }
        }
    }

    //最上方的组描点
    var _ui_tabs = new Array();
    if (_ui_data_list && _ui_data_list.length > 0) {
        if (_ui_data_list[0].isgroup) {
            _ui_data_list.forEach(ia => {
                if (ia.isgroup) {
                    _ui_tabs.push(ia);
                }
            });
            $(".newform").css('padding-top', '0px');
        }
    }


    //handlerChangeUiAttribute
    //整理changeui的联动
    if (_changeui_list && _changeui_list.length > 0) {
        var changeUiMaps = new Map();
        for (let change_item of _changeui_list) {
            var read = changeUiMaps.get(change_item.name);
            if (read) {
                read.attributes.push(change_item.attribute);
            } else {
                changeUiMaps.set(change_item.name, {
                    value: change_item.value.toString(),
                    attributes: [change_item.attribute]
                });
            }
        }
        changeUiMaps.forEach((data, key) => {
            privateChangeUiHandler(data.attributes, data.value, false);
        })
        // for (let change_item of _changeui_list) {
        //     //name表示哪个字段 理论name一样value肯定一样，因为是传入值，可能attribute有多个 在这里要进行合并
        //     handlerChangeUiAttribute(change_item.attribute, change_item.value, false);
        // }

    }

    // console.log(_ui_data_list);

    //进行多级模板渲染！
    var _template_form = $("#template_form").html();
    var _template_item = $("#template_tr_item").html();
    var _tpl_form = template(_template_form);
    var _tpl_item = template(_template_item);

    //以下是新版本
    var _ahtml = _tpl_form({ list: _ui_data_list, templatetr: _tpl_item, config: _config, tabs: _ui_tabs });
    $(".paste-form-body").html(_ahtml);

    //以下是旧版本
    // var _template = $("#templatemodel").html();
    // var _ahtml = template(_template, { list: properties, config: _config });
    // $(".paste-form-body").html(_ahtml);

    setTimeout(function () {
        FormLoaded(properties);
        funcAppendInputLength();
        //计算高度，返回
        var _height = $(".newform").height() + 140 + 70;
        if (_has_outer) {
            //如果计算的高度小于600，表单中有outer则至少保证高度为600
            if (_height < 600) {
                _height = 600;
            }
        }
        var _index = parent.layer.getFrameIndex(window.name); //获取窗口索引
        // if (window.parent.set_dialog_height) {
        //     window.parent.set_dialog_height(_height, _index);
        // } else {
        //     console.log('没有在父级找到函数set_dialog_height');
        // }
        //这里有问题，如果是父级的呢 估计要多重扩展！
        if (window.top.set_dialog_height) {
            window.top.set_dialog_height(_height, _index);
        } else {
            console.log('没有在父级找到函数set_dialog_height');
        }
        /**
         * selects lselect paste-form-body
         */
        $(".paste-form-body").on('click', '.ulitems li', function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            } else {
                $(this).addClass('selected');
            }
            let nameElc = $(this).parents(".ulitems");
            var _name = nameElc.attr('name');
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
                        var _selnum = nameElc.find(".selected").length;
                        if (_selnum > _maxnum) {
                            if (_maxnum > 1) {
                                $(this).removeClass('selected');
                                layer.msg(`限定最多选择${_maxnum}项`);
                            } else {
                                $(this).siblings().removeClass('selected');
                            }
                        }
                        if (_maxnum == 1) {
                            var _val = $(this).attr('value');
                            handlerReloadReplace(_pro.attributes, _name, _val);
                            handlerChangeUi(_pro.attributes, _val);
                            // var _attribute = funcFindAttribute(_pro.attributes, "reload");
                            // if (_attribute) {
                            //     var _val = $(this).attr('value');
                            //     if (_attribute.args1) {
                            //         _name = _attribute.args1;
                            //     }
                            //     handlerBuildNewUrlToLoad(_name, _val);
                            // }
                        }
                    }
                    if (_attribute.name == "selects") {
                        //从ui中获取选中得 使用比如{name value }
                        updateSelectText(nameElc, _pro);
                    }
                    handlerLoaditem(_pro);
                }
            }

        });
        // //分组的开关
        // $(".paste-form-body").on('click','.btngroup',function(){
        //     if($(this).hasClass("Hui-iconfont-arrow2-top")){
        //         $(this).removeClass("Hui-iconfont-arrow2-top").addClass("Hui-iconfont-arrow2-bottom");
        //         $(this).parents('.group').find('.gbody').addClass('close');//.hide();
        //     }else{
        //         $(this).removeClass("Hui-iconfont-arrow2-bottom").addClass("Hui-iconfont-arrow2-top");
        //         $(this).parents('.group').find('.gbody').removeClass("close");
        //     }
        // });
        registry_group_click_event();

        //绑定图片拖拽功能
        let boolBreak = false;
        for (let pro of _global_data_properties) {
            if (pro.attributes) {
                for (let la of pro.attributes) {
                    if (la.name == "image" && la.args1) {
                        let numCount = parseInt(la.args1);
                        if (numCount > 1) {
                            if (typeof $.fn.dragSort === 'function') {
                                if ($(`.drag_${pro.name}`).length > 0) {
                                    let dragdom = $(`.drag_${pro.name}`).dragSort({})
                                    pro.dragimages = dragdom.data('dragSort');
                                }
                            } else {
                                loadLibFile("./lib/img_drag.js", 'privateBindImgDrag');
                                boolBreak = true;
                                break;
                            }
                        }
                    }
                };
            }
            if (boolBreak) {
                break;
            }
        }

    }, 100);

    if (location.search.indexOf("model=sunobj") == -1) {
        $(".paste-btn-submit").fadeIn();
    }
}

/**
 * 把多选的结果渲染到UI中
 * @param {*} $ul 
 * @param {*} namePro 
 * @returns 
 */
function updateSelectText($ul, namePro) {
    var name = $ul.attr('name');
    if (!name) return;

    var $input = $('input[name="' + name + '__sval"]');
    if (!$input.length) return;

    var $selected = $ul.find('.selectli.selected');
    var count = $selected.length;

    if (count === 0) {
        $input.val('');
        return;
    }

    var texts = $selected.map(function () {
        return $(this).text().trim();
    }).get();

    let sval = '(' + count + ')' + texts.join(' ');

    $input.val(sval);
    namePro.__selected_values = sval;
}

/**
 * 点击selects或者lselect时，弹出选择区域
 * @param {*} elc 
 */
function funcChangeSelectArea(elc) {
    var $panel = $(elc).parent().find('.ulitems');
    var rect = elc.getBoundingClientRect();
    var panel = $panel[0];

    var GAP = 5;          // 面板与触发元素的间距
    var MAX_HEIGHT = 300; // 面板最大高度
    var vw = window.innerWidth;
    var vh = window.innerHeight;

    // ---------- 1. 量面板实际高度（用 max-height 限制后的） ----------
    // 先让它可测量：display 保持，用 visibility:hidden + fixed 量高度
    var prevDisplay = panel.style.display;
    var prevVisibility = panel.style.visibility;
    var prevMaxHeight = panel.style.maxHeight;

    // 临时显示但不被看到
    panel.style.display = 'block';
    panel.style.visibility = 'hidden';
    panel.style.maxHeight = MAX_HEIGHT + 'px';

    // 量高度（此时如果内容多，会被 max-height 限制）
    var panelHeight = panel.offsetHeight || MAX_HEIGHT;
    // 如果内容超过 max-height，offsetHeight 会是 300；不够 300 就是实际高度
    var panelWidth = panel.offsetWidth || rect.width;

    // 恢复
    panel.style.display = prevDisplay;
    panel.style.visibility = prevVisibility;
    panel.style.maxHeight = prevMaxHeight;

    // ---------- 2. 垂直方向：优先下方 ----------
    var spaceBelow = vh - rect.bottom - GAP;
    var spaceAbove = rect.top - GAP;

    var top, finalHeight = Math.min(panelHeight, MAX_HEIGHT);

    if (spaceBelow >= finalHeight) {
        // 下方够 → 放下方
        top = rect.bottom + GAP;
    } else if (spaceAbove >= finalHeight) {
        // 下方不够，上方够 → 放上方（面板底边贴触发元素顶边）
        top = rect.top - GAP - finalHeight;
    } else {
        // 上下都不够 → 选空间大的一边，并把高度压到该空间
        if (spaceBelow >= spaceAbove) {
            top = rect.bottom + GAP;
            finalHeight = Math.max(0, spaceBelow);
        } else {
            finalHeight = Math.max(0, spaceAbove);
            top = rect.top - GAP - finalHeight;
        }
    }

    // 防止 top 出视口
    if (top < GAP) top = GAP;
    if (top + finalHeight > vh - GAP) {
        finalHeight = vh - GAP - top;
    }

    // ---------- 3. 水平方向：默认左对齐，右边不够就往左挪 ----------
    var left = rect.left;
    if (left + panelWidth > vw - GAP) {
        // 超出右边 → 右对齐触发元素
        left = rect.right - panelWidth;
    }
    if (left < GAP) left = GAP;

    // ---------- 4. 应用 ----------
    $panel.css({
        top: top + 'px',
        left: left + 'px',
        width: rect.width + 'px',      // 宽度还是跟触发元素对齐
        maxHeight: finalHeight + 'px', // 关键：动态限制高度
        overflowY: 'auto'              // 内容多时面板内部滚动
    });

    $panel.toggle();
}

// /**
//  * 打开和收缩多选区域 selects
//  * @param {*} elc 
//  */
// function funcChangeSelectArea(elc) {
//     var $panel = $(elc).parent().find('.ulitems');
//     var rect = elc.getBoundingClientRect();

//     $panel.css({
//         top: rect.bottom + 5 + 'px',
//         left: rect.left + 'px',
//         width: rect.width + 'px'
//     });

//     // 用 display 切换，简单可靠
//     $panel.toggle();
// }

/**
 * 载入图片拖拽功能的支持 绑定图片的拖拽功能
 * 字符串在其他地方引用了，不能修改名称
 */
function privateBindImgDrag() {
    for (let pro of _global_data_properties) {
        if (pro.attributes) {
            pro.attributes.forEach(la => {
                if (la.name == "image" && la.args1) {
                    let numCount = parseInt(la.args1);
                    if (numCount > 1) {
                        if (typeof $.fn.dragSort === 'function') {
                            if ($(`.drag_${pro.name}`).length > 0) {
                                let dragdom = $(`.drag_${pro.name}`).dragSort({})
                                pro.dragimages = dragdom.data('dragSort');
                            }
                        } else {
                            console.log('载入img_drag.js失败了？');
                        }
                    }
                }
            });
        }
    }
}

/**
 * 注册分组的点击事件
 */
function registry_group_click_event() {
    //分组的开关
    $(".paste-form-body").on('click', '.ghead', function () {
        var _current = $(this).find(".btnexcept");//展开收缩按钮
        if (_current) {
            if ($(_current).hasClass("Hui-iconfont-arrow2-top")) {
                $(_current).removeClass("Hui-iconfont-arrow2-top").addClass("Hui-iconfont-arrow2-bottom");
                $(_current).parents('.group').find('.gbody').addClass('close');//.hide();
            } else {
                $(_current).removeClass("Hui-iconfont-arrow2-bottom").addClass("Hui-iconfont-arrow2-top");
                $(_current).parents('.group').find('.gbody').removeClass("close");
            }
        }
    });
}

/**
 * 删除输入框的值
 * @param {*} elc 
 */
function handlerClean(elc) {
    var _parent = $(elc).parents('.formControls');
    if (_parent) {
        _parent.find('input,textarea').val('');
    }
}

/**
 * 加载表单的表的数据
 */
function loadHeader(obj) {
    if (obj.title) {
        $(".form-header").find(".ititle").html(obj.title);
    }
    if (obj.desc) {
        $(".form-header").find(".idesc").html(obj.desc);
    }
    if (obj.attributes) {
        _global_class_attributes = obj.attributes;
        for (var _attribute of obj.attributes) {
            switch (_attribute.name) {
                case "button":
                    {
                        if (_attribute.args1) {
                            $(".paste-btn-submit").val(_attribute.args1);
                        }
                    }
                    break;
                case "loadlib":
                    {
                        //载入资源
                        loadLibFile(_attribute.args1, _attribute.args2);
                    }
                    break;
            }
        }
    }
    //_global_attributes = o.attributes;
}


/**
 * parseForm之后的表单数据处理，处理完成后就上传接口了，所以要做一些额外的校验等
 * 读取富文本的内容
 * 读取daterange的内容
 * @param {*} _info 
 */
function FuncHandlerParseFormValue(_info) {
    if (_global_data_properties != null) {
        for (var item of _global_data_properties) {
            switch (item.dataType) {
                case "richtext":
                    {
                        //注意id不一样
                        if ($('#' + item.name + 'editor').is(':visible')) {
                            if (item.editor) {
                                item.value = item.editor.txt.html();
                                _info[item.name] = item.value;
                                var _attribute = funcFindAttribute(item.attributes, "richtext");
                                if (_attribute) {
                                    if (_attribute.args2) {
                                        _info[_attribute.args2] = item.editor.txt.text();
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "markdown":
                    {
                        //注意id不一样
                        if ($('#' + item.name + 'mdeditor').is(':visible')) {
                            if (item.editor) {
                                item.value = item.editor.getMarkdown();
                                _info[item.name] = item.value;
                                if (item.value) {
                                    var _attribute = funcFindAttribute(item.attributes, "markdown");
                                    if (_attribute) {
                                        if (_attribute.args2) {
                                            _info[_attribute.args2] = item.editor.getHTML();
                                        }
                                    }
                                }
                                delete _info[`${item.name}mdeditor-html-code`];
                                delete _info[`${item.name}mdeditor-markdown-doc`];
                            }
                        }
                    }
                    break;
                case "DateTime":
                case "Guid":
                    {
                        if (!item.required) {
                            if (_info[item.name] == '' || _info[item.name] == 'null') {
                                delete _info[item.name];
                            }
                        }
                    }
                    break;
                case "fentoyuan":
                    {
                        var _value = _info[item.name];
                        if (_value) {
                            _info[item.name] = Number((_value * 100).toFixed(2));// Number(Math.fround(_value * 100));
                        }
                    }
                    break;
                case "tosize":
                    {
                        //示例34KB转化34000类似这样的，会有耗损,其实这种情况很少用
                        var _value = _info[item.name];
                        if (_value) {
                            _info[item.name] = parseFileSize(_value);
                        }
                    }
                    break;
                case "score":
                    {
                        _info[item.name] = item.value;
                    }
                    break;
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
                                                if (_config.next_end_date) {
                                                    let _next_end = new Date(_end);
                                                    _next_end.setDate(_next_end.getDate() + 1);
                                                    let _next_end_str = `${_next_end.getFullYear()}-${(_next_end.getMonth() + 1).toString().padStart(2, '0')}-${_next_end.getDate().toString().padStart(2, '0')} 00:00:00`;
                                                    setValueToSameLevel(item.name, _info, _attribute.args2, _next_end_str);
                                                } else {
                                                    setValueToSameLevel(item.name, _info, _attribute.args2, _end);
                                                }
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
                case "outers":
                    {
                        var _dataFrom = item.dataFrom;
                        var elc = `.outers${item.name}`;
                        var lis = $(elc).find('li');
                        var _ids = new Array();
                        if (lis.length > 0) {
                            for (var k = 0; k < lis.length; k++) {
                                var ii = lis[k];
                                if (_dataFrom == 'Int32[]' || _dataFrom == 'Int64[]' || _dataFrom == 'UInt32[]' || _dataFrom == 'UInt64[]') {
                                    _ids.push(Number($(ii).attr('dataid')));
                                } else {
                                    _ids.push($(ii).attr('dataid'));
                                }
                            }
                            if (_dataFrom.indexOf("[]") > 0) {
                                _info[item.name] = _ids;
                            } else {
                                _info[item.name] = _ids.toString();
                            }

                        }
                    }
                    break;
                case "selects":
                case "lselect":
                    {
                        //基于结果进行值拼接！ 可能是多个 可能是一个... .. 
                        let handlered = false;
                        var _elcs = $(`[name=${item.name}]`).find('.selected');
                        if (_elcs && _elcs.length > 0) {
                            var _array = new Array();
                            for (var k = 0; k < _elcs.length; k++) {
                                _array.push($(_elcs[k]).attr('value'));
                            }
                            if (item.dataFrom.indexOf("[]") >= 0) {
                                if (item.dataFrom == "Int32[]" || item.dataFrom == "Int64[]" || item.dataFrom == "UInt32[]" || item.dataFrom == "UInt64[]" || item.dataFrom == "number[]") {
                                    var _newarray = new Array();
                                    _array.forEach(_ary => {
                                        _newarray.push(Number(_ary));
                                    });
                                    _info[item.name] = _newarray;
                                } else {
                                    _info[item.name] = _array;
                                }
                                handlered = true;
                            }
                            if (!handlered) {
                                let isnumber = false;
                                if (item.valueType) {
                                    if (item.valueType === "number") {
                                        isnumber = true;
                                    }
                                }
                                if (isnumber) {
                                    if (_array.length == 1) {
                                        _info[item.name] = Number(_array[0]);
                                        handlered = true;
                                    }
                                }
                                if (!handlered) {
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
                case "image":
                    {
                        //这里处理图片
                        if (item.num > 1) {
                            var _read = handlerReadImages(item.name);
                            if (_read) {
                                if (item.dataFrom == "String[]") {
                                    _info[item.name] = _read.split(',');
                                } else {
                                    _info[item.name] = _read;
                                }
                            } else {
                                delete _info[item.name];
                            }
                        } else {
                            if (_info[item.name] == 'null') {
                                delete _info[item.name];
                            }
                        }
                    }
                    break;
                case "object":
                    {
                        // if (item.dataFrom == "Object") {
                        if (_map_values.has(item.name)) {
                            _info[item.name] = _map_values.get(item.name);
                        } else {
                            delete _info[item.name];
                        }
                        // }
                    }
                    break;
                case "objects":
                    {
                        if (_map_values.has(item.name)) {
                            _info[item.name] = _map_values.get(item.name);
                        } else {
                            delete _info[item.name];
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
                case "cover":
                    {
                        let _img = $(".coverts .active");
                        if (_img) {
                            item.value = _img.attr("src");
                            _info[item.name] = item.value;
                        }
                        // var imgs = $(".coverts").find("img");
                        // if (imgs.length > 0) {
                        //     var _imgs = new Array();
                        //     for (var i = 0; i < imgs.length; i++) {
                        //         _imgs.push(imgs[i].attr("src"));
                        //     }
                        //     item.value = _imgs;
                        //     var _attribute = funcFindAttribute(item.attributes, "cover");
                        //     if (_attribute) {
                        //         if (_attribute.args2) {
                        //             _info[_attribute.args2] = _imgs[0];
                        //         }
                        //     }
                        // }
                    }
                    break;
                case "sunform":
                    {
                        var _id = "frame_" + item.name;
                        var _frame = document.getElementById(_id);
                        if (_frame) {
                            var _con = _frame.contentWindow;
                            if (_con) {
                                if (typeof _con.funcSubmitForm === 'function') {
                                    var _read = _con.funcSubmitForm();
                                    if (!_read) {
                                        return;
                                    } else {
                                        _info[item.name] = _read;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "datetime":
                    {
                        var _value = null;
                        if (_info.hasOwnProperty(item.name)) {
                            _value = _info[item.name];
                        }
                        var _attribute = funcFindAttribute(item.attributes, "datetime");
                        if (_attribute) {
                            if (_attribute.args2) {
                                switch (_attribute.args2) {
                                    case "times":
                                        {
                                            if (_value) {
                                                //_value要转换成完整的数据格式！
                                                var _time = parseDate(_value).getTime();
                                                _info[item.name] = _time / 1000;
                                            }
                                        }
                                        break;
                                    case "timems":
                                        {
                                            if (_value) {
                                                var _time = parseDate(_value).getTime();
                                                _info[item.name] = _time;
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            if (_value) {
                                                var _time = parseDate(_value).Format(_attribute.args2);
                                                _info[item.name] = _time;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    break;
                case "select":
                    {
                        if (item.valueType) {
                            if (item.valueType == "number") {
                                if (_info.hasOwnProperty(item.name)) {
                                    var _value = _info[item.name];
                                    if (typeof (_value) != "number") {
                                        _info[item.name] = Number(_value);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }

            if (item.nullable) {
                var _value = null;
                if (_info.hasOwnProperty(item.name)) {
                    _value = _info[item.name];
                }
                if (!_value) {
                    delete _info[item.name];
                } else {
                    if (_value == 'null') {
                        delete _info[item.name];
                    }
                }
            }

            if (item.dataFrom == "Guid") {
                var _value = null;
                if (_info.hasOwnProperty(item.name)) {
                    _value = _info[item.name];
                }
                if (_value == 'null') {
                    _value = null;
                }
                if (!_value) {
                    delete _info[item.name];
                }
                if (!item.nullable) {
                    if (!_value) {
                        _info[item.name] = "00000000-0000-0000-0000-000000000000";
                    }
                }
            }

            //校验必填项目!!!
            if (item.attributes) {
                //部分非官方的required的校验
                var _required = funcFindAttribute(item.attributes, "required");
                if (_required) {
                    //这里好像有问题啊 如果是group展开收缩呢?
                    if (item.hidden) {
                        continue;
                    }
                    var _hidden = $(`[name=${item.name}]`).closest('.hidden');
                    if (!_hidden || _hidden.length == 0) {
                        // console.log('检验field:',item.name,' 的 required的值！',_info[item.name]);
                        if (!_info[item.name]) {
                            const $target = $(`[name=${item.name}]`);
                            if ($target.is(":hidden")) {
                                // 查找同级的第一个input[type=text]元素
                                const siblingInput = $target.siblings('input[type="text"]').first();
                                if (siblingInput.length) {
                                    // 找到了同级元素，进行后续操作
                                    layer.tips(`数据 ${item.title} 必填，请重新填写`, siblingInput, { tipsMore: true }, 1);
                                } else {
                                    console.log("未找到同级input[type=text]元素");
                                }
                            } else {
                                $(`[name=${item.name}]`).focus();
                                layer.tips(`数据 ${item.title} 必填，请重新填写`, `[name=${item.name}]`, { tipsMore: true }, 1);
                            }
                            layer.msg(`数据 ${item.title} 必填，请重新填写`);
                            //这里有问题，如果是outer的，则应该是同级别中显示的UI提示!
                            return false;
                        }
                    }
                }
            }
        }
    }

    // if (_global_class_attributes) {
    //     for (var _attribute of _global_class_attributes) {
    //         if (_attribute.name == "group") {
    //             if (_attribute.args4) {
    //                 //可能是directsun的模式
    //                 if (_attribute.args4 == "edit") {
    //                     if (_attribute.args1) {
    //                         var _group_name = _attribute.args1.split('__')[0];
    //                         if (!_info.hasOwnProperty(_group_name)) {
    //                             _info[_group_name] = null;//directsun模式，删除了内容,防止提交后后端用默认值填充了
    //                         }
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }
    // console.log(JSON.stringify(_info));
    // console.log(_info);

    return true;
}

/**
 * 解析时间字符串
 * @param {*} dateStr 
 * @returns 
 */
function parseDate(dateStr) {
    // 如果已经是Date对象，直接返回
    if (dateStr instanceof Date) return dateStr;

    // 如果是数字时间戳
    if (typeof dateStr === 'number') return new Date(dateStr);

    // 如果是字符串
    if (typeof dateStr === 'string') {
        // 处理 yyyyMMddHHmmss 格式 (14位数字)
        if (/^\d{14}$/.test(dateStr)) {
            const year = parseInt(dateStr.substring(0, 4));
            const month = parseInt(dateStr.substring(4, 6)) - 1;
            const day = parseInt(dateStr.substring(6, 8));
            const hours = parseInt(dateStr.substring(8, 10));
            const minutes = parseInt(dateStr.substring(10, 12));
            const seconds = parseInt(dateStr.substring(12, 14));
            return new Date(year, month, day, hours, minutes, seconds);
        }

        // 处理 yyyyMMddHHmm 格式 (12位数字)
        if (/^\d{12}$/.test(dateStr)) {
            const year = parseInt(dateStr.substring(0, 4));
            const month = parseInt(dateStr.substring(4, 6)) - 1;
            const day = parseInt(dateStr.substring(6, 8));
            const hours = parseInt(dateStr.substring(8, 10));
            const minutes = parseInt(dateStr.substring(10, 12));
            return new Date(year, month, day, hours, minutes, 0);
        }

        // 处理 yyyyMMdd 格式 (8位数字)
        if (/^\d{8}$/.test(dateStr)) {
            const year = parseInt(dateStr.substring(0, 4));
            const month = parseInt(dateStr.substring(4, 6)) - 1;
            const day = parseInt(dateStr.substring(6, 8));
            return new Date(year, month, day);
        }

        // 处理 yyyyMM 格式 (6位数字)
        if (/^\d{6}$/.test(dateStr)) {
            const year = parseInt(dateStr.substring(0, 4));
            const month = parseInt(dateStr.substring(4, 6)) - 1;
            return new Date(year, month, 1);
        }

        // 处理常见分隔符格式
        if (/^\d{4}-\d{2}-\d{2}/.test(dateStr)) {
            return new Date(dateStr);
        }

        // 其他格式尝试用Date解析
        const date = new Date(dateStr);
        if (!isNaN(date.getTime())) {
            return date;
        }
    }

    // 如果都无法解析，返回当前时间或抛出错误
    console.warn(`无法解析日期格式: ${dateStr} 默认访问当前的时间！`);
    return new Date();
}

/**
 * 
 * @param {*} name 对比字段 示例age或者info__age 
 * @param {*} target_info 哪个对象
 * @param {*} target_name 给哪个字段赋值，实例name val
 * @param {*} target_value 给字段赋予哪个值
 */
function setValueToSameLevel(name, target_info, target_name, target_value) {
    if (name.indexOf("__") >= 0) {
        let _last = name.lastIndexOf("__");
        let _new_name = name.substring(0, _last) + '__' + target_name;
        target_info[_new_name] = target_value;
    } else {
        target_info[target_name] = target_value;
    }
}

/**
 * 支持参数化url 会从当前得properies中获取value进行宏替换 宏的写法{{:=item.xxx}}
 * TODO 这里有个问题，如果参数化的url中有多个参数，且有些参数没有值，这时候应该怎么处理？目前的做法是直接把这个参数去掉了，
 * 可能会导致接口访问错误，所以后续得优化成如果没有值就替换成空字符串，或者直接替换成null字符串，或者干脆不替换这个参数了，这个需要根据实际情况来定了！
 * 会组合当前url的query，以便给后续使用，如果不要当前的url，则使用handlerMicroString
 * @param {*} _url 
 * @returns 
 */
function handlerBuildDynamicUrl(_url) {

    var newUrl = handlerMicroString(_url);
    const urlParts = newUrl.split('?');
    let queryString = urlParts[1] || '';
    const queryParams = new URLSearchParams(queryString);
    let baseUrl = urlParts[0];//基础
    var query = location.search;
    if (!query || query.indexOf("?") == -1) {
        return newUrl;
    }

    //从当前的url中获取query参数，组合到新的url中
    if (query) {
        const currentQueryParams = new URLSearchParams(query);
        for (const [key, value] of currentQueryParams.entries()) {
            if (!queryParams.has(key)) {
                queryParams.append(key, value);
            }
        }
    }
    const newQueryString = queryParams.toString();
    _url = baseUrl + (newQueryString ? `?${newQueryString}` : '');
    return _url;

}

/**
 * 动态构建信息 
 * 后续用handlerMicroString更合理
 * 基于{{:=item.property}}的宏处理，替换为对应的属性值
 * @param {string} inputStr 示例：<a href="./page/?id={{:=item.id}}" title="{{:=item.name}}">{{:=item.id}}</a>
 * @returns {string} 替换后的字符串
 */
function handlerBuildDynamicData(inputStr) {
    return handlerMicroString(inputStr);
}

/**
 * 动态构建信息 使用global_data_properties 和 当前得Url 中的数据进行替换
 * 基于{{:=item.property}}的宏处理，替换为对应的属性值
 * @param {string} inputStr 示例：<a href="./page/?id={{:=item.id}}" title="{{:=item.name}}">{{:=item.id}}</a>
 * @param {Array} arrModel 数组处理方式，如果不填写使用ids=1&ids=2的模式，如果填写表示用1,2的模式
 * @returns {string} 替换后的字符串
 */
function handlerMicroString(inputStr, arrModel) {
    // 匹配所有{{:=item.property}}模式
    const regex = /{{:=item\.(\w+)}}/g;
    // 存储所有匹配到的变量
    const variables = [];
    let match;
    // 收集所有变量
    while ((match = regex.exec(inputStr)) !== null) {
        variables.push({
            fullMatch: match[0],      // 完整匹配，如：{{:=item.id}}
            propertyName: match[1],   // 属性名，如：id
            index: match.index        // 在字符串中的位置
        });
    }
    // 如果没有找到变量，直接返回原字符串
    if (variables.length === 0) {
        return inputStr;
    }
    // 创建一个副本用于替换
    let result = inputStr;
    // 使用倒序替换，避免索引位置变化问题
    variables.sort((a, b) => b.index - a.index);
    //直接从当前表单读取数据
    var formData = null;//可能当前不是表单页面
    if (typeof readFormDataSkipValidity === 'function') {
        formData = readFormDataSkipValidity();
    }
    //当前url中的query
    let mapVals = handlerReadQueryFromSearch(location.search);
    for (const variable of variables) {
        let found = false;
        let curVal = null;

        // 1. 表单数据
        if (formData && formData.hasOwnProperty(variable.propertyName)) {
            const val = formData[variable.propertyName];
            if (val !== undefined && val !== null) {
                curVal = val;
                found = true;
            }
        }

        // 2. 元数据
        if (!found && _global_data_properties) {
            const property = funcFindPropertry(variable.propertyName);
            if (property && property.value !== undefined && property.value !== null) {
                curVal = property.value;
                found = true;
            }
        }

        // 3. URL参数
        if (!found && mapVals.has(variable.propertyName)) {
            const val = mapVals.get(variable.propertyName);
            if (val !== undefined && val !== null) {
                curVal = val;
                found = true;
            }
        }

        // 4. 替换或删除
        if (found) {
            // 有值：正常替换
            result = privaiteRegexReplace(curVal, arrModel, variable, result);
        } else {
            // 没值：删除整个参数
            result = privateRemoveParamFromUrl(result, variable);
        }
    }
    return result;
}

/**
 * 从URL中删除包含占位符的参数
 * @param {string} url 完整的URL字符串
 * @param {object} variable 变量对象 { fullMatch: '{{:=item.id}}', propertyName: 'id' }
 * @returns {string} 删除参数后的URL
 */
function privateRemoveParamFromUrl(url, variable) {
    // 转义特殊字符
    const escapedMatch = variable.fullMatch.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');

    // 匹配几种情况：
    // 1. &id={{:=item.id}}  → 删除 &id=xxx
    // 2. ?id={{:=item.id}}  → 删除 ?id=xxx 或 ?id=xxx&  → 变成 ? 或 空
    // 3. id={{:=item.id}}   → 删除 id=xxx

    // 情况1: 删除 &参数名=占位符 (中间可能有其他参数)
    let result = url.replace(new RegExp(`&${variable.propertyName}=${escapedMatch}`, 'g'), '');

    // 情况2: 删除 ?参数名=占位符 (开头)
    // 如果是 ?id=xxx  → 删除后变成 ''
    // 如果是 ?id=xxx&other=yyy → 删除后变成 ?other=yyy
    result = result.replace(new RegExp(`\\?${variable.propertyName}=${escapedMatch}(&)?`, 'g'), (match, amp) => {
        // 如果有 &，说明后面还有参数，保留 ?
        return amp ? '?' : '';
    });

    // 情况3: 如果占位符不在参数中（比如在path中），直接替换为空
    result = result.replace(new RegExp(escapedMatch, 'g'), '');

    // 清理多余的 ? 和 &
    // 如果 URL 以 ?& 开头，修复为 ?
    result = result.replace(/^\?&/, '?');
    // 如果 URL 以 & 开头，修复为 ?
    result = result.replace(/^&/, '?');
    // 如果 URL 以 ?$ 结尾（只有?没有参数），去掉 ?
    result = result.replace(/\?$/, '');
    // 删除多余的 & 在 ? 后面
    result = result.replace(/\?&/, '?');

    return result;
}

/**
 * 基于正则表达式的替换方法，处理数组和单值的情况
 * @param {*} curVal 
 * @param {*} arrModel 
 * @param {*} variable 
 * @param {*} result 
 * @returns 
 */
function privaiteRegexReplace(curVal, arrModel, variable, result) {
    if (Array.isArray(curVal)) {
        if (arrModel && arrModel.length > 0) {
            curVal = curVal.join(arrModel);
        } else {
            // ids=[1,2,3] → "1&ids=2&ids=3"
            curVal = curVal.map((v, index) => {
                return index === 0 ? v : `${variable.propertyName}=${v}`;
            }).join('&');
        }
    }
    const regexEscaped = variable.fullMatch.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    return result.replace(new RegExp(regexEscaped, 'g'), curVal);
}

/**
 * 读取location.search中的query参数，支持重复参数，并将其存储在一个Map中
 * @param {*} search 
 */
function handlerReadQueryFromSearch(search) {
    // 当前url中的query - 支持重复参数
    let pars = new URLSearchParams(search);
    let mapVals = new Map();
    for (const [key, value] of pars.entries()) {
        // 如果已经存在，改为存储数组
        if (mapVals.has(key)) {
            const existing = mapVals.get(key);
            if (Array.isArray(existing)) {
                existing.push(value);
            } else {
                mapVals.set(key, [existing, value]);
            }
        } else {
            mapVals.set(key, value);
        }
    }
    return mapVals;
}

/**
 * 为输入框添加对长度显示的支持
 */
function funcAppendInputLength() {
    var inputarr = $(".newform").find("input[type=text],textarea,input[type=password]")
    for (let i = 0; i < inputarr.length; i++) {
        if ($(inputarr[i]).attr('maxlength')) {
            if ($(inputarr[i]).css("display") == 'none') {
                continue;
            }
            $(inputarr[i]).parent().css('position', 'relative')
            var str = `<span class="spanlen">${$(inputarr[i]).val().length}/${$(inputarr[i]).attr('maxlength')}</span>`
            $(inputarr[i]).after(str)
            $(inputarr[i]).on("input", function () {
                var numstr = $(this).val().length + '/' + $(this).attr('maxlength')
                $(this).siblings('.spanlen').text(numstr)
            })
        }
    }
}

/**
 * 基于递归方法，把一级数据拆分为多级,name中包含__的分拆一级，如果是__num__的分拆数组
 * @param {*} flattenedData 
 * @returns 
 */
function convertFlattenedToNested(flattenedData) {
    // 判断是否是数组格式（键名中包含数字索引 xx__xx ）
    const isArrayFormat = Object.keys(flattenedData).some(key => /^\d+__/.test(key));
    if (isArrayFormat) {
        // 如果是数组格式，初始化结果为数组
        const result = [];
        for (const key in flattenedData) {
            if (flattenedData.hasOwnProperty(key)) {
                const value = flattenedData[key];
                const parts = key.split('__');
                // 递归构建嵌套结构
                buildNestedStructure(result, parts, value);
            }
        }
        //   const _result = result.filter((obj)=>{
        //     return Object.values(obj).length>0;
        //   });
        //   return result;
        return removeEmptyValues(result);
    } else {
        // 否则，初始化结果为对象
        const result = {};
        for (const key in flattenedData) {
            if (flattenedData.hasOwnProperty(key)) {
                const value = flattenedData[key];
                const parts = key.split('__');
                // 递归构建嵌套结构
                buildNestedStructure(result, parts, value);
            }
        }
        //   return result;
        return removeEmptyValues(result);
    }
}

/**
 * 递归去除空值 为撒要去除空值呢？
 * 因为如果有空值提交到后端，
 * 可能会导致后端用默认值填充了，
 * 所以这里直接去掉了，
 * 当然也可以替换成null字符串或者空字符串，
 * 这个需要根据实际情况来定了！ 
 * 目前的做法是直接去掉了，
 * 可能会导致接口访问错误，
 * 所以后续得优化成如果没有值就替换成空字符串，
 * 或者直接替换成null字符串，
 * 或者干脆不替换这个参数了，
 * 这个需要根据实际情况来定了！
 * @param {*} obj 
 * @returns 
 */
function removeEmptyValues(obj) {
    if (_config.remove_empty) {
        if (Array.isArray(obj)) {
            // 如果是数组，递归处理每个元素
            return obj
                .map(item => removeEmptyValues(item)) // 递归处理每个元素
                .filter(item => item !== null && item !== undefined && item !== ''); // 过滤掉空值
        } else if (typeof obj === 'object' && obj !== null) {
            // 如果是对象，递归处理每个属性
            const result = {};
            for (const key in obj) {
                if (obj.hasOwnProperty(key)) {
                    const value = removeEmptyValues(obj[key]); // 递归处理属性值
                    // 如果值不是空值，则保留该属性
                    if (value !== undefined && value !== '') {
                        // if (value !== null && value !== undefined && value !== '') {
                        result[key] = value;
                    }
                }
            }
            // 如果对象没有任何属性，返回 undefined（表示移除该对象）
            return Object.keys(result).length > 0 ? result : undefined;
        } else {
            // 如果是基本类型，直接返回
            return obj;
        }
    } else {
        if (Array.isArray(obj)) {
            // 如果是数组，递归处理每个元素
            return obj
                .map(item => removeEmptyValues(item)) // 递归处理每个元素
                .filter(item => item !== null && item !== undefined); // 过滤掉空值
            // .filter(item => item !== null && item !== undefined && item !== ''); // 过滤掉空值
        } else if (typeof obj === 'object' && obj !== null) {
            // 如果是对象，递归处理每个属性
            const result = {};
            for (const key in obj) {
                if (obj.hasOwnProperty(key)) {
                    const value = removeEmptyValues(obj[key]); // 递归处理属性值
                    // 如果值不是空值，则保留该属性
                    if (value !== undefined) {
                        result[key] = value;
                    }
                    //if (value !== null && value !== undefined && value !== '') {
                }
            }
            // 如果对象没有任何属性，返回 undefined（表示移除该对象）
            return Object.keys(result).length > 0 ? result : undefined;
        } else {
            // 如果是基本类型，直接返回
            return obj;
        }
    }
}

/**
 * 递归赋值
 * @param {*} current 
 * @param {*} parts 
 * @param {*} value 
 * @returns 
 */
function buildNestedStructure(current, parts, value) {
    if (current) {
        const part = parts[0]; // 当前处理的键部分

        // 如果是最后一个部分，直接赋值
        if (parts.length === 1) {
            current[part] = value;
            return;
        }

        // 判断下一级是否是数组
        if (!isNaN(parts[1])) {
            const index = parseInt(parts[1]);

            // 如果当前部分不存在，创建一个新的数组
            if (!current[part]) {
                current[part] = [];
            }

            // 获取或创建数组元素
            while (current[part].length <= index) {
                current[part].push({});
            }

            // 递归处理剩余部分
            buildNestedStructure(current[part][index], parts.slice(2), value);
        } else {
            // 如果当前部分不存在，创建一个新的对象
            if (!current[part]) {
                current[part] = {};
            }

            // 递归处理剩余部分
            buildNestedStructure(current[part], parts.slice(1), value);
        }
    }
}


/**
 * 删除objects的某一项
 * @param {*} elc 
 */
function handlerRemoveObjectItem(elc) {
    var _li = $(elc).parents("li");
    var _name = _li.attr('dataname');
    var _id = _li.attr('dataid');
    var objs = _map_values.get(_name);
    if (objs) {
        var _porperty = funcFindPropertry(_name);
        if (_porperty) {
            for (var i = 0; i < objs.length; i++) {
                if (objs[i]["_id"].toString() == _id) {
                    objs.splice(i, 1);
                    break;
                }
            }
        }
    }
    _li.remove();
    return false;
}

/**
 * 使用outer选择外表的ID，和显示对应的友好名称 navigater的升级版
 * 这个是给Form使用的
 * @param {*} elc 
 */
function handler_outer_value(elc) {
    var _name = $(elc).attr('dataname');
    if (_global_data_properties) {
        var find = funcFindPropertry(_name);
        if (find) {
            if (find.dataType == "outer") {
                var _attribute = funcFindAttribute(find.attributes, 'outer');
                if (_attribute) {
                    // if (typeof funcLinkOuterOpenSelect == "function") {
                    funcLinkOuterOpenSelect(`选择${find.title}`, _attribute.args1, '1', (obj) => {
                        if (obj) {
                            $(elc).val(handlerColumnValue(obj, _attribute.args4));
                            //用parent是因为可能是在某一个gourp内部，这样不会影响其他的
                            // $(elc).parent().find(`[name=${_name}]`).val(obj[_attribute.args3]);
                            // //args3可能是多个... .. .
                            // find.value = obj[_attribute.args3];

                            if (_attribute.args3.indexOf(",") > 0) {
                                //多个模式
                                var ids = _attribute.args3.split(",");
                                let zindex = 0;
                                for (let icol of ids) {
                                    zindex++;
                                    if (zindex == 1) {
                                        $(elc).parent().find(`[name=${_name}]`).val(obj[icol]);
                                        find.value = obj[icol];
                                    } else {
                                        $(elc).parent().find(`[name=${icol}]`).val(obj[icol]);
                                    }
                                }
                            } else {
                                //单个模式
                                $(elc).parent().find(`[name=${_name}]`).val(obj[_attribute.args3]);
                                find.value = obj[_attribute.args3];
                            }
                            //扩展到加载对象中，比如extendUser _attribute.args2
                            if (_attribute.args2) {
                                _global_expend_object[_attribute.args2] = obj;
                            }
                            handlerLoaditem(find);
                            if (_config.search_after_selected) {
                                if (typeof readPageData == 'function') {
                                    readPageData(1);
                                }
                            }
                        }
                    });
                }
            }
            if (find.dataType == "outers") {
                var _attribute = funcFindAttribute(find.attributes, 'outers');
                if (_attribute) {
                    funcLinkOuterOpenSelect(`选择${find.title}`, _attribute.args1, '100', (objs) => {
                        handler_display_outers_toui(objs, _attribute, elc);
                        handlerLoaditem(find);
                    });
                }
            }
        }
    }
}

/**
 * 给table的edit模式的outer使用
 * @param {*} elc 
 */
function handler_edit_outer(elc) {
    var _name = $(elc).attr('dataname');
    if (_global_list_properties) {
        var find = funcQueryPropertry(_name);
        if (find) {
            if (find.dataType == "outer") {
                var _attribute = funcFindAttribute(find.attributes, 'outer');
                if (_attribute) {
                    funcLinkOuterOpenSelect(`选择${find.title}`, _attribute.args1, '1', (obj) => {
                        if (obj) {
                            $(elc).val(handlerColumnValue(obj, _attribute.args4));
                            $(elc).parent().find(`[name=${_name}]`).val(obj[_attribute.args3]);
                            find.value = obj[_attribute.args3];
                            //扩展到加载对象中，比如extendUser _attribute.args2
                            if (_attribute.args2) {
                                _global_expend_object[_attribute.args2] = obj;
                            }
                        }
                    });
                }
            }
        }
    }
}


/**
 * 打开选择一个外表的值
 */
function funcLinkOuterOpenSelect(_title, _className, num, callback) {
    var _url = _className;
    if (_url.indexOf('/') == -1) {
        _url = `${handlerPasteFormPath()}index.html?model=select&path=${_className}`;
    }
    _url = handlerBuildDynamicUrl(_url);
    open_select(_title, _url + '&select_num=' + num, "", "", callback);
}

/**
 * 显示外选择的内容到UI
 * @param {*} objs 
 * @param {*} _attribute 
 * @param {*} elc 
 */
function handler_display_outers_toui(objs, _attribute, elc) {
    if (!Array.isArray(objs)) {
        objs = [objs];
    } else {
        // $(elc).parents('.outers').find('.ulouter').empty();//如果数据要从多个页中获取，不能用这个
    }
    for (var obj of objs) {
        if (obj) {
            var _name = handlerColumnValue(obj, _attribute.args4);// (obj[_attribute.args4]);
            var _id = (obj[_attribute.args3]);
            //判断当前列表是否存在，不存在则新增
            var par = $(elc).parents('.outers').find(`[dataid="${_id}"]`);
            if (par.length == 0) {
                var _ul = $(elc).parents('.outers').find('.ulouter');
                if (_ul) {
                    _ul.append(`<li dataid="${_id}">${_name}<span class="outer_close" onclick="$(this).parents('li').remove();">x</span></li>`);
                }
            }
        }
    }

    if (_attribute.args2) {
        _global_expend_object[_attribute.args2] = objs;
    }

}

/**
 * 使用object的模式，进行外部表单的创建，一般用于创建的时候
 * 这里要做页面的map处理，回传的时候基于对象的类型是object格式 还是string格式
 * @param {*} elc 
 */
function handler_object_value(elc) {
    var _name = $(elc).attr('dataname');
    if (_global_data_properties) {
        var find = funcFindPropertry(_name);
        if (find) {
            var _attribute = funcFindAttribute(find.attributes, 'object');
            if (_attribute) {
                funcObjectOpenSelect(`选择${find.title}`, _attribute.args1, find.session_key || '', (obj) => {
                    if (obj) {
                        $(elc).val(handlerColumnValue(obj, _attribute.args4));
                        $(elc).parent().find(`[name=${_name}]`).val(obj[_attribute.args3]);
                        _map_values.set(_name, obj);
                    }
                });
            } else {
                _attribute = funcFindAttribute(find.attributes, 'objects');
                if (_attribute) {
                    funcObjectOpenSelect(`选择${find.title}`, _attribute.args1, find.session_key || '', (obj) => {
                        if (Array.isArray(obj)) {
                            // _map_values.delete(_name);
                            for (var _obj of obj) {
                                displayObjectsToUI(_obj, _name, find);
                            }
                        } else {
                            displayObjectsToUI(obj, _name, find);
                        }
                    });
                }
            }
        }
    }
}

/**
 * 显示Objects的数据到UI
 * @param {*} obj 
 * @param {*} _name 
 * @param {*} find 
 */
function displayObjectsToUI(obj, _name, find) {
    if (obj) {
        var objs = _map_values.get(_name);
        if (!objs) {
            objs = new Array();
        }
        var _find_index = -1;
        var _find_id = handlerColumnValue(obj, find.display_id);
        for (var k = 0; k < objs.length; k++) {
            if (_find_id == objs[k]._id) {
                _find_index = 0;
                break;
            }
        }
        if (_find_index == -1) {
            objs.push(obj);
        } else {
            objs[_find_index] = obj;
        }

        var _ihtml = '';
        objs.forEach(oitem => {
            var _display = handlerColumnValue(oitem, find.display_name);
            var _id = handlerColumnValue(oitem, find.display_id);
            oitem._display = _display;
            oitem._id = _id;
            _ihtml += `<li dataid="${_id}" dataname="${_name}" onclick="handlerEditObject(this);">${_display}<span class="outer_close" onclick="handlerRemoveObjectItem(this);">x</span></li>`;
        });
        $(`.outers${_name}`).html(_ihtml);
        _map_values.set(_name, objs);
    }
}

// /**
//  * 读取linkquery的参数以&xxx=yyy&zzz=www的形式
//  * @returns 
//  */
// function handlerBuildLinkQueryUrl() {
//     var _linkquery = "";
//     if (_global_class_attributes) {
//         _global_class_attributes.forEach(_attribute => {
//             if (_attribute.name == "linkquery") {
//                 if (_attribute.args1) {
//                     var _args = _attribute.args1.split(',');
//                     _args.forEach(arg => {
//                         var _argval = readQueryFromSearch(arg);
//                         if (_argval != null) {
//                             _linkquery += `&${arg}=${_argval}`;
//                         }
//                     });
//                 }
//             }
//         });
//     }
//     return _linkquery;
// }

// /**
//  * 从当前url中获取对应的参数
//  */
// function readQueryFromSearch(key) {
//     // 获取参数
//     var url = window.location.search;
//     // 正则筛选地址栏
//     var reg = new RegExp("(^|&)" + key + "=([^&]*)(&|$)");
//     // 匹配目标参数
//     var result = url.substr(1).match(reg);
//     //返回参数值
//     return result ? decodeURIComponent(result[2]) : null;
// }

/**
 * 读取linkquery的参数以&xxx=yyy&zzz=www的形式
 * 25.04.13 适配array模式 注意Object的要自己实现，建议使用sessionStorage的方式
 * @returns {string} 构建好的linkquery字符串
 */
function handlerBuildLinkQueryUrl() {
    let linkquery = [];
    if (Array.isArray(_global_class_attributes)) {
        for (const attribute of _global_class_attributes) {
            if (attribute.name === "linkquery" && attribute.args1) {
                // 如果是 "*"，获取所有URL参数
                if (attribute.args1 === "*") {
                    const allParams = getAllUrlParams();
                    for (const [key, values] of Object.entries(allParams)) {
                        for (const value of values) {
                            linkquery.push(`${key}=${value}`);
                        }
                    }
                } else {
                    // 原有的逻辑：只获取指定的参数
                    const args = attribute.args1.split(',');
                    for (const arg of args) {
                        const argvals = readQueryFromSearch(arg);
                        if (argvals.length > 0) {
                            for (const argval of argvals) {
                                linkquery.push(`${arg}=${argval}`);
                            }
                        }
                    }
                }
            }
        }
    }
    return linkquery.length > 0 ? '&' + linkquery.join('&') : '';
}

/**
 * 获取URL中的所有参数
 * @returns {Object} 包含所有参数及其值的对象，格式：{key: [value1, value2]}
 */
function getAllUrlParams() {
    const url = window.location.search;
    const params = {};

    if (url.length <= 1) {
        return params;
    }

    const queryString = url.substring(1); // 去掉 '?'
    const pairs = queryString.split('&');

    for (const pair of pairs) {
        const [key, value] = pair.split('=');
        if (key) {
            const decodedKey = decodeURIComponent(key);
            const decodedValue = value ? decodeURIComponent(value) : '';

            if (!params[decodedKey]) {
                params[decodedKey] = [];
            }
            params[decodedKey].push(decodedValue);
        }
    }

    return params;
}

/**
 * 从当前url中获取对应的参数
 * 25.04.13 适配array模式 注意Object的要自己实现，建议使用sessionStorage的方式
 * @param {string} key - 要获取的参数名
 * @param {string} [urlStr=window.location.search] - 可选的URL字符串，默认为当前页面的查询字符串
 * @returns {Array<string>} 参数值数组，如果未找到则返回空数组
 */
function readQueryFromSearch(key, urlStr) {
    const url = urlStr || window.location.search;
    const reg = new RegExp(`[?&]${key}=([^&]*)`, 'g');
    const results = [];
    let match;
    while ((match = reg.exec(url)) !== null) {
        results.push(decodeURIComponent(match[1]));
    }
    return results;
}

/**
 * 从字符串或者当前地址中获取某一个参数，会返回第一个，如果是需要返回数组用另外一个函数readQueryFromSearch
 * @param {*} key 
 * @param {*} urlStr 
 * @returns 
 */
function readFirstQueryFromSearch(key, urlStr) {
    const url = urlStr || window.location.search;
    const reg = new RegExp(`[?&]${key}=([^&]*)`, 'g');
    const results = [];
    let match;
    while ((match = reg.exec(url)) !== null) {
        results.push(decodeURIComponent(match[1]));
    }
    return results.length == 0 ? null : results[0];
}



/**
 * 基于linkquery的模式，自动整理query
 * @param {*} url 
 * @returns 
 */
function handlerAutoQueryIncludeLinkQuery(url) {
    var _link = {};
    if (_global_class_attributes) {
        _global_class_attributes.forEach(_attribute => {
            if (_attribute.name == "linkquery") {
                if (_attribute.args1) {
                    //var _args = _attribute.args1.split(',');
                    //_args.forEach(arg => {
                    //    var _argval = _apigetquery(arg);
                    //    if (_argval != null) {
                    //        // _linkquery += `&${arg}=${_argval}`;
                    //        _link[arg] = _argval;
                    //    }
                    //});
                    if (_attribute.args1 === "*") {
                        const allParams = getAllUrlParams();
                        for (const [key, values] of Object.entries(allParams)) {
                            for (const value of values) {
                                if (value) {
                                    _link[key] = value;
                                }
                            }
                        }
                    } else {
                        var _args = _attribute.args1.split(',');
                        _args.forEach(arg => {
                            var _argval = _apigetquery(arg);
                            if (_argval != null) {
                                _link[arg] = _argval;
                            }
                        });
                    }
                }
            }
        });
    }
    return appendQueryParams(url, _link);
}

/**
 * 解析 URL 并追加参数（如果参数不存在）
 * @param {string} url - 原始 URL
 * @param {Object} queryMap - 包含需要追加的 query 键值对的对象
 * @returns {string} - 处理后的 URL
 */
function appendQueryParams(url, queryMap) {
    if (typeof url !== 'string' || !url) {
        return url;
    }

    if (!queryMap || typeof queryMap !== 'object' || Array.isArray(queryMap)) {
        return url;
    }

    // 解析 URL 的路径、参数和哈希部分
    const [baseUrl, hash] = url.split('#');
    const [path, queryString] = baseUrl.split('?');

    // 解析现有的查询参数
    const queryParams = new URLSearchParams(queryString || '');

    // 遍历 queryMap，如果参数不存在则追加
    for (const [key, value] of Object.entries(queryMap)) {
        if (value !== undefined && value !== null && !queryParams.has(key)) {
            queryParams.set(key, value);
        }
    }

    // 拼接处理后的 URL
    const newQueryString = queryParams.toString();
    const newUrl = newQueryString ? `${path}?${newQueryString}` : path;

    // 如果有哈希部分，重新追加
    return hash ? `${newUrl}#${hash}` : newUrl;
}


/**
 * 基于表达式获取对象数据，返回字符串的，用于objects的+字段
 * @param {*} find 
 * @param {*} columnstr 
 * @returns 
 */
function handlerColumnValue(find, columnstr) {
    if (columnstr.indexOf("+") > 0) {
        var _strs = columnstr.split('+');
        var _aval = '';
        _strs.forEach(ii => {
            if (find.hasOwnProperty(ii)) {
                _aval += find[ii];
            } else {
                _aval += ii;
            }
        });
        return _aval;
    }
    return find[columnstr];
}

/**
 * 编辑objects的单个内容
 * @param {*} elc 
 */
function handlerEditObject(elc) {
    var _li = $(elc);
    var _name = _li.attr('dataname');
    var _id = _li.attr('dataid');
    var objs = _map_values.get(_name);
    if (objs) {
        var _porperty = funcFindPropertry(_name);
        if (_porperty) {
            var _attribute = funcFindAttribute(_porperty.attributes, "objects");
            if (_attribute) {
                var _current = null;
                for (var i = 0; i < objs.length; i++) {
                    if (objs[i]["_id"].toString() == _id) {
                        _current = objs[i];
                        break;
                    }
                }
                let session_key = `${_porperty.name}_000`;
                if (_current) {
                    //查看object
                    sessionStorage.setItem(session_key, JSON.stringify(_current));
                    funcObjectOpenSelect(`编辑${_current[_porperty.display_name]}`, _attribute.args1, session_key || '', (obj) => {
                        displayObjectsToUI(obj, _name, _porperty);
                    });
                } else {
                    sessionStorage.removeItem(session_key);
                }
            }
        }
    }
}

/**
 * 加载来源参数内容 在form渲染前，所以影响的是Property的信息
 * v25.04.13
 * @param {*} properties 
 */
function LoadQuery(properties) {
    properties.forEach(item => {
        if (item.attributes) {
            item.attributes.forEach(_attribute => {
                if (_attribute.name == 'query') {
                    if (_attribute.args1) {
                        let queryValArray = readQueryFromSearch(_attribute.args1);
                        if (queryValArray.length > 0) {
                            //这里还要处理，比如splitarray的特性等... 
                            switch (item.dataType) {
                                case "Int32": case "UInt32": case "Int64": case "UInt64":
                                    {
                                        item.value = Number(queryValArray[0]);
                                    }
                                    break;
                                case "Int32[]": case "UInt32[]": case "Int64[]": case "UInt64[]":
                                    {
                                        let _varray = new Array();
                                        for (let a of queryValArray) {
                                            _varray.push(Number(a));
                                        }
                                        item.value = _varray;
                                    }
                                    break;
                                default:
                                    {
                                        if (item.dataType.indexOf("[]") >= 0) {
                                            item.value = queryValArray;
                                        } else {
                                            item.value = queryValArray[0];
                                        }
                                    }
                                    break;
                            }
                            //如果事objects 我只是想一会提交的时候一并提交呢???
                            if (_attribute.args2) {
                                item.hidden = true;
                            }
                        }
                    }
                }
            });
        }
    });
}

/**
 * 内容嵌入后，执行一些加载绑定等！ 
 */
function FormLoaded(properties) {
    properties.forEach(item => {
        if (item.dataType == "daterange") {
            if (item.name) {
                HandlerInitDateRange(item.name, item.value);
            }
        }
        if (item.dataType == "richtext") {
            InitRichEditor(item);//可以有多个富文本 {{name}}editortoolbar {{name}}editorbody window.{{name}}editor
        }
        if (item.dataType == "markdown") {
            InitMarkDownEditor(item);
        }
        if (item.dataType == "score") {
            $(`.raty${item.name}`).ratyli({
                step: false,
                onRated: function (score) {
                    item.value = score;
                }
            });
        }
        // if (item.attributes) {
        //     item.attributes.forEach(_attribute => {
        //         if (_attribute.name == 'query') {
        //             if (_attribute.args1) {
        //                 var _query_val = _apigetquery(_attribute.args1);
        //                 if (_query_val != null && _query_val != undefined) {
        //                     $(`[name=${item.name}]`).val(_query_val);
        //                     if (_attribute.args2) {
        //                         $(`[name=${item.name}]`).parents(".row").hide();
        //                     }
        //                 }
        //             }
        //         }
        //     });
        // }
    });
}

/**
 * 点击删除图片
 * @param {*} elc 
 */
function handlerTapImage(elc) {
    if (!$(elc).hasClass("active")) {
        $(elc).addClass("active").siblings().removeClass("active");
    }
}


/**
 * 更新封面的图片的选择项
 */
function funcUpdateImages() {
    if ($(".coverts").length > 0) {
        let imgs = new Array();
        $('.paste-form-body img').each(function () {
            var src = $(this).attr('src');
            if (src != 'undefined' && src != undefined && src != null && src != '') {
                if (imgs.indexOf(src) == -1) {
                    imgs.push(src);
                }
            }
        });
        var _html = '';
        var _zindex = 0;
        for (let src of imgs) {
            if (_zindex == 0) {
                _html += `<img class="cover_img active" onerror="this.onerror=null; this.src='./lib/default.png';" onclick="handlerTapImage(this);" src="${src}">`;
            } else {
                _html += `<img class="cover_img" onerror="this.onerror=null; this.src='./lib/default.png';" onclick="handlerTapImage(this);" src="${src}">`;
            }
            _zindex++;
        }
        $(".coverts").html(_html);
    }
}

/**
 * 初始化Markdown编辑器
 * @param {*} item 
 */
function InitMarkDownEditor(item) {
    var _editor = `${item.name}mdeditor`;
    var _high = 600;
    var _upload_url = `/${readRootPath()}/Upload/Markdown`;
    var _attribute = funcFindAttribute(item.attributes, "markdown");
    if (_attribute) {
        if (_attribute.args1) {
            var _h = Number(_attribute.args1);
            if (_h) {
                _high = _h;
            }
        }
        //2给另外一个值了 比如richtext回传txt markdown回传 html
        if (_attribute.args3) {
            _upload_url = handlerBuildDynamicUrl(_attribute.args3);
        }
    }

    BtnExpend = editormd.BtnMenu;

    _mdEditor = editormd(_editor, {
        height: _high,
        saveHTMLToTextarea: true,
        imageUpload: true,
        imageFormats: ["jpeg", "jpg", "gif", "png", "bmp", "webp"],
        imageUploadURL: _upload_url,
        imageUploadHeaders: { token: readToken() },
        markdown: item.value || '',
        path: '../lib/editor.md/lib/',
        // 自定义工具栏图标
        toolbarIcons: function () {
            // 添加自定义图标到默认工具栏
            return editormd.toolbarModes.full.concat(["insertTime"]);
        },

        // 自定义图标样式（使用 Font Awesome）
        toolbarIconsClass: {
            insertTime: "fa-clock-o" // 时钟图标
        },

        // 自定义图标点击事件
        toolbarHandlers: {
            insertTime: function (cm, icon, cursor, selection) {
                // 获取当前时间
                const now = new Date();
                const timeString = now.toLocaleTimeString();

                // 插入时间到编辑器
                // this.insertText(`当前时间: ${timeString}`);

                // 在光标位置插入文本，选中文本会被替换
                // cm.replaceSelection("插入的文本");

                // 示例：插入时间戳
                const time = new Date().toLocaleTimeString();
                cm.replaceSelection(`⏰ ${time}`);

                // global_load_netword_images

                // 可添加其他逻辑，如显示提示
                console.log("插入时间:", timeString);
            }
        }
    });

    if (typeof initPasteDragImg == 'function') {
        initPasteDragImg(_mdEditor);
    }
    item.editor = _mdEditor;
}

/**
 * 获取根路径 示例api/app
 * @returns 
 */
function readRootPath() {
    var root_path = "api/app";
    if (_config) {
        if (_config.root_path) {
            root_path = _config.root_path;
        }
    }
    return root_path;
}

/**
 * 初始化富文本 注意看命名规则 ui的ID的规则，要相互，支持单个页面多个富文本
 * @param {*} item 
 */
function InitRichEditor(item) {
    //注意这里的命名规则需要和ui一致
    var _toolbar = `${item.name}editorbar`;
    var _editor = `${item.name}editor`;

    var _high = 600;
    var _upload_url = `/${readRootPath()}/Upload/UpImage`;
    var _attribute = funcFindAttribute(item.attributes, "richtext");
    if (_attribute) {
        if (_attribute.args1) {
            var _h = Number(_attribute.args1);
            if (_h) {
                _high = _h;
            }
        }
        //2给另外一个值了 比如richtext回传txt markdown回传 html
        if (_attribute.args3) {
            _upload_url = handlerBuildDynamicUrl(_attribute.args3);
        }
    }

    const editor = new E(`#${_toolbar}`, `#${_editor}`) // 传入两个元素
    // 默认情况下，显示所有菜单
    editor.config.menus = [
        'head',
        'bold',
        'fontSize',
        // 'fontName',
        'italic',
        'underline',
        // 'strikeThrough',
        'indent',
        'lineHeight',
        'foreColor',
        // 'backColor',
        'link',
        // 'list',
        'todo',
        'justify',
        'quote',
        // 'emoticon',
        'image',
        // 'video',
        'table',
        'code',
        'splitLine',
        'undo',
        'redo'
    ]
    editor.config.uploadImgServer = _upload_url
    editor.config.uploadImgMaxSize = 2 * 1024 * 1024 // 2M
    editor.config.uploadImgAccept = ['jpg', 'jpeg', 'png', 'gif', 'webp']
    editor.config.uploadImgMaxLength = 3 // 一次最多上传 5 个图片
    editor.config.uploadImgHeaders = { token: readToken() }//注意这里要按照实际情况处理 这里估计要搞成配置的

    editor.config.customUploadImg = (files, insert) => {
        const formData = new FormData();
        for (let i = 0; i < files.length; i++) {
            formData.append('file', files[i]);
        }
        fetch(`${auto_root_path()}/Upload/UpImage?type=rich&size=1920*0`, {
            method: 'POST',
            headers: {
                'token': readToken()
            },
            body: formData
        })
            .then(response => {
                console.log(response);
                return response.json();
            })
            .then(result => {
                console.log(result);
                if (result.errno == 0) {
                    var _url = result.data[0].url;
                    // 插入图片到编辑器
                    insert(_url);
                    // 可以在这里添加其他自定义逻辑
                    if (_imageList.indexOf(_url) == -1) {
                        _imageList.push(_url);
                        //如果有coverts，则执行渲染操作
                    }
                    // 例如，记录上传日志
                    console.log('图片上传成功:', _url);
                } else {
                    console.error('图片上传失败:', result);
                }
            })
            .catch(error => {
                console.error('图片上传出错:', error);
            });
    };


    if (item.placeholder) {
        editor.config.placeholder = item.placeholder;
    }
    editor.create();
    if (item.value) {
        editor.txt.html(item.value);
    }
    item.editor = editor;
}
/**
 * 点击删除某一个图片
 * @param {*} elc 
 */
function handlerRemoveImageItem(event, elc) {

    event.preventDefault();  // 阻止默认行为（如 label 触发）
    event.stopPropagation(); // 阻止事件冒泡
    event.stopImmediatePropagation(); // 阻止同一元素上的其他监听器

    var _elcControlls = $(elc).parents(".formControls");
    //单图模式
    if ($(elc).attr('data-num')) {
        $(_elcControlls).find('img').attr('src', '');
        $(elc).hide();
        $(_elcControlls).find('.icon-add').show();
        $(_elcControlls).find('input[type=text]').val('');
        // event.stopImmediatePropagation();
        console.log('执行事件阻断... .. .');
        return;
    }
    //多图模式

    var _name = $(_elcControlls).find("input[type=text]").attr('name');
    $(elc).parents("li").remove();
    handlerReadImages(_name);
    //判断数量，是否显示+按钮
    var _find = funcFindPropertry(_name);
    if (_find != null) {
        if (_find.num > 1) {
            var _imageul = $(_elcControlls).find(".imageul");
            if ($(_imageul).find("li").length > _find.num) {
                $(_imageul).find("li").eq(0).hide();
            } else {
                $(_imageul).find("li").eq(0).show();
            }
        }
    }
    // event.stopImmediatePropagation();
    return false;
}

/**
 * 读取多图模式的值 并给对应的输入框赋值，为了给后面的form读取值
 * @param {*} _name 
 */
function handlerReadImages(_name) {
    var _array = new Array();
    var pars = $(`[name=${_name}]`).parents(".formControls").find("img");
    for (var k = 0; k < pars.length; k++) {
        var _img = pars[k];
        var _img_src = ($(_img).attr('src'));
        _array.push(_img_src);
    }
    if (_array.length == 0) {
        $(`[name=${_name}]`).val('');
        return '';
    } else {
        $(`[name=${_name}]`).val(_array.join(','));
        return _array.join(',');
    }
    //这里可以做数量限制... .. .
}

/**
 * 上传非图片 比如文件等
 */
function handlerUploadOnlyFile(elc) {
    var _url = `${auto_root_path()}/Upload/UpFile`;
    var _dataurl = $(elc).attr('dataurl');
    if (_dataurl) {
        _url = _dataurl;
    }
    var _type = $(elc).attr('datatype');
    if (_type) {
        _url = `${_url}${(_url.indexOf("?") >= 0 ? '&' : '?')}type=${_type}`;
    }
    var _name = $(elc).attr("id");
    var _num = 1;
    if (elc.files && elc.files.length > _num) {
        layer.msg(`选择的文件过多，至多选择${_num}个文件！`);
    }
    if (elc.files.length > 0) {
        var loadid = layer.load();
        var formData = new FormData();
        for (var k = 0; k < elc.files.length; k++) {
            formData.append(elc.files[k].name, elc.files[k]);
        }
        $.ajax({
            url: _url,
            type: 'post',
            contentType: false,
            processData: false,
            async: true,
            data: formData,
            headers: { "token": readToken() },
            success: function (obj) {
                layer.close(loadid);
                $(elc).parents(".formControls").find(`[name=${_name}]`).val(obj.data[0].url);
                elc.value = null;
            },
            error: function (res) {
                if (loadid != 0) {
                    layer.close(loadid);
                }
                elc.value = null;
                layer.msg(res.statusText, { icon: 'error', time: 3000 });
            }
        });
    } else {
        // layer.msg("请选择至少一个有效的文件！");readmemberToken
    }
}

/**
 * 处理图片的上传，这里最好限制下文件格式 这个要看实际需求 基于这个可以改造一个，上传文件的比如xlsx
 */
function handlerUploadFile(elc) {
    var _size = $(elc).attr("datasize");
    var _type = $(elc).attr("datatype");
    var _num = $(elc).attr("datanum");
    var _name = $(elc).attr("id");
    if (!_num) {
        _num = 1;
    }

    var haveNum = $(".drag_" + _name).find("img").length;
    var lessNum = _num - haveNum;
    if (elc.files && elc.files.length > lessNum) {
        layer.msg(`选择的文件过多，至多选择${lessNum}个文件！`);
        //这里还是有问题的 比如最多3个 先选择2个 后面又选择2个... .. . 
        return;
    }
    if (elc.files.length > 0) {
        var loadid = layer.load();
        var formData = new FormData();
        for (var k = 0; k < elc.files.length; k++) {
            formData.append(elc.files[k].name, elc.files[k]);
        }

        var uploadUrl = autoAppendCurrentQueryString(`${auto_root_path()}/Upload/UpImage?type=${_type}&size=${_size}`);

        $.ajax({
            url: uploadUrl,
            type: 'post',
            contentType: false,
            processData: false,
            async: true,
            data: formData,
            headers: { "token": readToken() },
            success: function (obj) {
                // console.log(obj);
                layer.close(loadid);
                appendImageToUI(_num, _name, obj);
                elc.value = null;
            },
            error: function (res) {
                console.log(res);
                if (loadid != 0) {
                    layer.close(loadid);
                }
                elc.value = null;
                layer.msg(res.statusText, { icon: 'error', time: 3000 });
            }
        });
    } else {
        // layer.msg("请选择至少一个有效的文件！");readmemberToken
    }
}

/**
 * 图片上传后 默认情况下地址就是值，有些情况下图片是图片，提交的不是图片，比如mediaid的情况下，这个情况下更新有问题！！！
 * @param {*} _num 
 * @param {*} _name 
 * @param {*} obj 上传的格式 data[] url
 */
function appendImageToUI(_num, _name, obj) {
    // console.log('appendImageToUI', _name, _num, obj);
    var property = funcFindPropertry(_name);
    if (_num == 1) {
        var currentElc = $(`[name=${_name}]`);
        currentElc.parents(".formControls").find(".form-image-head").attr('src', obj.data[0].url);
        currentElc.parents(".formControls").find(".iconadd").hide();
        currentElc.val(obj.data[0].url);
        currentElc.parents(".formControls").find(".icon-close").show();
        if (property) {
            var expend = funcFindAttribute(property.attributes, "imgexpend");
            if (expend) {
                if (expend.args3) {
                    if (obj.data[0].hasOwnProperty(expend.args3)) {
                        currentElc.val(obj.data[0][expend.args3]);
                    }
                }
            }
        }
    } else {
        // var _ahtml = "";
        // obj.data.forEach(img => {
        //     _ahtml += '<li class="draggable-item"><img src="' + img.url + '"  onerror="this.onerror=null; this.src=\'./lib/default.png\';"><i class="Hui-iconfont Hui-iconfont-close2 icon-close"  onclick="handlerRemoveImageItem(event,this)"></i></li>';
        // });
        // // var _imageul = $(elc).parents(".formControls").find(".imageul");//drag_{name}
        // var _imageul = `.drag_${_name}`;
        // $(_imageul).append(_ahtml);

        const fragment = document.createDocumentFragment();
        obj.data.forEach(img => {
            // 创建 li
            const li = document.createElement('li');
            li.className = 'draggable-item';

            // 创建 img
            const imgEl = document.createElement('img');
            imgEl.src = img.url || './lib/default.png';
            imgEl.alt = img.alt;
            imgEl.onerror = function () {
                this.onerror = null;
                this.src = './lib/default.png';
            };

            // 创建删除图标
            const icon = document.createElement('i');
            icon.className = 'Hui-iconfont Hui-iconfont-close2 icon-close';
            icon.onclick = function (e) {
                handlerRemoveImageItem(e, this);
            };

            li.appendChild(imgEl);
            li.appendChild(icon);
            fragment.appendChild(li);
        });

        // 使用 jQuery 或原生方式添加
        $(`.drag_${_name}`).append(fragment);
        // 或者
        // document.querySelector(`.drag_${_name}`).appendChild(fragment);

        //读取所有值到输入框中，作为值
        handlerReadImages(_name);
        //超出数量，隐藏添加按钮！
        if ($(_imageul).find("li").length > _num) {
            $(_imageul).find("li").eq(0).hide();
        } else {
            $(_imageul).find("li").eq(0).show();
        }
        if (_num > 1) {
            //更新UI信息 针对多图的拖拽顺序问题
            // var property = funcFindPropertry(_name);
            if (property) {
                if (property.dragimages) {
                    property.dragimages.initItems();
                }
            }
        }
    }
}

/**
 * 执行reload判断，判断是否要重新加载页面，需要API配合 还有一个类似的叫changeui
 * @param {*} attributes 
 * @param {*} name 
 * @param {*} val 
 */
function handlerReloadReplace(attributes, name, val) {
    if (attributes) {
        var _attribute = funcFindAttribute(attributes, "reload");
        if (_attribute) {
            if (_attribute.args1) {
                name = _attribute.args1;
            }
            handlerBuildNewUrlToLoad(name, val);
        }
    }
}

/**
 * 执行changeui，是否需要变更ui的显示和隐藏，注意默认值和必填项，如果一个项必填，但是UI隐藏了，那是否还是必填???
 */
function handlerChangeUi(attributes, val) {
    if (attributes) {
        // for (let _attribute of attributes) {
        //     if (_attribute.name == "changeui") {
        //         handlerChangeUiAttribute(_attribute, val, true);
        //     }
        // }
        privateChangeUiHandler(attributes, val, true);
    }
}


//-----------------------------------------------------------------
/*
gname:表示一个组
name:表示一个字段
hit:命中次数 大于0保持显示 为0表示要隐藏
obj:gname或者name表示的对象，类型动态的，用于快速定位信息
key:gname+name，用于锁定是否存在,主要为了hit++
*/

/**
 * changeui功能的总入口，因对多个attribute的互斥问题
 * @param {*} attributes 
 * @param {*} val 当前值
 * @param {*} changeUi 
 */
function privateChangeUiHandler(attributes, val, changeUi) {
    if (attributes) {
        var maps = new Map();
        for (let _attribute of attributes) {
            if (_attribute.name == "changeui") {
                privateHandlerChangeUiAttribute(_attribute, val, maps);
            }
        }
        maps.forEach((map, key) => {
            if (map.hit > 0) {
                //去显示
                if (map.name != "") {
                    privateChangeUiItemToShow(map.obj, changeUi);
                }
                if (map.gname != "") {
                    privateChangeUiGroupToShow(map.obj, changeUi);
                }
            } else {
                //去隐藏
                if (map.name != "") {
                    privateChangeUiItemToHide(map.obj, changeUi);
                }
                if (map.gname != "") {
                    privateChangeUiGroupToHide(map.obj, changeUi);
                }
            }
        });
    }
}

/**
 * 
 * @param {*} _item 
 * @param {*} isui 
 */
function privateChangeUiItemToHide(_item, isui) {
    if (!_item.hide) {
        _item.hidden = true;
    }
    if (isui) {
        if (!$(`.item_${_item.name}`).hasClass("hide")) {
            $(`.item_${_item.name}`).addClass("hidden").hide();
        }
    }
}

/**
 * 
 * @param {*} _item 
 * @param {*} isui 
 */
function privateChangeUiItemToShow(_item, isui) {
    if (!_item.hide) {
        _item.hidden = false;
    }
    if (isui) {
        if (!$(`.item_${_item.name}`).hasClass("hide")) {
            $(`.item_${_item.name}`).removeClass("hidden").show();
        }
        if (_item.dataType == "markdown") {
            if (_item.editor) {
                _item.editor.resize();
            }
        }
    }
}

/**
 * 
 * @param {*} _gn 
 * @param {*} isui 
 */
function privateChangeUiGroupToShow(_gn, isui) {
    if (!_gn.hide) {
        _gn.hidden = false;
    }
    if (isui) {
        if (!$(`#group_id_${_gn.gcode}`).hasClass("hide")) {
            $(`#group_id_${_gn.gcode}`).removeClass("hidden").show();
        }
    }
}

/**
 * 
 * @param {*} _gn 
 * @param {*} isui 
 */
function privateChangeUiGroupToHide(_gn, isui) {
    if (!_gn.hide) {
        _gn.hidden = true;
    }
    if (isui) {
        if (!$(`#group_id_${_gn.gcode}`).hasClass("hide")) {
            $(`#group_id_${_gn.gcode}`).addClass("hidden").hide();
        }
    }
}

/**
 * 
 * @param {*} _attribute 
 * @param {*} val 
 * @param {*} maps 
 */
function privateHandlerChangeUiAttribute(_attribute, val, maps) {
    if (_attribute.args1) {
        let match = false;
        if (_attribute.args1.split(',')) {
            for (let _val of _attribute.args1.split(',')) {
                if (_val == val) {
                    match = true;
                    break;
                }
            }
        }
        if (!match) {
            privateChangeUiAutoForeach(_attribute, maps, 0);
        } else {
            privateChangeUiAutoForeach(_attribute, maps, 1);
        }
    }
}

/**
 * 
 * @param {*} _attribute 
 * @param {*} maps 
 * @param {*} strs 
 * @param {*} math 
 */
function privateChangeUiAutoForeach(_attribute, maps, math) {
    if (_attribute.args2) {
        var strs = _attribute.args2.split(',');
        for (var _str of strs) {
            if (_str.indexOf("group:") >= 0) {
                var _gname = _str.substring(6);
                if (_gname.indexOf("regexp:") >= 0) {
                    let _reg = new RegExp(_gname.substring(7));
                    for (const _gn of _ui_data_list) {
                        if (_gn.isgroup) {
                            if (_reg.test(_gn.gcode)) {
                                privateAutoSetMap(maps, _gn.gcode, "", _gn, math);
                            }
                        }
                    }
                } else {
                    for (const _gn of _ui_data_list) {
                        if (_gn.isgroup) {
                            if (_gn.gcode == _gname) {
                                privateAutoSetMap(maps, _gn.gcode, "", _gn, math);
                            }
                        }
                    }
                }
                continue;
            }
            if (_str.indexOf("regexp:") >= 0) {
                let _reg = new RegExp(_str.substring(7));
                for (const _item of _global_data_properties) {
                    if (_reg.test(_item.name)) {
                        privateAutoSetMap(maps, "", _item.name, _item, math);
                    }
                }
                continue;
            }
            for (const _item of _global_data_properties) {
                if (_item.name == _str) {
                    privateAutoSetMap(maps, "", _item.name, _item, math);
                }
            }
        }
    }
}


/**
 * 自动++或者写入
 * @param {*} maps 
 * @param {*} gname 
 * @param {*} name 
 * @param {*} obj 对象，可能是group 也可能是property
 * @param {*} math 命中填写1，未命中填写0
 */
function privateAutoSetMap(maps, gname, name, obj, math) {
    var onlyKey = `g${gname}-i${name}`;
    var find = maps.get(onlyKey);
    if (find) {
        if (math) {
            find.hit++;
        }
    } else {
        maps.set(onlyKey, {
            gname: gname,
            name: name,
            hit: math,
            obj: obj
        });
    }
}

//======================================================================




/**
 * 适应reload
 * @param {*} _name 
 * @param {*} _val 
 */
function handlerBuildNewUrlToLoad(_name, _val) {
    // URLSearchParams 这个兼容性
    var _saerch = location.search.substring(1);//当前模式下必然有?path=xxx等
    var argms = _saerch.split('&');
    var argses = new Array();
    for (var _args of argms) {
        if (_args.indexOf(`${_name}=`) == -1) {
            argses.push(_args);
        }
    }
    argses.push(`${_name}=${_val}`);
    //组合query进行reload
    // URLSearchParams.toString
    var _newUrl = `${location.pathname}?${argses.join('&')}`;
    location.replace(_newUrl);
}

/**
 * 组合带上当前页面的query生成新的url 如果参数已经存在，则不会进行覆盖
 * @param {*} url 
 * @returns 
 */
function autoAppendCurrentQueryString(url) {
    var queryIndex = url.indexOf('?');
    var hasFields = new Array();
    if (queryIndex > 0) {
        var has = url.substring(queryIndex + 1);
        var hasquerys = has.split('&');
        for (var arg of hasquerys) {
            if (arg.indexOf('=') > 0) {
                let na = arg.split('=')[0];
                // let va = arg.split('=')[1];
                hasFields.push(na);
            }
        }
    }
    var _saerch = location.search.substring(1);//当前模式下必然有?path=xxx等
    var argms = _saerch.split('&');
    var appendUrl = new Array();
    for (var _args of argms) {
        if (_args.indexOf('=') > 0) {
            let na = _args.split('=')[0];
            if (hasFields.indexOf(na) == -1) {
                appendUrl.push(_args);
            }
        }
    }
    if (appendUrl.length > 0) {
        var appStr = appendUrl.join('&');
        if (url.indexOf('?') >= 0) {
            url = `${url}&${appStr}`;
        } else {
            url = `${url}?${appStr}`;
        }
    }
    return url;
}

/**
 * 判断select是否要执行reload事件
 * @param {*} elc 
 */
function handlerSectionChange(elc) {
    var _name = $(elc).attr("name");
    var _property = funcFindPropertry(_name);
    if (_property) {
        var _val = $(elc).val();
        handlerReloadReplace(_property.attributes, _name, _val);
        handlerChangeUi(_property.attributes, _val);
        // var _attribute = funcFindAttribute(_property.attributes, "reload");
        // if (_attribute) {
        //     var _val = $(elc).val();
        //     if (_attribute.args1) {
        //         _name = _attribute.args1;
        //     }
        //     handlerBuildNewUrlToLoad(_name, _val);
        // }
        handlerLoaditem(_property);
    }
}

/**
 * 转化数据类型，从数据库的类型转化为UI使用的类型
 * @param {*} properties 
 */
function handlerExchangeDataTypeToUIType(properties) {
    properties.forEach(item => {
        handlerExchangeItemToUIData(item);
        if (_config) {
            if (!_config.dynamic) {
                if (item.name.indexOf("__") >= 0) {
                    _config.dynamic = true;
                }
            }
        }
    });
}

/**
 * 转化数据格式为UI格式
 */
function handlerExchangeItemToUIData(item) {
    item.dataFrom = item.dataType;
    //这里需要处理字段的属性
    if (item.attributes) {
        item.attributes.forEach(_attribute => {
            switch (_attribute.name) {
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
                case "hide":
                    {
                        item.hide = true;//始终隐藏
                        item.hidden = true;//可以被changeui唤醒
                    }
                    break;
                case "required":
                    {
                        item.required = true;
                    }
                    break;
                case "empty":
                    {
                        item.empty = true;
                    }
                    break;
                case "readonly":
                    {
                        item.readonly = true;
                    }
                    break;
                case "textarea":
                    {
                        item.dataType = "textarea";
                        item.singlerow = true;
                        if (_attribute.args1) {
                            item.areastyle = _attribute.args1;
                        }
                        if (_attribute.args2) {
                            if (_attribute.args2 == "half") {
                                item.singlerow = false;
                            }
                        }
                    }
                    break;
                case "cover":
                    {
                        item.dataType = "cover";
                        if (!item.value) {
                            item.value = new Array();
                        } else {
                            if (!Array.isArray(item.value)) {
                                item.value = new Array(item.value);
                            }
                        }
                    }
                    break;
                case "text":
                    {
                        item.dataType = "text";
                        if (_attribute.args1) {
                            item.singlerow = true;
                        }
                        if (item.maxlength) {
                            if (item.maxlength > 64) {
                                item.singlerow = true;
                            }
                        }
                        if (_attribute.args2) {
                            item.class = _attribute.args2;
                        }
                    }
                    break;
                case "fentoyuan":
                    {
                        if (item.value) {
                            item.value = (parseFloat(item.value) / 100).toFixed(2);//(parseFloat(item.value) / 100).toFixed(2);
                        }
                        item.dataType = "fentoyuan";
                    }
                    break;
                case "file":
                    {
                        item.dataType = 'file';
                        if (_attribute.args1) {
                            item.url = _attribute.args1;
                        }
                        if (_attribute.args2) {
                            item.type = _attribute.args2;
                        }
                        if (_attribute.args3) {
                            item.filehandler = _attribute.args3;
                        }
                        if (!item.filehandler) {
                            if (!_attribute.args1) {
                                item.url = `/${readRootPath()}/Upload/UpFile`;
                            }
                        }
                        if (_attribute.args4) {
                            item.accept = _attribute.args4;
                        } else {
                            item.accept = "*.*";
                        }
                    }
                    break;
                case "image":
                    {
                        item.dataType = 'image';
                        if (_attribute.args1) {
                            item.num = parseInt(_attribute.args1);
                        } else {
                            item.num = 1;
                        }
                        if (_attribute.args2) {
                            item.type = _attribute.args2;
                        }
                        if (_attribute.args3) {
                            item.size = _attribute.args3;
                        }
                        if (item.value && item.value != "null") {
                            if (item.num > 1) {
                                if (item.dataFrom == "String[]") {
                                    item.images = item.value;//多个图片是一个数组，字符串数组的形式
                                    item.value = item.value.join(',');
                                } else {
                                    item.images = item.value.split(',');//多个图片之间使用,隔开
                                }
                            }
                        } else {
                            item.value = null;
                        }
                        if (item.num > 1) {
                            item.singlerow = true;
                        }
                        //如何针对mediaid的情况，也就是需要url+mediaid
                        if (!item.imgid) {
                            item.imgid = item.value;
                        }
                    }
                    break;
                case "imgexpend":
                    {
                        if (_attribute.args1) {
                            item.imgid = _attribute.args1;
                        }
                        if (_attribute.args2) {
                            item.value = _attribute.args2;
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
                case "datalist":
                    {
                        item.dataType = 'datalist';
                        item.datalistid = item.name;
                        if (_attribute.args1) {
                            try {
                                item.selects = JSON.parse(_attribute.args1);
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
                case "selects":
                case "select":
                case "lselect":
                    {
                        try {
                            item.selects_max = "lsingle";
                            if (_attribute.args4) {
                                if (_attribute.args4 != "1") {
                                    item.selects_max = "lbatch";
                                }
                            }
                            item.selects = JSON.parse(_attribute.args1);
                            if (item.value || item.value == 0) {
                                var _typeof = "number";
                                var valMap = new Map();
                                var _value = item.value;
                                if (Array.isArray(_value)) {
                                    for (const _val of _value) {
                                        _typeof = typeof (_val);
                                        valMap.set(_val, true);
                                    }
                                } else {
                                    _typeof = typeof (item.value);
                                    valMap.set(item.value, true);
                                }
                                // var _typeof = typeof (item.value);
                                item.valueType = _typeof;
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
                                            item.selects.forEach(ii => {
                                                if (valMap.has(ii.value)) {
                                                    ii.selected = true;
                                                }
                                            });
                                        }
                                        break;
                                    case "object":
                                        {
                                            if (Array.isArray(item.value)) {
                                                // item.selects.forEach(ii => {
                                                //     if (item.value.indexOf(ii.value) >= 0) {
                                                //         ii.selected = true;
                                                //     }
                                                //     // typeof(ii);
                                                // });
                                                for (const ii of item.selects) {
                                                    item.dataFrom = `${typeof (ii.value)}[]`;
                                                    if (item.value.indexOf(ii.value) >= 0) {
                                                        ii.selected = true;
                                                        break;
                                                    }
                                                }
                                            } else {
                                                var _vals = item.value.toString();
                                                // item.selects.forEach(ii => {
                                                //     if (_vals.indexOf(ii.value.toString()) >= 0) {
                                                //         ii.selected = true;
                                                //     }
                                                // });
                                                for (const ii of item.selects) {
                                                    item.dataFrom = `${typeof (ii.value)}[]`;
                                                    if (_vals.indexOf(ii.value.toString()) >= 0) {
                                                        ii.selected = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            item.dataType = _attribute.name;
                            if (item.selects) {
                                if (_attribute.name == "selects") {
                                    //name value selected
                                    var selectedItems = item.selects.filter(function (o) {
                                        return o.selected;
                                    });
                                    if (selectedItems.length > 0) {
                                        item.__selected_values = '(' + selectedItems.length + ')'
                                            + selectedItems.map(function (o) { return o.name; }).join(' ');
                                    }
                                }
                            }
                        } catch (err) {
                            console.log(item.name, "属性 select 的值 args1 配置错误", err);
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
                case "style":
                    {
                        item.style = _attribute.args1;
                    }
                    break;
                case "class":
                    {
                        item.class = _attribute.args1;
                    }
                    break;
                case "button":
                    {
                        item.dataType = "button";
                        if (!item.buttons) {
                            item.buttons = new Array();
                        }
                        var _btn = ({
                            name: _attribute.args1,
                            func: null
                        });
                        if (_attribute.args1) {
                            _btn.value = _attribute.args1;
                        } else {
                            _btn.value = item.value;
                            _btn.name = item.name;
                        }
                        if (_attribute.args2) {
                            _btn.btnfunc = _attribute.args2;
                        }
                        item.buttons.push(_btn);
                    }
                    break;
                case "score":
                    {
                        item.dataType = "score";
                    }
                    break;
                case "singlerow": {
                    item.singlerow = true;
                }
                    break;
                case "markdown":
                    {
                        item.dataType = "markdown";
                        item.singlerow = true;
                        _global_need_loadmarkdown = true;
                        if (_attribute.args2) {
                            var _target = funcFindExpendPropertry(_attribute.args2, item.name);
                            if (_target) {
                                _target.hidden = true;
                            }
                        }
                    }
                    break;
                case "richtext":
                    {
                        item.dataType = "richtext";
                        item.singlerow = true;
                        _global_need_loadrichhtml = true;
                    }
                    break;
                case "unit":
                    {
                        item.unit = _attribute.args1;
                    }
                    break;
                case "simg":
                    {
                        // 用于图片选择器，之前旧版是使用unit标注，新版本得使用PasteImageselectAttribute标注
                        var simg = new {
                            url: './images/index.html',
                            query: '?type=imgs'
                        };
                        if (_attribute.args1) {
                            simg.url = _attribute.args1;
                        }
                        if (_attribute.args2) {
                            simg.query = _attribute.args2;
                        }
                        item.simg = simg;
                    }
                    break;
                case "halfrow":
                    {
                        //所以这个要放最后面
                        item.singlerow = false;
                        item.halfrow = true;
                    }
                    break;
                case "object":
                    {
                        item.dataType = 'object';
                        item.session_key = `${item.name}_0000`;
                        if (item.value) {
                            sessionStorage.setItem(item.session_key, JSON.stringify(item.value));
                            //UI上如何显示
                            if (_attribute.args4) {
                                if (item.value.hasOwnProperty(_attribute.args4)) {
                                    item.display = item.value[_attribute.args4];
                                }
                            }
                        } else {
                            sessionStorage.removeItem(item.session_key);
                        }
                        if (!item.hidden) {
                            _has_outer = true;
                        }
                    }
                    break;
                case "objects":
                    {
                        item.dataType = 'objects';
                        if (item.value) {
                            item.objects = item.value;

                        }
                        if (_attribute.args3) {
                            item.display_id = _attribute.args3;
                        } else {
                            item.display_id = "id";
                        }
                        if (_attribute.args4) {
                            item.display_name = _attribute.args4;
                        } else {
                            item.display_name = "name";
                        }
                        if (item.objects) {
                            var _list = new Array();
                            item.objects.forEach(_item => {
                                _item._id = handlerColumnValue(_item, item.display_id);
                                _item._display = handlerColumnValue(_item, item.display_name);
                                _list.push(_item);
                            });
                            _map_values.set(item.name, _list);
                        }
                        _has_outer = true;
                    }
                    break;
                case "outer":
                    {
                        item.dataType = 'outer';
                        let _set_value = true;
                        if (_attribute.args2) {
                            //这里args2有问题，需要对应转化
                            if (item.name.indexOf("__") > 0) {
                                var lastIndex = item.name.lastIndexOf("__") + 2;
                                _attribute.args2 = item.name.substring(0, lastIndex) + _attribute.args2;
                            }
                            var _target = funcFindExpendPropertry(_attribute.args2, item.name);
                            if (_target) {
                                _target.hidden = true;
                                if (_target.value) {
                                    if (typeof _target.value == "object") {
                                        if (_attribute.args4) {
                                            // item.display = _target.value[_attribute.args4];
                                            item.display = handlerColumnValue(_target.value, _attribute.args4);// _target.value[_attribute.args4];
                                            _set_value = false;
                                        }
                                    }
                                }
                            }
                        }
                        if (_set_value) {
                            if (item.value) {
                                item.display = item.value;
                            }
                            item.valueType = typeof (item.value);
                        }
                        _has_outer = true;
                    }
                    break;
                case "outers":
                    {
                        item.dataType = 'outers';
                        if (_attribute.args2) {
                            if (item.name.indexOf("__") > 0) {
                                var lastIndex = item.name.lastIndexOf("__") + 2;
                                _attribute.args2 = item.name.substring(0, lastIndex) + _attribute.args2;
                            }
                            var _target = funcFindExpendPropertry(_attribute.args2, item.name);
                            if (_target) {
                                _target.hidden = true;
                                if (_target.value && _target.value != '') {
                                    item.display = _target.value;
                                    item.display.forEach(_dis => {
                                        _dis._display = handlerColumnValue(_dis, _attribute.args4);
                                    });
                                }
                            }
                        }
                        item.display_id = _attribute.args3;
                        item.display_name = _attribute.args4;
                        _has_outer = true;
                    }
                    break;
                case "splitarray":
                    {
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
                case "datetime":
                    {
                        item.dataType = "datetime";
                        var _format = "yyyy-MM-dd HH:mm:ss";
                        if (_attribute.args1) {
                            _format = _attribute.args1;
                        }
                        item.format = _format;
                        if (item.value) {
                            if (_attribute.args2) {
                                switch (_attribute.args2) {
                                    case "times":
                                        {
                                            item.value = parseDate(item.value * 1000).Format(_format);
                                        }
                                        break;
                                    case "timems":
                                        {
                                            item.value = parseDate(item.value).Format(_format);
                                        }
                                        break;
                                    default:
                                        {
                                            item.value = parseDate(item.value).Format(_format);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    break;
                case "point":
                    {
                        item.point = true;
                    }
                    break;
                case "sunform":
                    {
                        item.dataType = "sunform";
                        item.singlerow = true;
                        item.frame = {
                            height: 300,
                            url: "",
                            class: ""
                        };
                        var session_key = `${item.name}_00000`;
                        if (item.value) {
                            sessionStorage.setItem(session_key, JSON.stringify(item.value));
                        } else {
                            sessionStorage.removeItem(session_key);
                        }
                        if (_attribute.args3) {
                            item.frame.class = _attribute.args3;
                        }
                        //model=sunobj
                        var _tar_url = handlerBuildDynamicUrl(_attribute.args1);
                        if (_tar_url.indexOf('/') >= 0) {
                            if (_tar_url.indexOf("?") >= 0) {
                                item.frame.url = _tar_url + '&session_key=' + session_key;
                            } else {
                                item.frame.url = _tar_url + '?session_key=' + session_key;
                            }
                        } else {
                            item.frame.url = `${handlerPasteFormPath()}view.html?path=${_tar_url}&model=sunobj&session_key=${session_key}`;
                        }
                        if (_attribute.args2) {
                            item.frame.height = _attribute.args2;
                        }
                    }
                    break;
                case "notice":
                case "password":
                    {
                        item.dataType = _attribute.name;
                    }
                    break;
                case "changeui":
                    {
                        if (_changeui_list) {
                            _changeui_list.push({
                                attribute: _attribute,
                                value: item.value,
                                name: item.name
                            });
                            //基于name排重到时候！！！
                        }
                        //这里有问题 当前值是多少 changeui ... .. .
                    }
                    break;
                case "html":
                    {
                        //在form中，html直接读取字段的值!
                        item.dataType = "html";
                    }
                    break;
                default:
                    break;
            }
        });
    }

    //如果是故意要暴露id的呢？
    if (item.name == "id") {
        if (!item.placeholder) {
            item.hidden = true;
        }
        //编辑模式必须隐藏，防止修改到其他信息去
        if (_config.model == 'edit') {
            item.hidden = true;
        }
        //所以如果id需要自定义的需要特别备注比如 ID 可填可不填
    }

    //Guid的默认处理
    if (item.dataFrom == "Guid") {
        if (item.value == "00000000-0000-0000-0000-000000000000") {
            item.value = "";
            if (item.display == "00000000-0000-0000-0000-000000000000") {
                item.display = "";
            }
        }
    }

    switch (item.dataType) {
        case "Int32":
        case "Int64":
        case "UInt32":
        case "UInt64":
            {
                item.dataType = "number";
                if (item.value == "0") {
                    item.value = "";
                }
            }
            break;
        case "DateTime":
            {
                item.dataType = "datetime";
                if (item.value && item.value == '01/01/0001 00:00:00') {
                    item.value = "";
                }
                if (!item.format) {
                    item.format = "yyyy-MM-dd HH:mm:ss";
                }
            }
            break;
        case "Boolean":
            {
                item.dataType = "switch";
            }
            break;
        case "Double":
        case "Decimal":
            {
                item.step = "0.01";
                if (item.attributes) {
                    item.attributes.forEach(_attribute => {
                        if (_attribute.name == 'double') {
                            item.step = _attribute.args1;
                        }
                    });
                }
            }
            break;
        case "String":
            {
                if (item.value == null) {
                    item.value = "";
                }
                if (item.maxlength == 0) {
                    item.dataType = "richtext";
                    item.singlerow = true;
                    _global_need_loadrichhtml = true;
                }
                if (item.maxlength > 128) {
                    item.dataType = "textarea";
                    item.singlerow = true;
                }
                if (item.maxlength <= 128 && item.maxlength != 0) {
                    item.dataType = "text";
                    if (item.maxlength > 64) {
                        item.singlerow = true;
                    }
                }
                //可能前面标记了
                if (item.halfrow) {
                    item.singlerow = false;
                }
            }
            break;
    }

    if (item.hidden) {
        if (item.required) {
            item.required = false;
        }
    }
    if (item.required) {
        if (item.point) {
            item.point = false;
        }
    }
}

/**
 * 查找组信息 并添加到数据集合 或者是添加子项
 * @param {*} _group_name 
 * @param {*} _ui_data_list 
 * @returns 
 */
function handlerTakeGroupInfo(_group_name, _ui_data_list, item) {
    var _handler = false;
    if (_ui_data_list) {
        for (var ii of _ui_data_list) {
            if (ii.isgroup) {
                if (ii.gcode == _group_name) {
                    ii.groups.push(item);
                    _handler = true;
                    break;
                }
            }
        }
    }

    if (_global_class_attributes && !_handler) {
        for (var ii of _global_class_attributes) {
            if (ii.name == "group") {
                if (ii.args1 == _group_name) {
                    var _info = {
                        isgroup: true,
                        singlerow: true,
                        gcode: _group_name,
                        gname: ii.args2,
                        isopen: true,
                        groups: new Array(),
                        hidden: false
                    };
                    if (ii.args3) {
                        if (ii.args3 != "open") {
                            _info.isopen = false;
                        }
                    }
                    if (_info.isopen) {
                        _info.gclass = "open";
                    } else {
                        _info.gclass = "close";
                    }
                    if (ii.args4) {
                        _info.gmodel = 'edit';
                    }
                    if (ii.args3 == 'hidden') {
                        _info.hidden = true;
                    }
                    _handler = true;
                    _info.groups.push(item);
                    _ui_data_list.push(_info);
                    break;
                }
            }
        }
    }
    //如果没有找到，是否基于当前New一个
    if (!_handler) {
        var ii = funcFindAttribute(item.attributes, "group");
        if (ii) {
            var _info = {
                isgroup: true,
                singlerow: true,
                gcode: _group_name,
                gname: ii.args2,
                isopen: true,
                groups: new Array(),
                hidden: false
            };
            if (ii.args3) {
                if (ii.args3 != "open") {
                    _info.isopen = false;
                }
            }
            if (_info.isopen) {
                _info.gclass = "open";
            } else {
                _info.gclass = "close";
            }
            if (ii.args4) {
                _info.gmodel = 'edit';
            }
            if (ii.args3 == 'hidden') {
                _info.hidden = true;
            }
            _handler = true;
            _info.groups.push(item);
            _ui_data_list.push(_info);
        }
    }
    return null;
}

/**
 * 输入框失去焦点后
 * @param {*} elc 
 */
function handlerBlur(elc) {
    var name = $(elc).attr("name");
    if (name) {
        var find = funcFindPropertry(name);
        if (find) {
            handlerLoaditem(find);
        }
    }
}

/**
 * 执行特性loaditem
 * @param {*} find 当前是某一个字段 
 */
function handlerLoaditem(find) {
    if (!find.attributes) {
        return;
    }
    var _laterread = funcFindAttribute(find.attributes, "loaditem");
    if (_laterread) {
        var _url = _laterread.args1;
        if (_url) {
            var _read_url = handlerBuildDynamicUrl(_url);
            _apiget(_read_url, true, (cc, oo) => {
                if (cc == 200) {
                    var newList = new Array();
                    var mapKeys = new Map();
                    if (Array.isArray(oo)) {
                        for (var _item of oo) {
                            mapKeys.set(_item.name, _item);
                        }
                    } else {
                        mapKeys.set(oo.name, oo);
                    }
                    if (_global_data_properties) {
                        //进行更替
                        _global_data_properties.forEach(_item => {
                            if (mapKeys.has(_item.name)) {
                                newList.push(mapKeys.get(_item.name));
                            } else {
                                newList.push(_item);
                            }
                        });
                        _global_data_properties = newList;
                    }
                    //oo可能是一个集合，先要进行数据更替
                    if (Array.isArray(oo)) {
                        oo.forEach(ooitem => {
                            handler_replace_item(ooitem);
                        });
                    } else {
                        handler_replace_item(oo);
                    }
                }
            });
        }
    }
}

/**
 * 更换UI的某一块 表单中的某一块
 * @param {*} item 
 */
function handler_replace_item(item) {
    if (item.hidden) {
        return;
    }
    handlerExchangeItemToUIData(item);
    //如果是Outer模式呢???
    var _name = item.name;
    var _theml = $("#template_tr_item").html();
    var _vhtml = template(_theml, { item: item });
    if ($(`.item_${_name}`).length > 0) {
        $(`.item_${_name}`).replaceWith(_vhtml);
    } else {
        $(".paste-form-body").append(_vhtml);
    }
}


/**
 * 从父级查找对应的页面，然后吧值更新到页面中，这里针对的是table的行数据新增或者变更！
 * 查找并更新数据行，一般用于表格数据行需要变动的时候
 * @param {*} obj 导入的数据，可能是单行的ListDto也可能是ListDto数组
 * @param {*} urlKey 可选参数，表示url需要匹配这个关键字，一般用path的内容做找寻
 * @returns 
 */
function findAndCallHandler(obj, urlKey) {
    // 定义一个内部查找函数，专门负责在指定的窗口(window)里找 iframe 并调用
    const searchInWindow = (targetWin) => {
        // 1. 优先找 layer 弹窗 (层级通常最高)
        // 注意：layer 的 iframe 类名通常是 layui-layer-iframe
        var iframes = targetWin.document.querySelectorAll('.layui-layer-iframe iframe');

        // 2. 如果没找到，或者想兼容普通容器，再找 .show_iframe
        if (iframes.length === 0) {
            // var containers = targetWin.document.querySelectorAll('.show_iframe');
            // if (containers.length > 0) {
            //     // 找到了容器，从容器里取 iframe
            //     // 这里假设一个容器里只有一个主要 iframe，或者取第一个
            //     iframes = containers[0].querySelectorAll('iframe');
            // }
            iframes = targetWin.document.querySelectorAll('.show_iframe iframe');
        }
        if (iframes.length === 0) {
            console.log('searchInWindow: 没有找到任何 iframe');
            //没有iframe 可能就是当前自己
            return false;
        }
        // 3. 倒序遍历 (reverse)，确保最新打开的（显示在上面的）先被调用
        // 这一步替代了复杂的 z-index 计算
        var iframeList = Array.from(iframes).reverse();
        for (let iframe of iframeList) {
            if (!iframe.contentWindow) {
                console.warn('iframe.contentWindow 不存在，跳过', iframe.src);
                continue;
            }
            if (urlKey) {
                var iframeBaseURI = iframe.contentWindow.location.href;
                if (iframeBaseURI.indexOf(urlKey) == -1) {
                    // console.log('callUpdateRow iframe.src不匹配', iframeBaseURI, urlKey);
                    continue;
                }
            }
            try {
                if (iframe.contentWindow && typeof iframe.contentWindow.handlerUpdateRow == 'function') {
                    // console.log('找到并调用 handlerUpdateRow');
                    var result = iframe.contentWindow.handlerUpdateRow(obj);
                    if (result) {
                        return true;
                    }
                }
            } catch (e) {
                // 跨域或错误忽略
                console.log(e);
            }
        }
        return false;
    };

    // ================= 核心逻辑判断 =================
    var find = false;
    if (window.top !== window.self) {
        const rootWindow = window.top;;
        // 针对/page/index.html中的页面框架的内页 工作台页面找
        const showIframes = rootWindow.document.querySelectorAll('.show_iframe');
        for (let showIframe of showIframes) {
            var handler = callUpdateRow(showIframe, obj, urlKey);
            if (handler) {
                return true;
            }
        }
        find = searchInWindow(window.top);
        if (find) {
            return true;
        }
    }

    if (window.parent != window.self) {
        const rootWindow = window.parent;
        // 针对/page/index.html中的页面框架的内页 工作台页面找
        const showIframes = rootWindow.document.querySelectorAll('.show_iframe');
        for (let showIframe of showIframes) {
            var handler = callUpdateRow(showIframe, obj, urlKey);
            if (handler) {
                return true;
            }
        }
        find = searchInWindow(window.top);
        if (find) {
            return true;
        }
    }

    //都没有找到 可能是父级页面
    if (window.parent !== window.self) {
        var currwin = window.parent;
        if (currwin) {
            if (typeof currwin.handlerUpdateRow === 'function') {
                var result = currwin.handlerUpdateRow(obj);
                if (result) {
                    return true;
                }
            }
        }
    }

    var currwin = window;//就是自己当前页面
    if (currwin) {
        if (typeof currwin.handlerUpdateRow === 'function') {
            currwin.handlerUpdateRow(obj);
            return true;
        }
    }

}


/**
 * 主要的作用是从工作台找到对应的页面，然后更新表格的数据行，单行或者多行
 * @param {*} showIframe 
 * @returns 
 */
function callUpdateRow(showIframe, obj, pathName) {
    try {
        // 检查 .show_iframe 是否可见
        const style = window.getComputedStyle(showIframe);
        if (style.display !== 'none' && style.visibility !== 'hidden') {
            // 在显示的 .show_iframe 中查找 iframe
            const iframes = showIframe.querySelectorAll('iframe');
            for (let iframe of iframes) {
                if (pathName) {
                    var iframeBaseURI = iframe.contentWindow.location.href;
                    if (iframeBaseURI.indexOf(pathName) == -1) {
                        console.log('callUpdateRow iframe.src不匹配', iframeBaseURI, pathName);
                        continue;
                    }
                    console.log('callUpdateRow iframe.src匹配', iframeBaseURI, pathName);
                }
                try {
                    const iframeStyle = window.getComputedStyle(iframe);
                    // 检查 iframe 是否可见且已加载
                    if (iframeStyle.display !== 'none' && iframeStyle.visibility !== 'hidden' && iframe.contentWindow) {
                        const iframeWindow = iframe.contentWindow;
                        // 检查是否存在 handlerUpdateRow 函数
                        if (typeof iframeWindow.handlerUpdateRow === 'function') {
                            // 调用 handlerUpdateRow 并传入 obj
                            let result = iframeWindow.handlerUpdateRow(obj);
                            if (result) {
                                return true;
                            }
                        }
                    }
                } catch (error) {
                    // 处理跨域限制等情况
                    console.warn('无法访问 iframe 内容:', error);
                }
            }
        }
    } catch (error) {
        console.warn('无法访问 .show_iframe 元素:', error);
    }
    return false;
}



/*******************************/

/**
 * 读取模型和默认值，值等
 */
function dynamicReadModel(_url) {
    // var _url=`/api/app/dynamicHelper/read${location.search}`;
    // if(location.search.indexOf("datakey")==-1){
    //     _url=`/api/app/dynamicHelper/view${location.search}`;
    // }
    _apiget(_url, true, (c, o) => {
        if (c == 200) {
            loadHeader(o);

            if (o.code) {
                _classPath = o.code;
                _config.className = o.code;
            }

            if (o.title) {
                this.document.title = "更新" + o.title;
            }



            _global_data_properties = new Array();

            //整合数据，整合字段信息用于显示UI
            if (o.data) {
                if (o.properties) {
                    _setting_data_properties = o.properties;
                } else {
                    _setting_data_properties = new Array();
                }
                dynamicBuildPropertyInfo(o.data, "", -1);
            }

            //检查必显项目
            if (o.properties) {
                for (var _pro of o.properties) {
                    if ((_pro.showView && _config.model == "view")) {
                        var _find_pro = false;
                        for (var ii of _global_data_properties) {
                            if (ii.name == _pro.name) {
                                _find_pro = true;
                                break;
                            }
                        }
                        if (!_find_pro) {
                            if (!_pro.matchRegex) {
                                _global_data_properties.push(_pro);
                            }
                        }
                    }
                }
            }

            //组合goup
            if (!o.attributes) {
                o.attributes = new Array();
            }
            //处理分组的问题
            var groups = new Array();
            _global_data_properties.forEach(_property => {
                if (_property.attributes) {
                    for (var _attribute of _property.attributes) {
                        if (_attribute.name == 'group') {
                            if (groups.indexOf(_attribute.args1) == -1) {
                                groups.push(_attribute.args1);
                            }
                        }
                    }
                }
            });
            if (groups.length > 0) {
                groups.forEach(igroup => {
                    var _find = false;
                    for (var _oa of o.attributes) {
                        if (_oa.name == igroup) {
                            _find = true;
                            break;
                        }
                    }
                    if (!_find) {
                        //这里要做成动态的，可能有配置信息
                        o.attributes.push(dynamicBuildGroupAttribute(igroup));
                    }
                });
                _global_class_attributes = o.attributes;
            }

            // //检查必显项目
            // if(o.properties){
            //     for(var _pro of o.properties){
            //         if((_pro.showView && _config.model=="view")){
            //             var _find_pro = false;
            //             for(var ii of _global_data_properties){
            //                 if(ii.name == _pro.name){
            //                     _find_pro=true;
            //                     break;
            //                 }
            //             }
            //             if(!_find_pro){
            //                 if(!_pro.matchRegex){
            //                     _global_data_properties.push(_pro);
            //                 }
            //             }
            //         }
            //     }
            // }

            //使用pasteform模式加载表单内容到UI
            LoadModelProperity(_global_data_properties);

            if (o.table) {
                //paste-btn-submit
                if (o.table.readOnly) {
                    $(".paste-btn-submit").hide();
                }
            }
        }
    });
}

/**
 * 构建分组 
 * @param {*} igroup 
 * @returns 
 */
function dynamicBuildGroupAttribute(igroup) {
    //未完待续，是否结合配置信息
    return {
        name: "group",
        args1: igroup,
        args2: dynamicReadSettingName(igroup)
    };
}

//递归处理数据类型！ new Property


/**
 * 基于json数据创建字段信息
 * @param {*} jobj 
 * @param {*} _head 名称前缀
 * @param {*} _index -1表示顶层，0表示第二层
 */
function dynamicBuildPropertyInfo(jobj, _head, _index) {
    // console.log('build obj:', jobj,' head:', _head);
    if (_head.indexOf('__') == 0) {
        _head = _head.substring(2);
    }
    if (jobj) {
        for (var _name in jobj) {
            var _value = jobj[_name];
            var _set_name = dynamicReadSettingName(`${_head}${_name}`);
            var _property = dynamicFindPropertryFromSetting(_set_name);
            //转化一下，引用类型会被干扰
            if (_property) {
                var _pro_str = JSON.stringify(_property);
                _property = JSON.parse(_pro_str);
            }
            //编辑模式必须和配置匹配
            if (!_property) {
                // if (_config.model != "edit" && _config.model != "add") {
                var _new_pro = dynamicBuildNewProperty(`${_head}${_name}`, _value, _index);
                var _need_push = true;
                var _type = typeof (_value);
                switch (_type) {
                    case "number":
                        {
                            _new_pro.dataType = "number";
                            _new_pro.value = _value;
                        }
                        break;
                    case "string":
                        {
                            _new_pro.dataType = "text";
                            _new_pro.value = _value;
                        }
                        break;
                    case "boolean":
                        {
                            _new_pro.dataType = "switch";
                            _new_pro.value = _value;
                        }
                        break;
                    default: {
                        _need_push = false;
                        if (Array.isArray(_value)) {
                            //是一个数组 这里有问题，未完待续
                            // console.log('is array val:',_value,' name:',_name,' jobj:',jobj);

                            //检查内容类型
                            var _item_is_object = true;
                            for (var ii of _value) {
                                if (typeof (ii) == 'object') {
                                    _item_is_object = true;
                                } else {
                                    _item_is_object = false;
                                    _need_push = true;
                                    if (typeof (ii) == 'number') {
                                        _new_pro.dataType = "Int64[]";
                                    }
                                }
                                break;
                            }
                            if (_item_is_object) {
                                for (var k = 0; k < _value.length; k++) {
                                    var ii = _value[k];
                                    // dynamicBuildPropertyInfo(ii, `${_head}__${_name}__${k}__`, k);
                                    dynamicBuildPropertyInfo(ii, `${dynamicBuildName(_head, _name)}__${k}__`, k);
                                }
                            }
                        } else {
                            // console.log('next object name:', _name,' head:',_head, _value);
                            //可能是下一级
                            // dynamicBuildPropertyInfo(_value, `${_head}__${_name}__`, -1);
                            dynamicBuildPropertyInfo(_value, `${dynamicBuildName(_head, _name)}__`, -1);
                        }
                    }
                }
                if (_need_push) {
                    _global_data_properties.push(_new_pro);
                }
                // }
            } else {
                _property.value = _value;
                // if(!_property.needRemove ){
                dynamicAppendGroupAttribute(_property, dynamicBuildName(_head, _name));
                //这里还要做一个过滤，匹配模式，才添加进去！
                _global_data_properties.push(_property);

                // }
            }
        }
    }
}

/**
 * 
 * @param {*} _head 
 * @param {*} _name 
 */
function dynamicBuildName(_head, _name) {
    if (_head.lastIndexOf('__') == _head.length - 2) {
        return `${_head}${_name}`;
    } else {
        return `${_head}__${_name}`;
    }
}

/**
 * 查找这个属性
 */
function dynamicFindPropertryFromSetting(_name) {
    if (_setting_data_properties) {
        for (var one of _setting_data_properties) {
            if (!one.matchRegex) {
                if (one.name == _name) {
                    return one;
                }
            }
        }

        //基于正则方式查找！
        for (var one of _setting_data_properties) {
            if (one.matchRegex) {
                var _reg = new RegExp(one.name);
                if (_reg.test(_name)) {
                    var _item = JSON.stringify(one);
                    var _item_obj = JSON.parse(_item);
                    _item_obj.name = _name;
                    return _item_obj;
                }
            }
        }
    }
    return null;
}

/**
 * 获取配置名称，示例info__0__name为info__name
 * @param {*} _name 
 * @returns 
 */
function dynamicReadSettingName(_name) {
    var _setting_name = _name;
    if (_name.indexOf("__") >= 0) {
        var strs = _name.split('__');
        var _new_strs = new Array();
        for (var i = 0; i < strs.length; i++) {
            var _str = strs[i];
            var _num = Number(_str);
            if (isNaN(_num)) {
                _new_strs.push(_str);
            }
        }
        _setting_name = _new_strs.join('__');
    }
    return _setting_name;
}

/**
 * 生成一个字段信息
 * @param {*} _name 
 * @param {*} _value 
 * @returns 
 */
function dynamicBuildNewProperty(_name, _value, _index) {
    // var _set_name = dynamicReadSettingName(_name);
    var _info = {
        name: _name,
        title: "",
        value: _value,
        dataType: "text"
    };
    dynamicAppendGroupAttribute(_info, _name);
    if (!_info.title) {
        _info.title = _name;
    }
    return _info;
}

/**
 * 添加分组特性 也有可能不添加，name中有__的才添加
 * @param {*} _info 
 * @param {*} _name 
 */
function dynamicAppendGroupAttribute(_info, _name) {
    if (_name.indexOf("__") > 0) {
        var _la = _name.lastIndexOf('__');
        var _group = _name.substring(0, _la);
        if (!_info.attributes) {
            _info.attributes = new Array();
        }
        _info.attributes.push({
            name: "group",
            args1: _group
        });
        if (!_info.title) {
            _info.title = _name.substring(_la + 2);
        }
    }
}



/*******************************/

/**
 * 配置table的菜单列的宽度，最大500 尽量保持2行
 */
function toolResizeTapMenuWidth() {

    setTimeout(() => {
        if ($(".tapmenu").length > 0) {
            var $tapMenu = $(".tapmenu").eq(0);
            var $buttons = $tapMenu.find("a");
            var buttonCount = $buttons.length;

            if (buttonCount === 0) return;

            // 获取所有按钮的实际宽度
            var buttonWidths = [];
            var totalWidth = 0;
            var maxButtonWidth = 0;

            $buttons.each(function () {
                var btnWidth = $(this).outerWidth(true);
                buttonWidths.push(btnWidth);
                totalWidth += btnWidth;
                if (btnWidth > maxButtonWidth) {
                    maxButtonWidth = btnWidth;
                }
            });

            var targetWidth;
            var maxWidth = 280;
            if (_config) {
                if (_config.menu_max_width) {
                    maxWidth = _config.menu_max_width;
                }
            }
            var minWidth = 120;
            var spacing = 15;//间隔大小

            // 计算单行宽度 56+56+15 = 127 + 15*2 = 157
            var singleRowWidth = totalWidth + spacing * (buttonCount - 1) + spacing * 2;

            // 如果单行能放下
            if (singleRowWidth <= maxWidth) {
                targetWidth = Math.max(singleRowWidth, minWidth);
                //127
            } else {
                // 估算两行宽度
                var twoRowWidth;
                if (buttonCount <= 3) {
                    // 3个按钮时，可能是1+2或2+1布局
                    twoRowWidth = Math.max(
                        maxButtonWidth * Math.ceil(buttonCount / 2),
                        totalWidth / 2
                    ) + spacing + spacing * 2;
                } else {
                    // 4个以上按钮，平均分配
                    var buttonsPerRow = Math.ceil(buttonCount / 2);
                    twoRowWidth = Math.max(
                        maxButtonWidth * buttonsPerRow,
                        totalWidth / 2
                    ) + spacing * (buttonsPerRow - 1) + spacing * 2;
                }

                // 两行方案是否可行（不超过最大宽度）
                if (twoRowWidth <= maxWidth) {
                    targetWidth = Math.max(twoRowWidth, minWidth);
                } else {
                    // 两行也放不下，使用最大宽度
                    targetWidth = maxWidth;
                    console.log('使用最大宽度，按钮将自动换行（可能超过2行）');
                }
            }

            console.log('计算出的目标宽度:', targetWidth);

            function applyWidthToLastTh() {
                var $lastTh = $(".table th:last");
                if ($lastTh.length > 0) {
                    // console.log('找到th:last，应用宽度:', targetWidth);
                    $lastTh.css({
                        "min-width": targetWidth + "px",
                        "width": targetWidth + "px",
                        "max-width": targetWidth + "px"
                    });
                    return true;
                }
                return false;
            }

            // 立即尝试应用
            if (!applyWidthToLastTh()) {
                // 重试机制
                var retryCount = 0;
                var maxRetries = 2; // 更多重试次数

                function retryApply() {
                    retryCount++;
                    console.log('尝试查找动态th:last (第' + retryCount + '次)');

                    if (applyWidthToLastTh()) {
                        console.log('成功应用到动态添加的th');
                    } else if (retryCount < maxRetries) {
                        setTimeout(retryApply, 200); // 每200ms重试一次
                    } else {
                        console.warn('尝试' + maxRetries + '次后仍未找到th:last');
                    }
                }

                setTimeout(retryApply, 200);
            }
        }
    }, 100);

    // setTimeout(() => {
    //     if ($(".tapmenu").length > 0) {
    //         var asize = $(".tapmenu").eq(0).find("a").length;
    //         var to_width = 500;
    //         var tosixe = asize * 80 / 2;
    //         if (tosixe < to_width) {
    //             to_width = tosixe;
    //         }
    //         console.log('计算菜单宽度',to_width);
    //         // $("th:last").width(toWidth);
    //         if (to_width > 100) {
    //             // 使用animate添加动画
    //             // $("th:last").animate({
    //             //     width: toWidth
    //             // }, 50); // 300毫秒的动画时长
    //             // 先设置min-width确保不会被内容撑开
    //             $("th:last").css({
    //                 "min-width": to_width + "px",
    //                 "width": to_width + "px",
    //                 "max-width": to_width + "px"
    //             });
    //             // 再使用animate添加动画
    //             $("th:last").animate({
    //                 width: to_width
    //             }, 50);
    //         }
    //     }
    // }, 100);
}


/**
 * 存储大小美化显示
 * @param {number} bytes - 字节数
 * @param {number} decimals - 小数位数，默认0
 * @returns {string} 美化后的大小字符串
 */
function formatFileSize(bytes, decimals = 0) {
    if (bytes === 0) return '0 B';
    const units = ['B', 'KB', 'MB', 'GB', 'TB'];
    const base = 1024;
    const exponent = Math.floor(Math.log(bytes) / Math.log(base));
    const unitIndex = Math.min(exponent, units.length - 1);
    const size = (bytes / Math.pow(base, unitIndex)).toFixed(decimals);
    return size + units[unitIndex];
}

/**
 * 将美化后的大小字符串转换回字节数
 * @param {string} sizeString - 美化后的大小字符串，如 "79KB", "1.5MB"
 * @returns {number} 字节数
 */
function parseFileSize(sizeString) {
    if (!sizeString || typeof sizeString !== 'string') return 0;
    const units = {
        'B': 1,
        'KB': 1024,
        'MB': 1024 * 1024,
        'GB': 1024 * 1024 * 1024,
        'TB': 1024 * 1024 * 1024 * 1024
    };
    // 匹配数字和单位
    const match = sizeString.match(/^([\d.]+)\s*([KMGTP]?B)$/i);
    if (!match) return 0;
    const size = parseFloat(match[1]);
    const unit = match[2].toUpperCase();
    // 处理KB特殊情况（K和KB都支持）
    const normalizedUnit = unit === 'K' ? 'KB' : unit;
    if (units[normalizedUnit]) {
        return size * units[normalizedUnit];
    }
    return 0;
}

/**
 * 应对Object.assign会改变类型的问题
 * @param {*} target 
 * @param {*} source 
 * @returns 
 */
function PastedeepMerge(target, source) {
    const result = { ...target };

    for (const key in source) {
        // 如果目标字段是数组，且源字段是字符串，则追加到数组
        if (Array.isArray(target[key]) && typeof source[key] === 'string') {
            result[key] = [...target[key], source[key]];
        }
        // 如果目标字段是数组，且源字段也是数组，则合并
        else if (Array.isArray(target[key]) && Array.isArray(source[key])) {
            result[key] = [...target[key], ...source[key]];
        }
        // 其他情况直接覆盖
        else {
            result[key] = source[key];
        }
    }

    return result;
}

/**
 * 打印特定区域 添加css引入 添加延迟
 */
function printTable() {
    const printWindow = window.open('', '_blank');
    const tableWrap = document.querySelector('.table_wrap');

    // 获取所有样式表
    let styles = '';
    for (let styleSheet of document.styleSheets) {
        try {
            for (let rule of styleSheet.cssRules) {
                styles += rule.cssText + '\n';
            }
        } catch (e) {
            console.warn('无法访问某些样式表（可能是跨域限制）');
        }
    }

    const cssLinks = Array.from(document.querySelectorAll('link[rel="stylesheet"]'))
        .map(link => `<link rel="stylesheet" href="${link.href}">`)
        .join('');

    printWindow.document.write(`
        <html>
            <head>
                <title>打印表格</title>
                ${cssLinks}
                <style>
                    @page {
                        size: A4 landscape;
                        margin: 1cm;
                    }
                    ${styles}
                    /* 添加打印专用样式 */
                    @media print {
                        body { margin: 0; padding: 20px; }
                        .no-print { display: none !important; }
                        table { width: 100% !important; }
                    }
                    /* 加载提示样式 */
                    .loading-message {
                        position: fixed;
                        top: 50%;
                        left: 50%;
                        transform: translate(-50%, -50%);
                        background: rgba(0,0,0,0.8);
                        color: white;
                        padding: 20px;
                        border-radius: 5px;
                        z-index: 9999;
                    }
                </style>
            </head>
            <body>
                <div class="loading-message">正在准备打印，请稍候...</div>
                ${tableWrap.outerHTML}
            </body>
        </html>
    `);

    printWindow.document.close();

    // 显示加载提示
    const loadingMessage = printWindow.document.querySelector('.loading-message');

    // 等待 CSS 加载完成
    setTimeout(() => {
        // 移除加载提示
        if (loadingMessage) {
            loadingMessage.remove();
        }

        // 执行打印
        printWindow.print();

        // 可选：打印后自动关闭窗口
        printWindow.onafterprint = function () {
            printWindow.close();
        };

    }, 1000); // 延迟1秒，确保CSS加载完成
}

/**
 * location.search转化成object
 * @param {*} search 
 * @returns 
 */
function parseSearch(search) {
    const params = new URLSearchParams(search);
    const result = {};

    for (const [key, value] of params) {
        // 解析值
        let parsedValue;

        // 检查是否为布尔值
        if (value === 'true' || value === 'false') {
            parsedValue = value === 'true';
        }
        // 检查是否为整数
        else if (/^-?\d+$/.test(value)) {
            parsedValue = parseInt(value, 10);
        }
        // 检查是否为浮点数
        else if (/^-?\d+\.\d+$/.test(value)) {
            parsedValue = parseFloat(value);
        }
        // 默认为字符串
        else {
            parsedValue = value;
        }
        // 处理重复的键
        if (key in result) {
            // 如果已经存在，转换为数组
            if (Array.isArray(result[key])) {
                result[key].push(parsedValue);
            } else {
                result[key] = [result[key], parsedValue];
            }
        } else {
            result[key] = parsedValue;
        }
    }
    return result;
}

/**
 * 清理当前页面的 列排序缓存
 */
function cleanTableColumnOrder() {
    if (typeof global_table_column_order_handler != "undefined") {
        if (global_table_column_order_handler != null) {
            global_table_column_order_handler.cleanOrderCache();
            layer.msg('当前页面列排序缓存已经重置!');
        }
    } else {
        layer.msg('组件未载入，无法执行列排序缓存清理');
    }
}