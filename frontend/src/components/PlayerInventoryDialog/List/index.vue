<template>
    <div class="size-full">
        <Tabs value="0">
            <TabList>
                <Tab value="0">{{ $t('components.playerInventoryDialog.bag') }}</Tab>
                <Tab value="1">{{ $t('components.playerInventoryDialog.belt') }}</Tab>
                <Tab value="2">{{ $t('components.playerInventoryDialog.equipment') }} </Tab>
            </TabList>
            <TabPanels>
                <TabPanel value="0">
                    <Table :tableData="bag" />
                </TabPanel>
                <TabPanel value="1">
                    <Table :tableData="belt" />
                </TabPanel>
                <TabPanel value="2">
                    <Table :tableData="equipment?.filter((i) => i)" />
                </TabPanel>
            </TabPanels>
        </Tabs>
    </div>
</template>

<script setup>
import Table from './Table.vue';

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
</script>
