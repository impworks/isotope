import { Action, IDisposable, IObservable } from "./Interfaces";

/**
 * Sample implementation of IObservable interface
 */
export class Observable<T> implements IObservable<T> {

    // -----------------------------------
    // Fields
    // -----------------------------------

    private _subscriptions: Array<(x: T) => void>;

    private _mustCache: boolean;
    private _wasNotified: boolean;
    private _value: T;

    // -----------------------------------
    // Constructor
    // -----------------------------------

    constructor(mustCache?: boolean) {
        this._subscriptions = [];
        this._mustCache = mustCache;
        this._value = null;
    }

    // -----------------------------------
    // IObservable implementation
    // -----------------------------------

    /**
     * Registers the callback for this event.
     * @param handler Callback to invoke.
     */
    subscribe(handler: Action<T>): IDisposable {
        this._subscriptions.push(handler);

        if (this._mustCache && this._wasNotified != null)
            handler(this._value);

        return {
            dispose: () => this.unsubscribe(handler)
        };
    }

    /**
     * Registers the one-time callback for this event.
     * @param handler Callback to invoke.
     */
    subscribeOnce(handler: Action<T>): void {
        const wrapper = (x: T) => {
            handler(x);
            this.unsubscribe(wrapper);
        };
        this.subscribe(wrapper);
    }

    /**
     * Notifies all subscribed callbacks.
     * @param value
     */
    notify(value: T) {
        for (let i = 0; i < this._subscriptions.length; i++) {
            this._subscriptions[i](value);
        }

        if (this._mustCache) {
            this._value = value;
            this._wasNotified = true;
        }
    }

    // -----------------------------------
    // IDisposable implementation
    // -----------------------------------

    dispose() {
        this._subscriptions = [];
    }

    // -----------------------------------
    // Private methods
    // -----------------------------------

    private unsubscribe(handler: Action<T>) {
        for (let i = 0; i < this._subscriptions.length; i++) {
            const curr = this._subscriptions[i];
            if (curr === handler) {
                this._subscriptions[i] = null;
                this._subscriptions.splice(i, 1);
                return;
            }
        }
    }
}