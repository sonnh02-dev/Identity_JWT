import { BrowserRouter, Routes, Route } from "react-router-dom";
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import ChangePasswordForm from "./pages/auth/ChangePassword";
import RequestPasswordResetForm from "./pages/auth/RequestPasswordReset";
import ResetPasswordForm from "./pages/auth/ResetPassword";
import RequestChangeEmailForm from "./pages/auth/RequestChangeEmail";
import ConfirmChangeEmailPage from "./pages/auth/ConfirmChangeEmail";

export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                {/* Auth */}
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />

                {/* Password */}
                <Route path="/change-password" element={<ChangePasswordForm />} />
                <Route path="/forgot-password" element={<RequestPasswordResetForm />} />
                <Route path="/reset-password" element={<ResetPasswordForm />} />

                {/* Email */}
                <Route path="/change-email" element={<RequestChangeEmailForm />} />
                <Route path="/confirm-change-email" element={<ConfirmChangeEmailPage />} />

                {/* Default route */}
                <Route path="*" element={<div className="p-4 text-center">404 - Page Not Found</div>} />
            </Routes>
        </BrowserRouter>
    );
}
