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
    color: string;
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

class PollutantLevel {
    Name: string;
    MinValue: number;
    MaxValue: number;
    MaxColor: string;
    MinColor: string;
}

class Pollutant {
    Name: string = "sample";
    DisplayName: string = "样本";
    MaxValue: number = 100;
    MinValue: number = 1;
    Unit: string = "mg/m3";
    Levels: PollutantLevel[] = [];
}

class MapGridOptions {
    sideLength: number;
    blockList: any[];
    //colorBegin: string;
    //colorEnd: string;
    opacity: number;
    //dataName: string;
    //maxValue: number;
    //minValue: number;
    pollutants: Pollutant[];
    pollutant: Pollutant;
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

class EventSubscribe {
    name: MapEvents;
    enable: boolean;
}

abstract class MapBase implements IMapProvider, IEventAggregator {

    abstract mapClearTempReport();
    abstract mapShowTempReport(d: any);
    abstract mapPointConvert(seq: number, p: Point[]);
    abstract uavPathRefresh(name: string);
    abstract uavRemove(name: string);
    abstract uavExist(name: string): boolean;
    abstract uavFocus(name: string);
    abstract gridClear();
    abstract uavAdd(name: string, lng: number, lat: number, d: any);
    abstract uavMove(name: string, lng: number, lat: number, d: any);
    abstract gridInit(opt: MapGridOptions);
    abstract mapInit(container: string);
    abstract mapInitMenu(edit: boolean);
    abstract gridRefresh();
    abstract onSubscribe(eventName: MapEvents);
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
    protected m_events: Array<EventSubscribe> = [];
    on(eventName: MapEvents, arg?: any) {
        try {
            var sub = this.m_events.first(o => o.name == eventName);
            if (sub && sub.enable) {
                if (arg) {
                    arg = JSON.stringify(arg);
                }
                window.external.On(eventName, arg);
                return;
            }
        } catch (e) {
            //ignore;
        }
        console.log("triger event [%s] arguments is :", eventName);
        console.dir(arg);
    }
    subscribe(eventName: MapEvents, enable: boolean) {
        var evt = this.m_events.first(o => o.name == eventName)
        if (evt) {
            evt.enable = enable;
        } else {
            this.m_events.push(<EventSubscribe>{ name: eventName, enable: enable });
        }
        if (enable) {
            return this.onSubscribe(eventName)
        }
    }
}

/**
 *百度地图选择器。用于界面元素框选。 
 */
class BaiduMapSelector {
    private map;
    private callback;
    private selector;
    private pointOne: Point;
    private enable: boolean = true;
    /**
     * 创建百度地图选择器，默认使用右键进行框选。
     * @param map 百度地图对象。
     * @param callbackFn 选择后回调框选区域。
     */
    constructor(map, callbackFn: (b) => any) {
        this.map = map;
        this.callback = callbackFn;
        map.addEventListener("mousedown", o => {
            if (!this.enable) return;
            if (o.domEvent.which == 3 || o.domEvent.button == 2) {
                var selector = this.selector;
                if (!selector) {
                    selector = this.selector = new BMap.Polygon([], {
                        strokeColor: "blue",
                        strokeWeight: 1,
                        fillColor: "blue",
                        fillOpacity: 0.1,
                    });
                }
                var point: Point = this.pointOne = o.point;
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
        map.addEventListener("mousemove", o => {
            var selector = this.selector;
            var p1 = this.pointOne;
            if (selector && p1) {
                var p2: Point = o.point;
                selector.setPath([
                    new BMap.Point(p1.lng, p1.lat),
                    new BMap.Point(p1.lng, p2.lat),
                    new BMap.Point(p2.lng, p2.lat),
                    new BMap.Point(p2.lng, p1.lat),
                ]);
            }
        });
        map.addEventListener("mouseup", o => {
            var selector = this.selector;
            if (selector) {
                if (this.callback) {
                    this.callback({ bound: selector.getBounds(), event: o });
                }
                this.map.removeOverlay(selector);
                delete this.pointOne;
                delete this.selector;
            }
        });
    }

    setEnable(enable: boolean) {
        this.enable = enable;
    }

    getEnable() { return this.enable; }
}
/**数据分析区域。 */
class BaiduMapAnalysisArea {
    private evt: IEventAggregator;
    private map: any;
    private selectingArea: boolean;
    private border: any;
    private bound: Bound;

    constructor(map, evt: IEventAggregator) {
        this.map = map;
        this.evt = evt;
    }

    isEnabled(): boolean {
        return this.selectingArea || this.border;
    }

    setBounds(bound: Bound) {
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
    }

    getBounds(): Bound {
        return this.bound;
    }

    enable() {
        this.disable();
        this.selectingArea = true;
    }

    disable() {
        this.selectingArea = false;
        if (this.border) {
            this.map.removeOverlay(this.border);
            delete this.bound;
            delete this.border;
            this.evt.on(MapEvents.clearAnalysisArea);
        }
    }
}


enum MapEvents {
    load = "load",
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
}

enum MapMenuItems {
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
/**地图方块选择动作 */
enum MapBlockSelectAction {
    //开关
    switch,
    //强制选择
    focusSelect,
    //强制反选
    focusUnselect,
}

class BaiduMapProvider extends MapBase {

    private map: any;
    private menuItems: MenuItem[];
    private convertor: any;
    private blockGrid: MapGrid;
    private uavList: Uav[];
    private uavFollow: boolean = true;
    private uavPath: boolean = false;
    private loading: boolean;
    private analysisArea: BaiduMapAnalysisArea;
    private tempSelectedData: Array<any>;    //临时选中数据。
    private getColor(value: number, min: number = undefined, max: number = undefined, minColor: string = undefined, maxColor: string = undefined): string {
        var opt = this.blockGrid.options;
        if (!min) min = opt.pollutant.MinValue
        if (!max) max = opt.pollutant.MaxValue
        if (!maxColor) maxColor = "#ff0000";
        if (!minColor) minColor = "#00ff00";
        var percent = (value - min) / (max - min);
        percent = percent > 1 ? 1 : percent;
        percent = percent < 0 ? 0 : percent;

        var br = parseInt(minColor.substring(1, 3), 16);
        var bg = parseInt(minColor.substring(3, 5), 16);
        var bb = parseInt(minColor.substring(5, 7), 16);

        var er = parseInt(maxColor.substring(1, 3), 16);
        var eg = parseInt(maxColor.substring(3, 5), 16);
        var eb = parseInt(maxColor.substring(5, 7), 16);
        var getColorValue = function (percent: number, begin: number, end: number) {
            var flag = end > begin ? 1 : -1;
            var result = Math.round(Math.abs(end - begin) * percent * flag + begin).toString(16);
            return result.length < 2 ? "0" + result : result;
        }

        return "#" + getColorValue(percent, br, er) +
            getColorValue(percent, bg, eg) +
            getColorValue(percent, bb, eb);
    }
    private getColorByReport(report: PollutantReport): string {
        var lv = report.pollutant.Levels.first(o => o.MaxValue >= report.avg && o.MinValue <= report.avg);
        if (!lv) {
            if (report.avg > report.pollutant.MaxValue) {
                lv = report.pollutant.Levels.max(o => o.MaxValue);
            }
            if (report.avg < report.pollutant.MinValue) {
                lv = report.pollutant.Levels.min(o => o.MinValue);
            }
        }
        if (!lv) return "#ffffff"
        return this.getColor(report.avg, lv.MinValue, lv.MaxValue, lv.MinColor, lv.MaxColor);
    }
    private getInfoWindowContentTemplate(opt) {
        var template = '<div class="pollutant_message"><div class="pollutant_block" style="background:{{background}};opacity:{{opacity}}"></div><div class="pollutant_title">{{title}}</div><div class="pollutant_val">{{min}}</div><div class="pollutant_val">{{avg}}</div><div class="pollutant_val">{{max}}</div><div class="pollutant_unit">{{unit}}</div></div>';
        return template
            .replace("{{title}}", opt.title)
            .replace("{{min}}", opt.min)
            .replace("{{max}}", opt.max)
            .replace("{{avg}}", opt.avg)
            .replace("{{unit}}", opt.unit)
            .replace("{{background}}", opt.background)
            .replace("{{opacity}}", opt.opacity);
    }
    private createInfoWindowContent(report: PollutantReport) {
        return this.getInfoWindowContentTemplate({
            title: report.pollutant.DisplayName,
            min: Math.round(report.min * 100) / 100,
            max: Math.round(report.max * 100) / 100,
            avg: Math.round(report.avg * 100) / 100,
            unit: report.pollutant.Unit,
            background: this.getColorByReport(report),
            opacity: this.blockGrid.options.opacity,
        });
    }
    /**
     * 创建方块。
     * @param point 中心点。
     * @param opt 选项。
     */
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
                strokeOpacity: 0.2,
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
    /**
     * 选中方块。
     * @param b 方块对象。
     * @param act 强制选中或者不选中。
     */
    private onSelectBlock(b: Block, act: MapBlockSelectAction = MapBlockSelectAction.switch) {
        var index = null;
        var block: Block = null;
        for (var i = 0; i < this.blockGrid.selectedBlocks.length; i++) {
            block = this.blockGrid.selectedBlocks[i]
            if (block == b) {
                index = i;
                break;
            }
        }
        if (index === null) {
            if (act == MapBlockSelectAction.focusUnselect) return;
            block = b;
            block.setStrokeColor("red");
            block.setStrokeOpacity(1);
            block.setStrokeWeight(1);
            block.setStrokeStyle("solid");
            this.blockGrid.selectedBlocks.push(block);
        } else {
            if (act == MapBlockSelectAction.focusSelect) return;
            block.setStrokeColor("white");
            block.setStrokeOpacity(0.2);
            block.setStrokeWeight(1);
            block.setStrokeStyle("solid");
            this.blockGrid.selectedBlocks.splice(i, 1);
        }
    }
    /**
     * 查找当前加载的无人机。
     * @param name 无人机名称、标识
     * @param exist 如果存在执行操作。
     * @param notExist 如果不存在执行操作。
     */
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
    /** 地图显示发生变更。 */
    private onMapBoundChanged(): any {
        var bound: Bound = this.map.getBounds();
        this.on(MapEvents.boundChanged, { bound: { sw: bound.getSouthWest(), ne: bound.getNorthEast() } });
    }
    private onCheckContextMenu() {
        var blocks = this.blockGrid.selectedBlocks
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
        var setChecked = (name: MapMenuItems, checked) => {
            var i = this.menuItems.first(o => o.name == name);
            if (i) {
                if (checked) {
                    i.setText(name + " √");
                } else {
                    i.setText(name);
                }
            }
        }
        if (!blocks) {
            this.menuItems.forEach(o => { if (o) o.disable() });
        } else {
            setEnable(MapMenuItems.savePoints, blocks.length > 0);
            setEnable(MapMenuItems.reports, blocks.length > 0);
            setEnable(MapMenuItems.horizontal, blocks.length > 0);
            setEnable(MapMenuItems.vertical, blocks.length > 0);
            setEnable(MapMenuItems.clear, blocks.length > 0);
        }
        setEnable(MapMenuItems.selectAnalysisArea, !this.analysisArea.isEnabled())
        setEnable(MapMenuItems.clearAnalysisArea, this.analysisArea.isEnabled())
        setChecked(MapMenuItems.uavFollow, this.uavFollow);
        setChecked(MapMenuItems.uavPath, this.uavPath);
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
            this.on(MapEvents.verticalAspect, {
                blocks: this.getBlocksData(blocks)
            });
        }
    }
    private onClearSelectedBlock() {
        this.blockGrid.selectedBlockLine.forEach(o => this.map.removeOverlay(o));
        this.blockGrid.selectedBlocks.filter(o => true).forEach(o => this.onSelectBlock(o));
        this.on(MapEvents.clearAspect);
    }
    private onShowHorizontalAspect(): any {
        var blocks = this.blockGrid.selectedBlocks;
        if (blocks) {
            var min = blocks.min(o => o.context.center.lat);
            var max = blocks.max(o => o.context.center.lat);
            this.blockGrid.selectedBlockLine.forEach(o => this.map.removeOverlay(o));
            this.addLine(min.getBounds().getSouthWest(), 10000, 0);
            this.addLine(max.getBounds().getNorthEast(), 10000, 0);
            this.on(MapEvents.horizontalAspect, {
                blocks: this.getBlocksData(blocks)
            });
        }
    }
    private getBlocksBounds(blocks: Block[]): Bound {
        var minLat = blocks.min(o => o.context.center.lat).getBounds().getSouthWest().lat;
        var maxLat = blocks.max(o => o.context.center.lat).getBounds().getNorthEast().lat;
        var minLng = blocks.min(o => o.context.center.lng).getBounds().getSouthWest().lng;
        var maxLng = blocks.max(o => o.context.center.lng).getBounds().getNorthEast().lng;
        return new BMap.Bounds(
            new BMap.Point(minLng, minLat),
            new BMap.Point(maxLng, maxLat)
        );
    }
    private onShowSelectedBlockReport() {
        var blocks = this.blockGrid.selectedBlocks;
        if (blocks) {
            var bound = this.getBlocksBounds(blocks)
            var reports: PollutantReport[] = [];
            var time;
            blocks.forEach(block => {
                if (!time) {
                    time = block.context.time;
                }
                var rp = block.context.getReports(i => true);
                rp.forEach(report => {
                    var tmp = reports.first(o => o.pollutant.Name == report.pollutant.Name)
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
    }
    private onShowBlockReport(block: Block) {
        this.onShowReport(block.getBounds(), block.context.getReports(o => true), block.context.time);
    }
    private onShowReport(bound: Bound, reports: PollutantReport[], time: string) {
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
            blockGrid.infoWindow.addEventListener("close", o => {
                var win: InfoWindow = o.target;
                //console.dir(win);
                if (win.targetBorder) {
                    this.map.removeOverlay(win.targetBorder);
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
        reports.forEach(o => content += this.createInfoWindowContent(o));
        blockGrid.infoWindow.setContent(content);
        this.map.openInfoWindow(blockGrid.infoWindow, bound.getNorthEast())
        this.map.addOverlay(blockGrid.infoWindow.targetBorder);
    }
    private onSaveSelectedBlocks() {
        if (this.tempSelectedData && this.tempSelectedData.length) {
            this.on(MapEvents.savePoints, { points: this.tempSelectedData });
            delete this.tempSelectedData
            return;
        }
        var blocks = this.blockGrid.selectedBlocks;
        if (blocks && blocks.length) {
            this.on(MapEvents.savePoints, { points: blocks.selectMany(o => o.context.getPoints(i => true).select(i => i.data)) })
        }
    }

    /**无人机定位。 */
    private onUavLoaction(): any {
        if (this.uavList) {
            var uav = this.uavList.first(i => true);
            if (uav) {
                this.uavFocus(uav.name);
            }
        }
    }
    /**刷新 */
    private onRefresh(): any {
        window.location.reload(true);
    }
    /**获取地图边界。 */
    private getMapBounds() {
        var bounds: Bound = this.map.getBounds();
        if (bounds) {
            return { sw: bounds.getSouthWest(), ne: bounds.getNorthEast() }
        }
    }
    /**获取地图中所有无人机数据 */
    private getUavData() {
        return this.uavList.select(o => {
            var i = o.marker.getPosition()
            return {
                lat: i.lat,
                lng: i.lng,
                name: o.name,
            };
        });
    }
    /**获取所有在地图上的方块数据。 */
    private getBlocksData(blocks: Block[]) {
        return blocks.filter(o => o.context.color).select(o => {
            var b = o.getBounds();
            return {
                sw: b.getSouthWest(),
                ne: b.getNorthEast(),
                center: o.context.center,
                points: o.context.getPoints(i => true).select(i => i.data),
                reports: o.context.getReports(i => true),
                color: o.context.color,
                opacity: this.blockGrid.options.opacity
            }
        })
    }



    private onMapLoad(map) {
        this.convertor = new BMap.Convertor();
        this.analysisArea = new BaiduMapAnalysisArea(map, this);
        map.centerAndZoom(new BMap.Point(113.140761, 23.033974), 17);  // 初始化地图,设置中心点坐标和地图级别
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
        map.enableScrollWheelZoom(true);     //开启鼠标滚轮缩放

        new BaiduMapSelector(map, o => {
            if (this.analysisArea.isEnabled() && !this.analysisArea.getBounds()) {
                this.analysisArea.setBounds(o.bound);
            } else {
                this.blockGrid.blocks.forEach(b => {
                    if (o.bound.containsPoint(b.context.center)) {
                        if (o.event.shiftKey) {
                            this.onSelectBlock(b, MapBlockSelectAction.focusUnselect);
                        } else if (o.event.ctrlKey) {
                            this.onSelectBlock(b, MapBlockSelectAction.focusSelect);
                        } else {
                            this.onSelectBlock(b);
                        }
                    }
                })
            }
        });

        //map.setMapStyle({
        //    style: 'midnight'
        //});
        // 地图自定义样式
        map.setMapStyle({
            styleJson: [{
                "featureType": "water",
                "elementType": "all",
                "stylers": {
                    "color": "#044161"
                }
            }, {
                "featureType": "land",
                "elementType": "all",
                "stylers": {
                    "color": "#091934"
                }
            }, {
                "featureType": "boundary",
                "elementType": "geometry",
                "stylers": {
                    "color": "#064f85"
                }
            }, {
                "featureType": "railway",
                "elementType": "all",
                "stylers": {
                    "visibility": "off"
                }
            }, {
                "featureType": "highway",
                "elementType": "geometry",
                "stylers": {
                    "color": "#004981"
                }
            }, {
                "featureType": "highway",
                "elementType": "geometry.fill",
                "stylers": {
                    "color": "#005b96",
                    "lightness": 1
                }
            }, {
                "featureType": "highway",
                "elementType": "labels",
                "stylers": {
                    "visibility": "on"
                }
            }, {
                "featureType": "arterial",
                "elementType": "geometry",
                "stylers": {
                    "color": "#004981",
                    "lightness": -39
                }
            }, {
                "featureType": "arterial",
                "elementType": "geometry.fill",
                "stylers": {
                    "color": "#00508b"
                }
            }, {
                "featureType": "poi",
                "elementType": "all",
                "stylers": {
                    "visibility": "off"
                }
            }, {
                "featureType": "green",
                "elementType": "all",
                "stylers": {
                    "color": "#056197",
                    "visibility": "off"
                }
            }, {
                "featureType": "subway",
                "elementType": "all",
                "stylers": {
                    "visibility": "off"
                }
            }, {
                "featureType": "manmade",
                "elementType": "all",
                "stylers": {
                    "visibility": "off"
                }
            }, {
                "featureType": "local",
                "elementType": "all",
                "stylers": {
                    "visibility": "off"
                }
            }, {
                "featureType": "arterial",
                "elementType": "labels",
                "stylers": {
                    "visibility": "off"
                }
            }, {
                "featureType": "boundary",
                "elementType": "geometry.fill",
                "stylers": {
                    "color": "#029fd4"
                }
            }, {
                "featureType": "building",
                "elementType": "all",
                "stylers": {
                    "color": "#1a5787"
                }
            }, {
                "featureType": "label",
                "elementType": "all",
                "stylers": {
                    "visibility": "off"
                }
            }, {
                "featureType": "poi",
                "elementType": "labels.text.fill",
                "stylers": {
                    "color": "#ffffff"
                }
            }, {
                "featureType": "poi",
                "elementType": "labels.text.stroke",
                "stylers": {
                    "color": "#1e1c1c"
                }
            }, {
                "featureType": "administrative",
                "elementType": "labels",
                "stylers": {
                    "visibility": "off"
                }
            }, {
                "featureType": "road",
                "elementType": "labels",
                "stylers": {
                    "visibility": "off"
                }
            }]
        });

        this.map = map;

        this.blockGrid = new MapGrid();
        this.blockGrid.blocks = new Array<any>();
        this.uavList = new Array<Uav>();
        this.subscribe(MapEvents.load, true);
        this.subscribe(MapEvents.clearAnalysisArea, true);
        this.subscribe(MapEvents.clearAspect, true);
        this.subscribe(MapEvents.horizontalAspect, true);
        this.subscribe(MapEvents.pointConvert, true);
        this.subscribe(MapEvents.savePoints, true);
        this.subscribe(MapEvents.selectAnalysisArea, true);
        this.subscribe(MapEvents.verticalAspect, true);
        this.on(MapEvents.load);
        var h = setInterval(() => {
            var i = $("a[title='到百度地图查看此区域']");
            var b = $("span[_cid='1']");
            if (!i.hasClass("hide") || !b.hasClass("hide")) {
                i.addClass("hide");
                b.addClass("hide");
            } else {
                clearInterval(h);
            }
        }, 100);
        map.addEventListener("moveend", o => this.onMapBoundChanged());
        map.addEventListener("zoomend", o => this.onMapBoundChanged());
    }
    /**
     * 初始化地图。
     * @param container 地图容器id
     */
    mapInit(container: string) {
        this.loadJs("http://api.map.baidu.com/getscript?v=2.0&ak=TCgR2Y0IGMmPR4qteh4McpXzMyYpFrEx", () => this.onMapLoad(new BMap.Map(container)));
    }

    mapInitMenu(edit: boolean) {
        var menu: ContextMenu = new BMap.ContextMenu();
        var createItem = (name: MapMenuItems, func) => {
            var i = new BMap.MenuItem(name, func);
            i.name = name;
            return i;
        };
        if (edit) {
            this.menuItems = [
                //createItem(MapMenuItems.compare, o => this.onShowReport()),
                createItem(MapMenuItems.refresh, o => this.onRefresh()),
                false,
                createItem(MapMenuItems.uavLocation, o => this.onUavLoaction()),
                createItem(MapMenuItems.uavFollow, o => this.uavFollow = !this.uavFollow),
                createItem(MapMenuItems.uavPath, o => this.uavPath = !this.uavPath),
                false,
                createItem(MapMenuItems.savePoints, o => this.onSaveSelectedBlocks()),
                createItem(MapMenuItems.reports, o => this.onShowSelectedBlockReport()),
                createItem(MapMenuItems.horizontal, o => this.onShowHorizontalAspect()),
                createItem(MapMenuItems.vertical, o => this.onShowVerticalAspect()),
                createItem(MapMenuItems.clear, o => this.onClearSelectedBlock()),
                false,
                createItem(MapMenuItems.selectAnalysisArea, o => this.analysisArea.enable()),
                createItem(MapMenuItems.clearAnalysisArea, o => this.analysisArea.disable()),
            ];
        } else {
            this.menuItems = [
                //createItem(MapMenuItems.compare, o => this.onShowReport()),
                createItem(MapMenuItems.refresh, o => this.onRefresh()),
                false,
                createItem(MapMenuItems.savePoints, o => this.onSaveSelectedBlocks()),
                createItem(MapMenuItems.reports, o => this.onShowSelectedBlockReport()),
                createItem(MapMenuItems.horizontal, o => this.onShowHorizontalAspect()),
                createItem(MapMenuItems.vertical, o => this.onShowVerticalAspect()),
                createItem(MapMenuItems.clear, o => this.onClearSelectedBlock()),
            ];
        }
        this.menuItems.forEach(o => o ? menu.addItem(o) : menu.addSeparator());
        menu.addEventListener("open", o => this.onCheckContextMenu());
        this.map.addContextMenu(menu);
    }

    /**
     * 地图坐标转换。转换完成的点会以pointConvert事件回调。
     * @param seq 序列号
     * @param p 转换的点。
     */
    mapPointConvert(seq: number, p: Point[]) {
        var points = this.parseJson(p);
        this.convertor.translate(points, 1, 5, o => {
            if (o.status == 0) {
                this.on(MapEvents.pointConvert, { Seq: seq, Points: o.points })
            }
        });
    }
    /**
     * 显示临时报表。
     * @param d
     */
    mapShowTempReport(d: any) {
        var data: Array<any> = this.parseJson(d);
        this.tempSelectedData = data;
        if (data) {
            var reports: PollutantReport[] = [];
            var time: string;
            var blocks: Block[] = [];
            this.blockGrid.options.pollutants.forEach(pollutant => {
                var rp = new PollutantReport();
                rp.pollutant = pollutant;
                rp.count = 0;
                reports.push(rp);
            });
            data.forEach(d => {
                if (!time) {
                    time = d.time;
                }
                var lat = d.ActualLat;
                var lng = d.ActualLng;
                var block = this.blockGrid.blocks.first(o => o.getBounds().containsPoint(new BMap.Point(lng, lat)))
                if (block && !blocks.first(o => o == block)) {
                    blocks.push(block)
                }
                reports.forEach(rp => {
                    var val = d[rp.pollutant.Name];
                    if (rp.count == 0) {
                        rp.avg = val;
                        rp.sum = val;
                        rp.min = val;
                        rp.max = val;
                    } else {
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
    }
    mapClearTempReport() {
        delete this.tempSelectedData;
        this.map.closeInfoWindow(this.blockGrid.infoWindow)
        this.map.removeOverlay(this.blockGrid.infoWindow.targetBorder);
    }
    gridInit(opt: MapGridOptions) {
        opt = this.parseJson(opt);
        if (!opt) opt = new MapGridOptions();
        if (!opt.sideLength) opt.sideLength = 100;//默认边长100米，地图比例尺约1米约等于0.00001经纬度
        if (!opt.blockList) opt.blockList = [];//网格列表。
        //if (!opt.colorBegin) opt.colorBegin = "#FF0000";
        //if (!opt.colorEnd) opt.colorEnd = "#00FF00";
        if (!opt.opacity) opt.opacity = 0.5;
        if (!opt.pollutant) opt.pollutant = new Pollutant();

        if (!opt.pollutant.Name) opt.pollutant.Name = "sample";
        if (!opt.pollutant.MaxValue) opt.pollutant.MaxValue = 100;
        if (!opt.pollutant.MinValue) opt.pollutant.MinValue = 0;
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
            var report = block.context.getReports(o => o.pollutant.Name == opt.pollutant.Name).first(o => true);
            if (report) {
                var color = this.getColorByReport(report);
                if (block.context.color != color) {
                    block.context.color = color;
                    this.on(MapEvents.blockChanged, { blocks: this.getBlocksData(this.blockGrid.blocks) });
                }
                block.setFillColor(block.context.color);
            }
        });
    }
    gridClear() {
        if (this.blockGrid.blocks) {
            this.blockGrid.blocks.forEach(o => this.map.removeOverlay(o));
        }
        this.onClearSelectedBlock();
        delete this.blockGrid.blocks;
        this.blockGrid.blocks = [];
    }
    uavAdd(name: string, lng: number, lat: number, d: any) {
        var data: Array<any> = this.parseJson(d);
        this.uav(name, null, () => {
            var point = new BMap.Point(lng, lat);
            point.data = data[0];
            var icon = new BMap.Icon("marker.png", new BMap.Size(30, 30));
            var uav = new Uav();
            uav.name = name;
            uav.marker = new BMap.Marker(point, { icon: icon });
            uav.pathPoint = data.select(o => {
                var p: Point = new BMap.Point(o.ActualLng, o.ActualLat);
                p.data = o;
                return p;
            });
            this.uavList.push(uav);
            this.map.addOverlay(uav.marker);
            this.on(MapEvents.uavChanged, { uav: this.getUavData() });
        });
    }
    uavMove(name: string, lng: number, lat: number, d: any) {
        var data = this.parseJson(d);
        this.uav(name, o => {
            var point = new BMap.Point(lng, lat);
            point.data = data;
            o.pathPoint.push(point);
            o.marker.setPosition(point);
            this.on(MapEvents.uavChanged, { uav: this.getUavData() });
            if (this.uavFollow) {
                this.map.panTo(point);
            }
        }, null);
    }
    uavPathRefresh(name: string) {
        this.uav(name, o => {
            if (!o.pathMarker) {
                o.pathMarker = new BMap.Polyline([], {
                    enableClicking: false,
                    strokeWeight: 1,
                    strokeColor: "red",
                    strokeOpacity: 0.5
                });
            }
            this.map.removeOverlay(o.pathMarker);
            if (this.uavPath) {
                o.pathMarker.setPath(o.pathPoint);
                this.map.addOverlay(o.pathMarker);
            }
        }, null);
    }
    uavRemove(name: string) {
        var i = -1;
        this.uavList.forEach((o, index) => {
            if (o.name == name) {
                this.map.removeOverlay(o.marker);
                this.uavPath = false;
                this.uavPathRefresh(name);
                this.map.removeOverlay(o.pathMarker);
                i = index;
                delete o.marker;
                delete o.pathMarker;
                this.on(MapEvents.uavChanged, { uav: this.getUavData() });
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
    onSubscribe(eventName: MapEvents) {
        var result: any;
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