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
                    <Monitor :timestamp="systemMetricsSnapshot.timestamp" :cpuTimes="systemMetricsSnapshot.cpuTimes" :networkInfos="systemMetricsSnapshot.networkInfos" />
                </MyCard>
            </div>
            <div class="col-span-12 xl:col-span-4">
                <MyCard :header="$t('views.dashboard.headers.recentActivity')">
                    <div>
                        <p class="m-0">Online</p>
                    </div>
                </MyCard>
                <MyCard :header="$t('views.dashboard.headers.systemPlatformInfo')" class="mt-4">
                    <div class="system-info">
                        <span>{{ $t('views.dashboard.systemInfo.deviceName') }}</span>
                        <span :title="systemPlatformInfo.deviceName">{{ systemPlatformInfo.deviceName }}</span>
                        <span>{{ $t('views.dashboard.systemInfo.deviceModel') }}</span>
                        <span :title="systemPlatformInfo.deviceModel">{{ systemPlatformInfo.deviceModel }}</span>
                        <span>{{ $t('views.dashboard.systemInfo.deviceType') }}</span>
                        <span :title="systemPlatformInfo.deviceType">{{ systemPlatformInfo.deviceType }}</span>
                        <span>{{ $t('views.dashboard.systemInfo.deviceUniqueIdentifier') }}</span>
                        <span :title="systemPlatformInfo.deviceUniqueIdentifier">{{ systemPlatformInfo.deviceUniqueIdentifier }}</span>
                        <span>{{ $t('views.dashboard.systemInfo.operatingSystem') }}</span>
                        <span :title="systemPlatformInfo.operatingSystem">{{ systemPlatformInfo.operatingSystem }}</span>
                        <span>{{ $t('views.dashboard.systemInfo.processorType') }}</span>
                        <span :title="systemPlatformInfo.processorType">{{ systemPlatformInfo.processorType }}</span>
                        <span>{{ $t('views.dashboard.systemInfo.processorCount') }}</span>
                        <span :title="systemPlatformInfo.processorCount">{{ systemPlatformInfo.processorCount }}</span>
                        <span>{{ $t('views.dashboard.systemInfo.systemMemorySize') }}</span>
                        <span :title="Math.round(systemPlatformInfo.systemMemorySize / 1024) + ' GB'">{{ Math.round(systemPlatformInfo.systemMemorySize / 1024) }} GB</span>
                        <span>{{ $t('views.dashboard.systemInfo.userName') }}</span>
                        <span :title="systemPlatformInfo.userName">{{ systemPlatformInfo.userName }}</span>
                    </div>
                </MyCard>
            </div>
        </div>
    </div>
</template>

<script setup>
import Overview from './Overview/index.vue';
import Status from './Status/index.vue';
import Monitor from './Monitor/index.vue';
import * as gameServerApi from '~/api/gameServer';
import * as devicesApi from '~/api/devices';

const gameServerStats = ref({});
const systemMetricsSnapshot = ref({});
const systemPlatformInfo = ref({});

devicesApi.getSystemPlatformInfo().then((data) => {
    systemPlatformInfo.value = data;
});

const { pause, resume, isActive } = useIntervalFn(
    async () => {
        gameServerStats.value = await gameServerApi.getStats();
        systemMetricsSnapshot.value = await devicesApi.getSystemMetricsSnapshot();
    },
    3000,
    { immediateCallback: true }
);

onActivated(resume);
onDeactivated(pause);
</script>

<style lang="scss" scoped>
.system-info {
    --uno: 'grid grid-cols-[auto_1fr] gap-4';
    span {
        &:nth-child(even) {
            --uno: 'text-p-text-muted-color overflow-hidden whitespace-nowrap text-ellipsis';
        }
    }
}
</style>
