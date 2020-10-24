import { Func2 } from "./Interfaces";

export class ArrayHelper {
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
}