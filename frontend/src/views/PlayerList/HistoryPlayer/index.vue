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
                    <img v-if="data.isAdmin" :src="serverFavoriteImgUrl" class="size-5" :title="$t('views.playerList.title.admin')" />
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
    { field: 'playerName', header: t('views.playerList.header.playerName'), sortable: true, frozen: true, class: 'min-w-40' },
    { field: 'isOffline', header: t('views.playerList.header.status'), sortable: true, class: 'min-w-30' },
    // { field: 'entityId', header: t('views.playerList.header.entityId'), sortable: true, class: 'min-w-30' },
    { field: 'playGroup', header: t('views.playerList.header.playGroup'), sortable: true, class: 'min-w-35' },
    { field: 'lastLogin', header: t('views.playerList.header.lastLogin'), sortable: true, class: 'min-w-45' },
    { field: 'position', header: t('views.playerList.header.position'), class: 'min-w-40' },
    { field: 'permissionLevel', header: t('views.playerList.header.permissionLevel'), sortable: true, class: 'min-w-45' },
    { field: 'playerId', header: t('views.playerList.header.playerId') },
    { field: 'platformId', header: t('views.playerList.header.platformId') },
    { field: 'bedroll', header: t('views.playerList.header.bedroll'), class: 'min-w-40' },
]);

const selectedRows = ref([]);

const fetchData = async (params) => {
    return await getHistoryPlayers(params);
};

const batchMenuItems = ref([]);

const currentRowData = computed(() => tableRef.value?.currentRow);
const contextMenuItems = ref([
    {
        label: 'View Details',
        command: () => {
            console.log('View Details', currentRowData.value);
        },
    },
    {
        label: 'Ban Player',
        command: () => {
            console.log('Ban Player', currentRowData.value);
        },
    },
]);
</script>
