<template>
    <Image :src="src" preview />
</template>

<script setup>
const props = defineProps({
    iconName: {
        type: String,
        default: '',
    },
    iconColor: {
        type: String,
    },
    isUiIcon: {
        type: Boolean,
        default: false,
    },
});

const getIconUrl = (category, iconName, iconColor = null) => {
    if (!iconName) {
        return null;
    }

    // Append color to the name if it exists and is not 'FFFFFF'
    const name = iconColor && iconColor.toUpperCase() !== 'FFFFFF' ? `${iconName}__${iconColor}` : iconName;

    // Dynamically create the path based on the category
    const basePath = `${import.meta.env.VITE_API_BASE_URL}GameServer/${category}/`;
    return `${basePath}${name}.png`;
};

const src = computed(() => {
    return getIconUrl(props.isUiIcon ? 'UiIcons' : 'ItemIcons', props.iconName, props.iconColor);
});
</script>
