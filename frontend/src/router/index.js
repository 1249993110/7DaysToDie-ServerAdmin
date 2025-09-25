import { createRouter, createWebHashHistory } from 'vue-router';
import nProgress from '~/plugins/nprogress';
import Layout from '~/layout/index.vue';
import qs from 'qs';
import { useUserInfoStore } from '~/store/userInfo';

const { t } = useI18n();

const fullRoutes = [
    {
        path: '/',
        redirect: '/dashboard',
        component: Layout,
        children: [
            {
                name: 'Dashboard',
                path: '/dashboard',
                component: () => import('../views/Dashboard/index.vue'),
                meta: {
                    title: () => t('menus.dashboard'),
                    icon: markIcon(() => import('~icons/mdi/monitor-dashboard')),
                    isKeepAlive: true,
                    isRequireAuth: true,
                    isHideInMenu: false,
                    isExternal: false,
                },
            },
            {
                name: 'PlayerList',
                path: '/player-list',
                component: () => import('../views/PlayerList/index.vue'),
                meta: {
                    title: () => t('menus.playerList'),
                    icon: markIcon(() => import('~icons/mdi/account-group')),
                    isRequireAuth: true,
                },
            },
            {
                name: 'GPSMap',
                path: '/gps-map',
                component: () => import('../views/GPSMap/index.vue'),
                meta: {
                    title: () => t('menus.gpsMap'),
                    icon: markIcon(() => import('~icons/mdi/map')),
                    isRequireAuth: true,
                },
            },
            {
                name: 'GameChat',
                path: '/game-chat',
                component: () => import('../views/Dashboard/index.vue'),
                meta: {
                    title: () => t('menus.gameChat'),
                    icon: markIcon(() => import('~icons/mdi/chat')),
                    isRequireAuth: true,
                },
            },
            {
                name: 'ServerConfig',
                path: '/server-config',
                component: () => import('../views/Dashboard/index.vue'),
                meta: {
                    title: () => t('menus.serverConfig'),
                    icon: markIcon(() => import('~icons/ic/baseline-settings')),
                    isRequireAuth: true,
                },
            },
            {
                name: 'BlackWhiteList',
                path: '/black-white-list',
                component: () => import('../views/Dashboard/index.vue'),
                meta: {
                    title: () => t('menus.blackWhiteList'),
                    icon: markIcon(() => import('~icons/mdi/list-status')),
                    isRequireAuth: true,
                },
            },
            {
                name: 'Permissions',
                path: '/permissions',
                component: () => import('../views/Dashboard/index.vue'),
                meta: {
                    title: () => t('menus.permissions'),
                    icon: markIcon(() => import('~icons/icon-park-outline/permissions')),
                    isRequireAuth: true,
                },
            },
            {
                name: 'ModManagement',
                path: '/mod-management',
                component: () => import('../views/Dashboard/index.vue'),
                meta: {
                    title: () => t('menus.modManagement'),
                    icon: markIcon(() => import('~icons/mdi/cogs')),
                    isRequireAuth: true,
                },
            },
            {
                name: 'Console',
                path: '/console',
                component: () => import('../views/Console/index.vue'),
                meta: {
                    title: () => t('menus.console'),
                    icon: markIcon(() => import('~icons/mdi/console')),
                    isRequireAuth: true,
                },
            },
            {
                name: 'AppSettings',
                path: '/app-settings',
                component: () => import('../views/Dashboard/index.vue'),
                meta: {
                    title: () => t('menus.appSettings'),
                    icon: markIcon(() => import('~icons/mdi/cog')),
                    isRequireAuth: true,
                },
            },
            {
                path: '/swagger',
                meta: {
                    title: () => t('menus.apiDocumentation'),
                    icon: markIcon(() => import('~icons/mdi/file-document')),
                    isExternal: true,
                }
            }
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

router.beforeEach(async (to, from) => {
    nProgress.start();

    // Check if this route requires authorization and if the user has logged in
    if (to.meta.isRequireAuth) {
        const isLoggedIn = await useUserInfoStore().isLoggedIn();
        
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

const mainRoutes = fullRoutes[0].children;

export { mainRoutes };
