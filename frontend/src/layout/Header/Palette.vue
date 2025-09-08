<template>
    <div class="config-panel hidden">
        <div class="config-section">
            <div>
                <span class="config-label">{{ $t('layout.header.palette.primary') }}</span>
                <div class="config-colors">
                    <button
                        v-for="pc of primaryColorPalettes"
                        :key="pc.name"
                        type="button"
                        :title="pc.name"
                        :class="['color-button', { selected: selectedPrimaryColor === pc.name }, { 'bg-p-surface-700 dark:bg-p-surface-0': pc.name === 'noir' }]"
                        :style="{ backgroundColor: pc.palette['500'] }"
                        @click="selectedPrimaryColor = pc.name"
                    />
                </div>
            </div>
            <div>
                <span class="config-label">{{ $t('layout.header.palette.surface') }}</span>
                <div class="config-colors">
                    <button
                        v-for="s of surfaces"
                        :key="s.name"
                        type="button"
                        :title="s.name"
                        :class="[
                            'color-button',
                            {
                                selected: selectedSurfaceColor === s.name,
                            },
                        ]"
                        :style="{ backgroundColor: s.palette['500'] }"
                        @click="selectedSurfaceColor = s.name"
                    />
                </div>
            </div>
            <div class="config-settings">
                <span class="config-label">{{ $t('layout.header.palette.theme') }}</span>
                <SelectButton
                    :pt="{ pcToggleButton: { root: { class: 'capitalize' } } }"
                    v-model="selectedPresetTheme"
                    :options="presetOptions"
                    optionLabel="label"
                    optionValue="value"
                    :allowEmpty="false"
                    size="small"
                />
            </div>
            <div class="flex">
                <div class="flex-1">
                    <div class="config-settings">
                        <span class="config-label">{{ $t('layout.header.palette.ripple') }}</span>
                        <ToggleSwitch v-model="isRippleActive" />
                    </div>
                </div>
                <div class="flex-1">
                    <div class="config-settings items-end">
                        <span class="config-label">{{ $t('layout.header.palette.rtl') }}</span>
                        <ToggleSwitch v-model="isRTL" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { themePresets } from '~/plugins/primevue';
import { usePrimeVue } from 'primevue/config';
import { useTheme } from '~/composables/useTheme';
import { useAppStore } from '~/store/app';

const primeVue = usePrimeVue();

const { selectedPresetTheme, selectedPrimaryColor, selectedSurfaceColor, primaryColorPalettes, surfaces } = useTheme();
const { isRippleActive, isRTL } = storeToRefs(useAppStore());

watch(isRippleActive, (val) => {
    primeVue.config.ripple = val;
});

const presetOptions = Object.keys(themePresets).map((key) => ({
    label: key,
    value: key,
}));
</script>

<style lang="scss" scoped>
.config-panel {
    position: absolute;
    top: 2rem;
    inset-inline-end: 0;
    width: 18rem;
    padding: 1rem;
    background-color: var(--p-surface-0);
    border-radius: 0.375rem;
    box-shadow: 0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1);
    border: 1px solid var(--p-surface-200);
    transform-origin: top;

    --uno: 'dark:bg-p-surface-900';
    --uno: 'dark:border-p-surface-700';
}

.config-section {
    display: flex;
    flex-direction: column;
    gap: 1rem;
}

.config-label {
    font-size: 0.875rem;
    color: var(--p-surface-600);
    font-weight: 600;

    --uno: 'dark:text-p-surface-400';
}

.config-colors {
    padding-top: 0.5rem;
    display: flex;
    gap: 0.5rem;
    flex-wrap: wrap;
    justify-content: space-between;
}

.config-settings {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.color-button {
    border: none;
    width: 1.25rem;
    height: 1.25rem;
    border-radius: 9999px;
    padding: 0;
    cursor: pointer;
}

.selected {
    --ring-offset-shadow: 0 0 0 var(--ring-offset-width) var(--ring-offset-color);
    --ring-shadow: 0 0 0 calc(var(--ring-width) + var(--ring-offset-width)) var(--ring-color);
    --ring-width: 2px;
    --ring-offset-width: 2px;
    --ring-color: var(--p-primary-500);
    --ring-offset-color: #ffffff;
    box-shadow: var(--ring-offset-shadow), var(--ring-shadow);
}
</style>
