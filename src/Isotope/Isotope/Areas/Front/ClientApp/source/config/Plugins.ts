import Vue from 'vue';

import { Plugin } from 'vue-fragment';
Vue.use(Plugin);

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