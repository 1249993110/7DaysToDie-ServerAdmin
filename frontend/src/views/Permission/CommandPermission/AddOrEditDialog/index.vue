<template>
    <Dialog
        v-model:visible="visible"
        modal
        :header="isEdit ? $t('views.permission.editCommandPermission') : $t('views.permission.addCommandPermission')"
        :style="{ width: '50rem' }"
        :closable="false"
    >
        <Form id="commandPermissionForm" :resolver @submit="onFormSubmit" class="flex flex-col gap-4" :initial-values="initialValues">
            <FormField v-slot="$field" name="command" class="flex items-start gap-4">
                <label for="command" class="w-32 flex-shrink-0 mt-2">{{ $t('views.permission.command') }}</label>
                <div class="flex-1">
                    <InputText
                        id="command"
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
            <FormField v-slot="$field" name="description" class="flex items-start gap-4">
                <label for="description" class="w-32 flex-shrink-0 mt-2">{{ $t('views.permission.description') }}</label>
                <div class="flex-1">
                    <Textarea
                        id="description"
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
                form="commandPermissionForm"
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
            command: props.editData.command || '',
            permissionLevel: props.editData.permissionLevel || 2000,
            description: props.editData.description || '',
        };
    }
    return {
        command: '',
        permissionLevel: 2000,
        description: '',
    };
});

const resolver = valibotResolver(
    v.object({
    command: v.pipe(v.string(), v.minLength(1)),
    permissionLevel: v.pipe(v.number(), v.minValue(0), v.maxValue(2000)),
    description: v.optional(v.string()),
    })
);

import * as api from '~/api/gameServer';

const onFormSubmit = async ({ valid, values }) => {
    if (valid) {
        loading.value = true;
        try {
            if (isEdit.value) {
                await api.addCommandPermission(values);
            } else {
                await api.addCommandPermission(values);
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