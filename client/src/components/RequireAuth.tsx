import { useLocation, Navigate, Outlet } from "react-router-dom";
import useAuth from "../hooks/useAuth"; // Assuming you have your auth context here

const RequireAuth = () => {
    const { auth } = useAuth();
    const location = useLocation();

    // Check if we have an access token in memory
    return (
        auth?.accessToken 
            ? <Outlet /> // Let them through to the protected page
            : <Navigate to="/login" state={{ from: location }} replace /> // Kick to login
    );
}

export default RequireAuth;