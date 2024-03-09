namespace AntSK.Domain.Utils
{
    public class PasswordUtil
    {
        public static string HashPassword(string password)
        {
            // 默认的工作因子是10，可以根据你的需求调整以增加散列的复杂度
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // 验证密码是否匹配散列值
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
