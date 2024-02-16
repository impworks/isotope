import { resolve } from 'path';

import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue2';

export default defineConfig({
  base: '/@assets',
  root: resolve(__dirname, 'Areas/Front/ClientApp'),
  plugins: [
    vue()
  ],
  build: {
    rollupOptions: {
      input: {
        front: resolve(__dirname, 'Areas/Front/ClientApp/front.html')
      }
    },
    outDir: resolve(__dirname, 'wwwroot/@assets'),
    assetsDir: '.',
    emptyOutDir: false,
    chunkSizeWarningLimit: 1024
  }
});