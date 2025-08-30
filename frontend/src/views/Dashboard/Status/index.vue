<template>
    <div class="grid grid-cols-5 gap-4">
        <Doughnut
            :title="$t('views.dashboard.status.players')"
            :used="playerStatus.used"
            :free="playerStatus.free"
            :centerText="playerStatus.centerText"
            :legendLabels="playerLegend"
        />
        <Doughnut
            :title="$t('views.dashboard.status.zombies')"
            :used="zombieStatus.used"
            :free="zombieStatus.free"
            :centerText="zombieStatus.centerText"
            :legendLabels="zombieLegend"
        />
        <Doughnut
            :title="$t('views.dashboard.status.animals')"
            :used="animalStatus.used"
            :free="animalStatus.free"
            :centerText="animalStatus.centerText"
            :legendLabels="animalLegend"
        />
        <Doughnut
            :title="$t('views.dashboard.status.memory')"
            :used="memoryStatus.used"
            :free="memoryStatus.free"
            :centerText="memoryStatus.centerText"
            :legendLabels="memoryLegend"
            unit="MB"
        />
        <Doughnut
            :title="$t('views.dashboard.status.disk')"
            :used="diskStatus.used"
            :free="diskStatus.free"
            :centerText="diskStatus.centerText"
            :legendLabels="diskLegend"
            unit="MB"
        />
    </div>
</template>

<script setup>
import Doughnut from './Doughnut.vue';

const props = defineProps({
    gameServerStats: {
        type: Object,
        required: true,
    },
    systemMetricsSnapshot: {
        type: Object,
        required: true,
    },
});

const { t } = useI18n();
const playerStatus = reactive({});
const playerLegend = computed(() => [t('views.dashboard.status.onlinePlayers'), t('views.dashboard.status.freeSlots')]);
const zombieStatus = reactive({});
const zombieLegend = computed(() => [t('views.dashboard.status.activeZombies'), t('views.dashboard.status.freeSlots')]);
const animalStatus = reactive({});
const animalLegend = computed(() => [t('views.dashboard.status.activeAnimals'), t('views.dashboard.status.freeSlots')]);
const memoryStatus = reactive({});
const memoryLegend = computed(() => [t('views.dashboard.status.usedMemory'), t('views.dashboard.status.freeMemory')]);
const diskStatus = reactive({});
const diskLegend = computed(() => [t('views.dashboard.status.usedDisk'), t('views.dashboard.status.freeDisk')]);

watch(
    () => props.gameServerStats,
    (newStats) => {
        playerStatus.used = newStats.onlinePlayers;
        playerStatus.free = newStats.maxOnlinePlayers - newStats.onlinePlayers;
        playerStatus.centerText = `${newStats.onlinePlayers} / ${newStats.maxOnlinePlayers}`;

        zombieStatus.used = newStats.zombies;
        zombieStatus.free = newStats.maxZombies - newStats.zombies;
        zombieStatus.centerText = `${newStats.zombies} / ${newStats.maxZombies}`;

        animalStatus.used = newStats.animals;
        animalStatus.free = newStats.maxAnimals - newStats.animals;
        animalStatus.centerText = `${newStats.animals} / ${newStats.maxAnimals}`;
    }
);

watch(
    () => props.systemMetricsSnapshot,
    (newMetrics) => {
        memoryStatus.used = bytesToMB(newMetrics.memoryInfo.totalPhysicalMemory - newMetrics.memoryInfo.availablePhysicalMemory);
        memoryStatus.free = bytesToMB(newMetrics.memoryInfo.availablePhysicalMemory);
        memoryStatus.centerText = `${newMetrics.memoryInfo.usedPercentage} %`;

        diskStatus.used = bytesToMB(newMetrics.diskInfos.map((i) => i.usedSize).reduce((a, b) => a + b, 0));
        diskStatus.free = bytesToMB(newMetrics.diskInfos.map((i) => i.freeSpace).reduce((a, b) => a + b, 0));
        diskStatus.centerText = `${((diskStatus.used / (diskStatus.free + diskStatus.used)) * 100) | 0} %`;
    }
);
</script>
