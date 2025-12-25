import { resolve } from 'path';
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

export default defineConfig({
  base: '/@assets',
  root: resolve(__dirname),
  plugins: [
    vue()
  ],
  css: {
    preprocessorOptions: {
      scss: {
        api: 'modern-compiler'
      }
    }
  },
  resolve: {
    alias: {
      '@': resolve(__dirname, 'source'),
      '@common': resolve(__dirname, '../../Common'),
      '@ui': resolve(__dirname, 'source/components/ui'),
      'luxon': resolve(__dirname, 'node_modules/luxon'),
      'lodash': resolve(__dirname, 'node_modules/lodash')
    }
  },
  build: {
    rollupOptions: {
      input: {
        admin: resolve(__dirname, 'admin.html')
      },
      output: {
        // Consolidate all JS into a single file
        manualChunks: undefined,
        inlineDynamicImports: true
      }
    },
    outDir: resolve(__dirname, '../../../wwwroot/@assets'),
    assetsDir: '.',
    emptyOutDir: false,
    chunkSizeWarningLimit: 1024,
    // Put all CSS in one file
    cssCodeSplit: false
  }
});
