<template>
    <Dialog
        v-model:visible="visible"
    modal
    :header="isEdit ? $t('views.permission.editAdminUser') : $t('views.permission.addAdminUser')"
        :style="{ width: '50rem' }"
        :closable="false"
    >
        <Form id="adminUserForm" :resolver @submit="onFormSubmit" class="flex flex-col gap-4" :initial-values="initialValues">
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
            <FormField v-slot="$field" name="permissionLevel" class="flex items-start gap-4">
                <label for="permissionLevel" class="w-32 flex-shrink-0 mt-2">{{ $t('views.permission.permissionLevel') }}</label>
                <div class="flex-1">
                    <InputNumber
                        id="permissionLevel"
                        v-model="$field.value"
                        fluid
                        :min="0"
                        :max="2000"
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
                form="adminUserForm"
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
            permissionLevel: props.editData.permissionLevel || 2000,
            displayName: props.editData.displayName || '',
        };
    }
    return {
        playerId: '',
        permissionLevel: 2000,
        displayName: '',
    };
});

const resolver = valibotResolver(
    v.object({
        playerId: v.pipe(v.string(), v.minLength(1)),
        permissionLevel: v.pipe(v.number(), v.minValue(0), v.maxValue(2000)),
        displayName: v.pipe(v.string(), v.minLength(1)),
    })
);

import * as api from '~/api/gameServer';

const onFormSubmit = async ({ valid, values }) => {
    if (valid) {
        loading.value = true;
        try {
            // 新增和编辑都用 addAdminUser，playerId 不可编辑
            await api.addAdminUser(values);
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