import useAuth from './useAuth';
import useBackendApi from './useBackendApi';

export default function useLogout() {
    const { setAuth } = useAuth();
    const apiPrivate = useBackendApi();

    const logout = async () => {
        try {
            // Call your backend to revoke the token and clear the HttpOnly cookie
            await apiPrivate.post('/accounts/logout');
        } catch (err) {
            console.error("Logout failed on server", err);
        } finally {
            // Always clear the local React memory state, regardless of server success
            setAuth({});
        }
    };

    return logout;
}