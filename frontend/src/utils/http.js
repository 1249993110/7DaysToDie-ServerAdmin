import axios from 'axios';
import nProgress from '~/plugins/nprogress';
import qs from 'qs';
import { authUrl } from '~/api/auth';

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
    async (config) => {
        nProgress.start();

        if (config.url != authUrl) {
            const token = await useUserInfoStore().getAccessToken();
            config.headers.Authorization = `Bearer ${token}`;
        }

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
    async (error) => {
        nProgress.done();

        if (!error.response) {
            console.error(error);
            return Promise.reject(error);
        }

        switch (error.response.status) {
            case 401:
                await useUserInfoStore().signOut();
                break;
            case 403:
                useRouter().push('/403');
                break;
            case 404:
                console.error('The requested resource does not found');
                break;
            case 400:
                console.error(error.response.data.title || 'Bad Request');
                break;
            case 500:
                console.error(error.response.data.message || 'Internal Server Error');
                break;
            default:
                console.error(error);
                break;
        }

        return Promise.reject(error);
    }
);

export default service;
