namespace IdentityAndJWT;

public static class Schemas
{

    public const string Identity = "Identity";       // Tài khoản, vai trò, phân quyền
    public const string Patient = "Patient";         // Hồ sơ bệnh nhân
    public const string Appointment = "Appointment"; // Quản lý lịch hẹn
    public const string Medical = "Medical";         // Thông tin khám, bệnh án
    public const string Staff = "Staff";             // Nhân viên y tế (bác sĩ, y tá)
    public const string Pharmacy = "Pharmacy";       // Kho thuốc, đơn thuốc
    public const string Billing = "Billing";         // Thanh toán, bảo hiểm
    public const string Default = "dbo";          // Cho bảng chung hoặc chưa phân loại

}
