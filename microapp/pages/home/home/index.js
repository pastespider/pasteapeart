// pages/home/home/index.js
let app = getApp();
let api = app.globalData.api;
Page({

  /**
   * 页面的初始数据
   */
  data: {
    menuList: [{
        image: "/static/images/indexMenus/index01.png",
        path: "/pages/pasteform/index/index?path=testTable",
        name: '案例表'
      },
      {
        image: "/static/images/indexMenus/index01.png",
        path: "/pages/pasteform/view/index?path=testTable",
        name: '新增案例'
      },
      {
        image: "/static/images/indexMenus/index01.png",
        path: "/pages/pasteform/index/index?path=roleInfo",
        name: '权限表'
      },
      {
        image: "/static/images/indexMenus/index01.png",
        path: "/pages/pasteform/view/index?path=roleInfo",
        name: '新增权限'
      },
      {
        image: "/static/images/indexMenus/index01.png",
        path: "/pages/pasteform/index/index?path=gradeRole",
        name: '角色权限'
      },
      {
        image: "/static/images/indexMenus/index01.png",
        path: "/pages/pasteform/view/index?path=gradeRole",
        name: '分配权限'
      },
      {
        image: "/static/images/indexMenus/index01.png",
        path: "/pages/pasteform/index/index?path=gradeInfo",
        name: '角色表'
      },
      {
        image: "/static/images/indexMenus/index01.png",
        path: "/pages/pasteform/view/index?path=gradeInfo",
        name: '新增角色'
      },
    ]
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad(options) {
    // api.getHomeHome().then((data)=>{
    //   var res = data.data;
    //   console.log("home",data.code,data.data);
    //   // this.bannerList = res.banners;
    //   if(res.banners){
    //     res.banners.forEach(item=>{
    //      // item.image = app.globalData.apiBaseUrl+item.image
    //     });
    //   }
    //   this.setData({
    //     bannerList:res.banners,
    //     menuList : res.menus,
    //     articleClassList:res.classes,
    //     articleList:res.articles
    //   });
    // });
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

  }
})