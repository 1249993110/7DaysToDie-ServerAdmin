<template>
    <div class="flex flex-col min-h-screen t-bg-1">
        <Header class="padding" />
        <div class="flex-grow flex padding p-t-20">
            <Sidebar />
            <div class="flex-grow">
                <Main />
            </div>
        </div>
    </div>
</template>

<script setup>
import Header from './Header/index.vue';
import Sidebar from './Sidebar/index.vue';
import Main from './Main/index.vue';
</script>

<style lang="scss" scoped>
.padding {
    --uno: 'px-1 sm:px-2 md:px-4 lg:px-6 xl:px-10 2xl:px-14';
    // margin-right: calc(100vw - 100%);
}
</style>
