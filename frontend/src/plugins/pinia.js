
const stores = [];

export default (app) => {
    const pinia = createPinia();
    pinia.use(({ store }) => {
        stores.push(store);
    });
    app.use(pinia);
};

export const disposeAllStores = () => {
    for (const store of stores) {
        store.$dispose();
    }
    stores.length = 0;
};

export const resetAllStores = () => {
    for (const store of stores) {
        store.$reset();
    }
};
