import Vue from 'vue';

import { Plugin } from 'vue-fragment';
Vue.use(Plugin);

import PortalVue from 'portal-vue';
Vue.use(PortalVue);

import GlobalEvents from 'vue-global-events';
Vue.component('GlobalEvents', GlobalEvents);

import Loading from "../components/utils/Loading.vue";
Vue.component("loading", Loading);

import PerfectScrollbar from 'vue2-perfect-scrollbar';
Vue.use(PerfectScrollbar);

import vSelect from 'vue-select';
Vue.component('v-select', vSelect);

import Datepicker from 'vuejs-datepicker';
Vue.component('datepicker', Datepicker);

import Autofocus from "../components/utils/Autofocus";
Vue.directive('autofocus', Autofocus);