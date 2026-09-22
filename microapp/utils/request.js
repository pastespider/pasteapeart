// 同时发送异步代码的次数
let ajaxTimes = 0;
let showLoading = 1100;//多少毫秒后显示遮罩层
let showMasking = false;//当前是否显示了遮罩层
// 基础url
const baseUrl = "http://localhost:22222";//find_api_url
import * as db from 'dbHelper.js'
var get = (url, params, obj) => {
	var _param = {};
	if (params) {
		var _url = '';
		for (let ii in params) {
			var _val = params[ii];
			if (_val) {
				if (_url) {
					_url += '&' + ii + '=' + encodeURIComponent(_val);
				} else {
					_url = ii + '=' + encodeURIComponent(_val);
				}
			}
		}
		// var _query =(new URLSearchParams(params)).toString();
		if (url.indexOf('?') >= 0) {
			url = url + '&' + _url;
		} else {
			url = url + '?' + _url;
		}
		_param.url = url;
	} else {
		_param.url = url;
	}
	_param.method = 'GET';
	return apibase(_param);
}
var post = (url, params, obj) => {
	// params.url = url;
	// params.method = 'POST';
	var _param = {};
	_param.data = params;
	_param.method = 'POST';
	_param.url = url;
	return apibase(_param);
}
// var _userToken = null;
//读取token
var get_token = () => {
	return db.get_token();
}
//修改token
var set_token = (val) => {
	db.set_token(val);
};
var total_request = 0;
var apibase = (parmas) => {
	// 当有地方调用请求方法的时候，就增加全局变量，用于判断有几个请求了
	ajaxTimes++;
	// 显示加载中loading效果
	var _current_count = ajaxTimes;
	setTimeout(function () {
		if (ajaxTimes >= _current_count) {
			console.log(_current_count, "current,ajax", ajaxTimes);
			if (!showMasking) {
				showMasking = true;
				wx.showLoading({
					title: "加载中",
					mask: true  //开启蒙版遮罩
				});
			}
		}
	}, showLoading);
	/**
	 *   根据不同的url接口,来设置不同的header请求头
	 **  判断 url中是否带有 /my/ 请求的是私有的路径 带上header token
	 **  { ...parmas.header }  ==> 先解构出传进来的header对象,然后再往这个对象里面添加Authorization字段数据，这样即使有传入header的其他字段也能保留下来
	 *?  如果传入的parmas参数中没有header，那myHeader就是个空的对象 {} 因为啥都没有
	 */
	let myHeader = { ...parmas.header };
	// if (parmas.url.includes("/neddToken/")) {
	// 	// 往myHeader这个对象里插入键值对 带上Storage中存储的token
	// 	myHeader["Authorization"] = wx.getStorageSync("token");
	// }
	myHeader["token"] = get_token();
	total_request++;
	return new Promise((resolve, reject) => {
		/**
				 *  ...parmas ===>就是将传进来的参数扩展开，一行行展示在这里面
				 * 比如：传进来
				 * {
				 *  url:'xx',
				 *  data:{key1:val1,key2:val2}
				 * }
				 * 那么通过  ...parmas  就会把这些内容展示到这里了
		*/
		wx.request({
			...parmas,
			// 注意，此行必须放在   ...parmas  之下，才能覆盖其传入的url:xxx参数
			url: baseUrl + parmas.url,
			/**
			 * !可以设置上默认的content-type，然后再扩展出传入的myHeader，如果传入的myHeader为空，那header就还是默认的content-type一个键值对
			 * !{ 'content-type': 'application/json', ...myHeader } ==》 扩展出myHeader这个对象中的键值对;
			 */
			header: { 'content-type': 'application/json', ...myHeader },
			success: (result) => {
				// console.log(result);
				// console.log('request success !');
				if (result.statusCode) {
					if (result.statusCode == 700 || result.statusCode == 206 || result.statusCode == 630) {
						if (result.data && result.data.message) {
							wx.showToast({
								title: result.data.message,
								icon: 'none'
							})
						}
					}
				}
        // 请求成功，就将成功的数据返回出去
        // console.log(result,'line.117');
				if(result.statusCode == 401) {
					db.del('userToken');
          // db.del('userInfo');
          wx.showToast({
            title: '需要登陆',
            icon:"none"
          })
          var nav = wx.navigateTo({
            url: '/pages/home/login/index',
          })     
          console.log(nav);
				}
				resolve({code:result.statusCode,data:result.data});
			},
			fail: (err) => {
				// console.log(err);
				// console.log('request fail !');
				reject(err)
			},
			// 不管请求成功还是失败，都会触发
			complete: () => {
				/**
				 * !loading效果同时被多个请求触发是可以显示一个的，但是关闭loading一旦被第一个请求完成后关闭，后面的请求触发的loading效果就没了
				 * !所以，需要通过全局设置一个变量，来监听同时触发了几个请求，当最后一个请求完成后，再关闭loading
				 * ?每次结束请求后，就减少全局变量，当为0时，就表示这是最后一个请求了
				 */
				ajaxTimes--;
				// 此时就可以关闭loading效果了
				if (ajaxTimes <= 0) {
					//  关闭正在等待loading效果
					if (showMasking) {
						showMasking = false;
						wx.hideLoading();
					}
				}
				// console.log('total request ' + total_request);
			}
		});
	})
}
export {
	get,
	post,
	set_token,
	baseUrl,
	db
};
