export const useAppStore = defineStore('app', () => {
    const keyPrefix = 'app.';
    const isDark = useDark();
    const primaryColor = useStorage(keyPrefix + 'primaryColor', 'emerald');
    const surfaceColor = useStorage(keyPrefix + 'surfaceColor', isDark.value ? 'zinc' : 'slate');
    const theme = useStorage(keyPrefix + 'theme', 'aura');
    const isRippleActive = useStorage(keyPrefix + 'ripple', true);
    const isRTL = useStorage(keyPrefix + 'rtl', false);

    const textDirection = useTextDirection();
    watch(
        isRTL,
        (val) => {
            const direction = val ? 'rtl' : 'ltr';
            if (!document.startViewTransition) {
                textDirection.value = direction;
            } else {
                document.startViewTransition(() => {
                    textDirection.value = direction;
                });
            }
        },
        { immediate: true }
    );

    return { theme, isDark, primaryColor, surfaceColor, isRippleActive, isRTL };
});
