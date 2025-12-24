import { Func2 } from "./Interfaces";
import { cloneDeep } from 'lodash';

export class StaticHelper {

    static nameOf<T>(descriptor: Func2<T, any>): keyof T;

    /**
     * Returns the name of the member captured by a function.
     * @param descriptor Function on the form of x => x.Prop.
     */
    static nameOf(descriptor: Function): string {
        const text = descriptor.toString();
        const rx = /return (.+)[;}]/m;
        const match = rx.exec(text);

        if (match.length === 0)
            return null;

        const parts = match[1].trim().split('.');
        return parts[parts.length - 1];
    }

    /**
     * Returns the names of members captured by functions.
     * @param descriptors Functions in the form of x => x.Prop.
     */
    static namesOf<T>(...descriptors: Func2<T, any>[]): (keyof T)[] {
        return descriptors.map(x => StaticHelper.nameOf(x));
    }

    /**
     * Checks if the object is any of the primitive types.
     * @param obj Object to check.
     */
    static isPrimitive(obj: any): boolean {
        if (typeof obj === "string" || typeof obj === "number" || typeof obj === "boolean")
            return true;

        if (obj instanceof Date)
            return true;

        return false;
    }

    /**
     * Maps the objects' properties to a GET query.
     * @param argSlices Property containers. 
     */
    static getQuery(...argSlices: any[]): string {
        let obj = {} as { [key: string]: any };
        for (let slice of argSlices)
            obj = { ...obj, ...slice };

        const parts: string[] = [];
        for (let pty in obj) {
            if (!obj.hasOwnProperty(pty)) continue;

            let value = obj[pty];
            if (typeof value === 'undefined' || value === null) continue;

            // special case for date: format it like in JSON
            if (value instanceof Date) {
                value = JSON.parse(JSON.stringify({ x: value })).x;
            }

            parts.push(encodeURIComponent(pty) + "=" + encodeURIComponent(value + ''));
        }

        return parts.join('&');
    }

    /**
     * Creates a URL query from an object and a list of property names in it.
     * @param obj Property container.
     * @param descriptors List of properties to include.
     */
    static getQueryPartial<T>(obj: T, ...descriptors: Func2<T, any>[]): string {
        return StaticHelper.getQuery(StaticHelper.getSubset(obj, ...descriptors));
    }

    /**
     * Returns a subset of object properties specified by the list of 
     * @param obj Original object.
     * @param descriptors List of object's properties to use.
     */
    static getSubset<T>(obj: T, ...descriptors: Func2<T, any>[]): any {
        const subset = {} as any;

        for (let desc of descriptors) {
            const key = StaticHelper.nameOf(desc);
            subset[key as string] = obj[key];
        }

        return subset;
    }

    /**
     * Creates a comple copy of the object.
     * @param obj Object to copy.
     */
    static deepCopy<T>(obj: T): T {
        return cloneDeep(obj);
    }

    /**
     * Returns the keys of the object that are different from the reference value.
     */
    static diff<T>(reference: T, obj: T): any {
        
        function equal(a: any, b: any) {
            if(a === b)
                return true;
            if(Array.isArray(a) && a.length === 0 && Array.isArray(b) && b.length === 0)
                return true;
            return false;
        }
        
        const result = {} as any;
        
        for(let key in obj) {
            if(!obj.hasOwnProperty(key))
                continue;
            
            if(equal(obj[key], reference[key]))
                continue;
            
            result[key] = obj[key];
        }
        
        return result;
    }
}