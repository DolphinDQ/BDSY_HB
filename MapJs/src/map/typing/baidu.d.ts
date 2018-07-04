
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
