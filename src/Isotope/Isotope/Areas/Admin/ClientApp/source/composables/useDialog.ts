import { ref, watch } from 'vue';

export function useDialog<TResult = boolean>() {
  const isOpen = ref(false);
  let resolvePromise: ((value: TResult) => void) | null = null;

  watch(isOpen, (value) => {
    if (value) {
      document.body.classList.add('dialog-open');
    } else {
      document.body.classList.remove('dialog-open');
    }
  });

  function open(): Promise<TResult> {
    isOpen.value = true;
    return new Promise<TResult>((resolve) => {
      resolvePromise = resolve;
    });
  }

  function close(result: TResult) {
    isOpen.value = false;
    if (resolvePromise) {
      resolvePromise(result);
      resolvePromise = null;
    }
  }

  return {
    isOpen,
    open,
    close
  };
}
