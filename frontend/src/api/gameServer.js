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

export const getServerConfig = () => {
    return http.get('/GameServer/Config');
};

export const updateServerConfig = (config) => {
    return http.post('/GameServer/Config', config);
};

//#region Players
export const getOnlinePlayers = (params) => {
    return http.get('/GameServer/OnlinePlayers', { params });
};

export const getOnlinePlayer = (playerId) => {
    return http.get(`/GameServer/OnlinePlayers/${playerId}`);
};

export const getHistoryPlayers = (params) => {
    return http.get('/GameServer/HistoryPlayers', { params });
};

export const getHistoryPlayer = (playerId) => {
    return http.get(`/GameServer/HistoryPlayers/${playerId}`);
};

export const getPlayerInventory = (playerId) => {
    return http.get(`/GameServer/PlayerInventory/${playerId}`);
};

export const getPlayerSkills = (playerId, lang) => {
    return http.get(`/GameServer/PlayerSkills/${playerId}`, { params: { language: lang } });
};

export const getPlayerDetails = (playerId) => {
    return http.get(`/GameServer/PlayerDetails/${playerId}`);
}
//#endregion
