var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
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
    };
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
                }
                else {
                    result = (result * i + val) / (i + 1);
                }
            }
            return result;
        }
    };
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
                }
                else {
                    result = query(result) < val ? arr[i] : result;
                }
            }
            return result;
        }
    };
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
                }
                else {
                    result = query(result) > val ? arr[i] : result;
                }
            }
            return result;
        }
    };
}
if (!Array.prototype.select) {
    Array.prototype.select = function (query) {
        var arr = this;
        if (query && arr) {
            var tmp = [];
            arr.forEach(function (o) { return tmp.push(query(o)); });
            arr = tmp;
        }
        return arr;
    };
}
if (!Array.prototype.selectMany) {
    Array.prototype.selectMany = function (query) {
        var arr = this;
        if (query && arr) {
            var tmp = [];
            arr.forEach(function (o) { return tmp = tmp.concat(query(o)); });
            arr = tmp;
        }
        return arr;
    };
}
var BlockContext = /** @class */ (function () {
    function BlockContext(center, pollutants) {
        var _this = this;
        this.points = [];
        this.reports = [];
        this.center = center;
        pollutants.forEach(function (o) {
            var report = new PollutantReport();
            report.pollutant = o;
            _this.reports.push(report);
        });
    }
    BlockContext.prototype.addPoint = function (p) {
        if (this.points.first(function (o) { return o == p; }))
            return;
        if (p.data) {
            if (!this.time) {
                this.time = p.data["time"];
            }
            this.reports.forEach(function (o) {
                var val = p.data[o.pollutant.Name];
                if (val) {
                    o.avg = (o.avg * o.count + val) / (o.count + 1);
                    if (o.count == 0) {
                        o.max = val;
                        o.min = val;
                        o.sum = val;
                    }
                    else {
                        o.max = val > o.max ? val : o.max;
                        o.min = val < o.min ? val : o.min;
                        o.sum += val;
                    }
                    o.count++;
                }
            });
        }
        this.points.push(p);
    };
    BlockContext.prototype.getPoints = function (query) {
        return this.points.filter(query);
    };
    BlockContext.prototype.getReports = function (query) {
        return this.reports.filter(query);
    };
    return BlockContext;
}());
var PollutantReport = /** @class */ (function () {
    function PollutantReport() {
        this.count = 0;
        this.avg = 0;
        this.sum = 0;
        this.max = 0;
        this.min = 0;
    }
    return PollutantReport;
}());
var PollutantLevel = /** @class */ (function () {
    function PollutantLevel() {
    }
    return PollutantLevel;
}());
var Pollutant = /** @class */ (function () {
    function Pollutant() {
        this.Name = "sample";
        this.DisplayName = "样本";
        this.MaxValue = 100;
        this.MinValue = 1;
        this.Unit = "mg/m3";
        this.Levels = [];
    }
    return Pollutant;
}());
var MapGridOptions = /** @class */ (function () {
    function MapGridOptions() {
    }
    return MapGridOptions;
}());
var MapGrid = /** @class */ (function () {
    function MapGrid() {
        this.selectedBlocks = [];
        this.selectedBlockLine = [];
    }
    return MapGrid;
}());
var Uav = /** @class */ (function () {
    function Uav() {
    }
    return Uav;
}());
var EventSubscribe = /** @class */ (function () {
    function EventSubscribe() {
    }
    return EventSubscribe;
}());
var MapBase = /** @class */ (function () {
    function MapBase() {
        this.m_events = [];
    }
    MapBase.prototype.loadJs = function (url, onLoad) {
        try {
            var file = document.createElement("script");
            file.setAttribute("type", "text/javascript");
            file.setAttribute("src", url);
            file.onload = onLoad;
            document.getElementsByTagName("head")[0].appendChild(file);
        }
        catch (e) {
            alert(e);
        }
    };
    MapBase.prototype.parseJson = function (obj) {
        if (typeof (obj) == "string")
            obj = JSON.parse(obj);
        return obj;
    };
    MapBase.prototype.on = function (eventName, arg) {
        try {
            var sub = this.m_events.first(function (o) { return o.name == eventName; });
            if (sub && sub.enable) {
                if (arg) {
                    arg = JSON.stringify(arg);
                }
                window.external.On(eventName, arg);
                return;
            }
        }
        catch (e) {
            //ignore;
        }
        console.log("triger event [%s] arguments is :", eventName);
        console.dir(arg);
    };
    MapBase.prototype.subscribe = function (eventName, enable) {
        var evt = this.m_events.first(function (o) { return o.name == eventName; });
        if (evt) {
            evt.enable = enable;
        }
        else {
            this.m_events.push({ name: eventName, enable: enable });
        }
        if (enable) {
            return this.onSubscribe(eventName);
        }
    };
    return MapBase;
}());
/**
 *百度地图选择器。用于界面元素框选。
 */
var BaiduMapSelector = /** @class */ (function () {
    /**
     * 创建百度地图选择器，默认使用右键进行框选。
     * @param map 百度地图对象。
     * @param callbackFn 选择后回调框选区域。
     */
    function BaiduMapSelector(map, callbackFn) {
        var _this = this;
        this.enable = true;
        this.map = map;
        this.callback = callbackFn;
        map.addEventListener("mousedown", function (o) {
            if (!_this.enable)
                return;
            if (o.domEvent.which == 3 || o.domEvent.button == 2) {
                var selector = _this.selector;
                if (!selector) {
                    selector = _this.selector = new BMap.Polygon([], {
                        strokeColor: "blue",
                        strokeWeight: 1,
                        fillColor: "blue",
                        fillOpacity: 0.1,
                    });
                }
                var point = _this.pointOne = o.point;
                var mixOffset = 0.000001;
                selector.setPath([
                    point,
                    new BMap.Point(point.lng + mixOffset, point.lat),
                    new BMap.Point(point.lng + mixOffset, point.lat + mixOffset),
                    new BMap.Point(point.lng, point.lat + mixOffset),
                ]);
                map.addOverlay(selector);
            }
        });
        map.addEventListener("mousemove", function (o) {
            var selector = _this.selector;
            var p1 = _this.pointOne;
            if (selector && p1) {
                var p2 = o.point;
                selector.setPath([
                    new BMap.Point(p1.lng, p1.lat),
                    new BMap.Point(p1.lng, p2.lat),
                    new BMap.Point(p2.lng, p2.lat),
                    new BMap.Point(p2.lng, p1.lat),
                ]);
            }
        });
        map.addEventListener("mouseup", function (o) {
            var selector = _this.selector;
            if (selector) {
                if (_this.callback) {
                    _this.callback({ bound: selector.getBounds(), event: o });
                }
                _this.map.removeOverlay(selector);
                delete _this.pointOne;
                delete _this.selector;
            }
        });
    }
    BaiduMapSelector.prototype.setEnable = function (enable) {
        this.enable = enable;
    };
    BaiduMapSelector.prototype.getEnable = function () { return this.enable; };
    return BaiduMapSelector;
}());
/**数据分析区域。 */
var BaiduMapAnalysisArea = /** @class */ (function () {
    function BaiduMapAnalysisArea(map, evt) {
        this.map = map;
        this.evt = evt;
    }
    BaiduMapAnalysisArea.prototype.isEnabled = function () {
        return this.selectingArea || this.border;
    };
    BaiduMapAnalysisArea.prototype.setBounds = function (bound) {
        if (bound) {
            this.bound = bound;
            var p1 = bound.getSouthWest();
            var p2 = bound.getNorthEast();
            this.border = new BMap.Polygon([
                p1,
                new BMap.Point(p1.lng, p2.lat),
                p2,
                new BMap.Point(p2.lng, p1.lat),
            ], {
                strokeColor: "green",
                strokeStyle: "dashed",
                strokeWeight: 1,
                strokeOpacity: 1,
                enableClicking: false,
                fillColor: "transparent"
            });
            this.map.addOverlay(this.border);
            this.evt.on(MapEvents.selectAnalysisArea, { sw: bound.getSouthWest(), ne: bound.getNorthEast() });
            this.selectingArea = false;
        }
    };
    BaiduMapAnalysisArea.prototype.getBounds = function () {
        return this.bound;
    };
    BaiduMapAnalysisArea.prototype.enable = function () {
        this.disable();
        this.selectingArea = true;
    };
    BaiduMapAnalysisArea.prototype.disable = function () {
        this.selectingArea = false;
        if (this.border) {
            this.map.removeOverlay(this.border);
            delete this.bound;
            delete this.border;
            this.evt.on(MapEvents.clearAnalysisArea);
        }
    };
    return BaiduMapAnalysisArea;
}());
var MapEvents;
(function (MapEvents) {
    MapEvents["load"] = "load";
    MapEvents["pointConvert"] = "pointConvert";
    MapEvents["horizontalAspect"] = "horizontalAspect";
    MapEvents["verticalAspect"] = "verticalAspect";
    MapEvents["clearAspect"] = "clearAspect";
    MapEvents["selectAnalysisArea"] = "selectAnalysisArea";
    MapEvents["clearAnalysisArea"] = "clearAnalysisArea";
    MapEvents["savePoints"] = "savePoints";
    MapEvents["boundChanged"] = "boundChanged";
    MapEvents["blockChanged"] = "blockChanged";
    MapEvents["uavChanged"] = "uavChanged";
})(MapEvents || (MapEvents = {}));
var MapMenuItems;
(function (MapMenuItems) {
    MapMenuItems["refresh"] = "\u5237\u65B0\u5730\u56FE";
    MapMenuItems["uavPath"] = "\u65E0\u4EBA\u673A\u8DEF\u5F84";
    MapMenuItems["uavLocation"] = "\u65E0\u4EBA\u673A\u5B9A\u4F4D";
    MapMenuItems["uavFollow"] = "\u65E0\u4EBA\u673A\u8DDF\u968F";
    MapMenuItems["compare"] = "\u5BF9\u6BD4\u6570\u636E";
    MapMenuItems["reports"] = "\u7EDF\u8BA1\u62A5\u8868";
    MapMenuItems["savePoints"] = "\u4FDD\u5B58";
    MapMenuItems["horizontal"] = "\u6A2A\u5411\u5207\u9762";
    MapMenuItems["vertical"] = "\u7EB5\u5411\u5207\u9762";
    MapMenuItems["selectAnalysisArea"] = "\u9009\u62E9\u5206\u6790\u533A\u57DF";
    MapMenuItems["clearAnalysisArea"] = "\u6E05\u9664\u5206\u6790\u533A\u57DF";
    MapMenuItems["clear"] = "\u6E05\u9664";
})(MapMenuItems || (MapMenuItems = {}));
/**地图方块选择动作 */
var MapBlockSelectAction;
(function (MapBlockSelectAction) {
    //开关
    MapBlockSelectAction[MapBlockSelectAction["switch"] = 0] = "switch";
    //强制选择
    MapBlockSelectAction[MapBlockSelectAction["focusSelect"] = 1] = "focusSelect";
    //强制反选
    MapBlockSelectAction[MapBlockSelectAction["focusUnselect"] = 2] = "focusUnselect";
})(MapBlockSelectAction || (MapBlockSelectAction = {}));
var BaiduMapProvider = /** @class */ (function (_super) {
    __extends(BaiduMapProvider, _super);
    function BaiduMapProvider() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.uavFollow = true;
        _this.uavPath = false;
        return _this;
    }
    BaiduMapProvider.prototype.getColor = function (value, min, max, minColor, maxColor) {
        if (min === void 0) { min = undefined; }
        if (max === void 0) { max = undefined; }
        if (minColor === void 0) { minColor = undefined; }
        if (maxColor === void 0) { maxColor = undefined; }
        var opt = this.blockGrid.options;
        if (!min)
            min = opt.pollutant.MinValue;
        if (!max)
            max = opt.pollutant.MaxValue;
        if (!maxColor)
            maxColor = "#ff0000";
        if (!minColor)
            minColor = "#00ff00";
        var percent = (value - min) / (max - min);
        percent = percent > 1 ? 1 : percent;
        percent = percent < 0 ? 0 : percent;
        var br = parseInt(minColor.substring(1, 3), 16);
        var bg = parseInt(minColor.substring(3, 5), 16);
        var bb = parseInt(minColor.substring(5, 7), 16);
        var er = parseInt(maxColor.substring(1, 3), 16);
        var eg = parseInt(maxColor.substring(3, 5), 16);
        var eb = parseInt(maxColor.substring(5, 7), 16);
        var getColorValue = function (percent, begin, end) {
            var flag = end > begin ? 1 : -1;
            var result = Math.round(Math.abs(end - begin) * percent * flag + begin).toString(16);
            return result.length < 2 ? "0" + result : result;
        };
        return "#" + getColorValue(percent, br, er) +
            getColorValue(percent, bg, eg) +
            getColorValue(percent, bb, eb);
    };
    BaiduMapProvider.prototype.getColorByReport = function (report) {
        var lv = report.pollutant.Levels.first(function (o) { return o.MaxValue >= report.avg && o.MinValue <= report.avg; });
        if (!lv) {
            if (report.avg > report.pollutant.MaxValue) {
                lv = report.pollutant.Levels.max(function (o) { return o.MaxValue; });
            }
            if (report.avg < report.pollutant.MinValue) {
                lv = report.pollutant.Levels.min(function (o) { return o.MinValue; });
            }
        }
        if (!lv)
            return "#ffffff";
        return this.getColor(report.avg, lv.MinValue, lv.MaxValue, lv.MinColor, lv.MaxColor);
    };
    BaiduMapProvider.prototype.getInfoWindowContentTemplate = function (opt) {
        var template = '<div class="pollutant_message"><div class="pollutant_block" style="background:{{background}};opacity:{{opacity}}"></div><div class="pollutant_title">{{title}}</div><div class="pollutant_val">{{min}}</div><div class="pollutant_val">{{avg}}</div><div class="pollutant_val">{{max}}</div><div class="pollutant_unit">{{unit}}</div></div>';
        return template
            .replace("{{title}}", opt.title)
            .replace("{{min}}", opt.min)
            .replace("{{max}}", opt.max)
            .replace("{{avg}}", opt.avg)
            .replace("{{unit}}", opt.unit)
            .replace("{{background}}", opt.background)
            .replace("{{opacity}}", opt.opacity);
    };
    BaiduMapProvider.prototype.createInfoWindowContent = function (report) {
        return this.getInfoWindowContentTemplate({
            title: report.pollutant.DisplayName,
            min: Math.round(report.min * 100) / 100,
            max: Math.round(report.max * 100) / 100,
            avg: Math.round(report.avg * 100) / 100,
            unit: report.pollutant.Unit,
            background: this.getColorByReport(report),
            opacity: this.blockGrid.options.opacity,
        });
    };
    /**
     * 创建方块。
     * @param point 中心点。
     * @param opt 选项。
     */
    BaiduMapProvider.prototype.createBlock = function (point, opt) {
        var _this = this;
        var center = this.blockGrid.firstPoint;
        var sideLength = opt.sideLength * 0.00001;
        var opacity = opt.opacity;
        var offset = sideLength / 2; //计算偏移经纬度。
        var lng = center.lng + sideLength * Math.round((point.lng - center.lng) / sideLength);
        var lat = center.lat + sideLength * Math.round((point.lat - center.lat) / sideLength);
        var polygon = new BMap.Polygon([
            new BMap.Point(lng + offset, lat + offset),
            new BMap.Point(lng - offset, lat + offset),
            new BMap.Point(lng - offset, lat - offset),
            new BMap.Point(lng + offset, lat - offset),
        ], {
            fillOpacity: opacity,
            strokeWeight: 1,
            strokeOpacity: 0.2,
            strokeColor: "white"
        });
        var context = new BlockContext(new BMap.Point(lng, lat), opt.pollutants);
        context.addPoint(point);
        polygon.context = context;
        polygon.addEventListener("click", function (o) { return _this.onShowBlockReport(o.target); });
        polygon.addEventListener("rightclick", function (o) { return _this.onSelectBlock(o.target); });
        return polygon;
    };
    /**
     * 选中方块。
     * @param b 方块对象。
     * @param act 强制选中或者不选中。
     */
    BaiduMapProvider.prototype.onSelectBlock = function (b, act) {
        if (act === void 0) { act = MapBlockSelectAction.switch; }
        var index = null;
        var block = null;
        for (var i = 0; i < this.blockGrid.selectedBlocks.length; i++) {
            block = this.blockGrid.selectedBlocks[i];
            if (block == b) {
                index = i;
                break;
            }
        }
        if (index === null) {
            if (act == MapBlockSelectAction.focusUnselect)
                return;
            block = b;
            block.setStrokeColor("red");
            block.setStrokeOpacity(1);
            block.setStrokeWeight(1);
            block.setStrokeStyle("solid");
            this.blockGrid.selectedBlocks.push(block);
        }
        else {
            if (act == MapBlockSelectAction.focusSelect)
                return;
            block.setStrokeColor("white");
            block.setStrokeOpacity(0.2);
            block.setStrokeWeight(1);
            block.setStrokeStyle("solid");
            this.blockGrid.selectedBlocks.splice(i, 1);
        }
    };
    /**
     * 查找当前加载的无人机。
     * @param name 无人机名称、标识
     * @param exist 如果存在执行操作。
     * @param notExist 如果不存在执行操作。
     */
    BaiduMapProvider.prototype.uav = function (name, exist, notExist) {
        try {
            var uav = this.uavList.first(function (o) { return o.name == name; });
            if (uav) {
                if (exist) {
                    exist(uav);
                }
                else {
                    console.log("uav named :%s already existed", name);
                }
            }
            else {
                if (notExist) {
                    notExist();
                }
                else {
                    console.log("uav named :%s not found", name);
                }
            }
        }
        catch (e) {
            // alert(e.message);
        }
    };
    /** 地图显示发生变更。 */
    BaiduMapProvider.prototype.onMapBoundChanged = function () {
        var bound = this.map.getBounds();
        this.on(MapEvents.boundChanged, { bound: { sw: bound.getSouthWest(), ne: bound.getNorthEast() } });
    };
    BaiduMapProvider.prototype.onCheckContextMenu = function () {
        var _this = this;
        var blocks = this.blockGrid.selectedBlocks;
        var setEnable = function (name, enable) {
            var i = _this.menuItems.first(function (o) { return o.name == name; });
            if (i) {
                if (enable) {
                    i.enable();
                }
                else {
                    i.disable();
                }
            }
        };
        var setChecked = function (name, checked) {
            var i = _this.menuItems.first(function (o) { return o.name == name; });
            if (i) {
                if (checked) {
                    i.setText(name + " √");
                }
                else {
                    i.setText(name);
                }
            }
        };
        if (!blocks) {
            this.menuItems.forEach(function (o) { if (o)
                o.disable(); });
        }
        else {
            setEnable(MapMenuItems.savePoints, blocks.length > 0);
            setEnable(MapMenuItems.reports, blocks.length > 0);
            setEnable(MapMenuItems.horizontal, blocks.length > 0);
            setEnable(MapMenuItems.vertical, blocks.length > 0);
            setEnable(MapMenuItems.clear, blocks.length > 0);
        }
        setEnable(MapMenuItems.selectAnalysisArea, !this.analysisArea.isEnabled());
        setEnable(MapMenuItems.clearAnalysisArea, this.analysisArea.isEnabled());
        setChecked(MapMenuItems.uavFollow, this.uavFollow);
        setChecked(MapMenuItems.uavPath, this.uavPath);
    };
    BaiduMapProvider.prototype.addLine = function (point, horizontalLen, verticalLen) {
        var line = new BMap.Polyline([
            new BMap.Point(point.lng - (horizontalLen / (2 * 10000)), point.lat - (verticalLen / (2 * 10000))),
            new BMap.Point(point.lng + (horizontalLen / (2 * 10000)), point.lat + (verticalLen / (2 * 10000))),
        ], {
            strokeStyle: "dashed",
            strokeWeight: 1,
            strokeOpacity: 0.8
        });
        this.map.addOverlay(line);
        this.blockGrid.selectedBlockLine.push(line);
    };
    BaiduMapProvider.prototype.onShowVerticalAspect = function () {
        var _this = this;
        var blocks = this.blockGrid.selectedBlocks;
        if (blocks) {
            var min = blocks.min(function (o) { return o.context.center.lng; });
            var max = blocks.max(function (o) { return o.context.center.lng; });
            this.blockGrid.selectedBlockLine.forEach(function (o) { return _this.map.removeOverlay(o); });
            this.addLine(min.getBounds().getSouthWest(), 0, 10000);
            this.addLine(max.getBounds().getNorthEast(), 0, 10000);
            this.on(MapEvents.verticalAspect, {
                blocks: this.getBlocksData(blocks)
            });
        }
    };
    BaiduMapProvider.prototype.onClearSelectedBlock = function () {
        var _this = this;
        this.blockGrid.selectedBlockLine.forEach(function (o) { return _this.map.removeOverlay(o); });
        this.blockGrid.selectedBlocks.filter(function (o) { return true; }).forEach(function (o) { return _this.onSelectBlock(o); });
        this.on(MapEvents.clearAspect);
    };
    BaiduMapProvider.prototype.onShowHorizontalAspect = function () {
        var _this = this;
        var blocks = this.blockGrid.selectedBlocks;
        if (blocks) {
            var min = blocks.min(function (o) { return o.context.center.lat; });
            var max = blocks.max(function (o) { return o.context.center.lat; });
            this.blockGrid.selectedBlockLine.forEach(function (o) { return _this.map.removeOverlay(o); });
            this.addLine(min.getBounds().getSouthWest(), 10000, 0);
            this.addLine(max.getBounds().getNorthEast(), 10000, 0);
            this.on(MapEvents.horizontalAspect, {
                blocks: this.getBlocksData(blocks)
            });
        }
    };
    BaiduMapProvider.prototype.getBlocksBounds = function (blocks) {
        var minLat = blocks.min(function (o) { return o.context.center.lat; }).getBounds().getSouthWest().lat;
        var maxLat = blocks.max(function (o) { return o.context.center.lat; }).getBounds().getNorthEast().lat;
        var minLng = blocks.min(function (o) { return o.context.center.lng; }).getBounds().getSouthWest().lng;
        var maxLng = blocks.max(function (o) { return o.context.center.lng; }).getBounds().getNorthEast().lng;
        return new BMap.Bounds(new BMap.Point(minLng, minLat), new BMap.Point(maxLng, maxLat));
    };
    BaiduMapProvider.prototype.onShowSelectedBlockReport = function () {
        var blocks = this.blockGrid.selectedBlocks;
        if (blocks) {
            var bound = this.getBlocksBounds(blocks);
            var reports = [];
            var time;
            blocks.forEach(function (block) {
                if (!time) {
                    time = block.context.time;
                }
                var rp = block.context.getReports(function (i) { return true; });
                rp.forEach(function (report) {
                    var tmp = reports.first(function (o) { return o.pollutant.Name == report.pollutant.Name; });
                    if (!tmp) {
                        tmp = new PollutantReport();
                        tmp.max = report.max;
                        tmp.min = report.min;
                        tmp.avg = report.avg;
                        tmp.count = 0;
                        tmp.pollutant = report.pollutant;
                        reports.push(tmp);
                    }
                    tmp.avg = (tmp.avg * tmp.count + report.avg) / (tmp.count + 1);
                    tmp.max = report.max > tmp.max ? report.max : tmp.max;
                    tmp.min = report.min < tmp.min ? report.min : tmp.min;
                    tmp.sum += report.sum;
                    tmp.count++;
                });
            });
            this.onShowReport(bound, reports, time);
        }
    };
    BaiduMapProvider.prototype.onShowBlockReport = function (block) {
        this.onShowReport(block.getBounds(), block.context.getReports(function (o) { return true; }), block.context.time);
    };
    BaiduMapProvider.prototype.onShowReport = function (bound, reports, time) {
        var _this = this;
        var blockGrid = this.blockGrid;
        var opt = blockGrid.options;
        if (!blockGrid.infoWindow) {
            blockGrid.infoWindow = new BMap.InfoWindow("", {
                width: 450,
                height: 350
            });
            blockGrid.infoWindow.targetBorder = new BMap.Polygon([], {
                strokeColor: "blue",
                strokeWeight: 1,
                fillOpacity: 0.2,
                fillColor: "blue",
            });
            blockGrid.infoWindow.addEventListener("close", function (o) {
                var win = o.target;
                //console.dir(win);
                if (win.targetBorder) {
                    _this.map.removeOverlay(win.targetBorder);
                }
            });
        }
        var p1 = bound.getNorthEast();
        var p2 = bound.getSouthWest();
        blockGrid.infoWindow.targetBorder.setPath([
            p1,
            new BMap.Point(p1.lng, p2.lat),
            p2,
            new BMap.Point(p2.lng, p1.lat),
        ]);
        var content = '<div><span>实时采样数据：</span><span>({{time}})</span><br /><span>东经:{{minLng}}-{{maxLng}}</span><br/><span>北纬:{{minLat}}-{{maxLat}}</span></div>';
        content = content.replace("{{time}}", time);
        content = content.replace("{{minLng}}", p2.lng.toFixed(6));
        content = content.replace("{{maxLng}}", p1.lng.toFixed(6));
        content = content.replace("{{minLat}}", p2.lat.toFixed(6));
        content = content.replace("{{maxLat}}", p1.lat.toFixed(6));
        content += this.getInfoWindowContentTemplate({
            title: "采样类型",
            min: "最小值",
            max: "最大值",
            avg: "平均值",
            unit: "单位",
            background: "white",
            opacity: 1,
        });
        reports.forEach(function (o) { return content += _this.createInfoWindowContent(o); });
        blockGrid.infoWindow.setContent(content);
        this.map.openInfoWindow(blockGrid.infoWindow, bound.getNorthEast());
        this.map.addOverlay(blockGrid.infoWindow.targetBorder);
    };
    BaiduMapProvider.prototype.onSaveSelectedBlocks = function () {
        if (this.tempSelectedData && this.tempSelectedData.length) {
            this.on(MapEvents.savePoints, { points: this.tempSelectedData });
            delete this.tempSelectedData;
            return;
        }
        var blocks = this.blockGrid.selectedBlocks;
        if (blocks && blocks.length) {
            this.on(MapEvents.savePoints, { points: blocks.selectMany(function (o) { return o.context.getPoints(function (i) { return true; }).select(function (i) { return i.data; }); }) });
        }
    };
    /**无人机定位。 */
    BaiduMapProvider.prototype.onUavLoaction = function () {
        if (this.uavList) {
            var uav = this.uavList.first(function (i) { return true; });
            if (uav) {
                this.uavFocus(uav.name);
            }
        }
    };
    /**刷新 */
    BaiduMapProvider.prototype.onRefresh = function () {
        window.location.reload(true);
    };
    /**获取地图边界。 */
    BaiduMapProvider.prototype.getMapBounds = function () {
        var bounds = this.map.getBounds();
        if (bounds) {
            return { sw: bounds.getSouthWest(), ne: bounds.getNorthEast() };
        }
    };
    /**获取地图中所有无人机数据 */
    BaiduMapProvider.prototype.getUavData = function () {
        return this.uavList.select(function (o) {
            var i = o.marker.getPosition();
            return {
                lat: i.lat,
                lng: i.lng,
                name: o.name,
            };
        });
    };
    /**获取所有在地图上的方块数据。 */
    BaiduMapProvider.prototype.getBlocksData = function (blocks) {
        var _this = this;
        return blocks.filter(function (o) { return o.context.color; }).select(function (o) {
            var b = o.getBounds();
            return {
                sw: b.getSouthWest(),
                ne: b.getNorthEast(),
                center: o.context.center,
                points: o.context.getPoints(function (i) { return true; }).select(function (i) { return i.data; }),
                reports: o.context.getReports(function (i) { return true; }),
                color: o.context.color,
                opacity: _this.blockGrid.options.opacity
            };
        });
    };
    BaiduMapProvider.prototype.onMapLoad = function (map) {
        var _this = this;
        this.convertor = new BMap.Convertor();
        this.analysisArea = new BaiduMapAnalysisArea(map, this);
        map.centerAndZoom(new BMap.Point(113.140761, 23.033974), 17); // 初始化地图,设置中心点坐标和地图级别
        //添加地图类型控件
        map.addControl(new BMap.MapTypeControl({
            mapTypes: [
                BMAP_NORMAL_MAP,
                BMAP_HYBRID_MAP
            ]
        }));
        //map.addControl(new BMap.ScaleControl());
        //map.addControl(new BMap.NavigationControl());
        map.addControl(new BMap.OverviewMapControl());
        //map.addControl(new BMap.GeolocationControl());
        map.enableScrollWheelZoom(true); //开启鼠标滚轮缩放
        var menu = new BMap.ContextMenu();
        var createItem = function (name, func) {
            var i = new BMap.MenuItem(name, func);
            i.name = name;
            return i;
        };
        this.menuItems = [
            //createItem(MapMenuItems.compare, o => this.onShowReport()),
            createItem(MapMenuItems.refresh, function (o) { return _this.onRefresh(); }),
            false,
            createItem(MapMenuItems.uavLocation, function (o) { return _this.onUavLoaction(); }),
            createItem(MapMenuItems.uavFollow, function (o) { return _this.uavFollow = !_this.uavFollow; }),
            createItem(MapMenuItems.uavPath, function (o) { return _this.uavPath = !_this.uavPath; }),
            false,
            createItem(MapMenuItems.savePoints, function (o) { return _this.onSaveSelectedBlocks(); }),
            createItem(MapMenuItems.reports, function (o) { return _this.onShowSelectedBlockReport(); }),
            createItem(MapMenuItems.horizontal, function (o) { return _this.onShowHorizontalAspect(); }),
            createItem(MapMenuItems.vertical, function (o) { return _this.onShowVerticalAspect(); }),
            createItem(MapMenuItems.clear, function (o) { return _this.onClearSelectedBlock(); }),
            false,
            createItem(MapMenuItems.selectAnalysisArea, function (o) { return _this.analysisArea.enable(); }),
            createItem(MapMenuItems.clearAnalysisArea, function (o) { return _this.analysisArea.disable(); }),
        ];
        this.menuItems.forEach(function (o) { return o ? menu.addItem(o) : menu.addSeparator(); });
        menu.addEventListener("open", function (o) { return _this.onCheckContextMenu(); });
        map.addContextMenu(menu);
        new BaiduMapSelector(map, function (o) {
            if (_this.analysisArea.isEnabled() && !_this.analysisArea.getBounds()) {
                _this.analysisArea.setBounds(o.bound);
            }
            else {
                _this.blockGrid.blocks.forEach(function (b) {
                    if (o.bound.containsPoint(b.context.center)) {
                        if (o.event.shiftKey) {
                            _this.onSelectBlock(b, MapBlockSelectAction.focusUnselect);
                        }
                        else if (o.event.ctrlKey) {
                            _this.onSelectBlock(b, MapBlockSelectAction.focusSelect);
                        }
                        else {
                            _this.onSelectBlock(b);
                        }
                    }
                });
            }
        });
        this.map = map;
        this.blockGrid = new MapGrid();
        this.blockGrid.blocks = new Array();
        this.uavList = new Array();
        this.subscribe(MapEvents.load, true);
        this.subscribe(MapEvents.clearAnalysisArea, true);
        this.subscribe(MapEvents.clearAspect, true);
        this.subscribe(MapEvents.horizontalAspect, true);
        this.subscribe(MapEvents.pointConvert, true);
        this.subscribe(MapEvents.savePoints, true);
        this.subscribe(MapEvents.selectAnalysisArea, true);
        this.subscribe(MapEvents.verticalAspect, true);
        this.on(MapEvents.load);
        var h = setInterval(function () {
            var i = $("a[title='到百度地图查看此区域']");
            var b = $("span[_cid='1']");
            if (!i.hasClass("hide") || !b.hasClass("hide")) {
                i.addClass("hide");
                b.addClass("hide");
            }
            else {
                clearInterval(h);
            }
        }, 100);
        map.addEventListener("moveend", function (o) { return _this.onMapBoundChanged(); });
        map.addEventListener("zoomend", function (o) { return _this.onMapBoundChanged(); });
    };
    /**
     * 初始化地图。
     * @param container 地图容器id
     */
    BaiduMapProvider.prototype.mapInit = function (container) {
        var _this = this;
        this.loadJs("http://api.map.baidu.com/getscript?v=2.0&ak=TCgR2Y0IGMmPR4qteh4McpXzMyYpFrEx", function () { return _this.onMapLoad(new BMap.Map(container)); });
    };
    /**
     * 地图坐标转换。转换完成的点会以pointConvert事件回调。
     * @param seq 序列号
     * @param p 转换的点。
     */
    BaiduMapProvider.prototype.mapPointConvert = function (seq, p) {
        var _this = this;
        var points = this.parseJson(p);
        this.convertor.translate(points, 1, 5, function (o) {
            if (o.status == 0) {
                _this.on(MapEvents.pointConvert, { Seq: seq, Points: o.points });
            }
        });
    };
    /**
     * 显示临时报表。
     * @param d
     */
    BaiduMapProvider.prototype.mapShowTempReport = function (d) {
        var _this = this;
        var data = this.parseJson(d);
        this.tempSelectedData = data;
        if (data) {
            var reports = [];
            var time;
            var blocks = [];
            this.blockGrid.options.pollutants.forEach(function (pollutant) {
                var rp = new PollutantReport();
                rp.pollutant = pollutant;
                rp.count = 0;
                reports.push(rp);
            });
            data.forEach(function (d) {
                if (!time) {
                    time = d.time;
                }
                var lat = d.ActualLat;
                var lng = d.ActualLng;
                var block = _this.blockGrid.blocks.first(function (o) { return o.getBounds().containsPoint(new BMap.Point(lng, lat)); });
                if (block && !blocks.first(function (o) { return o == block; })) {
                    blocks.push(block);
                }
                reports.forEach(function (rp) {
                    var val = d[rp.pollutant.Name];
                    if (rp.count == 0) {
                        rp.avg = val;
                        rp.sum = val;
                        rp.min = val;
                        rp.max = val;
                    }
                    else {
                        rp.avg = (rp.avg * rp.count + val) / (rp.count + 1);
                        rp.sum += val;
                        rp.max = rp.max > val ? rp.max : val;
                        rp.min = rp.min < val ? rp.min : val;
                    }
                    rp.count++;
                });
            });
            this.onShowReport(this.getBlocksBounds(blocks), reports, time);
        }
    };
    BaiduMapProvider.prototype.mapClearTempReport = function () {
        delete this.tempSelectedData;
        this.map.closeInfoWindow(this.blockGrid.infoWindow);
        this.map.removeOverlay(this.blockGrid.infoWindow.targetBorder);
    };
    BaiduMapProvider.prototype.gridInit = function (opt) {
        opt = this.parseJson(opt);
        if (!opt)
            opt = new MapGridOptions();
        if (!opt.sideLength)
            opt.sideLength = 100; //默认边长100米，地图比例尺约1米约等于0.00001经纬度
        if (!opt.blockList)
            opt.blockList = []; //网格列表。
        //if (!opt.colorBegin) opt.colorBegin = "#FF0000";
        //if (!opt.colorEnd) opt.colorEnd = "#00FF00";
        if (!opt.opacity)
            opt.opacity = 0.5;
        if (!opt.pollutant)
            opt.pollutant = new Pollutant();
        if (!opt.pollutant.Name)
            opt.pollutant.Name = "sample";
        if (!opt.pollutant.MaxValue)
            opt.pollutant.MaxValue = 100;
        if (!opt.pollutant.MinValue)
            opt.pollutant.MinValue = 0;
        if (!opt.pollutants)
            opt.pollutants = [new Pollutant()]; //格式：{Name:"",DisplayName:"",MaxValue:0,MinValue:0,Unit:""}
        this.blockGrid.options = opt;
    };
    BaiduMapProvider.prototype.gridRefresh = function () {
        var _this = this;
        var points = [];
        var blockGrid = this.blockGrid;
        var opt = blockGrid.options;
        this.uavList.forEach(function (uav) { return points = points.concat(uav.pathPoint); });
        //填充点数据到格子里
        points.forEach(function (point) {
            if (!blockGrid.firstPoint)
                blockGrid.firstPoint = point;
            var block = blockGrid.blocks.first(function (block) { return block.getBounds().containsPoint(point); });
            if (!block) {
                block = _this.createBlock(point, opt);
                blockGrid.blocks.push(block);
                _this.map.addOverlay(block);
            }
            else {
                block.context.addPoint(point);
            }
        });
        blockGrid.blocks.forEach(function (block) {
            var report = block.context.getReports(function (o) { return o.pollutant.Name == opt.pollutant.Name; }).first(function (o) { return true; });
            if (report) {
                var color = _this.getColorByReport(report);
                if (block.context.color != color) {
                    block.context.color = color;
                    _this.on(MapEvents.blockChanged, { blocks: _this.getBlocksData(_this.blockGrid.blocks) });
                }
                block.setFillColor(block.context.color);
            }
        });
    };
    BaiduMapProvider.prototype.gridClear = function () {
        var _this = this;
        if (this.blockGrid.blocks) {
            this.blockGrid.blocks.forEach(function (o) { return _this.map.removeOverlay(o); });
        }
        this.onClearSelectedBlock();
        delete this.blockGrid.blocks;
        this.blockGrid.blocks = [];
    };
    BaiduMapProvider.prototype.uavAdd = function (name, lng, lat, d) {
        var _this = this;
        var data = this.parseJson(d);
        this.uav(name, null, function () {
            var point = new BMap.Point(lng, lat);
            point.data = data[0];
            var icon = new BMap.Icon("marker.png", new BMap.Size(30, 30));
            var uav = new Uav();
            uav.name = name;
            uav.marker = new BMap.Marker(point, { icon: icon });
            uav.pathPoint = data.select(function (o) {
                var p = new BMap.Point(o.ActualLng, o.ActualLat);
                p.data = o;
                return p;
            });
            _this.uavList.push(uav);
            _this.map.addOverlay(uav.marker);
            _this.on(MapEvents.uavChanged, { uav: _this.getUavData() });
        });
    };
    BaiduMapProvider.prototype.uavMove = function (name, lng, lat, d) {
        var _this = this;
        var data = this.parseJson(d);
        this.uav(name, function (o) {
            var point = new BMap.Point(lng, lat);
            point.data = data;
            o.pathPoint.push(point);
            o.marker.setPosition(point);
            _this.on(MapEvents.uavChanged, { uav: _this.getUavData() });
            if (_this.uavFollow) {
                _this.map.panTo(point);
            }
        }, null);
    };
    BaiduMapProvider.prototype.uavPathRefresh = function (name) {
        var _this = this;
        this.uav(name, function (o) {
            if (!o.pathMarker) {
                o.pathMarker = new BMap.Polyline([], {
                    enableClicking: false,
                    strokeWeight: 1,
                    strokeColor: "red",
                    strokeOpacity: 0.5
                });
            }
            _this.map.removeOverlay(o.pathMarker);
            if (_this.uavPath) {
                o.pathMarker.setPath(o.pathPoint);
                _this.map.addOverlay(o.pathMarker);
            }
        }, null);
    };
    BaiduMapProvider.prototype.uavRemove = function (name) {
        var _this = this;
        var i = -1;
        this.uavList.forEach(function (o, index) {
            if (o.name == name) {
                _this.map.removeOverlay(o.marker);
                _this.uavPath = false;
                _this.uavPathRefresh(name);
                _this.map.removeOverlay(o.pathMarker);
                i = index;
                delete o.marker;
                delete o.pathMarker;
                _this.on(MapEvents.uavChanged, { uav: _this.getUavData() });
            }
        });
        if (i != -1) {
            this.uavList.splice(i, 1);
        }
    };
    BaiduMapProvider.prototype.uavExist = function (name) {
        var obj = this.uavList.first(function (o) { return o.name == name; });
        return obj ? true : false;
    };
    BaiduMapProvider.prototype.uavFocus = function (name) {
        var _this = this;
        this.uav(name, function (o) {
            var point = o.marker.getPosition();
            if (_this.map.getZoom() == 19) {
                _this.map.panTo(point);
            }
            else {
                _this.map.centerAndZoom(point, 19);
            }
        }, null);
    };
    BaiduMapProvider.prototype.onSubscribe = function (eventName) {
        var result;
        switch (eventName) {
            case MapEvents.blockChanged:
                result = { blocks: this.getBlocksData(this.blockGrid.blocks) };
                break;
            case MapEvents.boundChanged:
                result = { bound: this.getMapBounds() };
                break;
            case MapEvents.uavChanged:
                result = { uav: this.getUavData() };
                break;
            default:
                break;
        }
        if (result) {
            return JSON.stringify(result);
        }
    };
    BaiduMapProvider.prototype.testGrid = function () {
        this.gridInit(new MapGridOptions());
        var p = this.map.getCenter();
        this.uavAdd("default", p.lng, p.lat, { sample: Math.random() * 100, time: (new Date).toLocaleDateString() });
        for (var i = 0; i < 20; i++) {
            if (i > 10) {
                this.uavMove("default", p.lng + (i / 10000), p.lat + ((i - 10) / 10000), { sample: Math.random() * 100, time: (new Date).toLocaleDateString() });
            }
            else {
                this.uavMove("default", p.lng + (i / 10000), p.lat, { sample: Math.random() * 100, time: (new Date).toLocaleDateString() });
            }
        }
        this.gridRefresh();
    };
    return BaiduMapProvider;
}(MapBase));
(function () {
    var map = new BaiduMapProvider();
    map.mapInit("container");
    window.map = map;
})();
//# sourceMappingURL=map.js.map