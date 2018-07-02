

interface IEventAggregator {
    on(eventName: MapEvents, arg?: any);
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
    addSeparator();
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
    getCenter(): Point;
}

interface InfoWindow {
    setContent(text: string);
    setWidth(width: Number);
    setHeight(height: Number);
    addEventListener(name: string, callFn: Function);
    targetBorder: any;
}


interface PollutantReport {
    pollutant: Pollutant;//污染物;
    count: number;
    avg: number;
    sum: number;
    max: number;
    min: number;
}

interface PollutantLevel {
    Name: string;
    MinValue: number;
    MaxValue: number;
    MaxColor: string;
    MinColor: string;
}

interface Pollutant {
    Name: string ;
    DisplayName: string ;
    MaxValue: number;
    MinValue: number;
    Unit: string;
    Levels: PollutantLevel[] ;
}

interface MapGridOptions {
    sideLength: number;
    blockList: any[];
    opacity: number;
    pollutants: Pollutant[];
    pollutant: Pollutant;
}

interface MapGrid {
    options: MapGridOptions;
    blocks: Block[];
    firstPoint: Point;
    infoWindow: InfoWindow;
    selectedBlocks: Block[];
    selectedBlockLine: any[] ;
}

interface Uav {
    pathPoint: Point[];
    name: string;
    marker: any;
    pathMarker: any;
}

interface EventSubscribe {
    name: MapEvents;
    enable: boolean;
}
interface BlockContext{
    readonly center: Point;
    color: string;
    time: string;
    addPoint(p: Point) 
    getPoints(query: (o: Point) => boolean): Array<Point> 
    getReports(query: (o: PollutantReport) => boolean): Array<PollutantReport> 
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
interface IMapProvider {
    mapInit(container: string);
    mapInitMenu(edit: boolean);
    mapPointConvert(seq: number, p: Point[]);
    mapShowTempReport(d: any);
    mapClearTempReport();
    gridInit(opt: MapGridOptions);
    gridRefresh();
    gridClear();
    uavAdd(name: string, lng: number, lat: number, d: any);
    uavMove(name: string, lng: number, lat: number, d: any);
    uavPathRefresh(name: string);
    uavRemove(name: string);
    uavExist(name: string): boolean;
    uavFocus(name: string);
    subscribe(eventName: MapEvents, enable: boolean): any;
}
declare var BMap;
declare var BMAP_NORMAL_MAP;
declare var BMAP_HYBRID_MAP;
/**地图方块选择动作 */
 