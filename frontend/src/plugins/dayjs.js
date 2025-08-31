import dayjs from 'dayjs';
import isLeapYear from 'dayjs/plugin/isLeapYear'; // import plugin
import duration from 'dayjs/plugin/duration'; // import plugin
import relativeTime from 'dayjs/plugin/relativeTime'; // import plugin

const locales = {
    de: () => import('dayjs/locale/de'),
    en: () => import('dayjs/locale/en'),
    es: () => import('dayjs/locale/es'),
    fr: () => import('dayjs/locale/fr'),
    it: () => import('dayjs/locale/it'),
    ja: () => import('dayjs/locale/ja'),
    ko: () => import('dayjs/locale/ko'),
    pl: () => import('dayjs/locale/pl'),
    'pt-br': () => import('dayjs/locale/pt'),
    ru: () => import('dayjs/locale/ru'),
    tr: () => import('dayjs/locale/tr'),
    'zh-cn': () => import('dayjs/locale/zh-cn'),
    'zh-tw': () => import('dayjs/locale/zh-tw'),
};

// use plugin
dayjs.extend(isLeapYear);
dayjs.extend(duration);
dayjs.extend(relativeTime);

const defaultFormat = 'YYYY-MM-DD HH:mm:ss';
dayjs.extend((option, dayjsClass, dayjsFactory) => {
    const oldFormat = dayjsClass.prototype.format;
    dayjsClass.prototype.format = function (formatString) {
        return oldFormat.bind(this)(formatString ?? defaultFormat);
    };
});

export const changeLang = async (lang) => {
    const loader = locales[lang] || locales.en;
    const preset = await loader();
    dayjs.locale(preset);
};

export default dayjs;
