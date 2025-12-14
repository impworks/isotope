import { createRouter, createWebHistory } from 'vue-router';
import type { RouteRecordRaw } from 'vue-router';

import MainView from '../components/MainView.vue';

const routes: RouteRecordRaw[] = [
    { path: '/:pathMatch(.*)*', component: MainView }
];

export default createRouter({
    history: createWebHistory(),
    linkActiveClass: 'active',
    linkExactActiveClass: 'active',
    routes
});
