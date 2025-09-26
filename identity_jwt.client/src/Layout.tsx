import { Link, Outlet } from "react-router-dom";

export default function Layout() {
    return (
        <div className="min-h-screen flex flex-col">
            {/* Navbar */}
            <nav className="bg-gray-800 text-white p-4 flex gap-4">
                <Link to="/login" className="hover:underline">Login</Link>
                <Link to="/register" className="hover:underline">Register</Link>
                <Link to="/change-password" className="hover:underline">Change Password</Link>
                <Link to="/forgot-password" className="hover:underline">Forgot Password</Link>
                <Link to="/reset-password" className="hover:underline">Reset Password</Link>
                <Link to="/change-email" className="hover:underline">Change Email</Link>
                <Link to="/confirm-change-email" className="hover:underline">Confirm Email</Link>
            </nav>

            {/* Page content */}
            <main className="flex-1 p-6">
                <Outlet />
            </main>
        </div>
    );
}
