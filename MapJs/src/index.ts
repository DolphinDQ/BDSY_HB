import map from './map/baidu'
//import map from './map/amap'


(function () {
    map.mapInit("container");
    (<any>window).map = map;
   
})();