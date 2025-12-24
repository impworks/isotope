import { resolve } from 'path';
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

export default defineConfig({
  base: '/@assets',
  root: resolve(__dirname),
  plugins: [
    vue()
  ],
  resolve: {
    alias: {
      '@': resolve(__dirname, 'source'),
      '@common': resolve(__dirname, '../../../Common'),
      'luxon': resolve(__dirname, 'node_modules/luxon'),
      'lodash': resolve(__dirname, 'node_modules/lodash')
    }
  },
  build: {
    rollupOptions: {
      input: {
        front: resolve(__dirname, 'front.html')
      }
    },
    outDir: resolve(__dirname, '../../../wwwroot/@assets'),
    assetsDir: '.',
    emptyOutDir: false,
    chunkSizeWarningLimit: 1024
  }
});
