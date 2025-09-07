export const getStats = () => {
    return http.get('/GameServer/Stats');
};

export const executeConsoleCommand = (command, inMainThread = true) => {
    return http.post('/GameServer/ExecuteConsoleCommand', {
        command,
        inMainThread,
    });
};

export const getAllowedCommands = () => {
    return http.get('/GameServer/AllowedCommands');
};
