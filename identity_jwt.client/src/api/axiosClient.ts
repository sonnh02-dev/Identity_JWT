import axios, { AxiosError, InternalAxiosRequestConfig } from "axios";
import {
    getAccessToken,
    getRefreshToken,
    setAccessToken,
    clearAuth,
} from "@/utils/auth";

const axiosClient = axios.create({
    baseURL: "/api", // proxy trong vite.config sẽ forward sang backend
    headers: {
        "Content-Type": "application/json",
    },
});

// ✅ Gắn access token trước khi gửi request
axiosClient.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
        const token = getAccessToken();
        if (token && config.headers) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// ✅ Biến để tránh gọi refresh trùng lặp
let isRefreshing = false;
let failedQueue: {
    resolve: (value?: unknown) => void;
    reject: (reason?: any) => void;
}[] = [];

const processQueue = (error: any, token: string | null = null) => {
    failedQueue.forEach((prom) => {
        if (error) {
            prom.reject(error);
        } else {
            prom.resolve(token);
        }
    });
    failedQueue = [];
};

// ✅ Xử lý khi token hết hạn
axiosClient.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {
        const originalRequest = error.config as InternalAxiosRequestConfig & {
            _retry?: boolean;
        };

        // Nếu không phải 401 hoặc request đã retry thì reject
        if (error.response?.status !== 401 || originalRequest._retry) {
            return Promise.reject(error);
        }

        if (isRefreshing) {
            return new Promise((resolve, reject) => {
                failedQueue.push({ resolve, reject });
            })
                .then((token) => {
                    if (originalRequest.headers && typeof token === "string") {
                        originalRequest.headers.Authorization = `Bearer ${token}`;
                    }
                    return axiosClient(originalRequest);
                })
                .catch((err) => Promise.reject(err));
        }

        originalRequest._retry = true;
        isRefreshing = true;

        try {
            const refreshToken = getRefreshToken();
            if (!refreshToken) throw new Error("No refresh token");

            const res = await axios.post("/api/authentication/refresh-token", {
                refreshToken,
            });

            const newAccessToken = res.data?.accessToken;
            if (!newAccessToken) throw new Error("No access token in response");

            setAccessToken(newAccessToken);
            processQueue(null, newAccessToken);

            if (originalRequest.headers) {
                originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
            }
            return axiosClient(originalRequest);
        } catch (err) {
            processQueue(err, null);
            clearAuth();
            window.location.href = "/login"; // logout user
            return Promise.reject(err);
        } finally {
            isRefreshing = false;
        }
    }
);

export default axiosClient;
