
interface IEventAggregator {
    on(eventName: MapEvents, arg?: any);
}

interface Point {
    lat: number;
    lng: number;
    data: any;//采样数据
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
    Name: string;
    DisplayName: string;
    MaxValue: number;
    MinValue: number;
    Unit: string;
    Levels: PollutantLevel[];
}

interface PollutantSetting {
    Pollutant: Pollutant[];
    CorrectAltitude: number;
    MaxAltitude: number;
    AltitudeUnit: string;
    Opacity: number;
    SideLength: number;
}

interface MapGridOptions {
    blockList: any[];
    pollutant: Pollutant;
    settings: PollutantSetting;
}

interface MapGrid {
    options: MapGridOptions;
    blocks: Block[];
    firstPoint: Point;
    infoWindow: InfoWindow;
    selectedBlocks: Block[];
    selectedBlockLine: any[];
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
interface BlockContext {
    readonly center: Point;
    color: string;
    time: string;
    addPoint(p: Point)
    getPoints(query: (o: Point) => boolean): Array<Point>
    getReports(query: (o: PollutantReport) => boolean): Array<PollutantReport>
}

interface IMapProvider {
    mapInit(container: string);
    mapInitMenu(edit: boolean);
    mapPointConvert(seq: number, p: Point[]);
    mapShowTempReport(d: any);
    mapClearTempReport();
    mapStyle(name: string);
    mapCenter(point: Point): Point;
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
