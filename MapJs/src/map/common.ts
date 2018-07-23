
if (!Array.prototype.first) {
    Array.prototype.first = function (query) {
        var arr = this;
        if (query && arr) {
            for (var i = 0; i < arr.length; i++) {
                if (query(arr[i])) {
                    return arr[i];
                }
            }
        }
    }
}
if (!Array.prototype.avg) {
    Array.prototype.avg = function (query) {
        var arr = this;
        if (query && arr) {
            var result = null;
            for (var i = 0; i < arr.length; i++) {
                var val = query(arr[i]);
                if (result === null) {
                    result = val;
                } else {
                    result = (result * i + val) / (i + 1);
                }
            }
            return result;
        }
    }
}
if (!Array.prototype.max) {
    Array.prototype.max = function (query) {
        var arr = this;
        if (query && arr) {
            var result = null;
            for (var i = 0; i < arr.length; i++) {
                var val = query(arr[i]);
                if (result === null) {
                    result = arr[i];
                } else {
                    result = query(result) < val ? arr[i] : result;
                }
            }
            return result;
        }
    }
}
if (!Array.prototype.min) {
    Array.prototype.min = function (query) {
        var arr = this;
        if (query && arr) {
            var result = null;
            for (var i = 0; i < arr.length; i++) {
                var val = query(arr[i]);
                if (result === null) {
                    result = arr[i];
                } else {
                    result = query(result) > val ? arr[i] : result;
                }
            }
            return result;
        }
    }
}
if (!Array.prototype.select) {
    Array.prototype.select = function (query) {
        var arr = this;
        if (query && arr) {
            var tmp = [];
            arr.forEach(o => tmp.push(query(o)));
            arr = tmp;
        }
        return arr;
    }
}
if (!Array.prototype.selectMany) {
    Array.prototype.selectMany = function (query) {
        var arr = this;
        if (query && arr) {
            var tmp = [];
            arr.forEach(o => tmp = tmp.concat(query(o)));
            arr = tmp;
        }
        return arr;
    }
}

const enum MapEvents {
    /**地图加载事件 */
    load = "load",
    /** */
    pointConvert = "pointConvert",
    horizontalAspect = "horizontalAspect",
    verticalAspect = "verticalAspect",
    clearAspect = "clearAspect",
    selectAnalysisArea = "selectAnalysisArea",
    clearAnalysisArea = "clearAnalysisArea",
    savePoints = "savePoints",
    boundChanged = "boundChanged",
    blockChanged = "blockChanged",
    uavChanged = "uavChanged",
    reportDisplay = "reportDisplay",
}
// (window as {MapEvents?}).MapEvents=MapEvents;

const enum MapMenuItems {
    refresh = "刷新地图",
    uavPath = "无人机路径",
    uavLocation = "无人机定位",
    uavFollow = "无人机跟随",
    compare = "对比数据",
    reports = "统计报表",
    savePoints = "保存",
    horizontal = "横向切面",
    vertical = "纵向切面",
    selectAnalysisArea = "选择分析区域",
    clearAnalysisArea = "清除分析区域",
    clear = "清除",
}
// (window as {MapMenuItems?}).MapMenuItems=MapMenuItems;
/**地图方块选择动作 */
const enum MapBlockSelectAction {
    //开关
    switch,
    //强制选择
    focusSelect,
    //强制反选
    focusUnselect,
}
// (window as {MapBlockSelectAction?}).MapBlockSelectAction=MapBlockSelectAction;

