import { IObservable } from "../../../../Common/source/utils/Interfaces";
import { Observable } from "../../../../Common/source/utils/Observable";

export class EventBusService {
    constructor() {
        this.uiUpdated = new Observable<void>();
    }
    
    uiUpdated: IObservable<void>;    
}