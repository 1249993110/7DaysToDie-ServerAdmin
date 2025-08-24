import PrimeVue from 'primevue/config';
import Aura from '@primeuix/themes/aura';
import StyleClass from 'primevue/styleclass';

export default (app) => {
    app.use(PrimeVue, {
        theme: {
            preset: Aura,
            options: {
                darkModeSelector: '.dark',
            },
        },
        ripple: true,
    });
    app.directive('styleclass', StyleClass);
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
    const load = locales[lang] || locales.en;
    const preset = await load();
    primevue.config.locale = Object.values(preset)[0];
};
