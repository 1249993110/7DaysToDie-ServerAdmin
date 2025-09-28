import Swal from 'sweetalert2';
// import { ZIndex } from '@primeuix/utils/zindex';

const { t } = useI18n();
const isDark = useDark();

export const myToast = (options = {}) => {
    Swal.fire({
        title: options.title,
        text: options.text,
        icon: options.icon || 'info',
        toast: true,
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        position: 'top',
        theme: isDark.value ? 'dark' : 'light',
    });
};

export const myConfirm = async (options = {}) => {
    const result = await Swal.fire({
        title: options.title || t('plugins.myConfirm.title'),
        text: options.text || t('plugins.myConfirm.text'),
        icon: options.icon || 'warning',
        showCancelButton: true,
        // confirmButtonColor: options.confirmButtonColor || '#3085d6',
        // cancelButtonColor: options.cancelButtonColor || '#d33',
        confirmButtonText: options.confirmButtonText || t('plugins.myConfirm.confirmButtonText'),
        cancelButtonText: options.cancelButtonText || t('plugins.myConfirm.cancelButtonText'),
        theme: isDark.value ? 'dark' : 'light',
    });

    return result.isConfirmed;
};

export const myPrompt = async (options = {}) => {
    const { value: text } = await Swal.fire({
        title: options.title || t('plugins.myPrompt.title'),
        input: 'text',
        inputLabel: options.inputLabel || t('plugins.myPrompt.inputLabel'),
        inputValue: options.inputValue || '',
        showCancelButton: true,
        confirmButtonText: options.confirmButtonText || t('plugins.myPrompt.confirmButtonText'),
        cancelButtonText: options.cancelButtonText || t('plugins.myPrompt.cancelButtonText'),
        theme: isDark.value ? 'dark' : 'light',
    });

    return text;
};