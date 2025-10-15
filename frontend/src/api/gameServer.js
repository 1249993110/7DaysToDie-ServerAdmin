export const getStats = () => {
    return http.get('/GameServer/Stats');
};

export const executeConsoleCommand = (command, inMainThread = true) => {
    return http.post('/GameServer/ExecuteConsoleCommand', {
        command,
        inMainThread,
    });
};

//#region Send Message
export const sendGlobalMessage = (message, senderName = null) => {
    return http.post('/GameServer/SendGlobalMessage', { message, senderName });
};
export const sendPrivateMessage = (targetPlayerIdOrName, message, senderName = null) => {
    return http.post('/GameServer/SendPrivateMessage', { targetPlayerIdOrName, message, senderName });
};
//#endregion

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

export const getPlayerSkills = (playerId, language) => {
    return http.get(`/GameServer/PlayerSkills/${playerId}`, { params: { language } });
};

export const getPlayerDetails = (playerId) => {
    return http.get(`/GameServer/PlayerDetails/${playerId}`);
};
//#endregion

//#region Map
export const getMapInfo = () => {
    return http.get('/GameServer/MapInfo');
};

export const renderFullMap = () => {
    return http.post('/GameServer/RenderFullMap');
};

export const renderExploredArea = () => {
    return http.post('/GameServer/RenderExploredArea');
};
//#endregion

//#region Locations
export const getLocations = (entityType) => {
    return http.get('/GameServer/Locations', { params: { entityType } });
};

export const getLocation = (entityId) => {
    return http.get(`/GameServer/Locations/${entityId}`);
};
//#endregion

//#region Localization
export const getLocalizationDict = (language) => {
    return http.get('/GameServer/Localization', { params: { language } });
};
export const getLocalizationByKey = (key, language, isCaseInsensitive = false) => {
    return http.get(`/GameServer/Localization/${key}`, { params: { language, caseInsensitive: isCaseInsensitive } });
};
export const getKnownLanguages = () => {
    return http.get('/GameServer/KnownLanguages');
};
//#endregion

//#region LandClaims
export const getLandClaims = () => {
    return http.get('/GameServer/LandClaims');
};
export const removePlayerLandClaim = (playerId) => {
    return http.delete(`/GameServer/LandClaims/${playerId}`);
};
export const removePlayerLandClaimByPosition = (x, y, z) => {
    return http.delete(`/GameServer/LandClaims`, { data: { x, y, z } });
};
//#endregion

//#region ServerSettings
export const getServerSettings = () => {
    return http.get('/GameServer/ServerSettings');
};

export const updateServerSettings = (settings) => {
    return http.put('/GameServer/ServerSettings', settings);
};
//#endregion

//#region Bans
export const getBannedPlayers = (params) => {
    return http.get('/GameServer/Bans', { params });
};
export const banPlayer = (playerId, bannedUntil, displayName, reason = null) => {
    return http.post('/GameServer/Bans', { playerId, bannedUntil, displayName, reason });
};
export const unbanPlayers = (playerIds) => {
    return http.delete(`/GameServer/Bans`, { params: { playerIds } });
};
//#endregion

//#region Whitelist
export const getWhitelistedPlayers = (params) => {
    return http.get('/GameServer/Whitelist', { params });
};
export const addPlayerToWhitelist = (playerId, displayName) => {
    return http.post('/GameServer/Whitelist', { playerId, displayName });
};
export const removePlayerFromWhitelist = (playerIds) => {
    return http.delete(`/GameServer/Whitelist`, { params: { playerIds } });
};
//#endregion

//#region Permissions
export const getAdminUsers = (params) => {
    return http.get('/GameServer/AdminUsers', { params });
}

export const addAdminUser = (data) => {
    return http.post('/GameServer/AdminUsers', data);
}

export const deleteAdminUsers = (playerIds) => {
    return http.delete('/GameServer/AdminUsers', { params: { playerIds } });
}

export const getCommandPermissions = (params) => {
    return http.get('/GameServer/CommandPermissions', { params });
}

export const addCommandPermission = (data) => {
    return http.post('/GameServer/CommandPermissions', data);
}

export const deleteCommandPermissions = (playerIds) => {
    return http.delete('/GameServer/CommandPermissions', { params: { playerIds } });
}
//#endregion

//#region AppSettings
export const getAppSettings = () => {
    return http.get('/AppSettings');
};
export const updateAppSettings = (settings) => {
    return http.put('/AppSettings', settings);
}
//#endregion