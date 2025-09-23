<template>
    <Dialog class="w-[64vw]" v-model:visible="visible" maximizable modal :header="$t('components.playerSkillsDialog.header')" @hide="modelValue = []">
        <div v-if="loading" class="f-center h-[50vh]">
            <ProgressSpinner />
        </div>
        <DataView v-else :value="[{}]">
            <template #header>
                <div class="flex justify-between items-center gap-4">
                    <span>{{ title }}</span>
                    <SelectButton v-model="layout" :options="options" :allowEmpty="false">
                        <template #option="{ option }">
                            <icon-mdi:unfold-less-horizontal v-if="option === 'fold'" />
                            <icon-mdi:unfold-more-horizontal v-else />
                        </template>
                    </SelectButton>
                </div>
            </template>
            <template #list>
                <Tabs :value="0" lazy>
                    <TabList>
                        <Tab v-for="(item, index) in modelValue" :key="index" :value="index">
                            <div class="flex items-center gap-1">
                                <GameIcon v-if="item.iconName" isUiIcon :iconName="item.iconName" :size="24" :preview="false" />
                                <span>{{ (item.localizationName || item.name) + ` (${$t('components.playerSkillsDialog.level')} ${item.level})` }}</span>
                            </div>
                        </Tab>
                    </TabList>
                    <TabPanels>
                        <TabPanel v-for="(item, index) in modelValue" :key="index" :value="index">
                            <Table :tableData="item.children" :isExpandAll="layout === 'expand'" />
                        </TabPanel>
                    </TabPanels>
                </Tabs>
            </template>
        </DataView>
    </Dialog>
</template>

<script setup>
import { getPlayerSkills } from '~/api/gameServer';
import { useLocaleStore } from '~/store/locale';
import Table from './Table.vue';

const modelValue = ref([]);
const visible = ref(false);
const loading = ref(false);
const title = ref('');

const localeStore = useLocaleStore();
const options = ref(['fold', 'expand']);
const layout = ref('expand');

const show = async (playerId, playerName) => {
    title.value = `${playerName} (${playerId})`;
    loading.value = true;
    visible.value = true;
    try {
        modelValue.value = await getPlayerSkills(playerId, localeStore.getLanguageEnglishName());
    } finally {
        loading.value = false;
    }
};

defineExpose({
    show,
});
</script>
