import { useEffect, useRef, useState, type SubmitEventHandler } from "react";
import api from "./../services/api";
import useAuth from "../hooks/useAuth";
import { useNavigate, Link } from "react-router-dom";

const LOGIN_URL = "/accounts/login";

function Login() {
  const navigate = useNavigate();
  const { setAuth } = useAuth();
  const userRef = useRef<HTMLInputElement>(null);
  const errRef = useRef<HTMLParagraphElement>(null);

  const [user, setUser] = useState("");
  const [pwd, setPwd] = useState("");
  const [errMsg, setErrMsg] = useState("");

  useEffect(() => {
    userRef.current?.focus();
  }, []);

  useEffect(() => {
    setErrMsg("");
  }, [user, pwd]);

  const handleSubmit: SubmitEventHandler = async (e) => {
    e.preventDefault();
    try {
      const response = await api.post(
        LOGIN_URL,
        {
          Username: user,
          Password: pwd,
        },
        {
          headers: { "Content-Type": "application/json" },
          withCredentials: true,
        },
      );

      const accessToken = response?.data;
      setAuth( accessToken );
      navigate("/incidents/feed");
      setPwd("");
      setUser("");
    } catch (error: any) {
      if (!error?.response) {
        setErrMsg("No Server Response");
      } else {
        setErrMsg("Log in Failed");
      }
      errRef.current?.focus();
    }
  };

  return (
    // Outer wrapper: Full height, flexbox to center the card, light gray background
    <section className="flex min-h-screen items-center justify-center bg-gray-50 p-4">
      
      {/* The Login Card */}
      <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-xl border border-gray-100">
        
        {/* Header / Logo Space */}
        <div className="mb-8 text-center">
          <div className="mx-auto mb-4 inline-flex h-12 w-12 items-center justify-center rounded-full bg-blue-100">
            <span className="text-2xl">🛡️</span> {/* Placeholder for an actual logo/SVG */}
          </div>
          <h2 className="text-2xl font-bold text-gray-900">Welcome Back</h2>
          <p className="mt-2 text-sm text-gray-500">Sign in to your account to continue</p>
        </div>

        {/* Error Message Alert */}
        <p
          ref={errRef}
          className={`${
            errMsg ? "mb-6 block" : "hidden"
          } rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700`}
          aria-live="assertive"
        >
          {errMsg}
        </p>

        {/* The Form */}
        <form onSubmit={handleSubmit} className="space-y-5">
          <div>
            <label htmlFor="username" className="mb-1 block text-sm font-medium text-gray-700">
              Username
            </label>
            <input
              type="text"
              id="username"
              ref={userRef}
              autoComplete="on"
              onChange={(e) => setUser(e.target.value)}
              value={user}
              required
              placeholder="Enter your username"
              className="w-full rounded-lg border border-gray-300 px-4 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 transition-all"
            />
          </div>

          <div>
            <label htmlFor="password" className="mb-1 block text-sm font-medium text-gray-700">
              Password
            </label>
            <input
              type="password"
              id="password"
              onChange={(e) => setPwd(e.target.value)}
              value={pwd}
              required
              placeholder="••••••••"
              className="w-full rounded-lg border border-gray-300 px-4 py-2 text-gray-900 placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 transition-all"
            />
          </div>

          <button 
            type="submit"
            className="mt-6 w-full cursor-pointer rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all active:scale-[0.98]"
          >
            Log in
          </button>
        </form>

        {/* Footer */}
        <div className="mt-8 text-center text-sm text-gray-600">
          <p>
            Need an Account?{" "}
            <Link 
              to="/register" 
              className="font-semibold text-blue-600 hover:text-blue-500 hover:underline"
            >
              Create an Account
            </Link>
          </p>
        </div>
      </div>
    </section>
  );
}

export default Login;