<template>
    <div class="activity-list h-62 overflow-y-auto" :class="{ 'pe-2!': activities.length > 3 }">
        <div v-for="(activity, index) in activities" :key="index" class="activity-item">
            <component :is="activity.icon" :style="{ color: activity.color }" />
            <div class="activity-content">
                <span class="activity-text">{{ activity.text }}</span>
                <span class="activity-time">{{ activity.time }}</span>
            </div>
        </div>
    </div>
</template>

<script setup>
const { activities } = storeToRefs(useRecentActivityStore());
</script>

<style scoped>
.activity-list {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
    padding: 0.5rem 0;
}

.activity-item {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    padding: 0.75rem;
    border: 1px solid var(--p-surface-200);
    border-radius: 0.5rem;
    background-color: var(--p-surface-50);
}

.dark .activity-item {
    background-color: var(--p-surface-800);
    border-color: var(--p-surface-700);
}

.activity-content {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
}

.activity-text {
    font-size: 0.875rem;
    font-weight: 500;
}

.activity-time {
    font-size: 0.75rem;
    color: var(--p-surface-600);
}

.dark .activity-time {
    color: var(--p-surface-400);
}
</style>
