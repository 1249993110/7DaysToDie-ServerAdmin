<template>
    <DataTable
        :value="tableData"
        v-model:selection="selectedRows"
        selectionMode="multiple"
        lazy
        @page="onPage"
        @sort="onSort"
        :loading="loading"
        paginator
        :rows="10"
        :rowsPerPageOptions="[5, 10, 20, 50, 100]"
        :totalRecords="totalRecords"
        paginatorTemplate="CurrentPageReport RowsPerPageDropdown FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink JumpToPageInput"
        :currentPageReportTemplate="`${$t('components.myTable.total', [totalRecords])}`"
        stripedRows
        scrollable
        :alwaysShowPaginator="false"
        scrollHeight="flex"
    >
        <template #header>
            <div class="flex justify-between">
                <div class="flex gap-2">
                    <Button type="button" outlined raised rounded v-tooltip.left="$t('components.myTable.batch')">
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
                        filter
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
        <Column v-if="isSelectable" selectionMode="multiple" headerStyle="width: 3rem"></Column>
        <Column v-for="col of selectedColumns" :field="col.field" :header="col.header" :key="col.field" :sortable="col.sortable"></Column>
    </DataTable>
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
});

const selectedColumns = ref(props.columns);
const onToggleColumns = (val) => {
    selectedColumns.value = props.columns.filter((col) => val.includes(col));
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

const loadLazyData = async () => {
    selectedRows.value = [];
    loading.value = true;
    try {
        const params = {
            pageNumber: first.value / rowsPerPage.value + 1,
            pageSize: rowsPerPage.value,
            sortField: sortField.value,
            sortOrder: sortOrder.value === 1 ? 'asc' : sortOrder.value === -1 ? 'desc' : null,
            keyword: searchQuery.value,
        };

        const data = await Promise.resolve(props.fetchData(params));

        tableData.value = data.items;
        totalRecords.value = data.total;
    } finally {
        loading.value = false;
    }
};
loadLazyData();

const onPage = (event) => {
    first.value = event.first;
    rowsPerPage.value = event.rows;
    sortField.value = event.sortField;
    sortOrder.value = event.sortOrder;
    loadLazyData();
};

const onSort = (event) => {
    first.value = 0;
    sortField.value = event.sortField;
    sortOrder.value = event.sortOrder;
    loadLazyData();
};

const onSearch = () => {
    first.value = 0;
    sortField.value = null;
    sortOrder.value = null;
    loadLazyData();
};
</script>
