export interface JwtPayload{
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameIdentifier"?: string; // User ID
    unique_name?: string;
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"?: string; // Username
    given_name?: string;
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenName"?: string; // DisplayName
    role: string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?: string; // Role
    exp: number; // Expiration time
}