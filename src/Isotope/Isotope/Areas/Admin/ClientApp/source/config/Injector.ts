import injector from "vue-inject";
import { Vue } from "vue-property-decorator";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { ApiService } from "../services/ApiService";
import { ToastService } from "../services/ToastService";

Vue.use(injector);

injector.constant('$host', process.env.API_HOST || '');
injector.service('$toast', ToastService);
injector.service('$auth', AuthService);
injector.service('$api', ['$host', '$auth'], ApiService);

Vue.mixin({
    beforeCreate() {
        (this as any)['$toast'] = injector.get('$toast');
    }
});

declare module 'vue/types/vue' {
    interface Vue {
        readonly $toast: ToastService;
    }
}
