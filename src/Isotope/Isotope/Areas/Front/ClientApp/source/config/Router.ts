import Vue from 'vue';
import VueRouter from 'vue-router';

import MainView from "../components/MainView.vue";

Vue.use(VueRouter);

export default new VueRouter({
    linkActiveClass: 'active',
    linkExactActiveClass: "active",
    mode: 'history',
    routes: [
        { path: '*', component: MainView }
    ]
})