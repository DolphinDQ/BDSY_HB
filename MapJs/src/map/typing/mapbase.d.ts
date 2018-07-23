
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
/**事件订阅兑现，用它标识事件是否被订阅 */
interface EventSubscribe {
    name: MapEvents;
    enable: boolean;
}
/**地图上显示的方块 */
interface BlockContext {
    readonly center: Point;
    color: string;
    time: string;
    addPoint(p: Point)
    getPoints(query: (o: Point) => boolean): Array<Point>
    getReports(query: (o: PollutantReport) => boolean): Array<PollutantReport>
}
/**地图接口，实现了这个接口，就可以和应用程序对接。 */
interface IMapProvider {
    /**
     * 初始化地图。在加载地图完成后会调用。
     * @param container 地图容器，看html用于显示地图的div.id
     */
    mapInit(container: string);
    /**
     * 初始化地图右键菜单。
     * @param edit 是否是编辑模式，如果是编辑模式，右键菜单选项可能会多一点。
     */
    mapInitMenu(edit: boolean);
    /**
     * 转换地图坐标点。原始坐标为GPS坐标，如果使用的地图坐标系不是GPS标准坐标系，可以通过这个接口转换。转换完成的坐标以事件形式发回。
     * @param seq 序列号。
     * @param p 原始坐标。
     */
    mapPointConvert(seq: number, p: Point[]);
    /**
     * 显示临时报表。可能是用户选中的某些点。
     * @param d 用户选择的点的接合。
     */
    mapShowTempReport(d: any);
    /**清除临时报表 */
    mapClearTempReport();
    /**
     * 地图样式
     * @param name 地图样式名称。
     */
    mapStyle(name: string);
    /**
     * 获取或设置地图中心点。
     * @param point null的时候为获取中心点，非null为定位地图，并返回当前中心点。
     */
    mapCenter(point: Point): Point;
    /**
     * 初始化网格（方块）。
     * @param opt 网格选线。
     */
    gridInit(opt: MapGridOptions);
    /**刷新网格 */
    gridRefresh();
    /**清除网格 */
    gridClear();
    /**
     * 添加无人机。
     * @param name 无人机名称（标识）
     * @param lng 无人机的经度
     * @param lat 无人机纬度
     * @param d 无人机携带的默认数据。
     */
    uavAdd(name: string, lng: number, lat: number, d: any);
    /**
     * 无人机移动。
     * @param name 无人机名称
     * @param lng 无人机移动到的经度
     * @param lat 无人机移动到的纬度
     * @param d 无人机移动期间采集的数据。
     */
    uavMove(name: string, lng: number, lat: number, d: any);
    /**
     * 无人机移动路径刷新。
     * @param name 无人机名称（标识）
     */
    uavPathRefresh(name: string);
    /**
     * 删除无人机
     * @param name 无人机名称
     */
    uavRemove(name: string);
    /**
     * 确认无人机是否存在。
     * @param name 无人机名称。
     */
    uavExist(name: string): boolean;
    /**
     * 无人机定位。地图跳转到指定无人机的位置。
     * @param name 无人机名称。
     */
    uavFocus(name: string);
    /**
     * 订阅事件。事件类型可以参考MapEvents枚举。
     * @param eventName 见MapEvents枚举
     * @param enable 是否订阅。
     */
    subscribe(eventName: MapEvents, enable: boolean): any;
}
