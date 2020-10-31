import injector from "vue-inject";
import { Vue } from "vue-property-decorator";
import { ApiService } from "../services/ApiService";
import { FilterStateService } from "../services/FilterStateService";
import { AuthService } from "../../../../Common/source/services/AuthService";

Vue.use(injector);

injector.constant('$host', process.env.API_HOST || '');

injector.service('$auth', AuthService);
injector.service('$filter', FilterStateService);
injector.service('$api', ['$host', '$auth', '$filter'], ApiService);