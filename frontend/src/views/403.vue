<template>
    <div class="error-page">
        <div class="error-code">4<span>0</span>3</div>
        <div class="error-desc">{{ message }}</div>
        <div class="error-handle">
            <router-link to="/">
                <el-button type="primary" size="large">Go back to Home</el-button>
            </router-link>
            <router-link to="/login" class="error-btn">
                <el-button type="primary" size="large">Go back to login page</el-button>
            </router-link>
        </div>
    </div>
</template>

<script>
export default {
    name: '403',
    setup() {
        const route = useRoute();
        const message = computed(() => {
            return route.query.message ?? 'Sorry, you do not have permission to access this page.';
        });
        return {
            message,
        };
    },
};
</script>

<style scoped>
.error-page {
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
    width: 100%;
    height: 100vh;
    background: #f3f3f3;
    box-sizing: border-box;
}
.error-code {
    line-height: 1;
    font-size: 250px;
    font-weight: bolder;
    color: #f02d2d;
}
.error-code span {
    color: #00a854;
}
.error-desc {
    font-size: 30px;
    color: #777;
}
.error-handle {
    margin-top: 30px;
    padding-bottom: 200px;
}
.error-btn {
    margin-left: 100px;
}
</style>
