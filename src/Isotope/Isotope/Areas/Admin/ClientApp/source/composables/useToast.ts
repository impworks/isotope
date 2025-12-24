import { inject } from 'vue';
import { ToastServiceKey } from '@/config/Injector';
import type { ToastService } from '@/services/ToastService';

export function useToast(): ToastService {
  const toast = inject(ToastServiceKey);
  if (!toast) {
    throw new Error('ToastService not provided');
  }
  return toast;
}
