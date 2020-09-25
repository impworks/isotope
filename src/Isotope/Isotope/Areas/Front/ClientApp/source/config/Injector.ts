import injector from "vue-inject";
import { Vue } from "vue-property-decorator";
import { ApiService } from "../services/ApiService";
import { AuthService } from "../services/AuthService";
import FilterStateService from "../services/FilterStateService";

Vue.use(injector);

injector.constant('$apiHost', process.env.API_HOST);

injector.service('$auth', AuthService);
injector.service('$filter', FilterStateService);
injector.service('$api', ['$apiHost', '$auth', '$filter'], ApiService);