import Vue from 'vue';

// -----------------------------------
// External plugins
// -----------------------------------

import { Plugin } from 'vue-fragment';
Vue.use(Plugin);

import PortalVue from 'portal-vue';
Vue.use(PortalVue);

import GlobalEvents from 'vue-global-events';
Vue.component('GlobalEvents', GlobalEvents);

import simplebar from 'simplebar-vue';
import 'simplebar/dist/simplebar.min.css';
Vue.component("simplebar", simplebar);

import vSelect from 'vue-select';
Vue.component('v-select', vSelect);

import Datepicker from '@sum.cumo/vue-datepicker';
import '@sum.cumo/vue-datepicker/dist/vuejs-datepicker.css'
Vue.component('datepicker', {
    extends: Datepicker,
    props: {
        format: {
            default: 'yyyy.MM.dd'
        }
    }
});

import { PopoverPlugin } from 'bootstrap-vue';
Vue.use(PopoverPlugin);

import { VueHammer } from 'vue2-hammer'
Vue.use(VueHammer)

// -----------------------------------
// Custom components
// -----------------------------------

import Loading from "../components/utils/Loading.vue";
Vue.component("loading", Loading);

import Autofocus from "../components/utils/Autofocus";
Vue.directive('autofocus', Autofocus);