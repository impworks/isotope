import type { App, InjectionKey } from 'vue';
import { ApiService } from '../services/ApiService';
import { FilterStateService } from '../services/FilterStateService';
import { AuthService } from '../../../../Common/source/services/AuthService';
import { EventBusService } from '../services/EventBusService';

// Injection keys for type-safe inject
export const ApiServiceKey: InjectionKey<ApiService> = Symbol('$api');
export const AuthServiceKey: InjectionKey<AuthService> = Symbol('$auth');
export const FilterStateServiceKey: InjectionKey<FilterStateService> = Symbol('$filter');
export const EventBusServiceKey: InjectionKey<EventBusService> = Symbol('$eventBus');
export const HostKey: InjectionKey<string> = Symbol('$host');

/**
 * Registers services for dependency injection in Vue3.
 */
export function setupInjector(app: App) {
    const host = import.meta.env.API_HOST || '';

    // Create service instances
    const authService = new AuthService();
    const filterService = new FilterStateService();
    const eventBusService = new EventBusService();
    const apiService = new ApiService(host, authService, filterService);

    // Provide services globally
    app.provide(HostKey, host);
    app.provide(AuthServiceKey, authService);
    app.provide(FilterStateServiceKey, filterService);
    app.provide(EventBusServiceKey, eventBusService);
    app.provide(ApiServiceKey, apiService);

    // For backwards compatibility, also attach to globalProperties
    // This allows components to use this.$api, this.$auth, etc.
    app.config.globalProperties.$host = host;
    app.config.globalProperties.$auth = authService;
    app.config.globalProperties.$filter = filterService;
    app.config.globalProperties.$eventBus = eventBusService;
    app.config.globalProperties.$api = apiService;

    return {
        host,
        authService,
        filterService,
        eventBusService,
        apiService
    };
}
