import { createApp } from 'vue';
import App from './App.vue';
import router from './router';
import usePinia from './plugins/pinia';
import useI18n from './plugins/i18n';

import './plugins/dayjs';
import 'virtual:uno.css'

const app = createApp(App);
app.use(router);
usePinia(app);
useI18n(app);

// Support async router
router.isReady().then(() => {
    app.mount('#app');
});
