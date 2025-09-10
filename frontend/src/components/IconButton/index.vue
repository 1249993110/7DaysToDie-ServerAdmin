<template>
    <Button
        class="mr-2 !size-7 !p-0 !t-border-color-1"
        text
        severity="contrast"
        :as="aTag ? 'a' : undefined"
        :target="aTag ? '_blank' : undefined"
        :rel="aTag ? 'noopener' : undefined"
    >
        <template #icon>
            <slot><span style="font-size: 12px">Icon</span></slot>
        </template>
    </Button>
</template>

<script setup>
const props = defineProps({
    aTag: {
        type: Boolean,
        default: false,
    },
});
</script>
