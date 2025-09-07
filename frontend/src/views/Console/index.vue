<template>
    <div class="h-full flex flex-col">
        <Card class="flex-1">
            <template #content>
                <div ref="consoleContentRef" class="h-[calc(100vh-210px)] overflow-y-auto">
                    <p v-for="logEntry in gameEventStore.logs" :key="logEntry.id" :style="{ color: getColor(logEntry.logType) }" class="font-mono whitespace-pre-wrap">
                        {{ logEntry.message }}
                    </p>
                </div>
            </template>
        </Card>
        <div class="mt-2 flex" v-focustrap>
            <AutoComplete
                class="flex-1"
                ref="autoCompleteRef"
                v-model="selectedCommand"
                optionLabel="cmd"
                :suggestions="filteredCommands"
                @complete="search"
                @keyup.enter="sendCommand"
                @keydown.enter="lastOverlayVisible = autoCompleteRef.overlayVisible"
                @keydown.up.prevent="handleArrowUp"
                @keydown.down.prevent="handleArrowDown"
                dropdown
                :pt="{ pcInputText: { root: { autofocus: true } }, list: { class: 'max-w-full' } }"
                :virtualScrollerOptions="{ itemSize: 38 }"
                :invalid="isCommandInvalid"
                :showEmptyMessage="false"
            >
                <template #option="{ option }">
                    <div class="flex items-center w-full" v-tooltip.top="{ value: option.help, autoHide: false, pt: { root: { style: { maxWidth: 'none' } } } }">
                        <div class="me-20">{{ option.cmd }}</div>
                        <div class="ms-auto overflow-hidden text-ellipsis whitespace-nowrap t-text-secondary text-sm">{{ option.desc }}</div>
                    </div>
                </template>
            </AutoComplete>
            <Button class="ms-2" type="button" :loading="isLoading" severity="secondary" label="Send" @click="sendCommand">
                <template #icon>
                    <icon-mdi:send />
                </template>
            </Button>
        </div>
    </div>
</template>

<script>
export default {
    name: 'Console',
};
</script>

<script setup>
import { executeConsoleCommand, getAllowedCommands } from '~/api/gameServer';

const { t } = useI18n();
const gameEventStore = useGameEventStore();
const autoCompleteRef = ref();
const consoleContentRef = ref();
let allowedCommands = [];
const filteredCommands = ref([]);
const isLoading = ref(false);
const selectedCommand = ref();
const commandText = computed(() => (typeof selectedCommand.value === 'object' ? selectedCommand.value.cmd : selectedCommand.value));
const isCommandInvalid = computed(() => {
    const trimmedVal = commandText.value ? commandText.value.trim().toLowerCase() : '';
    return !trimmedVal || !allowedCommands.some((group) => group.commands.some((cmd) => trimmedVal === cmd.toLowerCase() || trimmedVal.startsWith(cmd.toLowerCase() + ' ')));
});

const commandHistory = [];
let historyIndex = -1;
const handleArrowUp = () => {
    if (autoCompleteRef.value.overlayVisible) {
        return;
    }
    if (commandHistory.length > 0 && historyIndex < commandHistory.length - 1) {
        historyIndex++;
        selectedCommand.value = commandHistory[commandHistory.length - 1 - historyIndex];
    }
};
const handleArrowDown = () => {
    if (autoCompleteRef.value.overlayVisible) {
        return;
    }
    if (commandHistory.length > 0 && historyIndex >= 0) {
        historyIndex--;
        selectedCommand.value = historyIndex === -1 ? '' : commandHistory[commandHistory.length - 1 - historyIndex];
    }
};

getAllowedCommands()
    .then((data) => {
        allowedCommands = data;
    })
    .catch((error) => {});

let lastOverlayVisible = false;
const sendCommand = async () => {
    if (lastOverlayVisible) {
        return;
    }

    if (isCommandInvalid.value) {
        myToast({
            title: 'Invalid Command',
            text: 'Please enter a valid command.',
            icon: 'error',
        });
        return;
    }

    isLoading.value = true;
    try {
        const data = await executeConsoleCommand(commandText.value, true);

        if (commandHistory.length === 0 || commandHistory[commandHistory.length - 1] !== commandText.value) {
            commandHistory.push(commandText.value);
        }
        historyIndex = -1;

        selectedCommand.value = '';

        data.forEach((message) => {
            gameEventStore.addLog(message, 'Assert');
        });
    } finally {
        isLoading.value = false;
    }
};

const getColor = (logType) => {
    switch (logType) {
        case 'Error':
        case 'Exception':
            return 'red';
        case 'Assert':
            return '#BBBFC4';
        case 'Warning':
            return 'yellow';
        case 'Log':
        default:
            return '#00C814';
    }
};

const { pause, resume } = watch(
    () => gameEventStore.logs.length,
    () => {
        const element = consoleContentRef.value;
        if (element) {
            const isScrolledToBottom = element.scrollTop + element.clientHeight >= element.scrollHeight - 1;
            if (isScrolledToBottom) {
                nextTick(() => {
                    element.scrollTop = element.scrollHeight;
                });
            }
        }
    }
);

onActivated(() => {
    consoleContentRef.value.scrollTop = consoleContentRef.value.scrollHeight;
    resume();
});

onDeactivated(() => {
    pause();
});

const search = (event) => {
    let input = event.query;
    if (input.indexOf(' ') !== -1) {
        filteredCommands.value = [];
        return;
    }

    const result = [];
    if (!input) {
        allowedCommands.forEach((group) => {
            group.commands.forEach((cmd) => {
                result.push({ cmd, desc: group.description, help: group.help });
            });
        });
    } else {
        input = input.trim().toLowerCase();
        allowedCommands
            .flatMap((group) => group.commands)
            .filter((cmd) => cmd.toLowerCase().startsWith(input))
            .forEach((cmd) => {
                const group = allowedCommands.find((g) => g.commands.includes(cmd));
                if (input === cmd) {
                    result.unshift({ cmd, desc: group.description, help: group.help });
                } else {
                    result.push({ cmd, desc: group.description, help: group.help });
                }
            });
    }

    filteredCommands.value = result;
};
</script>
