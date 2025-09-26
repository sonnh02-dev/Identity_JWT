import { useState } from "react";

export default function Register() {
    const [form, setForm] = useState({
        email: "",
        password: "",
        confirmPassword: ""
    });
    const [message, setMessage] = useState("");

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleRegister = async(e :React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setMessage("");

        const response = await fetch("/api/authentication/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(form)
        });

        if (response.ok) {
            setMessage("Register success. Please check your email.");
        } else {
            const err = await response.text();
            setMessage(err);
        }
    };

    return (
        <div className="flex items-center justify-center min-h-screen bg-gray-100">
            <form
                onSubmit={handleRegister}
                className="bg-white p-6 rounded-2xl shadow-md w-96"
            >
                <h1 className="text-2xl font-bold mb-4">Register</h1>
                {message && <p className="text-red-500 mb-2">{message}</p>}

                <input
                    type="email"
                    name="email"
                    placeholder="Email"
                    className="w-full p-2 mb-3 border rounded-lg"
                    value={form.email}
                    onChange={handleChange}
                />

                <input
                    type="password"
                    name="password"
                    placeholder="Password"
                    className="w-full p-2 mb-3 border rounded-lg"
                    value={form.password}
                    onChange={handleChange}
                />

                <input
                    type="password"
                    name="confirmPassword"
                    placeholder="Confirm Password"
                    className="w-full p-2 mb-3 border rounded-lg"
                    value={form.confirmPassword}
                    onChange={handleChange}
                />

                <button
                    type="submit"
                    className="w-full bg-green-600 text-white py-2 rounded-lg hover:bg-green-700"
                >
                    Register
                </button>
            </form>
        </div>
    );
}
