import Vue from 'vue';

import "./config/Plugins";
import Router from './config/Router';

import Root from './components/Root.vue';

new Vue({
    router: Router,
    render: h => h(Root)
}).$mount('#root');

import '../styles/main.scss';