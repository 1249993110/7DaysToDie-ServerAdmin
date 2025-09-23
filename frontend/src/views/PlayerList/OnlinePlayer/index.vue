<template>
    <div class="h-[calc(100vh-250px)]">
        <MyTable
            ref="tableRef"
            v-model:selection="selectedRows"
            dataKey="playerId"
            :columns="columns"
            :fetchData="fetchData"
            :batchMenuItems="batchMenuItems"
            :isShowEditBtn="false"
            :isShowDeleteBtn="false"
            :contextMenuItems="contextMenuItems"
        >
            <template #playerName-body="{ data }">
                <span class="flex items-center">
                    {{ data.playerName }}
                    <img v-if="data.isAdmin" :src="serverFavoriteImgUrl" width="20" :title="$t('views.playerList.admin')" />
                </span>
            </template>
            <template #position-body="{ data }"> {{ formatPosition(data.position) }} </template>
            <template #isTwitchEnabled-body="{ data }">
                {{ data.isTwitchEnabled ? $t('common.yes') : $t('common.no') }}
            </template>
        </MyTable>
        <PlayerInventoryDialog ref="playerInventoryDialogRef" />
        <PlayerSkillsDialog ref="playerSkillsDialogRef" />
        <PlayerDetailsDialog ref="playerDetailsDialogRef" />
    </div>
</template>

<script setup>
import { getOnlinePlayers } from '~/api/gameServer';
import serverFavoriteImgUrl from '~/assets/images/server_favorite.png';
import { formatPosition } from '~/utils';

const tableRef = ref();
const { t } = useI18n();
const columns = computed(() => [
    { field: 'playerName', header: t('views.playerList.playerName'), sortable: true, frozen: true, class: 'min-w-40' },
    { field: 'entityId', header: t('views.playerList.entityId'), sortable: true, class: 'min-w-30' },
    { field: 'position', header: t('views.playerList.position'), class: 'min-w-40' },
    { field: 'zombieKills', header: t('views.playerList.zombieKills'), sortable: true, class: 'min-w-40' },
    { field: 'playerKills', header: t('views.playerList.playerKills'), sortable: true, class: 'min-w-40' },
    { field: 'deaths', header: t('views.playerList.deaths'), sortable: true, class: 'min-w-30' },
    { field: 'level', header: t('views.playerList.level'), sortable: true, class: 'min-w-30' },
    { field: 'expToNextLevel', header: t('views.playerList.expToNextLevel'), class: 'min-w-40' },
    { field: 'skillPoints', header: t('views.playerList.skillPoints'), sortable: true, class: 'min-w-40' },
    { field: 'gameStage', header: t('views.playerList.gameStage'), sortable: true, class: 'min-w-40' },
    { field: 'ip', header: t('views.playerList.ip'), class: 'min-w-45' },
    { field: 'ping', header: t('views.playerList.ping'), sortable: true, class: 'min-w-30' },
    { field: 'isTwitchEnabled', header: t('views.playerList.isTwitchEnabled'), class: 'min-w-40' },
    { field: 'playerId', header: t('views.playerList.playerId'), class: 'min-w-40' },
    { field: 'platformId', header: t('views.playerList.platformId'), class: 'min-w-40' },
    { field: 'permissionLevel', header: t('views.playerList.permissionLevel'), sortable: true, class: 'min-w-45' },
]);

const selectedRows = ref([]);

const fetchData = async (params) => {
    return await getOnlinePlayers(params);
};

const batchMenuItems = ref([]);
const playerInventoryDialogRef = ref();
const playerSkillsDialogRef = ref();
const playerDetailsDialogRef = ref();

const currentRowData = computed(() => tableRef.value?.currentRow);
const contextMenuItems = computed(() => [
    {
        label: t('views.playerList.viewInventory'),
        command: () => {
            playerInventoryDialogRef.value.show(currentRowData.value.playerId, currentRowData.value.playerName);
        },
    },
    {
        label: t('views.playerList.viewSkills'),
        command: () => {
            playerSkillsDialogRef.value.show(currentRowData.value.playerId, currentRowData.value.playerName);
        },
    },
    {
        label: t('views.playerList.viewDetails'),
        command: () => {
            playerDetailsDialogRef.value.show(currentRowData.value.playerId, currentRowData.value.playerName);
        },
    },
]);
</script>
