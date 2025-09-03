import { createApp } from 'vue';
import App from './App.vue';
import usePinia from './plugins/pinia';
import usePrimeVue from './plugins/primevue';
import i18n from './plugins/i18n';
import router from './router';

import 'virtual:uno.css';
import './assets/styles/common.scss';

console.log('Application bootstrapping...');

const app = createApp(App);

usePinia(app);

app.use(i18n);
app.use(router);

usePrimeVue(app);

app.mount('#app');
