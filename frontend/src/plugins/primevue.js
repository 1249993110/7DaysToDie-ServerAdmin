import PrimeVue from 'primevue/config';
import FocusTrap from 'primevue/focustrap';
import StyleClass from 'primevue/styleclass';
import Aura from '@primeuix/themes/aura';
import Lara from '@primeuix/themes/lara';
import Material from '@primeuix/themes/material';
import Nora from '@primeuix/themes/nora';

export const themePresets = {
    'aura': Aura,
    'material': Material,
    'lara': Lara,
    'nora': Nora,
};

export default (app) => {
    const { theme, isRippleActive } = useAppStore();
    app.use(PrimeVue, {
        theme: {
            preset: themePresets[theme],
            options: {
                darkModeSelector: '.dark',
            },
        },
        ripple: isRippleActive,
    });
    app.directive('styleclass', StyleClass);
    app.directive('focustrap', FocusTrap);
};

const locales = {
    de: () => import('primelocale/js/de.js'),
    en: () => import('primelocale/js/en.js'),
    es: () => import('primelocale/js/es.js'),
    fr: () => import('primelocale/js/fr.js'),
    it: () => import('primelocale/js/it.js'),
    ja: () => import('primelocale/js/ja.js'),
    ko: () => import('primelocale/js/ko.js'),
    pl: () => import('primelocale/js/pl.js'),
    'pt-br': () => import('primelocale/js/pt.js'),
    ru: () => import('primelocale/js/ru.js'),
    tr: () => import('primelocale/js/tr.js'),
    'zh-cn': () => import('primelocale/js/zh_CN.js'),
    'zh-tw': () => import('primelocale/js/zh_TW.js'),
};

export const changeLang = async (lang, primevue) => {
    const loader = locales[lang] || locales.en;
    const preset = await loader();
    primevue.config.locale = Object.values(preset)[0];
};
