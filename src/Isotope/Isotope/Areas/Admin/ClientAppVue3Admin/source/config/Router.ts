import { createRouter, createWebHistory } from 'vue-router';

const routes = [
  {
    path: '/',
    redirect: '/folders'
  },
  {
    path: '/folders',
    name: 'Folders',
    component: () => import('@/components/pages/FoldersPage.vue')
  },
  {
    path: '/media',
    name: 'Media',
    component: () => import('@/components/pages/MediaPage.vue')
  },
  {
    path: '/tags',
    name: 'Tags',
    component: () => import('@/components/pages/TagsPage.vue')
  },
  {
    path: '/users',
    name: 'Users',
    component: () => import('@/components/pages/UsersPage.vue')
  },
  {
    path: '/shared-links',
    name: 'SharedLinks',
    component: () => import('@/components/pages/SharedLinksPage.vue')
  },
  {
    path: '/config',
    name: 'Config',
    component: () => import('@/components/pages/ConfigPage.vue')
  }
];

const router = createRouter({
  history: createWebHistory('/@admin'),
  routes
});

export default router;
