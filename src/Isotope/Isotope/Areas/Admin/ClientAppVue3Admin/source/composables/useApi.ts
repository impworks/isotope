import { inject } from 'vue';
import { ApiServiceKey } from '@/config/Injector';
import type { ApiService } from '@/services/ApiService';

export function useApi(): ApiService {
  const api = inject(ApiServiceKey);
  if (!api) {
    throw new Error('ApiService not provided');
  }
  return api;
}
