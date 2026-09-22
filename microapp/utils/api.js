import { get, post,set_token,baseUrl,db } from "./request";

// 首页-------------------------------------------

//登录


let getPasteAddModel = (params = {}) => { return get(`/api/app/${params.path}/readAddModel`) };

let getPasteEditModel = (params = {}) => { return get(`/api/app/${params.path}/${params.id}/readUpdateModel`) };

/**
 * params.url
 * 自定义Url请求，示例/api/app/Work/AddModel
 * @param {*} params 
 */
let getCustom = (params = {}) => { return get(params.__url,params) };
/**
 * params.url 这个会不会冲突？
 * 自定义的post请求 示例
 * @param {}} params 
 */
let postCustom = (params = {}) => { return post(params.__url, params, { need: true }) };

//---------------------------work部分--------------------------------
//首页获取数据
let getHomeHome = (params = {}) => { return get(`/api/app/Home/Home`, params) };

//==================================================================
let userLogin = (params = {}) => { return post('/api/app/User/Login', params, { need: false }) };

let apiBaseUrl = baseUrl;

// let db = db;

// 上传图片
let uploadImage = (num, callback) => {

	// 获取用户token
	// let userToken = db.get("userToken");
	// if (!userToken) {
	// 		this.$store.commit('showLoginTip', true);
	// 		return false;
	// };
	wx.chooseImage({
			count: num,
			success: (res) => {
					wx.showLoading({
							title: '上传中...'
					});
					let tempFilePaths = res.tempFilePaths;
					for (var i = 0; i < tempFilePaths.length; i++) {
							wx.uploadFile({
									url: apiBaseUrl + '/api/app/Upload/UploadMedia',
									filePath: tempFilePaths[i],
									fileType: 'image',
									name: 'file',
									header: {
											'Accept': 'application/json',
											'Content-Type': 'multipart/form-data',
											'utoken': db.get_token()
									},
									success: (uploadFileRes) => {
											callback(JSON.parse(uploadFileRes.data));
									},
									fail: (error) => {
											if (error && error.response) {
													showError(error.response);
											}
									},
									complete: () => {
											setTimeout(function () {
													wx.hideLoading();
											},
													250);
									},
							});
					}
			},
	fail: (err) => {
		console.log(err);
	}
	});
};

// 上传图片
let uploadHead = (num, callback) => {

	wx.chooseImage({
			count: num,
			success: (res) => {
					wx.showLoading({
							title: '上传中...'
					});
					let tempFilePaths = res.tempFilePaths;
					for (var i = 0; i < tempFilePaths.length; i++) {
							wx.uploadFile({
									url: apiBaseUrl + '/api/openuser/UploadHead',
									filePath: tempFilePaths[i],
									fileType: 'image',
									name: 'file',
									header: {
											'Accept': 'application/json',
											'Content-Type': 'multipart/form-data',
											'utoken': db.get_token()
									},
									formData: {
											'method': 'images.upload',
											'upfile': tempFilePaths[i]
									},
									success: (uploadFileRes) => {
											callback(JSON.parse(uploadFileRes.data));
									},
									fail: (error) => {
											if (error && error.response) {
													showError(error.response);
											}
									},
									complete: () => {
											setTimeout(function () {
													wx.hideLoading();
											},
													250);
									},
							});
					}
			}
	});
};

function showError(msg){
	wx.showToast({
		title: msg,
		icon: 'none'
	})
}

// 上传聊天图片
let uploadChatImage = (num, callback) => {
	// 获取用户token
	// let userToken = db.get("socketInfo").token;
	wx.chooseImage({
			count: num,
			success: (res) => {
					wx.showLoading({
							title: '上传中...'
					});
					let tempFilePaths = res.tempFilePaths;
					for (var i = 0; i < tempFilePaths.length; i++) {
							wx.uploadFile({
									url: apiBaseUrl+'/api/talk/Upload/UploadImage',
									filePath: tempFilePaths[i],
									fileType: 'image',
									name: 'file',
									header: {
											'Accept': 'application/json',
											'Content-Type': 'multipart/form-data',
											'ttoken': db.get_token()
									},
									formData: {
											'method': 'images.upload',
											'upfile': tempFilePaths[i]
									},
									success: (uploadFileRes) => {
											callback(JSON.parse(uploadFileRes.data));
									},
									fail: (error) => {
											if (error && error.response) {
													showError(error.response);
											}
									},
									complete: () => {
											setTimeout(function () {
													wx.hideLoading();
											},
													250);
									},
							});
					}
			}
	});
};

/**
 * 
 * @param {*} src 
 */
let handleUrl = function handleUrl(src){
    if(src){
			if(src.indexOf('http')==-1){
				return apiBaseUrl+src;
			}
		}
	return src;
}

export {
	set_token,
	apiBaseUrl,
  db,
  getCustom,
  postCustom,
  getHomeHome,
  userLogin,
  getPasteAddModel,
  getPasteEditModel,
};