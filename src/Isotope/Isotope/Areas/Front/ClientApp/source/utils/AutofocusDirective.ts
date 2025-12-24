import type { Directive } from 'vue';

/**
 * Vue3 autofocus directive - focuses an input element when mounted.
 */
const vAutofocus: Directive = {
    mounted(el) {
        setTimeout(() => {
            const input = el.tagName === 'INPUT' ? el : el.querySelector('input');
            input?.focus();
        }, 100);
    }
};

export default vAutofocus;
