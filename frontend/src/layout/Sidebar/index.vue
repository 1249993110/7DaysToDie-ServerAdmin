<template>
    <div
        class="flex flex-col flex-none w-[250px] h-[calc(100vh-8rem)] pe-1 ltr:left-0 rtl:right-0 ltr:mr-4 rtl:ml-4 ltr:lg:mr-8 rtl:lg:ml-8 ltr:2xl:mr-12 rtl:2xl:ml-12 overflow-auto p-0 sticky top-20 transition-all duration-400 ease-in-out select-none"
    >
        <PanelMenu :model="items" class="w-full" multiple>
            <template #item="{ item }">
                <a v-if="item.isExternal" v-ripple class="flex items-center cursor-pointer text-surface-700 dark:text-surface-0 px-4 py-2" :href="item.route" target="_blank">
                    <span class="f-center size-7 border border-solid t-border-color-1 rounded-md">
                        <component :is="item.icon" />
                    </span>
                    <span class="ltr:ml-2 rtl:mr-2">{{ item.label }}</span>
                    <icon-ic:baseline-keyboard-arrow-down class="ltr:ml-auto rtl:mr-auto text-p-primary-color" v-if="item.items" />
                </a>
                <router-link v-else v-slot="{ href, navigate }" :to="item.route" custom>
                    <a
                        v-ripple
                        class="flex items-center cursor-pointer text-surface-700 dark:text-surface-0 px-4 py-2"
                        :class="{ 'text-p-primary-color': isActiveRoute(item.route) }"
                        :href="href"
                        @click="navigate"
                    >
                        <span class="f-center size-7 border border-solid t-border-color-1 rounded-md">
                            <component :is="item.icon" />
                        </span>
                        <span class="ltr:ml-2 rtl:mr-2">{{ item.label }}</span>
                        <icon-ic:baseline-keyboard-arrow-down class="ltr:ml-auto rtl:mr-auto text-p-primary-color" v-if="item.items" />
                    </a>
                </router-link>
            </template>
        </PanelMenu>
    </div>
</template>

<script setup>
import { mainRoutes } from '~/router';

const resolveTitle = (title) => {
    return typeof title === 'function' ? title() : title;
};

const currentRoute = useRoute();
const isActiveRoute = (route) => {
    return currentRoute.path === route;
};

/**
 * Recursively converts Vue Router routes to PrimeVue menu item format.
 * @param {Array} routes Array of routes.
 * @param {string} parentPath Parent route path (optional).
 * @returns {Array} Converted menu items array.
 */
const mapRoutesToMenuItems = (routes, parentPath = '') => {
    return routes
        .map((route) => {
            if (route.meta?.isHideInMenu === true) {
                return null; // Skip this route if it's hidden in the menu
            }

            // If the route has a redirect or no meta, usually skip it as a menu item
            if (route.redirect || !route.meta) {
                // If there are child routes, process them recursively
                if (route.children) {
                    return mapRoutesToMenuItems(route.children, route.path);
                }
                return null; // Ignore routes without meta
            }

            const menuItem = {
                isExternal: route.meta.isExternal,
                label: resolveTitle(route.meta.title),
                icon: route.meta.icon, // You may need to adjust this depending on your icon library
                // Combine parent path and current path
                route: parentPath && route.path[0] !== '/' ? `${parentPath}/${route.path}` : route.path,
            };

            // Recursively process child routes

            if (route.children) {
                menuItem.isExternal = true;
                menuItem.route = undefined;
                menuItem.items = mapRoutesToMenuItems(route.children, menuItem.route);
            }

            return menuItem;
        })
        .flat() // Use flat() to handle nested arrays from recursion
        .filter(Boolean); // Filter out null or undefined values
};

const items = ref(mapRoutesToMenuItems(mainRoutes));

const localeStore = useLocaleStore();
watch(
    () => localeStore.lang,
    () => {
        items.value = mapRoutesToMenuItems(mainRoutes);
    }
);
</script>
