<template>
    <div class="flex flex-col min-h-screen  t-bg-1">
        <Header class="padding" />
        <div class="flex-grow flex p-t-14">
            <Sidebar />
            <div class="flex-grow padding">
                <Card>
                    <template #title></template>
                    <template #content>
                    </template>
                </Card>
                <!-- <div class="size-100 t-bg-2">

                </div> -->
            </div>
        </div>
    </div>
</template>

<script setup>
import Header from './Header/index.vue';
import Sidebar from './Sidebar/index.vue';
</script>

<style lang="scss" scoped>
.padding {
    --uno: 'px-1 sm:px-2 md:px-4 lg:px-6 xl:px-10 2xl:px-14';
}
</style>
