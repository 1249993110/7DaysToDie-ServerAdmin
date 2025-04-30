import { defineStore } from 'pinia';
import * as auth from '~/api/auth';

export const useUserInfoStore = defineStore('user-info', () => {
    const username = useLocalStorage('username', '');
    const password = useLocalStorage('password', '');
    const gameServerId = ref('');

    const route = useRoute();
    const router = useRouter();

    const login = async (_username, _password) => {
        username.value = _username;
        password.value = _password;
        await auth.login();
    };

    const logout = () => {
        username.value = '';
        password.value = '';
        if (route.path != '/login') {
            router.push('/login');
        }
    };

    const isLoggedIn = () => {
        return username.value !== '' && password.value !== '';
    };

    const getToken = () => {
        return `Basic ${btoa(`${username.value}:${password.value}`)}`;
    };

    return { username, gameServerId, login, logout, isLoggedIn, getToken };
});
