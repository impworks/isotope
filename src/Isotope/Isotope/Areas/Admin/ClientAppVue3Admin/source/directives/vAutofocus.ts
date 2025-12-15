import type { Directive } from 'vue';

export const vAutofocus: Directive = {
  mounted(el: HTMLElement) {
    const input = el.tagName === 'INPUT' || el.tagName === 'TEXTAREA'
      ? el as HTMLInputElement
      : el.querySelector('input, textarea') as HTMLInputElement;

    if (input) {
      setTimeout(() => input.focus(), 100);
    }
  }
};
