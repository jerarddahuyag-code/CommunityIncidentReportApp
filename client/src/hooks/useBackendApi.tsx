import privateApi from "../services/api";
import { useEffect } from "react";
import useAuth from "./useAuth";
import useRefreshToken from "./useRefreshToken";
import { useNavigate, useLocation } from "react-router-dom";

const useBackendApi = () => {
    const refresh = useRefreshToken();
    const { auth, setAuth, logout } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        const requestIntercept = privateApi.interceptors.request.use(
            config => {
                if (!config.headers['Authorization']) {
                    config.headers['Authorization'] = `Bearer ${auth?.accessToken}`;
                }
                return config;
            },
            error => Promise.reject(error)
        );

        const responseIntercept = privateApi.interceptors.response.use(
            response => response,
            async (error) => {
                const prevRequest = error?.config;

                if (error?.response?.status === 401 && !prevRequest?.sent) {
                    prevRequest.sent = true;
                    try {
                        const newAccessToken = await refresh();

                        prevRequest.headers['Authorization'] = `Bearer ${newAccessToken}`;

                        return privateApi(prevRequest);
                    } catch (refreshError) {
                        logout();

                        navigate('/login', { state: { from: location }, replace: true });

                        return Promise.reject(refreshError);
                    }
                }

                const errorMessage = error.response?.data?.message || 'An unexpected network error occurred.';

                window.dispatchEvent(new CustomEvent('api-error', { detail: errorMessage }));

                return Promise.reject(error);
            }
        );

        // Cleanup function to remove interceptors when the component unmounts
        // This prevents memory leaks and overlapping interceptors
        return () => {
            privateApi.interceptors.request.eject(requestIntercept);
            privateApi.interceptors.response.eject(responseIntercept);
        }
    }, [auth, refresh, navigate, location, setAuth]);

    return privateApi;
}

export default useBackendApi;