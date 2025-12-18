
//#region Console Commands
/**
 * Sends a console command to the game server, optionally running it on the main thread.
 * @param {string} command Console text to execute.
 * @param {boolean} [inMainThread=true] Run the command on the primary game thread.
 * @returns {Promise} Execution result notice.
 */
export const executeConsoleCommand = (command, inMainThread = true) => {
    return http.post('/GameServer/ExecuteConsoleCommand', {
        command,
        inMainThread,
    });
};
//#endregion

//#region Statistics
/**
 * Retrieves global game server statistics such as player counts and uptime.
 * @returns {Promise} Server stats payload.
 */
export const getStats = () => {
    return http.get('/GameServer/Stats');
};
//#endregion


//#region Send Message
/**
 * Broadcasts a message to every player on the server with an optional sender name.
 * @param {string} message Text to display globally.
 * @param {string|null} [senderName=null] Optional sender label shown to players.
 * @returns {Promise} Server response acknowledgment.
 */
export const sendGlobalMessage = (message, senderName = null) => {
    return http.post('/GameServer/SendGlobalMessage', { message, senderName });
};
/**
 * Sends a direct private message to a specific player by ID or name.
 * @param {string|number} targetPlayerIdOrName Player identifier or display name.
 * @param {string} message Message text to deliver.
 * @param {string|null} [senderName=null] Optional sender label shown to the recipient.
 * @returns {Promise} Server response acknowledgment.
 */
export const sendPrivateMessage = (targetPlayerIdOrName, message, senderName = null) => {
    return http.post('/GameServer/SendPrivateMessage', { targetPlayerIdOrName, message, senderName });
};
//#endregion

/**
 * Retrieves the list of console commands the current user can run.
 * @returns {Promise} Allowed command metadata.
 */
export const getAllowedCommands = () => {
    return http.get('/GameServer/AllowedCommands');
};

/**
 * Reads the current game server configuration payload exposed to the frontend.
 * @returns {Promise} Server configuration details.
 */
export const getServerConfig = () => {
    return http.get('/GameServer/Config');
};

/**
 * Persists updated server configuration data.
 * @param {object} config Configuration object accepted by the server API.
 * @returns {Promise} Confirmation of the update operation.
 */
export const updateServerConfig = (config) => {
    return http.post('/GameServer/Config', config);
};

//#region Players
/**
 * Queries the currently online players with optional filters.
 * @param {object} params Query parameters such as pagination or filters.
 * @returns {Promise} Online player list.
 */
export const getOnlinePlayers = (params) => {
    return http.get('/GameServer/OnlinePlayers', { params });
};

/**
 * Retrieves detailed info for a single online player.
 * @param {string} playerId Unique identifier of the player.
 * @returns {Promise} Player record.
 */
export const getOnlinePlayer = (playerId) => {
    return http.get(`/GameServer/OnlinePlayers/${playerId}`);
};

/**
 * Lists historical players (offline or recently active) with search controls.
 * @param {object} params Search and pagination filters.
 * @returns {Promise} History player list.
 */
export const getHistoryPlayers = (params) => {
    return http.get('/GameServer/HistoryPlayers', { params });
};

/**
 * Fetches details for a specific historical player record.
 * @param {string} playerId Player identifier from the history table.
 * @returns {Promise} Historical player data.
 */
export const getHistoryPlayer = (playerId) => {
    return http.get(`/GameServer/HistoryPlayers/${playerId}`);
};

/**
 * Loads a player's inventory, optionally translated to a language.
 * @param {string} playerId Player identifier.
 * @param {string} language Language code used for localization.
 * @returns {Promise} Inventory payload.
 */
export const getPlayerInventory = (playerId, language) => {
    return http.get(`/GameServer/PlayerInventory/${playerId}`, { params: { language } });
};

/**
 * Fetches skill data for a player, with localization controls.
 * @param {string} playerId Player identifier.
 * @param {string} language Language code for skill descriptions.
 * @returns {Promise} Skill set payload.
 */
export const getPlayerSkills = (playerId, language) => {
    return http.get(`/GameServer/PlayerSkills/${playerId}`, { params: { language } });
};

/**
 * Retrieves comprehensive player metadata such as stats, binds, etc.
 * @param {string} playerId Player identifier.
 * @returns {Promise} Player detail response.
 */
export const getPlayerDetails = (playerId) => {
    return http.get(`/GameServer/PlayerDetails/${playerId}`);
};
//#endregion

//#region Map
/**
 * Loads metadata about the currently loaded map, including dimensions and generation info.
 * @returns {Promise} Map metadata payload.
 */
export const getMapInfo = () => {
    return http.get('/GameServer/MapInfo');
};

/**
 * Requests a server-side render of the full map (all explored and unexplored tiles).
 * @returns {Promise} Map render job reference.
 */
export const renderFullMap = () => {
    return http.post('/GameServer/RenderFullMap');
};

/**
 * Requests a server-side render of only the explored regions of the map.
 * @returns {Promise} Partial map render job reference.
 */
export const renderExploredArea = () => {
    return http.post('/GameServer/RenderExploredArea');
};
//#endregion

//#region Locations
/**
 * Searches locations of a specific entity type such as POIs or bases.
 * @param {string} entityType Type filter recognized by the API.
 * @returns {Promise} Location list.
 */
export const getLocations = (entityType) => {
    return http.get('/GameServer/Locations', { params: { entityType } });
};

/**
 * Retrieves a single location record by its unique identifier.
 * @param {number} entityId Location identifier.
 * @returns {Promise} Location details.
 */
export const getLocation = (entityId) => {
    return http.get(`/GameServer/Locations/${entityId}`);
};
//#endregion

//#region Localization
/**
 * Downloads the localization dictionary for a given language.
 * @param {string} language Language code (e.g., en, zh-cn).
 * @returns {Promise} Key/value localization entries.
 */
export const getLocalizationDict = (language) => {
    return http.get('/GameServer/Localization', { params: { language } });
};
/**
 * Looks up a single localization entry by key.
 * @param {string} key Localization key to query.
 * @param {string} language Language code for the translation.
 * @param {boolean} [isCaseInsensitive=false] Case-insensitive lookup flag.
 * @returns {Promise} Localization entry.
 */
export const getLocalizationByKey = (key, language, isCaseInsensitive = false) => {
    return http.get(`/GameServer/Localization/${key}`, { params: { language, caseInsensitive: isCaseInsensitive } });
};
/**
 * Retrieves the list of languages that the server currently supports.
 * @returns {Promise} Supported language codes.
 */
export const getKnownLanguages = () => {
    return http.get('/GameServer/KnownLanguages');
};
//#endregion

//#region LandClaims
/**
 * Pulls the list of registered land claims on the server.
 * @returns {Promise} Land claim entries with owner info.
 */
export const getLandClaims = () => {
    return http.get('/GameServer/LandClaims');
};
/**
 * Deletes every land claim associated with a player identifier.
 * @param {string} playerId Owner identifier used for cleanup.
 * @returns {Promise} Deletion result.
 */
export const removePlayerLandClaim = (playerId) => {
    return http.delete(`/GameServer/LandClaims/${playerId}`);
};
/**
 * Removes a claim located at the supplied coordinates.
 * @param {number} x X coordinate.
 * @param {number} y Y coordinate.
 * @param {number} z Z coordinate.
 * @returns {Promise} Deletion acknowledgement.
 */
export const removePlayerLandClaimByPosition = (x, y, z) => {
    return http.delete(`/GameServer/LandClaims`, { data: { x, y, z } });
};
//#endregion

//#region ServerSettings
/**
 * Fetches the runtime server settings such as difficulty and auto-save.
 * @returns {Promise} Server settings payload.
 */
export const getServerSettings = () => {
    return http.get('/GameServer/ServerSettings');
};

/**
 * Persists server settings modifications.
 * @param {object} settings Edited settings object.
 * @returns {Promise} Update confirmation.
 */
export const updateServerSettings = (settings) => {
    return http.put('/GameServer/ServerSettings', settings);
};
//#endregion

//#region Bans
/**
 * Lists currently banned players with optional search filters.
 * @param {object} params Filters such as pagination or status.
 * @returns {Promise} Banned player list.
 */
export const getBannedPlayers = (params) => {
    return http.get('/GameServer/Bans', { params });
};
/**
 * Adds or extends a ban for a specified player.
 * @param {string} playerId Identifier of the player to ban.
 * @param {string} bannedUntil ISO timestamp when the ban expires.
 * @param {string} displayName Name shown in the ban list.
 * @param {string|null} [reason=null] Optional ban reason presented to the player.
 * @returns {Promise} Ban operation result.
 */
export const banPlayer = (playerId, bannedUntil, displayName, reason = null) => {
    return http.post('/GameServer/Bans', { playerId, bannedUntil, displayName, reason });
};
/**
 * Lifts bans for one or more players.
 * @param {Array<string>} playerIds Identifiers of players to unban.
 * @returns {Promise} Unban acknowledgement.
 */
export const unbanPlayers = (playerIds) => {
    return http.delete(`/GameServer/Bans`, { params: { playerIds } });
};
//#endregion

//#region Whitelist
/**
 * Retrieves whitelisted players subject to optional filters.
 * @param {object} params Optional pagination or search filters.
 * @returns {Promise} Whitelist entries.
 */
export const getWhitelistedPlayers = (params) => {
    return http.get('/GameServer/Whitelist', { params });
};
/**
 * Adds a player to the whitelist so they bypass restrictions.
 * @param {string} playerId Identifier of the player.
 * @param {string} displayName Display name for bookkeeping.
 * @returns {Promise} Addition result.
 */
export const addPlayerToWhitelist = (playerId, displayName) => {
    return http.post('/GameServer/Whitelist', { playerId, displayName });
};
/**
 * Removes players from the whitelist.
 * @param {Array<string>} playerIds Identifiers to remove.
 * @returns {Promise} Removal confirmation.
 */
export const removePlayerFromWhitelist = (playerIds) => {
    return http.delete(`/GameServer/Whitelist`, { params: { playerIds } });
};
//#endregion

//#region Permissions
/**
 * Lists administrator users with optional filters.
 * @param {object} params Pagination or search filters.
 * @returns {Promise} Admin user list.
 */
export const getAdminUsers = (params) => {
    return http.get('/GameServer/AdminUsers', { params });
}

/**
 * Grants admin access to a player.
 * @param {object} data Admin user payload (playerId, permissions, etc.).
 * @returns {Promise} Creation result.
 */
export const addAdminUser = (data) => {
    return http.post('/GameServer/AdminUsers', data);
}

/**
 * Revokes admin privileges for the specified players.
 * @param {Array<string>} playerIds Identifiers to remove from admins.
 * @returns {Promise} Deletion confirmation.
 */
export const deleteAdminUsers = (playerIds) => {
    return http.delete('/GameServer/AdminUsers', { params: { playerIds } });
}

/**
 * Fetches command permission overrides for players or roles.
 * @param {object} params Filtering criteria.
 * @returns {Promise} Command permission list.
 */
export const getCommandPermissions = (params) => {
    return http.get('/GameServer/CommandPermissions', { params });
}

/**
 * Adds or updates command permissions for a player/role.
 * @param {object} data Permission payload describing commands and rules.
 * @returns {Promise} Creation result.
 */
export const addCommandPermission = (data) => {
    return http.post('/GameServer/CommandPermissions', data);
}

/**
 * Removes assigned command permissions for the provided players.
 * @param {Array<string>} playerIds Identifiers whose permissions to delete.
 * @returns {Promise} Deletion confirmation.
 */
export const deleteCommandPermissions = (playerIds) => {
    return http.delete('/GameServer/CommandPermissions', { params: { playerIds } });
}
//#endregion

//#region AppSettings
/**
 * Retrieves application-specific settings exposed by the API.
 * @returns {Promise} App settings object.
 */
export const getAppSettings = () => {
    return http.get('/AppSettings');
};
/**
 * Updates application settings like theme or notification preferences.
 * @param {object} settings New settings object.
 * @returns {Promise} Update response.
 */
export const updateAppSettings = (settings) => {
    return http.put('/AppSettings', settings);
}
//#endregion

//#region Mods
/**
 * Lists the managed mods that the server can enable or disable.
 * @returns {Promise} Mod metadata list.
 */
export const getMods = () => {
    return http.get('/GameServer/Mods');
};

// export const uploadMod = (formData) => {
//     return http.post('/GameServer/Mods', formData, {
//         headers: { 'Content-Type': 'multipart/form-data' },
//     });
// }

/**
 * Toggles loading state for a mod folder.
 * @param {string} folderName Folder name representing the mod.
 * @returns {Promise} Toggle result.
 */
export const toggleModStatus = (folderName) => {
    return http.put(`/GameServer/Mods?folderName=${folderName}`);
};
//#endregion