import Vue from 'vue';
import VueRouter from 'vue-router';
import ConfigPage from "../components/config/ConfigPage.vue";

Vue.use(VueRouter);

export default new VueRouter({
    linkActiveClass: 'active',
    linkExactActiveClass: "active",
    mode: 'history',
    routes: [
        { path: '/', redirect: '/folders' },
        { path: '/folders' },
        { path: '/media' },
        { path: '/tags' },
        { path: '/shared-links' },
        { path: '/users' },
        { path: '/config', component: ConfigPage }
    ]
})