<template>
    <div class="h-[calc(100vh-250px)]">
        <MyTable
            ref="tableRef"
            v-model:selection="selectedRows"
            dataKey="command"
            :columns="columns"
            :fetchData="fetchData"
            :batchMenuItems="batchMenuItems"
            isShowIndex
            isShowEditBtn
            isShowAddBtn
            @add="onAdd"
            @edit="onEdit"
            @delete="onDelete"
        >
        </MyTable>
        <AddOrEditDialog ref="addOrEditDialogRef" :editData="editData" @saved="onSaved" />
    </div>
</template>

<script setup>
import * as api from '~/api/gameServer';
import AddOrEditDialog from './AddOrEditDialog/index.vue';
import { myConfirm } from '~/plugins/sweetalert2';
import { searchByKeyword, orderByField } from '~/utils/index';

const tableRef = ref();
const addOrEditDialogRef = ref();
const { t } = useI18n();
const editData = ref(null);

const columns = computed(() => [
    { field: 'command', header: t('views.permission.command'), class: 'min-w-40' },
    { field: 'permissionLevel', header: t('views.permission.permissionLevel'), sortable: true, class: 'min-w-50' },
    { field: 'description', header: t('views.permission.description'), class: 'min-w-40' },
]);

const selectedRows = ref([]);

const fetchData = async (params) => {
    const response = await api.getCommandPermissions(params);
    const transform = (list) => orderByField(searchByKeyword(list, params.keyword, ['command', 'description']), params.order, params.desc);

    if (response?.items && Array.isArray(response.items)) {
        const items = transform(response.items);
        return {
            items,
            total: response.total ?? items.length,
        };
    }

    if (response?.data && Array.isArray(response.data)) {
        const items = transform(response.data);
        return {
            items,
            total: response.total ?? items.length,
        };
    }

    const list = Array.isArray(response) ? response : [];
    const data = transform(list);
    return { items: data.slice((params.pageNumber - 1) * params.pageSize, params.pageNumber * params.pageSize), total: data.length };
};

const batchMenuItems = computed(() => [
    {
        icon: markIcon(() => import('~icons/mdi/delete-sweep')),
        label: t('common.batchDelete'),
        disabled: selectedRows.value.length === 0,
        command: async () => {
            if (await myConfirm()) {
                await api.deleteCommandPermissions(selectedRows.value.map((row) => row.command));
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
    await api.deleteCommandPermissions([rowData.command]);
    tableRef.value.reload();
};

const onSaved = () => {
    tableRef.value.reload();
    editData.value = null;
};
</script>