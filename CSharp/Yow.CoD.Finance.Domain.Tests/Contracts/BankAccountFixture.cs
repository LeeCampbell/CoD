using System;
using Xunit;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.Domain.Tests.Contracts
{
    public sealed class BankAccountFixture
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RequiresBsb(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new BankAccount(value, "12345678"));
            Assert.Equal("BSB is required (Parameter 'bsb')", ex.Message);
        }
        [Theory]
        [InlineData("Abc-123")]
        [InlineData("Abc123")]
        [InlineData("00600")]
        [InlineData("066-00")]
        [InlineData("066-0000")]
        [InlineData("0066-000")]
        public void RejectsInvalidBsb(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new BankAccount(value, "12345678"));
            Assert.Equal("BSB is not valid (Parameter 'bsb')", ex.Message);
        }
        [Theory]
        [InlineData("066000", "066-000")]
        [InlineData("123456", "123-456")]
        [InlineData("066-000", "066-000")]
        [InlineData("123-456", "123-456")]
        public void AcceptsValidBsb(string value, string expected)
        {
            var actual = new BankAccount(value, "12345678");
            Assert.Equal(expected, actual.Bsb);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RequiresAccountNumber(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new BankAccount("066-000", value));
            Assert.Equal("Account number is required (Parameter 'accountNumber')", ex.Message);
        }
        [Theory]
        [InlineData("1234567890123")]//Too long
        [InlineData(" 123")]//Spaces
        [InlineData("1")]//Minimum of 3
        [InlineData("12")]//Minimum of 3
        [InlineData("123a")]
        [InlineData("a123")]
        public void RejectsInvalidAccountNumber(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new BankAccount("066-000", value));
            Assert.Equal("Account number is not valid (Parameter 'accountNumber')", ex.Message);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("123456")]
        [InlineData("123456789012")]
        public void AcceptsValidAccountNumber(string value)
        {
            var actual = new BankAccount("066-000", value);
            Assert.Equal(value, actual.AccountNumber);
        }
    }
}