// components/paste-picker/datetime-picker.js
// component/DatePicker/DatePicker.js

// 公历日期 1900年 - 今天
const glYear = [];
for (let i = 1900; i <= 2049; i++) {
    glYear.push(i)
}
// 公历月份
const glMonth = [];
for (let i = 1; i <= 12; i++) {
    glMonth.push(i < 10 ? ('0' + i) : i)
}
// 公历天份 28天
const glDay28 = [];
for (let i = 1; i <= 28; i++) {
    glDay28.push(i < 10 ? ('0' + i) : i)
}
// 公历天份 29天
const glDay29 = [];
for (let i = 1; i <= 29; i++) {
    glDay29.push(i < 10 ? ('0' + i) : i)
}
// 公历天份 30天
const glDay30 = [];
for (let i = 1; i <= 30; i++) {
    glDay30.push(i < 10 ? ('0' + i) : i)
}
// 公历天份 31天
const glDay31 = [];
for (let i = 1; i <= 31; i++) {
    glDay31.push(i < 10 ? ('0' + i) : i)
}

/**
 * 公历年月日处理
 */
function glDateHandler(arr) {
    let indexArr = arr
    let data = {
        "yearCol": glYear,
        "monthCol": glMonth,
        "dayCol": []
    }
    if (arr[1] == 0 || arr[1] == 2 || arr[1] == 4 || arr[1] == 6 || arr[1] == 7 || arr[1] == 9 || arr[1] == 11)
        data.dayCol = glDay31
    else if (arr[1] != 1)
        data.dayCol = glDay30
    else {
        let year = glYear[arr[0]];
        data.dayCol = glDay28;
        // 判断闰年
        if ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0) {
            data.dayCol = glDay29;
        }
    }
    return data;
}
/**
 * 初始化时分秒
 */
function glInitTime(){
  var _info={};
  var _hours=[];
  for(var k=0;k<24;k++){
    _hours.push((k<10?'0'+k:k));
  }
  var _minutes=[];
  var _seconds=[];
  for(var k=0;k<60;k++){
    _minutes.push((k<10?'0'+k:k));
    _seconds.push((k<10?'0'+k:k));
  }
  _info={
    hours:_hours,
    minutes:_minutes,
    seconds:_seconds
  };
  return _info;
}

// /**
//  * 公历月日处理 不带年份
//  */
// function glDateHandlerNoYear(arr) {
//     let indexArr = arr
//     let data = {
//         "monthCol": [],
//         "dayCol": []
//     }
//     data.monthCol = glMonth
//     if (arr[1] == 0 || arr[1] == 2 || arr[1] == 4 || arr[1] == 6 || arr[1] == 7 || arr[1] == 9 || arr[1] == 11)
//         data.dayCol = glDay31
//     else if (arr[1] != 1)
//         data.dayCol = glDay30
//     else
//         data.dayCol = glDay29
//     return data;
// }

const formatTime = date => {
    const year = date.getFullYear()
    const month = date.getMonth() + 1
    const day = date.getDate()
    return [year, month, day].map(formatNumber).join('-')
}

const formatNumber = n => {
    n = n.toString()
    return n[1] ? n : '0' + n
}
/**
 * 
 * @param {*} num 
 */
const formatLength = (num)=>{
  return num<10?`0${num}`:`${num}`;
}

// https://blog.csdn.net/weixin_49909441/article/details/116694845

Component({
  behaviors: [],
  properties: {
      // 控制是否显示日期选择组件的flag
      showDatePickerPlus: {
          type: Boolean, // 类型（必填），目前接受的类型包括：String, Number, Boolean, Object, Array, null（表示任意类型）
          value: false, // 属性初始值（可选），如果未指定则会根据类型选择一个
          // 属性被改变时执行的函数（可选），也可以写成在methods段中定义的方法名字符串, 如：'_propertyChange'
          observer: function (newVal, oldVal) {
              if (newVal != oldVal) {
                  if (newVal)
                      this.showDatePickerPlus()
                  else
                      this.closeDatePickerPlus()
              }
          }
      },
      initDate: {
          type: String,
          value: formatTime(new Date())
      },
      pickerName:{
        type:String,
        value:"datetimepicker"
      }
  },
  // 私有数据，可用于模版渲染
  data: {
      glDateMutiIndex: [90, 0, 0,0,0,0],
      // glDateNoYearMutiIndex: [0, 0],
      glYears: [],
      glMonths: [],
      // glMonthsNoYear: [],
      glDays: [],
      // glDaysNoYear: [],
      glHours:[],
      glMinutes:[],
      glSeconds:[],
      animationData: {},// 动画
      showDatePickerPlus: false,
  },

  // 生命周期函数，可以为函数，或一个在methods段中定义的方法名
  attached: function () {
    // console.log("--- attached -- - ");
      // let glDateMutiIndex = this.data.glDateMutiIndex;
      // var _init = this.data.initDate;
      // if(_init){
      //   let [y,m,d] = _init.split(' ')[0].split('-');
      //   let [h,f,s] =_init.split(' ')[1].split(':');
      //   glDateMutiIndex = [glYear.indexOf(parseInt(y)), parseInt(m) - 1, parseInt(d) - 1,parseInt(h),parseInt(f),parseInt(s)];
      // }
      // let glDate = glDateHandler(glDateMutiIndex);
      // var _time = glInitTime();
      // this.setData({
      //     glDateMutiIndex: glDateMutiIndex,
      //     glYears: glDate.yearCol,
      //     glMonths: glDate.monthCol,
      //     glDays: glDate.dayCol,
      //     glHours:_time.hours,
      //     glMinutes:_time.minutes,
      //     glSeconds:_time.seconds
      // })
  },
  moved: function () {
  },
  detached: function () {
  },

  methods: {
      initPicker:function(){
       /**
       * 初始化阴阳历数据
       */
      let glDateMutiIndex = this.data.glDateMutiIndex;
      var _init = this.data.initDate;
      if(!_init){
        //重置到当前日期
        var _date=new Date();
        _init =`${_date.getFullYear()}-${formatLength(_date.getMonth()+1)}-${formatLength(_date.getDate())} ${formatLength(_date.getHours())}:${formatLength(_date.getMinutes())}:${formatLength(_date.getSeconds())}`;
      }
      if(_init){
        let [y,m,d] = _init.split(' ')[0].split('-');
        let [h,f,s] =_init.split(' ')[1].split(':');
        glDateMutiIndex = [glYear.indexOf(parseInt(y)), parseInt(m) - 1, parseInt(d) - 1,parseInt(h),parseInt(f),parseInt(s)];
      }
      let glDate = glDateHandler(glDateMutiIndex);
      var _time = glInitTime();
      this.setData({
          glYears: glDate.yearCol,
          glMonths: glDate.monthCol,
          glDays: glDate.dayCol,
          glHours:_time.hours,
          glMinutes:_time.minutes,
          glSeconds:_time.seconds
      });
      //因为这个时候的UI可能还没有处理好
      var _this =this;
      setTimeout(function(){
        _this.setData({
          glDateMutiIndex:glDateMutiIndex
        });
      },100);
      },
      /**
       * 公历数据改变监听
       */
      bindDateGl: function (e) {
          let value = e.detail.value;
          let data = glDateHandler(value);
          this.setData({
              glDateMutiIndex: value,
              glYears: data.yearCol,
              glMonths: data.monthCol,
              glDays: data.dayCol,
          })
      },
      /**
       * 公历不带年份数据改变监听
       */
      bindDateGlNoYear: function (e) {
        console.log(e);
          let value = e.detail.value;
          let data = glDateHandler(value);
          this.setData({
              glDateMutiIndex: value,
              glMonths: data.monthCol,
              glDays: data.dayCol
          })
      },
      /**
       * 选择日期->确定
       */
      submitSelectDate: function () {
          let date = '';
          var _years = this.data.glYears;
          var _months =this.data.glMonths;
          var _days =this.data.glDays;
          var _hours =this.data.glHours;
          var _minutes =this.data.glMinutes;
          var _seconds =this.data.glSeconds;
          var _hit =this.data.glDateMutiIndex;
          date =`${_years[_hit[0]]}-${_months[_hit[1]]}-${_days[_hit[2]]} ${_hours[_hit[3]]}:${_minutes[_hit[4]]}:${_seconds[_hit[5]]}`;
          
          this.closeDatePickerPlus();
          var myEventDetail = {
              date: date,
              name:this.data.pickerName
          } // detail对象，提供给事件监听函数
          var myEventOption = {} // 触发事件的选项
          this.triggerEvent('submit', myEventDetail, myEventOption)
      },
      // 显示日期选择控件
      showDatePickerPlus: function () {
          this.initPicker();
          // console.log('执行显示 datetimepicker UI');
          var animation = wx.createAnimation({
              duration: 160,
              timingFunction: 'linear',
              delay: 0
          })
          this.animation = animation
          animation.translateY(280).step({duration: 0})
          this.setData({
              animationData: animation.export(),
              showDatePickerPlus: true
          })
          setTimeout(function(){
              animation.translateY(0).step()
              this.setData({
                  animationData: animation.export(),
              })
          }.bind(this), 160);
      },
      // 隐藏日期选择控件
      closeDatePickerPlus: function () {
          this.animation.translateY(280).step()
          this.setData({
              animationData: this.animation.export(),
          })
          setTimeout(function () {
              this.setData({
                  showDatePickerPlus: false,
              })
          }.bind(this), 120)
      },
      onMyButtonTap: function () {
          this.setData({
              // 更新属性和数据的方法与更新页面数据的方法类似
          })
      },
      _propertyChange: function (newVal, oldVal) {

      }
  }         
// https://blog.csdn.net/weixin_49909441/article/details/116694845
})