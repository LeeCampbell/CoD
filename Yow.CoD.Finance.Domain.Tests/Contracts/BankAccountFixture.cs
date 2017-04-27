using System;
using NUnit.Framework;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.Domain.Tests.Contracts
{
    [TestFixture]
    public sealed class BankAccountFixture
    {
        [TestCase(null)]
        [TestCase("")]
        public void RequiresBsb(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new BankAccount(value, "12345678"));
            Assert.AreEqual("BSB is required\r\nParameter name: bsb", ex.Message);
        }
        [TestCase("Abc-123")]
        [TestCase("Abc123")]
        [TestCase("00600")]
        [TestCase("066-00")]
        [TestCase("066-0000")]
        [TestCase("0066-000")]
        public void RejectsInvalidBsb(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new BankAccount(value, "12345678"));
            Assert.AreEqual("BSB is not valid\r\nParameter name: bsb", ex.Message);
        }
        [TestCase("066000", "066-000")]
        [TestCase("123456", "123-456")]
        [TestCase("066-000", "066-000")]
        [TestCase("123-456", "123-456")]
        public void AcceptsValidBsb(string value, string expected)
        {
            var actual = new BankAccount(value, "12345678");
            Assert.AreEqual(expected, actual.Bsb);
        }
        
        [TestCase(null)]
        [TestCase("")]
        public void RequiresAccountNumber(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new BankAccount("066-000", value));
            Assert.AreEqual("Account number is required\r\nParameter name: accountNumber", ex.Message);
        }
        [TestCase("1234567890123")]//Too long
        [TestCase(" 123")]//Spaces
        [TestCase("1")]//Minimum of 3
        [TestCase("12")]//Minimum of 3
        [TestCase("123a")]
        [TestCase("a123")]
        public void RejectsInvalidAccountNumber(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new BankAccount("066-000", value));
            Assert.AreEqual("Account number is not valid\r\nParameter name: accountNumber", ex.Message);
        }

        [TestCase("123")]
        [TestCase("123456")]
        [TestCase("123456789012")]
        public void AcceptsValidAccountNumber(string value)
        {
            var actual = new BankAccount("066-000", value);
            Assert.AreEqual(value, actual.AccountNumber);
        }


    }
}