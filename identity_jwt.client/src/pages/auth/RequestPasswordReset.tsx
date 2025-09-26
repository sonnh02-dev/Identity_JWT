import { useState } from "react";

export default function RequestPasswordReset() {
    const [email, setEmail] = useState("");
    const [message, setMessage] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const res = await fetch("/api/authentication/request-password-reset", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email })
        });
        setMessage("Nếu email tồn tại, hướng dẫn reset đã được gửi.");
    };

    return (
        <form onSubmit={handleSubmit} className="space-y-4 max-w-sm mx-auto">
            <h2 className="text-xl font-bold">Forgot Password</h2>
            <input
                type="email"
                placeholder="Your Email"
                value={email}
                onChange={e => setEmail(e.target.value)}
                className="w-full p-2 border rounded"
            />
            <button className="bg-blue-600 text-white px-4 py-2 rounded w-full">Request Reset</button>
            {message && <p className="text-sm mt-2">{message}</p>}
        </form>
    );
}
