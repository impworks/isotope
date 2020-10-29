declare module "vue-inject" {
    interface GenericConstructor<T> {
        new(...args: any[]): T;
    }

    interface Injector {        
        constant(name: string, value: any): void;
    
        decorator(name: string, func: ObjectConstructor): void;
    
        factory<T>(name: string, ctor: GenericConstructor<T>): void;
        factory<T>(name: string, dependencies: string[], ctor: GenericConstructor<T>): void;

        service<T>(name: string, ctor: GenericConstructor<T>): void;
        service<T>(name: string, dependencies: string[], ctor: GenericConstructor<T>): void;
    
        get<T>(name: string, dependencies?: { [key: string]: string }): T;
    
        reset(): void;
        clearCache(): void;
    
        spawn(extend: boolean): Injector;

        install: Vue.PluginFunction<any>;
        [key: string]: any;
    }

    var injector: Injector;
    export default injector;
}