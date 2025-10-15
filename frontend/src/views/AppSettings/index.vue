<template>
	<Card>
		<template #content>
			<div v-if="isLoading" class="flex flex-col gap-4">
				<Skeleton v-for="index in 4" :key="index" height="3rem" />
			</div>
			<template v-else>
				<Form
					ref="formRef"
					:key="formKey"
					id="appSettingsForm"
					:initial-values="initialValues"
					:resolver="resolver"
					class="flex flex-col gap-6"
					@submit="onSubmit"
				>
					<div class="grid gap-4 md:grid-cols-2">
						<FormField v-slot="$field" name="webUrl" class="flex flex-col gap-2">
							<label class="text-sm font-semibold" for="webUrl">{{ t('views.appSettings.fields.webUrl') }}</label>
							<InputText id="webUrl" v-model="$field.value" fluid autocomplete="off" />
							<Message v-if="$field?.invalid" severity="error" size="small" variant="simple">{{ $field.error?.message }}</Message>
						</FormField>
						<FormField v-slot="$field" name="userName" class="flex flex-col gap-2">
							<label class="text-sm font-semibold" for="userName">{{ t('views.appSettings.fields.userName') }}</label>
							<InputText id="userName" v-model="$field.value" fluid autocomplete="off" />
							<Message v-if="$field?.invalid" severity="error" size="small" variant="simple">{{ $field.error?.message }}</Message>
						</FormField>
					</div>
					<div class="grid gap-4 md:grid-cols-2">
						<FormField v-slot="$field" name="password" class="flex flex-col gap-2">
							<label class="text-sm font-semibold" for="password">{{ t('views.appSettings.fields.password') }}</label>
							<Password v-model="$field.value" type="text" toggleMask fluid autocomplete="new-password" />
							<Message v-if="$field?.invalid" severity="error" size="small" variant="simple">{{ $field.error?.message }}</Message>
						</FormField>
						<FormField v-slot="$field" name="serverConfigFile" class="flex flex-col gap-2">
							<label class="text-sm font-semibold" for="serverConfigFile">{{ t('views.appSettings.fields.serverConfigFile') }}</label>
							<InputText id="serverConfigFile" v-model="$field.value" fluid autocomplete="off" />
							<Message v-if="$field?.invalid" severity="error" size="small" variant="simple">{{ $field.error?.message }}</Message>
						</FormField>
					</div>
					<div class="grid gap-4 md:grid-cols-2">
						<FormField v-slot="$field" name="accessTokenExpireTime" class="flex flex-col gap-2">
							<label class="text-sm font-semibold" for="accessTokenExpireTime">{{ t('views.appSettings.fields.accessTokenExpireTime') }}</label>
							<InputNumber id="accessTokenExpireTime" v-model="$field.value" fluid :min="0" :useGrouping="false" />
							<Message v-if="$field?.invalid" severity="error" size="small" variant="simple">{{ $field.error?.message }}</Message>
						</FormField>
						<FormField v-slot="$field" name="refreshTokenExpireTime" class="flex flex-col gap-2">
							<label class="text-sm font-semibold" for="refreshTokenExpireTime">{{ t('views.appSettings.fields.refreshTokenExpireTime') }}</label>
							<InputNumber id="refreshTokenExpireTime" v-model="$field.value" fluid :min="0" :useGrouping="false" />
							<Message v-if="$field?.invalid" severity="error" size="small" variant="simple">{{ $field.error?.message }}</Message>
						</FormField>
					</div>
				</Form>
				<div class="flex justify-end gap-2 mt-4">
					<Button
						type="button"
						outlined
						severity="secondary"
						:label="t('views.appSettings.actions.reset')"
						:disabled="isSubmitting"
						@click="onReset"
					>
						<template #icon>
							<icon-mdi:refresh />
						</template>
					</Button>
					<Button
						type="submit"
						form="appSettingsForm"
						:label="t('views.appSettings.actions.save')"
						:loading="isSubmitting"
					>
						<template #icon>
							<icon-mdi:check />
						</template>
					</Button>
				</div>
                <Toast />
			</template>
		</template>
	</Card>
</template>

<script>
export default {
	name: 'AppSettings',
};
</script>

<script setup>
import { valibotResolver } from '@primevue/forms/resolvers/valibot';
import { useToast } from 'primevue/usetoast';
import v from '~/plugins/valibot';
import { getAppSettings, updateAppSettings } from '~/api/gameServer';

const { t } = useI18n();
const toast = useToast();

const formRef = ref();
const formKey = ref(0);
const isLoading = ref(false);
const isSubmitting = ref(false);

const buildDefaults = () => ({
	webUrl: '',
	userName: '',
	password: '',
	accessTokenExpireTime: 0,
	refreshTokenExpireTime: 0,
	serverConfigFile: '',
});

const initialValues = ref(buildDefaults());

const resolver = valibotResolver(
	v.object({
		webUrl: v.pipe(v.string(), v.minLength(1), v.url()),
		userName: v.pipe(v.string(), v.minLength(1)),
		password: v.pipe(v.string(), v.minLength(1)),
		accessTokenExpireTime: v.pipe(v.number(), v.minValue(0)),
		refreshTokenExpireTime: v.pipe(v.number(), v.minValue(0)),
		serverConfigFile: v.pipe(v.string(), v.minLength(1)),
	})
);

const mapSettings = (data) => ({
	webUrl: data?.WebUrl ?? data?.webUrl ?? '',
	userName: data?.UserName ?? data?.userName ?? '',
	password: data?.Password ?? data?.password ?? '',
	accessTokenExpireTime: Number(data?.AccessTokenExpireTime ?? data?.accessTokenExpireTime ?? 0),
	refreshTokenExpireTime: Number(data?.RefreshTokenExpireTime ?? data?.refreshTokenExpireTime ?? 0),
	serverConfigFile: data?.ServerConfigFile ?? data?.serverConfigFile ?? '',
});

const loadSettings = async () => {
	isLoading.value = true;
	try {
		const data = await getAppSettings();
		initialValues.value = mapSettings(data);
		formKey.value += 1;
	} catch (error) {
		console.error(error);
		initialValues.value = buildDefaults();
		formKey.value += 1;
	} finally {
		isLoading.value = false;
	}
};

const onReset = () => {
	if (formRef.value) {
		formRef.value.reset();
	}
};

const toPayload = (values) => ({
	WebUrl: values.webUrl,
	UserName: values.userName,
	Password: values.password,
	AccessTokenExpireTime: Number(values.accessTokenExpireTime ?? 0),
	RefreshTokenExpireTime: Number(values.refreshTokenExpireTime ?? 0),
	ServerConfigFile: values.serverConfigFile,
});

const onSubmit = async ({ valid, values }) => {
	if (!valid) {
		return;
	}
	isSubmitting.value = true;
	try {
		await updateAppSettings(toPayload(values));
		toast.add({
			severity: 'success',
			summary: t('views.appSettings.actions.save'),
			detail: t('views.appSettings.messages.saveSuccess'),
			life: 3000,
		});
		await loadSettings();
	} catch (error) {
		console.error(error);
	} finally {
		isSubmitting.value = false;
	}
};

onMounted(() => {
	loadSettings();
});
</script>
