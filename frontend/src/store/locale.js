import { i18n, supportedLanguages } from '~/plugins/i18n';
import { setLocale as setDayjsLocale } from '~/plugins/dayjs';

export const useLocaleStore = defineStore('locale', () => {
    const locale = i18n.global.locale;
    watch(
        locale,
        (val) => {
            setDayjsLocale(val);
        },
        { immediate: true }
    );

    const getAppTitle = () => {
        return i18n.global.t('common.appTitle') + ' ' + import.meta.env.VITE_APP_VERSION;
    };

    const getLanguage = () => {
        return supportedLanguages[locale.value];
    };

    const getAvailableLocales = () => {
        return [
            { label: 'English', value: 'en' },
            { label: 'Deutsch', value: 'de' },
            { label: 'Español', value: 'es' },
            { label: 'Français', value: 'fr' },
            { label: 'Italiano', value: 'it' },
            { label: '日本語', value: 'ja' },
            { label: '한국어', value: 'ko' },
            { label: 'Polski', value: 'pl' },
            { label: 'Português (Brasil)', value: 'pt-br' },
            { label: 'Русский', value: 'ru' },
            { label: 'Türkçe', value: 'tr' },
            { label: '繁體中文', value: 'zh-tw' },
            { label: '简体中文', value: 'zh-cn' },
        ];
    };

    return { locale, getAppTitle, getLanguage, getAvailableLocales };
});
