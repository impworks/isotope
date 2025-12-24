import { onUnmounted } from 'vue';
import { CompositeDisposable } from '../../../../Common/source/utils/CompositeDisposable';
import type { Action, IObservable } from '../../../../Common/source/utils/Interfaces';

/**
 * Composable for handling subscriptions to events with automatic cleanup on component unmount.
 * Replaces the HasLifetime mixin from Vue2.
 */
export function useLifetime() {
    const subscriptions = new CompositeDisposable();

    /**
     * Subscribes to an observable with automatic unsubscription when component is unmounted.
     * @param obs Observable to monitor.
     * @param handler Handler for the events.
     */
    function observe<T>(obs: IObservable<T>, handler: Action<T>): void {
        subscriptions.add(obs.subscribe(handler));
    }

    // Auto cleanup on component unmount
    onUnmounted(() => {
        subscriptions.dispose();
    });

    return {
        observe
    };
}
