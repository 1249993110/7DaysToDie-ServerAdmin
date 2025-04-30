<template>
    <div class="flex items-center justify-center h-screen bg-gray-500">
        <el-form class="form" :model="formModel" ref="formRef" :rules="rules" @submit.native.prevent>
            <div class="title">
                {{ localeStore.getAppTitle() }}<br /><span>{{ $t('views.login.welcome') }}</span>
            </div>
            <el-form-item prop="username">
                <input :placeholder="$t('views.login.username')" class="input" v-model="formModel.username" />
            </el-form-item>
            <el-form-item prop="password">
                <input type="password" :placeholder="$t('views.login.password')" class="input" v-model="formModel.password" />
            </el-form-item>
            <button class="button-confirm" @click="onSubmit">{{ $t('views.login.login') }}</button>
        </el-form>
    </div>
</template>

<script>
export default {
    name: 'login',
};
</script>

<script setup>
import { disposeAllStores } from '~/plugins/pinia';

const { t } = useI18n();
const route = useRoute();
const router = useRouter();

disposeAllStores();
const localeStore = useLocaleStore();
const userInfoStore = useUserInfoStore();

const formModel = reactive({});
const formRef = ref();
const rules = {
    username: [{ required: true, message: t('common.formRule.required'), trigger: 'blur' }],
    password: [{ required: true, message: t('common.formRule.required'), trigger: 'blur' }],
};

const onSubmit = async () => {
    try {
        await formRef.value.validate();
        try {
            await userInfoStore.login(formModel.username, formModel.password);
            ElMessage.success(t('views.login.success'));
            router.push(route.query.redirect ?? '/');
        } catch (error) {
            ElMessage.error(t('views.login.failed'));
        }
    } catch {
    }
};
</script>

<style scoped lang="scss">
.form {
    --input-focus: #2d8cf0;
    --font-color: #323232;
    --font-color-sub: #666;
    --bg-color: #fff;
    --main-color: #323232;
    padding: 20px;
    background: lightgrey;
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    justify-content: center;
    gap: 20px;
    border-radius: 5px;
    border: 2px solid var(--main-color);
    box-shadow: 4px 4px var(--main-color);

    .el-form-item {
        width: 100%;
        :deep(.el-form-item__error) {
            padding-left: 8px;
            padding-top: 8px;
        }
    }
}

.title {
    color: var(--font-color);
    font-weight: 900;
    font-size: 20px;
    margin-bottom: 25px;
}

.title span {
    color: var(--font-color-sub);
    font-weight: 600;
    font-size: 17px;
}

.input {
    width: 100%;
    height: 40px;
    border-radius: 5px;
    border: 2px solid var(--main-color);
    background-color: var(--bg-color);
    box-shadow: 4px 4px var(--main-color);
    font-size: 15px;
    font-weight: 600;
    color: var(--font-color);
    padding: 5px 10px;
    outline: none;
}

.input::placeholder {
    color: var(--font-color-sub);
    opacity: 0.8;
}

.input:focus {
    border: 2px solid var(--input-focus);
}

.button-log {
    cursor: pointer;
    width: 40px;
    height: 40px;
    border-radius: 100%;
    border: 2px solid var(--main-color);
    background-color: var(--bg-color);
    box-shadow: 4px 4px var(--main-color);
    color: var(--font-color);
    font-size: 25px;
    display: flex;
    justify-content: center;
    align-items: center;
}

.icon {
    width: 24px;
    height: 24px;
    fill: var(--main-color);
}

.button-log:active,
.button-confirm:active {
    box-shadow: 0px 0px var(--main-color);
    transform: translate(3px, 3px);
}

.button-confirm {
    margin: 10px auto 0 auto;
    width: 120px;
    height: 40px;
    border-radius: 5px;
    border: 2px solid var(--main-color);
    background-color: var(--bg-color);
    box-shadow: 4px 4px var(--main-color);
    font-size: 17px;
    font-weight: 600;
    color: var(--font-color);
    cursor: pointer;
}
</style>
