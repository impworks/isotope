import { DirectiveOptions } from "vue";

const Autofocus: DirectiveOptions = {
    inserted(el) {
        setTimeout(() => el.focus(), 100);
    }
};

export default Autofocus;