import { DirectiveOptions } from "vue/types/options";

export function createActionRowDirective(): DirectiveOptions {
    function toggle(this: HTMLElement, state: boolean) {
        this.querySelectorAll('.hover-action').forEach(e => e.classList.toggle('hidden', !state))
    }
    function show(this: HTMLElement) {
        toggle.apply(this, [true]);
    }
    function hide(this: HTMLElement) {
        toggle.apply(this, [false]);
    }
    return {
        bind(el: HTMLElement) {
            hide.apply(el);
            el.addEventListener('mouseenter', show);
            el.addEventListener('mouseleave', hide);
        },
        unbind(el: HTMLElement) {
            el.removeEventListener('mouseenter', show);
            el.removeEventListener('mouseleave', hide);
        }
    };
}