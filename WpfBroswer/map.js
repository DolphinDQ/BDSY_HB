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
        }
    };
    return MapBase;
}());
var MapEvents;
(function (MapEvents) {
    MapEvents["load"] = "load";
    MapEvents["pointConvert"] = "pointConvert";
    MapEvents["boundChanged"] = "boundChanged";
})(MapEvents || (MapEvents = {}));
var MapMenuItems;
(function (MapMenuItems) {
    MapMenuItems["compare"] = "\u5BF9\u6BD4\u6570\u636E";
    MapMenuItems["horizontal"] = "\u6A2A\u5411\u5207\u9762";
    MapMenuItems["vertical"] = "\u7EB5\u5411\u5207\u9762";
})(MapMenuItems || (MapMenuItems = {}));
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
        polygon.addEventListener("rightclick", function (o) {
            var index = null;
            var block = null;
            for (var i = 0; i < _this.blockGrid.selectedBlocks.length; i++) {
                block = _this.blockGrid.selectedBlocks[i];
                if (block == o.target) {
                    index = i;
                    break;
                }
            }
            if (index === null) {
                block = o.target;
                block.setStrokeColor("blue");
                block.setStrokeOpacity(1);
                block.setStrokeWeight(2);
                block.setStrokeStyle("dashed");
                _this.blockGrid.selectedBlocks.push(block);
            }
            else {
                block.setStrokeColor("white");
                block.setStrokeOpacity(0.5);
                block.setStrokeWeight(1);
                block.setStrokeStyle("solid");
                _this.blockGrid.selectedBlocks.splice(i, 1);
            }
        });
        return polygon;
    };
    BaiduMapProvider.prototype.isInBlock = function (center, sideLength, point) {
        //块中心点，块边长，当前点是否在块里面。
        var offset = sideLength / 2 * 0.00001; //计算偏移经纬度。
        return point.lng > (center.lng - offset) &&
            point.lng < (center.lng + offset) &&
            point.lat > (center.lat - offset) &&
            point.lat < (center.lat + offset);
    };
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
            //setEnable(MapMenuItems.compare, blocks.length > 0 && blocks.length <= 2);
            setEnable(MapMenuItems.horizontal, blocks.length > 0);
            setEnable(MapMenuItems.vertical, blocks.length > 0);
        }
    };
    //private onShowReport(): any {
    //    var blocks = this.blockGrid.selectedBlocks
    //    debugger;
    //    blocks.forEach(o => this.onShowBlockReport(o));
    //}
    BaiduMapProvider.prototype.onShowVerticalAspect = function () {
    };
    BaiduMapProvider.prototype.onShowHorizontalAspect = function () {
    };
    BaiduMapProvider.prototype.onShowBlockReport = function (block) {
        var _this = this;
        var blockGrid = this.blockGrid;
        var opt = blockGrid.options;
        if (!blockGrid.infoWindow) {
            blockGrid.infoWindow = new BMap.InfoWindow("", {
                width: 450,
                height: 300
            });
        }
        var content = '<div><span>实时采样数据：</span><span>({{time}})</span></div>';
        content = content.replace("{{time}}", block.context.time);
        content += this.getInfoWindowContentTemplate({
            title: "采样类型",
            min: "最小值",
            max: "最大值",
            avg: "平均值",
            unit: "单位",
            background: "white",
            opacity: 1,
        });
        var reports = block.context.getReports(function (o) { return true; });
        reports.forEach(function (o) { return content += _this.createInfoWindowContent(o); });
        blockGrid.infoWindow.setContent(content);
        this.map.openInfoWindow(blockGrid.infoWindow, block.context.center);
    };
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
                createItem(MapMenuItems.horizontal, function (o) { return _this.onShowHorizontalAspect(); }),
                createItem(MapMenuItems.vertical, function (o) { return _this.onShowVerticalAspect(); })
            ];
            _this.menuItems.forEach(function (o) { return menu.addItem(o); });
            menu.addEventListener("open", function (o) { return _this.onCheckContextMenu(); });
            map.addContextMenu(menu);
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
    BaiduMapProvider.prototype.mapBoundChangedEvent = function (subscribe) {
        var _this = this;
        this.callbackBoundChanged = subscribe;
        var mapBoundChangedEvents = ["moveend", "zoomend", "resize"];
        if (subscribe) {
            mapBoundChangedEvents.forEach(function (o) { return _this.map.addEventListener(o, _this.onMapBoundChaned); });
            this.onMapBoundChaned();
        }
        else {
            mapBoundChangedEvents.forEach(function (o) { return _this.map.removeEventListener(o, _this.onMapBoundChaned); });
        }
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
            var block = blockGrid.blocks.first(function (block) { return _this.isInBlock(block.context.center, opt.sideLength, point); });
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
        for (var i = 0; i < 10; i++) {
            this.uavMove("default", p.lng + (i / 10000), p.lat, { sample: Math.random() * 100, time: (new Date).toLocaleDateString() });
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