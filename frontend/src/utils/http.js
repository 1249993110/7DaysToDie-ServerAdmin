import axios from 'axios';
import nProgress from '~/plugins/nprogress';
import qs from 'qs';

const service = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    timeout: import.meta.env.VITE_API_TIMEOUT,
    paramsSerializer: {
        serialize: (params) => {
            return qs.stringify(params, { arrayFormat: 'repeat' });
        },
    },
});

service.interceptors.request.use(
    (config) => {
        nProgress.start();
        const uerInfoStore = useUserInfoStore();
        config.headers.Authorization = uerInfoStore.getToken();
        config.headers.gameServerId = uerInfoStore.gameServerId;
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

service.interceptors.response.use(
    (response) => {
        nProgress.done();
        return response.data;
    },
    (error) => {
        nProgress.done();
        if (!error.response) {
            ElMessage.error(error.message);
            return Promise.reject(error);
        }

        switch (error.response.status) {
            case 401:
                useUserInfoStore().logout();
                break;
            case 403:
                useRouter().push('/403');
                break;
            case 404:
                ElMessage.error('The requested resource does not found');
                break;
            case 400:
                ElMessage.error(error.response.data.title || 'Bad Request');
                console.error(error.response.data);
                break;
            case 500:
                ElMessage.error(error.response.data.message || 'Internal Server Error');
                break;
            default:
                ElMessage.error(error.message);
                console.error(error);
                break;
        }

        return Promise.reject(error);
    }
);

export default service;
