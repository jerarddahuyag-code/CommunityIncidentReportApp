import useAuth from './useAuth';
import useBackendApi from './useBackendApi';

export default function useLogout() {
    const { logout } = useAuth();
    const apiPrivate = useBackendApi();

    const initiateLogout = async () => {
        try {
            // Call your backend to revoke the token and clear the HttpOnly cookie
            await apiPrivate.post('/accounts/logout', {}, {
                withCredentials: true
            });
        } catch (err) {
            console.error("Logout failed on server", err);
        } finally {
            // Always clear the local React memory state, regardless of server success
            logout();
        }
    };

    return initiateLogout;
}