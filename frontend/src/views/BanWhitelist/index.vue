<template>
    <Card class="h-full">
        <template #content>
            <Tabs value="0">
                <TabList>
                    <Tab value="0">{{ $t('views.banWhitelist.ban') }}</Tab>
                    <Tab value="1">{{ $t('views.banWhitelist.whitelist') }}</Tab>
                </TabList>
                <TabPanels>
                    <TabPanel value="0">
                        <Ban />
                    </TabPanel>
                    <TabPanel value="1">
                        <Whitelist />
                    </TabPanel>
                </TabPanels>
            </Tabs>
        </template>
    </Card>
</template>

<script>
export default {
    name: 'BanWhitelist',
};
</script>

<script setup>
import Ban from './Ban/index.vue';
import Whitelist from './Whitelist/index.vue';
</script>
