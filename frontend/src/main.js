import { createApp } from 'vue';
import App from './App.vue';
import usePinia from './plugins/pinia';
import usePrimeVue from './plugins/primevue';
import i18n from './plugins/i18n';
import router from './router';

import 'virtual:uno.css';
import './assets/styles/common.scss';

const app = createApp(App);

app.use(i18n);
usePinia(app);
usePrimeVue(app);
app.use(router);

// Load locale and then mount the app
useLocaleStore()
    .loadLocale()
    .then(() => {
        app.mount('#app');
    });

// // Support async router
// router.isReady().then(() => {
//     app.mount('#app');
// });
