import Vue from 'vue';
import VueRouter from 'vue-router';
import SharedLinksPage from "../components/shared-links/SharedLinksPage.vue";
import ConfigPage from "../components/config/ConfigPage.vue";
import TagsPage from "../components/tags/TagsPage.vue";
import UsersPage from "../components/users/UsersPage.vue";
import FoldersPage from "../components/folders/FoldersPage.vue";
import MediaPage from "../components/media/MediaPage.vue";

Vue.use(VueRouter);

export default new VueRouter({
    linkActiveClass: 'active',
    linkExactActiveClass: "active",
    mode: 'history',
    routes: [
        { path: '/', redirect: '/folders' },
        { path: '/folders', component: FoldersPage },
        { path: '/folders/:key', component: MediaPage },
        { path: '/tags', component: TagsPage },
        { path: '/shared-links', component: SharedLinksPage },
        { path: '/users', component: UsersPage },
        { path: '/config', component: ConfigPage }
    ]
})