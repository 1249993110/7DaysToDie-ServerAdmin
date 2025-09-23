<template>
    <div>
        <TreeTable v-model:expandedKeys="expandedKeys" :value="tableData" dataKey="name" size="small" scrollable scrollHeight="flex">
            <template #empty>
                <div class="text-center text-gray-500 py-4">
                    {{ $t('components.myTable.noData') }}
                </div>
            </template>
            <Column :header="$t('components.playerSkillsDialog.icon')" width="65px" expander>
                <template #body="{ node: data }">
                    <GameIcon isUiIcon :iconName="data.iconName" :size="36" :preview="false" />
                </template>
            </Column>
            <Column :header="$t('components.playerSkillsDialog.localizationName')">
                <template #body="{ node: data }">
                    <span :class="{ 'font-semibold': isSkillOrBookGroup(data) }">{{ data.localizationName }}</span>
                </template>
            </Column>
            <Column :header="$t('components.playerSkillsDialog.name')">
                <template #body="{ node: data }">
                    <span :class="{ 'font-semibold': isSkillOrBookGroup(data) }">{{ data.name }}</span>
                </template>
            </Column>
            <Column field="level" :header="$t('components.playerSkillsDialog.level')">
                <template #body="scope">
                    {{ getValue(scope) }}
                </template>
            </Column>
            <Column field="maxLevel" :header="$t('components.playerSkillsDialog.maxLevel')">
                <template #body="scope">
                    {{ getValue(scope) }}
                </template>
            </Column>
            <Column field="costForNextLevel" :header="$t('components.playerSkillsDialog.costForNextLevel')">
                <template #body="scope">
                    {{ getValue(scope) }}
                </template>
            </Column>
            <Column field="type" :header="$t('components.playerSkillsDialog.type')" m>
                <template #body="{ node: data }">
                    {{ data.type }}
                </template>
            </Column>
            <Column field="localizationDesc" :header="$t('components.playerSkillsDialog.localizationDesc')">
                <template #body="{ node: data }">
                    {{ data.localizationDesc }}
                </template>
            </Column>
        </TreeTable>
    </div>
</template>

<script setup>
const props = defineProps({
    tableData: {
        type: Array,
        default: () => [],
    },
    isExpandAll: {
        type: Boolean,
        default: true,
    },
});

const isSkillOrBookGroup = (data) => data.type === 'Skill' || data.type === 'BookGroup';

const getValue = ({ node: data, column }) => {
    if (isSkillOrBookGroup(data)) {
        return '';
    } else {
        return data[column.props.field];
    }
};

const expandedKeys = ref({});
watch(
    () => props.isExpandAll,
    (newVal) => {
        if (newVal) {
            const keys = {};
            props.tableData.forEach((item) => {
                keys[item.name] = true;
            });
            expandedKeys.value = keys;
        } else {
            expandedKeys.value = {};
        }
    },
    { immediate: true }
);
</script>
