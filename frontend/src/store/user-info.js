import { defineStore } from 'pinia';
import * as authApi from '~/api/auth';

export const useUserInfoStore = defineStore('user-info', () => {
    const username = ref(localStorage.getItem('username') || sessionStorage.getItem('username') || '');
    const accessToken = ref(localStorage.getItem('access_token') || sessionStorage.getItem('access_token') || '');
    const expiresAt = ref(localStorage.getItem('expires_at') || sessionStorage.getItem('expires_at') || '');
    const refreshToken = ref(localStorage.getItem('refresh_token') || sessionStorage.getItem('refresh_token') || '');

    const route = useRoute();
    const router = useRouter();
    const { t } = useI18n();

    const save = (isRememberMe) => {
        const storage = isRememberMe ? localStorage : sessionStorage;

        // Clean up another storage to ensure consistent state
        const otherStorage = isRememberMe ? sessionStorage : localStorage;
        otherStorage.removeItem('username');
        otherStorage.removeItem('access_token');
        otherStorage.removeItem('expires_at');
        otherStorage.removeItem('refresh_token');

        // Store the tokens in the selected storage
        storage.setItem('username', username.value);
        storage.setItem('access_token', accessToken.value);
        storage.setItem('expires_at', expiresAt.value);
        storage.setItem('refresh_token', refreshToken.value);
    };

    const signIn = async (_username, password, isRememberMe) => {
        try {
            const data = await authApi.signIn(_username, password);

            username.value = _username;
            accessToken.value = data.access_token;
            expiresAt.value = dayjs().add(data.expires_in, 'second').toISOString();
            refreshToken.value = data.refresh_token;
            save(isRememberMe);

            Toast.fire({
                title: t('views.login.successTitle'),
                text: t('views.login.successMessage'),
                icon: 'success',
            });

            const redirect = route.query.redirect?.toString() || '/';
            router.push(redirect);
        } catch (error) {
            console.error(error);
            Toast.fire({
                title: t('views.login.failedTitle'),
                text: t('views.login.failedMessage'),
                icon: 'error',
            });
        }
    };

    const signOut = async () => {
        username.value = '';
        accessToken.value = '';
        expiresAt.value = '';
        refreshToken.value = '';

        localStorage.removeItem('username');
        localStorage.removeItem('access_token');
        localStorage.removeItem('expires_at');
        localStorage.removeItem('refresh_token');

        sessionStorage.removeItem('username');
        sessionStorage.removeItem('access_token');
        sessionStorage.removeItem('expires_at');
        sessionStorage.removeItem('refresh_token');

        router.push('/login');
        await Promise.resolve();
    };

    const isLoggedIn = async () => {
        try {
            if (!!accessToken.value) {
                return false;
            }

            if (dayjs().isAfter(dayjs(expiresAt.value))) {
                await refresh();
            }

            return true;
        } catch {
            return false;
        }
    };

    const signInByToken = (_username, accessToken, expiresIn, refreshToken) => {
        username.value = _username;
        accessToken.value = accessToken;
        expiresAt.value = dayjs().add(expiresIn, 'second').toISOString();
        refreshToken.value = refreshToken;
        save(isRememberMe);
    };
    const refresh = async () => {
        const data = await authApi.refreshToken(refreshToken.value);
        accessToken.value = data.access_token;
        expiresAt.value = dayjs().add(data.expires_in, 'second').format();
        refreshToken.value = data.refresh_token;
        save(isRememberMe);
    };

    const getAccessToken = async () => {
        if (dayjs() > dayjs(expiresAt.value)) {
            await refresh();
        }
        return accessToken.value;
    };

    return { username, signIn, signOut, isLoggedIn, signInByToken, getAccessToken };
});
