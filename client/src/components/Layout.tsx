import { Outlet, useNavigate } from "react-router-dom";
import useLogout from "../hooks/useLogout";
import useAuth from "../hooks/useAuth";

export default function Layout() {
    const navigate = useNavigate();
    const logout = useLogout();
    const { auth } = useAuth(); 

    const handleLogout = async () => {
        await logout();
        navigate('/login', { replace: true });
    };

    return (
        <div className="min-h-screen flex flex-col bg-gray-50">
            {/* Top Navigation Bar */}
            <nav className="bg-white shadow-sm p-4 sticky top-0 z-50">
                <div className="max-w-4xl mx-auto flex justify-between items-center">
                    
                    {/* App Logo/Title */}
                    <h1 
                        className="font-bold text-blue-600 text-xl cursor-pointer hover:text-blue-700 transition-colors"
                        onClick={() => navigate('/incidents/feed')}
                    >
                        Community Watch
                    </h1>
                    
                    {/* Conditional Right-Side Menu */}
                    {auth?.accessToken ? (
                        <div className="flex items-center gap-4">
                            {/* Optional: Show username if you store it in your auth state */}
                            {auth.user && (
                                <span className="text-sm text-gray-500 hidden sm:block">
                                    Welcome, <span className="font-semibold text-gray-900">{auth.user}</span>
                                </span>
                            )}
                            
                            <button 
                                onClick={handleLogout}
                                className="text-sm font-medium text-gray-600 hover:text-red-600 transition-colors bg-gray-100 hover:bg-red-50 px-4 py-2 rounded-lg"
                            >
                                Logout
                            </button>
                        </div>
                    ) : (
                        /* If they aren't logged in, maybe show a login link (useful if Layout wraps public pages) */
                        <button 
                            onClick={() => navigate('/login')}
                            className="text-sm font-medium text-blue-600 hover:text-blue-800"
                        >
                            Log in
                        </button>
                    )}
                </div>
            </nav>
            
            {/* Main Content Area */}
            {/* flex-grow pushes the footer (if you add one) to the bottom, max-w-4xl keeps everything centered and readable */}
            <main className="flex-grow w-full max-w-4xl mx-auto p-4 md:p-6 lg:p-8">
                <Outlet />
            </main>
        </div>
    );
}