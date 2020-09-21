import injector from "vue-inject";
import { UserStateService } from "../services/UserStateService";
import { ClientApiService } from "../services/ClientApiService";
import { Vue } from "vue-property-decorator";

Vue.use(injector);

injector.constant('$apiHost', process.env.API_HOST);

injector.service('$userState', UserStateService);
injector.service('$api', ['$apiHost', '$userState'], ClientApiService);