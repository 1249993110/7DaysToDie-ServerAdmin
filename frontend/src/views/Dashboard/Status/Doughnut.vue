<template>
    <div class="flex flex-col items-center justify-center">
        <div class="font-semibold">{{ title }}</div>
        <Chart type="doughnut" :data="chartData" :options="chartOptions" :plugins="[centerTextPlugin]" class="w-full" />
    </div>
</template>

<script setup>
import { useAppStore } from '~/store/app';

const props = defineProps({
    title: {
        type: String,
        required: true,
    },
    used: {
        type: Number,
        required: true,
        default: 0,
    },
    free: {
        type: Number,
        required: true,
        default: 0,
    },
    centerText: {
        type: String,
        required: true,
        default: '',
    },
    legendLabels: {
        type: Array,
        required: false,
    },
    unit: {
        type: String,
        required: false,
    },
});

const appStore = useAppStore();

const centerTextPlugin = {
    id: 'centerText',
    beforeDraw: (chart) => {
        // Get chart context
        const ctx = chart.ctx;

        // Get chart center coordinates
        const data = chart.getDatasetMeta(0).data;
        const xCenter = data[0].x;
        const yCenter = data[0].y;

        const documentStyle = getComputedStyle(document.documentElement);

        // Configure text style
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.font = '24px Arial'; // Adjust font and size as needed
        ctx.fillStyle = documentStyle.getPropertyValue('--primary-color'); // Text color

        // Draw text at chart center
        ctx.fillText(props.centerText, xCenter, yCenter - 10); // Adjust y coordinate for vertical centering
    },
};

const chartData = ref();
const getChartData = () => {
    return {
        labels: props.legendLabels,
        datasets: [
            {
                data: [props.used, props.free],
                backgroundColor: [
                    '#D94F00', // Your color (orange)
                    '#E5E5E5', // Remaining part color (gray)
                ],
                borderWidth: 0, // Remove border for smoother effect
            },
        ],
    };
};

const chartOptions = ref();
const getChartOptions = () => {
    return {
        responsive: true,
        maintainAspectRatio: false,
        // Key configuration
        cutout: '70%', // Adjust ring thickness
        rotation: 270, // Rotate chart 1/4 turn so semicircle opening faces down
        circumference: 180, // Set circumference to 180 degrees to create semicircle

        plugins: {
            tooltip: {
                enabled: true, // Enable tooltip since it shows both data parts
                rtl: appStore.isRTL,
                callbacks: {
                    label: (context) => {
                        return props.unit ? `${context.parsed} ${props.unit}` : context.parsed;
                    },
                },
            },
            legend: {
                display: false, // Hide legend
            },
        },
    };
};

onMounted(() => {
    chartData.value = getChartData();
    chartOptions.value = getChartOptions();
});

watch([() => props.legendLabels, () => props.used, () => props.free], () => {
    chartData.value = getChartData();
});

watch([() => appStore.primaryColor, () => appStore.isRTL], async () => {
    await nextTick();
    chartOptions.value = getChartOptions();
});
</script>
