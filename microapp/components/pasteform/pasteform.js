const app = getApp();
Component({
  options: {
    addGlobalClass: true
  },
  /**
   * 组件的属性列表
   */
  properties: {

  },

  /**
   * 组件的初始数据
   */
  data: {
    modelData: [],
    config: {
      url: "https://www.abc.com/api/app/Upload/Image",
      host: "https://www.abc.com",
      header: {
        token: '1233'
      },
      num: 3
    },
    imgs: [],//用于封面选中
    imgindex: -1,
    showButton: false,
    modelInfo: {},
    datetimePickerShow: false,
    datetimePickerDate: '2024-06-06 12:30:30',
    datetimePickerName: 'picker_name',
    dateplanPickerShow: false,
    dateplanPickerDate: '2024-06-06 上午',
    dateplanPickerName: 'picker_plan',
    linkQuery:{},
    _options:{}
  },
  // 生命周期函数，可以为函数，或一个在methods段中定义的方法名
  attached: function () {
    if (app && app.globalData) {
      if (app.globalData.formconfig) {
        this.data.config = app.globalData.formconfig;
      }
    } else {
      console.log('可以在app.globalData.formconfig配置上传图片的配置');
    }
  },
  /**
   * 组件的方法列表
   */
  methods: {
    handlerBlur(e) {
      var _value = (e.detail.value);
      var _name = e.currentTarget.id;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var ione = _current.item;
        if (_value) {
          if (ione.attributes) {
            for (var _attribute of ione.attributes) {
              if (_attribute.name == "pattern") {
                var _reg = new RegExp(_attribute.args1);
                if (!_reg.test(_value)) {
                  _value = "";
                  ione.error = _attribute.error || "输入不符合规范要求，请重新输入";
                }
              }
            }
          }

        }
        ione.value = _value;
        ione.currentlength = _value.length;
        this.funcSetDataItem(_current.zindex, ione);
      }
    },
    handlerTouch(e) {
      var _name = e.currentTarget.id;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var ione = _current.item;
        ione.error = "";
        this.funcSetDataItem(_current.zindex, ione);
      }
    },
    handlerRadioChange(e) {
      var _zindex = parseInt(e.detail.value);
      var _name = e.currentTarget.id;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var ione = _current.item;
        ione.display = ione.array[_zindex];
        ione.value = ione.items[_zindex].value;
        this.funcSetDataItem(_current.zindex, ione);
      }
    },
    handlerAreaInput(e) {
      var _value = (e.detail.value);
      var _name = e.currentTarget.id;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var ione = _current.item;
        ione.value = _value;
        ione.currentlength = _value.length;
        if (ione.maxlength) {
          if (_value.length > ione.maxlength) {
            _value = _value.slice(0, ione.maxlength);
          }
        }
        this.funcSetDataItemColumn(_current.zindex, "currentlength", ione.currentlength);
        // this.funcSetDataItem(_current.zindex, ione);
      }
    },
    checkboxChange(e) {
      // console.log(e);
      var _value = (e.detail.value);
      var _name = e.currentTarget.id;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var ione = _current.item;
        if (ione.items) {
          for (var vi of ione.items) {
            if (_value.indexOf(vi.name) >= 0) {
              vi.checked = true;
            } else {
              vi.checked = false;
            }
          }
        }
        ione.value = _value;
        this.funcSetDataItem(_current.zindex, ione);
      }
    },
    handlerSwitch(e) {
      var _value = (e.detail.value);
      var _name = e.currentTarget.id;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var ione = _current.item;
        ione.value = _value;
        // this.funcSetDateItem(_current.zindex,ione);
      }
    },
    handlerPickerChange(e) {
      var _value = (e.detail.value);
      var _name = e.currentTarget.id;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var ione = _current.item;
        ione.display = _value;
        if (ione.dataType == 'time') {
          _value = `${_value}:00`;
        }
        if (typeof _value != "string") {
          ione.display = _value.toString();
        }
        ione.value = _value;
        this.funcSetDataItem(_current.zindex, ione);
      }
    },
    handlerSelectedDateTime(e) {
      console.log(e);
      var _name = e.detail.name;
      var _value = e.detail.date;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var ione = _current.item;
        ione.value = _value;
        this.funcSetDataItem(_current.zindex, ione);
      }
    },
    handlerSelectedDatePlan(e) {
      console.log(e);
      var _name = e.detail.name;
      var _value = e.detail.date;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var ione = _current.item;
        ione.value = _value;
        this.funcSetDataItem(_current.zindex, ione);
      }
    },
    handlerStartDateTimePicker(e) {
      var _value = (e.detail.value);
      var _name = e.currentTarget.id;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var ione = _current.item;
        this.setData(
          {
            datetimePickerName: _name,
            datetimePickerShow: true,
            datetimePickerDate: ione.value
          }
        );
        // this.funcSetDateItem(_current.zindex, ione);
      }
    },
    handlerStartDatePlanPicker(e) {
      var _value = (e.detail.value);
      var _name = e.currentTarget.id;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var ione = _current.item;
        this.setData(
          {
            dateplanPickerName: _name,
            dateplanPickerShow: true,
            dateplanPickerDate: ione.value
          }
        );
        // this.funcSetDateItem(_current.zindex, ione);
      }
    },
    richHandlerInsertImage(e) {
      //上传图片，然后传入到富文本中
      var _name = e.currentTarget.id;
      var _richText = this.selectComponent(`#${_name}`);
      if (_richText) {
        //准备发送图片
        this.uploadImage(1, (data) => {
          // if (data.errno == 0) {
          //这里不对劲 转化成组组
          if (data.errno == 0) {
            var _imgurl = data.data[0].url;
            _richText.insertImageMethod(_imgurl);
          }

          // }
        }, (err) => {
          console.log(err);
        }, () => {
        });
      }

    },
    richHandlerReady(e) {
      console.log('rich ready', e);
      var _name = e.currentTarget.id;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var item = _current.item;
        // console.log(item);
        if (item.value) {
          var _richText = this.selectComponent(`#${_name}`);
          if (_richText) {
            _richText.FuncSetBody(item.value);
          } else {
            console.log('没有找到这个组件？');
          }
        }
      }
    },
    btnPrimary(e) {
      //校验输入是否合法，
      var _info = {};
      for (var k = 0; k < this.data.modelData.length; k++) {
        var item = this.data.modelData[k];
        if (item.required) {
          if (!item.value) {
            item.focus = true;
            wx.showToast({
              title: `${item.title} 的值不能为空，请重新输入`,
              icon: 'none'
            })
            this.funcSetDataItem(k, item)
            return;
          }
        }
        _info[item.name] = item.value;
      }
      console.log(_info);
      //执行业务函数
    },
    uploadImage(num, success, failed, complated) {
      var _config = this.data.config;
      var url = _config.url;
      var header = _config.header;
      let myHeader = {
        'Accept': 'application/json',
        'Content-Type': 'multipart/form-data',
      };
      if (header) {
        myHeader = { ...header };
      }
      wx.chooseImage({
        count: num,
        success: (res) => {
          wx.showLoading({
            title: '上传中...'
          });
          let tempFilePaths = res.tempFilePaths;
          for (var i = 0; i < tempFilePaths.length; i++) {
            wx.uploadFile({
              url: url,
              filePath: tempFilePaths[i],
              fileType: 'image',
              name: 'file',
              header: myHeader,
              success: (uploadFileRes) => {
                console.log(uploadFileRes);
                var _data = JSON.parse(uploadFileRes.data);
                success(_data);
                //格式错误 需要处理
                if (_data.errno == 0) {
                  var _url = _data.data[0].url;
                  if (this.data.imgs.indexOf(_url) == -1) {
                    this.data.imgs.push(_url);
                    if (this.data.imgindex == -1) {
                      this.data.imgindex = 0;
                    }
                    this.setData({
                      imgs: this.data.imgs,
                      imgindex: this.data.imgindex
                    });
                  }
                }
              },
              fail: (error) => {
                console.log(error);
                if (error.message) {
                  wx.showToast({
                    title: error.message,
                    icon: "none"
                  })
                }
                if (failed) {
                  failed(error);
                }
              },
              complete: () => {
                setTimeout(function () {
                  wx.hideLoading();
                },
                  250);
                if (complated) {
                  complated();
                }
              },
            });
          }
        },
        fail: (err) => {
          if (err) {
            failed(err);
          }
        }
      });
    },
    /**
     * 微信选择地址
     * @param {}} e 
     */
    handlerChooseLocation(e) {
      console.log('选择施工地点');
      var _name = e.currentTarget.id;
      wx.chooseLocation({
        success(res) {
          var _current = this.funcFindItemByName(_name);
          if (_current.zindex >= 0) {
            _current.value = res;
            var _attribute = this.findAttributeByName(_current.attributes, "location");
            if (_attribute) {
              if (_attribute.args1) {
                var _c = this.funcFindItemByName(_attribute.args1);
                if (_c.zindex >= 0) {
                  this.funcSetDataItemColumn(_c.zindex, "value", res.name)
                }
              }
              if (_attribute.args2) {
                var _c = this.funcFindItemByName(_attribute.args2);
                if (_c.zindex >= 0) {
                  this.funcSetDataItemColumn(_c.zindex, "value", res.address)
                }
              }
              if (_attribute.args3) {
                var _c = this.funcFindItemByName(_attribute.args3);
                if (_c.zindex >= 0) {
                  this.funcSetDataItemColumn(_c.zindex, "value", res.latitude)
                }
              }
              if (_attribute.args4) {
                var _c = this.funcFindItemByName(_attribute.args4);
                if (_c.zindex >= 0) {
                  this.funcSetDataItemColumn(_c.zindex, "value", res.longitude)
                }
              }
            }
            //如果用户要稍微调整呢？？？
            //查看args1是否有值? args2 args3
          }
        }, fail(err) {
          console.log(err);
        }
      })
    },
    handlerUploadImage(e) {
      var _config = this.data.config;
      var _name = e.currentTarget.id;
      // console.log(e);
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var item = _current.item;
        this.uploadImage(_config.num, (data) => {
          if (data.errno == 0) {
            //这里不对劲 转化成组组
            if (item.dataType == "image" && item.num > 1) {
              if (!item.value) {
                item.value = new Array();
              }
              for (var _img of data.data) {
                item.value.push(_img.url);
              }
              //这里要把历史的弄过来 由最后的数据校验结果的时候检查，是否是数组还是字符串
            } else {
              item.value = `${data.data[0].url}`;// this.data.config.host data.data[0].url
            }
            this.funcSetDataItem(_current.zindex, item)
          }
        }, (err) => {
          console.log(err);
        }, () => {
        });
      }
    },
    handlerRemoveImage(e) {
      console.log(e);
      var _name = e.currentTarget.dataset.name;
      var _current = this.funcFindItemByName(_name);
      if (_current.zindex >= 0) {
        var item = _current.item;
        var _index = e.currentTarget.dataset.index;
        //移除，然后更新
        if (item.value && item.value.length > _index) {
          item.value.splice(_index, 1);
          this.funcSetDataItem(_current.zindex, item)
        }
      }
    },
    /**
     * 获取某一项
     * @param {*} _name 
     */
    funcFindItemByName(_name) {
      for (var k = 0; k < this.data.modelData.length; k++) {
        var ione = this.data.modelData[k];
        if (ione.name == _name) {
          return {
            zindex: k,
            item: ione
          };
        }
      }
      console.log(`没有从数据中获取到数据${_name}`, this.data.modelData);
      return {
        zindex: -1,
        item: null
      };
    },
    /**
     * 从属性列表中获取某一个属性
     * @param {*} _attributes 
     * @param {*} _name 
     */
    findAttributeByName(_attributes, _name) {
      if (_attributes) {
        for (var _attribute of _attributes) {
          if (_attribute.name == _name) {
            return _attribute;
          }
        }
      }
      return null;
    },
    /**
     * 设置某一项到数据源
     * @param {*} zindex 
     * @param {*} item 
     */
    funcSetDataItem(zindex, item) {
      var _setKey = "modelData[" + zindex + "]";
      this.setData({
        [`${_setKey}`]: item
      });
    },
    /**
 * 设置某一项到数据源
 * @param {*} zindex 
 * @param {*} item 
 */
    funcSetDataItemColumn(zindex, _column, item) {
      var _setKey = `modelData[${zindex}].${_column}`;
      // console.log(_setKey);
      this.setData({
        [`${_setKey}`]: item
      });
    },
      /**
 * 把linkquery的参数传递给下一个
 * @param {*} _queryname 
 */
funcJoinLinkQuery(_queryname){
  if(this.data._options){
    if(this.data._options[_queryname]){
      this.data.linkQuery[_queryname] = this.data._options[_queryname];
    }
  }
},
/**
 * 组合新的url
 * @param {*} _path 
 */
funcBuildQuery(_path){
  if(this.data.linkQuery){
    if(_path.indexOf("?")>=0){
      var _pa = _path.split('?');
      if(_pa.length>1){
        var ars =_pa[1].split('&');
        var _info={};
        for(var args of ars){
           var aa=args.split('=');
           if(aa.length>1){
             _info[aa[0]]=aa[1];
           }
        }
        for(var arg in this.data.linkQuery){
          _info[arg] = this.data.linkQuery[arg];
        }
      }
      var _links=new Array();
      for(var agg in _info){
        _links.push(`${agg}=${_info[agg]}`);
      }
      return `${_pa[0]}?${_links.join('&')}`;
    }
  }
  return _path;
},
    FuncSetConfig(_config) {
      this.data.config = _config;
    },
    /**
     * 初始化表单
     * @param {*} modelData 
     */
    FuncInitForm(_modelInfo, _options) {
      // this.data.modelInfo = _modelInfo;
      this.data._options = _options;

      if(_modelInfo.attributes){
        for(var _attribute of _modelInfo.attributes){
          if(_attribute.name=='linkquery'){
            if(_attribute.args1){
            this.funcJoinLinkQuery(_attribute.args1);
            }
          }
        }
      }

      var modelData = _modelInfo.properties;
      modelData.forEach(item => {
        item.dataFrom = item.dataType;
        if (item.attributes) {
          item.attributes.forEach(_attribute => {
            switch (_attribute.name) {
              case "maxlength":
                {
                  var _max = Number(_attribute.args1);
                  if (_max) {
                    item.maxlength = _max;
                  }
                }
                break;
              case "hidden":
                {
                  item.hidden = true;
                }
                break;
              case "image":
                {
                  item.dataType = "image";
                  var _num = Number(_attribute.args1);
                  if (_num > 1) {
                    item.num = _num;
                  } else {
                    item.num = 1;
                  }
                }
                break;
              case "outer":
                {
                  item.dataType = "outer";
                  if (!item.value) {
                    if (!item.placeholder) {
                      item.display = " ";
                    } else {
                      item.display = "";
                    }
                  }
                  // item.path = `/pages/form/index/index?path=${_attribute.args1}&model=select&name=${item.name}&title=${item.title}`;
                  item.path =this.funcBuildQuery(`/pages/paste/index/index?path=${_attribute.args1}&model=select&name=${item.name}&title=${item.title}`);
                }
                break;
              case "outers":
                {
                  item.dataType = "outers";
                  //组合路径后写入
                  // item.path = `/pages/form/index/index?path=${_attribute.args1}&model=select&name=${item.name}&title=${item.title}`;
                  item.path=this.funcBuildQuery( `/pages/paste/index/index?path=${_attribute.args1}&model=select&name=${item.name}&title=${item.title}`);
                }
                break;
              case "object":
                {
                  item.dataType = "object";
                  //组合路径后写入
                  // item.path = `/pages/form/view/index?path=${_attribute.args1}&model=object&name=${item.name}&title=${item.title}`;
                  item.path = this.funcBuildQuery(`/pages/paste/view/index?path=${_attribute.args1}&model=object&name=${item.name}&title=${item.title}`);
                  if (item.value) {
                    if (typeof item.value == 'object') {
                      if (_attribute.args3) {
                        item.display = item.value[_attribute.args3];
                      }
                    }
                  }
                }
                break;
              case "objects":
                {
                  item.dataType = "objects";
                  item.display = "点击添加";
                  //组合路径后写入 querylink如何介入?
                  // item.path = `/pages/form/view/index?path=${_attribute.args1}&model=object&name=${item.name}&title=${item.title}`;
                  item.path=this.funcBuildQuery(`/pages/paste/view/index?path=${_attribute.args1}&model=object&name=${item.name}&title=${item.title}`);
                  if (item.value) {
                    item.objects = item.value;
                    item.objects.forEach(ii => {
                      ii.__display = ii[_attribute.args3];
                    });
                  }
                  // item.display_id = _attribute.args2;
                  // item.display_name = _attribute.args3;
                }
                break;
              case "select":
                {
                  if (_attribute.args1) {
                    item.dataType = "select";
                    var objs = JSON.parse(_attribute.args1);
                    item.items = objs;
                    var _arr = new Array();
                    objs.forEach(ii => {
                      _arr.push(ii.name);
                    });
                    item.array = _arr;
                    if (item.value != null) {
                      for (var _s of item.items) {
                        if (_s.value == item.value) {
                          item.display = _s.name;
                          break;
                        }
                      }
                    }
                  }
                }
                break;
              case "region":
                {
                  item.dataType = "region";
                  if (_attribute.args1) {
                    item.level = _attribute.args1;
                  }
                }
                break;
              case "unit":
                {
                  item.unit = _attribute.args1;
                }
                break;
              case "fentoyuan":
                {
                  // item.fentoyuan = true;
                  item.dataType = "double";
                  if (item.value) {
                    item.value = Number(Number(item.value / 100).toFixed(2));
                  }
                }
                break;
              case "readonly":
                {
                  item.readonly = true;
                }
                break;
              case "location":
              case "password":
              case "dateplan":
              case "text":
              case "textarea":
              case "richtext":
                {
                  item.dataType = _attribute.name;
                }
                break;
              case "cover":
                {
                  item.dataType = "cover";
                  if (!item.value) {
                    item.value = new Array();
                  } else {
                    //点击切换某一个？
                  }
                }
                break;
              case "query":
                {
                  if (_attribute.args1) {
                    if (_options) {
                      if (_options.hasOwnProperty(_attribute.args1)) {
                        item.value = _options[_attribute.args1];
                      }
                    }
                  }
                }
                break;
            }
          });
        }
        //最后的补充矫正
        if (item.dataType == item.dataFrom) {
          switch (item.dataType) {
            case "String":
              {
                if (!item.maxlength) {
                  item.dataType = "richtext";
                } else {
                  if (item.maxlength > 128) {
                    item.dataType = "textarea";
                  } else {
                    item.dataType = "text";
                  }
                }
              }
              break;
            case "Boolean":
              {
                item.dataType = "switch";
              }
              break;
            case "Int32":
            case "Int64":
              {
                item.dataType = "number";
              }
              break;
            case "Decimal":
            case "Double":
              {
                item.dataType = "double";
              }
              break;
            case "Guid":
              {
                item.dataType = "text";
                item.maxlength = 36;
              }
              break;
          }
        }
      });
      console.log("修正格式后", modelData);
      this.setData(
        {
          modelData: modelData,
          showButton: true
        }
      );
    },
    /**
     * 判断验证是否通过
     * @param {*} item 
     */
    funcValidateItem(k, item) {
      // console.log('validate.item', item);
      if (item.required) {
        if (!item.value) {
          item.focus = true;
          wx.showToast({
            title: `${item.title} 的值不能为空，请重新输入`,
            icon: 'none'
          })
          this.funcSetDataItem(k, item);
          var _this = this;
          setTimeout(function () {
            item.focus = false;
            _this.funcSetDataItem(k, item);
          }, 300);
          return false;
        }
      }
      if (item.maxlength) {
        if (item.value) {
          if (item.value.length > item.maxlength) {
            wx.showToast({
              title: `${item.title} 输入值过长，不能大于${item.maxlength}位，请重新输入`,
              icon: 'none'
            })
            this.funcSetDataItem(k, item)
            return false;
          }
        }
      }
      return true;
    },
    /**
     * 为某一项赋值
     * @param {*} item 
     */
    async funcFixedItemValue(item, _info) {
      switch (item.dataType) {
        case "richtext":
          {
            if (item.name) {
              var _richText = this.selectComponent(`#${item.name}`);
              if (_richText) {
                var _read = await _richText.FuncAsyncReadBody();
                if (_read) {
                  item.value = _read.html;
                }
              }
            }
          }
          break;
        case "number":
        case "double":
          {
            if (item.attributes) {
              for (var _attribute of item.attributes) {
                if (_attribute.name == "fentoyuan") {
                  if (item.value) {
                    var _val = Number(item.value);
                    item.value = Number(Number(_val * 100).toFixed(0));
                  }
                }
              }
            }
          }
          break;
        case "image":
          {
            if (item.value) {
              if (typeof item.value == "array") {
                item.value = item.value[0];
              }
            }
          }
          break;
        case "object":
          {
            if (item.object) {
              item.value = item.object;
            } else {
              item.value = null;
            }
          }
          break;
        case "objects":
          {
            if (item.objects) {
              item.value = item.objects;
            } else {
              item.value = null;
            }
          }
          break;
        case "cover":
          {
            //封面图
            if (this.data.imgs && this.data.imgs.length > 0) {
              var _current = this.data.imgs[this.data.imgindex];
              var _imgs = new Array();
              _imgs.push(_current);
              for (var ii of this.data.imgs) {
                if (_imgs.indexOf(ii) == -1) {
                  _imgs.push(ii);
                }
              }
              item.value = _imgs;
              if (item.attributes) {
                var _attribute = this.findAttributeByName(item.attributes, "cover");
                if (_attribute) {
                  if (_attribute.args2) {
                    _info[_attribute.args2] = _current;
                  }
                }
              }
            } else {
              item.value = [];
            }
            // item.value = this.data.imgs;
          }
          break;
      }
    },
    /**
     * 获取数据的值 不管验证
     */
    async FuncParseSerial() {
      //校验输入是否合法，
      var _info = {};
      for (var k = 0; k < this.data.modelData.length; k++) {
        var item = this.data.modelData[k];
        if (item.name) {
          await this.funcFixedItemValue(item, _info);
          _info[item.name] = item.value;
        }
      }
      return _info;
    },
    handlerRemoveItem(e) {
      console.log(e);
      // e.currentTarget.dataset.info
      // e.currentTarget.datasetwx.name
      var _info = e.currentTarget.dataset.info;
      var _name = e.currentTarget.dataset.name;
      var _find = this.funcFindItemByName(_name);
      if (_find.zindex >= 0) {
        var item = _find.item;
        var _attribute = this.findAttributeByName(item.attributes, "objects");
        if (_attribute) {
          var _targetid = _info[_attribute.args2];
          console.log('将要删除的对象是', _attribute.args2, _targetid);
          for (var k = 0; k < item.objects.length; k++) {
            let ii = item.objects[k];
            if (ii[_attribute.args2] == _targetid) {
              item.objects.splice(k, 1);
              break;
            }
          }
        }
        var _this = this;
        setTimeout(function () {
          _this.funcSetDataItem(_find.zindex, _find.item);
        }, 300);
      } else {
        console.log("没有找到这个项", _name);
      }
    },
    /**
     * 获取验证通过的值，验证不通过会阻断
     */
    async FuncParseForm() {
      //校验输入是否合法，
      var _info = {};
      for (var k = 0; k < this.data.modelData.length; k++) {
        var item = this.data.modelData[k];
        if (item.name) {
          await this.funcFixedItemValue(item, _info);
          var _validate = this.funcValidateItem(k, item);
          //一些其他的判断 数据校验，处理等
          if (!_validate) {
            return;
          }
          _info[item.name] = item.value;
        }
      }
      // console.log('准备返回数据给上一个页面');
      return _info;
    },
    /**
     * 给某一个项赋值
     * @param {*} _name 
     * @param {*} _value 
     */
    FuncSetValue(_name, _value, _display) {
      // console.log("外部某某一项配置值",_name,_value,_display);
      var _find = this.funcFindItemByName(_name);
      if (_find.zindex >= 0) {
        var item = _find.item;
        item.value = _value;
        item.display = _display;
        var _this = this;
        setTimeout(function () {
          _this.funcSetDataItem(_find.zindex, _find.item);
        }, 300);
      } else {
        console.log("没有找到这个项", _name);
      }
    },

    /**
     * 封面图，点击选择封面图
     * @param {*} e 
     */
    handlerTapCoverImage(e) {
      // console.log(e);
      this.setData({
        imgindex: e.currentTarget.dataset.index
      });
    },
    /**
     * outer的选择返回
     * @param {*} _name 字段名称
     * @param {*} _obj 返回的对象
     */
    FuncOuterValue(_name, _obj) {
      console.log("将要更新新的值到", _name, _obj);
      var _find = this.funcFindItemByName(_name);
      if (_find.zindex >= 0) {
        var item = _find.item;
        if (item) {
          if (item.attributes) {
            item.attributes.forEach(_attribute => {
              console.log(_attribute.name);
              switch (_attribute.name) {
                case "outer":
                  {
                    item.value = _obj[_attribute.args3];
                    item.display = _obj[_attribute.args4];
                  }
                  break;
                case "outers":
                  {
                    item.value = _obj[_attribute.args3];
                    item.display = _obj[_attribute.args4];
                    // item.object = _obj;
                    //写入数据到集合，或者整理到集合...
                  }
                  break;
                case "object":
                  {
                    item.value = _obj[_attribute.args3];
                    item.display = _obj[_attribute.args4];
                    item.object = _obj;
                  }
                  break;
                case "objects":
                  {
                    // item.value = _obj[_attribute.args3];
                    // item.display = _obj[_attribute.args4];
                    // item.object = _obj;
                    //写入数据到集合，或者整理到集合...
                    //使用哪一个进行排重?
                    if (!item.objects) {
                      item.objects = new Array();
                    }
                    _obj.__display = _obj[_attribute.args3];
                    item.objects.push(_obj);
                    item.value = item.objects;
                  }
                  break;
              }
            });
          }
        }
        var _this = this;
        console.log('配置新的值更新到UI', _find.item);
        setTimeout(function () {
          _this.funcSetDataItem(_find.zindex, _find.item);
        }, 300);
      } else {
        console.log("没有找到这个项", _name);
      }
    },
    /**
     * 提交表单给父级
     */
    async funcSubmitForm() {
      var _info = await this.FuncParseForm();
      console.log("pasteform.funcSubmitForm.submit", _info);
      this.triggerEvent("submit", _info);
    }
  }
})