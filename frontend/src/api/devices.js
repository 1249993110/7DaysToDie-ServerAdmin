export const getSystemMetricsSnapshot = () => {
    return http.get('/Devices/SystemMetricsSnapshot');
};


export const getSystemPlatformInfo = () => {
    return http.get('/Devices/SystemPlatformInfo');
};