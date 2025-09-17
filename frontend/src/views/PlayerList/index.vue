<template>
    <Card class="h-full">
        <template #content>
            <Tabs value="0">
                <TabList>
                    <Tab value="0">Online Player</Tab>
                    <Tab value="1">History Player</Tab>
                </TabList>
                <TabPanels>
                    <TabPanel value="0">
                        <p class="m-0">
                            Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam,
                            quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum
                            dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                        </p>
                    </TabPanel>
                    <TabPanel value="1">
                        <HistoryPlayer />
                    </TabPanel>
                </TabPanels>
            </Tabs>
        </template>
    </Card>
</template>

<script>
export default {
    name: 'PlayerList',
};
</script>

<script setup>
import HistoryPlayer from './HistoryPlayer/index.vue';
</script>
