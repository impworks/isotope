import { Func2, ILookup } from "./Interfaces";

type KeyFunc<TItem> = (x: TItem) => number|string;

export class ArrayHelper {
    /**
     * Maps the array to a lookup using key and result getters.
     */
    public static toLookup<TItem, TResult>(arr: TItem[], keyFunc: KeyFunc<TItem>, resultFunc: Func2<TItem, TResult>): ILookup<TResult> {
        const result = { } as ILookup<TResult>;

        if(arr?.length)
            for(let item of arr)
                result[keyFunc(item)] = resultFunc(item);

        return result;
    }
    
    /**
     * Finds a tree element that matches the predicate.
     */
    static findRecursive<T>(arr: T[], predicate: Func2<T, boolean>, childGetter: Func2<T, T[]>): T {
        if(arr) {
            for (let x of arr) {
                if (predicate(x))
                    return x;

                const childResult = ArrayHelper.findRecursive(childGetter(x), predicate, childGetter);
                if(childResult)
                    return childResult;
            }
        }
        
        return null;
    }

    /**
     * Removes an item if it exists in the array.
     */
    static removeItem<T>(arr: T[], item: T): void {
        const idx = arr.indexOf(item);
        if(idx !== -1)
            arr.splice(idx, 1);
    }
    
    /**
     * Counts the number of elements matching the predicate.
     */
    static count<T>(arr: T[], predicate: Func2<T, boolean>): number {
        return arr.reduce((a, b) => a + (predicate(b) ? 1 : 0), 0);
    }
}