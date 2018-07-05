//import * as testdata from './test.json'
//(<any>window).test = () => {
//    var p = (<Array<any>>(testdata.default.Samples)).first(o => true);
//    var standard: PollutantSetting = testdata.default.Standard;
//    var opt = <MapGridOptions>{
//        pollutant: standard.Pollutant[0],
//        settings: standard,
//    };
//    map.gridInit(opt);
//    map.uavAdd("default", p.ActualLng, p.ActualLat, JSON.stringify(testdata.default.Samples));
//    //for (var i = 0; i < 20; i++) {
//    //    if (i > 10) {
//    //        map.uavMove("default", p.lng + (i / 10000), p.lat + ((i - 10) / 10000), { sample: Math.random() * 100, time: (new Date).toLocaleDateString() })
//    //    } else {
//    //        map.uavMove("default", p.lng + (i / 10000), p.lat, { sample: Math.random() * 100, time: (new Date).toLocaleDateString() })
//    //    }
//    //}
//    map.uavFocus("default");
//    map.gridRefresh();
//};
//# sourceMappingURL=test.js.map