<template>
    <div class="grid grid-cols-2 gap-4">
        <CPU_RAM :timestamp="timestamp" :cpuTimes="cpuTimes" :memoryInfo="memoryInfo" class="w-full h-60" />
        <Network :timestamp="timestamp" :networkInfos="networkInfos" class="w-full h-60" />
    </div>
</template>

<script setup>
import CPU_RAM from './CPU_RAM.vue';
import Network from './Network.vue';

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
    networkInfos: {
        type: Array,
    },
});
</script>
