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
var Pollutant = /** @class */ (function () {
    function Pollutant() {
        this.Name = "sample";
        this.DisplayName = "样本";
        this.MaxValue = 100;
        this.MinValue = 1;
        this.Unit = "mg/m3";
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
var MapBase = /** @class */ (function () {
    function MapBase() {
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
            if (arg) {
                arg = JSON.stringify(arg);
            }
            window.external.On(eventName, arg);
        }
        catch (e) {
            //ignore;
            console.log("triger event [%s] arguments is :", eventName);
            console.dir(arg);
        }
    };
    return MapBase;
}());
var MapEvents;
(function (MapEvents) {
    MapEvents["load"] = "load";
    MapEvents["pointConvert"] = "pointConvert";
    MapEvents["boundChanged"] = "boundChanged";
    MapEvents["horizontalAspect"] = "horizontalAspect";
    MapEvents["verticalAspect"] = "verticalAspect";
    MapEvents["clearAspect"] = "clearAspect";
})(MapEvents || (MapEvents = {}));
var MapMenuItems;
(function (MapMenuItems) {
    MapMenuItems["compare"] = "\u5BF9\u6BD4\u6570\u636E";
    MapMenuItems["reports"] = "\u7EDF\u8BA1\u62A5\u8868";
    MapMenuItems["horizontal"] = "\u6A2A\u5411\u5207\u9762";
    MapMenuItems["vertical"] = "\u7EB5\u5411\u5207\u9762";
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
            if (o.domEvent.which == 3) {
                var selector = _this.selector;
                if (!selector) {
                    selector = _this.selector = new BMap.Polygon([], {
                        strokeColor: "blue",
                        strokeWeight: 1
                    });
                    selector.setFillColor("white");
                    selector.setFillOpacity(0.5);
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
            if (o.domEvent.which == 3 && selector && p1) {
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
var BaiduMapProvider = /** @class */ (function (_super) {
    __extends(BaiduMapProvider, _super);
    function BaiduMapProvider() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    BaiduMapProvider.prototype.getColor = function (value, min, max) {
        if (min === void 0) { min = undefined; }
        if (max === void 0) { max = undefined; }
        var opt = this.blockGrid.options;
        if (!min)
            min = opt.minValue;
        if (!max)
            max = opt.maxValue;
        var percent = (value - min) / (max - min);
        percent = percent > 1 ? 1 : percent;
        percent = percent < 0 ? 0 : percent;
        var br = parseInt(opt.colorBegin.substring(1, 3), 16);
        var bg = parseInt(opt.colorBegin.substring(3, 5), 16);
        var bb = parseInt(opt.colorBegin.substring(5, 7), 16);
        var er = parseInt(opt.colorEnd.substring(1, 3), 16);
        var eg = parseInt(opt.colorEnd.substring(3, 5), 16);
        var eb = parseInt(opt.colorEnd.substring(5, 7), 16);
        return "#" + this.getColorValue(percent, br, er) +
            this.getColorValue(percent, bg, eg) +
            this.getColorValue(percent, bb, eb);
    };
    BaiduMapProvider.prototype.getColorValue = function (percent, begin, end) {
        var flag = end > begin ? 1 : -1;
        var result = Math.round(Math.abs(end - begin) * percent * flag + begin).toString(16);
        return result.length < 2 ? "0" + result : result;
    };
    BaiduMapProvider.prototype.getInfoWindowContentTemplate = function (opt) {
        var template = '<div class="pollutant_message"><div class="pollutant_block" style="background:{{background}};opacity:{{opacity}}"></div><div class="pollutant_title">{{title}}</div><div class="pollutant_val">{{min}}</div><div class="pollutant_val">{{avg}}</div><div class="pollutant_val">{{max}}</div><div class="pollutant_unit">{{unit}}</div></div>';
        template = template.replace("{{title}}", opt.title);
        template = template.replace("{{min}}", opt.min);
        template = template.replace("{{max}}", opt.max);
        template = template.replace("{{avg}}", opt.avg);
        template = template.replace("{{unit}}", opt.unit);
        template = template.replace("{{background}}", opt.background);
        template = template.replace("{{opacity}}", opt.opacity);
        return template;
    };
    BaiduMapProvider.prototype.createInfoWindowContent = function (report) {
        return this.getInfoWindowContentTemplate({
            title: report.pollutant.DisplayName,
            min: Math.round(report.min * 100) / 100,
            max: Math.round(report.max * 100) / 100,
            avg: Math.round(report.avg * 100) / 100,
            unit: report.pollutant.Unit,
            background: this.getColor(report.avg, report.pollutant.MinValue, report.pollutant.MaxValue),
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
            strokeOpacity: 0.5,
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
            block.setStrokeOpacity(0.5);
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
    BaiduMapProvider.prototype.onMapBoundChaned = function () {
        if (this.callbackBoundChanged) {
            this.on(MapEvents.boundChanged, this.map.getBounds());
        }
    };
    BaiduMapProvider.prototype.onCheckContextMenu = function () {
        var _this = this;
        var blocks = this.blockGrid.selectedBlocks;
        if (!blocks) {
            this.menuItems.forEach(function (o) { return o.disable(); });
        }
        else {
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
            setEnable(MapMenuItems.reports, blocks.length > 0);
            setEnable(MapMenuItems.horizontal, blocks.length > 0);
            setEnable(MapMenuItems.vertical, blocks.length > 0);
        }
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
                blocks: blocks.select(function (o) {
                    return {
                        center: o.context.center,
                        points: o.context.getPoints(function (i) { return true; }).select(function (i) { return i.data; }),
                    };
                })
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
                blocks: blocks.select(function (o) {
                    return {
                        center: o.context.center,
                        points: o.context.getPoints(function (i) { return true; }).select(function (i) { return i.data; }),
                    };
                })
            });
        }
    };
    BaiduMapProvider.prototype.onShowSelectedBlockReport = function () {
        var blocks = this.blockGrid.selectedBlocks;
        if (blocks) {
            var minLat = blocks.min(function (o) { return o.context.center.lat; }).getBounds().getSouthWest().lat;
            var maxLat = blocks.max(function (o) { return o.context.center.lat; }).getBounds().getNorthEast().lat;
            var minLng = blocks.min(function (o) { return o.context.center.lng; }).getBounds().getSouthWest().lng;
            var maxLng = blocks.max(function (o) { return o.context.center.lng; }).getBounds().getNorthEast().lng;
            var bound = new BMap.Bounds(new BMap.Point(minLng, minLat), new BMap.Point(maxLng, maxLat));
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
                height: 300
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
        var content = '<div><span>实时采样数据：</span><span>({{time}})</span></div>';
        content = content.replace("{{time}}", time);
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
    /**
     * 初始化地图。
     * @param container 地图容器id
     */
    BaiduMapProvider.prototype.mapInit = function (container) {
        var _this = this;
        this.loadJs("http://api.map.baidu.com/getscript?v=2.0&ak=TCgR2Y0IGMmPR4qteh4McpXzMyYpFrEx", function (e) {
            // 百度地图API功能
            var map = new BMap.Map(container); // 创建Map实例
            _this.convertor = new BMap.Convertor();
            map.centerAndZoom(new BMap.Point(113.140761, 23.033974), 17); // 初始化地图,设置中心点坐标和地图级别
            //添加地图类型控件
            map.addControl(new BMap.MapTypeControl({
                mapTypes: [
                    BMAP_NORMAL_MAP,
                    BMAP_HYBRID_MAP
                ]
            }));
            map.addControl(new BMap.ScaleControl());
            map.addControl(new BMap.NavigationControl());
            map.addControl(new BMap.OverviewMapControl());
            //map.addControl(new BMap.GeolocationControl());
            map.enableScrollWheelZoom(true); //开启鼠标滚轮缩放
            var menu = new BMap.ContextMenu();
            var createItem = function (name, func) {
                var i = new BMap.MenuItem(name, func);
                i.name = name;
                return i;
            };
            _this.menuItems = [
                //createItem(MapMenuItems.compare, o => this.onShowReport()),
                createItem(MapMenuItems.reports, function (o) { return _this.onShowSelectedBlockReport(); }),
                createItem(MapMenuItems.horizontal, function (o) { return _this.onShowHorizontalAspect(); }),
                createItem(MapMenuItems.vertical, function (o) { return _this.onShowVerticalAspect(); }),
                createItem(MapMenuItems.clear, function (o) { return _this.onClearSelectedBlock(); })
            ];
            _this.menuItems.forEach(function (o) { return menu.addItem(o); });
            menu.addEventListener("open", function (o) { return _this.onCheckContextMenu(); });
            map.addContextMenu(menu);
            new BaiduMapSelector(map, function (o) {
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
            });
            _this.map = map;
            _this.blockGrid = new MapGrid();
            _this.blockGrid.blocks = new Array();
            _this.uavList = new Array();
            _this.on(MapEvents.load);
        });
    };
    BaiduMapProvider.prototype.mapPointConvert = function (seq, p) {
        var _this = this;
        var points = this.parseJson(p);
        this.convertor.translate(points, 1, 5, function (o) {
            if (o.status == 0) {
                _this.on(MapEvents.pointConvert, { Seq: seq, Points: o.points });
            }
        });
    };
    BaiduMapProvider.prototype.gridInit = function (opt) {
        opt = this.parseJson(opt);
        if (!opt)
            opt = new MapGridOptions();
        if (!opt.sideLength)
            opt.sideLength = 100; //默认边长100米，地图比例尺约1米约等于0.00001经纬度
        if (!opt.blockList)
            opt.blockList = []; //网格列表。
        if (!opt.colorBegin)
            opt.colorBegin = "#FF0000";
        if (!opt.colorEnd)
            opt.colorEnd = "#00FF00";
        if (!opt.opacity)
            opt.opacity = 0.5;
        if (!opt.dataName)
            opt.dataName = "sample";
        if (!opt.maxValue)
            opt.maxValue = 100;
        if (!opt.minValue)
            opt.minValue = 0;
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
            var report = block.context.getReports(function (o) { return o.pollutant.Name == opt.dataName; }).first(function (o) { return true; });
            if (report) {
                block.setFillColor(_this.getColor(report.avg));
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
            point.data = data;
            var icon = new BMap.Icon("marker.png", new BMap.Size(30, 30));
            var uav = new Uav();
            uav.name = name;
            uav.marker = new BMap.Marker(point, { icon: icon });
            uav.pathPoint = [point];
            _this.uavList.push(uav);
            _this.map.addOverlay(uav.marker);
        });
    };
    BaiduMapProvider.prototype.uavMove = function (name, lng, lat, d) {
        var data = this.parseJson(d);
        this.uav(name, function (o) {
            var point = new BMap.Point(lng, lat);
            point.data = data;
            o.pathPoint.push(point);
            o.marker.setPosition(point);
        }, null);
    };
    BaiduMapProvider.prototype.uavShowPath = function (name) {
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
            else {
                _this.map.removeOverlay(o.pathMarker);
            }
            o.pathMarker.setPath(o.pathPoint);
            _this.map.addOverlay(o.pathMarker);
            o.pathMarker.show();
        }, null);
    };
    BaiduMapProvider.prototype.uavHidePath = function (name) {
        this.uav(name, function (o) {
            if (o.pathMarker) {
                o.pathMarker.hide();
            }
        }, null);
    };
    BaiduMapProvider.prototype.uavRemove = function (name) {
        var _this = this;
        var i = -1;
        this.uavList.forEach(function (o, index) {
            if (o.name == name) {
                _this.map.removeOverlay(o.marker);
                _this.uavHidePath(name);
                _this.map.removeOverlay(o.pathMarker);
                i = index;
                delete o.marker;
                delete o.pathMarker;
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