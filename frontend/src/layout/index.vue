<template>
    <div class="flex flex-col" :class="{ 'h-screen': isDrawerMenuVisible, 'overflow-hidden': isDrawerMenuVisible }">
        <Header class="padding z-10" />
        <div class="flex-grow flex gap-2 lg:gap-4 xl:gap-6 2xl:gap-8 3xl:gap-10 pt-20 pb-10 padding">
            <div class="flex-shrink-0">
                <Sidebar v-if="!isMenuButtonVisible" />
                <Drawer v-else v-model:visible="isDrawerMenuVisible" header=" " :position="isRTL ? 'right' : 'left'">
                    <Sidebar />
                </Drawer>
            </div>
            <div class="flex-1 min-w-0">
                <Main />
            </div>
        </div>
    </div>
</template>

<script setup>
import Header from './Header/index.vue';
import Sidebar from './Sidebar/index.vue';
import Main from './Main/index.vue';
import { useAppStore } from '~/store/app';
import { useGameEventStore } from '~/store/gameEvent';

const { isMenuButtonVisible, isRTL, isDrawerMenuVisible } = storeToRefs(useAppStore());
useGameEventStore();
</script>

<style lang="scss" scoped>
.padding {
    --uno: 'px-1 sm:px-2 md:px-4 lg:px-6 xl:px-8 2xl:px-10';
}
</style>
