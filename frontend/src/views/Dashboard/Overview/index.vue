<template>
    <div class="grid grid-cols-12 gap-4">
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.serverName')">
                <p class="content">{{ model.serverName || $t('common.unknown') }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.serverIp')">
                <p class="content">{{ model.serverIp || $t('common.unknown') }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.serverPort')">
                <p class="content">{{ model.serverPort || $t('common.unknown') }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.region')">
                <p class="content">{{ model.region || $t('common.unknown') }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.language')">
                <p class="content">{{ model.language || $t('common.unknown') }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.serverVersion')">
                <p class="content">{{ model.serverVersion || $t('common.unknown') }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.uptime')">
                <p class="content">{{ formatUptime(model.uptime) || $t('common.unknown') }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.gameTime')">
                <p class="content">{{ formatGameTime(model.gameTime) || $t('common.unknown') }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.gameName')">
                <p class="content">{{ model.gameName || $t('common.unknown') }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.gameMode')">
                <p class="content">{{ model.gameMode || $t('common.unknown') }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.gameWorld')">
                <p class="content">{{ model.gameWorld || $t('common.unknown') }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.gameDifficulty')">
                <p class="content">{{ formatGameDifficulty(model.gameDifficulty) || $t('common.unknown') }}</p>
            </Fieldset>
        </div>
    </div>
</template>

<script setup>
const props = defineProps({
    model: {
        type: Object,
        required: true,
    },
});

const { t } = useI18n();

const formatUptime = (time) => {
    if (!time) {
        return '';
    }
    const dur = dayjs.duration(time, 'seconds');

    const days = Math.floor(dur.asDays());
    const hours = dur.hours();
    const minutes = dur.minutes();
    const seconds = dur.seconds();

    return t('views.dashboard.overview.uptimeFormat', days + 1, { named: { days, hours, minutes, seconds } });
};

const formatGameTime = (time) => {
    if (!time) {
        return '';
    }
    const days = time.days;
    const hours = time.hours;
    const minutes = time.minutes;
    return t('views.dashboard.overview.gameTimeFormat', { days, hours, minutes });
};

const formatGameDifficulty = (gameDifficulty) => {
    if (!gameDifficulty) {
        return '';
    }
    return t('views.dashboard.overview.gameDifficultys.' + gameDifficulty);
};
</script>

<style lang="scss" scoped>
.content {
    --uno: 'ms-3 text-p-primary-color 3xl:whitespace-nowrap';
}
</style>
