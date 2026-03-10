import { createContext, useState, type ReactNode } from "react";

interface AuthData {
  user?: string;
  roles?: number[];
  accessToken?: string;
}

// 2. Define the Context shape
interface AuthContextType {
  auth: AuthData;
  setAuth: React.Dispatch<React.SetStateAction<AuthData>>;
}
const AuthContext = createContext<AuthContextType>({} as AuthContextType);


export const AuthProvider = ( {children} : {children : ReactNode}) => {
    const [auth, setAuth] = useState<AuthData>({});

    return (
        <AuthContext.Provider value={{auth, setAuth}}>
            {children}
        </AuthContext.Provider>
    )
}

export default AuthContext;