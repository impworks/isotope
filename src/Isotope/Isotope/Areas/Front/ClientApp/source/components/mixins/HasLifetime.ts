import { Vue, Component } from "vue-property-decorator";
import { CompositeDisposable } from "../../utils/CompositeDisposable";
import { IObservable, Action } from "../../utils/Interfaces";

/**
 * Mixin for handling subscriptions to events and automatic unsubscription on component dispose.
 */
@Component
export class HasLifetime extends Vue {

    // -----------------------------------
    // Properties
    // -----------------------------------

    private _subscriptions: CompositeDisposable;

    // -----------------------------------
    // Methods
    // -----------------------------------

    created() {
        this._subscriptions = new CompositeDisposable();
    }

    destroyed() {
        this._subscriptions.dispose();
    }

    /**
     * Subscribes to an observable with automatic unsubscription when this component is disposed.
     * @param obs Observable to monitor.
     * @param handler Handler for the events.
     */
    protected observe<T>(obs: IObservable<T>, handler: Action<T>): void {
        this._subscriptions.add(obs.subscribe(handler));
    }
}