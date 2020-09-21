export interface Action<T> {
    (value: T): void;
}

export interface Func<T> {
    (): T;
}

export interface Func2<T, T2> {
    (x: T): T2;
}

export interface IDisposable {
    dispose(): void;
}

export interface IObservable<T> extends IDisposable {
    subscribe: (x: Action<T>) => IDisposable;
    subscribeOnce: (x: Action<T>) => void;
    notify: (x: T) => void;
}

export interface ILookup<T> {
    [key: string]: T;
}