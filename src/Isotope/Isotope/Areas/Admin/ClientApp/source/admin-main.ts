import { createApp } from 'vue';
import { createHead } from '@unhead/vue';
import router from './config/Router';
import { setupInjector } from './config/Injector';
import Root from './components/layout/Root.vue';
import { vAutofocus } from './directives/vAutofocus';

// Import styles
import '../styles/main.css';

// Create Vue app
const app = createApp(Root);

// Setup head management
const head = createHead();
app.use(head);

// Setup router
app.use(router);

// Setup dependency injection
setupInjector(app);

// Register directives
app.directive('autofocus', vAutofocus);

// Mount app
app.mount('#root');
