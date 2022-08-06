import { DirectiveOptions } from "vue";
import { EventHelper } from "../../../../Common/source/utils/EventHelper";

export function createBurstSelectionDirective(): DirectiveOptions {
    return {
        bind (el, binding, vnode) {
            vnode.componentInstance.$on('option:selected', () => {
                const input = el.querySelector('input[type="search"]') as HTMLElement;
                setTimeout(() => EventHelper.sendMouseClick(input), 100);
            });
        }
    };
}