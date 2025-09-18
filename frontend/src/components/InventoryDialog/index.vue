<template>
    <Dialog v-model:visible="visible" modal :header="$t('components.inventoryDialog.title')" @hide="inventory = {}">
        <DataView :value="[{}]" :layout="layout" :loading="loading">
            <template #header>
                <div class="flex justify-between items-center gap-4">
                    <span>{{ title }}</span>
                    <SelectButton v-model="layout" :options="options" :allowEmpty="false">
                        <template #option="{ option }">
                            <icon-ic:round-view-list v-if="option === 'list'" />
                            <icon-ic:round-grid-view v-else />
                        </template>
                    </SelectButton>
                </div>
            </template>

            <template #list>
                <List :bag="inventory.bag" :belt="inventory.belt" :equipment="inventory.equipment" />
            </template>
            <template #grid>
                <Grid :bag="inventory.bag" :belt="inventory.belt" :equipment="inventory.equipment" />
            </template>
        </DataView>
    </Dialog>
</template>

<script setup>
import Grid from './Grid/index.vue';
import List from './List/index.vue';
import { getPlayerInventory } from '~/api/gameServer';

const inventory = ref({});
const layout = ref('grid');
const options = ref(['list', 'grid']);

const visible = ref(false);
const loading = ref(false);

const title = ref('');

const show = async (playerId, playerName) => {
    title.value = `${playerName} (${playerId})`;
    loading.value = true;
    visible.value = true;
    try {
        inventory.value = await getPlayerInventory(playerId);
    } finally {
        loading.value = false;
    }
};

defineExpose({
    show,
});
</script>
