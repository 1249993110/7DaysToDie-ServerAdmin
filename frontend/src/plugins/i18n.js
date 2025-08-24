import { createI18n } from 'vue-i18n';
/*
 * All i18n resources specified in the plugin `include` option can be loaded
 * at once using the import syntax
 */
import messages from '@intlify/unplugin-vue-i18n/messages';

const preferredLocale = () => {
    let lang = localStorage.getItem('lang');
    if (lang) {
        return lang;
    }

    lang = navigator.language.toLowerCase();
    console.log('Navigator lang:', lang);

    for (const i of Object.keys(messages)) {
        if (i === lang || i === lang.substring(0, 2)) {
            return i;
        }
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

watch(i18n.global.locale, (lang) => {
    localStorage.setItem('lang', lang);
});

export default (app) => {
    app.use(i18n);
};
