// pages/home/work/index.js
let app = getApp();
let api = app.globalData.api;
let db = api.db;
Page({

  /**
   * 页面的初始数据
   */
  data: {
    img_url: "",
    img_guid_code: "",
    input_code:"",
    input_email:"admin@admin.com",
    input_pass:"123456",
    user_token:""
  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad(options) {
    this.data._options = options;
    this.btnRefreshImage();
  },
  btnRefreshImage(){
    this.data.img_guid_code = 'abc' + new Date().getTime().toString();
    this.setData({
      img_url: api.apiBaseUrl + "/api/app/User/Image?guid=" + this.data.img_guid_code
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
    var _token = db.get_token();
    if(!_token){
      _token='未登录';
    }
    this.setData({
      user_token:_token
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
    setTimeout(() => {
      this.data.pageIndex = 1;
      this.funcReadWorks();
    }, 500);
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
  handlerTapMenu(e) {
    console.log(e);
  },
  btnLogin(){
    console.log(this.data);

    let info={
      email:this.data.input_email,
      password:this.data.input_pass,
      code:this.data.input_code,
      guid:this.data.img_guid_code
    };

    api.userLogin(info).then(data=>{
      // console.log(data);
      db.set_token(data.data.token);
      wx.showToast({
        title: '登录成功',
        icon:'none'
      })
      this.setData({
        user_token:'登录成功'
      });
    });
  },
  inputCode(e){
    this.data.input_code = e.detail.value;
  },
  inputPass(e){
    this.data.input_email = e.detail.value;
  },
  inputEmail(e){
    this.data.input_pass = e.detail.value;
  }
})