// pages/home/center/index.js
let app = getApp();
let api = app.globalData.api;
Page({

  /**
   * 页面的初始数据
   */
  data: {

  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad(options) {
    //读取个人信息
    this.funcReadCenter();
  },
funcReadCenter(){
    api.getCenterInfo().then(res=>{
      if(res.code==200){
        var _data =res.data;
          console.log(_data);
      }
    });
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
    // var _read = wx.getStorage("userInfo");
    // console.log(_read);
    // if(_read){
    //   this.setData({
    //     userInfo:_read
    //   });
    // }

    var _this =this;
    wx.getStorage({
      key: "userInfo",
      success: function (res) {
        // console.log(res);
        _this.setData({
        userInfo:res.data
      });
      }
  });

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

  }
})