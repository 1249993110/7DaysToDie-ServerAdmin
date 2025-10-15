<template>
    <Card class="h-full">
        <template #content>
            <Tabs value="0">
                <TabList>
                    <Tab value="0">{{ $t('views.permission.adminUser') }}</Tab>
                    <Tab value="1">{{ $t('views.permission.commandPermission') }}</Tab>
                </TabList>
                <TabPanels>
                    <TabPanel value="0">
                        <AdminUser />
                    </TabPanel>
                    <TabPanel value="1">
                        <CommandPermission />
                    </TabPanel>
                </TabPanels>
            </Tabs>
        </template>
    </Card>
</template>

<script>
export default {
    name: 'Permission',
};
</script>

<script setup>
import AdminUser from './AdminUser/index.vue';
import CommandPermission from './CommandPermission/index.vue';
</script>