import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:7089",
});

export const privateApi = axios.create({
  baseURL: "https://localhost:7089",
  headers: { "Content-Type": "application/json" },
  withCredentials: true,
});

export default api;
