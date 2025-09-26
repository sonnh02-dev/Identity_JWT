import { useState } from "react";
import axiosClient from "../../api/axiosClient";

export default function ChangePassword() {
    const [oldPassword, setOldPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [message, setMessage] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await axiosClient.post("/authentication/change-password", {
                oldPassword,
                newPassword,
                confirmPassword,
            });
            setMessage("Password changed successfully!");
        } catch{
            setMessage( "Failed to change password");
        }
    };

    return (
        <form onSubmit={handleSubmit} className="space-y-4 max-w-sm mx-auto">
            <h2 className="text-xl font-bold">Change Password</h2>
            <input
                type="password"
                placeholder="Old Password"
                value={oldPassword}
                onChange={e => setOldPassword(e.target.value)}
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
            <button className="bg-blue-600 text-white px-4 py-2 rounded w-full">Submit</button>
            {message && <p className="text-sm mt-2">{message}</p>}
        </form>
    );
}
