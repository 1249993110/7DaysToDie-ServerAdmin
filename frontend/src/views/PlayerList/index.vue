<template>
    <Card class="h-full">
        <template #content>
            <Tabs value="0">
                <TabList>
                    <Tab value="0">{{ $t('views.playerList.onlinePlayers') }}</Tab>
                    <Tab value="1">{{ $t('views.playerList.historyPlayers') }}</Tab>
                </TabList>
                <TabPanels>
                    <TabPanel value="0">
                        <OnlinePlayer />
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
import OnlinePlayer from './OnlinePlayer/index.vue';
import HistoryPlayer from './HistoryPlayer/index.vue';
</script>
