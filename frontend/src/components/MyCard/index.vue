<template>
    <Card>
        <template #header>
            <div
                class="relative mt-4 mx-8 text-base font-medium before:absolute before:top-1 before:left--3 rtl:before:right--3 before:w-1 before:h-4 before:content-[''] before:bg-p-primary-color before:rounded-xl"
            >
                {{ header }}
            </div>
        </template>
        <template #content>
            <slot />
        </template>
    </Card>
</template>

<script setup>
defineProps({
    header: String,
});
</script>
