var _dbmap =new Map();

/**
 * 
 * @param {*} key 
 * @param {*} sync 
 * @returns 
 */
function get(key, sync = true) {
	if(_dbmap.has(key)){
		return _dbmap.get(key);
	}
    try {
        if (sync) {
            var _read = wx.getStorageSync(key);
            if(_read){
                _dbmap.set(key,_read);
            }
			return _read;
        } else {
            let data = '';
            wx.getStorage({
                key: key,
                success: function (res) {
                    data = res.data;
                    _dbmap.set(key,data);
                }
            });
            return data;
        }
    } catch (e) {
        return false;
    }
}

/**
 * 
 * @param {*} key 
 * @param {*} value 
 * @param {*} sync 
 */
function set(key, value, sync = true) {
    try {
        _dbmap.set(key,value);
        if (sync) {
         wx.setStorageSync(key, value);
        } else {
            wx.setStorage({
                key: key,
                data: value
            });
        }

    } catch (e) {
	console.log(e);
    }
}

/**
 * 
 * @param {*} key 
 * @param {*} sync 
 * @returns 
 */
function del(key, sync = true) {
    try {
        _dbmap.delete(key);
        if (sync) {
           wx.removeStorageSync(key);
        } else {
            wx.removeStorage({
                key: key
            });
        }
    } catch (e) {
        return false;
    }
}
/**
 * 
 * @param {*} sync 
 * @returns 
 */
function clear(sync = true) {
    try {
        _dbmap.clear();
        if (sync) {
         wx.clearStorageSync();
        } else {
            wx.clearStorage();
        }

    } catch (e) {
        return false;
    }
}

/**
 * 对象转成&拼接参数
 * @param {*} params 
 * @returns 
 */
function addQueryString(params) {
	var str = '';
	for (var Key in params) {
		if(Key == 'storeid') {
			wx.setStorageSync('storeid', params[Key])
		}
		str += Key + '=' + params[Key] + '&';	
	}
	return '?' + str.substr(0, str.length -1); //严谨一些 
}

/**
 * javascript 对象转成&拼接参数
 * @param {*} params 
 * @returns 
 */
function addQueryStringnoQues(params) {
	var str = '';
	for (var Key in params) {
		str += Key + '=' + params[Key] + '&';
	}
	return str.substr(0, str.length -1); //严谨一些 
}

/**
 * 图片url拼接
 * @param {*} url 
 * @returns 
 */
function handleUrl(url) {
	if(url) {
		if(url.indexOf('http') != -1) {
			return url
		} else {
			return apiFilesUrl + url
		}
	} else {
		return
	}
}
/**
 * 
 * @returns 
 */
function get_token(){
	return get('userToken');
}

/**
 * 
 * @param {*} _token 
 */
function set_token(_token){
  set('userToken',_token);
}


/**
 * 返回完整的时间格式，如yyyy-MM-dd HH:mm:ss
 * @param {*} value 
 */
function formatDate(value) {
    if (value == undefined) {
        return;
    }
    // let date = new Date(value * 1000);
    var date = new Date(value);
    //时间戳为10位需*1000，时间戳为13位的话不需乘1000
    var y = date.getFullYear();
    var MM = date.getMonth() + 1;
    MM = MM < 10 ? ('0' + MM) : MM; //月补0
    var d = date.getDate();
    d = d < 10 ? ('0' + d) : d; //天补0
    var h = date.getHours();
    h = h < 10 ? ('0' + h) : h; //小时补0
    var m = date.getMinutes();
    m = m < 10 ? ('0' + m) : m; //分钟补0
    var s = date.getSeconds();
    s = s < 10 ? ('0' + s) : s; //秒补0
    // return y + '-' + MM + '-' + d; //年月日
    return y + '-' + MM + '-' + d + ' ' + h + ':' + m + ':' + s; //年月日时分秒
}

/**
 * 返回日期格式，如yyyy-MM-dd
 * @param {*} value 
 */
function formatShortDate(value) {
    if (value == undefined) {
        return;
    }
    // let date = new Date(value * 1000);
    var date = new Date(value);
    //时间戳为10位需*1000，时间戳为13位的话不需乘1000
    var y = date.getFullYear();
    var MM = date.getMonth() + 1;
    MM = MM < 10 ? ('0' + MM) : MM; //月补0
    var d = date.getDate();
    d = d < 10 ? ('0' + d) : d; //天补0
    var h = date.getHours();
    h = h < 10 ? ('0' + h) : h; //小时补0
    var m = date.getMinutes();
    m = m < 10 ? ('0' + m) : m; //分钟补0
    var s = date.getSeconds();
    s = s < 10 ? ('0' + s) : s; //秒补0
    // return y + '-' + MM + '-' + d; //年月日
    return y + '-' + MM + '-' + d ; //年月日时分秒
}



export {
    get,
    set,
    del,
    clear,
	addQueryString,
	addQueryStringnoQues,
	handleUrl,
  get_token,
  set_token,
    formatDate,
    formatShortDate
}