import { Vue, Component } from 'vue-property-decorator';
import { Func } from "../../../../../Common/source/utils/Interfaces";

/**
 * Default properties of a component.
 */
export interface IAsyncState {
    isLoading?: boolean;
    isSaving?: boolean;
}

export function HasAsyncState<T>(arg?: T) {
    @Component
    class HasAsyncStateClass extends Vue {
        
        constructor() {
            super();
            this.asyncState = { isLoading: false, isSaving: false, ...arg };
        }

        // -----------------------------------
        // Properties
        // -----------------------------------

        asyncState: IAsyncState & T;

        // -----------------------------------
        // Methods
        // -----------------------------------

        /**
         * Displays a loading indicator.
         */
        protected async showLoading(action: Func<Promise<any>>): Promise<void> {
            await this.showProgress('isLoading', action);
        }

        /**
         * Displays a saving indicator.
         */
        protected async showSaving(action: Func<Promise<any>>): Promise<void> {
            await this.showProgress('isSaving', action);
        }

        /**
         * Displays the progress indicator for a specified property.
         * @param action Action to execute.
         * @param prop Indicator property.
         */
        protected async showProgress(prop: keyof (IAsyncState & T), action: Func<Promise<any>>): Promise<void> {
            const propName = prop as string;
            Vue.set(this.asyncState, propName, true);
            try {
                await action();
            } finally {
                Vue.set(this.asyncState, propName, false);
            }
        }
    }
    
    return HasAsyncStateClass;
}