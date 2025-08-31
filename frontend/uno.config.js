import { defineConfig } from 'unocss';
import presetWind4 from '@unocss/preset-wind4';
import transformerDirectives from '@unocss/transformer-directives';

export default defineConfig({
    presets: [
        presetWind4({
            preflights: {
                reset: true,
            },
            dark: 'class',
        }),
    ],
    transformers: [
        transformerDirectives({
            applyVariable: ['--uno'],
        }),
    ],
    shortcuts: [
        {
            'f-center': 'flex items-center justify-center',
            // 'f-c-center': 'flex flex-col items-center justify-center',
            // 'f-x-center': 'flex justify-center',
            // 'f-y-center': 'flex items-center',

            // theme colors
            't-bg-1': 'bg-p-surface-50 dark:bg-p-surface-950',
            't-bg-2': 'bg-p-surface-100 dark:bg-p-surface-900',
            't-bg-4': 'bg-p-surface-200 dark:bg-p-surface-800',
            't-bg-5': 'bg-p-surface-300 dark:bg-p-surface-700',
            't-bg-6': 'bg-p-surface-400 dark:bg-p-surface-600',
            't-bg-7': 'bg-p-surface-500 dark:bg-p-surface-500',
            't-bg-8': 'bg-p-surface-600 dark:bg-p-surface-400',
            't-bg-9': 'bg-p-surface-700 dark:bg-p-surface-300',
            't-bg-10': 'bg-p-surface-800 dark:bg-p-surface-200',
            't-bg-11': 'bg-p-surface-900 dark:bg-p-surface-100',
            't-bg-12': 'bg-p-surface-950 dark:bg-p-surface-50',

            't-border-color-1': 'border-p-surface-200 dark:border-p-surface-700',
            't-border-color-2': 'border-p-surface-300 dark:border-p-surface-600',
            't-border-color-3': 'border-p-surface-400 dark:border-p-surface-500',
            't-border-color-4': 'border-p-surface-500 dark:border-p-surface-400',

            't-bg-overlay': 'bg-overlay dark:bg-overlay-dark backdrop-blur-md',
            't-border-card': 'border border-solid t-border-color-1',

            't-text-primary': 'text-p-primary-color',
            't-text-regular': 'text-p-text-color',
            't-text-secondary': 'text-p-text-muted-color',
        },
    ],
    include: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
    theme: {
        colors: {
            overlay: {
                DEFAULT: 'hsla(0, 0%, 100%, 0.7)',
                dark: 'rgba(0, 0, 0, 0.3)',
            },
        },
    },
    extendTheme: (theme) => {
        return {
            ...theme,
            breakpoint: {
                ...theme.breakpoint,
                '3xl': '112rem',
                '4xl': '128rem',
            },
        };
    },
    rules: [
        [
            /^(text|bg|border)-p-(.*)$/,
            ([, type, color], { theme }) => {
                const properties = {
                    text: 'color',
                    bg: 'background-color',
                    border: 'border-color',
                };

                const property = properties[type];
                if (!property) return;

                return { [property]: 'var(--p-' + color + ')' };
            },
        ],
    ],
});
