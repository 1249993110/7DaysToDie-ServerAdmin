/**
 * A Vue composable for managing command-line history.
 * @param {object} options - Configuration options
 * @param {number} [options.maxSize=50] - Maximum number of history entries
 */
export const useCommandHistory = (options = {}) => {
    const { maxSize = 50 } = options;

    // Store confirmed command history
    const history = ref([]);
    
    // Pointer to the current command index being viewed in history.
    // The special value `history.value.length` indicates the user is in the "draft" state (typing a new command).
    const historyIndex = ref(0);

    // Current command shown in the input box
    const currentCommand = ref('');

    // Internal variable to store the user's draft before navigation starts
    let draftCommand = '';

    /**
     * Navigate up (view older commands)
     */
    const navigateUp = () => {
        // Only navigate up when the pointer is not at the oldest record
        if (historyIndex.value > 0) {
            // If starting navigation from the "draft" state, save the current input first
            if (historyIndex.value === history.value.length) {
                draftCommand = currentCommand.value;
            }
            historyIndex.value--;
            currentCommand.value = history.value[historyIndex.value];
        }
    };

    /**
     * Navigate down (view newer commands or return to draft)
     */
    const navigateDown = () => {
        // Only navigate down when the pointer is not in the "draft" state
        if (historyIndex.value < history.value.length) {
            historyIndex.value++;
            // If navigated to the "draft" state, restore the previously saved draft
            if (historyIndex.value === history.value.length) {
                currentCommand.value = draftCommand;
            } else {
                currentCommand.value = history.value[historyIndex.value];
            }
        }
    };

    /**
     * Add a command to history
     * @param {string} command - The command to add
     */
    const addCommandToHistory = (command) => {
        const trimmedCommand = command.trim();
        // Avoid adding empty commands or commands identical to the previous one
        if (trimmedCommand && history.value[history.value.length - 1] !== trimmedCommand) {
            history.value.push(trimmedCommand);
            // If history exceeds the limit, remove the oldest entry
            if (history.value.length > maxSize) {
                history.value.shift();
            }
        }
        // Reset pointer to "draft" state and clear draft
        historyIndex.value = history.value.length;
        draftCommand = '';
    };

    /**
     * Called when the user types in the input box
     * @param {string} command - Current value of the input box
     */
    const onInputChange = (command) => {
        currentCommand.value = command;
        // Once the user starts typing, they are editing the "draft", so reset the navigation pointer
        historyIndex.value = history.value.length;
        // And update the draft content
        draftCommand = command;
    };

    return {
        currentCommand: readonly(currentCommand),
        navigateUp,
        navigateDown,
        addCommandToHistory,
        onInputChange,
    };
}