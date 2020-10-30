import { IDisposable, Action, IObservable, Func2 } from './Interfaces';

/**
 * Contains a collection of IDisposable items and disposes them at once.
 */
export class CompositeDisposable implements IDisposable {

    // -----------------------------------
    // Constructor
    // -----------------------------------

    constructor() {
        this._disposables = [];
    }

    // -----------------------------------
    // Fields
    // -----------------------------------

    private _disposables: IDisposable[];

    // -----------------------------------
    // Methods
    // -----------------------------------

    /**
     * Adds a new IDisposable object to the list.
     */
    add(obj: IDisposable) {
        if (this._isDisposed) {
            obj.dispose();
        } else {
            this._disposables.push(obj);
        }
    }

    /**
     * Subscribes a list of objects to a callback and stores subscriptions inside.
     * @param items List of objects that expose an event.
     * @param eventGetter Function to retrieve an event.
     * @param callback Subscription to the event.
     */
    subscribe<TItem, TEventArg>(items: TItem[],
        eventGetter: Func2<TItem, IObservable<TEventArg>>,
        callback: Action<TEventArg>) {
        items.forEach(item => {
            var subscription = eventGetter(item).subscribe(callback);
            this.add(subscription);
        });
    }

    /**
     * Clears all existing subscriptions.
     */
    clear() {
        this._disposables.forEach(d => d.dispose());
        this._disposables = [];
    }

    // -----------------------------------
    // IDisposable implementation
    // -----------------------------------

    private _isDisposed: boolean = false;

    dispose() {
        if (!this._isDisposed) {
            this._disposables.forEach(d => d.dispose());
            this._isDisposed = true;
        }
    }
}