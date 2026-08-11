
//---------------------------------------------------
$.fn.json_serialize = function () {
    var a = this.serializeArray();
    var $radio = $('input[type=radio],input[type=checkbox]', this);
    $.each($radio, function () {
        if ($("input[name='" + this.name + "']").is(':checked')) {
            // console.log(this.name + " is checked!");
            for (var k = 0; k < a.length; k++) {
                if (a[k].name == this.name) {
                    a[k].value = true;
                    break;
                }
            }
        } else {
            var find = false;
            for (var k = 0; k < a.length; k++) {
                if (a[k].name == this.name) {
                    a[k].value = false;
                    find = true;
                    break;
                }
            }
            if (!find) {
                a.push({ name: this.name, value: false });
            }
        }
    });
    var $num = $('input[type=number]', this);
    $.each($num, function () {
        for (var k = 0; k < a.length; k++) {
            if (a[k].name == this.name) {
                a[k].value = Number(a[k].value);
                break;
            }
        }
    });

    var $snum = $('select', this);
    $.each($snum, function () {
        var _needchange = true;
        var _dtype = $($snum).attr('dtype');
        if (_dtype) {
            if (_dtype == 'String') {
                _needchange = false;
            }
        }
        if (_needchange) {
            for (var k = 0; k < a.length; k++) {
                if (a[k].name == this.name) {
                    var val = a[k].value;
                    if (isNumeric(val)) {
                        a[k].value = Number(a[k].value);
                    }
                    break;
                }
            }
        }
    });
    return a;
};

/**
 * 判断是否是数字
 * @param {*} value 
 * @returns 
 */
function isNumeric(value) {
    return !isNaN(parseFloat(value)) && !isNaN(value);
}

$.fn.parseForm = function () {
    let serializeObj = {};
    let array = this.json_serialize();
    //let str = this.serialize();
    $(array).each(function () {
        if (serializeObj[this.name]) {
            if ($.isArray(serializeObj[this.name])) {
                serializeObj[this.name].push(this.value);
            } else {
                serializeObj[this.name] = [serializeObj[this.name], this.value];
            }
        } else {
            serializeObj[this.name] = this.value;
        }
    });
    return serializeObj;
};


//————————————————===================

//---------------md的粘贴图片的支持----------------------------
function initPasteDragImg(Editor) {
    var doc = document.getElementById(Editor.id)
    doc.addEventListener('paste', function (event) {
        var items = (event.clipboardData || window.clipboardData).items;
        var file = null;
        if (items && items.length) {
            // 搜索剪切板items
            for (var i = 0; i < items.length; i++) {
                if (items[i].type.indexOf('image') !== -1) {
                    file = items[i].getAsFile();
                    break;
                }
            }
        } else {
            console.log("当前浏览器不支持");
            return;
        }
        if (!file) {
            console.log("粘贴内容非图片");
            return;
        }
        uploadImg(file, Editor);
    });
    var dashboard = document.getElementById(Editor.id)
    dashboard.addEventListener("dragover", function (e) {
        e.preventDefault()
        e.stopPropagation()
    })
    dashboard.addEventListener("dragenter", function (e) {
        e.preventDefault()
        e.stopPropagation()
    })
    dashboard.addEventListener("drop", function (e) {
        e.preventDefault()
        e.stopPropagation()
        var files = this.files || e.dataTransfer.files;
        uploadImg(files[0], Editor);
    })
}
function uploadImg(file, Editor) {
    var formData = new FormData();
    // var fileName=new Date().getTime()+"."+file.name.split(".").pop();
    formData.append('file', file, file.name);
    $.ajax({
        "type": 'POST',
        "url": Editor.settings.imageUploadURL,//获取我们配置的url
        "data": formData,
        "dateType": "json",
        "headers": {
            "token": readToken()
        },
        "processData": false,
        "contentType": false,
        "mimeType": "multipart/form-data",
        success: function (ret) {
            var json = $.parseJSON(ret);
            if (json.errno == 0) {
                var url = json.data[0].url;
                // var type = json.type; 
                // if(/(png|jpg|jpeg|gif|bmp|ico|image)/.test(url)){
                Editor.insertValue("![图片alt](" + url + " ''图片title'')");
                // }else{
                // Editor.insertValue("[下载附件]("+url+")");
                // }    
            } else {
                // alert("上传失败");
                layer.msg(json.message);
            }
        },
        error: function (err) {
            console.log('请求失败', err)
        }
    });
}
//===========================================


var _waitloading = 500;

/**
 * 自行拼接url的方式请求查询
 * @param {*} httpurl 完整的url，获取没有包含域名信息
 * @param {*} async 是否使用异步请求
 * @param {*} callback 
 */
function _apiget(httpurl, async, callback) {

    var session_data = SessionReadModel(httpurl);
    if (session_data) {
        if (typeof (callback) == "function") {
            callback(200, JSON.parse(session_data));
        }
        return;
    }

    var loadid = 0;
    var ajaxcompleted = false;
    var tickevent = function () {
        if (!ajaxcompleted) {
            if (typeof (layer) != "undefined") {
                loadid = layer.load();
            }
        }
    }
    setTimeout(tickevent, _waitloading);

    $.ajax({
        //提交数据的类型 POST GET
        type: "GET",
        //提交的网址
        url: httpurl,
        async: async,
        //提交的数据
        //data: postdata,
        //返回数据的格式
        datatype: "jsonp",//"xml", "html", "script", "json", "jsonp", "text".
        headers: {
            "token": readToken()
        },
        //在请求之前调用的函数
        beforeSend: function (request) {
        },
        //成功返回之后调用的函数
        success: function (data) {
            //console.log("执行到了success");
        },
        //调用执行后调用的函数
        complete: function (XMLHttpRequest, textStatus) {
            ajaxcompleted = true;

            if (XMLHttpRequest.status == 200) {
                SessionSaveModel(httpurl, XMLHttpRequest.responseText);
            }
            _funccallback(XMLHttpRequest, loadid, callback);

        },
        //调用出错执行的函数
        error: function () {
            //请求出错处理
            //console.log("执行到了error");
        }
    });
}

/**
 * 使用query的方式传递查询参数
 * @param {*} url 没有?的url串
 * @param {*} _async 是否异步方式请求
 * @param {*} obj 查询的对象，比如{page:1,size:20}
 * @param {*} callback 回调函数
 */
function _apiquery(url, _async, obj, callback) {
    var loadid = 0;
    var ajaxcompleted = false;
    var tickevent = function () {
        if (!ajaxcompleted) {
            if (typeof (layer) != "undefined") {
                loadid = layer.load();
            }
        }
    }
    setTimeout(tickevent, _waitloading);
    if (obj != null) {
        // var _querystr = (new URLSearchParams(obj)).toString();
        var params = new URLSearchParams();
        for (var k in obj) {
            var _val = obj[k];
            //防止0不回传，这样可能会命中后台的非0默认值
            if (_val || _val === 0) {
                if (typeof _val == 'object' && Array.isArray(_val)) {
                    _val.forEach(iv => {
                        params.append(k, iv);
                    });
                } else {
                    params.set(k, _val);
                }
            }
        }
        url = url + "?" + params.toString();
    }
    $.ajax({
        //提交数据的类型 POST GET
        type: "GET",
        //提交的网址
        url: url,
        async: _async,
        //返回数据的格式
        datatype: "jsonp",//"xml", "html", "script", "json", "jsonp", "text".
        headers: { "token": readToken() },
        //在请求之前调用的函数
        beforeSend: function (request) {
        },
        //成功返回之后调用的函数
        success: function (data) {
        },
        //调用执行后调用的函数
        complete: function (XMLHttpRequest, textStatus) {
            // console.log("complete");
            ajaxcompleted = true;
            _funccallback(XMLHttpRequest, loadid, callback);
        },
        //调用出错执行的函数
        error: function () {
            //请求出错处理
            console.log("error");
        }
    });
}

/**
 * 
 * @param {*} httpurl 完整的请求url
 * @param {*} async 
 * @param {*} data 
 * @param {*} callback 
 */
function _apipost(httpurl, async, data, callback) {
    var loadid = 0;
    var ajaxcompleted = false;
    var tickevent = function () {
        if (!ajaxcompleted) {
            if (typeof (layer) != "undefined") {
                loadid = layer.load();
            }
        }
    }
    setTimeout(tickevent, _waitloading);
    if (typeof data != "string") {
        if (data != null) {
            data = JSON.stringify(data);
        }
    }
    $.ajax({
        //提交数据的类型 POST GET
        type: "POST",
        //提交的网址
        url: httpurl,
        async: async,
        //提交的数据
        data: data,
        //返回数据的格式
        datatype: "jsonp",//"xml", "html", "script", "json", "jsonp", "text".
        headers: {
            "token": readToken()
        },
        contentType: "application/json; charset=UTF-8",
        //在请求之前调用的函数
        beforeSend: function (request) {
            //request.setRequestHeader("order", order);
        },
        //成功返回之后调用的函数
        success: function (data) {
            //console.log("执行到了success");
        },
        //调用执行后调用的函数
        complete: function (XMLHttpRequest, textStatus) {
            //console.log(XMLHttpRequest);
            ajaxcompleted = true;

            _funccallback(XMLHttpRequest, loadid, callback);

        },
        //调用出错执行的函数
        error: function () {
            //请求出错处理
            //console.log("执行到了error");
        }
    });
}

/**
 * 封装的post
 * @param {*} url 
 * @param {*} data 
 * @returns code info
 */
function apipostPromise(url, data) {
    return new Promise((resolve, reject) => {
        _apipost(url, true, data, (code, response) => {
            resolve({ code: code, info: response });
        });
    });
}

/**
 * _apiquery的Prosime的封装
 * @param {*} url 
 * @param {*} data 
 * @returns code info
 */
function apiqueryPromise(url, data) {
    return new Promise((resolve, reject) => {
        _apiquery(url, true, data, (code, response) => {
            resolve({ code: code, info: response });
        });
    });
}

/**
 * _apiget和_apiquery的封装
 * @param {*} url 
 * @param {*} queryObj 
 * @returns 
 */
function apigetPromise(url, queryObj) {
    return new Promise((resolve, reject) => {
        if (queryObj) {
            _apiquery(url, true, queryObj, (code, response) => {
                resolve({ code: code, info: response });
            })
        } else {
            _apiget(url, true, (code, response) => {
                resolve({ code: code, info: response });
            });
        }
    });
}

/**
 * 
 * @param {*} path 
 * @returns 
 */
function ReadPathName(path) {
    return path;
    var baseUrl = new URL(window.location.href);
    const url = new URL(path, baseUrl);
    var path_name = url.pathname;
    var model = _filterquery("model", path);
    if (model != null) {
        pathname = pathname + '_' + model;
    }
    return path_name;
}

/**
 * 
 * @param {*} path 
 * @param {*} data 
 */
function SessionSaveModel(path, data) {
    var path_name = ReadPathName(path);
    if (path_name.endsWith("readAddModel") || path_name.endsWith("readListModel")) {
        sessionStorage.setItem(path_name, data);
    }
}

/**
 * 
 * @param {*} path 
 * @param {*} data 
 * @returns 
 */
function SessionReadModel(path) {
    var path_name = ReadPathName(path);
    if (path_name.endsWith("readAddModel") || path_name.endsWith("readListModel")) {

        var _data = sessionStorage.getItem(path_name);
        if (_data) {
            return _data;
        } else {
            return null;
        }
    }
}

/**
 * 
 * @param {*} key 
 * @returns 
 */
function _filterquery(key, url) {
    // 获取参数
    // var url = window.location.search;
    // 正则筛选地址栏
    var reg = new RegExp("(^|&)" + key + "=([^&]*)(&|$)");
    // 匹配目标参数
    var result = url.substr(1).match(reg);
    //返回参数值
    return result ? decodeURIComponent(result[2]) : null;
}

/**
 * 
 * @param {*} XMLHttpRequest 
 * @param {*} loadid 
 * @param {*} callback 
 */
function _funccallback(XMLHttpRequest, loadid, callback) {
    var needCallBack = true;
    var responsestr = XMLHttpRequest.responseText;
    var responsecode = XMLHttpRequest.status;
    if (responsecode == 401) {
        needCallBack = false;
        if (typeof (layer) != "undefined") {
            layer.msg("当前操作需要登录！", { time: 1500 }, function () {
                window.parent.location.href = "/page/login/index.html?t=" + (new Date()).getTime();
            });
        } else {
            window.parent.location.href = "/page/login/index.html?t=" + (new Date()).getTime();
        }
    } else if (responsecode == 403 || responsecode == 500 || responsecode == 400 || responsecode == 700 || responsecode == 630) {
        needCallBack = false;
        try {
            if (responsestr.indexOf("{") >= 0 || responsestr.indexOf("[") >= 0) {
                var _info = JSON.parse(responsestr);
                if (_info.message) {
                    layer.msg(_info.message);
                } else {
                    layer.msg(responsestr);
                }
                // callback(responsecode, _info);
            } else {
                // callback(responsecode, (responsestr));
                layer.msg("当前无权限进行此操作");
            }
        } catch (err) {
            console.log(err);
            layer.msg(err.message);
            // callback(responsecode, (responsestr));
        }
        // layer.msg(XMLHttpRequest.responseText);
    } else if (responsecode == 404) {
        // console.log(XMLHttpRequest);
        needCallBack = false;
        layer.msg("请求的资源不存在");
    }
    // if (needCallBack) {
    try {
        if (responsestr.indexOf("{") >= 0 || responsestr.indexOf("[") >= 0) {
            callback(responsecode, JSON.parse(responsestr));
        } else {
            callback(responsecode, (responsestr));
        }
    } catch (err) {
        console.log(err);
        callback(responsecode, (responsestr));
    }
    // }
    if (loadid > 0) {
        if (typeof (layer) != "undefined") {
            layer.close(loadid);
        }
    }
}

/**
 * 
 * @param {*} key 
 * @returns 
 */
function _apigetquery(key) {
    // 获取参数
    var url = window.location.search;
    // 正则筛选地址栏
    var reg = new RegExp("(^|&)" + key + "=([^&]*)(&|$)");
    // 匹配目标参数
    var result = url.substr(1).match(reg);
    //返回参数值
    return result ? decodeURIComponent(result[2]) : null;
}

/**
 * 
 * @returns 
 */
function readToken() {
    return _locReadData("webtoken");
}
/**
 * 
 * @param {*} sval 
 */
function saveToken(sval) {
    if (sval.length > 0) {
        _locSaveData("webtoken", sval);
    } else {
        localStorage.removeItem("webtoken");
    }
}

function _locSaveData(skey, sval) {
    localStorage.setItem(skey, sval);
}

function _locReadData(skey) {
    return localStorage.getItem(skey);
}

//=====================================

/**
 * 打开编辑页面
 * @param {any} title
 * @param {any} url
 * @param {any} id
 * @param {any} w
 * @param {any} h
 */
function _showedit(title, url, id, w, h) {

    if (url.indexOf('?') > 0) {
        layer_show(title, url + "&id=" + id, w, h);
    } else {
        layer_show(title, url + "?id=" + id, w, h);
    }


}

/**
 * 
 * @param {*} title 
 * @param {*} url 
 * @param {*} w 
 * @param {*} h 
 */
function _showwindow(title, url, w, h) {
    layer_show(title, url, w, h);
}

/**
 * 打开新增页面
 * @param {any} title
 * @param {any} url
 * @param {any} w
 * @param {any} h
 */
function _showadd(title, url, w, h) {
    layer_show(title, url, w, h);
}

//Array的扩展
Array.prototype.indexOf = function (val) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == val) return i;
    }
    return -1;
};

Array.prototype.remove = function (val) {
    var index = this.indexOf(val);
    if (index > -1) {
        this.splice(index, 1);
    }
};

Date.prototype.Format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份
        "d+": this.getDate(), //日
        "H+": this.getHours(), //小时
        "m+": this.getMinutes(), //分
        "s+": this.getSeconds(), //秒
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度
        "S": this.getMilliseconds() //毫秒
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

String.prototype.Format = function (fmt) {
    var date = new Date(this.replace(/-/g, '/'));
    var o = {
        "M+": date.getMonth() + 1, //月份
        "d+": date.getDate(), //日
        "H+": date.getHours(), //小时
        "m+": date.getMinutes(), //分
        "s+": date.getSeconds(), //秒
        "q+": Math.floor((date.getMonth() + 3) / 3), //季度
        "S": date.getMilliseconds() //毫秒
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}


function isPicture(name) {
    //判断是否是图片 - strFilter必须是小写列举
    var strFilter = ".jpeg|.gif|.jpg|.png|.bmp|.pic|.webp|"
    if (name.indexOf(".") > -1) {
        var p = name.lastIndexOf(".");
        var strPostfix = name.substring(p, name.length) + '|';
        strPostfix = strPostfix.toLowerCase();
        if (strFilter.indexOf(strPostfix) > -1) { return true; }
    }
    return false;
}


/**
 * 前端验证
 * @param {*} elcselect 
 * @returns 
 */
function _beforeValidity(elcselect) {
    var findvalid = true;
    var fileds = $(elcselect)[0].querySelectorAll(":invalid");
    for (var k = 0; k < fileds.length; k++) {
        var namestr = fileds[k].name;
        var valinfo = fileds[k].validity;
        if (!valinfo.valid) {
            findvalid = false;
            //如果这个UI是主动被隐藏的，则不校验整个规则 但是这个有一个问题，后端会校验...
            var _hidden = $(`[name=${namestr}]`).closest('.hidden');
            if (_hidden) {
                return true;
            }
            if (valinfo.valueMissing) {
                $(elcselect + " [name=" + namestr + "]").focus();
                layer.tips('当前项必须填写' + namestr, elcselect + " [name=" + namestr + "]", { tipsMore: true }, 1);
            }
            if (valinfo.typeMismatch) {
                $(elcselect + " [name=" + namestr + "]").focus();
                layer.tips('输入的数据类型不匹配，请按需输入', elcselect + " [name=" + namestr + "]", { tipsMore: true }, 1);
            }

            if (valinfo.patternMismatch) {
                $(elcselect + " [name=" + namestr + "]").focus();
                layer.tips('输入的数据格式不匹配，请按需输入', elcselect + " [name=" + namestr + "]", { tipsMore: true }, 1);
            }

            if (valinfo.tooLong) {
                $(elcselect + " [name=" + namestr + "]").focus();
                layer.tips('输入的数据位数过大，请按需输入', elcselect + " [name=" + namestr + "]", { tipsMore: true }, 1);
            }
            if (valinfo.tooShort) {
                $(elcselect + " [name=" + namestr + "]").focus();
                layer.tips('输入的数据位数不足，请按需输入', elcselect + " [name=" + namestr + "]", { tipsMore: true }, 1);
            }

            if (valinfo.rangeOverflow) {
                $(elcselect + " [name=" + namestr + "]").focus();
                layer.tips('输入的数据过大，请按需输入', elcselect + " [name=" + namestr + "]", { tipsMore: true }, 1);
            }

            if (valinfo.rangeUnderflow) {
                $(elcselect + " [name=" + namestr + "]").focus();
                layer.tips('输入的数据过小，请按需输入', elcselect + " [name=" + namestr + "]", { tipsMore: true }, 1);
            }
        }
    }
    return findvalid;
}


var _mark_window = null;

/**
 * 打开书签 某一个模块的某一项
 * @param {*} _module 
 * @param {*} _field 
 */
async function global_tap_mark(_module, _field, _style, elc) {

    //暂未实现，请先到文档中心创建项目后再试
    console.log('功能未实现 global_tap_mark ../lib/api.js line 603  !，可以参考贴代码文档，以下是案例');
    var apiRoute = null;// /api/doc/read/{project}/{module}/{field}
    if (_style == "popup" && apiRoute != null) {
        //popup模式？？？
        var read = await apigetPromise(apiRoute);
        if (read.code == 200) {
            layer.tips(read.info, $(elc), { tipsMore: true }, 1);
            return;
        }
    }

    //layer.tips('输入的数据类型不匹配，请按需输入', $(elc), { tipsMore: true }, 1);


    var _project = "form";
    var _url = `https://doc.pastecode.cn/doc/${_project}/${_module}/${_field}#${_project}-${_module}-${_field}`;
    if (_mark_window && !_mark_window.closed) {
        _mark_window.location.href = _url;
        _mark_window.focus();
    } else {
        _mark_window = window.open(_url);
    }
}

/**
 * 点击动态表单的快链进行跳转
 * @param {*} elc 
 */
function global_tap_link(elc) {
    var _url = $(elc).val();
    var _u = encodeURIComponent(_url);
    // var _href =`/api/app/dynamicHelper/view?url=${_u}`;
    // location.href = _href;

    var _href = `./index.html?url=${_u}`;
    location.href = _href;

    // var _read =_apiget(`/api/app/dynamicHelper/view?url=${_u}`,true,(cc,oo)=>{
    //     if(cc==200){
    //         location.href=`./index.html?datakey=${oo}&path=default`;
    //     }
    // });
}

/**
 * 基于表名和ID获取编辑的url
 * @param {*} _table 
 * @param {*} _id 
 */
function global_edit_view(_title, _table, _id) {
    var _url = "";
    switch (_table) {
        case "pool":
        case "iis_pool":
            {
                _url = `/api/webserver/application-pools/${_id}`;
            }
            break;
        case "website":
        case "iis_website":
            {
                _url = `/api/webserver/websites/${_id}`;
            }
            break;
    }
    if (_url) {
        var _href = encodeURIComponent(_url);
        open_window(_title, `../dynamicform/index.html?url=${_href}&model=edit`, '1100px', '90%');
    }
}

/**
 * 以原始模式查看动态json表单
 * @param {*} _title 
 * @param {*} _table 
 * @param {*} _id 
 */
function global_dynamic_view(_title, _table, _id) {
    var _url = "";
    switch (_table) {
        case "pool":
        case "iis_pool":
            {
                _url = `/api/webserver/application-pools/${_id}`;
            }
            break;
        case "website":
        case "iis_website":
            {
                _url = `/api/webserver/websites/${_id}`;
            }
            break;
    }
    if (_url) {
        var _href = encodeURIComponent(_url);
        open_window(_title, `../dynamicform/index.html?url=${_href}&model=view`, '1100px', '90%');
    }
}

/**
 * 基于表名和ID获取编辑的url
 * @param {*} _table 
 * @param {*} _id 
 */
function global_dynamic_add(_title, _table) {
    var _url = "";
    switch (_table) {
        case "pool":
        case "iis_pool":
            {
                _url = `/api/webserver/application-pools`;
            }
            break;
        case "website":
        case "iis_website":
            {
                _url = `/api/webserver/websites`;
            }
            break;
    }
    if (_url) {
        var _href = encodeURIComponent(_url);
        open_window(_title, `../dynamicform/index.html?url=${_href}&model=add`, '1100px', '90%');
    }
}

/**
 * 选择一个程序池 为站点更换程序池
 * @param {*} websiteid 
 */
function global_change_pool(websiteid) {
    funcOuterOpenSelect(`选择${find.title}`, 'iis_pool', '', (obj) => {
        if (obj) {
            var _info = {
                id: obj.id,
                extend_website_id: websiteid
            };
            console.log(_info);
            _apipost('/api/app/iis_wpool/updateItem', false, _info, (c, o) => {
                if (c == 200) {
                    layer.msg(o);
                }
            });
        }
    });
}

/**
 * 备份权限列表
 */
function global_role_output() {
    _apipost(`/api/app/roleInfo/backup`, true, '', (c, o) => {
        if (c == 200) {
            layer.msg('已执行保存到backup/roles.json');
        }
    });
}
/**
 * 重载权限列表
 */
function global_role_default() {
    layer.confirm('重新载入，会重置角色绑定的权限，超级角色root除外！是否继续', { btn: ["确定", "取消"] }, function (index) {
        _apipost(`/api/app/roleInfo/load`, true, '', (c, o) => {
            layer.close(index);
            if (c == 200) {
                layer.msg('已执行，正在重新载入数据！');
                location.reload();
            }
        });
    }, function () {
    });
}

/**
 * 默认得post请求 无参模式
 * @param {*} url 
 */
async function func_api_post(url) {
    var read = await apipostPromise(url, '');
    if (read.code == 200) {
        layer.msg(read.info);
    }
}

/**
 * 请求并基于返回更新
 * @param {*} url 
 */
async function func_api_post_update_row(url) {
    var read = await apipostPromise(url, '');
    if (read.code == 200) {
        if (typeof read.info == 'string') {
            layer.msg(read.info);
        } else {
            if (typeof handlerUpdateRow == "function") {
                handlerUpdateRow(read.info);
            }
        }
    }
}

/**
 * 默认得get形式 无参形式
 * @param {*} url 
 */
async function func_api_get(url) {
    var read = await apigetPromise(url);
    if (read.code == 200) {
        layer.msg(read.info);
    }
}

$(window).ready(function () {
    //新版本中form中有做这个了
    $(".table tbody").on('click', 'tr', function () {
        $(this).addClass('hitline').siblings().removeClass('hitline');
    })
});

//自定义配置
var pasteform_custom_config = {
    className: '',//需要和路径的一致，表示后端的对象
    // model: 'view',//当前模式view bind select
    menubox: false,//是否有按钮盒子
    enable_view: false,
    next: null,
    show_type: 'table',//table表格显示chart图表显示
    ui_model: false,//是否被激活为table或者chart模式
    exchange_id: "id",//主键字段，默认为id
    show_key: false,//是否显示默认的id这个字段，也就是表的主键，有时候不想显示
    single_select: true,//选择模式下，是否是单选模式，默认是单选模式
    select_num: 1,//选中多少个后确定
    menu_max_width: 480,//菜单的最大宽度，默认480px
    root_path: "api/app",//后端根地址
    page_size: [20, 50, 100, 200],//分页的大小
    page_default_size: 20,//默认的分页大小
    batch: false//是否启用批量操作
};