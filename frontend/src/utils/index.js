export const markIcon = (source) => {
    return markRaw(defineAsyncComponent(source));
};

/**
 * Converts a number of bytes to megabytes (MB).
 * @param {number} bytes - The number of bytes to convert.
 * @param {number} [decimalPlaces=0] - The number of decimal places to retain in the result, defaulting to 0.
 * @returns {number} The number of megabytes after conversion.
 */
export const bytesToMB = (bytes, decimalPlaces = 0) => {
    // Ensure the input is a valid number
    if (typeof bytes !== 'number' || isNaN(bytes)) {
        return 0;
    }

    const megabytes = bytes / 1048576; // 1 MB = 1024 * 1024 bytes

    // Use the toFixed() method to control the number of decimal places
    return Number(megabytes.toFixed(decimalPlaces));
};

export const formatPosition = (position) => {
    if (!position) return '';
    return `${position.x}, ${position.y}, ${position.z}`;
};

export const formatMinute = (totalMinute) => {
    const { t } = useI18n();
    if (totalMinute < 1) {
        return `${t('common.lessThan')} 1 ${t('common.minute')}`;
    }

    const day = parseInt(totalMinute / 60 / 24);
    const hour = parseInt((totalMinute / 60) % 24);
    const minute = parseInt(totalMinute % 60);
    let result = '';
    if (day > 0) {
        result = day + ` ${t('common.day', day)} `;
    }
    if (hour > 0) {
        result += hour + ` ${t('common.hour', hour)} `;
    }
    if (minute > 0) {
        result += minute + ` ${t('common.minute', minute)} `;
    }
    return result;
};

export const searchByKeyword = (data, keyword, fields = []) => {
    if (!keyword || !keyword.trim()) {
        return data;
    }

    const regex = new RegExp(keyword, 'i');

    return data.filter((item) => {
        const keysToSearch = fields.length ? fields : Object.keys(item);

        return keysToSearch.some((field) => {
            return item[field] != null && regex.test(item[field].toString());
        });
    });
};

export const orderByField = (data, field, desc) => {
    if (!field) {
        return data;
    }
    return data.sort((a, b) => {
        if (!desc) {
            return a[field] > b[field] ? 1 : -1;
        } else if (desc) {
            return a[field] < b[field] ? 1 : -1;
        }
        return 0;
    });
};