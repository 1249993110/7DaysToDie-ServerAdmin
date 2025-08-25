<template>
    <div
        class="flex flex-col flex-none w-[250px] h-[calc(100vh-8rem)] rtl:right-0 ltr:left-0 mr-[4rem] rtl:ml-[4rem] overflow-auto p-0 sticky top-[5rem] transition-all duration-400 ease-in-out select-none"
    >
        <PanelMenu :model="items" class="w-full">
            <template #item="{ item }">
                <router-link v-if="item.route" v-slot="{ href, navigate }" :to="item.route" custom>
                    <a v-ripple class="flex items-center cursor-pointer text-surface-700 dark:text-surface-0 px-4 py-2" :href="href" @click="navigate">
                        <span class="f-center size-7 border border-solid t-border-color-1 rounded-md">
                            <component :is="item.icon" />
                        </span>
                        <span class="ml-2">{{ item.label }}</span>
                        <icon-ic:baseline-keyboard-arrow-down class="ml-auto text-p-primary-color" v-if="item.items" />
                    </a>
                </router-link>
                <a v-else v-ripple class="flex items-center cursor-pointer text-surface-700 dark:text-surface-0 px-4 py-2" :href="item.url" :target="item.target">
                    <span class="f-center size-7 border border-solid t-border-color-1 rounded-md">
                        <component :is="item.icon" />
                    </span>
                    <span class="ml-2">{{ item.label }}</span>
                    <icon-ic:baseline-keyboard-arrow-down class="ml-auto text-p-primary-color" v-if="item.items" />
                </a>
            </template>
        </PanelMenu>
    </div>
</template>

<script setup>
import { mainRoutes } from '~/router';

const router = useRouter();

/**
 * Recursively converts Vue Router routes to PrimeVue menu item format.
 * @param {Array} routes Array of routes.
 * @param {string} parentPath Parent route path (optional).
 * @returns {Array} Converted menu items array.
 */
const mapRoutesToMenuItems = (routes, parentPath = '') => {
    return routes
        .map((route) => {
            if (route.meta?.hideInMenu === true) {
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
                label: route.meta.title,
                icon: route.meta.icon, // You may need to adjust this depending on your icon library
                // Combine parent path and current path
                route: parentPath && route.path[0] !== '/' ? `${parentPath}/${route.path}` : route.path,
            };

            // Recursively process child routes

            if (route.children) {
                menuItem.route = null;
                menuItem.items = mapRoutesToMenuItems(route.children, menuItem.route);
            }

            return menuItem;
        })
        .flat() // Use flat() to handle nested arrays from recursion
        .filter(Boolean); // Filter out null or undefined values
};

const items = ref(mapRoutesToMenuItems(mainRoutes));

</script>
