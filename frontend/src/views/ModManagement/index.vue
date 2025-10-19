<template>
    <div class="h-[calc(100vh-130px)]">
        <MyTable ref="tableRef" :columns="columns" :fetchData="fetchData" :isSelectable="false" :isShowAddBtn="false" :isShowEditBtn="false" :isShowDeleteBtn="false">
            <template #displayName-body="{ data }">
                <div class="flex flex-col gap-1">
                    <span class="font-semibold">{{ data.displayName || data.name }}</span>
                    <span v-if="data.description" class="text-xs text-gray-500 dark:text-gray-300">{{ data.description }}</span>
                    <span class="text-xs text-gray-500 dark:text-gray-300">{{ data.folderName }}</span>
                </div>
            </template>
            <template #website-body="{ data }">
                <a v-if="data.website" :href="data.website" class="text-primary underline decoration-dotted" target="_blank" rel="noopener">
                    {{ data.website }}
                </a>
                <span v-else class="text-gray-500 dark:text-gray-300">--</span>
            </template>
            <template #isLoaded-body="{ data }">
                <Tag :severity="data.isLoaded ? 'success' : 'danger'">
                    {{ data.isLoaded ? t('views.modManagement.status.loaded') : t('views.modManagement.status.unloaded') }}
                </Tag>
            </template>
            <template #isUninstalled-body="{ data }">
                <Tag :severity="data.isUninstalled ? 'warning' : 'info'">
                    {{ data.isUninstalled ? t('views.modManagement.status.uninstalled') : t('views.modManagement.status.installed') }}
                </Tag>
            </template>
            <template #actions-body="{ data }">
                <div class="flex justify-center">
                    <Button
                        size="small"
                        outlined
                        :loading="isPending(getModKey(data))"
                        :label="data.isUninstalled ? t('views.modManagement.actions.install') : t('views.modManagement.actions.uninstall')"
                        @click="onToggleStatus(data)"
                    >
                        <template #icon>
                            <icon-mdi:power v-if="data.isUninstalled" />
                            <icon-mdi:power-plug v-else />
                        </template>
                    </Button>
                </div>
            </template>
        </MyTable>
    </div>
</template>

<script setup>
import { useToast } from 'primevue/usetoast';
import { getMods, toggleModStatus } from '~/api/gameServer';
import { searchByKeyword, orderByField } from '~/utils/index';

const { t } = useI18n();
const toast = useToast();

const tableRef = ref();

const columns = computed(() => [
    { field: 'displayName', header: t('views.modManagement.columns.displayName'), sortable: true, class: 'min-w-60' },
    { field: 'author', header: t('views.modManagement.columns.author'), sortable: true, class: 'min-w-40' },
    { field: 'version', header: t('views.modManagement.columns.version'), sortable: true, class: 'min-w-28' },
    { field: 'website', header: t('views.modManagement.columns.website'), class: 'min-w-60' },
    { field: 'isLoaded', header: t('views.modManagement.columns.loaded'), class: 'min-w-32 text-center' },
    { field: 'isUninstalled', header: t('views.modManagement.columns.uninstalled'), class: 'min-w-36 text-center' },
    { field: 'actions', header: t('views.modManagement.columns.actions'), exportable: false, class: 'min-w-40 text-center' },
]);

const pendingIds = ref(new Set());

const getModKey = (mod) => mod?.folderName || mod?.name;

const isPending = (id) => pendingIds.value.has(id);

const setPending = (id, value) => {
    const next = new Set(pendingIds.value);
    if (value) {
        next.add(id);
    } else {
        next.delete(id);
    }
    pendingIds.value = next;
};

const fetchData = async (params) => {
    const response = await getMods();
    const list = Array.isArray(response) ? response : response?.items ?? [];
    let data = searchByKeyword(list, params.keyword, ['displayName', 'name', 'author', 'folderName', 'description']);
    data = orderByField(data, params.order, params.desc);
    return { items: data.slice((params.pageNumber - 1) * params.pageSize, params.pageNumber * params.pageSize), total: data.length };
};

const onToggleStatus = async (mod) => {
    const key = getModKey(mod);
    if (!key || isPending(key)) {
        return;
    }

    setPending(key, true);
    try {
        await toggleModStatus(key);
        toast.add({
            severity: 'success',
            summary: t('views.modManagement.toast.title'),
            detail: t('views.modManagement.toast.toggleSuccess'),
            life: 2000,
        });
        tableRef.value?.reload();
    } catch (error) {
        console.error(error);
    } finally {
        setPending(key, false);
    }
};

defineExpose({ reload: () => tableRef.value?.reload() });
</script>
