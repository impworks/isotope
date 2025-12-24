import { inject } from 'vue';
import { AuthServiceKey } from '@/config/Injector';
import type { AuthService } from '@common/source/services/AuthService';

export function useAuth(): AuthService {
  const auth = inject(AuthServiceKey);
  if (!auth) {
    throw new Error('AuthService not provided');
  }
  return auth;
}
