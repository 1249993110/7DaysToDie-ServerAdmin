<template>
    <div class="flex gap-2">
        <div :style="{ minWidth: `${4 * iconSize + 50}px` }">
            <Tag class="my-1">{{ $t('components.playerInventoryDialog.bag') }}</Tag>
            <div class="flex flex-wrap gap-1 overflow-y-auto content-start" :style="{ height: `${5 * iconSize + 22}px` }">
                <GameIconEx v-for="(item, index) in bag" :key="index" :size="iconSize" v-bind="item" backgroundColor="#4d4d4d" />
            </div>
        </div>
        <div :style="{ minWidth: `${2 * iconSize + 8}px` }">
            <Tag class="my-1">{{ $t('components.playerInventoryDialog.equipment') }}</Tag>
            <div class="flex flex-wrap gap-1 overflow-y-auto h-full">
                <GameIconEx v-for="(item, index) in equipment?.filter((i) => i)" :key="index" :size="iconSize" v-bind="item" backgroundColor="#4d4d4d" />
            </div>
        </div>
    </div>
    <div>
        <Tag class="my-1">{{ $t('components.playerInventoryDialog.belt') }}</Tag>
        <div class="flex flex-wrap gap-1 overflow-y-auto" :style="{ height: `${iconSize + 8}px` }">
            <GameIconEx v-for="(item, index) in belt" :key="index" :size="iconSize" v-bind="item" backgroundColor="#4d4d4d" />
        </div>
    </div>
</template>

<script setup>
import GameIconEx from '../GameIconEx/index.vue';

const props = defineProps({
    bag: {
        type: Array,
    },
    belt: {
        type: Array,
    },
    equipment: {
        type: Array,
    },
});

const iconSize = 80;
</script>
