<template>
    <div class="h-[calc(100vh-250px)]">
        <MyTable v-model:selection="selectedRows" dataKey="name" :columns="columns" :fetchData="fetchData"> </MyTable>
    </div>
</template>

<script setup>
import { promiseTimeout } from '@vueuse/core';

const data = ref([]);
data.value = Array.from({ length: 100 }, (_, i) => ({
    name: `Product ${i + 1}`,
    category: ['Electronics', 'Clothing', 'Books', 'Home'][i % 4],
    price: (Math.random() * 1000).toFixed(2),
    status: ['In Stock', 'Low Stock', 'Out of Stock'][i % 3],
}));

const columns = ref([
    { field: 'name', header: 'Name', sortable: true },
    { field: 'category', header: 'Category' },
    { field: 'price', header: 'Price' },
    { field: 'status', header: 'Status' },
]);

const selectedRows = ref([]);

const fetchData = async (params) => {
    await new promiseTimeout(500);
    let _data = data.value.filter(
        (item) =>
            item.name.toLowerCase().includes(params.keyword.toLowerCase()) ||
            item.category.toLowerCase().includes(params.keyword.toLowerCase()) ||
            item.status.toLowerCase().includes(params.keyword.toLowerCase())
    );

    return {
        items: _data.slice((params.pageNumber - 1) * params.pageSize, params.pageNumber * params.pageSize),
        total: _data.length,
    };
};
</script>
