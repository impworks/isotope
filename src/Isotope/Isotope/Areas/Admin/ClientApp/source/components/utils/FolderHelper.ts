import { FolderTitle } from "../../vms/FolderTitle";

export class FolderHelper {
    /**
     * Returns the flat list of elements from a tree.
     */
    static flatten(list: NestedFolderTitle[], key: string): NestedFolderTitle[] {
        const result: NestedFolderTitle[] = [];
        applyFlatten(list, '');
        return result;

        function applyFlatten(list: NestedFolderTitle[], prefix: string) {
            if(!list || !list.length)
                return;

            for(let elem of list) {
                elem.prefix = prefix;
                result.push(elem);
                const isCurrent = elem.key === key;
                elem.selectable = !isCurrent;
                if(elem.subfolders) {
                    elem.selectable = !isCurrent && !elem.subfolders.some(x => x.key == key);
                    if(!isCurrent)
                        applyFlatten(elem.subfolders, prefix + '―');
                }
            }
        }
    }
}

export interface NestedFolderTitle extends FolderTitle {
    prefix?: string;
    selectable?: boolean;
}