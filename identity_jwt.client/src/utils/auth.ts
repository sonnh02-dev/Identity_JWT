export const getAccessToken = () => localStorage.getItem("accessToken");
export const setAccessToken = (token: string) =>
    localStorage.setItem("accessToken", token);
export const removeAccessToken = () => localStorage.removeItem("accessToken");

export const getRefreshToken = () => localStorage.getItem("refreshToken");
export const setRefreshToken = (token: string) =>
    localStorage.setItem("refreshToken", token);
export const removeRefreshToken = () =>
    localStorage.removeItem("refreshToken");

export const clearAuth = () => {
    removeAccessToken();
    removeRefreshToken();
};
