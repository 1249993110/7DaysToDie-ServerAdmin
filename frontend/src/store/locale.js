import { changeLang as changeDayjsLang } from '~/plugins/dayjs';
import { changeLang as changeValibotLang } from '~/plugins/valibot';
import { changeLang as changePrimevueLang } from '~/plugins//primevue';
import { usePrimeVue } from 'primevue/config';

export const useLocaleStore = defineStore('locale', () => {
    const { locale: lang, t } = useI18n();
    const primevue = usePrimeVue();

    const languageMap = {
        de: { englishName: 'German', nativeName: 'Deutsch' },
        en: { englishName: 'English', nativeName: 'English' },
        es: { englishName: 'Spanish', nativeName: 'Español' },
        fr: { englishName: 'French', nativeName: 'Français' },
        it: { englishName: 'Italian', nativeName: 'Italiano' },
        ja: { englishName: 'Japanese', nativeName: '日本語' },
        ko: { englishName: 'Korean', nativeName: '한국어' },
        pl: { englishName: 'Polish', nativeName: 'Polski' },
        'pt-br': { englishName: 'Brazilian Portuguese', nativeName: 'Português (Brasil)' },
        ru: { englishName: 'Russian', nativeName: 'Русский' },
        tr: { englishName: 'Turkish', nativeName: 'Türkçe' },
        'zh-tw': { englishName: 'Traditional Chinese', nativeName: '繁體中文' },
        'zh-cn': { englishName: 'Simplified Chinese', nativeName: '简体中文' },
    };

    watch(
        lang,
        async (val) => {
            await Promise.all([changeDayjsLang(val), changeValibotLang(val), changePrimevueLang(val, primevue)]);
        },
        { immediate: true }
    );

    const getAppTitle = () => {
        return t('common.appTitle') + ' ' + import.meta.env.VITE_APP_VERSION;
    };

    const getLanguageEnglishName = () => {
        return languageMap[lang.value].englishName;
    };

    const getLanguageNativeMap = () => {
        return Object.entries(languageMap).map(([_lang, item]) => ({
            label: item.nativeName,
            command: ({ item }) => {
                lang.value = item.value;
            },
            value: _lang,
        }));
    };

    return { lang: lang, getAppTitle, getLanguageEnglishName, getLanguageNativeMap };
});
