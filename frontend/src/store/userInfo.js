import * as authApi from '~/api/auth';
import { myToast } from '~/plugins/sweetalert2';

export const useUserInfoStore = defineStore('userInfo', () => {
    const keyPrefix = 'userInfo.';
    const isRememberMe = useStorage(keyPrefix + 'isRememberMe', true);
    const useMyStorage = (key) => useStorage(key, '', isRememberMe.value ? localStorage : sessionStorage);
    const username = computed({
        get() {
            return useMyStorage(keyPrefix + 'username').value;
        },
        set(val) {
            useMyStorage(keyPrefix + 'username').value = val;
        },
    });
    const accessToken = computed({
        get() {
            return useMyStorage(keyPrefix + 'access_token').value;
        },
        set(val) {
            useMyStorage(keyPrefix + 'access_token').value = val;
        },
    });
    const expiresAt = computed({
        get() {
            return useMyStorage(keyPrefix + 'expires_at').value;
        },
        set(val) {
            useMyStorage(keyPrefix + 'expires_at').value = val;
        },
    });
    const refreshToken = computed({
        get() {
            return useMyStorage(keyPrefix + 'refresh_token').value;
        },
        set(val) {
            useMyStorage(keyPrefix + 'refresh_token').value = val;
        },
    });

    const route = useRoute();
    const router = useRouter();
    const { t } = useI18n();

    const signIn = async (_username, password) => {
        try {
            const data = await authApi.signIn(_username, password);

            username.value = _username;
            accessToken.value = data.access_token;
            expiresAt.value = dayjs().add(data.expires_in, 'second').toISOString();
            refreshToken.value = data.refresh_token;

            myToast({
                title: t('views.login.successTitle'),
                text: t('views.login.successMessage'),
                icon: 'success',
            });

            const redirect = route.query.redirect?.toString() || '/';
            router.push(redirect);
        } catch (error) {
            console.error(error);
            myToast({
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
            if (!accessToken.value) {
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
    };
    const refresh = async () => {
        const data = await authApi.refreshToken(refreshToken.value);
        accessToken.value = data.access_token;
        expiresAt.value = dayjs().add(data.expires_in, 'second').toISOString();
        refreshToken.value = data.refresh_token;
    };

    const getAccessToken = async () => {
        if (dayjs() > dayjs(expiresAt.value)) {
            await refresh();
        }
        return accessToken.value;
    };

    return { username, isRememberMe, signIn, signOut, isLoggedIn, signInByToken, getAccessToken };
});
