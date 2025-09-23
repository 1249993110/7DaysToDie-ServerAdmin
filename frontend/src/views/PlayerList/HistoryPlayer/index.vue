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
            <template #lastLogin-body="{ data }"> {{ dayjs(data.lastLogin).format() }} </template>
            <template #position-body="{ data }"> {{ formatPosition(data.position) }} </template>
            <template #bedroll-body="{ data }"> {{ formatPosition(data.bedroll) }} </template>
            <template #isOffline-body="{ data }">
                <Tag :severity="data.isOffline ? 'danger' : 'success'">
                    {{ data.isOffline ? $t('common.offline') : $t('common.online') }}
                </Tag>
            </template>
        </MyTable>
        <PlayerInventoryDialog ref="playerInventoryDialogRef" />
        <PlayerSkillsDialog ref="playerSkillsDialogRef" />
        <PlayerDetailsDialog ref="playerDetailsDialogRef" />
    </div>
</template>

<script setup>
import { getHistoryPlayers } from '~/api/gameServer';
import dayjs from 'dayjs';
import serverFavoriteImgUrl from '~/assets/images/server_favorite.png';
import { formatPosition } from '~/utils';

const tableRef = ref();
const { t } = useI18n();
const columns = computed(() => [
    { field: 'playerName', header: t('views.playerList.playerName'), sortable: true, frozen: true, class: 'min-w-40' },
    { field: 'isOffline', header: t('views.playerList.status'), sortable: true, class: 'min-w-30' },
    // { field: 'entityId', header: t('views.playerList.entityId'), sortable: true, class: 'min-w-30' },
    { field: 'playGroup', header: t('views.playerList.playGroup'), sortable: true, class: 'min-w-35' },
    { field: 'lastLogin', header: t('views.playerList.lastLogin'), sortable: true, class: 'min-w-45' },
    { field: 'position', header: t('views.playerList.position'), class: 'min-w-40' },
    { field: 'playerId', header: t('views.playerList.playerId') },
    { field: 'platformId', header: t('views.playerList.platformId') },
    { field: 'permissionLevel', header: t('views.playerList.permissionLevel'), sortable: true, class: 'min-w-45' },
    { field: 'bedroll', header: t('views.playerList.bedroll'), class: 'min-w-40' },
]);

const selectedRows = ref([]);

const fetchData = async (params) => {
    return await getHistoryPlayers(params);
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
