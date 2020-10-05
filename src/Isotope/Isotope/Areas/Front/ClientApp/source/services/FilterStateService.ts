import cloneDeep from 'lodash.cloneDeep';
import { SearchMode } from "../vms/SearchMode";
import { Route } from "vue-router";
import { Func2, IObservable } from "../utils/Interfaces";
import { Observable } from "../utils/Observable";
import { StaticHelper } from "../utils/StaticHelper";

export interface IFilterState {
    folder: string;
    tags: number[];
    dateFrom: Date;
    dateTo: Date;
    searchMode: SearchMode;
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
            dateFrom: fmtDate(s.dateFrom),
            dateTo: fmtDate(s.dateTo),
            searchMode: this.isEmpty(s) ? null : s.searchMode,
            sh: this.shareId
        });
        
        return s.folder + (query ? '?' + query : '') + (s.mediaKey ? '#m:' + s.mediaKey : '');
        
        function fmtDate(d: Date) {
            return d ? JSON.stringify(d).substr(1, 10) : null;
        }
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
            dateFrom: getDate('dateFrom'),
            dateTo: getDate('dateTo'),
            searchMode: getVal('searchMode', x => parseInt(x)),
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
            state = { folder: state.folder };
        }
        
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
        if(!isInitial) {
            this.onStateChanged.notify({ ...cloneDeep(newState), source: source });
            this.onUrlChanged.notify(this.url);
        }
    }

    /**
     * Clears the filter form.
     */
    clear(source: string) {
        this.update(source, { tags: null, dateFrom: null, dateTo: null, searchMode: SearchMode.CurrentFolderAndSubfolders });
    }

    /**
     * Checks if the filter is defined.
     * @param f
     */
    isEmpty(f: Partial<IFilterState>) {
        return !(f.dateTo || f.dateFrom || f.tags?.length);
    }
}