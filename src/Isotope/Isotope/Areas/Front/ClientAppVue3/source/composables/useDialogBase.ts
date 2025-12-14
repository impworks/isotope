import { ref, watch } from 'vue';

/**
 * Composable that provides dialog visibility state management.
 * Replaces the Vue2 DialogBase class component.
 *
 * @param modelValue - The v-model value from parent
 * @param emit - The emit function from setup
 * @returns Object with isVisible ref and close function
 */
export function useDialogBase(
  modelValue: { value: boolean },
  emit: (event: 'update:modelValue', value: boolean) => void
) {
  const isVisible = ref(false);

  // Initialize from model value
  isVisible.value = modelValue.value;

  // Watch for external changes to model
  watch(modelValue, (newValue) => {
    isVisible.value = newValue;
  });

  // Watch for internal changes to visibility
  watch(isVisible, (newValue) => {
    emit('update:modelValue', newValue);
  });

  function close() {
    isVisible.value = false;
  }

  return {
    isVisible,
    close
  };
}
