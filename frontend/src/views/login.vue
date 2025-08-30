<template>
    <div class="size-screen f-center t-bg-1">
        <!-- From Uiverse.io by themrsami -->
        <div class="relative py-3 sm:max-w-xl sm:mx-auto">
            <div class="relative px-4 py-10 t-bg-2 t-border-card mx-8 md:mx-0 shadow rounded-3xl sm:p-10">
                <Form :resolver @submit="onFormSubmit" class="max-w-lg mx-auto" :initial-values="initialValues" v-focustrap>
                    <div class="flex items-center space-x-5 justify-center">
                        <div class="flex text-2xl font-extrabold tracking-wide">
                            <span class="bg-gradient-to-r from-purple-500 to-blue-500 bg-clip-text text-transparent">{{ localeStore.getAppTitle() }}</span>
                        </div>
                    </div>
                    <div class="mt-5">
                        <FormField v-slot="$field" name="username" class="mt-1 mb-5 text-sm">
                            <label class="font-semibold text-gray-600 pb-1 block" for="username">{{ $t('views.login.username') }}</label>
                            <InputText type="text" fluid autofocus ini />
                            <Message v-if="$field?.invalid" severity="error" size="small" variant="simple">{{ $field.error?.message }}</Message>
                        </FormField>
                        <FormField v-slot="$field" name="password" class="mt-1 mb-5 text-sm">
                            <label class="font-semibold text-gray-600 pb-1 block" for="password">{{ $t('views.login.password') }}</label>
                            <Password type="text" :feedback="false" toggleMask fluid />
                            <Message v-if="$field?.invalid" severity="error" size="small" variant="simple">{{ $field.error?.message }}</Message>
                        </FormField>
                    </div>
                    <div class="mb-4 flex items-center">
                        <span>
                            <Checkbox v-model="userInfoStore.isRememberMe" binary inputId="rememberMe" />
                            <label class="ml-1 text-xs font-semibold text-gray-500 hover:text-gray-600 cursor-pointer" for="rememberMe">
                                {{ $t('views.login.rememberMe') }}
                            </label>
                        </span>
                        <a class="ml-auto text-xs font-semibold text-gray-500 hover:text-gray-600 cursor-pointer underline" href="#">{{ $t('views.login.forgotPassword') }}</a>
                    </div>
                    <div class="mt-5">
                        <Button
                            class="cursor-pointer py-2 px-4 bg-blue-600 hover:bg-blue-700 focus:ring-blue-500 focus:ring-offset-blue-200 text-white w-full transition ease-in duration-200 text-center text-base font-semibold shadow-md focus:outline-none focus:ring-2 focus:ring-offset-2 rounded-lg"
                            type="submit"
                            :label="$t('views.login.submit')"
                            unstyled
                        />
                    </div>
                    <div class="flex items-center justify-between mt-4">
                        <span class="w-1/5 border-b border-gray-200 dark:border-gray-400 md:w-1/4"></span>
                        <span class="text-xs text-gray-500 uppercase dark:text-gray-400">Or Sign in with</span>
                        <span class="w-1/5 border-b border-gray-200 dark:border-gray-400 md:w-1/4"></span>
                    </div>
                    <div>
                        <Button
                            class="flex items-center justify-center cursor-pointer px-20 bg-gradient-to-r from-[#72A233] to-[#599342] text-white hover:bg-green-200 focus:ring-blue-500 focus:ring-offset-blue-200 text-gray-700 w-full transition ease-in duration-200 text-center text-base font-semibold shadow-md focus:outline-none focus:ring-2 focus:ring-offset-2 rounded-lg mt-4"
                            unstyled
                        >
                            <img class="w-10" src="../assets/images/steam-svgrepo-com.svg" />
                            <span class="ml-2">Sign in with Steam</span>
                        </Button>
                    </div>
                </Form>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'login',
};
</script>

<script setup>
import { disposeAllStores } from '~/plugins/pinia';
import { valibotResolver } from '@primevue/forms/resolvers/valibot';
import v from '~/plugins/valibot';

disposeAllStores();

const localeStore = useLocaleStore();
const userInfoStore = useUserInfoStore();

const initialValues = {
    username: '',
    password: '',
};

const resolver = valibotResolver(
    v.object({
        username: v.pipe(v.string(), v.minLength(1)),
        password: v.pipe(v.string(), v.minLength(6)),
    })
);

const onFormSubmit = async ({ valid, values }) => {
    if (valid) {
        await userInfoStore.signIn(values.username, values.password);
    }
};
</script>
