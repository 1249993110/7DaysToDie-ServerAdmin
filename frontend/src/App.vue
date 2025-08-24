<template>
  <router-view />
</template>

<script setup>
const localeStore = useLocaleStore();

// Monitor language changes and update document title
watch(
    () => localeStore.locale,
    () => {
        document.title = localeStore.getAppTitle();
    },
    { immediate: true }
);
</script>
