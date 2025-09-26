import { useState } from "react";

export default function RequestChangeEmail() {
    const [newEmail, setNewEmail] = useState("");
    const [message, setMessage] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const token = localStorage.getItem("accessToken");
        const res = await fetch(`/api/authentication/request-change-email/${newEmail}`, {
            method: "POST",
            headers: {
                Authorization: `Bearer ${token}`
            }
        });
        setMessage(res.ok ? "Confirmation email sent" : "Failed to request email change");
    };

    return (
        <form onSubmit={handleSubmit} className="space-y-4 max-w-sm mx-auto">
            <h2 className="text-xl font-bold">Request Change Email</h2>
            <input
                type="email"
                placeholder="New Email"
                value={newEmail}
                onChange={e => setNewEmail(e.target.value)}
                className="w-full p-2 border rounded"
            />
            <button className="bg-blue-600 text-white px-4 py-2 rounded w-full">Request Change</button>
            {message && <p className="text-sm mt-2">{message}</p>}
        </form>
    );
}
