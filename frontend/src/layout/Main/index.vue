<template>
    <router-view v-slot="{ Component, route }">
            <transition appear name="fade-transform" mode="out-in">
                <keep-alive :include="include">
                    <component :is="Component" :key="route.name"></component>
                </keep-alive>
            </transition>
        </router-view>
</template>

<script setup>
import { mainRoutes } from '~/router';

const include = [];
const filterKeepAlive = (routes) => {
    routes.forEach((item) => {
        if (item.meta?.keepAlive !== false && item.name) {
            include.push(item.name);
        }
        if (item?.children?.length) {
            filterKeepAlive(item.children);
        }
    });
};

filterKeepAlive(mainRoutes);
</script>