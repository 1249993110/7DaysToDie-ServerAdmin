import { EventSourcePlus } from 'event-source-plus';

export const useGameEventStore = defineStore('game-event', () => {
    const logs = ref([]);
    const chatMessages = ref([]);

    const MAX = 2000;

    const addLog = (message, logType) => {
        logs.value.push({ id: crypto.randomUUID(), message, logType });

        if (logs.value.length > MAX) {
            nextTick(() => logs.value.shift());
        }
    };

    const addChatMessage = (message, timestamp) => {
        chatMessages.value.push({ message, timestamp });

        if (chatMessages.value.length > MAX) {
            nextTick(() => chatMessages.value.shift());
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
        const serverInfoString = `
*** Connected with 7DTD server.
*** Server version: ${data.version.longString} Compatibility Version: ${data.version.compatibilityVersion}
*** Dedicated server only build

Server IP:   ${data.serverIP}
Server port: ${data.serverPort}
Max players: ${data.serverMaxPlayerCount}
Game mode:   ${data.gameMode}
World:       ${data.gameWorld}
Game name:   ${data.gameName}
Difficulty:  ${data.gameDifficulty}

Press 'help' to get a list of all commands.
`;
        console.log(serverInfoString);
        addLog(serverInfoString, 'Assert');
    };

    const controller = eventSource.listen({
        onMessage(message) {
            const data = JSON.parse(message.data);
            switch (message.event) {
                case 'Welcome':
                    onWelcomeMessage(data);
                case 'LogCallback':
                    addLog(data.message, data.logType);
                    break;
            }
            emitter.emit('Game.' + message.event, data);
        },
    });

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
        addLog,
    };
});
