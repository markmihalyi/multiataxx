import axios, { AxiosError, AxiosRequestConfig } from "axios";

export const API_BASE_URL = "http://localhost:5000";

const api = axios.create({
	baseURL: API_BASE_URL,
	withCredentials: true,
});

export const handleAxiosError = (error: unknown) => {
	if (axios.isAxiosError(error)) {
		return error.response?.data;
	}
	return null;
};

let isRefreshing = false;
let failedQueue: {
	resolve: (value?: unknown) => void;
	reject: (error: unknown) => void;
}[] = [];

const processQueue = (
	error: AxiosError | null,
	token: string | null = null
) => {
	failedQueue.forEach((prom) => {
		if (error) {
			prom.reject(error);
		} else {
			prom.resolve(token);
		}
	});
	failedQueue = [];
};

api.interceptors.response.use(
	(response) => response,
	async (error: AxiosError) => {
		const originalRequest = error.config as AxiosRequestConfig & {
			_retry?: boolean;
		};

		if (error.response?.status === 401 && !originalRequest._retry) {
			if (isRefreshing) {
				return new Promise((resolve, reject) => {
					failedQueue.push({ resolve, reject });
				})
					.then(() => api(originalRequest))
					.catch((err) => Promise.reject(err));
			}

			originalRequest._retry = true;
			isRefreshing = true;

			try {
				await axios.post(
					`${API_BASE_URL}/api/auth/refresh`,
					{},
					{ withCredentials: true }
				);

				processQueue(null);
				return api(originalRequest);
			} catch (err) {
				processQueue(err as AxiosError, null);
				return Promise.reject(err);
			} finally {
				isRefreshing = false;
			}
		}

		return Promise.reject(error);
	}
);

export default api;
