<template>
    <Dialog
        v-model:visible="visible"
        modal
        :header="isEdit ? $t('views.banWhitelist.editBan') : $t('views.banWhitelist.addBan')"
        :style="{ width: '50rem' }"
        :closable="false"
    >
        <Form id="banForm" :resolver @submit="onFormSubmit" class="flex flex-col gap-4" :initial-values="initialValues">
            <FormField v-slot="$field" name="playerId" class="flex items-start gap-4">
                <label for="playerId" class="w-32 flex-shrink-0 mt-2">{{ $t('views.banWhitelist.playerId') }}</label>
                <div class="flex-1">
                    <InputText
                        id="playerId"
                        v-model="$field.value"
                        fluid
                        :disabled="isEdit"
                    />
                    <Message v-if="$field?.invalid" severity="error" size="small" variant="simple" class="mt-1">{{ $field.error?.message }}</Message>
                </div>
            </FormField>
            <FormField v-slot="$field" name="displayName" class="flex items-start gap-4">
                <label for="displayName" class="w-32 flex-shrink-0 mt-2">{{ $t('views.banWhitelist.displayName') }}</label>
                <div class="flex-1">
                    <InputText
                        id="displayName"
                        v-model="$field.value"
                        fluid
                    />
                    <Message v-if="$field?.invalid" severity="error" size="small" variant="simple" class="mt-1">{{ $field.error?.message }}</Message>
                </div>
            </FormField>
            <FormField v-slot="$field" name="bannedUntil" class="flex items-start gap-4">
                <label for="bannedUntil" class="w-32 flex-shrink-0 mt-2">{{ $t('views.banWhitelist.bannedUntil') }}</label>
                <div class="flex-1">
                    <Calendar
                        id="bannedUntil"
                        v-model="$field.value"
                        showTime
                        hourFormat="24"
                        dateFormat="yy-mm-dd"
                        :showIcon="true"
                        fluid
                    />
                    <Message v-if="$field?.invalid" severity="error" size="small" variant="simple" class="mt-1">{{ $field.error?.message }}</Message>
                </div>
            </FormField>
            <FormField v-slot="$field" name="reason" class="flex items-start gap-4">
                <label for="reason" class="w-32 flex-shrink-0 mt-2">{{ $t('views.banWhitelist.reason') }}</label>
                <div class="flex-1">
                    <Textarea
                        id="reason"
                        v-model="$field.value"
                        rows="3"
                        fluid
                    />
                    <Message v-if="$field?.invalid" severity="error" size="small" variant="simple" class="mt-1">{{ $field.error?.message }}</Message>
                </div>
            </FormField>
        </Form>
        <template #footer>
            <Button
                :label="$t('common.cancel')"
                class="p-button-text"
                @click="visible = false"
            >
                <template #icon>
                    <icon-mdi:close />
                </template>
            </Button>
            <Button
                :label="isEdit ? $t('common.update') : $t('common.save')"
                class="p-button-text"
                type="submit"
                form="banForm"
                :loading="loading"
            >
                <template #icon>
                    <icon-mdi:check />
                </template>
            </Button>
        </template>
    </Dialog>
</template>

<script setup>
import { banPlayer, unbanPlayers } from '~/api/gameServer';
import { valibotResolver } from '@primevue/forms/resolvers/valibot';
import v from '~/plugins/valibot';

const { t } = useI18n();
const emit = defineEmits(['saved']);

const props = defineProps({
    editData: {
        type: Object,
        default: null,
    },
});

const visible = ref(false);
const loading = ref(false);

const isEdit = computed(() => !!props.editData);

const initialValues = computed(() => {
    if (props.editData) {
        return {
            playerId: props.editData.playerId || '',
            displayName: props.editData.displayName || '',
            bannedUntil: props.editData.bannedUntil ? dayjs(props.editData.bannedUntil).toDate() : null,
            reason: props.editData.reason || '',
        };
    }
    return {
        playerId: '',
        displayName: '',
        bannedUntil: null,
        reason: '',
    };
});

const resolver = valibotResolver(
    v.object({
        playerId: v.pipe(v.string(), v.minLength(1)),
        displayName: v.pipe(v.string(), v.minLength(1)),
        bannedUntil: v.date(),
        reason: v.optional(v.string()),
    })
);

const onFormSubmit = async ({ valid, values }) => {
    if (valid) {
        loading.value = true;
        try {
            if (isEdit.value) {
                // For edit, first unban the old one, then ban with new data
                await unbanPlayers([props.editData.playerId]);
                await banPlayer(
                    values.playerId,
                    dayjs(values.bannedUntil).toISOString(),
                    values.displayName,
                    values.reason || null
                );
            } else {
                await banPlayer(
                    values.playerId,
                    dayjs(values.bannedUntil).toISOString(),
                    values.displayName,
                    values.reason || null
                );
            }
            emit('saved');
            visible.value = false;
        } catch (error) {
            console.error(error);
        } finally {
            loading.value = false;
        }
    }
};

const show = () => {
    visible.value = true;
};

defineExpose({
    show,
});
</script>
