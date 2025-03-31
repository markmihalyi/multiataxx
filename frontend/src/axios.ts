import axios from "axios";

const API_BASE_URL = "http://localhost:5000";

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

export default api;
