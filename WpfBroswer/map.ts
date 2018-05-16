if (!Array.prototype.find) {
    Array.prototype.find = function (query) {
        var arr = this;
        if (query) {
            for (var i = 0; i < arr.length; i++) {
                if (query(arr[i])) {
                    return arr[i];
                }
            }
        }
    }
}
interface IMapProvider {
    onLoad: () => any;
    load(container: string);
    mapPointConvert(seq: number, p: Point[]);
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

interface Point {
    lat: number,
    lng: number;
}

class Pollutant {
    Name: string;
    DisplayName: string;
    MaxValue: number;
    MinValue: number;
    Unit: string;
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
    blocks: any[];
    firstPoint: Point;
    infoWindow: any;
}

class Uav {
    pathPoint: Point[];
    name: string;
    marker: any;
    pathMarker: any;
}

abstract class MapBase implements IMapProvider {
    abstract mapPointConvert(seq: number, p: Point[]);
    abstract uavShowPath(name: string);
    abstract uavHidePath(name: string);
    abstract uavRemove(name: string);
    abstract uavExist(name: string): boolean;
    abstract uavFocus(name: string);
    abstract gridClear();
    abstract uavAdd(name: string, lng: number, lat: number, d: any);
    abstract uavMove(name: string, lng: number, lat: number, d: any);
    onLoad: () => any;
    abstract gridInit(opt: MapGridOptions);
    abstract load(container: string);
    abstract gridRefresh();
    protected loadJs(url: string, onLoad: (e) => any) {
        var file = document.createElement("script");
        file.setAttribute("type", "text/javascript");
        file.setAttribute("src", url);
        file.onload = onLoad;
        document.getElementsByTagName("head")[0].appendChild(file);
    }
    protected parseJson<T>(obj: T): T {
        if (typeof (obj) == "string")
            obj = JSON.parse(obj)
        return obj;
    }
    protected on(eventName: string, arg: any) {
        try {
            window.external.On("pointConvert", JSON.stringify(arg));
        } catch (e) {
            //ignore;
        }
    }
}

declare var BMap;
declare var BMAP_NORMAL_MAP;
declare var BMAP_HYBRID_MAP;
class BaiduMapProvider extends MapBase {
    private map: any;
    private convertor: any;
    private blockGrid: MapGrid;
    private uavList: Uav[];
    private getColor(value, opt: MapGridOptions, min, max) {
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
    private getColorValue(percent, begin, end) {
        var flag = end > begin ? 1 : -1;
        var result = parseInt(Math.abs(end - begin) * percent * flag + begin).toString(16);
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
    private createInfoWindowContent(contentData, opt: MapGridOptions) {
        var pollutant = opt.pollutants.find(p => p.Name == contentData.title);
        if (!pollutant) return "";
        return this.getInfoWindowContentTemplate({
            title: pollutant.DisplayName,
            min: Math.round(contentData.min * 100) / 100,
            max: Math.round(contentData.max * 100) / 100,
            avg: Math.round(contentData.avg * 100) / 100,
            unit: pollutant.Unit,
            background: this.getColor(contentData.avg, opt, pollutant.MinValue, pollutant.MaxValue),
            opacity: opt.opacity,
        });
    }
    private createBlock(point, opt) {
        var center = opt.firstPoint;
        var sideLength = opt.sideLength * 0.00001;
        var opacity = opt.opacity;
        var offset = sideLength / 2;//计算偏移经纬度。
        var lng = center.lng + sideLength * Math.round((point.lng - center.lng) / sideLength);
        var lat = center.lat + sideLength * Math.round((point.lat - center.lat) / sideLength)
        var center = new BMap.Point(lng, lat);
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
        polygon.center = center;
        polygon.points = [point]
        return polygon;
    }
    private isInBlock(center, sideLength, point) {
        //块中心点，块边长，当前点是否在块里面。
        var offset = sideLength / 2 * 0.00001;//计算偏移经纬度。
        return point.lng > (center.lng - offset) &&
            point.lng < (center.lng + offset) &&
            point.lat > (center.lat - offset) &&
            point.lat < (center.lat + offset);
    }
    private uav(name: string, exist: (o: Uav) => any, notExist: () => any) {
        try {
            var uav = this.uavList.find(o => o.name == name);
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
    load(container: string) {
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
            this.map = map;
            this.blockGrid = new MapGrid();
            this.uavList = new Array<Uav>();
            if (this.onLoad) {
                this.onLoad();
            }
        });
    }
    mapPointConvert(seq: number, p: Point[]) {
        var points = this.parseJson(p);
        this.convertor.translate(points, 1, 5, function (o) {
            if (o.status == 0) {
                this.on("pointConvert", { Seq: seq, Points: o.points })
            }
        });
    }
    gridInit(opt: MapGridOptions) {
        opt = this.parseJson(opt);
        if (!opt) opt = new MapGridOptions();
        if (!opt.sideLength) opt.sideLength = 100;//默认边长100米，地图比例尺约1米约等于0.00001经纬度
        if (!opt.blockList) opt.blockList = [];//网格列表。
        if (!opt.colorBegin) opt.colorBegin = "#FF0000";
        if (!opt.colorEnd) opt.colorEnd = "#00FF00";
        if (!opt.opacity) opt.opacity = 0.5;
        if (!opt.dataName) opt.dataName = "";
        if (!opt.maxValue) opt.maxValue = 100;
        if (!opt.minValue) opt.minValue = 0;
        if (!opt.pollutants) opt.pollutants = [];//格式：{Name:"",DisplayName:"",MaxValue:0,MinValue:0,Unit:""}
        this.blockGrid.options = opt;

    }
    gridRefresh() {
        var points = [];
        for (var i = 0; i < this.uavList.length; i++) {
            points = points.concat(this.uavList[i].pathPoint);
        }
        var blockGrid = this.blockGrid;
        var opt = blockGrid.options;
        //填充点数据到格子里
        points.forEach(function (point) {
            if (!blockGrid.firstPoint) blockGrid.firstPoint = point;
            var block = blockGrid.blocks.find(block => this.isInBlock(block.center, opt.sideLength, point));
            if (!block) {
                block = this.createBlock(point, opt);
                blockGrid.blocks.push(block);
                block.addEventListener("click", function (e) {
                    console.dir(e);
                    if (!blockGrid.infoWindow) {
                        blockGrid.infoWindow = new BMap.InfoWindow("", {
                            width: 500,
                            height: 300
                        });
                    }
                    var pointsInBlock = e.target.points;//方块里的点。
                    var content = '<div><span>实时采样数据：</span><span>({{time}})</span></div>';
                    var contentData = [];
                    /* {
                        title: "",
                        sum: 0,
                        len: pointsInBlock.length
                    }*/
                    var time = null;
                    for (var i = 0; i < pointsInBlock.length; i++) {
                        var blockData = pointsInBlock[i].data;
                        if (!time) time = blockData.time;
                        for (var title in blockData) {
                            var val = blockData[title];
                            var tmp = contentData.find(d => d.title == title);
                            if (!tmp) {
                                tmp = {
                                    title: title,
                                    avg: 0,
                                    min: val,
                                    max: val
                                };
                                contentData.push(tmp);
                            }
                            if (tmp.min > val) tmp.min = val;
                            if (tmp.max < val) tmp.max = val;
                            tmp.avg = (tmp.avg * i + val) / (i + 1);
                        }
                    }
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
                    for (var i = 0; i < contentData.length; i++) {
                        content += this.createInfoWindowContent(contentData[i], opt)
                    }
                    blockGrid.infoWindow.setContent(content);
                    this.map.openInfoWindow(blockGrid.infoWindow, e.point)
                })
                this.map.addOverlay(block);
            } else {
                block.points.push(point);
            }
        });
        blockGrid.blocks.forEach(function (block) {
            var sum = 0;
            for (var i = 0; i < block.points.length; i++) {
                if (block.points[i].data) {
                    sum += block.points[i].data[opt.dataName]
                }
            }
            block.avgValue = sum / block.points.length;
            block.setFillColor(this.getColor(block.avgValue, opt));
        });
    }
    gridClear() {
        if (this.blockGrid.blocks) {
            this.blockGrid.blocks.forEach(function (o) {
                this.map.removeOverlay(o);
            });
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
                o.pathMarker.setPath(o.pathPoint);
            }
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
        this.uavList.forEach(function (o, index) {
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
    uavExist(name: string):boolean {
        var obj = this.uavList.find(function (o) { return o.name == name });
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
}

(function () {
    let map = new BaiduMapProvider();
    map.load("container");
    (<any>window).map = map;
})();