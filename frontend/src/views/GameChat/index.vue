<template>
    <div class="h-full flex flex-col">
        <Card class="flex-1">
            <template #content>
                <div ref="contentRef" class="h-[calc(100vh-210px)] overflow-y-auto">
                    <p v-for="item in gameEventStore.chatMessages" :key="item.id" :style="{ color: '#00C814' }" class="font-mono whitespace-pre-wrap">
                        {{ `${item.senderName}: ${item.message}` }}
                    </p>
                </div>
            </template>
        </Card>
        <div class="mt-2 flex" v-focustrap>
            <InputText
                class="flex-1"
                ref="autoCompleteRef"
                :placeholder="$t('views.gameChat.typeMessage')"
                :modelValue="currentCommand"
                @update:modelValue="onInputChange"
                @keyup.enter="onEnter"
                @keydown.up.prevent="handleArrowUp"
                @keydown.down.prevent="handleArrowDown"
            />
            <Button class="ms-2" type="button" :loading="isLoading" severity="secondary" :label="$t('views.gameChat.send')" @click="onEnter">
                <template #icon>
                    <icon-mdi:send />
                </template>
            </Button>
        </div>
    </div>
</template>

<script>
export default {
    name: 'GameChat',
};
</script>

<script setup>
import * as gameServerApi from '~/api/gameServer';
import { useCommandHistory } from '~/composables/useCommandHistory';
import { useGameEventStore } from '~/store/gameEvent';

const gameEventStore = useGameEventStore();
const autoCompleteRef = ref();
const contentRef = ref();
const isLoading = ref(false);
const { currentCommand, navigateUp, navigateDown, addCommandToHistory, onInputChange } = useCommandHistory();

const handleArrowUp = () => {
    navigateUp();
};
const handleArrowDown = () => {
    navigateDown();
};

const onEnter = async () => {
    isLoading.value = true;
    try {
        await gameServerApi.sendGlobalMessage(currentCommand.value);
        addCommandToHistory(currentCommand.value);
        onInputChange('');
    } finally {
        isLoading.value = false;
    }
};

const { pause, resume } = watch(
    () => gameEventStore.chatMessages.length,
    async () => {
        const element = contentRef.value;
        if (element) {
            const isScrolledToBottom = element.scrollTop + element.clientHeight >= element.scrollHeight - 1;
            if (isScrolledToBottom) {
                await nextTick(() => {
                    element.scrollTop = element.scrollHeight;
                });
            }
        }
    }
);

onActivated(() => {
    contentRef.value.scrollTop = contentRef.value.scrollHeight;
    resume();
});

onDeactivated(() => {
    pause();
});
</script>
