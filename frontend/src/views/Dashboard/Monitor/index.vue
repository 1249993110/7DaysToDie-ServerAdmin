<template>
    <div class="grid grid-cols-2 gap-4">
        <CPU :timestamp="timestamp" :cpuTimes="cpuTimes" class="w-full h-60" />
        <Network :timestamp="timestamp" :networkInfos="networkInfos" class="w-full h-60" />
    </div>
</template>

<script setup>
import CPU from './CPU.vue';
import Network from './Network.vue';

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
</script>
