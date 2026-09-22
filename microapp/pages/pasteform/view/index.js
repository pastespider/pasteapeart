// pages/form/view/index.js
let _id = 0;
let _path = "";
let _classPath = "needWork";
// let _options = null;
let app = getApp();
let api = app.globalData.api;
Page({

  /**
   * 页面的初始数据
   */
  data: {
    linkQuery:{}
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad(options) {
    this.data._options = options;
    console.log("pages/paste/view/index onload", options);
    if (options.id) {
      _id = options.id;
    }
    if (options.path) {
      _path = options.path;
    }
    if (options.title) {
      wx.setNavigationBarTitle({
        title: options.title,
      })
    }
    if (!options.id) {
      api.getCustom({ __url: `${this.funcReadUrl(true)}/readAddModel` }).then(obj => {
        if (obj.code == 200) {
          if (obj.data.title) {
            wx.setNavigationBarTitle({
              title: obj.data.title,
            })
          }
          let dom = this.selectComponent("#paste-form");
          dom.FuncInitForm(obj.data, options);
        } else {
          setTimeout(() => {
            wx.navigateBack();
          }, 2000);
        }
      });
    } else {
      api.getCustom({ __url: `${this.funcReadUrl(true)}/readUpdateModel` }).then(obj => {
        // console.log(obj);
        if (obj.code == 200) {
          let dom = this.selectComponent("#paste-form");
          dom.FuncInitForm(obj.data, options);
        } else {
          setTimeout(() => {
            wx.navigateBack();
          }, 2000);
        }
      });
    }
  },
  /**
   * 
   * @param {*} isadd 
   */
  funcReadUrl(isadd) {
    if (_path.indexOf("/") >= 0) {
      return `${_path}`;
    } else {
      return `/api/app/${_path}`;
    }
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
    if(typeof this["global_func1"] == 'function'){
      this["global_func1"]();
    }else{
      console.log(typeof(this["global_func1"]));
    }
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

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom() {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage() {

  },
  /**
 * 其他页面过来的
 */
  FuncSelectModel(_name, _info) {
    console.log(`view FuncSelectModel name:${_name}`, _info);
    let dom = this.selectComponent("#paste-form");
    dom.FuncOuterValue(_name, _info);
  },
  /**
   * 下一个页面返回的内容
   * @param {*} _name 
   * @param {*} _obj 
   */
  FuncOuterValue(_name, _obj) {
    let dom = this.selectComponent("#paste-form");
    dom.FuncOuterValue(_name, _obj);
  },
  async onSubmitForm() {
    let _options = this.data._options;
    let dom = this.selectComponent("#paste-form");
    var _info = await dom.FuncParseForm();
    if (_info) {
      console.log("页面从表单中读取到", _info);
    }
    if (_options.model && _options.model == "object") {
      let pages = getCurrentPages();
      let currPage = null; //当前页面
      let prevPage = null; //上一个页面
      if (pages.length >= 2) {
        currPage = pages[pages.length - 1]; //最后一个元素为当前页面。
        prevPage = pages[pages.length - 2]; //上一个页面
      }
      if (prevPage) {
        prevPage.FuncOuterValue(_options.name, _info);
        wx.navigateBack();
      }
    }

    //进行新增或者更新
    // if (_id && _id != '0' && _id != 0) {
    _info.url = "";
    if (_id) {
      _info.__url = `${this.funcReadUrl()}/updateItem`
    } else {
      _info.__url = `${this.funcReadUrl()}/item`
    }
    api.postCustom(_info).then(data => {
      console.log(data);
      if (data.code == 200) {
        wx.showToast({
          title: data.data,
          icon: "none"
        })
        setTimeout(() => {
          wx.navigateBack();
        }, 2000);
      }
    });
    // _apipost(`${auto_path()}/updateItem`, true, JSON.stringify(_info), function (code, obj) {
    //     if (code == 200) {
    //         layer.msg("更新成功", { time: 1500 }, function () {
    //             var index = parent.layer.getFrameIndex(window.name); //获取窗口索引
    //             parent.layer.close(index);  // 关闭layer  id = layui-layer(index)
    //         });
    //     }
    // });
    // } else {
    //     _apipost(`${auto_path()}/item`, true, JSON.stringify(_info), function (code, obj) {
    //         if (code == 200) {
    //             layer.msg("添加成功", { time: 1500 }, function () {
    //                 window.parent.location.reload();
    //                 var index = parent.layer.getFrameIndex(window.name); //获取窗口索引
    //                 parent.layer.close(index);  // 关闭layer
    //             });
    //         }
    //     });
    // }
  },
  /**
   * 组件主动通知的数据，一般是回值
   * @param {*} _info 
   */
  handlerSubmit(data) {
    let _options = this.data._options;
    console.log("form.view.index.handlerSubmit", data,_options);
    var _handlered=false;
    var _info = data.detail;
    if (_options) {
      if (_options.model) {
        if (_options.model == "select" || _options.model == "object") {
          let pages = getCurrentPages();
          let currPage = null; //当前页面
          let prevPage = null; //上一个页面
          if (pages.length >= 2) {
            currPage = pages[pages.length - 1]; //最后一个元素为当前页面。
            prevPage = pages[pages.length - 2]; //上一个页面
          }
          if (prevPage) {
            if (!prevPage.FuncSelectModel) {
              console.log(`没有从上一个页面找到函数FuncSelectModel`);
              return;
            }
            prevPage.FuncSelectModel(_options.name, _info);
            _handlered=true;
            wx.navigateBack();
          }
        }
      }
    }

    if(!_handlered){
    //执行新增 或者更新

    if (_info.id) {
      _info.__url = `/api/app/${_options.path}/updateItem`;
    } else {
      _info.__url = `/api/app/${_options.path}/item`;
    }
    api.postCustom(_info).then(data => {
      if (data.code == 200) {
        if(this.data._options.path=="linkWork"){
          wx.showToast({
            title: '发表成功',
            icon:"none"
          })
        }else{
          wx.showToast({
            title: '操作成功',
            icon:"none"
          })
        }
        setTimeout(()=>{
          wx.navigateBack();
        },1500);
      }
    });
  }
  },
  global_func1(){
    console.log("--- global_func1 called! ---");
  }
})