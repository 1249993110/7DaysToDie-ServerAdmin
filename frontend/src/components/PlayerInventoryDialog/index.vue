<template>
    <Dialog class="w-[64vw]" v-model:visible="visible" maximizable modal :header="$t('components.playerInventoryDialog.header')" @hide="modelValue = {}">
        <div v-if="loading" class="f-center h-[50vh]">
            <ProgressSpinner />
        </div>
        <DataView v-else :value="[{}]" :layout="layout">
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
                <List :bag="modelValue.bag" :belt="modelValue.belt" :equipment="modelValue.equipment" />
            </template>
            <template #grid>
                <Grid :bag="modelValue.bag" :belt="modelValue.belt" :equipment="modelValue.equipment" />
            </template>
        </DataView>
    </Dialog>
</template>

<script setup>
import Grid from './Grid/index.vue';
import List from './List/index.vue';
import { getPlayerInventory } from '~/api/gameServer';
import { useLocaleStore } from '~/store/locale';

const modelValue = ref({});
const layout = ref('grid');
const options = ref(['list', 'grid']);

const visible = ref(false);
const loading = ref(false);
const title = ref('');
const localeStore = useLocaleStore();

const show = async (playerId, playerName) => {
    title.value = `${playerName} (${playerId})`;
    loading.value = true;
    visible.value = true;
    try {
        modelValue.value = await getPlayerInventory(playerId, localeStore.getLanguageEnglishName());
    } finally {
        loading.value = false;
    }
};

defineExpose({
    show,
});
</script>
