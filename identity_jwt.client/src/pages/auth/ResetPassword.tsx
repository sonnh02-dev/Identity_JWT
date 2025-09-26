import { useState } from "react";

export default function ResetPassword() {
    const [email, setEmail] = useState("");
    const [token, setToken] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [message, setMessage] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const res = await fetch("/api/authentication/reset-password", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, token, newPassword, confirmPassword })
        });
        setMessage(res.ok ? "Password reset successfully" : "Failed to reset password");
    };

    return (
        <form onSubmit={handleSubmit} className="space-y-4 max-w-sm mx-auto">
            <h2 className="text-xl font-bold">Reset Password</h2>
            <input
                type="email"
                placeholder="Your Email"
                value={email}
                onChange={e => setEmail(e.target.value)}
                className="w-full p-2 border rounded"
            />
            <input
                type="text"
                placeholder="Reset Token"
                value={token}
                onChange={e => setToken(e.target.value)}
                className="w-full p-2 border rounded"
            />
            <input
                type="password"
                placeholder="New Password"
                value={newPassword}
                onChange={e => setNewPassword(e.target.value)}
                className="w-full p-2 border rounded"
            />
            <input
                type="password"
                placeholder="Confirm Password"
                value={confirmPassword}
                onChange={e => setConfirmPassword(e.target.value)}
                className="w-full p-2 border rounded"
            />
            <button className="bg-blue-600 text-white px-4 py-2 rounded w-full">Reset Password</button>
            {message && <p className="text-sm mt-2">{message}</p>}
        </form>
    );
}
