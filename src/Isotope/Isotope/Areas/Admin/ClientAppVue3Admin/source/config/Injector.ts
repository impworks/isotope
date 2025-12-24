import type { App, InjectionKey } from 'vue';
import axios from 'axios';
import { ApiService } from '../services/ApiService';
import { ToastService } from '../services/ToastService';
import { AuthService } from '@common/source/services/AuthService';

// Injection keys for type-safe inject
export const ApiServiceKey: InjectionKey<ApiService> = Symbol('$api');
export const AuthServiceKey: InjectionKey<AuthService> = Symbol('$auth');
export const ToastServiceKey: InjectionKey<ToastService> = Symbol('$toast');
export const HostKey: InjectionKey<string> = Symbol('$host');

/**
 * Sets up axios interceptor to handle 401 (Unauthorized) responses.
 * Redirects to root/login page when the API token is invalid or expired.
 */
function setupAxiosInterceptor(authService: AuthService) {
  axios.interceptors.response.use(
    response => response,
    error => {
      if (error.response?.status === 401) {
        // Clear auth state and redirect to login
        authService.user = null;
        window.location.href = '/';
      }
      return Promise.reject(error);
    }
  );
}

/**
 * Registers services for dependency injection in Vue3.
 */
export function setupInjector(app: App) {
  const host = '';

  // Create service instances
  const authService = new AuthService();
  const toastService = new ToastService();
  const apiService = new ApiService(host, authService);

  // Setup axios interceptor for 401 handling
  setupAxiosInterceptor(authService);

  // Provide services globally
  app.provide(HostKey, host);
  app.provide(AuthServiceKey, authService);
  app.provide(ToastServiceKey, toastService);
  app.provide(ApiServiceKey, apiService);

  return {
    host,
    authService,
    toastService,
    apiService
  };
}
