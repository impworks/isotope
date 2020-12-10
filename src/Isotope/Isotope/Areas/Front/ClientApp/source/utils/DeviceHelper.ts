export class DeviceHelper {

    static isTouch(): boolean {
        return ("ontouchstart" in document.documentElement);
    }
}