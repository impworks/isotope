import { DirectiveOptions } from "vue";
import { Action } from "../../../../Common/source/utils/Interfaces";

interface IClickOutsideElem extends HTMLElement {
    clickOutsideEvent: Action<Event>;
}

export function createClickOutsideDirective(): DirectiveOptions {
    return {
        bind: function (el: IClickOutsideElem, binding, vnode) {
            el.clickOutsideEvent = function (event) {
                if (!el.contains(event.target as Node)) {
                    (vnode.context as any)[binding.expression](event);
                }
            };
            document.body.addEventListener('click', el.clickOutsideEvent)
        },
        unbind: function (el: IClickOutsideElem) {
            document.body.removeEventListener('click', el.clickOutsideEvent)
        },
    }
} 