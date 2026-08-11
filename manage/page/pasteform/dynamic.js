//插件代码的支持
// Ratyli: jquery rating plugin
// version 0.2.1
// https://github.com/peet86/Ratyli
// <span class="ratyli" data-rate="3" data-ratemax="5"></span>    
// (c) 2015 Peter Varga [info@vargapeter.com]
// released under the Apache 2.0 license
(function ($) {
    $.ratyli = function (el, options) {
        var base = this;
        base.$el = $(el);
        base.el = el;

        base.$el.data("ratyli", base);

        base.init = function () {
            // extend with parameters
            base.options = $.extend({}, $.ratyli.defaultOptions, options);

            // extend with html5 dataset
            base.options = $.extend({}, base.options, base.$el.data());

            // for the loop
            base.set(base.options.rate, true);

            // event listeners for rating signs
            base.$el.on("click", "> *", function (e) {
                if (!base.options.disable) {
                    var target = e.target;
                    if (target.tagName != "SPAN") target = target.parentNode;
                    base.options.onSignClick.call(base, target);
                    var val = $(target).prevAll().length + 1;
                    base.set(val);
                }
            });

            base.$el.on("mouseenter", "> *", function (e) {
                var target = e.target;
                if (target.tagName != "SPAN") target = target.parentNode;

                if (!base.options.disable) {
                    $(target).addClass("rate-active");
                    $(target).prevAll().addClass("rate-active");
                }
                base.options.onSignEnter.apply(null, [base.options.rate, target]);
            });

            base.$el.on("mouseleave", "> *", function (e) {
                var target = e.target;
                if (target.tagName != "SPAN") target = target.parentNode;

                if (!base.options.disable) {
                    $(target).removeClass("rate-active");
                    $(target).prevAll().removeClass("rate-active");
                }
                base.options.onSignLeave.apply(null, [base.options.rate, target]);
            });

        };

        base.set = function (val, init) {
            if (val < 0 || (val % 1 != 0) || val > base.options.ratemax) val = 0; // reset to 0.

            if (val == 1 && base.options.rate == 1 && base.options.unrateable == true && !init) {
                val = 0;
            }

            base.options.rate = val;

            // reset html
            base.$el.html("");

            // set data-rate
            if (base.options.rate != 0) base.$el.attr("data-rate", base.options.rate);

            // set data-ratemax
            base.$el.attr("data-ratemax", base.options.ratemax);


            // generate signs
            var i = 0;
            while (i < base.options.ratemax) {
                var tmp = "";
                if (i < base.options.rate) {
                    tmp = base.signTemplate("full");
                } else {
                    tmp = base.signTemplate("empty");
                }
                base.$el.append(tmp);
                i++;
            }

            // set rated
            if (!init && !base.options.disable) {
                base.$el.addClass("rated");
                base.$el.attr("data-rate", val);
            }

            //rated callack
            base.options.onRated.call(base, val, init);

            return base.options.rate;
        };


        base.signTemplate = function (type) {
            return "<span class='rate rate-" + type + "' style='cursor:" + base.options.cursor + ";'>" + base.options[type] + "</span>";
        };

        base.init();

    };

    $.ratyli.defaultOptions = {
        disable: false,
        unrateable: false,
        full: "★",
        empty: "☆",
        cursor: "default",
        rate: 0,
        ratemax: 5,
        onSignEnter: function () { },
        onSignLeave: function () { },
        onSignClick: function () { },
        onRated: function () { }
    };

    $.fn.ratyli = function (options) {
        return this.each(function () {
            (new $.ratyli(this, options));
        });
    };

})(jQuery);

/**
 * 
 */
var _classPath = "";


/**
 * 字段集合
 */
var _global_data_properties = null;//当前模型的属性列表

/**
 * 配置项
 */
var _setting_data_properties = null;

/**
 * 表的属性
 */
var _global_class_attributes = null;

/**
 * 是否需要载入richhtml
 */
var _global_need_loadrichhtml = false;
/**
 * 是否需要载入markdown
 */
var _global_need_loadmarkdown = false;

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
    model: 'view',
    reload: true//执行完成后，是否重新加载父级页面
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

    var _path = _apigetquery("path");
    if (_path) {
        _classPath = _path;
        _config.className = _path;
    } else {
        // console.error("参数错误，请携带参数path,注意大小写！");
        _classPath = "default";
        _config.className = "default";
    }

    var _modelstr = _apigetquery("model");
    if (_modelstr) {
        _config.model = _modelstr;
    }

    // var uidstr = _apigetquery("id");//基于主表，可能有不一样的类型 int long string guid 等
    // if (uidstr != null) {
    //     _id = (uidstr);
    //     if (_id != '0') {
    //         _config.model = 'edit';
    //     } else {
    //         _config.model = 'add';
    //     }
    // } else {
    //     // _config.model = 'add';
    // }

    var _session = _apigetquery("session_key");
    if (_session) {
        var _read = sessionStorage.getItem(_session);
        if (_read) {
            try {
                _readObject = JSON.parse(_read);
            } catch (err) {
                console.log(err);
            }
        }
    }

    var _model = _apigetquery("model");
    if (_model) {
        if (_model == 'select' || _model == 'object') {
            if (parent) {
                if (parent.FuncHiddenSureButtons) {
                    parent.FuncHiddenSureButtons();
                }
            }
        }
    }

    var _unload = _apigetquery("unload");
    if (_unload) {
        _config.reload = false;
    }

    var _datakey_str = _apigetquery('datakey');
    var _dataurl_str = _apigetquery("url");
    if (_datakey_str) {

        var _url = `/api/app/dynamicHelper/read${location.search}`;
        // if(location.search.indexOf("datakey")==-1){
        //     _url=`/api/app/dynamicHelper/view${location.search}`;
        // }

        dynamicReadModel(_url);
        if (window.history.length > 1) {
            $(".tapback").fadeIn();
            $(".tapback").on('click', () => {
                window.history.back();
            })
        }
    } else if (_dataurl_str) {
        var _url = `/api/app/dynamicHelper/view${location.search}`;
        dynamicReadModel(_url);
        if (window.history.length > 1) {
            $(".tapback").fadeIn();
            $(".tapback").on('click', () => {
                window.history.back();
            })
        }
    } else {

        // var _query = encodeURIComponent("/api");
        var _link = `?url=default&model=view`;
        var _url = `/api/app/dynamicHelper/view${_link}`;
        dynamicReadModel(_url);
        if (window.history.length > 1) {
            $(".tapback").fadeIn();
            $(".tapback").on('click', () => {
                window.history.back();
            })
        }
        // _apiget('/api/app/dynamicHelper/main',true,(cc,oo)=>{
        //     if(cc==200){
        //         location.replace(`./index.html?datakey=${oo}&path=default`);
        //     }
        // });
    }
});


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

            if (o.properties) {
                _setting_data_properties = o.properties;
            } else {
                _setting_data_properties = new Array();
            }

            _global_data_properties = new Array();

            //整合数据，整合字段信息用于显示UI
            if (o.data) {
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
 **/
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



/**
 * 提交表单 提交表单的数据，最好前面加一层转化啥的
 **/
function funcSubmitForm() {
    //表单验证
    if (!_beforeValidity("#paste-form")) {
        return;
    }
    var _info = $("#paste-form").parseForm();
    //表单验证的补充
    var _result = FuncHandlerParseFormValue(_info);
    if (!_result) {
        return;
    }

    var info = convertFlattenedToNested(_info);

    //是否需要提交给API
    _apipost(`/api/app/dynamicHelper/update${location.search}`, false, info, (cc, oo) => {
        if (cc == 200) {
            layer.msg('提交成功！');
        }
    });
}

