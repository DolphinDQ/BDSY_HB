"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var mapbase_1 = require("./mapbase");
var AmapProvider = /** @class */ (function (_super) {
    __extends(AmapProvider, _super);
    function AmapProvider() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    AmapProvider.prototype.onLoad = function () {
        var map = new AMap.Map('container', {
            zoom: 17,
            center: [113.140761, 23.033974],
            viewMode: '3D' //使用3D视图
        });
        this.map = map;
    };
    AmapProvider.prototype.mapCenter = function (point) {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.mapStyle = function (name) {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.mapClearTempReport = function () {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.mapShowTempReport = function (d) {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.mapPointConvert = function (seq, p) {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.uavPathRefresh = function (name) {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.uavRemove = function (name) {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.uavExist = function (name) {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.uavFocus = function (name) {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.gridClear = function () {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.uavAdd = function (name, lng, lat, d) {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.uavMove = function (name, lng, lat, d) {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.gridInit = function (opt) {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.mapInit = function (container) {
        var _this = this;
        this.loadJs("http://webapi.amap.com/maps?v=1.4.7&key=8f120157a37d1ef0d837aabe7099e1d0", function () { return _this.onLoad(); });
    };
    AmapProvider.prototype.mapInitMenu = function (edit) {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.gridRefresh = function () {
        throw new Error("Method not implemented.");
    };
    AmapProvider.prototype.onSubscribe = function (eventName) {
        throw new Error("Method not implemented.");
    };
    return AmapProvider;
}(mapbase_1.MapBase));
exports.default = new AmapProvider();
//# sourceMappingURL=amap.js.map