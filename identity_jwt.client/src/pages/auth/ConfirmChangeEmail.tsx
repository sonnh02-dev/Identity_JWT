import { useEffect, useState } from "react";

export default function ConfirmChangeEmail() {
    const [message, setMessage] = useState("Confirming...");

    useEffect(() => {
        const params = new URLSearchParams(window.location.search);
        const userId = params.get("userId");
        const email = params.get("email");
        const token = params.get("token");

        if (userId && email && token) {
            fetch(`/api/authentication/confirm-change-email?userId=${userId}&email=${email}&token=${encodeURIComponent(token)}`)
                .then(res => setMessage(res.ok ? "Email changed successfully" : "Failed to confirm email change"));
        } else {
            setMessage("Invalid confirmation link");
        }
    }, []);

    return (
        <div className="max-w-sm mx-auto p-4 text-center">
            <h2 className="text-xl font-bold">{message}</h2>
        </div>
    );
}
