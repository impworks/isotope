export class EventHelper {

    /**
     * Sends a mouse event to the element.
     */
    public static sendMouseEvent(el: HTMLElement, evtType: 'mousedown' | 'mouseup' | 'mousemove') {
        const evt = document.createEvent('MouseEvent');
        evt.initEvent(evtType, true, true);
        el.dispatchEvent(evt);
    }
}