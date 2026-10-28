/**
 *=======================================================================
 *日期选择器js组件。
 *@author ：johnnyzheng(johnnyzheng@tencent.com) 郑灿双
 *@version ： 2012-07-11
 *@modification list：2012-08-16  规范样式名称
 *                    2013-01-04  增加主题设置接口
 *                    2013-01-31  增加自定义灰掉周末 周几的选项，增加自动初始化自动提交的功能
 *                    2013-03-15  支持一个页面多个日期选择器，快捷日期选择
 *                    2013-03-26  增加确认、取消按钮的隐藏，而直接自动提交
 * 					  2013-08-01  扩展接口，增加最近90天，增加自定义可选时间
 * 					  2013-08-12  日期选择器框体宽度超出视窗大小的时候制动鼓靠右对齐
 *					  2014-02-25  增加业务接口：获取当前日期对象的的选中日期
 *					  2014-10-13  扩展参数，支持日期下拉选择自定义年和月份，配合theme:ta来使用。
 *=======================================================================
 */
/**
  * @description 整个日期选择器对象的构造函数入口，支持丰富的接口参数传递，大多数提供默认配置，可传入覆盖
  * @param {String} inputId 日期选择器ID
  * @param {object} options 配置数组
  */
function pickerDateRange(inputId, options) {
	/**
	 * 默认配置参数数据，每个参数涵义在后解释
	 */
	var defaults = {
		aToday: 'aToday', //今天
		aYesterday: 'aYesterday', //昨天
		aRecent7Days: 'aRecent7Days', //最近7天
		aRecent14Days: 'aRecent14Days',//最近14天
		aRecent30Days: 'aRecent30Days', //最近30天
		aRecent90Days: 'aRecent90Days', //最近90天
		startDate: '', // 开始日期
		endDate: '', // 结束日期
		startCompareDate: '', // 对比开始日期
		endCompareDate: '', // 对比结束日期
		minValidDate: '315507600', //最小可用时间，控制日期选择器的可选力度
		maxValidDate: '', // 最大可用时间，与stopToday 配置互斥
		success: function (obj) { return true; }, //回调函数，选择日期之后执行何种操作
		clear: function (obj) { return true; }, //回调函数，选择日期之后执行何种操作
		startDateId: 'vpastestartDate', // 开始日期输入框ID
		startCompareDateId: 'startCompareDate', // 对比开始日期输入框ID
		endDateId: 'vpasteendDate', // 结束日期输入框ID
		endCompareDateId: 'endCompareDate', // 对比结束日期输入框ID
		target: '', // 日期选择框的目标，一般为 <form> 的ID值
		needCompare: false, // 是否需要进行日期对比
		suffix: '', //相应控件的后缀
		inputTrigger: 'input_trigger',
		compareTrigger: 'compare_trigger',
		compareCheckboxId: 'needCompare', // 比较选择框
		calendars: 2, // 展示的月份数，最大是2
		dayRangeMax: 0, // 日期最大范围(以天计算)
		monthRangeMax: 12, // 日期最大范围(以月计算)
		dateTable: 'dateRangeDateTable', // 日期表格的CSS类
		selectCss: 'dateRangeSelected', // 时间选择的样式
		compareCss: 'dateRangeCompare', // 比较时间选择的样式
		coincideCss: 'dateRangeCoincide', // 重合部分的样式
		firstCss: 'first', //起始样式
		lastCss: 'last', //结束样式
		clickCss: 'today', //点击样式
		disableGray: 'dateRangeGray', // 非当前月的日期样式
		isToday: 'dateRangeToday', // 今天日期的样式
		joinLineId: 'joinLine',
		isSingleDay: false,
		defaultText: ' 至 ',
		singleCompare: false,
		stopToday: false,
		isTodayValid: true,
		weekendDis: false, //灰掉周末不可选。
		disCertainDay: [], //不可用的周日期设置数组，如：[1,3]是要周一， 周三 两天不可选，每个周的周一，周三都不可选择。
		disCertainDate: [],//不可用的日期设置数组，如:[1,3]是要1号，3号 两天不可选，特别的，[true,1,3]则反之，只有1，3可选，其余不可选。
		shortOpr: false, //结合单天日期选择的短操作，不需要确定和取消的操作按钮。
		noCalendar: false, //日期输入框是否展示
		theme: 'ta', //日期选择器的主题，目前支持 'gri' / 'ta'
		magicSelect: false, //用户自定义选择年、月，与{theme:ta}配合使用。
		autoCommit: false, //加载后立马自动提交
		autoSubmit: false, //没有确定，取消按钮，直接提交 
		replaceBtn: 'btn_compare',
		quickSelect: true, //是否在面板顶部展示快捷选择条（昨日/今日/本月/上月/上度/本年/最近7天/最近30天），默认开启，传 false 可关闭
		quickSelectLabels: null, //自定义快捷标签，null 用默认 ['昨日','今日','本月','上月','上度','本年','最近7天','最近30天']
		enableTimeMode: true, //是否启用 补零模式/时分秒模式 切换
		defaultStartTime: '00:00:00', //时分秒模式下开始时间默认值
		defaultEndTime: '23:59:59' //时分秒模式下结束时间默认值
	};
	//将对象赋给__method变量
	var __method = this;

	this.inputId = inputId;
	this.inputCompareId = inputId + 'Compare';
	this.compareInputDiv = 'div_compare_' + inputId;

			    // ===== 新增：注册实例 =====
    window.__pickerRegistry = window.__pickerRegistry || {};
    window.__pickerRegistry[inputId] = this;

	// 配置参数
	this.mOpts = $.extend({}, defaults, options);
	//默认日历参数最大是3
	this.mOpts.calendars = Math.min(this.mOpts.calendars, 3);
	//根据不同主题需要初始化的变量
	this.mOpts.compareCss = this.mOpts.theme == 'ta' ? this.mOpts.selectCss : this.mOpts.compareCss
	//昨天,今天,最近7天,最近14天,最近30天	
	this.periodObj = {};
	this.periodObj[__method.mOpts.aToday] = 0;
	this.periodObj[__method.mOpts.aYesterday] = 1;
	this.periodObj[__method.mOpts.aRecent7Days] = 6;
	this.periodObj[__method.mOpts.aRecent14Days] = 13;
	this.periodObj[__method.mOpts.aRecent30Days] = 29;
	this.periodObj[__method.mOpts.aRecent90Days] = 89;
	// 记录初始默认时间
	this.startDefDate = '';
	// 随机ID后缀
	var suffix = '' == this.mOpts.suffix ? (new Date()).getTime() : this.mOpts.suffix;
	// 日期选择框DIV的ID
	this.calendarId = 'calendar_' + suffix;
	// 日期列表DIV的ID
	this.dateListId = 'dateRangePicker_' + suffix;
	// 日期比较层
	this.dateRangeCompareDiv = 'dateRangeCompareDiv_' + suffix;
	//日期选择层
	this.dateRangeDiv = 'dateRangeDiv_' + suffix;
	// 日期对比选择控制的checkbox
	this.compareCheckBoxDiv = 'dateRangeCompareCheckBoxDiv_' + suffix;
	// 时间选择的确认按钮
	this.submitBtn = 'submit_' + suffix;
	// 时间清空按钮
	this.clearBtn = 'clear_' + suffix;
	// 日期选择框关闭按钮
	this.closeBtn = 'closeBtn_' + suffix;
	// 上一个月的按钮
	this.preMonth = 'dateRangePreMonth_' + suffix;
	// 下一个月的按钮
	this.nextMonth = 'dateRangeNextMonth_' + suffix;
	// 表单中开始、结束、开始对比、结束对比时间
	this.startDateId = this.mOpts.startDateId + '_' + suffix;
	this.endDateId = this.mOpts.endDateId + '_' + suffix;
	this.compareCheckboxId = this.mOpts.compareCheckboxId + '_' + suffix;
	this.startCompareDateId = this.mOpts.startCompareDateId + '_' + suffix;
	this.endCompareDateId = this.mOpts.endCompareDateId + '_' + suffix;
	// ★ 新增：面板内快捷选择条 / 时分秒模式相关元素ID
	this.quickBarId = 'qsQuickBar_' + suffix;
	this.timeModeBtnId = 'qsTimeModeBtn_' + suffix;
	this.startTimeId = 'qsStartTime_' + suffix;
	this.endTimeId = 'qsEndTime_' + suffix;
	// ★ 新增：footer 时间行 / 长短时间格式切换链接 / 层级选择器（级联列表面板）元素ID
	this.timeRowId = 'qsTimeRow_' + suffix;
	this.formatBadgeId = 'qsFmtBadge_' + suffix;
	this.cascaderId = this.calendarId + '_csc';
	// ★ 时分秒模式开关（false=默认补零模式，只到天）
	this.timeMode = false;
	// ★ 时间格式：'short'=到分 HH:mm（默认）/ 'long'=到秒 HH:mm:ss，由 format="short/long" 属性或格式切换链接决定
	this.timeFormat = 'short';
	// ★ "上月"逐月回溯标记：第一次点击基于现实时间，之后基于当前结束日期回溯
	this._lmChained = false;
	// ★ 层级选择器：当前打开侧（'start' / 'end' / ''）与各列字段状态
	this._cscSide = '';
	this._cscState = null;
	// 初始化日期选择器面板的HTML代码串
	var wrapper = {
		gri: [
			'<div id="' + this.calendarId + '" class="gri_dateRangeCalendar">',
			'<table class="gri_dateRangePicker"><tr id="' + this.dateListId + '"></tr></table>',
			'<div class="gri_dateRangeOptions" ' + (this.mOpts.autoSubmit ? ' style="display:none" ' : '') + '>',
			'<div class="gri_dateRangeInput" id="' + this.dateRangeDiv + '" >',
			'<input type="text" class="gri_dateRangeInput" name="' + this.startDateId + '" id="' + this.startDateId + '" value="' + this.mOpts.startDate + '" readonly />',
			'<span id="' + this.mOpts.joinLineId + '"> - </span>',
			'<input type="text" class="gri_dateRangeInput" name="' + this.endDateId + '" id="' + this.endDateId + '" value="' + this.mOpts.endDate + '" readonly /><br />',
			'</div>',
			'<div class="gri_dateRangeInput" id="' + this.dateRangeCompareDiv + '">',
			'<input type="text" class="gri_dateRangeInput" name="' + this.startCompareDateId + '" id="' + this.startCompareDateId + '" value="' + this.mOpts.startCompareDate + '" readonly />',
			'<span class="' + this.mOpts.joinLineId + '"> - </span>',
			'<input type="text" class="gri_dateRangeInput" name="' + this.endCompareDateId + '" id="' + this.endCompareDateId + '" value="' + this.mOpts.endCompareDate + '" readonly />',
			'</div>',
			'<div>',
			'<input type="button" name="' + this.submitBtn + '" id="' + this.submitBtn + '" value="确定" />',
			'&nbsp;<a id="' + this.closeBtn + '" href="javascript:;">关闭</a>',
			'</div>',
			'</div>',
			'</div>'
		],
		ta: [
			'<div id="' + this.calendarId + '" class="ta_calendar ta_calendar2 cf">',
			'<div class="ta_calendar_cont cf" id="' + this.dateListId + '">',
			//'<table class="dateRangePicker"><tr id="' + this.dateListId + '"></tr></table>',
			'</div>',
			'<div class="ta_calendar_footer cf" ' + (this.mOpts.autoSubmit ? ' style="display:none" ' : '') + '>',
			'<div class="frm_msg">',
			// ★ 时间行（参考图布局：开始时间/结束时间输入框 + 右侧格式切换与年月日切换；仅 enableTimeMode 时渲染）
			(this.mOpts.enableTimeMode ? '<div class="qs_time_row" id="' + this.timeRowId + '"><span class="qs_time_only" style="display:none"><span class="qs_time_lbl">开始时间</span><input type="text" class="qs_vipt" id="' + this.startTimeId + '" value="" maxlength="8" autocomplete="off" readonly placeholder="点击选择" /><span class="qs_time_lbl">结束时间</span><input type="text" class="qs_vipt" id="' + this.endTimeId + '" value="" maxlength="8" autocomplete="off" readonly placeholder="点击选择" /><a href="javascript:void(0)" class="qs_format_toggle" id="' + this.formatBadgeId + '">短时间格式</a></span><a href="javascript:void(0)" class="qs_time_toggle" id="' + this.timeModeBtnId + '">时分秒</a></div>' : ''),
			'<div id="' + this.dateRangeDiv + '">',
			'<input type="text" class="ta_ipt_text_s" name="' + this.startDateId + '" id="' + this.startDateId + '" value="' + this.mOpts.startDate + '" readonly style="display:none" />',
			'<input type="text" class="qs_vipt" id="' + this.startDateId + '_v" value="" readonly placeholder="点击选择" />',
			'<span class="' + this.mOpts.joinLineId + '"> - </span>',
			'<input type="text" class="ta_ipt_text_s" name="' + this.endDateId + '" id="' + this.endDateId + '" value="' + this.mOpts.endDate + '" readonly style="display:none" />',
			'<input type="text" class="qs_vipt" id="' + this.endDateId + '_v" value="" readonly placeholder="点击选择" />',
			'</div>',
			'<div id="' + this.dateRangeCompareDiv + '">',
			'<input type="text" class="ta_ipt_text_s" name="' + this.startCompareDateId + '" id="' + this.startCompareDateId + '" value="' + this.mOpts.startCompareDate + '" readonly />',
			'<span class="' + this.mOpts.joinLineId + '"> - </span>',
			'<input type="text" class="ta_ipt_text_s" name="' + this.endCompareDateId + '" id="' + this.endCompareDateId + '" value="' + this.mOpts.endCompareDate + '" readonly />',
			'</div>',
			'</div>',
			'<div class="frm_btn">',
			'<a id="' + this.clearBtn + '" href="javascript:void(0)"  >清空</a>',
			'<input class="ta_btn ta_btn_primary" type="button" name="' + this.submitBtn + '" id="' + this.submitBtn + '" value="确定" />',
			'<input class="ta_btn" type="button" id="' + this.closeBtn + '" value="取消"/>',
			'</div>',
			'</div>',
			'</div>'
		]
	};


	//对比日期框体的html串
	var checkBoxWrapper = {
		gri: [
			'<label class="gri_contrast" for ="' + this.compareCheckboxId + '">',
			'<input type="checkbox" class="gri_pc" name="' + this.compareCheckboxId + '" id="' + this.compareCheckboxId + '" value="1"/>对比',
			'</label>',
			'<input type="text" name="' + this.inputCompareId + '" id="' + this.inputCompareId + '" value="" class="gri_date"/>'
		],
		ta: [
			'<label class="contrast" for ="' + this.compareCheckboxId + '">',
			'<input type="checkbox" class="pc" name="' + this.compareCheckboxId + '" id="' + this.compareCheckboxId + '" value="1"/>对比',
			'</label>',
			'<div class="ta_date" id="' + this.compareInputDiv + '">',
			'	<span name="dateCompare" id="' + this.inputCompareId + '" class="date_title"></span>',
			'	<a class="opt_sel" id="' + this.mOpts.compareTrigger + '" href="#">',
			'		<i class="i_orderd"></i>',
			'	</a>',
			'</div>'
		]
	};
	//把checkbox放到页面的相应位置,放置到inputid后面 added by johnnyzheng

	if (this.mOpts.theme == 'ta') {
		$(checkBoxWrapper[this.mOpts.theme].join('')).insertAfter($('#div_' + this.inputId));
	} else {
		$(checkBoxWrapper[this.mOpts.theme].join('')).insertAfter($('#' + this.inputId));
	}
	//根据传入参数决定是否展示日期输入框
	if (this.mOpts.noCalendar) {
		$('#' + this.inputId).css('display', 'none');
		$('#' + this.compareCheckboxId).parent().css('display', 'none');
	}
	// 把时间选择框放到页面中
	$(0 < $('#appendParent').length ? '#appendParent' : document.body).append(wrapper[this.mOpts.theme].join(''));
	$('#' + this.calendarId).css('z-index', 19999);

	// ★ ============ 面板内快捷选择条 ============
	if (this.mOpts.quickSelect) {
		var labels = this.mOpts.quickSelectLabels || ['昨日', '今日', '本月', '上月', '上度', '本年', '最近7天', '最近30天'];
		var $bar = $('<div class="qs_quick_bar" id="' + this.quickBarId + '"></div>');
		$.each(labels, function (i, l) {
			$bar.append('<a href="javascript:void(0)" class="qs_quick_btn" data-label="' + l + '">' + l + '</a>');
		});
		$('#' + this.calendarId).prepend($bar);
		$bar.on('click', '.qs_quick_btn', function () {
			$bar.find('.qs_quick_btn').removeClass('active');
			$(this).addClass('active');
			var range = __method.calcQuickRange($(this).attr('data-label'));
			var s = __method.formatDate(range[0].join('-'));
			var e = __method.formatDate(range[1].join('-'));
			// 填入面板内日期输入框与页面隐藏域（纯日期，时分秒由 composeTimeValues 处理）
			$('#' + __method.startDateId).val(s);
			$('#' + __method.endDateId).val(e);
			__method.updateViewInputs(); // ★ 新增：刷新 footer 触发框显示
			$('#' + __method.mOpts.startDateId).val(s);
			$('#' + __method.mOpts.endDateId).val(e);
			if (__method.mOpts.enableTimeMode && __method.timeMode) {
				// 保留用户已自定义的时分秒（非法值由 composeTimeValues 内部回退为默认），不强制重置
				__method.composeTimeValues();
			} else {
				var _tv = s + __method.mOpts.defaultText + e;
				var _el = document.getElementById(__method.inputId);
				(_el && _el.tagName == 'INPUT') ? $('#' + __method.inputId).val(_tv) : $('#' + __method.inputId).html(_tv);
			}
			// 重绘日历到所选范围并刷新选中样式（先解绑月份切换，避免 init 重复绑定）
			__method.mOpts.endDate = e;
			$('#' + __method.preMonth).unbind('click');
			$('#' + __method.nextMonth).unbind('click');
			__method.init(false);
			return false;
		});
	}

	// ★ ============ 补零模式 / 时分秒模式切换 + 长/短时间格式切换（均位于时间行右侧，参考图布局） ============
	if (this.mOpts.enableTimeMode) {
		$('#' + this.timeModeBtnId).on('click', function () {
			__method.timeMode = !__method.timeMode;
			$(this).text(__method.timeMode ? '年月日' : '时分秒').toggleClass('on', __method.timeMode);
			if (!__method.timeMode) {
				// 切回补零模式：隐藏域恢复为纯日期
				$('#' + __method.mOpts.startDateId).val($('#' + __method.startDateId).val());
				$('#' + __method.mOpts.endDateId).val($('#' + __method.endDateId).val());
				var _el = document.getElementById(__method.inputId);
				var _tv = $('#' + __method.startDateId).val() + ('' == $('#' + __method.endDateId).val() ? '' : __method.mOpts.defaultText + $('#' + __method.endDateId).val());
				(_el && _el.tagName == 'INPUT') ? $('#' + __method.inputId).val(_tv) : $('#' + __method.inputId).html(_tv);
			} else {
				// ★ 进入时分秒模式：按当前格式回填时间（空/非法回退默认），再组合进隐藏域
				__method._applyTimeFormatToInputs();
				__method.composeTimeValues();
			}
			// ★ 时间输入框与格式切换链接仅在时分秒模式显示；级联面板若正打开则关闭
			__method._syncTimeRow();
			if ($('#' + __method.cascaderId).is(':visible')) {
				$('#' + __method.cascaderId).hide();
				__method._cscSide = '';
			}
			__method.updateViewInputs(); // ★ 新增：刷新触发框显示
			__method.updateBadge();      // ★ 新增：刷新格式切换链接文字
			return false;
		});
		// ★ 长/短时间格式切换链接（紧邻 年月日/时分秒 切换链接，仅时间显示时可见）
		$('#' + this.formatBadgeId).on('click', function () {
			__method.timeFormat = ('long' == __method.timeFormat) ? 'short' : 'long';
			// 按新格式重排两个时间输入框（short 截断到分 / long 补 :00）
			__method._applyTimeFormatToInputs();
			$(this).text('long' == __method.timeFormat ? '长时间格式' : '短时间格式');
			// 时间级联列数随格式变化，若正打开则关闭
			if ($('#' + __method.cascaderId).is(':visible') && 'time' == __method._cscKind) {
				$('#' + __method.cascaderId).hide();
				__method._cscSide = '';
			}
			__method.composeTimeValues();
			return false;
		});
		// ★ 主输入框 format 属性——'short'=到分 / 'long'=到秒，载入即进入时分秒模式；不传保持默认只到天
		var _fmtAttr = $('#' + this.inputId).attr('format');
		if ('long' == _fmtAttr || 'short' == _fmtAttr) {
			this.timeFormat = ('long' == _fmtAttr) ? 'long' : 'short';
			this.timeMode = true;
			$('#' + this.timeModeBtnId).text('年月日').addClass('on');
			this._applyTimeFormatToInputs();
		}
		// ★ 新增：初始化时间行显隐与格式切换链接文字
		this._syncTimeRow();
		this.updateBadge();
	}
	// ★ ============ footer 触发框 + 层级选择器（仅 ta 主题；原日期文本框保留为隐藏状态载体） ============
	if (this.mOpts.theme == 'ta') {
		// 级联面板容器：与主面板同一父节点，绝对定位、独立于日历 DOM，主面板关闭时一并隐藏
		$(0 < $('#appendParent').length ? '#appendParent' : document.body)
			.append('<div id="' + this.cascaderId + '" class="qs_cascader"></div>');
		// 容器点击阻止冒泡，避免触发 document click 关闭主面板
		$('#' + this.cascaderId).on('click', function (e) { e.stopPropagation(); });
		// ★ 新增：底部"确定"按钮——提交级联当前显示值并关闭（只改月份/时分、或一列没点都能明确确认）
		$('#' + this.cascaderId).on('click', '.qs_csc_confirm', function () {
			// ★ 改造：确定=提交级联当前显示值（未点过任何列时把默认高亮值写入载体，修复"没选日期直接点确定取不到值"）
			var st = __method._cscState;
			if (st) {
				var z = function (x) { return ('0' + x).slice(-2); };
				if ('time' == __method._cscKind) {
					var tIpt = 'start' == __method._cscSide ? __method.startTimeId : __method.endTimeId;
					if (!$('#' + tIpt).val()) {
						$('#' + tIpt).val('long' == __method.timeFormat ? z(st.h) + ':' + z(st.mi) + ':' + z(st.s) : z(st.h) + ':' + z(st.mi));
					}
				} else {
					var dIpt = 'start' == __method._cscSide ? __method.startDateId : __method.endDateId;
					if (!$('#' + dIpt).val()) {
						var days = new Date(st.y, st.m, 0).getDate();
						$('#' + dIpt).val(st.y + '-' + z(st.m) + '-' + z(st.d > days ? days : st.d));
					}
				}
			}
			// ★ 开始>结束 时自动对换两侧日期与时间值（不弹窗）
			__method._ensureRangeOrder();
			// 与级联项点击一致的收尾：同步 mOpts、重绘日历、刷新显示与隐藏域
			__method.mOpts.startDate = $('#' + __method.startDateId).val();
			var _ev = $('#' + __method.endDateId).val();
			__method.mOpts.endDate = _ev || __method.mOpts.startDate || __method.mOpts.endDate;
			$('#' + __method.preMonth).unbind('click');
			$('#' + __method.nextMonth).unbind('click');
			__method.init(false);
			__method.updateViewInputs();
			if (__method.mOpts.enableTimeMode && __method.timeMode) {
				__method.composeTimeValues();
			}
			$('#' + __method.cascaderId).hide();
			__method._cscSide = '';
			return false;
		});
		// 级联项点击（事件委托，重建列后仍生效；日期级联=年/月/日，时间级联=时/分/秒，互不合并）
		$('#' + this.cascaderId).on('click', '.qs_csc_it', function () {
			var c = $(this).closest('.qs_csc_col').attr('data-c');
			var v = $(this).attr('data-v') * 1;
			var st = __method._cscState;
			if (!st) { return false; }
			var side = __method._cscSide;
			var z = function (x) { return ('0' + x).slice(-2); };
			// 本列高亮切换
			$(this).closest('.qs_csc_col').find('.qs_csc_it').removeClass('on');
			$(this).addClass('on');
			if ('time' == __method._cscKind) {
				// ★ 时间级联：短格式=时|分两列（到分），长格式=时|分|秒三列（到秒）；选完最后一列自动关闭
				var timeIpt = 'start' == side ? __method.startTimeId : __method.endTimeId;
				if ('h' == c) { st.h = v; } else if ('mi' == c) { st.mi = v; } else { st.s = v; }
				$('#' + timeIpt).val('long' == __method.timeFormat ? z(st.h) + ':' + z(st.mi) + ':' + z(st.s) : z(st.h) + ':' + z(st.mi));
				// ★ 新增：开始>结束 时自动对换两侧日期与时间值（不弹窗）
				__method._ensureRangeOrder();
				if (__method.mOpts.enableTimeMode && __method.timeMode) {
					__method.composeTimeValues();
				}
				if (('long' == __method.timeFormat && 's' == c) || ('long' != __method.timeFormat && 'mi' == c)) { // ★ 最后一列选完即关闭级联
					$('#' + __method.cascaderId).hide();
					__method._cscSide = '';
				}
				return false;
			}
			// ★ 日期级联：年/月/日三列（时分秒不合并进来）
			var dateIpt = 'start' == side ? __method.startDateId : __method.endDateId;
			if ('y' == c) { st.y = v; } else if ('m' == c) { st.m = v; } else { st.d = v; }
			// 按已选年月钳制日并重建日列（复用原"按年月算天数并钳制"逻辑）
			var days = new Date(st.y, st.m, 0).getDate();
			if (st.d > days) { st.d = days; }
			$('#' + __method.cascaderId).find('.qs_csc_col[data-c="d"] .qs_csc_list')
				.html(__method._cscItems('d', 1, days, st.d));
			// 写入该侧日期状态载体（纯 'yyyy-MM-dd'）
			$('#' + dateIpt).val(st.y + '-' + z(st.m) + '-' + z(st.d));
			// ★ 新增：开始>结束 时自动对换两侧日期与时间值（不弹窗）
			__method._ensureRangeOrder();
			// 与快捷条一致的后续流程：更新 mOpts、移除快捷条选中态、解绑月份切换后重绘日历
			__method.mOpts.startDate = $('#' + __method.startDateId).val();
			var _ev = $('#' + __method.endDateId).val();
			__method.mOpts.endDate = _ev || __method.mOpts.startDate || __method.mOpts.endDate;
			if (__method.mOpts.quickSelect) {
				$('#' + __method.quickBarId).find('.qs_quick_btn').removeClass('active');
			}
			$('#' + __method.preMonth).unbind('click');
			$('#' + __method.nextMonth).unbind('click');
			__method.init(false);
			if (__method.mOpts.enableTimeMode && __method.timeMode) {
				__method.composeTimeValues();
			}
			// 刷新触发框显示；日为最后一列，选完即关闭级联
			__method.updateViewInputs();
			if ('d' == c) {
				$('#' + __method.cascaderId).hide();
				__method._cscSide = '';
			}
			return false;
		});
		// 触发框点击：开/关本侧级联；点击另一侧触发框则级联切换
		$('#' + this.startDateId + '_v').on('click', function () { __method.toggleCascader('start', 'date'); });
		$('#' + this.endDateId + '_v').on('click', function () { __method.toggleCascader('end', 'date'); });
		// ★ 时间输入框点击：弹出本侧 时/分/秒 级联
		$('#' + this.startTimeId).on('click', function () { __method.toggleCascader('start', 'time'); });
		$('#' + this.endTimeId).on('click', function () { __method.toggleCascader('end', 'time'); });
		// ★ 新增：初始按状态载体输入框当前值（mOpts.startDate/endDate）刷新触发框显示
		this.updateViewInputs();
	}
	// 初始化目标地址的元素
	if (1 > $('#' + this.mOpts.startDateId).length) {
		$('' != this.mOpts.target ? '#' + this.mOpts.target : 'body').append('<input type="hidden" id="' + this.mOpts.startDateId + '" name="' + this.mOpts.startDateId + '" value="' + this.mOpts.startDate + '" />');
	} else {
		$('#' + this.mOpts.startDateId).val(this.mOpts.startDate);
	}
	if (1 > $('#' + this.mOpts.endDateId).length) {
		$('' != this.mOpts.target ? '#' + this.mOpts.target : 'body').append('<input type="hidden" id="' + this.mOpts.endDateId + '" name="' + this.mOpts.endDateId + '" value="' + this.mOpts.endDate + '" />');
	} else {
		$('#' + this.mOpts.endDateId).val(this.mOpts.endDate);
	}
	if (1 > $('#' + this.mOpts.compareCheckboxId).length) {
		$('' != this.mOpts.target ? '#' + this.mOpts.target : 'body').append('<input type="checkbox" id="' + this.mOpts.compareCheckboxId + '" name="' + this.mOpts.compareCheckboxId + '" value="0" style="display:none;" />');
	}
	// 如果不需要比较日期，则需要隐藏比较部分的内容
	if (false == this.mOpts.needCompare) {
		$('#' + this.compareInputDiv).css('display', 'none');
		$('#' + this.compareCheckBoxDiv).css('display', 'none');
		$('#' + this.dateRangeCompareDiv).css('display', 'none');
		$('#' + this.compareCheckboxId).attr('disabled', true);
		$('#' + this.startCompareDateId).attr('disabled', true);
		$('#' + this.endCompareDateId).attr('disabled', true);
		//隐藏对比的checkbox
		$('#' + this.compareCheckboxId).parent().css('display', 'none');
		$('#' + this.mOpts.replaceBtn).length > 0 && $('#' + this.mOpts.replaceBtn).hide();
	} else {
		if (1 > $('#' + this.mOpts.startCompareDateId).length) {
			$('' != this.mOpts.target ? '#' + this.mOpts.target : 'body').append('<input type="hidden" id="' + this.mOpts.startCompareDateId + '" name="' + this.mOpts.startCompareDateId + '" value="' + this.mOpts.startCompareDate + '" />');
		} else {
			$('#' + this.mOpts.startCompareDateId).val(this.mOpts.startCompareDate);
		}
		if (1 > $('#' + this.mOpts.endCompareDateId).length) {
			$('' != this.mOpts.target ? '#' + this.mOpts.target : 'body').append('<input type="hidden" id="' + this.mOpts.endCompareDateId + '" name="' + this.mOpts.endCompareDateId + '" value="' + this.mOpts.endCompareDate + '" />');
		} else {
			$('#' + this.mOpts.endCompareDateId).val(this.mOpts.endCompareDate);
		}
		if ('' == this.mOpts.startCompareDate || '' == this.mOpts.endCompareDate) {
			$('#' + this.compareCheckboxId).attr('checked', false);
			$('#' + this.mOpts.compareCheckboxId).attr('checked', false);
		} else {
			$('#' + this.compareCheckboxId).attr('checked', true);
			$('#' + this.mOpts.compareCheckboxId).attr('checked', true);
		}

	}
	// 输入框焦点定在第一个输入框
	this.dateInput = this.startDateId;
	// 为新的输入框加背景色
	this.changeInput(this.dateInput);

	// 开始时间 input 的 click 事件
	$('#' + this.startDateId).bind('click', function () {
		// 如果用户在选择基准结束时间时，换到对比时间了，则
		if (__method.endCompareDateId == __method.dateInput) {
			$('#' + __method.startCompareDateId).val(__method.startDefDate);
		}
		__method.startDefDate = '';
		__method.removeCSS(1);
		//__method.addCSS(1);
		__method.changeInput(__method.startDateId);
		return false;
	});
	$('#' + this.calendarId).bind('click', function (event) {
		//event.preventDefault();
		// 防止冒泡
		event.stopPropagation();
	});
	// 开始比较时间 input 的 click 事件
	$('#' + this.startCompareDateId).bind('click', function () {
		// 如果用户在选择基准结束时间时，换到对比时间了，则
		if (__method.endDateId == __method.dateInput) {
			$('#' + __method.startDateId).val(__method.startDefDate);
		}
		__method.startDefDate = '';
		__method.removeCSS(0);
		//__method.addCSS(0);
		__method.changeInput(__method.startCompareDateId);
		return false;
	});
	/**
	 * 设置回调句柄，点击成功后，返回一个时间对象，包含开始结束时间
	 * 和对比开始结束时间
	 */
	$('#' + this.submitBtn).bind('click', function () {
		// ★ 新增：保存前若 开始>结束 自动对换两侧日期与时间值（不弹窗）
		__method._ensureRangeOrder();
		__method.close(1);
		__method.mOpts.success({
			'startDate': $('#' + __method.mOpts.startDateId).val(),
			'endDate': $('#' + __method.mOpts.endDateId).val(),
			'needCompare': $('#' + __method.mOpts.compareCheckboxId).val(),
			'startCompareDate': $('#' + __method.mOpts.startCompareDateId).val(),
			'endCompareDate': $('#' + __method.mOpts.endCompareDateId).val()
		});
		try {
			if (typeof readPageData === 'function') {
				readPageData(1);
			} else if ($input[0] && $input[0].ownerDocument && typeof $input[0].ownerDocument.defaultView.readPageData === 'function') {
				$input[0].ownerDocument.defaultView.readPageData(1);
			} else if (window.parent && typeof window.parent.readPageData === 'function') {
				window.parent.readPageData(1);
			}
		} catch(e) {}
		return false;
	});
	$('#' + this.clearBtn).bind('click', function () {

		__method.close();
		__method.clear();

		__method.mOpts.clear();
		return false;
	});
	// 日期选择关闭按钮的 click 事件
	$('#' + this.closeBtn).bind('click', function () {
		__method.close();
		return false;
	});
	// 为输入框添加click事件
	$('#' + this.inputId).bind('click', function () {
		__method.init();
		__method.show(false, __method);
		return false;
	});
	$('#' + this.mOpts.inputTrigger).bind('click', function () {
		__method.init();
		__method.show(false, __method);
		return false;
	});
	$('#' + this.mOpts.compareTrigger).bind('click', function () {
		__method.init(true);
		__method.show(true, __method);
		return false;
	});
	// 为输入框添加click事件
	$('#' + this.inputCompareId).bind('click', function () {
		__method.init(true);
		__method.show(true, __method);
		return false;
	});

	//判断是否是实时数据,如果是将时间默认填充进去 added by johnnyzheng 12-06
	if (this.mOpts.singleCompare) {
		if (this.mOpts.theme === 'ta') {
			$('#' + __method.startDateId).val(__method.mOpts.startDate);
			$('#' + __method.endDateId).val(__method.mOpts.startDate);
			$('#' + __method.startCompareDateId).val(__method.mOpts.startCompareDate);
			$('#' + __method.endCompareDateId).val(__method.mOpts.startCompareDate);
		}
		else {
			$('#' + __method.startDateId).val(__method.mOpts.startDate);
			$('#' + __method.endDateId).val(__method.mOpts.startDate);
			$('#' + __method.startCompareDateId).val(__method.mOpts.startCompareDate);
			$('#' + __method.endCompareDateId).val(__method.mOpts.startCompareDate);
			$('#' + this.compareCheckboxId).attr('checked', true);
			$('#' + this.mOpts.compareCheckboxId).attr('checked', true);
		}


	}
	// 时间对比
	$('#' + this.dateRangeCompareDiv).css('display', $('#' + this.compareCheckboxId).attr('checked') ? '' : 'none');
	$('#' + this.compareInputDiv).css('display', $('#' + this.compareCheckboxId).attr('checked') ? '' : 'none');
	$('#' + this.compareCheckboxId).bind('click', function () {
		$('#' + __method.inputCompareId).css('display', this.checked ? '' : 'none');
		// 隐藏对比时间选择
		$('#' + __method.dateRangeCompareDiv).css('display', this.checked ? '' : 'none');
		$('#' + __method.compareInputDiv).css('display', this.checked ? '' : 'none');
		// 把两个对比时间框置为不可用
		$('#' + __method.startCompareDateId).css('disabled', this.checked ? false : true);
		$('#' + __method.endCompareDateId).css('disabled', this.checked ? false : true);
		// 修改表单的 checkbox 状态
		$('#' + __method.mOpts.compareCheckboxId).attr('checked', $('#' + __method.compareCheckboxId).attr('checked'));
		// 修改表单的值
		$('#' + __method.mOpts.compareCheckboxId).val($('#' + __method.compareCheckboxId).attr('checked') ? 1 : 0);
		// 初始化选框背景
		if ($('#' + __method.compareCheckboxId).attr('checked')) {
			sDate = __method.str2date($('#' + __method.startDateId).val());
			sTime = sDate.getTime();
			eDate = __method.str2date($('#' + __method.endDateId).val());
			eTime = eDate.getTime();
			scDate = $('#' + __method.startCompareDateId).val();
			ecDate = $('#' + __method.endCompareDateId).val();
			if ('' == scDate || '' == ecDate) {
				ecDate = __method.str2date(__method.date2ymd(sDate).join('-'));
				ecDate.setDate(ecDate.getDate() - 1);
				scDate = __method.str2date(__method.date2ymd(sDate).join('-'));
				scDate.setDate(scDate.getDate() - ((eTime - sTime) / 86400000) - 1);
				//这里要和STATS_START_TIME的时间进行对比，如果默认填充的对比时间在这个时间之前 added by johnnyzheng
				if (ecDate.getTime() < __method.mOpts.minValidDate * 1000) {
					scDate = sDate;
					ecDate = eDate;
				}
				if (ecDate.getTime() >= __method.mOpts.minValidDate * 1000 && scDate.getTime() < __method.mOpts.minValidDate * 1000) {
					scDate.setTime(__method.mOpts.minValidDate * 1000)
					scDate = __method.str2date(__method.date2ymd(scDate).join('-'));
					ecDate.setDate(scDate.getDate() + ((eTime - sTime) / 86400000) - 1);
				}
				$('#' + __method.startCompareDateId).val(__method.formatDate(__method.date2ymd(scDate).join('-')));
				$('#' + __method.endCompareDateId).val(__method.formatDate(__method.date2ymd(ecDate).join('-')));
			}
			__method.addCSS(1);
			// 输入框焦点切换到比较开始时间
			__method.changeInput(__method.startCompareDateId);

		} else {
			__method.removeCSS(1);
			// 输入框焦点切换到开始时间
			__method.changeInput(__method.startDateId);
		}
		//用户点击默认自动提交 added by johnnyzheng 12-08
		__method.close(1);
		__method.mOpts.success({
			'startDate': $('#' + __method.mOpts.startDateId).val(),
			'endDate': $('#' + __method.mOpts.endDateId).val(),
			'needCompare': $('#' + __method.mOpts.compareCheckboxId).val(),
			'startCompareDate': $('#' + __method.mOpts.startCompareDateId).val(),
			'endCompareDate': $('#' + __method.mOpts.endCompareDateId).val()
		});
	});

	// 初始化开始
	this.init();
	// 关闭日期选择框，并把结果反显到输入框
	this.close(1);
	if (this.mOpts.replaceBtn && $('#' + this.mOpts.replaceBtn).length > 0) {
		$('#' + __method.compareCheckboxId).hide();
		$('.contrast').hide();
		$('#' + this.mOpts.replaceBtn).bind('click', function () {
			var self = this;
			$('#' + __method.compareCheckboxId).attr('checked')
				? $('#' + __method.compareCheckboxId).removeAttr('checked')
				: $('#' + __method.compareCheckboxId).attr('checked', 'checked');
			$('#' + __method.compareCheckboxId).click();
			$('#' + __method.compareCheckboxId).attr('checked')
				? function () {
					$('#' + __method.compareCheckboxId).removeAttr('checked');
					$('.contrast').hide();
					$(self).text('按时间对比');
				}()
				: function () {
					$('#' + __method.compareCheckboxId).attr('checked', 'checked');
					$('.contrast').show();
					$(self).text('取消对比');
				}();
		});
	}

	if (this.mOpts.autoCommit) {
		this.mOpts.success({
			'startDate': $('#' + __method.mOpts.startDateId).val(),
			'endDate': $('#' + __method.mOpts.endDateId).val(),
			'needCompare': $('#' + __method.mOpts.compareCheckboxId).val(),
			'startCompareDate': $('#' + __method.mOpts.startCompareDateId).val(),
			'endCompareDate': $('#' + __method.mOpts.endCompareDateId).val()
		});
	}
	//让用户点击页面即可关闭弹窗
	$(document).bind('click', function () {
		__method.close();
	});
};

/**
 * 程序化设置日期区间（外部业务调用）
 * 支持 start/end 为以下任意格式，内部自动适配 timeMode / timeFormat：
 *   'yyyy-MM-dd'                → daily（补零模式，只到天）
 *   'yyyy-MM-dd HH:mm'          → short（时分秒模式，到分）
 *   'yyyy-MM-dd HH:mm:ss'       → long（时分秒模式，到秒）
 * @param {String} inputId 主输入框ID
 * @param {String} start
 * @param {String} end
 */
function setPickerDate(inputId, start, end) {
    var picker = window.__pickerRegistry && window.__pickerRegistry[inputId];
    if (!picker) { return; }

    // ---- 1. 解析：把任意格式拆成 { d: 'yyyy-MM-dd', t: 'HH:mm[:ss]' | '' } ----
    function parse(v) {
        v = (v || '').trim();
        var m = v.match(/^(\d{4})-(\d{1,2})-(\d{1,2})(?:[ T](\d{1,2}):(\d{1,2})(?::(\d{1,2}))?)?$/);
        if (!m) { return { d: '', t: '' }; }
        var z = function (x) { return ('0' + x).slice(-2); };
        var d = m[1] + '-' + z(m[2]) + '-' + z(m[3]);
        var t = '';
        if (m[4] !== undefined) {
            t = z(m[4]) + ':' + z(m[5]) + (m[6] !== undefined ? ':' + z(m[6]) : '');
        }
        return { d: d, t: t };
    }
    var ps = parse(start), pe = parse(end);

    // ---- 2. 推断目标档位：daily / short / long ----
    // 取两边里更"精细"的那个时间长度作为整体格式
    var tLen = Math.max(ps.t.length, pe.t.length);   // 0 / 5(HH:mm) / 8(HH:mm:ss)
    var targetFormat = tLen === 0 ? 'daily' : (tLen > 5 ? 'long' : 'short');

    // ---- 3. 适配组件状态 ----
    picker.timeMode   = (targetFormat !== 'daily');
    picker.timeFormat = (targetFormat === 'long') ? 'long' : 'short';

    // ---- 4. 写面板内隐藏载体（纯日期） ----
    $('#' + picker.startDateId).val(ps.d);
    $('#' + picker.endDateId).val(pe.d);

    // ---- 5. 写时间输入框（仅 timeMode 时有意义） ----
    if (picker.timeMode) {
        $('#' + picker.startTimeId).val(
            ps.t || picker.mOpts.defaultStartTime.slice(0, targetFormat === 'long' ? 8 : 5)
        );
        $('#' + picker.endTimeId).val(
            pe.t || picker.mOpts.defaultEndTime.slice(0, targetFormat === 'long' ? 8 : 5)
        );
        // 按 targetFormat 统一截断/补齐（比如 short 只留 HH:mm）
        picker._applyTimeFormatToInputs();
    }

    // ---- 6. 写 mOpts ----
    picker.mOpts.startDate = ps.d;
    picker.mOpts.endDate   = pe.d;

    // ---- 7. 刷新可见 UI ----
    picker._syncTimeRow();        // 时间行显隐
    picker.updateBadge();         // 长/短格式链接文字
    picker.updateViewInputs();    // footer 日期触发框（_v）

    // ---- 8. 写隐藏域 + 主输入框（走组件自带的组合逻辑，保证和"确定"路径一致） ----
    if (picker.timeMode) {
        picker.composeTimeValues();   // 内部会把 yyyy-MM-dd + HH:mm[:ss] 写进 mOpts 隐藏域和 #inputId
    } else {
        // daily：隐藏域 = 纯日期，主输入框 = 日期  至  日期
        $('#' + picker.mOpts.startDateId).val(ps.d);
        $('#' + picker.mOpts.endDateId).val(pe.d);
        var el = document.getElementById(inputId);
        var tv = ps.d + ('' === pe.d ? '' : picker.mOpts.defaultText + pe.d);
        (el && el.tagName === 'INPUT') ? $('#' + inputId).val(tv) : $('#' + inputId).html(tv);
    }

    // ---- 9. 若面板正开着，重绘日历选中态 ----
    if ($('#' + picker.calendarId).is(':visible')) {
        $('#' + picker.preMonth).unbind('click');
        $('#' + picker.nextMonth).unbind('click');
        picker.init(false);
    }
}

/**
 * 暂未完成，后续需要基于当前时间格式 进行动态适配，否则会有问题
 * @param {*} inputId 
 * @param {*} start 
 * @param {*} end 
 * @returns 
 */
function setPickerDateV3(inputId, start, end) {
    var picker = window.__pickerRegistry && window.__pickerRegistry[inputId];
    if (!picker) {
        // 至少保证显示值一致；建议提示或抛错
        $('#' + inputId).val(start + picker ? picker.mOpts.defaultText : ' 至 ' + end);
        return;
    }

    var sep = picker.mOpts.defaultText;
    // 1. 主输入框
    $('#' + inputId).val(start + sep + end);
    $('#' + inputId).attr('datas', start).attr('datae', end);

    // 2. 面板内隐藏载体
    $('#' + picker.startDateId).val(start);
    $('#' + picker.endDateId).val(end);

    // 3. 页面隐藏域
    $('#' + picker.mOpts.startDateId).val(start);
    $('#' + picker.mOpts.endDateId).val(end);

    // 4. mOpts 同步
    picker.mOpts.startDate = start;
    picker.mOpts.endDate   = end;

    // 5. ★ 关键：刷新可见触发框（_v）显示
    picker.updateViewInputs();

    // 6. ★ 时分秒模式：要么重置为补零模式，要么同步时间
    if (picker.mOpts.enableTimeMode) {
        picker.timeMode = false;
        // 若要保留时间模式，应改为：
        // picker._applyTimeFormatToInputs();
        // picker.composeTimeValues();
        picker._syncTimeRow();
        picker.updateBadge();
    }

    // 7. 若面板正开着，重绘日历选中态
    if ($('#' + picker.calendarId).is(':visible')) {
        $('#' + picker.preMonth).unbind('click');
        $('#' + picker.nextMonth).unbind('click');
        picker.init(false);
    }
}

/**
 * @description 日期选择器的初始化方法，对象原型扩展
 * @param {Boolean} isCompare 标识当前初始化选择面板是否是对比日期
 */
pickerDateRange.prototype.init = function (isCompare) {
	var __method = this;
	var minDate, maxDate;
	var isNeedCompare = typeof (isCompare) != 'undefined' ? isCompare && $("#" + __method.compareCheckboxId).attr('checked') : $("#" + __method.compareCheckboxId).attr('checked');


	// 清空日期列表的内容
	$("#" + this.dateListId).empty();


	// 如果开始日期为空，则取当天的日期为开始日期
	var endDate = '' == this.mOpts.endDate ? (new Date()) : this.str2date(this.mOpts.endDate);
	// 日历结束时间
	this.calendar_endDate = new Date(endDate.getFullYear(), endDate.getMonth() + 1, 0);

	//如果是magicSelect 自定义年和月份，则自定义填充日期
	if (this.mOpts.magicSelect && this.mOpts.theme == 'ta') {
		var i = 0;
		do {
			var td = null;
			if (i == 0) {
				td = this.fillDate(this.str2date($('#' + this.endDateId).val()).getFullYear(), this.str2date($('#' + this.endDateId).val()).getMonth(), i);
				$("#" + this.dateListId).append(td);
			}
			else {
				td = this.fillDate(this.str2date($('#' + this.startDateId).val()).getFullYear(), this.str2date($('#' + this.startDateId).val()).getMonth(), i);
				var firstTd = (this.mOpts.theme == 'ta' ? $("#" + this.dateListId).find('table').get(0) : $("#" + this.dateListId).find('td').get(0));
				$(firstTd).before(td);
			}
			i++;
		} while (i < 2);
		// 日历开始时间
		this.calendar_startDate = new Date(this.str2date($('#' + this.startDateId).val()).getFullYear(), this.str2date($('#' + this.startDateId).val()).getMonth(), 1);

	} else {
		// 计算并显示以 endDate 为结尾的最近几个月的日期列表
		for (var i = 0; i < this.mOpts.calendars; i++) {
			var td = null;
			if (this.mOpts.theme == 'ta') {
				td = this.fillDate(endDate.getFullYear(), endDate.getMonth(), i);
			}
			else {
				td = document.createElement('td');
				$(td).append(this.fillDate(endDate.getFullYear(), endDate.getMonth(), i));
				$(td).css('vertical-align', 'top');
			}
			if (0 == i) {
				$("#" + this.dateListId).append(td);
			} else {
				var firstTd = (this.mOpts.theme == 'ta' ? $("#" + this.dateListId).find('table').get(0) : $("#" + this.dateListId).find('td').get(0));
				$(firstTd).before(td);
			}
			endDate.setMonth(endDate.getMonth() - 1, 1);
		}
		// 日历开始时间
		this.calendar_startDate = new Date(endDate.getFullYear(), endDate.getMonth() + 1, 1);
	}

	// 上一个月
	$('#' + this.preMonth).bind('click', function () {
		__method.calendar_endDate.setMonth(__method.calendar_endDate.getMonth() - 1, 1);
		__method.mOpts.endDate = __method.date2ymd(__method.calendar_endDate).join('-');
		__method.init(isCompare);
		//如果是单月选择的时候，要控制input输入框 added by johnnyzheng 2011-12-19
		if (1 == __method.mOpts.calendars) {
			if ('' == $('#' + __method.startDateId).val()) {
				__method.changeInput(__method.startDateId);
			}
			else {
				__method.changeInput(__method.endDateId);
			}
		}
		return false;
	});
	// 下一个月
	$('#' + this.nextMonth).bind('click', function () {
		__method.calendar_endDate.setMonth(__method.calendar_endDate.getMonth() + 1, 1);
		__method.mOpts.endDate = __method.date2ymd(__method.calendar_endDate).join('-');
		__method.init(isCompare);
		//如果是单月选择的时候，要控制input输入框 added by johnnyzheng 2011-12-19
		if (1 == __method.mOpts.calendars) {
			if ('' == $('#' + __method.startDateId).val()) {
				__method.changeInput(__method.startDateId);
			}
			else {
				__method.changeInput(__method.endDateId);
			}
		}
		return false;
	});

	//如果有用户自定义选择月份，则为其绑定事件
	if (this.mOpts.magicSelect) this.bindChangeForSelect();


	// 初始化时间选区背景
	if (this.endDateId != this.dateInput && this.endCompareDateId != this.dateInput) {
		(isNeedCompare && typeof (isCompare) != 'undefined') ? this.addCSS(1) : this.addCSS(0);
	}

	if (isNeedCompare && typeof (isCompare) != 'undefined') {
		__method.addCSS(1);
	}
	else {
		__method.addCSS(0);

	}

	// 隐藏对比日期框
	$('#' + __method.inputCompareId).css('display', isNeedCompare ? '' : 'none');
	$('#' + this.compareInputDiv).css('display', $('#' + this.compareCheckboxId).attr('checked') ? '' : 'none');
	//昨天，今天，最近7天，最近30天快捷的点击，样式要自己定义，id可以传递默认，也可覆盖
	for (var property in __method.periodObj) {
		if ($('#' + property).length > 0) {
			$('#' + property).unbind('click');
			$('#' + property).bind('click', function () {
				//处理点击样式
				var cla = __method.mOpts.theme == 'ta' ? 'active' : 'a';
				$(this).parent().nextAll().removeClass(cla);
				$(this).parent().prevAll().removeClass(cla);
				$(this).parent().addClass(cla);
				//拼接提交时间串
				var timeObj = __method.getSpecialPeriod(__method.periodObj[$(this).attr('id')]);
				$('#' + __method.startDateId).val(__method.formatDate(timeObj.otherday));
				$('#' + __method.endDateId).val(__method.formatDate(timeObj.today));
				$('#' + __method.mOpts.startDateId).val($('#' + __method.startDateId).val());
				$('#' + __method.mOpts.endDateId).val($('#' + __method.endDateId).val());
				__method.mOpts.theme == 'ta' ? $('#' + __method.compareInputDiv).hide() : $('#' + __method.inputCompareId).css('display', 'none');
				$('#' + __method.compareCheckboxId).attr('checked', false);
				$('#' + __method.mOpts.compareCheckboxId).attr('checked', false);
				$('#' + this.compareInputDiv).css('display', $('#' + this.compareCheckboxId).attr('checked') ? '' : 'none');
				__method.close(1);
				//于此同时清空对比时间框的时间
				$('#' + __method.startCompareDateId).val('');
				$('#' + __method.endCompareDateId).val('');
				$('#' + __method.mOpts.startCompareDateId).val('');
				$('#' + __method.mOpts.endCompareDateId).val('');
				$('#' + __method.mOpts.compareCheckboxId).val(0);

				if ($('#' + __method.mOpts.replaceBtn).length > 0) {
					$('.contrast').hide();
					$('#' + __method.mOpts.replaceBtn).text('按时间对比');
				}
				//点击提交
				__method.mOpts.success({
					'startDate': $('#' + __method.mOpts.startDateId).val(),
					'endDate': $('#' + __method.mOpts.endDateId).val(),
					'needCompare': $('#' + __method.mOpts.compareCheckboxId).val(),
					'startCompareDate': $('#' + __method.mOpts.startCompareDateId).val(),
					'endCompareDate': $('#' + __method.mOpts.endCompareDateId).val()
				});
			});
		}
	}

	// 让用户手动关闭或提交日历，每次初始化的时候绑定，关闭的时候解绑 by zacharycai
	$(document).bind('click', function () {
		__method.close();
	});

	//完全清空日期控件的值 by zacharycai
	$('#' + this.inputId).bind('change', function () {
		if ($(this).val() === '') {
			$('#' + __method.startDateId).val('');
			$('#' + __method.endDateId).val('');
			$('#' + __method.startCompareDateId).val('');
			$('#' + __method.endCompareDateId).val('');
		}
	})
};

pickerDateRange.prototype.bindChangeForSelect = function () {
	var __method = this;
	//气泡弹窗
	var _popup = function (btn, ctn, wrap, css) {
		css = css || 'open';
		var ITEMS_TIMEOUT = null, time_out = 500;

		function hidePop() {
			$('#' + ctn).removeClass(css);
		}

		function showPop() {
			$('#' + ctn).addClass(css);
		}

		function isPopShow() {
			return $('#' + ctn).attr('class') == css;
		}


		$("#" + btn).click(function () {
			isPopShow() ? hidePop() : showPop();
		}).mouseover(function () {
			clearTimeout(ITEMS_TIMEOUT);
		}).mouseout(function () {
			ITEMS_TIMEOUT = setTimeout(hidePop, time_out);
		});

		$('#' + wrap).mouseover(function () {
			clearTimeout(ITEMS_TIMEOUT);
		}).mouseout(function () {
			ITEMS_TIMEOUT = setTimeout(hidePop, time_out);
		});
	};

	//自定义选择的触发动作
	try {
		$("#" + this.dateListId).find('div[id*="selected"]').each(function () {
			//绑定pop
			var _match = $(this).attr('id').match(/(\w+)_(\d)/i);
			if (_match) {
				var _name = _match[1];//名称
				var _idx = _match[2];//下标

				if (_name == 'yselected') {
					_popup('_ybtn_' + _idx, $(this).attr('id'), '_yctn_' + _idx);
				}
				else if (_name == 'mselected') {
					_popup('_mbtn_' + _idx, $(this).attr('id'), '_mctn_' + _idx);
				}

				$(this).find('li a').each(function () {
					$(this).click(function () {
						var match = $(this).parents('.select_wrap').attr('id').match(/(\w+)_(\d)/i);
						//if(match){
						var name = match[1];//名称
						var idx = match[2];//下标
						var nt = null;
						if (idx ^ 1 == 0) {
							//开始
							if (name == 'yselected') {
								__method.calendar_startDate.setYear($(this).text() * 1, 1);
								//__method.calendar_startDate.setMonth(__method.str2date($('#'+__method.startDateId).val()).getMonth(), 1);
							}
							else if (name = 'mselected') {
								//__method.calendar_startDate.setYear(__method.str2date($('#'+__method.startDateId).val()).getFullYear(), 1);
								__method.calendar_startDate.setMonth($(this).text() * 1 - 1, 1);
							}
							__method.mOpts.startDate = __method.date2ymd(__method.calendar_startDate).join('-');
							nt = __method.fillDate(__method.calendar_startDate.getFullYear(), __method.calendar_startDate.getMonth(), idx);
						}
						else {
							//结束
							if (name == 'yselected') {
								__method.calendar_endDate.setYear($(this).text() * 1, 1);
								//__method.calendar_endDate.setMonth(__method.str2date($('#'+__method.endDateId).val()).getMonth(), 1);
							}
							else if (name = 'mselected') {
								//__method.calendar_endDate.setYear(__method.str2date($('#'+__method.endDateId).val()).getFullYear(), 1);
								__method.calendar_endDate.setMonth($(this).text() * 1 - 1, 1);
							}
							__method.mOpts.endDate = __method.date2ymd(__method.calendar_endDate).join('-');
							nt = __method.fillDate(__method.calendar_endDate.getFullYear(), __method.calendar_endDate.getMonth(), idx);
						}
						var tb = $("#" + __method.dateListId).find('table').get(idx ^ 1);
						$(tb).replaceWith(nt);
						//}
						__method.removeCSS(0);
						__method.bindChangeForSelect();
					});
				});
			}
		});
	} catch (e) {
		window.console && console.log(e);
	}
}
/**
 * @description 计算今天，昨天，最近7天，最近30天返回的时间范围
 * @param {Num} period 快捷选择的时间段，今天、昨天、最近7天、最近30天
 */
pickerDateRange.prototype.getSpecialPeriod = function (period) {
	var __method = this;
	var date = new Date();
	//如果今天不可用，则从昨天向前推 added by johnnyzheng 12-07
	(true == __method.mOpts.isTodayValid && ('' != __method.mOpts.isTodayValid) || 2 > period) ? '' : date.setTime(date.getTime() - (1 * 24 * 60 * 60 * 1000));
	var timeStamp = ((date.getTime() - (period * 24 * 60 * 60 * 1000)) < (__method.mOpts.minValidDate * 1000)) ? (__method.mOpts.minValidDate * 1000) : (date.getTime() - (period * 24 * 60 * 60 * 1000));
	var todayStr = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
	date.setTime(timeStamp);
	var otherdayStr = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
	if (period == __method.periodObj.aYesterday) {
		todayStr = otherdayStr;
	}
	return { today: todayStr, otherday: otherdayStr };
}

/**
 * ★ 快捷选择条：根据标签计算 [开始ymd数组, 结束ymd数组]
 * 支持：昨日/今日(今天)/本月/上月(逐月回溯)/上季度(上度)/本年(今年)
 */
pickerDateRange.prototype.calcQuickRange = function (label) {
	var n = new Date();
	var s, e;
	if (label.indexOf('昨日') > -1 || label.indexOf('昨天') > -1) {
		this._lmChained = false;
		s = new Date(n.getFullYear(), n.getMonth(), n.getDate() - 1); e = s;
	} else if (label.indexOf('今日') > -1 || label.indexOf('今天') > -1) {
		this._lmChained = false;
		s = new Date(n.getFullYear(), n.getMonth(), n.getDate()); e = s;
	} else if (label.indexOf('上月') > -1) {
		// ★ 第一次点击：基于现实时间跳到上个月；继续点击：基于面板当前结束日期逐月回溯
		var ref = n;
		if (this._lmChained) {
			var cur = this.str2date($('#' + this.endDateId).val() || '');
			if (cur && !isNaN(cur.getTime())) { ref = cur; }
		}
		this._lmChained = true;
		s = new Date(ref.getFullYear(), ref.getMonth() - 1, 1);
		e = new Date(ref.getFullYear(), ref.getMonth(), 0);
	} else if (label.indexOf('本月') > -1) {
		this._lmChained = false;
		s = new Date(n.getFullYear(), n.getMonth(), 1);
		e = new Date(n.getFullYear(), n.getMonth() + 1, 0);
	} else if (label.indexOf('最近7') > -1 || label.indexOf('7天') > -1) {
		// ★ 新增：与 footer 原 aRecent7Days 链接语义一致（getSpecialPeriod(6)：开始=今天-6，结束=今天）
		this._lmChained = false;
		var t7 = this.getSpecialPeriod(this.periodObj[this.mOpts.aRecent7Days]);
		s = this.str2date(t7.otherday); e = this.str2date(t7.today);
	} else if (label.indexOf('最近30') > -1 || label.indexOf('30天') > -1) {
		// ★ 新增：与 footer 原 aRecent30Days 链接语义一致（getSpecialPeriod(29)：开始=今天-29，结束=今天）
		this._lmChained = false;
		var t30 = this.getSpecialPeriod(this.periodObj[this.mOpts.aRecent30Days]);
		s = this.str2date(t30.otherday); e = this.str2date(t30.today);
	} else if (label.indexOf('季') > -1 || label.indexOf('度') > -1) {
		this._lmChained = false;
		var lastQm = Math.floor(n.getMonth() / 3) * 3 - 3;
		s = new Date(n.getFullYear(), lastQm, 1);
		e = new Date(n.getFullYear(), lastQm + 3, 0);
	} else if (label.indexOf('年') > -1) {
		this._lmChained = false;
		s = new Date(n.getFullYear(), 0, 1);
		e = new Date(n.getFullYear(), 11, 31);
	} else {
		s = n; e = n;
	}
	return [this.date2ymd(s), this.date2ymd(e)];
};

/**
 * ★ 新增：生成层级选择器单列的选项行 HTML
 * @param {String} c 列类型：y年 / m月 / d日 / h时 / mi分 / s秒
 * @param {Number} from 起始值
 * @param {Number} to 结束值
 * @param {Number} cur 当前选中值（高亮 .on）
 */
pickerDateRange.prototype._cscItems = function (c, from, to, cur) {
	var h = '';
	for (var i = from; i <= to; i++) {
		var label = '' + i;
		if ('m' == c) { label = i + '月'; }
		else if ('d' == c) { label = i + '日'; }
		else if ('h' == c || 'mi' == c || 's' == c) { label = ('0' + i).slice(-2); }
		h += '<div class="qs_csc_it' + (i == cur ? ' on' : '') + '" data-v="' + i + '">' + label + '</div>';
	}
	return h;
};

/**
 * ★ 新增：渲染并打开某一侧的层级选择器（级联列表面板）
 *   kind='date'：年(当前年-20..+1)/月(1-12)/日(按年月动态) 三列，不含时分秒
 *   kind='time'：时(0-23)/分(0-59)/秒(0-59) 三列
 *   初始字段值取自该侧状态载体；载体为空/非法时回退为今天与默认时间（仅显示，不写入）
 * @param {String} side 'start' | 'end'
 * @param {String} kind 'date' | 'time'
 */
pickerDateRange.prototype._renderCascader = function (side, kind) {
	var $csc = $('#' + this.cascaderId);
	if (0 === $csc.length) { return; }
	kind = kind || 'date';
	this._cscKind = kind;
	this._cscSide = side;
	var now = new Date();
	var yMin = now.getFullYear() - 20, yMax = now.getFullYear() + 1;
	var cols, $t;
	if ('time' == kind) {
		var timeIpt = 'start' == side ? this.startTimeId : this.endTimeId;
		var tv = this._validTime($('#' + timeIpt).val(), 'start' == side ? this.mOpts.defaultStartTime : this.mOpts.defaultEndTime).split(':');
		this._cscState = { h: tv[0] * 1, mi: tv[1] * 1, s: tv[2] * 1 };
		cols = [
			{ c: 'h', hd: '时', html: this._cscItems('h', 0, 23, this._cscState.h) },
			{ c: 'mi', hd: '分', html: this._cscItems('mi', 0, 59, this._cscState.mi) }
		];
		// ★ 短格式到分（两列）；长格式到秒（追加秒列）
		if ('long' == this.timeFormat) {
			cols.push({ c: 's', hd: '秒', html: this._cscItems('s', 0, 59, this._cscState.s) });
		}
		$t = $('#' + timeIpt);
	} else {
		var dateIpt = 'start' == side ? this.startDateId : this.endDateId;
		var dv = ($('#' + dateIpt).val() || '').trim();
		var m = dv.match(/^(\d{4})-(\d{1,2})-(\d{1,2})$/);
		var y, mo, d;
		if (m && m[1] * 1 >= yMin && m[1] * 1 <= yMax) {
			y = m[1] * 1; mo = m[2] * 1; d = m[3] * 1;
		} else {
			y = now.getFullYear(); mo = now.getMonth() + 1; d = now.getDate();
		}
		if (!(mo >= 1 && mo <= 12)) { mo = now.getMonth() + 1; }
		var days = new Date(y, mo, 0).getDate();
		if (!(d >= 1 && d <= days)) { d = days; }
		this._cscState = { y: y, m: mo, d: d };
		cols = [
			{ c: 'y', hd: '年', html: this._cscItems('y', yMin, yMax, y) },
			{ c: 'm', hd: '月', html: this._cscItems('m', 1, 12, mo) },
			{ c: 'd', hd: '日', html: this._cscItems('d', 1, days, d) }
		];
		$t = $('#' + dateIpt + '_v');
	}
	var h = '';
	$.each(cols, function (i, col) {
		h += '<div class="qs_csc_col" data-c="' + col.c + '"><div class="qs_csc_hd">' + col.hd + '</div><div class="qs_csc_list">' + col.html + '</div></div>';
	});
	// ★ 新增：底部确定按钮栏（日期/时间级联通用；不放在 .qs_csc_col 内避免误触发列高亮逻辑）
	h += '<div class="qs_csc_footer"><a href="javascript:void(0)" class="qs_csc_confirm">确定</a></div>';
	$csc.html(h);
	// 定位到该侧触发框/时间输入框正下方
	var off = $t.offset();
	$csc.css({ left: off.left + 'px', top: (off.top + $t.outerHeight() + 2) + 'px' }).show();
	// 各列滚动定位到当前选中项附近
	$csc.find('.qs_csc_list').each(function () {
		var $it = $(this).find('.on');
		if ($it.length) { $(this).scrollTop(Math.max(0, $it[0].offsetTop - $(this)[0].offsetTop - 60)); }
	});
};

/**
 * ★ 新增：点击触发框/时间输入框——本侧同类级联已打开则关闭，否则打开/切换
 * @param {String} side 'start' | 'end'
 * @param {String} kind 'date' | 'time'
 */
pickerDateRange.prototype.toggleCascader = function (side, kind) {
	var $csc = $('#' + this.cascaderId);
	if (0 === $csc.length) { return; }
	kind = kind || 'date';
	if ($csc.is(':visible') && this._cscSide == side && this._cscKind == kind) {
		$csc.hide();
		this._cscSide = '';
		return;
	}
	this._renderCascader(side, kind);
};

/**
 * ★ 新增：区间校验——开始/结束日期均非空且 开始>结束 返回 true；
 * 时间模式开启且两侧时间非空时，比较精确到时间（短格式 HH:mm 补 :00 后按字典序比较）
 */
pickerDateRange.prototype._isRangeInvalid = function () {
	var s = $('#' + this.startDateId).val(), e = $('#' + this.endDateId).val();
	if (!s || !e) { return false; }
	if (this.mOpts.enableTimeMode && this.timeMode) {
		var st = $('#' + this.startTimeId).val() || '', et = $('#' + this.endTimeId).val() || '';
		if (st && et) {
			if (5 == st.length) { st += ':00'; }
			if (5 == et.length) { et += ':00'; }
			return (s + ' ' + st) > (e + ' ' + et);
		}
	}
	return s > e;
};
/**
 * ★ 新增：开始>结束 时对换两侧的日期值（不弹窗，直接纠正顺序；时间值暂不对换）
 */
pickerDateRange.prototype._swapRange = function () {
	var $s = $('#' + this.startDateId), $e = $('#' + this.endDateId);
	var t = $s.val(); $s.val($e.val()); $e.val(t);
	// 时间值暂不对换（后面有需要再打开）
	// var $st = $('#' + this.startTimeId), $et = $('#' + this.endTimeId);
	// if ($st.length && $et.length) {
	// 	var tt = $st.val(); $st.val($et.val()); $et.val(tt);
	// }
};
// ★ 新增：区间顺序兜底——非法则对换，返回是否发生过对换
pickerDateRange.prototype._ensureRangeOrder = function () {
	if (this._isRangeInvalid()) {
		this._swapRange();
		return true;
	}
	return false;
};
/**
 * ★ 新增：从隐藏状态载体计算两个可见日期触发框的显示值（仅 ta 主题）
 *   只显示纯日期 'yyyy-MM-dd'；时分秒由独立的时间输入框自行显示，不再合并
 */
pickerDateRange.prototype.updateViewInputs = function () {
	if (this.mOpts.theme != 'ta') { return; }
	var __method = this;
	$.each([this.startDateId, this.endDateId], function (i, id) {
		var $v = $('#' + id + '_v');
		if (0 === $v.length) { return; }
		var d = ($('#' + id).val() || '').trim();
		if (!/^\d{4}-\d{1,2}-\d{1,2}$/.test(d)) { $v.val(''); return; }
		$v.val(__method.formatDate(d));
	});
};

/**
 * ★ 新增：按 timeFormat 更新时间行右侧的格式切换链接文字（'long'→长时间格式 / 'short'→短时间格式）
 */
pickerDateRange.prototype.updateBadge = function () {
	if (!this.mOpts.enableTimeMode) { return; }
	var $b = $('#' + this.formatBadgeId);
	if (0 === $b.length) { return; }
	$b.text('long' == this.timeFormat ? '长时间格式' : '短时间格式');
};

/**
 * ★ 新增：时间输入框与格式切换链接仅在时分秒模式（timeMode）显示
 */
pickerDateRange.prototype._syncTimeRow = function () {
	if (!this.mOpts.enableTimeMode) { return; }
	$('#' + this.timeRowId).find('.qs_time_only').toggle(this.timeMode);
};

/**
 * ★ 新增：按当前 timeFormat 重排两个时间输入框的值（空/非法回退默认；short 截断到分 / long 补 :00）
 */
pickerDateRange.prototype._applyTimeFormatToInputs = function () {
	var st = this._validTime($('#' + this.startTimeId).val(), this.mOpts.defaultStartTime);
	var et = this._validTime($('#' + this.endTimeId).val(), this.mOpts.defaultEndTime);
	if ('long' != this.timeFormat) { st = st.slice(0, 5); et = et.slice(0, 5); }
	$('#' + this.startTimeId).val(st);
	$('#' + this.endTimeId).val(et);
};

/**
 * ★ 校验时间串（H:mm / HH:mm:ss 均可），非法则返回默认值，输出统一补零为 HH:mm:ss
 */
pickerDateRange.prototype._validTime = function (t, def) {
	var m = /^(\d{1,2}):(\d{1,2})(?::(\d{1,2}))?$/.test((t || '').trim());
	if (!m) { return def; }
	var p = (t || '').trim().split(':');
	var h = +p[0], mi = +p[1], s = p[2] === undefined ? 0 : +p[2];
	if (h > 23 || mi > 59 || s > 59) { return def; }
	var z = function (x) { return ('0' + x).slice(-2); };
	return z(h) + ':' + z(mi) + ':' + z(s);
};

/**
 * ★ 时分秒模式下：把面板日期 + 时间输入框组合成 yyyy-MM-dd HH:mm（短格式）/ yyyy-MM-dd HH:mm:ss（长格式）写入隐藏域和主输入框
 *   补零模式（timeMode=false）下不做任何处理（保持原有只到天、由页面手动补0的行为）
 */
pickerDateRange.prototype.composeTimeValues = function () {
	if (!(this.mOpts.enableTimeMode && this.timeMode)) { return; }
	var sv = $('#' + this.startDateId).val();
	var ev = $('#' + this.endDateId).val();
	var st = this._validTime($('#' + this.startTimeId).val(), this.mOpts.defaultStartTime);
	var et = this._validTime($('#' + this.endTimeId).val(), this.mOpts.defaultEndTime);
	// ★ 短格式截断到分
	if ('long' != this.timeFormat) { st = st.slice(0, 5); et = et.slice(0, 5); }
	$('#' + this.startTimeId).val(st);
	$('#' + this.endTimeId).val(et);
	if (sv) { $('#' + this.mOpts.startDateId).val(sv + ' ' + st); }
	if (ev) { $('#' + this.mOpts.endDateId).val(ev + ' ' + et); }
	var _tv = (sv ? sv + ' ' + st : '') + ('' == ev ? '' : this.mOpts.defaultText + (ev ? ev + ' ' + et : ''));
	var _el = document.getElementById(this.inputId);
	(_el && _el.tagName == 'INPUT') ? $('#' + this.inputId).val(_tv) : $('#' + this.inputId).html(_tv);
};

pickerDateRange.prototype.getCurrentDate = function () {
	return {
		'startDate': $('#' + this.mOpts.startDateId).val(),
		'endDate': $('#' + this.mOpts.endDateId).val(),
		'needCompare': $('#' + this.mOpts.compareCheckboxId).val(),
		'startCompareDate': $('#' + this.mOpts.startCompareDateId).val(),
		'endCompareDate': $('#' + this.mOpts.endCompareDateId).val()
	};
};

/**
 * @description 移除选择日期面板的样式
 * @param {Boolean} isCompare 是否是对比日期面板
 * @param {String} specialClass 特殊的样式，这里默认是常规和对比日期两种样式的重合样式
 */
pickerDateRange.prototype.removeCSS = function (isCompare, specialClass) {
	// 初始化对比时间重合部分的样式类
	if ('undefined' == typeof (specialClass)) {
		specialClass = this.mOpts.theme + '_' + this.mOpts.coincideCss;
	}
	// 是否移除对比部分的样式:0 日期选择;1 对比日期选择
	if ('undefined' == typeof (isCompare)) {
		isCompare = 0;
	}

	// 整个日期列表的开始日期
	var s_date = this.calendar_startDate;
	var e_date = this.calendar_endDate;
	//如果是用户自定义选择的话，需要充值样式边界日期
	if (this.mOpts.magicSelect) {
		s_date = this.str2date($('#' + this.startDateId).val());
		e_date = this.str2date($('#' + this.endDateId).val());
	}
	var bDate = new Date(s_date.getFullYear(), s_date.getMonth(), s_date.getDate());
	var cla = '';
	// 从开始日期循环到结束日期
	for (var d = new Date(bDate); d.getTime() <= e_date.getTime(); d.setDate(d.getDate() + 1)) {
		if (0 == isCompare) {
			// 移除日期样式
			cla = this.mOpts.theme + '_' + this.mOpts.selectCss;
		} else {
			// 移除对比日期样式
			cla = this.mOpts.theme + '_' + this.mOpts.compareCss;
		}
		// 移除指定样式
		$('#' + this.calendarId + '_' + this.date2ymd(d).join('-')).removeClass(cla);
		$('#' + this.calendarId + '_' + this.date2ymd(d).join('-')).removeClass(this.mOpts.firstCss).removeClass(this.mOpts.lastCss).removeClass(this.mOpts.clickCss);
	}
};

/**
 * @description 为选中的日期加上样式：1=比较时间；0=时间范围
 * @param {Boolean} isCompare 是否是对比日期面板
 * @param {String} specialClass 特殊的样式，这里默认是常规和对比日期两种样式的重合样式
 */
pickerDateRange.prototype.addCSS = function (isCompare, specialClass) {

	// 初始化对比时间重合部分的样式类
	if ('undefined' == typeof (specialClass)) {
		specialClass = this.mOpts.theme + '_' + this.mOpts.coincideCss;
	}
	// 是否移除对比部分的样式:0 日期选择;1 对比日期选择
	if ('undefined' == typeof (isCompare)) {
		isCompare = 0;
	}
	// 获取4个日期
	var startDate = this.str2date($('#' + this.startDateId).val());
	var endDate = this.str2date($('#' + this.endDateId).val());
	var startCompareDate = this.str2date($('#' + this.startCompareDateId).val());
	var endCompareDate = this.str2date($('#' + this.endCompareDateId).val());

	// 循环开始日期
	var sDate = 0 == isCompare ? startDate : startCompareDate;
	// 循环结束日期
	var eDate = 0 == isCompare ? endDate : endCompareDate;
	var cla = '';
	for (var d = new Date(sDate); d.getTime() <= eDate.getTime(); d.setDate(d.getDate() + 1)) {
		if (0 == isCompare) {
			// 添加日期样式
			cla = this.mOpts.theme + '_' + this.mOpts.selectCss;
			$('#' + this.calendarId + '_' + this.date2ymd(d).join('-')).removeClass(this.mOpts.firstCss).removeClass(this.mOpts.lastCss).removeClass(this.mOpts.clickCss);
			$('#' + this.calendarId + '_' + this.date2ymd(d).join('-')).removeClass(cla);
		} else {
			// 添加对比日期样式
			cla = this.mOpts.theme + '_' + this.mOpts.compareCss;
		}

		$('#' + this.calendarId + '_' + this.date2ymd(d).join('-')).attr('class', cla);
	}
	if (this.mOpts.theme == 'ta') {
		//为开始结束添加特殊样式
		$('#' + this.calendarId + '_' + this.date2ymd(new Date(sDate)).join('-')).removeClass().addClass(this.mOpts.firstCss);
		$('#' + this.calendarId + '_' + this.date2ymd(new Date(eDate)).join('-')).removeClass().addClass(this.mOpts.lastCss);
		//如果开始结束时间相同
		sDate.getTime() == eDate.getTime() && $('#' + this.calendarId + '_' + this.date2ymd(new Date(eDate)).join('-')).removeClass().addClass(this.mOpts.clickCss);
	}
};

/**
 * @description 判断开始、结束日期是否处在允许的范围内
 * @param {String} startYmd 开始时间字符串
 * @param {String} endYmd 结束时间字符串
 */
pickerDateRange.prototype.checkDateRange = function (startYmd, endYmd) {
	var sDate = this.str2date(startYmd);
	var eDate = this.str2date(endYmd);
	var sTime = sDate.getTime();
	var eTime = eDate.getTime();
	var minEDate, maxEDate;

	if (eTime >= sTime) {
		// 判断是否超过最大日期外
		maxEDate = this.str2date(startYmd);
		maxEDate.setMonth(maxEDate.getMonth() + this.mOpts.monthRangeMax);
		maxEDate.setDate(maxEDate.getDate() + this.mOpts.dayRangeMax - 1);
		if (maxEDate.getTime() < eTime) {
			alert('结束日期不能大于：' + this.date2ymd(maxEDate).join('-'));
			return false;
		}
	} else {
		// 判断是否超过最大日期外
		//maxEDate = this.str2date(stPartYmd);
		maxEDate = this.str2date(endYmd);
		maxEDate.setMonth(maxEDate.getMonth() - this.mOpts.monthRangeMax);
		maxEDate.setDate(maxEDate.getDate() - this.mOpts.dayRangeMax + 1);
		if (maxEDate.getTime() > eTime) {
			alert('开始日期不能小于：' + this.date2ymd(maxEDate).join('-'));
			return false;
		}
	}
	return true;
}

/**
 *  @description 选择日期
 *  @param {String} ymd 时间字符串
 */
pickerDateRange.prototype.selectDate = function (ymd) {
	// 手动点击日期时，取消顶部快捷选择条的选中高亮
	if (this.mOpts.quickSelect) {
		$('#' + this.quickBarId).find('.qs_quick_btn').removeClass('active');
	}
	//点击日期点的时候添加对应输入框的样式，而不是之前的 聚焦到输入框时显示样式 by zacharycai
	this.changeInput(this.dateInput);
	// 格式化日期
	var ymdFormat = this.formatDate(ymd);

	// start <-> end 切换
	if (this.startDateId == this.dateInput) {
		// 移除样式
		this.removeCSS(0);
		this.removeCSS(1);
		// 为当前点加样式
		$('#' + this.calendarId + '_' + ymd).attr('class', (this.mOpts.theme == 'ta' ? this.mOpts.clickCss : this.mOpts.theme + '_' + this.mOpts.selectCss));
		// 获取开始时间的初始值
		this.startDefDate = $('#' + this.dateInput).val();
		// 更改对应输入框的值
		$('#' + this.dateInput).val(ymdFormat);
		// 切换输入框焦点,如果是实时数据那么选择一天的数据
		if (true == this.mOpts.singleCompare || true == this.mOpts.isSingleDay) {
			this.dateInput = this.startDateId;
			$('#' + this.endDateId).val(ymdFormat);
			(this.mOpts.shortOpr || this.mOpts.autoSubmit) && this.close(1);
			this.mOpts.success({
				'startDate': $('#' + this.mOpts.startDateId).val(),
				'endDate': $('#' + this.mOpts.endDateId).val(),
				'needCompare': $('#' + this.mOpts.compareCheckboxId).val(),
				'startCompareDate': $('#' + this.mOpts.startCompareDateId).val(),
				'endCompareDate': $('#' + this.mOpts.endCompareDateId).val()
			});

		} else {
			this.dateInput = this.endDateId;
		}

	} else if (this.endDateId == this.dateInput) {
		// 如果开始时间未选
		if ('' == $('#' + this.startDateId).val()) {
			this.dateInput = this.startDateId;
			this.selectDate(ymd);
			return false;
		}
		// 判断用户选择的时间范围
		if (false == this.checkDateRange($('#' + this.startDateId).val(), ymd)) {
			return false;
		}
		// 如果结束时间小于开始时间
		if (-1 == this.compareStrDate(ymd, $('#' + this.startDateId).val())) {
			// 更改对应输入框的值(结束时间)
			$('#' + this.dateInput).val($('#' + this.startDateId).val());
			// 更改对应输入框的值(开始时间)
			$('#' + this.startDateId).val(ymdFormat);
			ymdFormat = $('#' + this.dateInput).val();
		}
		// 更改对应输入框的值
		$('#' + this.dateInput).val(ymdFormat);
		// 切换输入框焦点
		this.dateInput = this.startDateId;
		this.removeCSS(0);
		this.addCSS(0);
		//this.addCSS(0, this.mOpts.coincideCss);
		this.startDefDate = '';
		if (this.mOpts.autoSubmit) {
			this.close(1);
			this.mOpts.success({
				'startDate': $('#' + this.mOpts.startDateId).val(),
				'endDate': $('#' + this.mOpts.endDateId).val(),
				'needCompare': $('#' + this.mOpts.compareCheckboxId).val(),
				'startCompareDate': $('#' + this.mOpts.startCompareDateId).val(),
				'endCompareDate': $('#' + this.mOpts.endCompareDateId).val()
			});
		}
	} else if (this.startCompareDateId == this.dateInput) {
		// 移除样式
		this.removeCSS(1);
		this.removeCSS(0);
		// 为当前点加样式
		$('#' + this.calendarId + '_' + ymd).attr('class', (this.mOpts.theme == 'ta' ? this.mOpts.clickCss : this.mOpts.theme + '_' + this.mOpts.compareCss));
		// 获取开始时间的初始值
		this.startDefDate = $('#' + this.dateInput).val();
		// 更改对应输入框的值
		$('#' + this.dateInput).val(ymdFormat);
		// 切换输入框焦点
		if (true == this.mOpts.singleCompare || true == this.mOpts.isSingleDay) {
			this.dateInput = this.startCompareDateId;
			$('#' + this.endCompareDateId).val(ymdFormat);
			(this.mOpts.shortOpr || this.mOpts.autoSubmit) && this.close(1);
			this.mOpts.success({
				'startDate': $('#' + this.mOpts.startDateId).val(),
				'endDate': $('#' + this.mOpts.endDateId).val(),
				'needCompare': $('#' + this.mOpts.compareCheckboxId).val(),
				'startCompareDate': $('#' + this.mOpts.startCompareDateId).val(),
				'endCompareDate': $('#' + this.mOpts.endCompareDateId).val()
			});
		}
		else {
			this.dateInput = this.endCompareDateId;
		}

	} else if (this.endCompareDateId == this.dateInput) {
		// 如果开始时间未选
		if ('' == $('#' + this.startCompareDateId).val()) {
			this.dateInput = this.startCompareDateId;
			this.selectDate(ymd);
			return false;
		}
		// 判断用户选择的时间范围
		if (false == this.checkDateRange($('#' + this.startCompareDateId).val(), ymd)) {
			return false;
		}
		// 如果结束时间小于开始时间
		if (-1 == this.compareStrDate(ymd, $('#' + this.startCompareDateId).val())) {
			// 更改对应输入框的值(结束时间)
			$('#' + this.dateInput).val($('#' + this.startCompareDateId).val());
			// 更改对应输入框的值(开始时间)
			$('#' + this.startCompareDateId).val(ymdFormat);
			ymdFormat = $('#' + this.dateInput).val();
		}
		// 更改对应输入框的值
		$('#' + this.dateInput).val(ymdFormat);
		// 切换输入框焦点
		this.dateInput = this.startCompareDateId;
		//this.addCSS(1, this.mOpts.coincideCss);
		this.removeCSS(1);
		this.addCSS(1);
		this.startDefDate = '';
		if (this.mOpts.autoSubmit) {
			this.close(1);
			this.mOpts.success({
				'startDate': $('#' + this.mOpts.startDateId).val(),
				'endDate': $('#' + this.mOpts.endDateId).val(),
				'needCompare': $('#' + this.mOpts.compareCheckboxId).val(),
				'startCompareDate': $('#' + this.mOpts.startCompareDateId).val(),
				'endCompareDate': $('#' + this.mOpts.endCompareDateId).val()
			});
		}
	}
	// ★ 新增：日历点选改变状态载体输入框值后，刷新 footer 触发框显示
	this.updateViewInputs();
	// 切换到下一个输入框
	//    this.changeInput(this.dateInput);
};

/**
 * @description显示日期选择框
 * @param {Boolean} isCompare 是否是对比日期选择框
 * @param {Object} __method 时期选择器超级对象
 */
pickerDateRange.prototype.show = function (isCompare, __method) {
	$('#' + __method.dateRangeDiv).css('display', isCompare ? 'none' : '');
	$('#' + __method.dateRangeCompareDiv).css('display', isCompare ? '' : 'none');
	var pos = isCompare ? $('#' + this.inputCompareId).offset() : $('#' + this.inputId).offset();
	var offsetHeight = isCompare ? $('#' + this.inputCompareId).height() : $('#' + this.inputId).height();
	var clientWidth = parseInt($(document.body)[0].clientWidth);
	var left = pos.left;
	$("#" + this.calendarId).css('display', 'block');
	if (true == this.mOpts.singleCompare || true == this.mOpts.isSingleDay) {
		$('#' + this.endDateId).css('display', 'none');
		$('#' + this.endCompareDateId).css('display', 'none');
		$('#' + this.mOpts.joinLineId).css('display', 'none');
		$('.' + this.mOpts.joinLineId).css('display', 'none');
	}
	// 如果和输入框左对齐时超出了宽度范围，则右对齐
	if (0 < clientWidth && $("#" + this.calendarId).width() + pos.left > clientWidth) {
		left = pos.left + $('#' + this.inputId).width() - $("#" + this.calendarId).width() + ((/msie/i.test(navigator.userAgent) && !(/opera/i.test(navigator.userAgent))) ? 5 : 0);
		__method.mOpts.theme == 'ta' && (left += 50);
	}
	$("#" + this.calendarId).css('left', left + 'px');
	//$("#" + this.calendarId).css('top', pos.top + (offsetHeight ? offsetHeight- 1 : (__method.mOpts.theme=='ta'?35:22)) + 'px');
	$("#" + this.calendarId).css('top', pos.top + (__method.mOpts.theme == 'ta' ? 35 : 22) + 'px');
	//第一次显示的时候，一定要初始化输入框
	isCompare ? this.changeInput(this.startCompareDateId) : this.changeInput(this.startDateId);
	// ★ 打开面板时：若隐藏域已带时分秒（上次在时分秒模式下提交），自动恢复时分秒模式并回填时间
	if (this.mOpts.enableTimeMode) {
		var _sv = ($('#' + this.mOpts.startDateId).val() || '').split(' ');
		var _ev = ($('#' + this.mOpts.endDateId).val() || '').split(' ');
		if (_sv[1] || _ev[1]) {
			this.timeMode = true;
			// ★ 按隐藏域时间部分长度识别格式：HH:mm=短（到分）/ HH:mm:ss=长（到秒）
			this.timeFormat = String(_sv[1] || _ev[1]).length > 5 ? 'long' : 'short';
			$('#' + this.timeModeBtnId).text('年月日').addClass('on');
			$('#' + this.startTimeId).val(this._validTime(_sv[1], this.mOpts.defaultStartTime));
			$('#' + this.endTimeId).val(this._validTime(_ev[1], this.mOpts.defaultEndTime));
			this._applyTimeFormatToInputs(); // ★ 按格式截断/补齐显示
		}
		// ★ 新增：回填后刷新格式切换链接文字与时间输入框显隐
		this.updateBadge();
		this._syncTimeRow();
	}
		// ★ 新增：打开面板时刷新 footer 触发框显示，并隐藏上一轮的级联面板
	this.updateViewInputs();
	$('#' + this.cascaderId).hide();
	this._cscSide = '';
	return false;
};

/**
 * @description 关闭日期选择框
 * @param {Boolean} btnSubmit 是否是点击确定按钮关闭的 
 */
pickerDateRange.prototype.close = function (btnSubmit) {
	var __method = this;
	//by zacharycai 关闭后就解绑了
	//$(document).unbind('click');

	// 把开始、结束时间显示到输入框 (PS:如果选择的今日，昨日，则只填入一个日期)
	// 如果开始和结束同个时间也照样分段by zacharycai
	//$('#' + this.inputId).val($('#' + this.startDateId).val() + ($('#' + this.startDateId).val() == $('#' + this.endDateId).val() ? '' : this.mOpts.defaultText + $('#' + this.endDateId).val()));
	if (btnSubmit) {
		//如果是单日快捷选择
		if (this.mOpts.shortOpr === true) {
			$('#' + this.inputId).val($('#' + this.startDateId).val());
			$('#' + this.inputCompareId).val($('#' + this.startCompareDateId).val());
		} else {
			$('#' + this.inputId).val($('#' + this.startDateId).val() + ('' == $('#' + this.endDateId).val() ? '' : this.mOpts.defaultText + $('#' + this.endDateId).val()));
		}
		//判断当前天是否可选，来决定从后往前推修改日期是从哪一点开始
		var nDateTime = ((true == this.mOpts.isTodayValid && '' != this.mOpts.isTodayValid)) ? new Date().getTime() : new Date().getTime() - (1 * 24 * 60 * 60 * 1000);
		var bDateTime = this.str2date($('#' + this.startDateId).val()).getTime();
		var eDateTime = this.str2date($('#' + this.endDateId).val()).getTime();
		//如果endDateTime小于bDateTime 相互交换
		if (eDateTime < bDateTime) {
			var tmp = $('#' + this.startDateId).val();
			$('#' + this.startDateId).val($('#' + this.endDateId).val());
			$('#' + this.endDateId).val(tmp);
		}
		var _val = this.mOpts.shortOpr == true ? $('#' + this.startDateId).val() : ($('#' + this.startDateId).val() + ('' == $('#' + this.endDateId).val() ? '' : this.mOpts.defaultText + $('#' + this.endDateId).val()));
		// 把开始、结束时间显示到输入框 (PS:如果选择的今日，昨日，则只填入一个日期)
		var input = document.getElementById(this.inputId);
		if (input && input.tagName == 'INPUT') {
			$('#' + this.inputId).val(_val);
			$('#' + this.inputCompareId).is(':visible') && $('#' + this.inputCompareId).val(_compareVal);
		} else {
			$('#' + this.inputId).html(_val);
			$('#' + this.inputCompareId).is(':visible') && $('#' + this.inputCompareId).html(_compareVal);
		}
		//	//在js侧就做好日期校准，以前面的日期选择的跨度为准，如果后面的跨度超过了当前可用时间，则以当前可用时间向前推 added by johnnyzheng 11-29
		if (this.mOpts.theme != 'ta') {
			if ('' != $('#' + this.startCompareDateId).val() && '' != $('#' + this.endCompareDateId).val()) {
				var bcDateTime = this.str2date($('#' + this.startCompareDateId).val()).getTime();
				var ecDateTime = this.str2date($('#' + this.endCompareDateId).val()).getTime();
				var _ecDateTime = bcDateTime + eDateTime - bDateTime;
				if (_ecDateTime > nDateTime) {
					//如果计算得到的时间超过了当前可用时间，那么就和服务器端保持一致，将当前可用的天数向前推日期选择器的跨度 added by johnnyzheng 11-29
					_ecDateTime = nDateTime;
					$('#' + this.startCompareDateId).val(this.formatDate(this.date2ymd(new Date(_ecDateTime + bDateTime - eDateTime)).join('-')));
				}
				$('#' + this.endCompareDateId).val(this.formatDate(this.date2ymd(new Date(_ecDateTime)).join('-')));

				//把开始结束对比时间大小重新矫正一下
				var bcDateTime = this.str2date($('#' + this.startCompareDateId).val()).getTime();
				var ecDateTime = this.str2date($('#' + this.endCompareDateId).val()).getTime();
				if (ecDateTime < bcDateTime) {
					var tmp = $('#' + this.startCompareDateId).val();
					$('#' + this.startCompareDateId).val($('#' + this.endCompareDateId).val());
					$('#' + this.endCompareDateId).val(tmp);
				}
			}
		}
		//把对比时间填入输入框 (PS:如果选择今日，昨日，则只填入一个日期)
		//$('#' + this.inputCompareId).val($('#' + this.startCompareDateId).val() + this.mOpts.defaultText + $('#' + this.endCompareDateId).val());
		var _compareVal = this.mOpts.shortOpr == true ? $('#' + this.startCompareDateId).val() : ($('#' + this.startCompareDateId).val() + ('' == $('#' + this.endCompareDateId).val() ? '' : this.mOpts.defaultText + $('#' + this.endCompareDateId).val()));
		if (input && input.tagName == 'INPUT') {
			$('#' + this.inputCompareId).val(_compareVal);
		} else {
			$('#' + this.inputCompareId).html(_compareVal);
		}
		// 计算相隔天数
		var step = (bDateTime - eDateTime) / 86400000;

		// 更改目标元素值
		$('#' + this.mOpts.startDateId).val($('#' + this.startDateId).val());
		$('#' + this.mOpts.endDateId).val($('#' + this.endDateId).val());
		$('#' + this.mOpts.startCompareDateId).val($('#' + this.startCompareDateId).val());
		$('#' + this.mOpts.endCompareDateId).val($('#' + this.endCompareDateId).val());
		// ★ 时分秒模式：确定提交时把时间组合进隐藏域与主输入框（补零模式下内部直接 return，行为不变）
		this.composeTimeValues();
		//点击确定按钮进行查询后将取消所有的今天 昨天 最近7天的快捷链接 added by johnnyzheng 11-29
		for (var property in this.periodObj) {
			if ($('#' + this.mOpts[property])) {
				$('#' + this.mOpts[property]).parent().removeClass('a');
			}
		}
	}
	// 隐藏日期选择框 延迟200ms 关闭日期选择框
	$("#" + __method.calendarId).css('display', 'none');
	// ★ 新增：主面板关闭时级联面板一并隐藏，并刷新触发框显示
	$('#' + this.cascaderId).hide();
	this._cscSide = '';
	this.updateViewInputs();
	return false;
};

/**
 * @description 清空日期选择框
 * @param {Boolean} btnSubmit 是否是点击确定按钮关闭的 
 */
pickerDateRange.prototype.clear = function (btnSubmit) {

	$('#' + this.inputId).val("");

	$('#' + this.startDateId).val("");
	$('#' + this.endDateId).val("");
	// ★ 新增：清空后刷新 footer 触发框显示（空值显示占位文字）
	this.updateViewInputs();

};
/**
 * @description 日期填充函数
 * @param {Num} year 年
 * @param {Num} month 月
 */
pickerDateRange.prototype.fillDate = function (year, month, index) {
	var __method = this;
	var isTaTheme = this.mOpts.theme == 'ta';
	// 当月第一天
	var firstDayOfMonth = new Date(year, month, 1);
	var dateBegin = new Date(year, month, 1);
	var w = dateBegin.getDay();
	// 计算应该开始的日期
	dateBegin.setDate(1 - w);

	// 当月最后一天
	var lastDayOfMonth = new Date(year, month + 1, 0);
	var dateEnd = new Date(year, month + 1, 0);
	w = dateEnd.getDay();
	// 计算应该结束的日期
	dateEnd.setDate(dateEnd.getDate() + 6 - w);

	var today = new Date();
	var dToday = today.getDate();
	var mToday = today.getMonth();
	var yToday = today.getFullYear();

	var table = document.createElement('table');
	if (isTaTheme) {
		table.className = this.mOpts.dateTable;

		cap = document.createElement('caption');

		//如果是magicSelect，用户自定义的选择年和月份
		if (this.mOpts.magicSelect) {
			var yh = ['<div class="select_wrap" id="yselected_' + index + '"><div class="select" id="_ybtn_' + index + '">' + year + '</div><div class="dropdown" id="_yctn_' + index + '"><ul class="list_menu">']
			var mh = ['<div class="select_wrap" id="mselected_' + index + '"><div class="select" id="_mbtn_' + index + '">' + (month + 1) + '</div><div class="dropdown" id="_mctn_' + index + '"><ul class="list_menu">']

			//var yh = ['<select name="yselected_'+index+'" class="xxxs">'];
			//var mh = ['<select name="mselected_'+index+'" class="xxxs">'];
			i = 1;
			yt = yToday;
			do {
				//yh.push('<option value="'+yt+'" '+(yt == year? 'selected' : '')+'>'+(yt--)+'</option>');
				//mh.push('<option value="'+i+'" '+(i == (month+1)? 'selected' : '')+'>'+(i++)+'</option>');
				yh.push('<li><a href="javascript:;">' + (yt--) + '</a></li>');
				mh.push('<li><a href="javascript:;">' + (i++) + '</a></li>');
			} while (i <= 12);
			//yh.push('</select>');
			//mh.push('</select>');		
			yh.push('</ul></div></div>');
			mh.push('</ul></div></div>');
			$(cap).append(yh.join('') + '<span class="joinLine"> 年 </span>' + mh.join('') + '<span class="joinLine"> 月 </span>');

		}
		else {
			$(cap).append(year + '年' + (month + 1) + '月');
		}

		$(table).append(cap);
		thead = document.createElement('thead');
		tr = document.createElement('tr');
		var days = ['日', '一', '二', '三', '四', '五', '六'];
		for (var i = 0; i < 7; i++) {
			th = document.createElement('th');
			$(th).append(days[i]);
			$(tr).append(th);
		}
		$(thead).append(tr);
		$(table).append(thead);

		tr = document.createElement('tr');
		td = document.createElement('td');
		// 如果是最后一个月的日期，则加上下一个月的链接
		if (!this.mOpts.magicSelect) {
			if (0 == index) {
				$(td).append('<a href="javascript:void(0);" id="' + this.nextMonth + '"><i class="i_next"></i></a>');
			}
			// 如果是第一个月的日期，则加上上一个月的链接
			if (index + 1 == this.mOpts.calendars) {
				$(td).append('<a href="javascript:void(0);" id="' + this.preMonth + '"><i class="i_pre"></i></a>');
			}
		}

		//    $(td).append('<span style="font-size:16px">' + year + '年' + (month + 1) + '月' + '</span>');
		$(td).attr('colSpan', 7);
		$(td).css('text-align', 'center');
		$(tr).append(td);
		$(table).append(tr);
	}
	else {
		table.className = this.mOpts.theme + '_' + this.mOpts.dateTable;

		tr = document.createElement('tr');
		td = document.createElement('td');
		// 如果是最后一个月的日期，则加上下一个月的链接
		if (0 == index) {
			$(td).append('<a href="javascript:void(0);" id="' + this.nextMonth + '" class="gri_dateRangeNextMonth"><span>next</span></a>');
		}
		// 如果是第一个月的日期，则加上上一个月的链接
		if (index + 1 == this.mOpts.calendars) {
			$(td).append('<a href="javascript:void(0);" id="' + this.preMonth + '" class="gri_dateRangePreMonth"><span>pre</span></a>');
		}
		$(td).append(year + '年' + (month + 1) + '月');
		$(td).attr('colSpan', 7);
		$(td).css('text-align', 'center');
		$(td).css('background-color', '#F9F9F9');
		$(tr).append(td);
		$(table).append(tr);

		var days = ['日', '一', '二', '三', '四', '五', '六'];
		tr = document.createElement('tr');
		for (var i = 0; i < 7; i++) {
			td = document.createElement('td');
			$(td).append(days[i]);
			$(tr).append(td);
		}
		$(table).append(tr);
	}
	// 当前月的所有日期(包括空白位置填充的日期)
	var tdClass = '', deviation = 0, ymd = '';
	for (var d = dateBegin; d.getTime() <= dateEnd.getTime(); d.setDate(d.getDate() + 1)) {
		if (d.getTime() < firstDayOfMonth.getTime()) { // 当前月之前的日期
			tdClass = this.mOpts.theme + '_' + this.mOpts.disableGray;
			deviation = '-1';
		} else if (d.getTime() > lastDayOfMonth.getTime()) { // 当前月之后的日期
			tdClass = this.mOpts.theme + '_' + this.mOpts.disableGray;
			deviation = '1';
		} else if ((this.mOpts.stopToday == true && d.getTime() > today.getTime()) || d.getTime() < __method.mOpts.minValidDate * 1000 || ('' !== __method.mOpts.maxValidDate && d.getTime() > __method.mOpts.maxValidDate * 1000)) { // 当前时间之后的日期，或者开启统计之前的日期
			tdClass = this.mOpts.theme + '_' + this.mOpts.disableGray;
			deviation = '2';
		} else { // 当前月日期
			deviation = '0';
			if (d.getDate() == dToday && d.getMonth() == mToday && d.getFullYear() == yToday) {
				if (true == this.mOpts.isTodayValid) {
					tdClass = this.mOpts.theme + '_' + this.mOpts.isToday;
				} else {
					tdClass = this.mOpts.theme + '_' + this.mOpts.disableGray;
					deviation = '2';
				}
			}
			else {
				tdClass = '';
			}
			//让周末不可选不可选
			if (this.mOpts.weekendDis && (d.getDay() == 6 || d.getDay() == 0)) {
				tdClass = this.mOpts.theme + '_' + this.mOpts.disableGray;
				deviation = '3';
			}
			//让周几不可选
			if (this.mOpts.disCertainDay && this.mOpts.disCertainDay.length > 0) {
				for (var p in this.mOpts.disCertainDay) {
					if (!isNaN(this.mOpts.disCertainDay[p]) && d.getDay() === this.mOpts.disCertainDay[p]) {
						tdClass = this.mOpts.theme + '_' + this.mOpts.disableGray;
						deviation = '4';
					}
				}
			}
			//让几号不可选
			if (this.mOpts.disCertainDate && this.mOpts.disCertainDate.length > 0) {
				var isDisabled = false;

				for (var p in this.mOpts.disCertainDate) {
					if (!isNaN(this.mOpts.disCertainDate[p]) || isNaN(parseInt(this.mOpts.disCertainDate[p]))) {
						if (this.mOpts.disCertainDate[0] === true) {
							isDisabled = !!(d.getDate() !== this.mOpts.disCertainDate[p]);
							if (!isDisabled) {
								break;
							}
						} else {
							isDisabled = !!(d.getDate() === this.mOpts.disCertainDate[p]);
							if (isDisabled) {
								break;
							}
						}

					}
				}

				if (isDisabled) {
					tdClass = this.mOpts.theme + '_' + this.mOpts.disableGray;
					deviation = '4';
				}

			}
		}

		// 如果是周日
		if (0 == d.getDay()) {
			tr = document.createElement('tr');
		}

		td = document.createElement('td');
		td.innerHTML = d.getDate();
		if ('' != tdClass) {
			$(td).attr('class', tdClass);
		}

		// 只有当前月可以点击
		if (0 == deviation) {
			ymd = d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate();
			$(td).attr('id', __method.calendarId + '_' + ymd);
			$(td).css('cursor', 'pointer');
			(function (ymd) {
				$(td).bind("click", ymd, function () {
					__method.selectDate(ymd);
					return false;
				});
			})(ymd);
		}

		$(tr).append(td);

		// 如果是周六
		if (6 == d.getDay()) {
			$(table).append(tr);
		}
	}

	return table;
};

/**
 * @description 把时间字串转成时间格式
 * @param {String} str 时间字符串
 */
pickerDateRange.prototype.str2date = function (str) {
	var ar = str.split('-');
	// 返回日期格式
	return new Date(ar[0], ar[1] - 1, ar[2]);
};

/**
 * @description 比较两个时间字串的大小:1 大于; 0 等于; -1 小于
 * @param {String} b 待比较时间串1
 * @param {String} e 待比较时间串2
 */
pickerDateRange.prototype.compareStrDate = function (b, e) {
	var bDate = this.str2date(b);
	var eDate = this.str2date(e);

	// 1 大于; 0 等于; -1 小于
	if (bDate.getTime() > eDate.getTime()) {
		return 1;
	} else if (bDate.getTime() == eDate.getTime()) {
		return 0;
	} else {
		return -1;
	}
};

/**
 * @description 把时间格式转成对象
 * @param {Date} d 时间
 */
pickerDateRange.prototype.date2ymd = function (d) {
	return [d.getFullYear(), (d.getMonth() + 1), d.getDate()];
};

/**
 * @description 切换焦点到当前输入框
 * @param {String} 日期框体ID
 */
pickerDateRange.prototype.changeInput = function (ipt) {
	// 强制修改为开始输入框
	if (true == this.mOpts.isSingleDay) {
		ipt = this.startDateId;
	}
	// 所有4个输入框
	var allInputs = [this.startDateId, this.startCompareDateId, this.endDateId, this.endCompareDateId];

	// 如果 ipt 是日期输入框，则为日期样式，否则为对比日期样式
	var cla = '';
	if (ipt == this.startDateId || ipt == this.endDateId) {
		cla = this.mOpts.theme + '_' + this.mOpts.selectCss;
	} else {
		cla = this.mOpts.theme + '_' + this.mOpts.compareCss;
	}
	if (ipt == this.endDateId && this.mOpts.singleCompare) {
		cla = this.mOpts.theme + '_' + this.mOpts.compareCss;
	}

	// 移除所有输入框的附加样式
	// for(var i in allInputs) {
	// 	$('#' + allInputs[i]).removeClass(this.mOpts.theme + '_' + this.mOpts.selectCss);
	// 	$('#' + allInputs[i]).removeClass(this.mOpts.theme + '_' + this.mOpts.compareCss);
	// }
	for (var i of allInputs) {
		$('#' + i).removeClass(this.mOpts.theme + '_' + this.mOpts.selectCss);
		$('#' + i).removeClass(this.mOpts.theme + '_' + this.mOpts.compareCss);
	}

	// 为指定输入框添加样式
	$('#' + ipt).addClass(cla);
	//背景图repeat
	$('#' + ipt).css('background-repeat', 'repeat');
	// 把输入焦点移到指定输入框
	this.dateInput = ipt;
};

/**
 * @description 日期格式化，加前导零
 */
pickerDateRange.prototype.formatDate = function (ymd) {
	return ymd.replace(/(\d{4})\-(\d{1,2})\-(\d{1,2})/g, function (ymdFormatDate, y, m, d) {
		if (m.length < 2) { // ★ 已是两位（如'01'）时不再补零，避免出现'001'
			m = '0' + m;
		}
		if (d.length < 2) {
			d = '0' + d;
		}
		return y + '-' + m + '-' + d;
	});
};
