import { SearchMode } from "../vms/SearchMode";
import { Route } from "vue-router";
import { Func2, IObservable } from "../utils/Interfaces";
import { Observable } from "../utils/Observable";
import cloneDeep from 'lodash.cloneDeep';
import { StaticHelper } from "../utils/StaticHelper";

export interface IFilterState {
    folder: string;
    tags: number[];
    dateFrom: string;
    dateTo: string;
    searchMode: SearchMode;
    
    [key: string]: any;
}

export default class FilterStateService {
    constructor() {
        this._state = null;
        this._shareId = null;
        this._onStateChanged = new Observable<IFilterState>();
        this._onUrlChanged = new Observable<string>();
    }
    
    private _state: IFilterState;
    private _shareId: string;
    private _onStateChanged: IObservable<IFilterState>;
    private _onUrlChanged: IObservable<string>;
    
    get shareId(): string {
        return this._shareId;
    }
    
    get onStateChanged(): IObservable<IFilterState> {
        return this._onStateChanged;
    }
    
    get onUrlChanged(): IObservable<string> {
        return this._onUrlChanged;
    }
    
    get url(): string {
        const s = this._state;
        const query = StaticHelper.getQuery({
            tags: s.tags && s.tags.length ? s.tags.join(',') : null,
            dateFrom: s.dateFrom,
            dateTo: s.dateTo,
            searchMode: s.searchMode,
            sh: this.shareId
        });
        
        return query ? s + '?' + query : query;
    }

    // -----------------------------------
    // Methods
    // -----------------------------------

    /**
     * Sets the initial values from current route.
     * @param route
     */
    updateFromRoute(route: Route) {
        this._shareId = getStr('sh');
        this.update({
            folder: route.path,
            tags: getVal('tags', x => x.split(',').map(y => parseInt(y)).filter(y => y > 0)),
            dateFrom: getStr('dateFrom'),
            dateTo: getStr('dateTo'),
            searchMode: getVal('mode', x => parseInt(x))
        });
        
        function getStr(key: string): string {
            const raw = route.query[key];
            return typeof raw === 'string' ? raw : null;
        }
        
        function getVal<T>(key: string, apply: Func2<string, T>) {
            const str = getStr(key);
            return typeof str === 'string' ? apply(str) : null;
        }
    }

    /**
     * Updates the filter state.
     * Omitted values are unchanged, null values are reset.
     * @param state
     */
    update(state: Partial<IFilterState>) {
        let isChanged = false;
        const newState = cloneDeep(this._state || {}) as IFilterState;
        
        for(let key in state) {
            if(!state.hasOwnProperty(key))
                continue;
            
            const newVal = state[key];
            const oldVal = newState[key];
            if(JSON.stringify(newVal) !== JSON.stringify(oldVal)) {
                isChanged = true;
                newState[key] = newVal;
            }
        }
        
        if(!isChanged) {
            return;
        }

        this._state = newState;
        this.onStateChanged.notify(cloneDeep(newState));
        this.onUrlChanged.notify(this.url);
    }    
}