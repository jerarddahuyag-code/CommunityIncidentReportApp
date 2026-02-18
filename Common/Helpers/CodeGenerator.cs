using System.Security.Cryptography;
using System.Text;

namespace Common.Helpers;

public static class CodeGenerator
{
    private const int length = 20;
    public static string GetRandomString(string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890")
    {
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            int index = RandomNumberGenerator.GetInt32(alphabet.Length);
            sb.Append(alphabet[index]);
        }
        return sb.ToString();
    }
}