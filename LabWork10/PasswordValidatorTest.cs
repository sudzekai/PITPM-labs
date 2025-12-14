using LabWork10_Library;

namespace LabWork10
{
    public class PasswordValidatorTest
    {
        [Theory]
        [InlineData("password1")]
        [InlineData("омфгхеллоb1")]
        public void IsValidPassword_Good(string password)
            => Assert.True(password.IsValidPassword());


        [Theory]
        [InlineData("abc")]
        [InlineData("омфгхелло")]
        [InlineData("омфгхелло123")]
        [InlineData("омфгхеллоb")]
        [InlineData("")]
        [InlineData(null)]
        public void IsValidPassword_Exception(string password)
            => Assert.False(password.IsValidPassword());
    }
}
