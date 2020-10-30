import Vue, { ComponentOptions } from "vue";
import { createDecorator } from "vue-class-component";
import { ILookup } from "./Interfaces";

export function Dep(alias: string = null) {
    
    function getDeps(opts: ComponentOptions<Vue>): ILookup<string> {
        let result = {} as ILookup<string>;
        const d = opts.dependencies;
        
        if(typeof d === "string") {
            result[d] = d;
        } else if (Array.isArray(d)) {
            for(let key of d)
                result[key] = key;
        } else if(typeof d !== "undefined") {
            result = d;
        }
        
        opts.dependencies = result;
        return result;
    }
    
    return createDecorator((opts, key) => {
        const deps = getDeps(opts);
        deps[alias || key] = key;
    });
}

