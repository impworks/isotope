import injector from "vue-inject";
import { Vue } from "vue-property-decorator";
import { AuthService } from "../../../../Common/source/services/AuthService";
import { ApiService } from "../services/ApiService";

Vue.use(injector);

injector.constant('$host', process.env.API_HOST || '');
injector.service('$auth', AuthService);
injector.service('$api', ['$host', '$auth'], ApiService);