import map from './map/baidu'
import * as testdata from './test.json'


(function () {
    map.mapInit("container");
    (<any>window).map = map;
    (<any>window).test = () => {
        
        var p = (<Array<any>>(testdata.default.Samples)).first(o=>true);
        debugger;
        var opt = <MapGridOptions>{
            pollutant:testdata.default.Standard.Pollutant[0],
            settings: testdata.default.Standard.Pollutant,
        };
        map.gridInit(opt);
        map.uavAdd("default", p.ActualLng, p.ActualLat, JSON.stringify(testdata.default.Samples));
        //for (var i = 0; i < 20; i++) {
        //    if (i > 10) {
        //        map.uavMove("default", p.lng + (i / 10000), p.lat + ((i - 10) / 10000), { sample: Math.random() * 100, time: (new Date).toLocaleDateString() })
        //    } else {
        //        map.uavMove("default", p.lng + (i / 10000), p.lat, { sample: Math.random() * 100, time: (new Date).toLocaleDateString() })
        //    }
        //}
        map.uavFocus("default");
        map.gridRefresh();
    };
})();