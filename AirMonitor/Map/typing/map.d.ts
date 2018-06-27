interface Array<T> {
    first(query: (a: T) => boolean): T;
    min(query: (a: T) => number): T;
    max(query: (a: T) => number): T;
    avg(query: (a: T) => number): T;
    select<I>(query: (a: T) => I): Array<I>;
    selectMany<I>(query: (a: T) => Array<I>): Array<I>;
}
interface External {
    On(eventName: string, arg: any);
}

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