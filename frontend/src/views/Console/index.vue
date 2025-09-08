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
                :modelValue="currentCommand"
                @update:modelValue="onInputChange"
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
                    <div
                        class="flex items-center w-full"
                        v-tooltip.top="{
                            value: option.help,
                            // autoHide: false,
                            pt: { root: { style: { maxWidth: '80vw' } } },
                        }"
                    >
                        <div class="me-20">{{ option.cmd }}</div>
                        <div class="ms-auto overflow-hidden text-ellipsis whitespace-nowrap t-text-secondary text-sm">{{ option.desc }}</div>
                    </div>
                </template>
            </AutoComplete>
            <Button class="ms-2" type="button" :loading="isLoading" severity="secondary" :label="$t('common.button.send')" @click="sendCommand">
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
import { useCommandHistory } from '~/composables/useCommandHistory';
import { myToast } from '~/plugins/sweetalert2';
import { useGameEventStore } from '~/store/gameEvent';

const { t } = useI18n();
const gameEventStore = useGameEventStore();
const autoCompleteRef = ref();
const consoleContentRef = ref();
let allCommands = [];
let commandLookup = new Set();
const filteredCommands = ref([]);
const isLoading = ref(false);
const { currentCommand, navigateUp, navigateDown, addCommandToHistory, onInputChange } = useCommandHistory();

const commandText = computed(() => (typeof currentCommand.value === 'object' ? currentCommand.value.cmd : currentCommand.value));
const isCommandInvalid = computed(() => {
    const trimmedVal = commandText.value.trim();
    if (!trimmedVal) {
        return true;
    }

    const commandPart = trimmedVal.split(' ')[0].toLowerCase();
    return !commandLookup.has(commandPart);
});

const handleArrowUp = () => {
    if (!autoCompleteRef.value.overlayVisible) {
        navigateUp();
    }
};
const handleArrowDown = () => {
    if (!autoCompleteRef.value.overlayVisible) {
        navigateDown();
    }
};

getAllowedCommands()
    .then((data) => {
        const processedCommands = [];
        const lookupSet = new Set();
        data.forEach((group) => {
            group.commands.forEach((cmd) => {
                processedCommands.push({ cmd, desc: group.description, help: group.help });
                lookupSet.add(cmd.toLowerCase());
            });
        });
        allCommands = processedCommands;
        commandLookup = lookupSet;
    })
    .catch((error) => {});

let lastOverlayVisible = false;
const sendCommand = async () => {
    if (lastOverlayVisible) {
        return;
    }

    if (isCommandInvalid.value) {
        myToast({
            title: t('views.console.invalidCommand.title'),
            text: t('views.console.invalidCommand.text'),
            icon: 'error',
        });
        return;
    }

    isLoading.value = true;
    try {
        const data = await executeConsoleCommand(commandText.value, true);

        addCommandToHistory(commandText.value);
        onInputChange('');

        data.forEach((message) => {
            gameEventStore.addLog(message, 'Assert');
        });
    } finally {
        isLoading.value = false;
    }
};

const logColorMap = {
    Error: 'red',
    Exception: 'red',
    Assert: '#BBBFC4',
    Warning: 'yellow',
    Log: '#00C814',
};
const getColor = (logType) => logColorMap[logType] || '#00C814';

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
    let query = event.query;
    if (query.indexOf(' ') !== -1) {
        filteredCommands.value = [];
        return;
    }

    if (!query) {
        filteredCommands.value = [...allCommands];
        
    } else {
        query = query.trim().toLowerCase();
        filteredCommands.value = allCommands.filter((c) => c.cmd.toLowerCase().startsWith(query));
    }


};
</script>
