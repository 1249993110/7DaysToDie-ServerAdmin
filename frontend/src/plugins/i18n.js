import { createI18n } from 'vue-i18n';
/*
 * All i18n resources specified in the plugin `include` option can be loaded
 * at once using the import syntax
 */
import messages from '@intlify/unplugin-vue-i18n/messages';

export const supportedLanguages = {
    en: 'English',
    de: 'German',
    es: 'Spanish',
    fr: 'French',
    it: 'Italian',
    ja: 'Japanese',
    ko: 'Koreana',
    pl: 'Polish',
    'pt-br': 'Brazilian',
    ru: 'Russian',
    tr: 'Turkish',
    'zh-cn': 'Schinese',
    'zh-tw': 'Tchinese',
};

const preferredLocale = () => {
    let lang = localStorage.getItem('lang');
    if (lang) {
        return lang;
    }

    lang = navigator.language.toLowerCase();
    console.log('Navigator lang:', lang);

    if (lang === 'pt-br' || lang === 'zh-cn' || lang === 'zh-tw') {
        return lang;
    }

    lang = lang.substring(0, 2);
    if (Object.keys(supportedLanguages).includes(lang)) {
        return lang;
    }

    return 'en';
};

export const i18n = createI18n({
    locale: preferredLocale(),
    fallbackLocale: 'en',
    legacy: false,
    messages,
    globalInjection: true, // In <template> can use $t
});

watch(i18n.global.locale, (val) => {
    localStorage.setItem('lang', val);
});

export default (app) => {
    app.use(i18n);
};
