import { createRouter, createWebHashHistory } from 'vue-router';
import nProgress from '~/plugins/nprogress';
import { useUserInfoStore } from '~/store/user-info';
import qs from 'qs';

const routes = [
    {
        path: '/',
        redirect: '/user/server-overview',
    },
    {
        path: '/login',
        name: 'login',
        component: () => import('../views/login.vue'),
    },
    {
        path: '/403',
        name: '403',
        component: () => import('../views/403.vue'),
    },
    {
        path: '/404',
        name: '404',
        component: () => import('../views/404.vue'),
    },
    {
        path: '/:pathMatch(.*)*',
        redirect: '/404',
    },
];

const router = createRouter({
    history: createWebHashHistory(),
    routes,
    parseQuery: qs.parse,
    stringifyQuery: qs.stringify,
});

router.beforeEach((to, from) => {
    nProgress.start();

    if (!!to.query.gameServerId) {
        useUserInfoStore().gameServerId = to.query.gameServerId;
        to.query.gameServerId = undefined;
        return to.path + '?' + qs.stringify(to.query);
    }

    // Check if this route requires authorization and if the user has logged in
    if (to.meta.requiresAuth) {
        const isLoggedIn = useUserInfoStore().isLoggedIn();
        if (!isLoggedIn) {
            // If not, redirect to the login page
            return '/login?redirect=' + to.fullPath;
        }
    }
});

router.afterEach(() => {
    nProgress.done();
});

export default router;

const keepAlives = [];
const filterKeepAlive = (routes, cache) => {
    routes.forEach((item) => {
        item.meta?.keepAlive && item.name && cache.push(item.name);
        item?.children?.length && filterKeepAlive(item.children, cache);
    });
};

filterKeepAlive(routes, keepAlives);

export { keepAlives };
