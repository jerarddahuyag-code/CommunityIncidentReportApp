import { createContext, useState, type ReactNode } from "react";
import { jwtDecode } from "jwt-decode"
import type { JwtPayload } from "../types/jwtPayload";

interface AuthData {
  username?: string;
  displayName?: string;
  role?: string;
  accessToken?: string;
}

interface AuthContextType {
  auth: AuthData;
  setAuth: (token: string) => void;
  logout: () => void;
}
const AuthContext = createContext<AuthContextType>({} as AuthContextType);


export const AuthProvider = ( {children} : {children : ReactNode}) => {
    const [auth, setAuthData] = useState<AuthData>({});

    const setAuth = (token: string) =>{
      try{
        const decoded = jwtDecode<JwtPayload>(token)
        console.log(decoded)
        const username = decoded.unique_name || decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || "";
        const displayName = decoded.given_name || decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenName"] || "";
        const role = decoded.role || decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

        console.log(username, displayName, role, token)
        setAuthData({ username, displayName, role, accessToken: token})
      } catch (error: any){
        console.error("Failed to decode token:", error);
        logout()
      }
    }

    const logout = () => {
      setAuthData({})
    }
    return (
        <AuthContext.Provider value={{auth, setAuth, logout}}>
            {children}
        </AuthContext.Provider>
    )
}

export default AuthContext;