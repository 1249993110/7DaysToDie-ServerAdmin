
/**
 * Fetches system metrics snapshot.
 * @returns {Promise} - A promise that resolves to the system metrics snapshot.
 */
export const getSystemMetricsSnapshot = () => {
    return http.get('/Devices/SystemMetricsSnapshot');
};

/**
 * Fetches system platform information.
 * @returns {Promise} - A promise that resolves to the system platform information.
 */
export const getSystemPlatformInfo = () => {
    return http.get('/Devices/SystemPlatformInfo');
};