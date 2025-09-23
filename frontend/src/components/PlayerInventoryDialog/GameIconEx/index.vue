<template>
    <div class="game-icon-ex" @contextmenu.prevent="handleContextMenu" v-tooltip="tooltipContent" :style="{ backgroundColor: backgroundColor }">
        <GameIcon :iconName="iconName" :iconColor="iconColor" :size="size" />
        <template v-if="quality">
            <span :style="qualityColor"></span>
            <span class="quality-number">
                {{ quality }}
            </span>
        </template>
        <span v-else class="count">{{ count }}</span>
    </div>
</template>

<script setup>
import { value } from 'valibot';

const props = defineProps({
    size: {
        type: Number,
        default: 80,
    },
    itemName: {
        type: String,
    },
    iconName: {
        type: String,
    },
    iconColor: {
        type: String,
    },
    localizationName: {
        type: String,
    },
    count: {
        type: Number,
        default: 0,
    },
    maxStackAllowed: {
        type: Number,
        default: 0,
    },
    quality: {
        type: Number,
    },
    qualityColor: {
        type: String,
    },
    useTimes: {
        type: Number,
    },
    maxUseTimes: {
        type: Number,
    },
    isMod: {
        type: Boolean,
    },
    isBlock: {
        type: Boolean,
    },
    backgroundColor: {
        type: String,
    },
    fontSize: {
        type: Number,
        default: 24,
    },
});

const qualityColor = computed(() => {
    const size = props.size;
    const durability = (1 - props.useTimes / props.maxUseTimes) * size;
    return {
        backgroundColor: `#${props.qualityColor}C8`,
        width: (durability > size ? size : durability) + 'px',
        height: (size * 2) / 10 + 'px',
        position: 'absolute',
        bottom: 0,
        left: 0,
    };
});

const sizePx = computed(() => {
    return props.size + 'px';
});

const fontSizePx = computed(() => {
    return props.fontSize + 'px';
});

const { t } = useI18n();
const tooltipContent = computed(() => {
    return {
        value: `${t('components.playerInventoryDialog.itemName')}: ${props.itemName}
        ${t('components.playerInventoryDialog.iconName')}: ${props.iconName}
        ${t('components.playerInventoryDialog.iconColor')}: ${props.iconColor ?? 'FFFFFF'}
        ${t('components.playerInventoryDialog.localizationName')}: ${props.localizationName}
        ${t('components.playerInventoryDialog.count')}: ${props.count}
        ${t('components.playerInventoryDialog.maxStackAllowed')}: ${props.maxStackAllowed}
        ${t('components.playerInventoryDialog.quality')}: ${props.quality ?? 0}
        ${t('components.playerInventoryDialog.useTimes')}: ${props.useTimes}
        ${t('components.playerInventoryDialog.maxUseTimes')}: ${props.maxUseTimes}
        ${t('components.playerInventoryDialog.isMod')}: ${props.isMod ? t('common.yes') : t('common.no')}
        ${t('components.playerInventoryDialog.isBlock')}: ${props.isBlock ? t('common.yes') : t('common.no')}`,
        escape: false,
        pt: { root: { style: { maxWidth: '80vw' } } },
        autoHide: false,
    };
});
</script>

<style scoped lang="scss">
.game-icon-ex {
    position: relative;
    height: v-bind(sizePx);
    width: v-bind(sizePx);

    .count {
        color: white;
        font-size: v-bind(fontSizePx);
        line-height: v-bind(fontSizePx);
        position: absolute;
        right: 1px;
        bottom: 1px;
        text-shadow: 0 0 4px #32003c, 0 0 4px #32003c;
    }

    .quality-number {
        color: white;
        font-size: v-bind(fontSizePx);
        line-height: v-bind(fontSizePx);
        position: absolute;
        bottom: 1px;
        text-shadow: 0 0 4px #32003c, 0 0 4px #32003c;
        display: flex;
        justify-content: center;
        width: 100%;
    }
}
</style>
