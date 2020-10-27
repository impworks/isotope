import injector from "vue-inject";
import { Vue } from "vue-property-decorator";

Vue.use(injector);

injector.constant('$host', process.env.API_HOST || '');