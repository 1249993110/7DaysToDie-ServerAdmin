export const authUrl = '/oauth/token';

/**
 * Sign in
 * @returns
 */
export const signIn = (username, password) => {
    return http.post(authUrl, { grant_type: 'password', username, password }, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } });
};

/**
 * Refresh token
 * @returns
 */
export const refreshToken = (refreshToken) => {
    return http.post(authUrl, { grant_type: 'refresh_token', refresh_token: refreshToken }, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } });
};
