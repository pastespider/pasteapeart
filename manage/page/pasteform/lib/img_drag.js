
/**
 * 作者 apeart
 * 主要针对的是多图模式的拖拽排序的支持
 * 增强版支持动态操作的jQuery拖拽排序组件 应对多图得拖拽移动顺序
 */
(function ($) {
    $.fn.dragSort = function (options) {
        // 默认配置
        const defaults = {
            itemSelector: '.draggable-item',
            placeholderClass: 'drag-placeholder',
            deleteBtnSelector: '.delete-btn',
            onDragStart: null,
            onDragEnd: null,
            onItemAdded: null,
            onItemRemoved: null,
            dataAttribute: 'src'
        };

        const settings = $.extend({}, defaults, options);

        return this.each(function () {
            const $container = $(this);
            let $draggedItem = null;
            let $placeholder = null;

            // 初始化
            init();

            function init() {
                // 初始化现有元素
                initItems();

                // 绑定容器事件
                $container.on('dragover', handleDragOver);
                $container.on('dragenter', handleDragEnter);
                $container.on('dragleave', handleDragLeave);
                $container.on('drop', handleDrop);

                // 绑定删除按钮事件（使用事件委托支持动态元素）
                $container.on('click', settings.deleteBtnSelector, function (e) {
                    e.stopPropagation();
                    const $item = $(this).closest(settings.itemSelector);
                    removeItem($item);
                });
            }

            // 初始化可拖拽元素
            function initItems() {
                $container.find(settings.itemSelector).each(function () {
                    const $item = $(this);
                    $item.attr('draggable', 'true');

                    // 绑定拖拽事件
                    $item.off('dragstart').on('dragstart', handleDragStart);
                    $item.off('dragend').on('dragend', handleDragEnd);
                });
            }

            // 添加新元素
            function addItem(itemData) {
                // 创建新元素
                const $newItem = $(`
                    <div class="draggable-item" data-src="${itemData.src}" data-id="${itemData.id}">
                        <button class="delete-btn">×</button>
                        <img src="${itemData.src}" alt="${itemData.alt}">
                        <div class="item-info">
                            <h3>${itemData.title}</h3>
                        </div>
                    </div>
                `);

                // 添加到容器
                $container.append($newItem);

                // 初始化新元素的拖拽功能
                initItems();

                // 触发回调
                if ($.isFunction(settings.onItemAdded)) {
                    settings.onItemAdded($newItem, getCurrentOrder());
                }

                return $newItem;
            }

            // 移除元素
            function removeItem($item) {
                if (!$item || !$item.length) return;

                const itemData = {
                    id: $item.data('id'),
                    src: $item.data('src')
                };

                // 如果删除的是正在拖拽的元素，清理状态
                if ($draggedItem && $draggedItem.is($item)) {
                    if ($placeholder && $placeholder.length) {
                        $placeholder.remove();
                        $placeholder = null;
                    }
                    $draggedItem = null;
                    $container.removeClass('drag-over');
                }

                // 移除元素
                $item.remove();

                // 触发回调
                if ($.isFunction(settings.onItemRemoved)) {
                    settings.onItemRemoved(itemData, getCurrentOrder());
                }
            }

            // 清空所有元素
            function clearAll() {
                $container.find(settings.itemSelector).remove();

                // 清理拖拽状态
                if ($placeholder && $placeholder.length) {
                    $placeholder.remove();
                    $placeholder = null;
                }
                $draggedItem = null;
                $container.removeClass('drag-over');
            }

            // 开始拖拽
            function handleDragStart(e) {
                $draggedItem = $(this);

                // 添加拖拽样式
                setTimeout(() => {
                    $draggedItem.addClass('dragging');
                }, 0);

                // 存储数据
                e.originalEvent.dataTransfer.setData('text', $draggedItem.data(settings.dataAttribute));

                // 回调
                if ($.isFunction(settings.onDragStart)) {
                    settings.onDragStart($draggedItem);
                }
            }

            // 结束拖拽
            function handleDragEnd() {
                $draggedItem.removeClass('dragging');

                // 移除占位符
                if ($placeholder && $placeholder.length) {
                    $placeholder.remove();
                    $placeholder = null;
                }

                $container.removeClass('drag-over');

                // 回调
                if ($.isFunction(settings.onDragEnd)) {
                    settings.onDragEnd($draggedItem, getCurrentOrder());
                }

                $draggedItem = null;
            }

            // 拖拽经过
            function handleDragOver(e) {
                e.preventDefault();

                // 如果没有拖拽元素，不处理
                if (!$draggedItem || !$draggedItem.length) return;

                // 创建占位符
                if (!$placeholder || !$placeholder.length) {
                    $placeholder = $('<div>').addClass(settings.placeholderClass);
                }

                // 计算插入位置
                const $afterElement = getDragAfterElement(e.originalEvent.clientX);

                // 插入占位符
                if ($afterElement === null) {
                    $container.append($placeholder);
                    $placeholder.removeClass('right');
                } else {
                    const $element = $($afterElement);
                    if ($element.length == 0) { return; }
                    const rect = $element[0].getBoundingClientRect();
                    const midX = rect.left + rect.width / 2;

                    if (e.originalEvent.clientX < midX) {
                        $element.before($placeholder);
                        $placeholder.removeClass('right');
                    } else {
                        $element.after($placeholder);
                        $placeholder.addClass('right');
                    }

                }
            }

            // 进入容器
            function handleDragEnter(e) {
                e.preventDefault();
                $container.addClass('drag-over');
            }

            // 离开容器
            function handleDragLeave(e) {
                if (!$.contains($container[0], e.relatedTarget)) {
                    $container.removeClass('drag-over');
                }
            }

            // 放置元素
            function handleDrop(e) {
                e.preventDefault();
                $container.removeClass('drag-over');

                if ($placeholder && $placeholder.length && $draggedItem && $draggedItem.length) {
                    $placeholder.before($draggedItem);
                }
            }

            // 计算拖拽后位置
            function getDragAfterElement(x) {
                // const draggableElements = [...$container.find(settings.itemSelector).not('.dragging')];
                // console.log($container.find(settings.itemSelector));
                // // 兼容版本（不依赖 .not() 方法）
                // const draggableElements = [...$container.find(settings.itemSelector).filter(function() {
                //     // 筛选出不包含 'dragging' 类的元素
                //     return !$(this).hasClass('dragging');
                // })];
                // 获取所有可拖拽元素
                var allItems = $container.find(settings.itemSelector);
                var draggableElements = [];

                // 循环遍历所有元素，手动筛选出不包含dragging类的元素
                for (var i = 0; i < allItems.length; i++) {
                    var item = allItems[i];
                    // 检查元素是否不包含'dragging'类
                    if (!$(item).hasClass('dragging')) {
                        draggableElements.push(item);
                    }
                }

                return draggableElements.reduce((closest, child) => {
                    const box = child.getBoundingClientRect();
                    const offset = x - (box.left + box.width / 2);

                    if (offset < 0 && offset > closest.offset) {
                        return { offset: offset, element: child };
                    } else {
                        return closest;
                    }
                }, { offset: Number.NEGATIVE_INFINITY }).element;
            }

            // 获取当前排序
            function getCurrentOrder() {
                return $container.find(settings.itemSelector).map(function () {
                    return {
                        id: $(this).data('id'),
                        src: $(this).data(settings.dataAttribute)
                    };
                }).get();
            }

            // 暴露公共方法
            $container.data('dragSort', {
                addItem: addItem,
                removeItem: removeItem,
                clearAll: clearAll,
                initItems: initItems,
                getCurrentOrder: getCurrentOrder
            });
        });
    };
})(jQuery);

