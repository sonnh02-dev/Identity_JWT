import React, { useState } from "react";

const Login: React.FC = () => {
    const [email, setEmail] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [error, setError] = useState<string>("");

    const handleLogin = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();
        setError("");

        try {
            const response = await fetch("/api/authentication/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email, password }),
            });

            if (response.ok) {
                const data = await response.json();
                console.log("Login success:", data);
                // Ví dụ lưu token:
                localStorage.setItem("token", data.token);
            } else {
                const errText = await response.text();
                setError(errText);
            }
        } catch {
            setError("Network error");
        }
    };

    return (
        <form onSubmit={handleLogin} className="bg-white p-6 rounded-2xl shadow-md w-96">
            <h1 className="text-2xl font-bold mb-4">Login</h1>
            {error && <p className="text-red-500">{error}</p>}

            <input
                type="email"
                placeholder="Email"
                className="w-full p-2 mb-3 border rounded-lg"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
            />

            <input
                type="password"
                placeholder="Password"
                className="w-full p-2 mb-3 border rounded-lg"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
            />

            <button type="submit" className="w-full bg-blue-600 text-white py-2 rounded-lg">
                Login
            </button>
        </form>
    );
};

export default Login;
