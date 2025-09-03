import * as v from 'valibot';

const locales = {
    de: () => import('@valibot/i18n/de'),
    // en: () => import('@valibot/i18n/en'),
    es: () => import('@valibot/i18n/es'),
    fr: () => import('@valibot/i18n/fr'),
    it: () => import('@valibot/i18n/it'),
    ja: () => import('@valibot/i18n/ja'),
    ko: () => import('@valibot/i18n/kr'),
    pl: () => import('@valibot/i18n/pl'),
    'pt-br': () => import('@valibot/i18n/pt'),
    ru: () => import('@valibot/i18n/ru'),
    tr: () => import('@valibot/i18n/tr'),
    'zh-cn': () => import('@valibot/i18n/zh-CN'),
    'zh-tw': () => import('@valibot/i18n/zh-TW'),
};

// Set the language configuration globally
export const changeLang = async (lang) => {
    const loader = locales[lang];
    if (loader) {
        await loader();

        const langMap = {
            kr: 'ko',
            'pt-br': 'pt',
            'zh-cn': 'zh-CN',
            'zh-tw': 'zh-TW',
        };
        v.setGlobalConfig({ lang: langMap[lang] ?? lang });
    } else {
        v.setGlobalConfig({ lang: 'en' });
    }
};

export default v;
