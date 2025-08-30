<template>
    <div class="grid grid-cols-12 gap-4">
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.serverName')">
                <p class="content">{{ model.serverName }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.serverIp')">
                <p class="content">{{ model.serverIp || t('common.unknown') }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.serverPort')">
                <p class="content">{{ model.serverPort }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.region')">
                <p class="content">{{ model.region }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.language')">
                <p class="content">{{ model.language }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.serverVersion')">
                <p class="content">{{ model.serverVersion }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.uptime')">
                <p class="content">{{ formatUptime(model.uptime) }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.gameTime')">
                <p class="content">{{ formatGameTime(model.gameTime) }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.gameName')">
                <p class="content">{{ model.gameName }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.gameMode')">
                <p class="content">{{ model.gameMode }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.gameWorld')">
                <p class="content">{{ model.gameWorld }}</p>
            </Fieldset>
        </div>
        <div class="col-span-4 2xl:col-span-3">
            <Fieldset :legend="$t('views.dashboard.overview.gameDifficulty')">
                <p class="content">{{ formatGameDifficulty(model.gameDifficulty) }}</p>
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
    --uno: 'ltr:ml-3 rtl:mr-3 text-p-primary-color';
}
</style>
