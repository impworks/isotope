import { toast } from '@ui/toast';

export class ToastService {
  success(message: string) {
    toast({
      title: message,
      variant: 'default',
      class: 'bg-green-50 border-green-200 text-green-900',
      description: '✓'
    });
  }

  error(message: string, error?: any) {
    if (error) {
      console.error(message, error);
    }
    toast({
      title: message,
      variant: 'destructive',
      description: '✕'
    });
  }

  info(message: string) {
    toast({
      title: message,
      variant: 'default',
      description: 'ℹ'
    });
  }

  warning(message: string) {
    toast({
      title: message,
      variant: 'default',
      class: 'bg-yellow-50 border-yellow-200 text-yellow-900',
      description: '⚠'
    });
  }
}
