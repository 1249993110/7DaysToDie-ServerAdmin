<template>
    <div class="grid grid-cols-2 gap-4">
        <div>CPU</div>
        <Chart type="line" :data="chartData" :options="chartOptions" class="w-full h-60" />
    </div>
</template>

<script setup>
const props = defineProps({
    timestamp: {
        type: String,
    },
    cpuTimes: {
        type: Object,
    },
    networkInfos: {
        type: Array,
    },
});

const { t } = useI18n();
const appStore = useAppStore();
const unit = ref('');

const getChartOptions = () => {
    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--p-text-color');
    const textColorSecondary = documentStyle.getPropertyValue('--p-text-muted-color');
    const surfaceBorder = documentStyle.getPropertyValue('--p-content-border-color');

    return {
        responsive: true,
        maintainAspectRatio: false,
        animation: {
            duration: 0, // Disable animations for smooth real-time updates
        },
        plugins: {
            tooltip: {
                enabled: true,
                rtl: appStore.isRTL,
                callbacks: {
                    label: (context) => {
                        return `${context.parsed.y} ${unit.value}`;
                    },
                },
            },
            legend: {
                rtl: appStore.isRTL,
                labels: {
                    color: textColor,
                },
            },
        },
        scales: {
            x: {
                title: {
                    display: true,
                    text: t('views.dashboard.monitor.time'),
                },
                ticks: {
                    color: textColorSecondary,
                },
                grid: {
                    color: surfaceBorder,
                },
                min: 0,
            },
            y: {
                ticks: {
                    color: textColorSecondary,
                    callback: (value) => {
                        return value + ' Kb';
                    },
                },
                grid: {
                    color: surfaceBorder,
                },
                title: {
                    display: true,
                    text: t('views.dashboard.monitor.network'),
                },
                min: 0,
            },
        },
    };
};

const chartData = ref();
const chartOptions = ref();

onMounted(() => {
    chartOptions.value = getChartOptions();
});

watch(
    () => [props.timestamp, props.networkInfos],
    (newVal, oldVal) => {
        const lastDate = dayjs(oldVal[0]);
        const length = 8;

        if (!oldVal[0]) {
            const labels = Array.from({ length: length }, (_, index) => lastDate.subtract((length - index - 1) * 3, 'seconds').format('HH:mm:ss'));
            chartData.value = {
                labels: labels,
                datasets: [
                    {
                        label: t('views.dashboard.monitor.trafficIn'),
                        data: Array.from({ length: length }, () => 0),
                        fill: true,
                        tension: 0.4,
                        borderColor: '#1F77B4',
                        backgroundColor: 'rgba(107, 114, 128, 0.2)',
                    },
                    {
                        label: t('views.dashboard.monitor.trafficOut'),
                        data: Array.from({ length: length }, () => 0),
                        fill: true,
                        borderColor: '#2CA02C',
                        tension: 0.4,
                        backgroundColor: 'rgba(107, 114, 128, 0.2)',
                    },
                ],
            };
            return;
        }

        const currentDate = dayjs(newVal[0]);
        const timeDifferenceInSeconds = currentDate.diff(lastDate, 'millisecond');

        const newBytesReceived = newVal[1].map((i) => i.bytesReceived).reduce((a, b) => a + b, 0);
        const oldBytesReceived = oldVal[1].map((i) => i.bytesReceived).reduce((a, b) => a + b, 0);
        const newBytesSent = newVal[1].map((i) => i.bytesSent).reduce((a, b) => a + b, 0);
        const oldBytesSent = oldVal[1].map((i) => i.bytesSent).reduce((a, b) => a + b, 0);

        const downloadSpeedInBytes = newBytesReceived - oldBytesReceived;
        // 将字节转换为兆比特（Mb）
        const downloadSpeedInMbps = (downloadSpeedInBytes * 8) / ((1024 * timeDifferenceInSeconds) / 1000);

        // 计算上行（上传）网速
        const uploadSpeedInBytes = newBytesSent - oldBytesSent;
        // 将字节转换为兆比特（Mb）
        const uploadSpeedInMbps = (uploadSpeedInBytes * 8) / ((1024 * timeDifferenceInSeconds) / 1000);

        var chartDataVal = chartData.value;
        chartDataVal.labels = chartDataVal.labels.slice(1).concat(currentDate.format('HH:mm:ss'));
        chartDataVal.datasets[0].data = chartDataVal.datasets[0].data.slice(1).concat(downloadSpeedInMbps.toFixed(2));
        chartDataVal.datasets[1].data = chartDataVal.datasets[1].data.slice(1).concat(uploadSpeedInMbps.toFixed(2));
    }
);

const localeStore = useLocaleStore();
watch([() => appStore.primaryColor, () => appStore.isRTL, () => appStore.isDark, () => localeStore.lang], async () => {
    await nextTick();
    chartOptions.value = getChartOptions();
});
</script>
