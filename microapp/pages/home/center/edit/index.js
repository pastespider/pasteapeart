// page_mycom/setting/userInfo/userInfo.js
const app=getApp();
let api = app.globalData.api;
Page({

  /**
   * 页面的初始数据
   */
  data: {
    user:{},
    modity:{},
    version:'',
    nick:''
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad(options) {
    
    // /api/app/Member/Info

    this.getInfo();
    const accountInfo = wx.getAccountInfoSync();
    let version_number = accountInfo.miniProgram.version; // 小程序 版本号
    this.setData({
      version:version_number
    });

    // 头像 昵称 签名  申请认证  读取(刷新)

    // /api/app/Member/Update

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
  getInfo(){
    api.getInfo().then(res=>{
      this.user = res;
      this.setData({
        user:res,
        nick:res.nickName
      });
    });
  },
  tapModityNick(){
    if(this.data.nick.length>0){
      api.updataNickName({
        id: this.data.user.id,
        body: this.data.nick,
      }).then(res => {
        if(res) {
          api.updataToken({
          }).then(res1 => {
            if(res1) {
              app.globalData.db.set('userInfo',res);
              app.globalData.userInfo=res;
            }
          })
        }
      })
    }
  }
})