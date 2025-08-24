import { createRouter, createWebHashHistory } from 'vue-router';
import nProgress from '~/plugins/nprogress';
import Layout from '~/layout/index.vue';
import qs from 'qs';

const markIcon = (source) => {
    return markRaw(defineAsyncComponent(source));
};

const fullRoutes = [
    {
        path: '/',
        redirect: '/dashboard',
        component: Layout,
        children: [
            {
                name: 'dashboard',
                path: '/dashboard',
                component: () => import('../views/Dashboard/index.vue'),
                meta: {
                    title: 'Dashboard',
                    icon: markIcon(() => import('~icons/mdi:dashboard')),
                    requiresAuth: false,
                },
            },
        ],
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
    routes: fullRoutes,
    parseQuery: qs.parse,
    stringifyQuery: qs.stringify,
});

router.beforeEach((to, from) => {
    nProgress.start();

    // Check if this route requires authorization and if the user has logged in
    if (to.meta.requiresAuth) {
        // const isLoggedIn = useUserInfoStore().isLoggedIn();
        // if (!isLoggedIn) {
        //     // If not, redirect to the login page
        //     return '/login?redirect=' + to.fullPath;
        // }
    }
});

router.afterEach(() => {
    nProgress.done();
});

export default router;

const mainRoutes = fullRoutes[0].children;

export { mainRoutes };
