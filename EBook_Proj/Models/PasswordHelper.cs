using System.Security.Cryptography;
using System.Text;

public static class PasswordHelper
{
    public static string HashWithSha256(string input)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes); // since .NET 5; if using older version use BitConverter.ToString().Replace("-", "")
        }
    }
}