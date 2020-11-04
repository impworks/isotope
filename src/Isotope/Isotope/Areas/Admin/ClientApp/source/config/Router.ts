import Vue from 'vue';
import VueRouter from 'vue-router';
import SharedLinksPage from "../components/shared-links/SharedLinksPage.vue";
import ConfigPage from "../components/config/ConfigPage.vue";
import TagsPage from "../components/tags/TagsPage.vue";

Vue.use(VueRouter);

export default new VueRouter({
    linkActiveClass: 'active',
    linkExactActiveClass: "active",
    mode: 'history',
    routes: [
        { path: '/', redirect: '/folders' },
        { path: '/folders' },
        { path: '/media' },
        { path: '/tags', component: TagsPage },
        { path: '/shared-links', component: SharedLinksPage },
        { path: '/users' },
        { path: '/config', component: ConfigPage }
    ]
})