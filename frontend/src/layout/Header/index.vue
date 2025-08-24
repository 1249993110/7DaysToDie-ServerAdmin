<template>
    <div class="header">
        <div class="inner">
            <div class="f-center">
                <img class="logo-img" src="../../assets/images/logo.png" />
                <span class="ml-1 text-md font-extrabold tracking-wide bg-gradient-to-r from-purple-500 to-blue-500 bg-clip-text text-transparent">{{
                    localeStore.getAppTitle()
                }}</span>
            </div>
            <div class="flex h-7">
                <IconButton class="mr-2" a-tag href="https://github.com/1249993110/7DaysToDie-ServerAdmin">
                    <icon-mdi:github />
                </IconButton>
                <IconButton class="mr-2">
                    <icon-ic:baseline-discord />
                </IconButton>
                <IconButton class="mr-2" a-tag href="https://docs.7daystodie.dev">
                    <icon-ic:baseline-help-outline />
                </IconButton>
                <IconButton class="mr-2" a-tag href="https://github.com/1249993110/7DaysToDie-ServerAdmin/issues">
                    <icon-ic:baseline-feedback />
                </IconButton>
                <IconButton @click="toggleDark()">
                    <icon-ic:outline-nights-stay v-if="isDark" />
                    <icon-ic:outline-wb-sunny v-else />
                </IconButton>
                <div class="relative">
                    <IconButton
                        class="!border-transparent"
                        v-styleclass="{
                            selector: '@next',
                            enterFromClass: 'hidden',
                            enterActiveClass: 'animate-scalein',
                            leaveToClass: 'hidden',
                            leaveActiveClass: 'animate-fadeout',
                            hideOnOutsideClick: true,
                        }"
                    >
                        <span
                            style="
                                animation-duration: 2s;
                                background: conic-gradient(
                                    from 90deg,
                                    #f97316,
                                    #f59e0b,
                                    #eab308,
                                    #84cc16,
                                    #22c55e,
                                    #10b981,
                                    #14b8a6,
                                    #06b6d4,
                                    #0ea5e9,
                                    #3b82f6,
                                    #6366f1,
                                    #8b5cf6,
                                    #a855f7,
                                    #d946ef,
                                    #ec4899,
                                    #f43f5e
                                );
                            "
                            class="absolute -top-5 -left-5 w-20 h-20 animate-spin"
                        ></span>
                        <span style="inset: 1px; border-radius: 4px" class="absolute z-2 !bg-p-primary-color transition-all"></span>
                        <icon-ic:outline-palette class="relative z-10 !text-p-primary-contrast-color" />
                    </IconButton>
                    <Palette />
                </div>
                <IconButton>
                    <icon-ic:baseline-settings />
                </IconButton>
            </div>
        </div>
    </div>
</template>

<script setup>
import Palette from './Palette.vue';

const isDark = useDark();
const toggleDark = useToggle(isDark);
const localeStore = useLocaleStore();
</script>

<style scoped lang="scss">
.header {
    --uno: 'h-14 t-bg-overlay border-b border-solid t-border-color-1';
    left: 0;
    top: 0;
    position: fixed;
    width: 100%;
    transition: background-color 0.5s, border-color 0.5s;

    .inner {
        display: flex;
        align-items: center;
        height: 100%;
        justify-content: space-between;
        .logo-img {
            object-fit: contain;
            height: 40px;
        }
    }
}
</style>
