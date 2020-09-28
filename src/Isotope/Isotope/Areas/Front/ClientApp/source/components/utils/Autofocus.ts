import { DirectiveOptions } from "vue";

const Autofocus: DirectiveOptions = {
    inserted(el) {
        setTimeout(() => {
            const input = el.tagName === 'INPUT' ? el : el.querySelector('input'); 
            input.focus();
        }, 100);
    }
};

export default Autofocus;