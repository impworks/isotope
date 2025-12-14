import { reactive } from 'vue';
import type { Func } from '@common/utils/Interfaces';

/**
 * Default properties of a component's async state.
 */
export interface IAsyncState {
    isLoading?: boolean;
    isSaving?: boolean;
}

/**
 * Composable for handling async state (loading, saving indicators).
 * Replaces the HasAsyncState mixin from Vue2.
 */
export function useAsyncState<T extends Record<string, any> = {}>(initial?: T) {
    const asyncState = reactive({
        isLoading: false,
        isSaving: false,
        ...initial
    }) as IAsyncState & T;

    /**
     * Displays a loading indicator while executing an action.
     */
    async function showLoading(action: Func<Promise<any>>): Promise<void> {
        await showProgress('isLoading', action);
    }

    /**
     * Displays a saving indicator while executing an action.
     */
    async function showSaving(action: Func<Promise<any>>): Promise<void> {
        await showProgress('isSaving', action);
    }

    /**
     * Displays the progress indicator for a specified property while executing an action.
     * @param prop Indicator property name.
     * @param action Action to execute.
     */
    async function showProgress(
        prop: keyof (IAsyncState & T),
        action: Func<Promise<any>>
    ): Promise<void> {
        const propName = prop as string;
        (asyncState as any)[propName] = true;
        try {
            await action();
        } finally {
            (asyncState as any)[propName] = false;
        }
    }

    return {
        asyncState,
        showLoading,
        showSaving,
        showProgress
    };
}
