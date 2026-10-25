/**
 * 作者 apeart
 * 主要针对的是多图模式的拖拽排序的支持
 * 增强版支持动态操作的jQuery拖拽排序组件 应对多图得拖拽移动顺序
 */
class ColumnDragSorter {
    // 使用示例
    // var COLUMN_ORDER_KEY="table_order"
    constructor(tableId) {
        if (tableId) {
            this.tableId = tableId;
            this.table = document.getElementById(tableId);
            this.draggedColumn = null;
            this.draggedColumnIndex = -1;
            this.isInitialized = false;//是否初始化
            // this.orginginColumnHeaders =null;//原始头部
            this.originalHeaders=null;//保存表头信息，用于查看第二页的时候，表头已经变更顺序了，但是tbody的顺序还是旧的，做一个描点
            this.init();
        }
        //可能只是用于清理
    }

    //初始化
    init() {
        if (this.isInitialized) return;
        this.saveOriginalOrder();
        var load = this.loadColumnOrder(true);
        if (!load) {
            this.bindDragEvents();
        }
        this.isInitialized = true;
    }

    //重新绑定顺序并绑定事件 外部表格数据重新载入后，重新执行绑定顺序
    rebind() {
        setTimeout(() => {
            var load = this.loadColumnOrder(false);
            if (!load) {
                this.bindDragEvents();
            } else {
                // console.log('从新加载了顺序');
            }
        }, 0);
    }

    // 生成基于页面路径的MD5加密key
    generateStorageKey() {
        const path = window.location.pathname + location.search;
        return 'table_column_order_' + this.md5(path);
    }

    // 简单的MD5实现（适用于这个场景）
    md5(input) {
        let hash = 0;
        if (input.length === 0) return hash.toString(16);
        for (let i = 0; i < input.length; i++) {
            const char = input.charCodeAt(i);
            hash = ((hash << 5) - hash) + char;
            hash = hash & hash; // Convert to 32bit integer
        }
        return Math.abs(hash).toString(16);
    }

    /**
     * 清理当前页面缓存
     */
    cleanOrderCache() {
        var key = this.generateStorageKey();
        localStorage.removeItem(key);
        //清理后，UI动了没动？
        location.reload();
    }

    // 保存原始列顺序
    saveOriginalOrder() {
        const headers = this.getColumnHeaders();
        this.originalHeaders = headers;
        this.originalColumnOrder = headers.map(th => th.dataset.column);
    }

    // 获取所有表头
    getColumnHeaders() {
        return Array.from(this.table.querySelectorAll('thead th'));
    }

    // 获取所有数据行
    getDataRows() {
        return Array.from(this.table.querySelectorAll('tbody tr'));
    }

    // 加载保存的列顺序
    loadColumnOrder(isall) {
        const savedOrder = localStorage.getItem(this.generateStorageKey());
        if (savedOrder) {
            try {
                const order = JSON.parse(savedOrder);
                this.applyColumnOrder(order, isall);
                this.showStatus('已加载您保存的列顺序');
                return true;
            } catch (e) {
                console.error('加载列顺序失败:', e);
            }
        }
        return false;
    }

    // 应用列顺序
    applyColumnOrder(columnOrder, isall) {
        
        var headers =null;

        if(isall || this.originalHeaders==null){
            headers = this.getColumnHeaders();
        }else{
            headers = this.originalHeaders;
        }
        // const headers = this.getColumnHeaders();

        const rows = this.getDataRows();
        // console.log(headers);

        // 创建当前列的索引映射
        const currentIndexMap = new Map();
        headers.forEach((th, index) => {
            currentIndexMap.set(th.dataset.column, index);
        });

        if (isall) {
            // 重新构建表头
            const theadRow = this.table.querySelector('thead tr');
            const newHeaders = [];

            columnOrder.forEach(column => {
                const originalIndex = currentIndexMap.get(column);
                if (originalIndex !== undefined) {
                    newHeaders.push(headers[originalIndex].cloneNode(true));
                }
            });

            // 清空并重新添加表头
            theadRow.innerHTML = '';
            newHeaders.forEach(th => theadRow.appendChild(th));
        }
        // 重新构建数据行
        rows.forEach(row => {
            const cells = Array.from(row.querySelectorAll('td'));
            const newCells = [];

            columnOrder.forEach(column => {
                const originalIndex = currentIndexMap.get(column);
                if (originalIndex !== undefined && cells[originalIndex]) {
                    newCells.push(cells[originalIndex].cloneNode(true));
                }
            });

            // 替换行内容
            row.innerHTML = '';
            newCells.forEach(td => row.appendChild(td));
            // console.log('写入了新的行',row.innerHTML);
        });

        // 重新绑定事件
        this.bindDragEvents();
    }

    // 绑定拖拽事件 - 修复版本
    bindDragEvents() {
        const headers = this.getColumnHeaders();
        const dragHandles = this.table.querySelectorAll('.drag-handle');

        // 为拖拽手柄绑定事件
        dragHandles.forEach(handle => {
            // 移除旧事件，绑定新事件
            const newHandle = handle.cloneNode(true);
            handle.parentNode.replaceChild(newHandle, handle);

            newHandle.addEventListener('mousedown', (e) => {
                e.stopPropagation(); // 阻止事件冒泡到表头
                this.startDrag(e);
            });

            // 阻止拖拽手柄的点击事件冒泡到表头
            newHandle.addEventListener('click', (e) => {
                e.stopPropagation();
            });
        });

        // 表头点击事件（用于排序）
        headers.forEach(th => {
            th.addEventListener('click', (e) => {
                // 如果点击的是拖拽手柄，不触发排序
                if (e.target.classList.contains('drag-handle')) {
                    return;
                }
                // 这里可以添加表头排序逻辑
                // console.log('点击表头排序:', th.dataset.column);
            });
        });

        // 全局拖拽事件
        document.addEventListener('mousemove', (e) => {
            this.handleDrag(e);
        });

        document.addEventListener('mouseup', () => {
            this.endDrag();
        });
    }

    // 开始拖拽
    startDrag(e) {
        const handle = e.target;
        const th = handle.closest('th');
        const headers = this.getColumnHeaders();
        this.draggedColumn = th;
        this.draggedColumnIndex = headers.indexOf(th);

        th.classList.add('dragging');

        // 创建拖拽镜像
        this.dragGhost = th.cloneNode(true);
        this.dragGhost.style.position = 'absolute';
        this.dragGhost.style.opacity = '0.8';
        this.dragGhost.style.pointerEvents = 'none';
        this.dragGhost.style.zIndex = '1000';
        this.dragGhost.style.width = th.offsetWidth + 'px';
        document.body.appendChild(this.dragGhost);

        this.updateDragGhost(e);
    }

    // 处理拖拽
    handleDrag(e) {
        if (!this.draggedColumn) return;

        this.updateDragGhost(e);
        this.updateDropTarget(e);
    }

    // 更新拖拽镜像位置
    updateDragGhost(e) {
        if (!this.dragGhost) return;

        this.dragGhost.style.left = (e.clientX - this.dragGhost.offsetWidth / 2) + 'px';
        this.dragGhost.style.top = (e.clientY - 10) + 'px';
    }

    // 更新放置目标
    updateDropTarget(e) {
        const headers = this.getColumnHeaders();

        // 移除所有drop-target样式
        headers.forEach(header => {
            header.classList.remove('drop-target', 'drop-target-left', 'drop-target-right');
        });

        // 找到最近的表头
        const targetTh = document.elementFromPoint(e.clientX, e.clientY)?.closest('th');
        if (!targetTh || targetTh === this.draggedColumn) return;

        const targetRect = targetTh.getBoundingClientRect();
        const targetCenterX = targetRect.left + targetRect.width / 2;

        if (e.clientX < targetCenterX) {
            targetTh.classList.add('drop-target', 'drop-target-left');
        } else {
            targetTh.classList.add('drop-target', 'drop-target-right');
        }
    }

    // 结束拖拽
    endDrag() {
        if (!this.draggedColumn) return;

        const headers = this.getColumnHeaders();
        const targetTh = headers.find(th => th.classList.contains('drop-target'));

        if (targetTh && targetTh !== this.draggedColumn) {
            const targetIndex = headers.indexOf(targetTh);
            const targetRect = targetTh.getBoundingClientRect();
            const dropPosition = targetTh.classList.contains('drop-target-left') ? 'left' : 'right';

            this.moveColumn(this.draggedColumnIndex, targetIndex, dropPosition);
            this.saveColumnOrder();
        }

        // 清理
        this.draggedColumn.classList.remove('dragging');
        headers.forEach(header => {
            header.classList.remove('drop-target', 'drop-target-left', 'drop-target-right');
        });

        if (this.dragGhost) {
            this.dragGhost.remove();
            this.dragGhost = null;
        }

        this.draggedColumn = null;
        this.draggedColumnIndex = -1;
    }

    // 移动列 - 修复最后一列问题
    moveColumn(fromIndex, toIndex, dropPosition) {
        if (fromIndex === toIndex) return;

        const headers = this.getColumnHeaders();
        const rows = this.getDataRows();
        const theadRow = this.table.querySelector('thead tr');

        // 计算实际的目标位置
        let actualTargetIndex = toIndex;
        if (dropPosition === 'right' && toIndex < headers.length - 1) {
            actualTargetIndex = toIndex + 1;
        }

        // 如果是拖拽到最后一个位置的右边，直接放到最后
        if (dropPosition === 'right' && toIndex === headers.length - 1) {
            actualTargetIndex = headers.length;
        }

        const thToMove = headers[fromIndex];

        // 移动表头
        if (actualTargetIndex >= headers.length) {
            // 放到最后
            theadRow.appendChild(thToMove);
        } else {
            const thTarget = headers[actualTargetIndex];
            theadRow.insertBefore(thToMove, thTarget);
        }

        // 移动所有数据行的单元格
        rows.forEach(row => {
            const cells = Array.from(row.cells);
            const cellToMove = cells[fromIndex];

            if (actualTargetIndex >= cells.length) {
                // 放到最后
                row.appendChild(cellToMove);
            } else {
                const cellTarget = cells[actualTargetIndex];
                row.insertBefore(cellToMove, cellTarget);
            }
        });
    }

    // 保存列顺序
    saveColumnOrder() {
        const headers = this.getColumnHeaders();
        const currentOrder = headers.map(th => th.dataset.column);

        localStorage.setItem(this.generateStorageKey(), JSON.stringify(currentOrder));
        this.showStatus('列顺序已保存到本地');
    }

    // 重置列顺序
    resetColumnOrder() {
        this.applyColumnOrder(this.originalColumnOrder, true);
        this.showStatus('已重置为默认列顺序');
    }

    // 显示状态信息
    showStatus(message) {
        const statusEl = document.getElementById('status');
        if (statusEl) {
            statusEl.textContent = message;
            statusEl.style.display = 'block';
            setTimeout(() => {
                statusEl.style.display = 'none';
            }, 3000);
        }
    }
}
