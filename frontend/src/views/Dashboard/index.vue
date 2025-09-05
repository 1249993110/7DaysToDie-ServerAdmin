<template>
    <div class="size-full">
        <div class="grid grid-cols-12 gap-4">
            <div class="col-span-12 xl:col-span-8">
                <MyCard :header="$t('views.dashboard.headers.overview')">
                    <Overview :model="gameServerStats" />
                </MyCard>
                <MyCard :header="$t('views.dashboard.headers.status')" class="mt-4">
                    <Status :gameServerStats="gameServerStats" :systemMetricsSnapshot="systemMetricsSnapshot" />
                </MyCard>
                <MyCard :header="$t('views.dashboard.headers.monitor')" class="mt-4">
                    <Monitor :timestamp="systemMetricsSnapshot.timestamp" :cpuTimes="systemMetricsSnapshot.cpuTimes" :memoryInfo="systemMetricsSnapshot.memoryInfo" :networkInfos="systemMetricsSnapshot.networkInfos" />
                </MyCard>
            </div>
            <div class="col-span-12 xl:col-span-4">
                <MyCard :header="$t('views.dashboard.headers.recentActivity')" >
                    <RecentActivity />
                </MyCard>
                <div class="grid grid-cols-6 gap-4 mt-4">
                    <MyCard :header="$t('views.dashboard.headers.historyPlayers')" class="stats-content 3xl:whitespace-nowrap">
                        <span>
                            {{ gameServerStats.historyPlayers ?? $t('common.unknown') }}
                        </span>
                    </MyCard>
                    <MyCard :header="$t('views.dashboard.headers.entities')" class="stats-content">
                        <span>
                            {{ gameServerStats.entities ?? $t('common.unknown') }}
                        </span>
                    </MyCard>
                    <MyCard :header="$t('views.dashboard.headers.fps')" class="stats-content">
                        <span>
                            {{ gameServerStats.fps?.toFixed(1) ?? $t('common.unknown') }}
                        </span>
                    </MyCard>
                    <MyCard :header="$t('views.dashboard.headers.residentSetSize')" class="stats-content">
                        <span>{{ gameServerStats.residentSetSize?.toFixed() ?? $t('common.unknown') }} MB</span>
                    </MyCard>
                    <MyCard :header="$t('views.dashboard.headers.heap')" class="stats-content">
                        <span>{{ gameServerStats.heap?.toFixed() ?? $t('common.unknown') }} MB</span>
                    </MyCard>
                    <MyCard :header="$t('views.dashboard.headers.chunks')" class="stats-content">
                        <span>
                            {{ gameServerStats.chunks ?? $t('common.unknown') }}
                        </span>
                    </MyCard>
                </div>
                <MyCard :header="$t('views.dashboard.headers.systemPlatformInfo')" class="mt-4">
                    <SystemInfo :model="systemPlatformInfo" />
                </MyCard>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'Dashboard',
};
</script>

<script setup>
import Overview from './Overview/index.vue';
import RecentActivity from './RecentActivity/index.vue';
import Status from './Status/index.vue';
import Monitor from './Monitor/index.vue';
import SystemInfo from './SystemInfo/index.vue';
import * as gameServerApi from '~/api/gameServer';
import * as devicesApi from '~/api/devices';

const gameServerStats = ref({});
const systemMetricsSnapshot = ref({});
const systemPlatformInfo = ref({});

devicesApi.getSystemPlatformInfo().then((data) => {
    systemPlatformInfo.value = data;
});

const { pause, resume, isActive } = useIntervalFn(
    () => {
        gameServerApi
            .getStats()
            .then((data) => {
                gameServerStats.value = data;
            })
            .catch((error) => {});
        devicesApi
            .getSystemMetricsSnapshot()
            .then((data) => {
                systemMetricsSnapshot.value = data;
            })
            .catch((error) => {});
    },
    3000,
    { immediateCallback: true }
);

onActivated(() => {
    if (!isActive.value) {
        resume();
    }
});
onDeactivated(pause);
</script>

<style lang="scss" scoped>
.stats-content {
    --uno: 'lg:col-span-2 xl:col-span-3 2xl:col-span-3 3xl:col-span-2';
    span {
        --uno: 'text-p-primary-color ms-2';
    }
}
</style>
