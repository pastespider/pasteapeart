// app.js
import * as api from './utils/api'
// import * as db from './utils/dbHelper'
const eventMap = new Map();
App({
  onLaunch() {

    let token = api.db.get_token();
    wx.setBackgroundFetchToken({
      token: token || ''
    })

    wx.getBackgroundFetchData({
      fetchType: 'pre',
      success: (res) => {
        if (res.fetchedData) {
          // console.log("fetchedData load")
          if (res.fetchedData.indexOf("{") >= 0) {
            let data = JSON.parse(res.fetchedData)
            this.globalData.userInfo = data
            api.db.set('userInfo', data)
            api.db.set('userToken', data.token)
            this.emit('getUserInfo', JSON.stringify(data));
          }
        }
      },
      fail: () => {
        if (!token) {
          wx.login({
            provider: 'weixin',
            success: (loginRes) => {

            }
          });
        } else {
          let userinfo = wx.getStorageSync('userInfo')
          this.globalData.userInfo = userinfo
        }
      }
    })

    wx.loadFontFace({
      family: 'icon',
      source: 'url("https://at.alicdn.com/t/c/font_2553510_kfwma2yq1rs.woff2?t=1694918397022")',
      success: () => {
        console.log(123321);
      }
    })

    // wx.preloadAssets({
    //   data: [
    //     {
    //       type: 'font',
    //       src: 'https://at.alicdn.com/t/c/font_2553510_kfwma2yq1rs.woff2?t=1694918397022',
    //     },
    //   ],
    //   success(resp) {
    //     console.log('preloadAssets success', resp)
    //   },
    //   fail(err) {
    //     console.log('preloadAssets fail', err)
    //   },
    // })

    if(token){
      this.globalData.formconfig.header.utoken=token;
      this.globalData.formconfig.header.token=token;
    }

    api.db.del('orderbackid')


    //如果有密钥信息，则赋值给form

  },
  // 事件订阅 app.on('sharenum',this.route,this.func.bind(this))
  on(action, path, event) {
    if (eventMap) {
      if (eventMap.has(action)) {
        var events = eventMap.get(action);
        events.set(path, event);
      } else {
        var events = new Map();
        events.set(path, event);
        eventMap.set(action, events)
      }
    }
    // app.on('event_name',this.route,this.functionname.bind(this));
    //functionname(msg){//这里处理接收到的msg信息};
  },
  // 事件卸载 app.off('sharenum' this.route);
  off(action, path) {
    if (eventMap) {
      if (eventMap.has(action)) {
        var events = eventMap.get(action);
        events.delete(path);
      }
    }
  },
  // 事件触发 arg需要为字符串，在对应的地方自己拆装箱
  emit(action, arg) {
    if(action=="getUserInfo"){
      var _info = JSON.parse(arg);
      if(_info){
        if(_info.token){
          this.globalData.formconfig.header.utoken=_info.token;
          this.globalData.formconfig.header.token=_info.token;
        }
      }
    }
    if (eventMap && eventMap.has(action)) {
      var events = eventMap.get(action);
      var funcs = events.values();
      for (var func of funcs) {
        func(arg);
      }
      // eventMap.get(action) && eventMap.get(action)(arg)
    }
    //app.emit('eventname',msg);
  },
  onShow(e) {
    // console.log(e, this.globalData.backPayStatus);
    this.globalData.backPayStatus = '11223'
    if (e.referrerInfo && e.referrerInfo.extraData) {
      console.log(e);
    }
  },
  FuncHandlerFormItem(propertires) {
    if (propertires) {
      propertires.forEach(item => {
        if (item.dataType == "String") {
          if (item.maxlength > 0) {
            if (item.maxlength > 64) {
              item.dataType = "textarea";
            } else {
              item.dataType = "text"
            }
          } else {
            item.dataType = "richtext";
          }
        }
        if (item.dataType == "Boolean") {
          item.dataType = "switch";
        }
        if (item.dataType == "DateTime") {
          item.dataType = "datetime";
        }
      });
    }
  },
  // https://blog.csdn.net/gaowenju1991/article/details/132321342
  globalData: {
    userInfo: null,
    api: api,
    db: api.db,//这里错了，应该是api.db
    apiBaseUrl: api.apiBaseUrl,
    ossUrl: '',
    backPayStatus: 0,
    formconfig: {
      url: api.apiBaseUrl+"/api/app/Upload/Upload",
      host: api.apiBaseUrl,
      header: {
        token: api.db.get_token(),
        utoken:api.db.get_token()
      }
    }
  }
})
