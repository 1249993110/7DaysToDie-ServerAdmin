<template>
    <div class="h-[50vh]">
        <DataTable :value="tableData" removableSort scrollable scrollHeight="flex" size="small">
            <template #empty>
                <div class="text-center text-gray-500 py-4">
                    {{ $t('components.myTable.noData') }}
                </div>
            </template>
            <Column :header="$t('components.playerInventoryDialog.icon')">
                <template #body="{ data }">
                    <GameIconEx :size="48" :fontSize="20" v-bind="data" />
                </template>
            </Column>
            <Column field="localizationName" :header="$t('components.playerInventoryDialog.localizationName')" sortable></Column>
            <Column field="itemName" :header="$t('components.playerInventoryDialog.itemName')" sortable></Column>
            <Column :header="$t('components.playerInventoryDialog.mod')">
                <template #body="{ data }">
                    <GameIconEx v-for="(item, index) in data?.parts?.filter((i) => i)" :key="index" :size="60" v-bind="item" />
                </template>
            </Column>
        </DataTable>
    </div>
</template>

<script setup>
import GameIconEx from '../GameIconEx/index.vue';

const props = defineProps({
    tableData: {
        type: Array,
    },
});
</script>
