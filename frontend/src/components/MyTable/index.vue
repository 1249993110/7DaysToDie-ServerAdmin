<template>
    <div class="size-full">
        <DataTable
            ref="tableRef"
            :value="tableData"
            v-model:selection="selectedRows"
            selectionMode="multiple"
            lazy
            @page="onPage"
            @sort="onSort"
            removableSort
            :loading="loading"
            paginator
            :rows="10"
            :rowsPerPageOptions="[5, 10, 20, 50, 100, 1000]"
            :totalRecords="totalRecords"
            paginatorTemplate="CurrentPageReport RowsPerPageDropdown FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink JumpToPageInput"
            :currentPageReportTemplate="`${$t('components.myTable.total', [totalRecords])}&nbsp;&nbsp;`"
            scrollable
            :alwaysShowPaginator="false"
            scrollHeight="flex"
            contextMenu
            v-model:contextMenuSelection="currentRow"
            @rowContextmenu="onRowContextMenu"
            v-bind="$attrs"
        >
            <template #header>
                <div class="flex justify-between">
                    <div class="flex gap-2">
                        <Button type="button" outlined raised rounded v-tooltip.left="$t('components.myTable.batch')" @click="onToggleBatchMenu">
                            <icon-ic:baseline-more-vert />
                        </Button>
                        <Button type="button" outlined raised rounded v-tooltip.right="$t('components.myTable.refresh')" @click="loadLazyData">
                            <icon-mdi:refresh />
                        </Button>
                    </div>
                    <div class="flex">
                        <IconField>
                            <InputIcon>
                                <icon-mdi:magnify />
                            </InputIcon>
                            <InputText :placeholder="$t('components.myTable.keywordSearch')" v-model="searchQuery" @keyup.enter="onSearch" />
                        </IconField>
                        <MultiSelect
                            class="ms-2"
                            :modelValue="selectedColumns"
                            :options="columns"
                            optionLabel="header"
                            @update:modelValue="onToggleColumns"
                            :maxSelectedLabels="0"
                            :placeholder="$t('components.myTable.view')"
                        />
                    </div>
                </div>
            </template>
            <template #empty>
                <div class="text-center text-gray-500 py-4">
                    {{ $t('components.myTable.noData') }}
                </div>
            </template>
            <Column v-if="isSelectable" selectionMode="multiple" headerStyle="width: 3rem" frozen></Column>
            <Column v-if="isShowIndex" header="#" headerStyle="width:3rem" frozen>
                <template #body="scope">
                    {{ scope.index + 1 }}
                </template>
            </Column>
            <Column v-for="col of selectedColumns" :key="col.field" v-bind="col">
                <template v-if="$slots[`${col.field}-body`]" #body="scope">
                    <slot :name="`${col.field}-body`" v-bind="scope" />
                </template>
            </Column>
            <Column v-if="isShowEditBtn || isShowDeleteBtn || isShowContextMenu" :exportable="false" frozen alignFrozen="right">
                <template #body="scope">
                    <Button v-if="isShowEditBtn" outlined rounded class="mr-2" @click="editProduct(scope.data)" size="small">
                        <icon-mdi:pencil />
                    </Button>
                    <Button v-if="isShowDeleteBtn" outlined rounded class="mr-2" severity="danger" @click="confirmDeleteProduct(scope.data)" size="small">
                        <icon-mdi:delete />
                    </Button>
                    <Button v-if="isShowContextMenu" outlined rounded @click="onToggleContextMenu($event, scope)" size="small">
                        <icon-ic:baseline-more-horiz />
                    </Button>
                </template>
            </Column>
        </DataTable>
        <TieredMenu ref="batchMenuRef" :model="batchMenuItems" popup>
            <template #itemicon="{ item }">
                <component :is="item.icon" />
            </template>
        </TieredMenu>
        <ContextMenu v-if="isShowContextMenu" ref="contextMenuRef" :model="contextMenuItems" popup @hide="currentRow = null">
            <template #itemicon="{ item }">
                <component :is="item.icon" />
            </template>
        </ContextMenu>
    </div>
</template>

<script setup>
const props = defineProps({
    columns: {
        type: Array,
        default: () => [],
    },
    fetchData: {
        type: Function,
        required: true,
    },
    isSelectable: {
        type: Boolean,
        default: true,
    },
    isShowIndex: {
        type: Boolean,
        default: false,
    },
    isShowEditBtn: {
        type: Boolean,
        default: true,
    },
    isShowDeleteBtn: {
        type: Boolean,
        default: true,
    },
    batchMenuItems: {
        type: Array,
        default: () => [],
    },
    contextMenuItems: {
        type: Array,
    },
    autoRefreshInterval: {
        type: Number,
        default: 0,
    },
});

const isShowContextMenu = computed(() => {
    return props.contextMenuItems && props.contextMenuItems.length;
});

const selectedColumns = ref();
watch(
    () => props.columns,
    (val) => {
        selectedColumns.value = val;
    },
    { immediate: true }
);
const onToggleColumns = (val) => {
    const fields = val.map((col) => col.field);
    selectedColumns.value = props.columns.filter((col) => fields.includes(col.field));
};

const selectedRows = defineModel('selectedRows', {
    type: Array,
    default: () => [],
});
const searchQuery = ref('');
const loading = ref(false);
const tableData = ref([]);
const totalRecords = ref(0);
const rowsPerPage = ref(10);
const first = ref(0);
const sortField = ref(null);
const sortOrder = ref(null);
const { t } = useI18n();

const loadLazyData = async () => {
    selectedRows.value = [];
    loading.value = true;
    try {
        const params = {
            pageNumber: first.value / rowsPerPage.value + 1,
            pageSize: rowsPerPage.value,
            order: sortField.value,
            desc: sortOrder.value === -1,
            keyword: searchQuery.value,
        };

        const data = await Promise.resolve(props.fetchData(params));

        tableData.value = data.items;
        totalRecords.value = data.total;
    } finally {
        loading.value = false;
    }
};

useIntervalFn(loadLazyData, () => props.autoRefreshInterval * 1000, { immediate: true, immediateCallback: true });

const onPage = async (event) => {
    first.value = event.first;
    rowsPerPage.value = event.rows;
    sortField.value = event.sortField;
    sortOrder.value = event.sortOrder;
    await loadLazyData();
};

const onSort = async (event) => {
    first.value = 0;
    sortField.value = event.sortField;
    sortOrder.value = event.sortOrder;
    await loadLazyData();
};

const onSearch = async () => {
    first.value = 0;
    sortField.value = null;
    sortOrder.value = null;
    await loadLazyData();
};

const tableRef = ref();
const batchMenuItems = computed(() => {
    let array = props.batchMenuItems;
    if (array.length) {
        array = array.concat({
            separator: true,
        });
    }
    return array.concat({
        label: t('components.myTable.exportCSV'),
        icon: markIcon(() => import('~icons/mdi/launch')),
        command: () => {
            tableRef.value.exportCSV();
        },
    });
});
const batchMenuRef = ref();
const onToggleBatchMenu = (event) => {
    batchMenuRef.value.toggle(event);
};

const currentRow = ref(null);
const contextMenuRef = ref();
const onToggleContextMenu = (event, scope) => {
    currentRow.value = scope.data;
    contextMenuRef.value.show(event);
};

const onRowContextMenu = (event) => {
    contextMenuRef.value.show(event.originalEvent);
};

defineExpose({ currentRow });
</script>
