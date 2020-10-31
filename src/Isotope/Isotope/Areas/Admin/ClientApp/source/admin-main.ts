import Vue from 'vue';

import "./config/Plugins";
import "./config/Injector";
import Router from './config/Router';

import '../../../../node_modules/toastr/toastr.scss';
import '../styles/main.scss';

import Root from './components/Root.vue';
new Vue({
    router: Router,
    render: h => h(Root)
}).$mount('#root');