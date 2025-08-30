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

const chartData = ref();
const chartOptions = ref();

const MAX_DATA_POINTS = 8;
const UNITS = ['b', 'Kb', 'Mb', 'Gb', 'Tb', 'Pb', 'Eb', 'Zb', 'Yb']; // Define the base and units

const formatBytesToSpeed = (bits, precision = 1) => {
    if (bits === 0) return '0 b';
    const k = 1024;

    const i = Math.floor(Math.log(bits) / Math.log(k));
    const finalUnitIndex = i < 0 ? 0 : i;
    const value = bits / Math.pow(k, finalUnitIndex);

    return `${parseFloat(value.toFixed(precision))} ${UNITS[finalUnitIndex]}`;
};

const getDynamicYAxisOptions = (maxBits) => {
    if (maxBits === 0) {
        return { unit: 'b', stepSize: 1 };
    }

    const k = 1024;
    const presets = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 200, 300, 400, 500, 600, 700, 800, 900];
    const maxPreset = presets[presets.length - 1];

    let unitIndex = 0;
    let value = maxBits;
    // When the value is greater than the maximum preset value, the unit is upgraded.
    while (value > maxPreset && unitIndex < UNITS.length - 1) {
        value /= k;
        unitIndex++;
    }
    // Find the first preset value that is greater than or equal to the current value
    let ceiledValue = presets.find((p) => p >= value);

    // If not found (i.e. value is still greater than the maximum preset value, but is already the last unit), round up to the nearest multiple of 100
    if (ceiledValue === undefined) {
        ceiledValue = Math.ceil(value / 100) * 100;
    }

    const max = ceiledValue * Math.pow(k, unitIndex);
    let stepSize;
    if (ceiledValue % 3 === 0) {
        stepSize = max / 3;
    } else if (ceiledValue % 4 === 0) {
        stepSize = max / 4;
    } else {
        stepSize = max / 2;
    }

    return {
        unit: UNITS[unitIndex],
        stepSize: stepSize,
        ceiledValue: ceiledValue,
    };
};

let lastMax = {
    value: 0,
    config: {},
};
const getChartOptions = () => {
    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--p-text-color');
    const textColorSecondary = documentStyle.getPropertyValue('--p-text-muted-color');
    const surfaceBorder = documentStyle.getPropertyValue('--p-content-border-color');

    const maxBytes = Math.max(...chartData.value.datasets.map((dataset) => Math.max(...dataset.data)));
    let yAxisConfig;
    if (maxBytes !== 0 && maxBytes === lastMax.value) {
        yAxisConfig = lastMax.config;
    } else {
        yAxisConfig = getDynamicYAxisOptions(maxBytes);
        lastMax = {
            value: maxBytes,
            config: yAxisConfig,
        };
    }
    console.log(yAxisConfig);

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
                        return formatBytesToSpeed(context.parsed.y) + 'ps';
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
                    stepSize: yAxisConfig.stepSize,
                    callback: (value) => {
                        return formatBytesToSpeed(value);
                    },
                },
                grid: {
                    color: surfaceBorder,
                },
                title: {
                    display: true,
                    text: `${t('views.dashboard.monitor.network')} (${yAxisConfig.unit}ps / s)`,
                },
                min: 0,
            },
        },
    };
};

const getChartData = (newDate, newData) => {
    var chartDataVal = chartData.value;
    const result = {
        datasets: [
            {
                label: t('views.dashboard.monitor.trafficIn'),
                fill: true,
                tension: 0.4,
                borderColor: '#1F77B4',
                backgroundColor: 'rgba(107, 114, 128, 0.2)',
            },
            {
                label: t('views.dashboard.monitor.trafficOut'),
                fill: true,
                borderColor: '#2CA02C',
                tension: 0.4,
                backgroundColor: 'rgba(107, 114, 128, 0.2)',
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

onMounted(() => {
    chartData.value = getChartData();
    chartOptions.value = getChartOptions();
});

watch(
    () => [props.timestamp, props.networkInfos],
    (newVal, oldVal) => {
        if (!oldVal[0]) {
            return;
        }

        const oldDate = dayjs(oldVal[0]);
        const newDate = dayjs(newVal[0]);
        const timeDifferenceInSeconds = newDate.diff(oldDate, 'millisecond') / 1000;

        const newBytesReceived = newVal[1].map((i) => i.bytesReceived).reduce((a, b) => a + b, 0);
        const oldBytesReceived = oldVal[1].map((i) => i.bytesReceived).reduce((a, b) => a + b, 0);
        const newBytesSent = newVal[1].map((i) => i.bytesSent).reduce((a, b) => a + b, 0);
        const oldBytesSent = oldVal[1].map((i) => i.bytesSent).reduce((a, b) => a + b, 0);

        const downloadSpeedInBytes = newBytesReceived - oldBytesReceived;
        const uploadSpeedInBytes = newBytesSent - oldBytesSent;

        chartData.value = getChartData(newDate, [(downloadSpeedInBytes / timeDifferenceInSeconds) * 8, (uploadSpeedInBytes / timeDifferenceInSeconds) * 8]);
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
