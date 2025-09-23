<template>
    <Dialog class="w-[64vw]" v-model:visible="visible" maximizable modal :header="$t('components.playerDetailsDialog.header')" @hide="modelValue = []">
        <div v-if="loading" class="f-center h-[50vh]">
            <ProgressSpinner />
        </div>
        <DataView v-else :value="[{}]">
            <template #header>
                <div class="flex justify-between items-center gap-4">
                    <span>{{ title }}</span>
                </div>
            </template>
            <template #list>
                <div class="grid grid-cols-2 overflow-auto">
                    <DataTable :value="leftTableData" showGridlines :pt="{ headerRow: { class: '!hidden' } }">
                        <Column field="label" class="max-w-[calc(16vw-16px)] overflow-hidden font-semibold"></Column>
                        <Column field="value" class="max-w-[calc(16vw-16px)] overflow-hidden t-bg-1"></Column>
                    </DataTable>
                    <DataTable :value="rightTableData" showGridlines :pt="{ headerRow: { class: '!hidden' } }">
                        <Column field="label" class="max-w-[calc(16vw-16px)] overflow-hidden font-semibold"></Column>
                        <Column field="value" class="max-w-[calc(16vw-16px)] overflow-hidden t-bg-1"></Column>
                    </DataTable>
                </div>
            </template>
        </DataView>
    </Dialog>
</template>

<script setup>
import { getPlayerDetails } from '~/api/gameServer';

const { t } = useI18n();

const modelValue = ref([]);
const leftTableData = computed(() => {
    const mid = Math.ceil(modelValue.value.length / 2);
    return modelValue.value.slice(0, mid);
});
const rightTableData = computed(() => {
    const mid = Math.ceil(modelValue.value.length / 2);
    return modelValue.value.slice(mid);
});
const visible = ref(false);
const loading = ref(false);
const title = ref('');

const formatDayTime = (days, time) => {
    return `${days} ${t('common.day', days)} ${time} ${t('common.hour', time)}`;
};

const show = async (playerId, playerName) => {
    title.value = `${playerName} (${playerId})`;
    loading.value = true;
    visible.value = true;
    try {
        modelValue.value = getModel(await getPlayerDetails(playerId));
    } finally {
        loading.value = false;
    }
};

defineExpose({
    show,
});

const getModel = (data) => {
    const result = [
        {
            label: t('components.playerDetailsDialog.playerName'),
            value: data.playerName,
        },
        {
            label: t('components.playerDetailsDialog.entityId'),
            value: data.entityId,
        },
        {
            label: t('components.playerDetailsDialog.playerId'),
            value: data.playerId,
        },
        {
            label: t('components.playerDetailsDialog.platformId'),
            value: data.platformId,
        },
        {
            label: t('components.playerDetailsDialog.playGroup'),
            value: data.playGroup,
        },
        {
            label: t('components.playerDetailsDialog.isAdmin'),
            value: data.isAdmin ? t('common.yes') : t('common.no'),
        },
        {
            label: t('components.playerDetailsDialog.isOnline'),
            value: !data.isOffline ? t('common.yes') : t('common.no'),
        },
        {
            label: t('components.playerDetailsDialog.ip'),
            value: data.ip,
        },
        {
            label: t('components.playerDetailsDialog.ping'),
            value: data.ping,
        },
        {
            label: t('components.playerDetailsDialog.position'),
            value: formatPosition(data.position),
        },
        {
            label: t('components.playerDetailsDialog.lastSpawnPosition'),
            value: formatPosition(data.lastSpawnPosition),
        },
        {
            label: t('components.playerDetailsDialog.gameStage'),
            value: data.gameStage,
        },
        {
            label: t('components.playerDetailsDialog.lastLogin'),
            value: dayjs(data.lastLogin).format(),
        },
        {
            label: t('components.playerDetailsDialog.playerKills'),
            value: data.playerKills,
        },
        {
            label: t('components.playerDetailsDialog.zombieKills'),
            value: data.zombieKills,
        },
        {
            label: t('components.playerDetailsDialog.deaths'),
            value: data.deaths,
        },
        {
            label: t('components.playerDetailsDialog.score'),
            value: data.score,
        },
        {
            label: t('components.playerDetailsDialog.health'),
            value: data.stats.health.toFixed(1),
        },
        {
            label: t('components.playerDetailsDialog.stamina'),
            value: data.stats.stamina.toFixed(1),
        },
        // {
        //     label: t('components.playerDetailsDialog.coreTemp'),
        //     value: data.stats.coreTemp.toFixed(1),
        // },
        {
            label: t('components.playerDetailsDialog.food'),
            value: data.stats.food.toFixed(1),
        },
        {
            label: t('components.playerDetailsDialog.water'),
            value: data.stats.water.toFixed(1),
        },
        {
            label: t('components.playerDetailsDialog.level'),
            value: data.level,
        },
        {
            label: t('components.playerDetailsDialog.expToNextLevel'),
            value: data.expToNextLevel,
        },
        {
            label: t('components.playerDetailsDialog.skillPoints'),
            value: data.skillPoints,
        },
        {
            label: t('components.playerDetailsDialog.isLandProtectionActive'),
            value: data.isLandProtectionActive ? t('components.playerDetailsDialog.active') : t('components.playerDetailsDialog.inactive'),
        },
        {
            label: t('components.playerDetailsDialog.distanceWalked'),
            value: data.distanceWalked.toFixed(1),
        },
        {
            label: t('components.playerDetailsDialog.totalItemsCrafted'),
            value: data.totalItemsCrafted,
        },
        {
            label: t('components.playerDetailsDialog.totalTimePlayed'),
            value: formatMinute(data.totalTimePlayed),
        },
        {
            label: t('components.playerDetailsDialog.currentLife'),
            value: formatMinute(data.currentLife),
        },
        {
            label: t('components.playerDetailsDialog.longestLife'),
            value: formatMinute(data.longestLife),
        },
        {
            label: t('components.playerDetailsDialog.alreadyCraftedList'),
            value: data.alreadyCraftedList,
        },
        {
            label: t('components.playerDetailsDialog.unlockedRecipeList'),
            value: data.unlockedRecipeList,
        },
        {
            label: t('components.playerDetailsDialog.rentedVMPosition'),
            value: formatPosition(data.rentedVMPosition),
        },
        {
            label: t('components.playerDetailsDialog.rentalEndDayTime'),
            value: formatDayTime(data.rentalEndDay, data.rentalEndTime),
        },
        {
            label: t('components.playerDetailsDialog.bedroll'),
            value: formatPosition(data.bedroll),
        },
    ];

    return result.filter((i) => i.value);
};
</script>
