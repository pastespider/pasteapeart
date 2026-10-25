// expend.js
// 作者 lll
// window._global_list_properties
// window.global_page_data
// window._global_list_properties
// window.global_page_data
(function () {

    // 保存原始函数引用
    const oldPageLoad = window.afterReadPageData;

    // console.log(oldPageLoad, 'oldPageLoad');

    // 闭包内缓存当前页面数据，避免跨页面数据污染
    var pageDataCache = null;

    window.afterReadPageData = function (code, obj, page) {
        if (obj && Array.isArray(obj)) {
            pageDataCache = obj;
        } else if (obj && typeof obj === 'object' && obj.items && Array.isArray(obj.items)) {
            pageDataCache = obj.items;
        }

        let result;
        if (oldPageLoad) {
            result = oldPageLoad.call(this, code, obj, page);
        }
        return result;
    };

    // HTML转纯文本
    function htmlToText(html) {
        if (html === null || html === undefined) return '';
        if (typeof html !== 'string') return String(html);
        // 处理<br>和<br/>换行
        let text = html.replace(/<br\s*\/?>/gi, '\n');
        // 处理</span>、</div>等块级元素闭合标签后加换行
        text = text.replace(/<\/(span|div|p|li|tr)>/gi, function (match, tag) {
            return tag === 'span' ? ' ' : '\n';
        });
        // 去除所有HTML标签
        text = text.replace(/<[^>]+>/g, '');
        // 解码HTML实体
        text = text.replace(/&amp;/g, '&')
                   .replace(/&lt;/g, '<')
                   .replace(/&gt;/g, '>')
                   .replace(/&quot;/g, '"')
                   .replace(/&#39;/g, "'")
                   .replace(/&nbsp;/g, ' ');
        // 清理多余空白
        text = text.replace(/[ \t]+/g, ' ').trim();
        return text;
    }

    // 获取单元格的值，处理对象和HTML
    function getCellValue(value) {
        if (value === null || value === undefined) return '';
        // 对象类型取name属性（如 extendSubject: {id: 11, name: "财聚天下"}）
        if (typeof value === 'object') {
          if (value.name !== undefined) return value.name;
          if (value.id !== undefined) return value.id;
          return JSON.stringify(value);
        }
        // 字符串类型检查是否包含HTML标签
        if (typeof value === 'string' && /<[a-zA-Z][^>]*>/.test(value)) {
          return htmlToText(value);
        }
        return String(value);
    }

    // 导出Excel
    function exportExcel() {
        var properties = window._global_list_properties;
        var cache = pageDataCache;
        // console.log(properties, 'properties');
        
        if (!properties || !Array.isArray(properties) || properties.length === 0) {
          console.warn('导出Excel: 未找到表头配置 _global_list_properties');
          return;
        }
        if (!cache || !Array.isArray(cache) || cache.length === 0) {
          console.warn('导出Excel: 未找到数据 _pageDataCache');
          return;
        }

        // 获取数据列表
        var items = cache;
        // 如果缓存的是分页对象，取items
        if (cache.items && Array.isArray(cache.items)) {
          items = cache.items;
        }

        // 过滤隐藏列，构建表头
        // ★ 过滤 attributes 含 outignore / ifmenu 的列（操作按钮、忽略导出类字段）
        var visibleProps = properties.filter(function (p) {
          if(p.name == p.title) return false;
          if (!p.attributes || !Array.isArray(p.attributes) || !p.attributes.length) return true;
          return !p.attributes.some(function (a) {
            return a.name === 'outignore' || a.name === 'ifmenu' || a.name === 'menuhtml' ;
          });
        });

        var headers = visibleProps.map(function (p) {
          return p.title || p.name || '';
        });

        var keys = visibleProps.map(function (p) {
          return p.name;
        });

        // console.log('★ headers:', headers, '| keys:', keys);

        // 构建HTML表格
        var html = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40">';
        html += '<head><meta charset="UTF-8">';
        html += '<!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>Sheet1</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]-->';
        html += '<style>td{mso-number-format:"\\@";}</style>';
        html += '</head><body>';
        html += '<table border="1">';

        // 表头行（用<td>+内联样式，Excel兼容性更好）
        html += '<tr>';
        headers.forEach(function (h) {
          html += '<td style="background:#4472C4;color:#fff;font-weight:bold;text-align:center;">' + h + '</td>';
        });
        html += '</tr>';

        // console.log('★ 表头HTML:', html); // 验证表头是否写入

        // 数据行
        items.forEach(function (row) {
          html += '<tr>';
          keys.forEach(function (key) {
            var val = '';
            // 处理嵌套字段（如 extendBusinessModel?.name）
            if (key.indexOf('?.') !== -1) {
              var parts = key.split('?.');
              var cur = row;
              for (var pi = 0; pi < parts.length; pi++) {
                if (cur == null || typeof cur !== 'object') { cur = undefined; break; }
                cur = cur[parts[pi]];
              }
              val = getCellValue(cur);
            } else {
              val = getCellValue(row[key]);
            }
            html += '<td>' + val + '</td>';
          });
          html += '</tr>';
        });

        html += '</table></body></html>';

        // 生成Blob并下载
        var blob = new Blob([html], { type: 'application/vnd.ms-excel;charset=utf-8' });
        var url = URL.createObjectURL(blob);
        var link = document.createElement('a');
        link.href = url;

        // 生成文件名：列表数据_日期
        var now = new Date();
        var dateStr = now.getFullYear() + '-' +
          String(now.getMonth() + 1).padStart(2, '0') + '-' +
          String(now.getDate()).padStart(2, '0') + ' ' +
          String(now.getHours()).padStart(2, '0') + ':' +
          String(now.getMinutes()).padStart(2, '0');
        link.download = '列表数据_' + dateStr + '.xls';

        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
    }

    // 暴露到全局，外部可直接调用
    window.exportExcel = exportExcel;

})();