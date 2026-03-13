import api from '../services/api';
import useAuth from './useAuth';

export default function useRefreshToken() {
    const { setAuth } = useAuth();

    const refresh = async () => {
        const response = await api.post('/accounts/refresh', {}, {
            withCredentials: true 
        });

        const newAccessToken = response.data;

        setAuth(prev => {
            return { 
                ...prev, 
                accessToken: newAccessToken 
            };
        });
        
        return newAccessToken;
    };

    return refresh;
}