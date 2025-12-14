import { resolve } from 'path';
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue2';

export default defineConfig({
  base: '/@assets',
  root: resolve(__dirname),
  plugins: [
    vue()
  ],
  resolve: {
    alias: {
      '@': resolve(__dirname, 'source'),
      '@common': resolve(__dirname, '../../Common')
    }
  },
  build: {
    rollupOptions: {
      input: {
        admin: resolve(__dirname, 'admin.html')
      }
    },
    outDir: resolve(__dirname, '../../../wwwroot/@assets'),
    assetsDir: '.',
    emptyOutDir: false,
    chunkSizeWarningLimit: 1024
  }
});
