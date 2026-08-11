(function ($) {

    $.fn.pagination = function (options) {

        var me = this;
        options = $.extend({
            total: 0,
            current: 1,
            pageSize: 20,
            sizeArray: [20, 50,100,300,500,1000],
            ellipsis: '…',
            callback: null
        }, options);

        var pageCount, pages, current = options.current, total = options.total;
        let pageSizeArray = options.sizeArray;
        let pageSize = options.pageSize;

        window.pageConfig = pages;

        // 初始化时绑定事件委托（只执行一次）
        me.on('change', '.pagination-select', function () {
            // goToPage(parseInt($(this).val()));
            options.pageSize = parseInt($(this).val());
            pageSize = options.pageSize;
            current = 1;
            // render();
            if (options.callback) {
                options.callback(1, me,pageSize);
            }
        });

        function render() {
            calcPages();
            var html = '<div class="pagination">'

            //这里写入一个select 下拉切换，然后执行第一页
            // 生成下拉框
            html += '<span>每页<select class="pagination-select">';
            for (let _size of pageSizeArray) {
                html += `<option value="${_size}" ${pageSize === _size ? 'selected' : ''}>${_size}</option>`;
            }
            html += '</select>条</span>';


            if (pages.length) {
                html += '<ul class="pagination-list"><li data-page="' + (current - 1) + '" class="' + (current == 1 ? "disabled" : "") + '" >上一页</li>';
                for (var i = 0, j = pages.length; i < j; i++) {
                    if (pages[i] == options.ellipsis) {
                        html += '<li class="ellipsis">' + options.ellipsis + '</li>'
                    } else {
                        html += '<li data-page="' + pages[i] + '" class="' + (current == pages[i] ? "active" : "") + '">' + pages[i] + '</li>'
                    }
                }
                html += '<li data-page="' + (current + 1) + '" class="' + (current == pageCount ? "disabled" : "") + '">下一页</li></ul>';
            }

            html += '<div class="pagination-total">总计' + total + '条</div></div>';
            me.html(html);
        }

        function calcPages() {
            pageCount = Math.ceil(total / options.pageSize);
            pages = [];
            
            if (pageCount < 1) {
                return;
            }
            if (current > pageCount) {
                current = pageCount;
            }
            if (pageCount > 11) {
                var len = pageCount - current;
                if (current > 6 && len > 5) {
                    pages = [1, 2, options.ellipsis, current - 2, current - 1, current, current + 1, current + 2, options.ellipsis, pageCount - 1, pageCount];
                } else if (current > 6 && len <= 5) {
                    pages = [1, 2, options.ellipsis];
                    for (var i = 7; i >= 0; i--) {
                        pages.push(pageCount - i);
                    }
                } else if (current <= 6 && len > 5) {
                    for (var i = 1; i <= 8; i++) {
                        pages.push(i);
                    }
                    pages = pages.concat([options.ellipsis, pageCount - 1, pageCount]);
                }
            } else {
                for (var i = 1; i <= pageCount; i++) {
                    pages.push(i);
                }
            }
        }

        me.setTotal = function (t) {
            total = t;
            current = 1;
            render();
        };

        me.on('click', '[data-page]:not(.disabled)', function () {
            var page = $(this).data('page');
            if (!isNaN(page)) {
                current = page;
                render();
                if (options.callback) {
                    options.callback(page, me,options.pageSize);
                }
            }
        });
        if (total > 0) {
            render();
        }else{
            console.log('total',total);
        }
        return this;
    };

})(jQuery);

/**
 * 使用案例
 * $('.wrap').pagination({
    callback: function (no, pagination) {
        // no是点击的页码
        // pagination是当前组件
        $.ajax({
            url: '',
            data: {
                pageIndex: no,
            },
            success: function (res) {
                // 总数一般是动态的，调用setTotal更新
                pagination.setTotal(res.data.total);
            }
        })
    }
})
 * 
 */