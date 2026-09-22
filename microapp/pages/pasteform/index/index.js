// pages/form/index/index.js
// let _path = "";
// let _classPath = "needWork";
// let _options = null;
// let _properties = null;
let app = getApp();
let api = app.globalData.api;
Page({

  /**
   * 页面的初始数据
   */
  data: {
    queryList: [],
    dataList: [],
    linkQuery:{},
    _options:{},
    pageIndex:1,
    pageSize:20,
    pageNext:false,
    _properties:[]
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad(options) {
    this.data._options = options;
    // console.log(options);
    this.data._options = options;
    // options.path
    //基于path请求数据
    api.getCustom({ __url: `/api/app/${options.path}/readListModel` }).then(data => {
      var obj = data.data;
      // console.log(obj);
      if (data.code == 200) {
        if(obj.title){
          wx.setNavigationBarTitle({
            title: obj.title,
          })
        }
        if (obj.queryProperties) {
          for(var _porperty of obj.queryProperties){
              if(_porperty.attributes){
                _porperty.attributes.forEach(_attribute=>{
                });
              }
              if(_porperty.dataType=="Int32" || _porperty.dataType=="Int64"){
                _porperty.dataType="number";
              }else{
                _porperty.dataType="text";
              }
          }
          this.setData({
            queryList: obj.queryProperties
          });
        }
        this.data._properties = obj.properties;
        if(obj.attributes){
          for(var _attribute of obj.attributes){
            if(_attribute.name=='linkquery'){
              if(_attribute.args1){
              this.funcJoinLinkQuery(_attribute.args1);
              }
            }
          }
        }
        this.handlerProperties();
        this.funcReadPageData();
      }
    });
    if(typeof(this["funcTestAction"]) == "function"){
      this["funcTestAction"]('abc');
    }
  },
  funcTestAction(e){
    console.log("function funcTestAction called!",e);
  },
  /**
   * 获取页面数据
   */
  funcReadPageData(){
    var _info={
      page:this.data.pageIndex,
      size:this.data.pageSize,
      __url:`/api/app/${this.data._options.path}/page`
    };
    api.getCustom(_info).then(oob => {
      if (oob.code == 200) {
        var datas = oob.data;
        if (datas.items && datas.items.length > 0) {
          var _datas = new Array();
          for (var item of datas.items) {
            var _line = new Array();
            for (let _name in item) {
              var _property = this.funcFindProperty(_name);
              if(_property){
                var _new = {
                  dataType: _property.dataType,
                  value: item[_name],
                  title: _property.title,
                  unit: _property.unit || '',
                  name: _name
                };
                if(_property.display){
                    var __obj = item[_property.display.column];
                    if(__obj){
                      _new.value = __obj[_property.display.name];
                    }
                }
                if (!_property.hidden) {
                  _line.push(_new);
                }
              }
            }
            _datas.push(_line);
            this.setData({
              dataList: _datas
            });
            if(_datas.length>=this.data.pageSize){
              this.data.pageNext=true;
            }
          }
        }else{
          this.data.pageNext=false;
        }
      }else{
        this.data.pageNext=false;
      }
    })
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
            if(!_info[arg]){
            _info[arg] = this.data.linkQuery[arg];
            }
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
  /**
   * 处理字段属性
   */
  handlerProperties() {
    if (this.data._properties) {
      for (var _property of this.data._properties) {
        if (_property.attributes) {
          for (var _attribute of _property.attributes) {
            switch (_attribute.name) {
              case "hidden":
              case "wxhidden":
                {
                  _property.hidden = true;
                }
                break;
              case "image":
                {
                  _property.dataType = "image";
                }
                break;
              case "wxdisplay":
                {
                  // args1 extendUser args2 name
                  // 另外一个数据的某一个字段？？？
                    //另外一个字段的某一个值！ extendUser.name 
                    _property.display={
                      column:_attribute.args1,
                      name:_attribute.args2
                    };
                }
                break;
                case "linkquery":
                  {
                    if(_attribute.args1){
                      this.funcJoinLinkQuery(_attribute.args1);
                    }
                  }
                  break;
            }
          }
        }
      }
    }
  },
  /**
   * 获取对应字段的属性
   * @param {*} _name 
   */
  funcFindProperty(_name) {
    if (this.data._properties) {
      for (var item of this.data._properties) {
        if (item.name == _name) {
          return item;
        }
      }
    }
    return null;
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady() {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow() {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide() {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload() {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh() {
    setTimeout(()=>{
      this.data.pageIndex=1;
      this.funcReadPageData();
      wx.stopPullDownRefresh();
    },300);
  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom() {
    //是否有下一页?
    if(this.data.pageNext){
      wx.showNavigationBarLoading()
    this.data.pageIndex =this.data.pageIndex+1;
    this.funcReadPageData();
    wx.hideNavigationBarLoading()
    }
  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage() {

  },
  /**
   * 其他页面过来的
   */
  FuncSelectModel(_name,_info){
    // console.log(`index FuncSelectModel name:${_name}`,_info);
    let dom = this.selectComponent("#paste-form");
    dom.FuncOuterValue(_name,_info);
  },
  /**
   * 点击选择项
   * @param {*} e 
   */
  handlerSectionTap(e) {
    // console.log(e);
    let _options = this.data._options;
    var datas =e.currentTarget.dataset.info;
    var _info={};
     for(var data of datas){
       _info[data.name] = data.value;
     }
    if(_options){
      if(_options.model){
        if(_options.model =="select" || _options.model=="object"){
          let pages = getCurrentPages();
          let currPage = null; //当前页面
          let prevPage = null; //上一个页面
          if (pages.length >= 2) {
            currPage = pages[pages.length - 1]; //最后一个元素为当前页面。
            prevPage = pages[pages.length - 2]; //上一个页面
          }
          if (prevPage) {
            if(!prevPage.FuncSelectModel){
               console.log(`没有从上一个页面找到函数FuncSelectModel`);
              return;
            }
            prevPage.FuncSelectModel(_options.name,_info);
            wx.navigateBack();
          }
        }
      }
    }
  },
  handlerTap(){
    let pages = getCurrentPages();
    let currPage = null; //当前页面
    let prevPage = null; //上一个页面
    if (pages.length >= 2) {
      currPage = pages[pages.length - 1]; //最后一个元素为当前页面。
      prevPage = pages[pages.length - 2]; //上一个页面
    }
    if (prevPage) {
      prevPage.SetFormItemValue('workClassId',5,'清洗空调');
      wx.navigateBack();
    }
  }
})