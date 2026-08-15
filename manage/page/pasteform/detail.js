var _classPath = "";
var _id = "";
/**
 * 字段的属性列表
 */
var _global_data_properties = null;//当前模型的属性列表
/**
 * 表的属性
 */
var _global_class_attributes = null;

/**
 * 
 */
var _config = {
    single_line_length:128//大于多少长度，是独占一行，否则是半行
};

/**
 * 
 */
$(window).ready(function () {

    // if (typeof pasteform_custom_config != 'undefined') {
    //     // _config = pasteform_custom_config;
    //     _config = Object.assign({}, _config, pasteform_custom_config);
    // }

        //全局配置覆盖本地的
        if (typeof pasteform_custom_config != 'undefined') {
            // _config = pasteform_custom_config;
            _config = Object.assign({}, _config, pasteform_custom_config);
        }
    
        //参数配置覆盖本地的
        if (typeof parseSearch != 'undefined') {
            var searchObj = parseSearch(location.search);
            if (searchObj) {
                _config = Object.assign({}, _config, searchObj);
            }
        }

    var _path = _apigetquery("path");
    if (_path) {
        _classPath = _path;
    } else {
        log.console.error("参数错误，请携带参数path,注意大小写！");
    }
    var uidstr = _apigetquery("id");//基于主表，可能有不一样的类型 int long string guid 等
    if (uidstr != null) {
        _id = (uidstr);
    }
    FuncFilexModel();

    bindViewImage();

});

/**
 * 绑定，预览图片
 */
function bindViewImage() {
    $(".newform").on('click', 'img', function () {

        var currentUrl = $(this).attr('src');

        var images = [];
        var currentIndex = 0;
        let zindex = 0;
        $('.newform img').each(function () {
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

        // 找出当前点击的图片索引
        // var index = $('.newform img').index(this);

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
 * 读取数据的模型 处理数据的模型
 * @param {*} properties 
 */
function DetailLoadModelProperity(properties) {

    _global_data_properties = properties;

    // _global_data_properties = properties;
    DetailhandlerExchangeDataTypeToUIType(properties);

    //处理group模式
    _ui_data_list = new Array();
    for (var k = 0; k < _global_data_properties.length; k++) {
        var item = _global_data_properties[k];
        if (item.attributes) {
            var _group = funcFindAttribute(item.attributes, "group");
            if (_group) {
                //是初始化，还是新增一个进去?
                handlerTakeGroupInfo(_group.args1, _ui_data_list, item);
            } else {
                _ui_data_list.push(item);
            }
        } else {
            _ui_data_list.push(item);
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
        }
    }
    // var _template = $("#templatemodel").html();
    // var _ahtml = template(_template, { list: properties });
    // $(".paste-form-body").html(_ahtml);

    //进行多级模板渲染！
    var _template_form = $("#template_form").html();
    var _template_item = $("#template_tr_item").html();
    var _tpl_form = template(_template_form);
    var _tpl_item = template(_template_item);

    //以下是新版本
    var _ahtml = _tpl_form({ list: _ui_data_list, templatetr: _tpl_item, config: _config, tabs: _ui_tabs });
    $(".paste-form-body").html(_ahtml);


    if (_ui_data_list && _ui_data_list.length > 0) {
        setTimeout(() => {
            registry_group_click_event();
        }, 100);
    }
}



/**
 * 转化数据类型，从数据库的类型转化为UI使用的类型
 * @param {*} properties 
 */
function DetailhandlerExchangeDataTypeToUIType(properties) {
    properties.forEach(item => {
        item.dataFrom = item.dataType;
        //这里需要处理字段的属性
        if (item.attributes) {
            item.attributes.forEach(_attribute => {

                switch (_attribute.name) {
                    case 'outer': {
                        item.dataType = 'outer';
                        if (_attribute.args2) {
                            var _target = funcFindPropertry(_attribute.args2);
                            if (_target) {
                                if (_target.value) {
                                    if (typeof _target.value == "object") {
                                        if (_attribute.args4) {
                                            item.value = _target.value[_attribute.args4];
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                    case 'outers': {
                        item.dataType = 'outers';
                        if (_attribute.args2) {
                            var _target = funcFindPropertry(_attribute.args2);
                            if (_target) {
                                if (_target.value) {
                                    item.display = _target.value;
                                }
                            }
                        }
                        item.display_id = _attribute.args3;
                        item.display_name = _attribute.args4;
                        break;
                    }
                    case 'lselect':
                    case 'select':
                    case 'selects':
                    case 'datalist': {
                        try {
                            item.dataType = 'select';
                            item.selects = JSON.parse(_attribute.args1);
                            if (Array.isArray(item.value)) {
                                item.value = item.value.map(val => {
                                    const selectedItem = item.selects.find(ii => ii.value === val);
                                    return selectedItem ? selectedItem.name : val;
                                }).join(',');
                            } else {
                                const selectedItem = item.selects.find(ii => ii.value === item.value);
                                item.value = selectedItem ? selectedItem.name : item.value;
                            }
                        } catch (err) {
                            console.log(item.name, `属性 ${_attribute.name} 的值 args1 配置错误`, err);
                        }
                        break;
                    }
                    case 'hidden': {
                        item.hidden = true;
                        break;
                    }
                    case 'image': {
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
                        if (item.value) {
                            if (item.value.indexOf(',') > 0) {
                                item.value = item.value.split(',');
                            }
                        }
                        break;
                    }
                    case 'head': {
                        item.dataType = 'head';
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
                        break;
                    }
                    case "display":
                        {
                            var _args = "name";
                            if (_attribute.args1) {
                                _args = _attribute.args1;
                            }
                            let _val = item.value;
                            if (_val) {
                                if (typeof _val == "object") {
                                    if (_val[_args]) {
                                        item.value = _val[_args];
                                    }
                                }
                            }
                        }
                        break;
                    case 'daterange': {
                        item.dataType = "daterange";
                        var _start = "";
                        var _end = "";
                        if (_attribute.args1 == item.name) {
                            _start = item.value;
                        } else {
                            if (_attribute.args1) {
                                var _args1obj = funcFindPropertry(_attribute.args2);
                                if (_args1obj) {
                                    if (_args1obj.value) {
                                        _start = _args1obj.value;
                                    }
                                }
                            }
                        }
                        if (_start && _attribute.args2) {
                            var _args1obj = funcFindPropertry(_attribute.args2);
                            if (_args1obj) {
                                if (_args1obj.value) {
                                    _end = _args1obj.value;
                                    var _fulldate = `${new Date(_start).Format('yyyy - MM - dd')}  至  ${new Date(_end).Format('yyyy - MM - dd')}`;
                                    item.value = _fulldate;
                                }
                            }
                        }
                        break;
                    }
                    case 'textarea': {
                        item.dataType = "textarea";
                        item.maxlength = 5000;
                        item.singlerow = true;
                        break;
                    }
                    case 'singlerow': {
                        item.singlerow = true;
                        break;
                    }
                    case "halfrow":
                        {
                            item.singlerow=false;
                            item.halfrow=true;
                        }
                        break;
                    case 'text': {
                        item.dataType = "text";
                        break;
                    }
                    case "unit": {
                        if (_attribute.args1) {
                            item.unit = _attribute.args1;
                        }
                        break;
                    }
                    case "fentoyuan": {
                        if (item.value) {
                            item.value =(parseFloat(item.value) / 100).toFixed(2);// Math.round(parseFloat(item.value) / 100).toFixed(2);
                            item.unit = "元";
                        }
                        item.dataType = "fentoyuan";
                        break;
                    }
                    case "tokm": {
                        if (item.value) {
                            item.value =(parseFloat(item.value) / 1000).toFixed(2);// Math.round(parseFloat(item.value) / 1000).toFixed(2);
                        }
                        item.dataType = "tokm";
                        item.unit = "KM";
                        break;
                    }
                    case "html":
                        {
                            if(_attribute.args1){
                                item.value = handlerMicroString(_attribute.args1);
                                item.dataType="html";
                            }
                        }
                        break;
                    case "sunform":
                        {
                            if (item.value) {
                                item.value = traverse(item.value);
                            }
                            item.dataType = "sunform";
                        }
                        break;
                }


            });
        }

        //如果是故意要暴露id的呢？
        if (item.name == "id") {
            if (!item.required) {
                item.hidden = true;
            }
        }

        switch (item.dataType) {
            case "Int32":
            case "Int64":
            case "UInt32":
            case "UInt64": {
                item.dataType = "number";
                if (item.value === "0") {
                    item.value = "";
                }
                break;
            }
            case "DateTime": {
                item.dataType = "datetime";
                if (item.value && item.value === '01/01/0001 00:00:00') {
                    item.value = "";
                }
                if (!item.format) {
                    item.format = "yyyy-MM-dd HH:mm:ss";
                }
                break;
            }
            case "Boolean": {
                item.dataType = "switch";
                break;
            }
            case "Double": {
                item.step = "0.01";
                if (item.attributes) {
                    item.attributes.forEach(_attribute => {
                        if (_attribute.name === 'double') {
                            item.step = _attribute.args1;
                        }
                    });
                }
                break;
            }
            case "String": {
                if (item.value === null) {
                    item.value = "";
                }
                if (item.maxlength === 0) {
                    item.dataType = "richtext";
                    item.singlerow = true;
                }
                if (item.maxlength >= 128) {
                    item.dataType = "textarea";
                    item.singlerow = true;
                }
                if (item.maxlength < 128 && item.maxlength !== 0) {
                    item.dataType = "text";
                }
                if(item.halfrow){
                    item.singlerow=false;
                }
                break;
            }
            case "image": {
                item.singlerow = true;
                break;
            }
        }

        if (item.hidden) {
            if (item.required) {
                item.required = false;
            }
        }

    });

    _global_data_properties = properties.filter(item => {
        return item.value != null;
    });
}

/**
 * 解析sunform的值
 * @param {*} obj 
 * @returns 
 */
function traverse(obj) {
    let result = "";
    if (Array.isArray(obj)) {
        for (let i = 0; i < obj.length; i++) {
            if (typeof obj[i] === 'object' && obj[i] !== null) {
                result += `${i}: ${traverse(obj[i])}<br>`;
            } else {
                result += `${i}: ${obj[i]}<br>`;
            }
        }
    } else if (typeof obj === 'object' && obj !== null) {
        for (const key in obj) {
            if (obj.hasOwnProperty(key)) {
                result += `${key}: ${obj[key]} `;
            }
        }
    }
    return result;
}

/**
 * 动态对象的配置信息
 */
var _setting_data_properties=null;

/**
 * 读取模型和默认值，值等
 */
function FuncFilexModel() {
    if (!_id) {
        layer.msg(`当前请求需要参数id，当前id值为${_id}，请求失败!`);
        return;
    }
    _apiget(`${auto_path()}/${_id}/readDetailModel${location.search}`, true, (c, o) => {
        if (c == 200) {
            // loadHeader(o);

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

            if (o.attributes) {
                _global_class_attributes = o.attributes;
            }
            // if (o.properties) {
                // DetailLoadModelProperity(o.properties);
            // }
            DetailLoadModelProperity(_global_data_properties);
            if (o.title) {
                this.document.title = "查看" + o.title;
            }
        }
    });
}
