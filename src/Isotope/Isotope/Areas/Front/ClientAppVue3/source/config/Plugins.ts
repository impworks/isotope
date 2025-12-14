import type { App } from 'vue';

// -----------------------------------
// External plugins
// -----------------------------------

// import simplebar from 'simplebar-vue3';
// import 'simplebar/dist/simplebar.min.css';

import vSelect from 'vue-select';
import 'vue-select/dist/vue-select.css';

import { VueDatePicker } from '@vuepic/vue-datepicker';
import '@vuepic/vue-datepicker/dist/main.css';

// Note: simplebar-vue and vue2-hammer need Vue3 alternatives
// TODO: Add these packages when correct versions are available
// For now, we'll handle these in components that need them

import { createHead } from '@unhead/vue';

// -----------------------------------
// Custom components and directives
// -----------------------------------

import Loading from '../components/utils/Loading.vue';
import vAutofocus from '../utils/AutofocusDirective';

export function registerPlugins(app: App) {
    // Register components
    // app.component('simplebar', simplebar);
    app.component('v-select', vSelect);
    app.component('datepicker', VueDatePicker);
    app.component('loading', Loading);

    // Register directives
    app.directive('autofocus', vAutofocus);

    // Setup unhead for meta tags
    const head = createHead();
    app.use(head);

    return { head };
}
