import { EventSourcePlus } from 'event-source-plus';
import emitter, { EVENT_TYPES } from '~/plugins/mitt';
import { useUserInfoStore } from '~/store/userInfo';

export const useGameEventStore = defineStore('gameEvent', () => {
    const logs = ref([]);
    const chatMessages = ref([]);
    const MAX = 2000;

    const addLog = async (message, logType) => {
        logs.value.push({ id: crypto.randomUUID(), message, logType });

        if (logs.value.length > MAX) {
            await nextTick(() => logs.value.shift());
        }
    };

    const addChatMessage = async (message, timestamp, senderName) => {
        chatMessages.value.push({ id: crypto.randomUUID(), message, timestamp, senderName });

        if (chatMessages.value.length > MAX) {
            await nextTick(() => chatMessages.value.shift());
        }
    };

    const userInfoStore = useUserInfoStore();
    const eventSource = new EventSourcePlus('/api/sse', {
        headers: async () => {
            const token = await userInfoStore.getAccessToken();
            return {
                Authorization: `Bearer ${token}`,
            };
        },
    });

    const onWelcomeMessage = (data) => {
        console.log(data.message);
        addLog(data.message, 'Assert');
    };

    const controller = eventSource.listen({
        onMessage(message) {
            const data = JSON.parse(message.data);
            emitter.emit('Game.' + message.event, data);
        },
    });

    emitter.on(EVENT_TYPES.GAME.WELCOME, onWelcomeMessage);
    emitter.on(EVENT_TYPES.GAME.LOG_CALLBACK, (data) => addLog(data.message, data.logType));
    emitter.on(EVENT_TYPES.GAME.CHAT_MESSAGE, (data) => addChatMessage(data.message, data.timestamp, data.senderName));

    const isLoggedIn = asyncComputed(() => userInfoStore.isLoggedIn());
    watch(isLoggedIn, (val) => {
        if (val) {
            controller.reconnect();
        } else {
            controller.abort();
        }
    });

    return {
        logs,
        chatMessages,
        addLog,
        addChatMessage,
    };
});
