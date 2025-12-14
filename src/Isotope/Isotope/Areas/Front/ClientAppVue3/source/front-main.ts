import { createApp } from 'vue';

import Router from './config/Router';
import { registerPlugins } from './config/Plugins';
import { setupInjector } from './config/Injector';

import 'font-awesome/scss/font-awesome.scss';
import '../../../Common/styles/logotype.scss';
import '../styles/main.scss';

import Root from './components/Root.vue';

const app = createApp(Root);

// Register router
app.use(Router);

// Register plugins (includes unhead)
const { head } = registerPlugins(app);

// Setup dependency injection
setupInjector(app);

// Mount the app
app.mount('#root');
