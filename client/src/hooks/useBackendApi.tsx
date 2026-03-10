import { privateApi } from "../services/api";
import { useEffect } from "react";
import useAuth from "./useAuth";

const useBackendApi = () => {
    const { auth } = useAuth();

    useEffect(() => {
        const requestIntercept = privateApi.interceptors.request.use(
            config => {
                if (!config.headers['Authorization']) {
                    config.headers['Authorization'] = `Bearer ${auth?.accessToken}`;
                }
                return config;
            }, (error) => Promise.reject(error)
        );

        // Cleanup function to remove interceptor when component unmounts
        return () => {
            privateApi.interceptors.request.eject(requestIntercept);
        }
    }, [auth]);

    return privateApi;
}

export default useBackendApi;