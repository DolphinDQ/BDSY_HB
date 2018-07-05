import map from './map/baidu'


(function () {
    map.mapInit("container");
    (<any>window).map = map;
   
})();