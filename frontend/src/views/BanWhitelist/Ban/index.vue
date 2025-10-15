<template>
    <div class="h-[calc(100vh-250px)]">
        <MyTable
            ref="tableRef"
            v-model:selection="selectedRows"
            dataKey="playerId"
            :columns="columns"
            :fetchData="fetchData"
            :batchMenuItems="batchMenuItems"
            isShowIndex
            isShowEditBtn
            @add="onAdd"
            @edit="onEdit"
            @delete="onDelete"
        >
            <template #bannedUntil-body="{ data }"> {{ dayjs(data.bannedUntil).format() }} </template>
        </MyTable>
        <AddOrEditDialog ref="addOrEditDialogRef" :editData="editData" @saved="onSaved" />
    </div>
</template>

<script setup>
import * as api from '~/api/gameServer';
import dayjs from 'dayjs';
import AddOrEditDialog from './AddOrEditDialog/index.vue';
import { myConfirm } from '~/plugins/sweetalert2';
import { searchByKeyword, orderByField } from '~/utils/index';

const tableRef = ref();
const addOrEditDialogRef = ref();
const { t } = useI18n();
const editData = ref(null);

const columns = computed(() => [
    { field: 'playerId', header: t('views.banWhitelist.playerId'), class: 'min-w-40' },
    { field: 'displayName', header: t('views.banWhitelist.displayName'), sortable: true, class: 'min-w-40' },
    { field: 'bannedUntil', header: t('views.banWhitelist.bannedUntil'), sortable: true, class: 'min-w-40' },
    { field: 'reason', header: t('views.banWhitelist.reason') },
]);

const selectedRows = ref([]);

const fetchData = async (params) => {
    let data = await api.getBannedPlayers(params);
    data = searchByKeyword(data, params.keyword, ['playerId', 'displayName', 'reason']);
    data = orderByField(data, params.order, params.desc);
    return data;
};

const batchMenuItems = computed(() => [
    {
        icon: markIcon(() => import('~icons/mdi/delete-sweep')),
        label: t('common.batchDelete'),
        disabled: selectedRows.value.length === 0,
        command: async () => {
            if (await myConfirm()) {
                await api.unbanPlayers(selectedRows.value.map((row) => row.playerId));
                tableRef.value.reload();
            }
        },
    },
]);

const onAdd = () => {
    editData.value = null;
    addOrEditDialogRef.value.show();
};

const onEdit = (rowData) => {
    editData.value = rowData;
    addOrEditDialogRef.value.show();
};

const onDelete = async (rowData) => {
    await api.unbanPlayers([rowData.playerId]);
    tableRef.value.reload();
};

const onSaved = () => {
    tableRef.value.reload();
    editData.value = null;
};
</script>
