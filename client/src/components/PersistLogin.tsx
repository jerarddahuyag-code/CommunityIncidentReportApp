import { Outlet } from "react-router-dom";
import { useState, useEffect } from "react";
import useRefreshToken from "../hooks/useRefreshToken";
import useAuth from "../hooks/useAuth";

const PersistLogin = () => {
    const [isLoading, setIsLoading] = useState(true);
    const refresh = useRefreshToken();
    const { auth } = useAuth();

    useEffect(() => {
        let isMounted = true;

        const verifyRefreshToken = async () => {
            try {
                await refresh(); // Try to get a new JWT using the HttpOnly cookie
            } catch (err) {
                console.error("No valid refresh token found.", err);
            } finally {
                isMounted && setIsLoading(false);
            }
        }

        // Only run the refresh check if we don't currently have a token
        !auth?.accessToken ? verifyRefreshToken() : setIsLoading(false);

        return () => { isMounted = false; }
    }, [auth?.accessToken, refresh])

    return (
        <>
            {isLoading 
                ? <div className="flex items-center justify-center min-h-screen text-gray-500 animate-pulse">Loading Application...</div> 
                : <Outlet />
            }
        </>
    )
}

export default PersistLogin;