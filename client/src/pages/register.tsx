import { useEffect, useRef, useState, type SubmitEventHandler } from "react";
import { Link } from "react-router-dom";
import api from "../services/api";

const USER_REGEX = /^[a-zA-Z][a-zA-Z0-9-_]{3,23}$/;
const PWD_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%?*_]).{8,24}$/;
const REGISTER_URL = "/accounts/register"; // Updated to match your backend route!

function Register() {
  const userRef = useRef<HTMLInputElement>(null);
  const errRef = useRef<HTMLParagraphElement>(null);

  const [displayName, setDisplayName] = useState(""); // Added for .NET backend
  const [user, setUser] = useState("");
  const [validName, setValidName] = useState(false);

  const [pwd, setPwd] = useState("");
  const [validPwd, setValidPwd] = useState(false);

  const [matchPwd, setMatchPwd] = useState("");
  const [validMatch, setValidMatch] = useState(false);

  const [errMsg, setErrMsg] = useState("");
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    userRef.current?.focus();
  }, []);

  useEffect(() => {
    setValidName(USER_REGEX.test(user));
  }, [user]);

  useEffect(() => {
    setValidPwd(PWD_REGEX.test(pwd));
    setValidMatch(pwd === matchPwd);
  }, [pwd, matchPwd]);

  useEffect(() => {
    setErrMsg("");
  }, [user, pwd, matchPwd, displayName]);

  const handleSubmit: SubmitEventHandler = async (e) => {
    e.preventDefault();

    const v1 = USER_REGEX.test(user);
    const v2 = PWD_REGEX.test(pwd);
    if (!v1 || !v2 || !displayName) {
      setErrMsg("Invalid Entry or Missing Fields");
      return;
    }

    try {
      const response = await api.post(
        REGISTER_URL,
        {
          Username: user,
          Password: pwd,
          DisplayName: displayName,
          Role: 2,
        },
        {
          headers: { "Content-Type": "application/json" },
          withCredentials: true,
        },
      );
      if (response.status >= 200 && response.status < 400) {
        setSuccess(true);
      }
      setUser("");
      setPwd("");
      setMatchPwd("");
      setDisplayName("");
    } catch (error: any) {
      if (!error?.response) {
        setErrMsg("No Server Response");
      } else if (error.response?.status === 409) {
        setErrMsg("Username Taken");
      } else {
        setErrMsg("Registration Failed");
      }
      errRef.current?.focus();
    }
  };

  return (
    <section className="flex min-h-screen items-center justify-center bg-gray-50 p-4">
      <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-xl border border-gray-100">
        {success ? (
          <div className="text-center">
            <div className="mx-auto mb-4 inline-flex h-16 w-16 items-center justify-center rounded-full bg-green-100 text-3xl">
              ✅
            </div>
            <h1 className="text-2xl font-bold text-gray-900">Success!</h1>
            <p className="mt-2 text-gray-600 mb-6">
              Your account has been created.
            </p>
            <Link
              to="/login"
              className="w-full inline-block rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm hover:bg-blue-700 transition-all"
            >
              Sign In Now
            </Link>
          </div>
        ) : (
          <>
            <div className="mb-6 text-center">
              <h1 className="text-2xl font-bold text-gray-900">
                Create an Account
              </h1>
              <p className="mt-2 text-sm text-gray-500">
                Join the Community Watch
              </p>
            </div>

            <p
              ref={errRef}
              className={`${
                errMsg ? "mb-6 block" : "hidden"
              } rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700`}
              aria-live="assertive"
            >
              {errMsg}
            </p>

            <form onSubmit={handleSubmit} className="space-y-4">
              {/* Display Name Field */}
              <div>
                <label
                  htmlFor="displayName"
                  className="mb-1 block text-sm font-medium text-gray-700"
                >
                  Full Name
                </label>
                <input
                  type="text"
                  id="displayName"
                  ref={userRef}
                  autoComplete="off"
                  onChange={(e) => setDisplayName(e.target.value)}
                  value={displayName}
                  required
                  placeholder="John Doe"
                  className="w-full rounded-lg border border-gray-300 px-4 py-2 text-gray-900 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20"
                />
              </div>

              {/* Username Field */}
              <div>
                <label
                  htmlFor="username"
                  className="mb-1 block text-sm font-medium text-gray-700"
                >
                  Username{" "}
                  {user && !validName && (
                    <span className="text-red-500 text-xs ml-2">
                      4-24 chars, starts with a letter
                    </span>
                  )}
                </label>
                <input
                  type="text"
                  id="username"
                  autoComplete="off"
                  onChange={(e) => setUser(e.target.value)}
                  value={user}
                  required
                  aria-invalid={validName ? "false" : "true"}
                  className={`w-full rounded-lg border px-4 py-2 text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500/20 ${user && !validName ? "border-red-300 focus:border-red-500" : "border-gray-300 focus:border-blue-500"}`}
                />
              </div>

              {/* Password Field */}
              <div>
                <label
                  htmlFor="password"
                  className="mb-1 block text-sm font-medium text-gray-700"
                >
                  Password{" "}
                  {pwd && !validPwd && (
                    <span className="text-red-500 text-xs ml-2">
                      Requires uppercase, lowercase, number, symbol
                    </span>
                  )}
                </label>
                <input
                  type="password"
                  id="password"
                  onChange={(e) => setPwd(e.target.value)}
                  value={pwd}
                  required
                  aria-invalid={validPwd ? "false" : "true"}
                  className={`w-full rounded-lg border px-4 py-2 text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500/20 ${pwd && !validPwd ? "border-red-300 focus:border-red-500" : "border-gray-300 focus:border-blue-500"}`}
                />
              </div>

              {/* Confirm Password Field */}
              <div>
                <label
                  htmlFor="confirm_pwd"
                  className="mb-1 block text-sm font-medium text-gray-700"
                >
                  Confirm Password{" "}
                  {matchPwd && !validMatch && (
                    <span className="text-red-500 text-xs ml-2">
                      Must match password
                    </span>
                  )}
                </label>
                <input
                  type="password"
                  id="confirm_pwd"
                  onChange={(e) => setMatchPwd(e.target.value)}
                  value={matchPwd}
                  required
                  aria-invalid={validMatch ? "false" : "true"}
                  className={`w-full rounded-lg border px-4 py-2 text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-500/20 ${matchPwd && !validMatch ? "border-red-300 focus:border-red-500" : "border-gray-300 focus:border-blue-500"}`}
                />
              </div>

              <button
                disabled={
                  !validName || !validPwd || !validMatch || !displayName
                }
                className="mt-6 w-full cursor-pointer rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm hover:bg-blue-700 focus:outline-none disabled:bg-gray-400 disabled:cursor-not-allowed transition-all"
              >
                Sign up
              </button>
            </form>

            <div className="mt-6 text-center text-sm text-gray-600">
              <p>
                Already registered?{" "}
                <Link
                  to="/login"
                  className="font-semibold text-blue-600 hover:underline"
                >
                  Sign In
                </Link>
              </p>
            </div>
          </>
        )}
      </div>
    </section>
  );
}

export default Register;
