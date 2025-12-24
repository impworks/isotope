import { reactive } from 'vue';
import { useToast } from './useToast';
import type { Func } from '@common/source/utils/Interfaces';

export interface IAsyncState {
  isLoading?: boolean;
  isSaving?: boolean;
  isWorking?: boolean;
}

export function useAsyncState<T extends Record<string, any> = {}>(initial?: T) {
  const toast = useToast();

  const asyncState = reactive({
    isLoading: false,
    isSaving: false,
    isWorking: false,
    ...initial
  }) as IAsyncState & T;

  async function showLoading(action: Func<Promise<any>>, error?: string): Promise<boolean> {
    return showProgress('isLoading', action, error);
  }

  async function showSaving(action: Func<Promise<any>>, error?: string): Promise<boolean> {
    return showProgress('isSaving', action, error);
  }

  async function showWorking(action: Func<Promise<any>>, error?: string): Promise<boolean> {
    return showProgress('isWorking', action, error);
  }

  async function showProgress(
    prop: keyof (IAsyncState & T),
    action: Func<Promise<any>>,
    error?: string
  ): Promise<boolean> {
    const propName = prop as string;
    (asyncState as any)[propName] = true;

    try {
      await action();
      return true;
    } catch (e: any) {
      if (e.response?.status === 400) {
        toast.error(e.response.data.error);
      } else if (error) {
        toast.error(error, e);
      }
      return false;
    } finally {
      (asyncState as any)[propName] = false;
    }
  }

  return {
    asyncState,
    showLoading,
    showSaving,
    showWorking,
    showProgress
  };
}
