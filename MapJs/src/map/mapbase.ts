import './map.css'

export class BlockContextImp implements BlockContext {
    constructor(center: Point, pollutants: Pollutant[]) {
        this.center = center;
        pollutants.forEach(o => {
            var report = <PollutantReport>{};
            report.pollutant = o;
            report.count = 0;
            report.avg = 0;
            report.avg = 0;
            report.sum = 0;
            report.max = 0;
            report.min = 0;
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


export abstract class MapBase implements IMapProvider, IEventAggregator {

    abstract mapCenter(point: Point): Point;
    abstract mapClearTempReport();
    abstract mapShowTempReport(d: any);
    abstract mapPointConvert(seq: number, p: Point[]);
    abstract mapInit(container: string);
    abstract mapInitMenu(edit: boolean);
    abstract mapStyle(name: string);
    abstract uavPathRefresh(name: string);
    abstract uavRemove(name: string);
    abstract uavExist(name: string): boolean;
    abstract uavFocus(name: string);
    abstract uavAdd(name: string, lng: number, lat: number, d: any);
    abstract uavMove(name: string, lng: number, lat: number, d: any);
    abstract gridClear();
    abstract gridInit(opt: MapGridOptions);
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
