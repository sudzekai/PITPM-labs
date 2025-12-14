
using System.Text.RegularExpressions;

namespace LabWork10_Library
{
    public static partial class PasswordValidator
    {
        [GeneratedRegex(@"^(?=.*\d)(?=.*[a-zA-Z]).{8,}$")]
        private static partial Regex PasswordRegex();

        public static bool IsValidPassword(this string password)
            => string.IsNullOrEmpty(password) ? false : PasswordRegex().IsMatch(password);
    }
}
