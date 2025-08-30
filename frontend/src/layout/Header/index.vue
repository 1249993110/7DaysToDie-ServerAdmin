<template>
    <div class="header">
        <div class="inner">
            <div class="f-center">
                <img class="logo-img" src="../../assets/images/logo.png" />
                <span class="mx-1 text-md font-extrabold tracking-wide bg-gradient-to-r from-purple-500 to-blue-500 bg-clip-text text-transparent">{{
                    localeStore.getAppTitle()
                }}</span>
            </div>
            <div class="flex h-7">
                <IconButton class="mr-2" a-tag href="https://github.com/1249993110/7DaysToDie-ServerAdmin">
                    <icon-mdi:github />
                </IconButton>
                <IconButton
                    class="mr-2"
                    a-tag
                    :href="
                        localeStore.lang === 'zh-cn'
                            ? 'https://qm.qq.com/cgi-bin/qm/qr?k=p3TKGDnBAxxyVsR79pF-WYHI3BjsYiHe&jump_from=webapi&authKey=wTpnGpOGOsAaNTD4TqL4kukLQnxT+TmDFQx803v+Q2zWU0E7LYuSkBQQI+WhrqFB'
                            : 'https://discord.gg/7daystodie'
                    "
                >
                    <icon-mdi:qqchat v-if="localeStore.lang === 'zh-cn'" />
                    <icon-ic:baseline-discord v-else />
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
                        <span style="inset: 1px; border-radius: 4px" class="absolute z-1 !bg-p-primary-color transition-all"></span>
                        <icon-ic:outline-palette class="relative z-2 !text-p-primary-contrast-color" />
                    </IconButton>
                    <Palette />
                </div>
                <IconButton @click="onLangButtonClick" aria-haspopup="true" :aria-controls="langMenuId">
                    <icon-ion:language />
                </IconButton>
                <Menu
                    ref="langMenuRef"
                    :id="langMenuId"
                    :model="langNativeMap"
                    :popup="true"
                    :pt="{
                        itemLabel: ({ context }) => {
                            return context.item.value === localeStore.lang ? { class: 'text-p-primary-color' } : undefined;
                        },
                    }"
                />
                <IconButton @click="onAccountButtonClick">
                    <icon-material-symbols:person />
                </IconButton>
                <Menu ref="accountMenuRef" :id="accountMenuId" :model="accountMenuItems" :popup="true">
                    <template #itemicon="{ item }">
                        <component :is="item.icon" />
                    </template>
                </Menu>
                <IconButton v-if="isMenuButtonVisible" @click="onMenuButtonClick">
                    <icon-mdi:menu />
                </IconButton>
            </div>
        </div>
    </div>
</template>

<script setup>
import Palette from './Palette.vue';

const isDark = useDark();
const userInfoStore = useUserInfoStore();

const toggleDark = useToggle(isDark);
const localeStore = useLocaleStore();
const { isMenuButtonVisible, isDrawerMenuVisible } = storeToRefs(useAppStore());

const onMenuButtonClick = () => {
    isDrawerMenuVisible.value = !isDrawerMenuVisible.value;
};

const langMenuId = `menu_${crypto.randomUUID()}`;
const langMenuRef = ref();
const langNativeMap = localeStore.getLanguageNativeMap();
const onLangButtonClick = (event) => {
    langMenuRef.value.toggle(event);
};

const accountMenuId = `menu_${crypto.randomUUID()}`;
const accountMenuRef = ref();
const accountMenuItems = [
    {
        label: 'Logout',
        icon: markIcon(() => import('~icons/material-symbols/logout')),
        command: async () => {
            if (await myConfirm()) {
                await userInfoStore.signOut();
            }
        },
    },
];
const onAccountButtonClick = (event) => {
    accountMenuRef.value.toggle(event);
};
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
