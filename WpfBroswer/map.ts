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
interface IMapProvider {
    mapInit(container: string);
    mapPointConvert(seq: number, p: Point[]);
    mapBoundChangedEvent(subscribe: boolean);
    gridInit(opt: MapGridOptions);
    gridRefresh();
    gridClear();
    uavAdd(name: string, lng: number, lat: number, d: any);
    uavMove(name: string, lng: number, lat: number, d: any);
    uavShowPath(name: string);
    uavHidePath(name: string);
    uavRemove(name: string);
    uavExist(name: string): boolean;
    uavFocus(name: string);
}
interface MenuItem {
    setText(text: String);
    enable();
    disable();
    name: MapMenuItems;
}
interface ContextMenu {
    addItem(item: MenuItem);
    getItem(index: Number): MenuItem
    removeItem(item: MenuItem);
    addEventListener(name, call);
}

interface Point {
    lat: number;
    lng: number;
    data: any;//采样数据
}
interface Bound {
    getSouthWest(): Point;
    getNorthEast(): Point;
    containsPoint(point: Point): boolean;
}

interface InfoWindow {
    setContent(text: string);
    setWidth(width: Number);
    setHeight(height: Number);
}
//方块
interface Block {
    context: BlockContext;
    getBounds(): Bound;
    setFillColor(color: String);
    setStrokeColor(color: String);
    setStrokeStyle(style: String);
    setStrokeWeight(weight: Number);
    setStrokeOpacity(opacity: Number);
}


class BlockContext {
    constructor(center: Point, pollutants: Pollutant[]) {
        this.center = center;
        pollutants.forEach(o => {
            var report = new PollutantReport();
            report.pollutant = o;
            this.reports.push(report);
        })
    }
    readonly center: Point;
    time: string;
    private points: Point[] = [];
    private reports: PollutantReport[] = [];
    addPoint(p: Point) {
        if (this.points.first(o => o == p)) return;
        if (p.data) {
            if (!this.time) {
                this.time = p.data["time"];
            }
            this.reports.forEach(o => {
                var val = p.data[o.pollutant.Name];
                if (val) {
                    o.avg = (o.avg * o.count + val) / (o.count + 1);
                    if (o.count == 0) {
                        o.max = val;
                        o.min = val;
                        o.sum = val;
                    } else {
                        o.max = val > o.max ? val : o.max;
                        o.min = val < o.min ? val : o.min;
                        o.sum += val;
                    }
                    o.count++;
                }
            });
        }
        this.points.push(p);
    }
    getPoints(query: (o: Point) => boolean): Array<Point> {
        return this.points.filter(query);
    }
    getReports(query: (o: PollutantReport) => boolean): Array<PollutantReport> {
        return this.reports.filter(query);
    }
}

class PollutantReport {
    pollutant: Pollutant;//污染物;
    count: number = 0;
    avg: number = 0;
    sum: number = 0;
    max: number = 0;
    min: number = 0;
}

class Pollutant {
    Name: string = "sample";
    DisplayName: string = "样本";
    MaxValue: number = 100;
    MinValue: number = 1;
    Unit: string = "mg/m3";
}

class MapGridOptions {
    sideLength: number;
    blockList: any[];
    colorBegin: string;
    colorEnd: string;
    opacity: number;
    dataName: string;
    maxValue: number;
    minValue: number;
    pollutants: Pollutant[];
}

class MapGrid {
    options: MapGridOptions;
    blocks: Block[];
    firstPoint: Point;
    infoWindow: InfoWindow;
    selectedBlocks: Block[] = [];
    selectedBlockLine: any[] = [];
}

class Uav {
    pathPoint: Point[];
    name: string;
    marker: any;
    pathMarker: any;
}

abstract class MapBase implements IMapProvider {
    abstract mapBoundChangedEvent(subscribe: boolean);
    abstract mapPointConvert(seq: number, p: Point[]);
    abstract uavShowPath(name: string);
    abstract uavHidePath(name: string);
    abstract uavRemove(name: string);
    abstract uavExist(name: string): boolean;
    abstract uavFocus(name: string);
    abstract gridClear();
    abstract uavAdd(name: string, lng: number, lat: number, d: any);
    abstract uavMove(name: string, lng: number, lat: number, d: any);
    abstract gridInit(opt: MapGridOptions);
    abstract mapInit(container: string);
    abstract gridRefresh();
    protected loadJs(url: string, onLoad: (e) => any) {
        try {
            var file = document.createElement("script");
            file.setAttribute("type", "text/javascript");
            file.setAttribute("src", url);
            file.onload = onLoad;
            document.getElementsByTagName("head")[0].appendChild(file);
        } catch (e) {
            alert(e);
        }
    }
    protected parseJson<T>(obj: T): T {
        if (typeof (obj) == "string")
            obj = JSON.parse(obj)
        return obj;
    }
    protected on(eventName: MapEvents, arg?: any) {
        try {
            if (arg) {
                arg = JSON.stringify(arg);
            }
            window.external.On(eventName, arg);
        } catch (e) {
            //ignore;
            console.log("triger event [%s] arguments is :", eventName);
            console.dir(arg);
        }
    }
}

enum MapEvents {
    load = "load",
    pointConvert = "pointConvert",
    boundChanged = "boundChanged",
    horizontalAspect = "horizontalAspect",
    verticalAspect = "verticalAspect",
}

enum MapMenuItems {
    compare = "对比数据",
    horizontal = "横向切面",
    vertical = "纵向切面",
    clear = "清除",
}

declare var BMap;
declare var BMAP_NORMAL_MAP;
declare var BMAP_HYBRID_MAP;
class BaiduMapProvider extends MapBase {

    private callbackBoundChanged
    private map: any;
    private menuItems: MenuItem[];
    private convertor: any;
    private blockGrid: MapGrid;
    private uavList: Uav[];
    private getColor(value: number, min: number = undefined, max: number = undefined): string {
        var opt = this.blockGrid.options;
        if (!min) min = opt.minValue
        if (!max) max = opt.maxValue

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
    }
    private getColorValue(percent: number, begin: number, end: number) {
        var flag = end > begin ? 1 : -1;
        var result = Math.round(Math.abs(end - begin) * percent * flag + begin).toString(16);
        return result.length < 2 ? "0" + result : result;
    }
    private getInfoWindowContentTemplate(opt) {
        var template = '<div class="pollutant_message"><div class="pollutant_block" style="background:{{background}};opacity:{{opacity}}"></div><div class="pollutant_title">{{title}}</div><div class="pollutant_val">{{min}}</div><div class="pollutant_val">{{avg}}</div><div class="pollutant_val">{{max}}</div><div class="pollutant_unit">{{unit}}</div></div>';
        template = template.replace("{{title}}", opt.title);
        template = template.replace("{{min}}", opt.min);
        template = template.replace("{{max}}", opt.max);
        template = template.replace("{{avg}}", opt.avg);
        template = template.replace("{{unit}}", opt.unit);
        template = template.replace("{{background}}", opt.background);
        template = template.replace("{{opacity}}", opt.opacity);
        return template;
    }
    private createInfoWindowContent(report: PollutantReport) {
        return this.getInfoWindowContentTemplate({
            title: report.pollutant.DisplayName,
            min: Math.round(report.min * 100) / 100,
            max: Math.round(report.max * 100) / 100,
            avg: Math.round(report.avg * 100) / 100,
            unit: report.pollutant.Unit,
            background: this.getColor(report.avg, report.pollutant.MinValue, report.pollutant.MaxValue),
            opacity: this.blockGrid.options.opacity,
        });
    }

    private createBlock(point: Point, opt: MapGridOptions): Block {
        var center = this.blockGrid.firstPoint;
        var sideLength = opt.sideLength * 0.00001;
        var opacity = opt.opacity;
        var offset = sideLength / 2;//计算偏移经纬度。
        var lng = center.lng + sideLength * Math.round((point.lng - center.lng) / sideLength);
        var lat = center.lat + sideLength * Math.round((point.lat - center.lat) / sideLength)
        var polygon = new BMap.Polygon(
            [
                new BMap.Point(lng + offset, lat + offset),
                new BMap.Point(lng - offset, lat + offset),
                new BMap.Point(lng - offset, lat - offset),
                new BMap.Point(lng + offset, lat - offset),
            ],
            {
                fillOpacity: opacity,
                strokeWeight: 1,
                strokeOpacity: 0.5,
                strokeColor: "white"
            }
        );
        var context = new BlockContext(new BMap.Point(lng, lat), opt.pollutants);
        context.addPoint(point);
        polygon.context = context;
        polygon.addEventListener("click", o => this.onShowBlockReport(o.target));
        polygon.addEventListener("rightclick", o => this.onSelectBlock(o.target))
        return polygon;
    }

    private onSelectBlock(b: Block) {
        var index = null;
        var block = null;
        for (var i = 0; i < this.blockGrid.selectedBlocks.length; i++) {
            block = this.blockGrid.selectedBlocks[i]
            if (block == b) {
                index = i;
                break;
            }
        }
        if (index === null) {
            block = b;
            block.setStrokeColor("blue");
            block.setStrokeOpacity(1);
            block.setStrokeWeight(2);
            block.setStrokeStyle("dashed");
            this.blockGrid.selectedBlocks.push(block);
        } else {
            block.setStrokeColor("white");
            block.setStrokeOpacity(0.5);
            block.setStrokeWeight(1);
            block.setStrokeStyle("solid");
            this.blockGrid.selectedBlocks.splice(i, 1);
        }
    }

    private isInBlock(center: Point, sideLength: number, point: Point) {
        //块中心点，块边长，当前点是否在块里面。
        var offset = sideLength / 2 * 0.00001;//计算偏移经纬度。
        return point.lng > (center.lng - offset) &&
            point.lng < (center.lng + offset) &&
            point.lat > (center.lat - offset) &&
            point.lat < (center.lat + offset);
    }
    private uav(name: string, exist: (o: Uav) => any, notExist: () => any) {
        try {
            var uav = this.uavList.first(o => o.name == name);
            if (uav) {
                if (exist) {
                    exist(uav);
                } else {
                    console.log("uav named :%s already existed", name)
                }
            } else {
                if (notExist) {
                    notExist();
                } else {
                    console.log("uav named :%s not found", name)
                }
            }
        } catch (e) {
            // alert(e.message);
        }
    }
    private onMapBoundChaned() {
        if (this.callbackBoundChanged) {
            this.on(MapEvents.boundChanged, this.map.getBounds())
        }
    }
    private onCheckContextMenu() {
        var blocks = this.blockGrid.selectedBlocks
        if (!blocks) {
            this.menuItems.forEach(o => o.disable());
        } else {
            var setEnable = (name: MapMenuItems, enable) => {
                var i = this.menuItems.first(o => o.name == name);
                if (i) {
                    if (enable) {
                        i.enable();
                    } else {
                        i.disable();
                    }
                }
            }
            setEnable(MapMenuItems.horizontal, blocks.length > 0);
            setEnable(MapMenuItems.vertical, blocks.length > 0);
        }
    }

    private addLine(point: Point, horizontalLen: number, verticalLen: number) {
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
    }
    private onShowVerticalAspect(): any {
        var blocks = this.blockGrid.selectedBlocks;
        if (blocks) {
            var min = blocks.min(o => o.context.center.lng);
            var max = blocks.max(o => o.context.center.lng);
            this.blockGrid.selectedBlockLine.forEach(o => this.map.removeOverlay(o));
            this.addLine(min.getBounds().getSouthWest(), 0, 10000);
            this.addLine(max.getBounds().getNorthEast(), 0, 10000);
            this.on(MapEvents.verticalAspect, blocks.selectMany(o => o.context.getPoints(i => true).select(c => c.data)))
        }
    }
    private onClearSelectedBlock() {
        this.blockGrid.selectedBlockLine.forEach(o => this.map.removeOverlay(o));
        this.blockGrid.selectedBlocks.filter(o => true).forEach(o => this.onSelectBlock(o));
    }
    private onShowHorizontalAspect(): any {
        var blocks = this.blockGrid.selectedBlocks;
        if (blocks) {
            var min = blocks.min(o => o.context.center.lng);
            var max = blocks.max(o => o.context.center.lng);
            this.blockGrid.selectedBlockLine.forEach(o => this.map.removeOverlay(o));
            this.addLine(min.getBounds().getSouthWest(), 10000, 0);
            this.addLine(max.getBounds().getNorthEast(), 10000, 0);
            this.on(MapEvents.horizontalAspect, blocks.selectMany(o => o.context.getPoints(i => true).select(c => c.data)))
        }
    }
    private onShowBlockReport(block: Block) {
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
        var reports = block.context.getReports(o => true);
        reports.forEach(o => content += this.createInfoWindowContent(o));
        blockGrid.infoWindow.setContent(content);
        this.map.openInfoWindow(blockGrid.infoWindow, block.context.center)
    }

    mapInit(container: string) {
        this.loadJs("http://api.map.baidu.com/getscript?v=2.0&ak=TCgR2Y0IGMmPR4qteh4McpXzMyYpFrEx", e => {
            // 百度地图API功能
            let map = new BMap.Map(container);    // 创建Map实例
            this.convertor = new BMap.Convertor();
            map.centerAndZoom(new BMap.Point(113.140761, 23.033974), 17);  // 初始化地图,设置中心点坐标和地图级别
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
            map.enableScrollWheelZoom(true);     //开启鼠标滚轮缩放
            var menu: ContextMenu = new BMap.ContextMenu();
            var createItem = (name: MapMenuItems, func) => {
                var i = new BMap.MenuItem(name, func);
                i.name = name;
                return i;
            };
            this.menuItems = [
                //createItem(MapMenuItems.compare, o => this.onShowReport()),
                createItem(MapMenuItems.horizontal, o => this.onShowHorizontalAspect()),
                createItem(MapMenuItems.vertical, o => this.onShowVerticalAspect()),
                createItem(MapMenuItems.clear, o => this.onClearSelectedBlock())
            ];
            this.menuItems.forEach(o => menu.addItem(o));
            menu.addEventListener("open", o => this.onCheckContextMenu());
            map.addContextMenu(menu);
            this.map = map;
            this.blockGrid = new MapGrid();
            this.blockGrid.blocks = new Array<any>();
            this.uavList = new Array<Uav>();
            this.on(MapEvents.load);
        });
    }
    mapPointConvert(seq: number, p: Point[]) {
        var points = this.parseJson(p);
        this.convertor.translate(points, 1, 5, o => {
            if (o.status == 0) {
                this.on(MapEvents.pointConvert, { Seq: seq, Points: o.points })
            }
        });
    }
    mapBoundChangedEvent(subscribe: boolean) {
        this.callbackBoundChanged = subscribe;
        var mapBoundChangedEvents = ["moveend", "zoomend", "resize"];
        if (subscribe) {
            mapBoundChangedEvents.forEach(o => this.map.addEventListener(o, this.onMapBoundChaned));
            this.onMapBoundChaned();
        } else {
            mapBoundChangedEvents.forEach(o => this.map.removeEventListener(o, this.onMapBoundChaned));
        }
    }
    gridInit(opt: MapGridOptions) {
        opt = this.parseJson(opt);
        if (!opt) opt = new MapGridOptions();
        if (!opt.sideLength) opt.sideLength = 100;//默认边长100米，地图比例尺约1米约等于0.00001经纬度
        if (!opt.blockList) opt.blockList = [];//网格列表。
        if (!opt.colorBegin) opt.colorBegin = "#FF0000";
        if (!opt.colorEnd) opt.colorEnd = "#00FF00";
        if (!opt.opacity) opt.opacity = 0.5;
        if (!opt.dataName) opt.dataName = "sample";
        if (!opt.maxValue) opt.maxValue = 100;
        if (!opt.minValue) opt.minValue = 0;
        if (!opt.pollutants) opt.pollutants = [new Pollutant()];//格式：{Name:"",DisplayName:"",MaxValue:0,MinValue:0,Unit:""}
        this.blockGrid.options = opt;
    }
    gridRefresh() {
        var points: Point[] = [];
        var blockGrid = this.blockGrid;
        var opt = blockGrid.options;
        this.uavList.forEach(uav => points = points.concat(uav.pathPoint));

        //填充点数据到格子里
        points.forEach(point => {
            if (!blockGrid.firstPoint) blockGrid.firstPoint = point;
            var block = blockGrid.blocks.first(block => block.getBounds().containsPoint(point));
            if (!block) {
                block = this.createBlock(point, opt);
                blockGrid.blocks.push(block);
                this.map.addOverlay(block);
            } else {
                block.context.addPoint(point);
            }
        });
        blockGrid.blocks.forEach(block => {
            var report = block.context.getReports(o => o.pollutant.Name == opt.dataName).first(o => true);
            if (report) {
                block.setFillColor(this.getColor(report.avg));
            }
        });
    }
    gridClear() {
        if (this.blockGrid.blocks) {
            this.blockGrid.blocks.forEach(o => this.map.removeOverlay(o));
        }
        delete this.blockGrid.blocks;
        this.blockGrid.blocks = [];
    }
    uavAdd(name: string, lng: number, lat: number, d: any) {
        var data = this.parseJson(d);
        this.uav(name, null, () => {
            var point = new BMap.Point(lng, lat);
            point.data = data;
            var icon = new BMap.Icon("marker.png", new BMap.Size(30, 30));
            var uav = new Uav();
            uav.name = name;
            uav.marker = new BMap.Marker(point, { icon: icon });
            uav.pathPoint = [point];
            this.uavList.push(uav);
            this.map.addOverlay(uav.marker);
        });
    }
    uavMove(name: string, lng: number, lat: number, d: any) {
        var data = this.parseJson(d);
        this.uav(name, o => {
            var point = new BMap.Point(lng, lat);
            point.data = data;
            o.pathPoint.push(point);
            o.marker.setPosition(point);
        }, null);
    }
    uavShowPath(name: string) {
        this.uav(name, o => {
            if (!o.pathMarker) {
                o.pathMarker = new BMap.Polyline([], {
                    enableClicking: false,
                    strokeWeight: 1,
                    strokeColor: "red",
                    strokeOpacity: 0.5
                });
            } else {
                this.map.removeOverlay(o.pathMarker);
            }
            o.pathMarker.setPath(o.pathPoint);
            this.map.addOverlay(o.pathMarker);
            o.pathMarker.show();
        }, null);
    }
    uavHidePath(name: string) {
        this.uav(name, o => {
            if (o.pathMarker) {
                o.pathMarker.hide();
            }
        }, null);
    }
    uavRemove(name: string) {
        var i = -1;
        this.uavList.forEach((o, index) => {
            if (o.name == name) {
                this.map.removeOverlay(o.marker);
                this.uavHidePath(name);
                this.map.removeOverlay(o.pathMarker);
                i = index;
                delete o.marker;
                delete o.pathMarker;
            }
        });
        if (i != -1) {
            this.uavList.splice(i, 1);
        }
    }
    uavExist(name: string): boolean {
        var obj = this.uavList.first((o) => o.name == name);
        return obj ? true : false;
    }
    uavFocus(name: string) {
        this.uav(name, o => {
            var point = o.marker.getPosition();
            if (this.map.getZoom() == 19) {
                this.map.panTo(point);
            } else {
                this.map.centerAndZoom(point, 19);
            }
        }, null);
    }
    testGrid() {
        this.gridInit(new MapGridOptions());
        var p = this.map.getCenter();
        this.uavAdd("default", p.lng, p.lat, { sample: Math.random() * 100, time: (new Date).toLocaleDateString() })
        for (var i = 0; i < 20; i++) {
            if (i > 10) {
                this.uavMove("default", p.lng + (i / 10000), p.lat + ((i - 10) / 10000), { sample: Math.random() * 100, time: (new Date).toLocaleDateString() })
            } else {
                this.uavMove("default", p.lng + (i / 10000), p.lat, { sample: Math.random() * 100, time: (new Date).toLocaleDateString() })
            }
        }
        this.gridRefresh();
    }
}

(function () {
    let map = new BaiduMapProvider();
    map.mapInit("container");
    (<any>window).map = map;
})();