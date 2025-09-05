<template>
    <Chart type="line" :data="chartData" :options="chartOptions" />
</template>

<script setup>
const props = defineProps({
    timestamp: {
        type: String,
    },
    cpuTimes: {
        type: Object,
    },
    memoryInfo: {
        type: Object,
    },
});

const { t } = useI18n();
const appStore = useAppStore();

const chartData = ref();
const chartOptions = ref();

const MAX_DATA_POINTS = 8;

/**
 * Calculates the CPU usage between two time points.
 * @param {object} previousCpuTimes The cpuTimes object at the previous time point.
 * @param {object} currentCpuTimes The cpuTimes object at the current time point.
 * @returns {number} The percentage of CPU usage (0-100).
 */
const calculateCpuUsage = (previousCpuTimes, currentCpuTimes) => {
    // 1. Calculate the change in each metric between two time points
    const idleTimeDelta = currentCpuTimes.idleTime - previousCpuTimes.idleTime;
    const kernelTimeDelta = currentCpuTimes.kernelTime - previousCpuTimes.kernelTime;
    const userTimeDelta = currentCpuTimes.userTime - previousCpuTimes.userTime;

    // 2. Calculate the change in total CPU time
    const totalCpuTimeDelta = idleTimeDelta + kernelTimeDelta + userTimeDelta;

    // 3. Avoid divide-by-zero errors
    if (totalCpuTimeDelta <= 0) {
        return 0;
    }

    // 4. Calculate CPU usage
    // CPU usage = 1 - (idle time / total time)
    const cpuUsage = 1 - idleTimeDelta / totalCpuTimeDelta;

    // 5. Return the percentage with two decimal places
    return Math.ceil(cpuUsage * 100);
};

const getChartData = (newDate, newData) => {
    var chartDataVal = chartData.value;
    const result = {
        datasets: [
            {
                label: t('views.dashboard.monitor.cpu'),
                fill: false,
                tension: 0.4,
                borderColor: '#028FF1',
            },
            {
                label: t('views.dashboard.monitor.ram'),
                fill: false,
                tension: 0.4,
                borderColor: '#F97316',
            },
        ],
    };

    if (!chartDataVal) {
        const now = dayjs();
        result.labels = Array.from({ length: MAX_DATA_POINTS }, (_, index) => now.subtract((MAX_DATA_POINTS - index - 1) * 3, 'seconds').format('HH:mm:ss'));
        for (let i = 0, len = result.datasets.length; i < len; i++) {
            result.datasets[i].data = Array.from({ length: MAX_DATA_POINTS }, () => 0);
        }
    } else {
        if (!newDate || !newData) {
            result.labels = chartDataVal.labels;
            for (let i = 0, len = result.datasets.length; i < len; i++) {
                result.datasets[i].data = chartDataVal.datasets[i].data;
            }
        } else {
            result.labels = chartDataVal.labels.slice(1).concat(newDate.format('HH:mm:ss'));
            for (let i = 0, len = chartDataVal.datasets.length; i < len; i++) {
                result.datasets[i].data = chartDataVal.datasets[i].data.slice(1).concat(newData[i]);
            }
        }
    }

    return result;
};

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
                        return context.parsed.y + ' %';
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
                        return value + ' %';
                    },
                },
                grid: {
                    color: surfaceBorder,
                },
                title: {
                    display: true,
                    text: t('views.dashboard.monitor.usage'),
                },
                min: 0,
                max: 100,
            },
        },
    };
};

onMounted(() => {
    chartData.value = getChartData();
    chartOptions.value = getChartOptions();
});

watch(
    () => [props.timestamp, props.cpuTimes, props.memoryInfo],
    (newVal, oldVal) => {
        if (!oldVal[0]) {
            return;
        }

        const newDate = dayjs(newVal[0]);
        const newCpuTimes = newVal[1];
        const oldCpuTimes = oldVal[1];

        const cpuUsage = calculateCpuUsage(oldCpuTimes, newCpuTimes);

        chartData.value = getChartData(newDate, [cpuUsage, newVal[2].usedPercentage]);
        chartOptions.value = getChartOptions();
    }
);

const localeStore = useLocaleStore();
watch([() => appStore.primaryColor, () => appStore.isRTL, () => appStore.isDark, () => localeStore.lang], async () => {
    await nextTick();
    chartData.value = getChartData();
    chartOptions.value = getChartOptions();
});
</script>
