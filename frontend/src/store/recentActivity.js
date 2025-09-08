import emitter, { EVENT_TYPES } from '~/plugins/mitt';

export const useRecentActivityStore = defineStore('recentActivity', () => {
    const max = 8;
    const { t } = useI18n();
    const state = ref([
        {
            icon: markIcon(() => import('~icons/ic/baseline-login')),
            text: () => t('views.dashboard.recentActivity.login'),
            time: dayjs(),
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

    emitter.on(EVENT_TYPES.GAME.PLAYER_SPAWNED_IN_WORLD, (data) => {
        addActivity({
            icon: markIcon(() => import('~icons/ic/baseline-gamepad')),
            text: () => t('views.dashboard.recentActivity.playerEnterGame', [data.playerInfo.entityName]),
            time: dayjs(),
            color: '#22c55e',
        });
    });

    emitter.on(EVENT_TYPES.GAME.PLAYER_DISCONNECTED, (data) => {
        addActivity({
            icon: markIcon(() => import('~icons/ic/baseline-gamepad')),
            text: () => t('views.dashboard.recentActivity.playerLeaveGame', [data.playerInfo.entityName]),
            time: dayjs(),
            color: '#c52238ff',
        });
    });
    
    return {
        activities,
        addActivity,
    };
});
