import { resolve } from 'path';

import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue2';

export default defineConfig({
  base: '/@assets',
  root: resolve(__dirname, 'Areas/Admin/ClientApp'),
  plugins: [
    vue()
  ],
  build: {
    rollupOptions: {
      input: {
        admin: resolve(__dirname, 'Areas/Admin/ClientApp/admin.html')
      }
    },
    outDir: resolve(__dirname, 'wwwroot/@assets'),
    assetsDir: '.',
    emptyOutDir: false,
    chunkSizeWarningLimit: 1024
  }
});