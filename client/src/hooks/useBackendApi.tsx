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
        // 1. Request Interceptor: Attach the JWT to every request
        const requestIntercept = privateApi.interceptors.request.use(
            config => {
                if (!config.headers['Authorization']) {
                    config.headers['Authorization'] = `Bearer ${auth?.accessToken}`;
                }
                return config;
            },
            error => Promise.reject(error)
        );

        // 2. Response Interceptor: Catch the 401s
        const responseIntercept = privateApi.interceptors.response.use(
            response => response, // If response is good, just return it
            async (error) => {
                const prevRequest = error?.config;
                
                // If it's a 401 AND we haven't retried yet
                if (error?.response?.status === 401 && !prevRequest?.sent) {
                    prevRequest.sent = true; // Mark as retried so we don't infinite loop!
                    
                    try {
                        // Attempt to get a new token
                        const newAccessToken = await refresh();
                        
                        // Update the failed request with the new token
                        prevRequest.headers['Authorization'] = `Bearer ${newAccessToken}`;
                        
                        // Retry the request
                        return privateApi(prevRequest);
                    } catch (refreshError) {
                        // If the refresh fails (e.g. Refresh token is expired/invalid)
                        logout();
                        
                        // Kick them to login, but remember where they came from
                        navigate('/login', { state: { from: location }, replace: true });
                        
                        return Promise.reject(refreshError);
                    }
                }
                
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