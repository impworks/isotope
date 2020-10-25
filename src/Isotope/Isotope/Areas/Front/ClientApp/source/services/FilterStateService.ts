import cloneDeep from 'lodash.cloneDeep';
import { SearchScope } from "../vms/SearchScope";
import { Route } from "vue-router";
import { Func2, IObservable } from "../utils/Interfaces";
import { Observable } from "../utils/Observable";
import { StaticHelper } from "../utils/StaticHelper";
import { DateHelper } from "../utils/DateHelper";

export interface IFilterState {
    folder: string;
    tags: number[];
    from: Date;
    to: Date;
    scope: SearchScope;
    mediaKey: string;
    
    [key: string]: any;
}

export interface IFilterStateChangedEvent extends IFilterState {
    source: any;
}

export class FilterStateService {
    constructor() {
        this._state = null;
        this._shareId = null;
        this._onStateChanged = new Observable<IFilterStateChangedEvent>();
        this._onUrlChanged = new Observable<string>();
    }
    
    private _state: IFilterState;
    private _shareId: string;
    private _onStateChanged: IObservable<IFilterStateChangedEvent>;
    private _onUrlChanged: IObservable<string>;
    
    get shareId(): string {
        return this._shareId;
    }
    
    get onStateChanged(): IObservable<IFilterStateChangedEvent> {
        return this._onStateChanged;
    }
    
    get onUrlChanged(): IObservable<string> {
        return this._onUrlChanged;
    }
    
    get state(): IFilterState {
        return cloneDeep(this._state);
    }
    
    get url(): string {
        const s = this._state;
        const query = StaticHelper.getQuery({
            tags: s.tags && s.tags.length ? s.tags.join(',') : null,
            from: DateHelper.format(s.from),
            to: DateHelper.format(s.to),
            scope: this.isEmpty(s) ? null : s.scope,
            sh: this.shareId
        });
        
        return s.folder + (query ? '?' + query : '') + (s.mediaKey ? '#m:' + s.mediaKey : '');
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
        this.update(null, {
            folder: route.path,
            tags: getVal('tags', x => x.split(',').map(y => parseInt(y)).filter(y => y > 0)),
            from: getDate('from'),
            to: getDate('to'),
            scope: getVal('scope', x => parseInt(x)) || SearchScope.CurrentFolderAndSubfolders,
            mediaKey: route.hash?.startsWith('#m:') ? route.hash.substr(3) : null
        });
        
        function getStr(key: string): string {
            const raw = route.query[key];
            return typeof raw === 'string' ? raw : null;
        }
        
        function getVal<T>(key: string, apply: Func2<string, T>) {
            const str = getStr(key);
            return typeof str === 'string' ? apply(str) : null;
        }
        
        function getDate(key: string) {
            const str = getStr(key);
            return typeof str === 'string' && /^[0-9]{4}-[0-9]{2}-[0-9]{2}$/.test(str) ? new Date(str) : null; 
        }
    }

    /**
     * Updates the filter state.
     * Omitted values are unchanged, null values are reset.
     */
    update(source: any, state: Partial<IFilterState>) {
        let isChanged = false;
        const isInitial = this._state === null;
        const newState = cloneDeep(this._state || {}) as IFilterState;
        
        if(this._shareId) {
            delete state.tags;
            delete state.from;
            delete state.to;
            delete state.scope;
        }
        
        for(let key in state) {
            if(!state.hasOwnProperty(key))
                continue;
            
            const newVal = state[key];
            const oldVal = newState[key];
            
            if(typeof newVal === "undefined")
                continue;            
            
            if(JSON.stringify(newVal) !== JSON.stringify(oldVal)) {
                isChanged = true;
                newState[key] = newVal;
            }
        }
        
        if(!isChanged)
            return;

        this._state = newState;
        if(!isInitial) {
            this.onStateChanged.notify({ ...cloneDeep(newState), source: source });
            this.onUrlChanged.notify(this.url);
        }
    }

    /**
     * Clears the filter form.
     */
    clear(source: string) {
        this.update(source, { tags: null, dateFrom: null, dateTo: null, searchMode: SearchScope.CurrentFolderAndSubfolders });
    }

    /**
     * Checks if the filter is defined.
     * @param f
     */
    isEmpty(f: Partial<IFilterState>) {
        return !f || !(f.dateTo || f.dateFrom || f.tags?.length);
    }
}