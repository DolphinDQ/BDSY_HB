import { MapBase } from "./mapbase";
declare var AMap;

class AmapProvider extends MapBase {
 
    private map: any;

    private onLoad(): any {
        var map = new AMap.Map('container', {
            zoom: 17,//级别
            center: [113.140761, 23.033974],//中心点坐标
            viewMode: '3D'//使用3D视图
        });

        this.map = map;
    }

    mapCenter(point: Point): Point {
        throw new Error("Method not implemented.");
    }
    mapStyle(name: string) {
        throw new Error("Method not implemented.");
    }
    mapClearTempReport() {
        throw new Error("Method not implemented.");
    }
    mapShowTempReport(d: any) {
        throw new Error("Method not implemented.");
    }
    mapPointConvert(seq: number, p: Point[]) {
        throw new Error("Method not implemented.");
    }
    uavPathRefresh(name: string) {
        throw new Error("Method not implemented.");
    }
    uavRemove(name: string) {
        throw new Error("Method not implemented.");
    }
    uavExist(name: string): boolean {
        throw new Error("Method not implemented.");
    }
    uavFocus(name: string) {
        throw new Error("Method not implemented.");
    }
    gridClear() {
        throw new Error("Method not implemented.");
    }
    uavAdd(name: string, lng: number, lat: number, d: any) {
        throw new Error("Method not implemented.");
    }
    uavMove(name: string, lng: number, lat: number, d: any) {
        throw new Error("Method not implemented.");
    }
    gridInit(opt: MapGridOptions) {
        throw new Error("Method not implemented.");
    }
    mapInit(container: string) {
        this.loadJs("http://webapi.amap.com/maps?v=1.4.7&key=8f120157a37d1ef0d837aabe7099e1d0", () => this.onLoad());
    }
    mapInitMenu(edit: boolean) {
        throw new Error("Method not implemented.");
    }
    gridRefresh() {
        throw new Error("Method not implemented.");
    }
    onSubscribe(eventName: MapEvents) {
        throw new Error("Method not implemented.");
    }
}

export default <IMapProvider>new AmapProvider();