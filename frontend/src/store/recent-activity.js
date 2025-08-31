export const useRecentActivityStore = defineStore('recent-activity', () => {
    const max = 4;
    const { t } = useI18n();
    const state = ref([
        {
            icon: markIcon(() => import('~icons/ic/baseline-gamepad')),
            text: () => t('views.dashboard.recentActivity.playerEnterGame', ['IceCoffee']),
            time: dayjs(),
            color: '#22c55e',
        },
        {
            icon: markIcon(() => import('~icons/ic/baseline-login')),
            text: () => t('views.dashboard.recentActivity.login'),
            time: dayjs().add(-2, 'minute'),
            color: '#22c55e',
        },
    ]);

    const activities = computed(() => {
        const result = [];

        for (const activity of state.value) {
            result.push({
                ...activity,
                time: computed(() => dayjs(activity.time).fromNow()),
                text: activity.text(),
            });
        }
        return result;
    });

    const addActivity = (activity) => {
        state.value.unshift(activity);
        if (state.value.length > max) {
            state.value.pop();
        }
    };

    return {
        activities,
        addActivity,
    };
});
