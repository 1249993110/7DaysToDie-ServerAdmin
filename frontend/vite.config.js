import { defineConfig, loadEnv } from 'vite';
import vue from '@vitejs/plugin-vue';
import path from 'path';
import AutoImport from 'unplugin-auto-import/vite';
import Components from 'unplugin-vue-components/vite';
import { PrimeVueResolver } from '@primevue/auto-import-resolver';
import Icons from 'unplugin-icons/vite';
import IconsResolver from 'unplugin-icons/resolver';
import VueI18nPlugin from '@intlify/unplugin-vue-i18n/vite';
import UnoCSS from 'unocss/vite';

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd());
    process.env.BROWSER = env.VITE_DEV_BROWSER;
    return {
        plugins: [
            vue(),
            AutoImport({
                // global imports to register
                imports: [
                    // presets
                    'vue',
                    'vue-router',
                    'pinia',
                    '@vueuse/core',
                    // 'vue-i18n',
                    {
                        '~/plugins/dayjs.js': [
                            // import { default as dayjs } from '~/plugins/dayjs.js'
                            ['default', 'dayjs'],
                        ],
                        // '~/plugins/sweetalert2.js': [
                        //     ['myToast'], ['myConfirm'],
                        // ],
                        // '~/plugins/mitt.js': [
                        //     ['default', 'emitter'],
                        //     ['EVENT_TYPES'],
                        // ],
                        '~/plugins/i18n.js': [
                            ['useI18n'],
                        ],
                    },
                ],

                // Filepath to generate corresponding .d.ts file.
                // Defaults to './auto-imports.d.ts' when `typescript` is installed locally.
                // Set `false` to disable.
                dts: true,

                dirs: ['./src/utils/*'],
                resolvers: [],
                ignore: ['h'],
            }),
            Components({
                resolvers: [
                    // Auto register PrimeVue components
                    PrimeVueResolver(),
                    // Auto register icon components
                    IconsResolver({
                        prefix: 'icon',
                    }),
                ],
                dts: true,
                globs: ['./src/components/*.vue', './src/components/*/index.vue'],
            }),
            Icons({
                // https://icones.js.org/
                autoInstall: true,
                compiler: 'vue3',
            }),
            VueI18nPlugin({
                include: [path.resolve(__dirname, './src/locales/**')],
            }),
            UnoCSS(),
        ],
        base: env.VITE_APP_PUBLIC_BASE_PATH,
        server: {
            host: env.VITE_DEV_HOST,
            port: parseInt(env.VITE_DEV_PORT),
            open: env.VITE_DEV_OPEN_BROWSER === 'true',
            proxy: {
                '/api': {
                    target: env.VITE_DEV_API_PROXY_TARGET,
                    changeOrigin: true,
                    // timeout: 0,
                    // followRedirects: true
                },
                '/swagger': {
                    target: env.VITE_DEV_API_PROXY_TARGET,
                }
            },
        },
        build: {
            // sourcemap: true,
        },
        resolve: {
            alias: {
                '~/': `${path.resolve(__dirname, 'src')}/`,
            },
        },
    };
});
