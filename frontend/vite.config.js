import { defineConfig, loadEnv } from 'vite';
import vue from '@vitejs/plugin-vue';
import path from 'path';
import AutoImport from 'unplugin-auto-import/vite';
import Components from 'unplugin-vue-components/vite';
import Icons from 'unplugin-icons/vite';
import IconsResolver from 'unplugin-icons/resolver';
import { ElementPlusResolver } from 'unplugin-vue-components/resolvers';
import VueI18nPlugin from '@intlify/unplugin-vue-i18n/vite';
import UnoCSS from 'unocss/vite';
import { compression } from 'vite-plugin-compression2';

process.env.BROWSER = 'chrome';

const elementPlusResolver = ElementPlusResolver({
    importStyle: 'sass',
});

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd());
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
                    'vue-i18n',
                    {
                        '~/plugins/dayjs': [
                            // import { default as dayjs } from '~/plugins/dayjs'
                            ['default', 'dayjs'],
                        ],
                    },
                ],

                // Filepath to generate corresponding .d.ts file.
                // Defaults to './auto-imports.d.ts' when `typescript` is installed locally.
                // Set `false` to disable.
                dts: true,

                dirs: ['./src/utils/*', './src/store/*'],
                resolvers: [
                    // Auto import element API
                    elementPlusResolver,
                ],
            }),
            Components({
                resolvers: [
                    // Auto register Element Plus components
                    elementPlusResolver,
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
            compression({
                threshold: 1024,
                algorithm: 'gzip',
            }),
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
                },
            },
        },
        build: {
            // sourcemap: true,
        },
        resolve: {
            alias: {
                '~/': `${path.resolve(__dirname, './src')}/`,
            },
        },
    };
});
