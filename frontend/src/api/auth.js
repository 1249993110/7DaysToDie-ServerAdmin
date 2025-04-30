/**
 * login
 * @returns
 */
export const login = () => {
    return http.get('/auth/status');
};
