using System;
using NUnit.Framework;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.Domain.Tests.Contracts
{
    [TestFixture]
    public sealed class CustomerContactFixture
    {
        [TestCase(null)]
        [TestCase("")]
        public void RequiresCustomerName(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new CustomerContact(value, "0412341234", "0856785678", "10 St Georges Terrace, Perth, WA 6000"));
            Assert.AreEqual("Name is required\r\nParameter name: name", ex.Message);
        }

        [TestCase(null)]
        [TestCase("")]
        public void RequiresPreferredPhoneNumber(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new CustomerContact("Jane Doe", value, "0856785678", "10 St Georges Terrace, Perth, WA 6000"));
            Assert.AreEqual("Phone number is required\r\nParameter name: preferredPhoneNumber", ex.Message);
        }

        [TestCase("0")]
        [TestCase("1234")]
        [TestCase("04123456")]
        [TestCase("04123456789012")]//Too long
        public void RejectsInvalidFormatPreferredPhoneNumber(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new CustomerContact("Jane Doe", value, "0856785678", "10 St Georges Terrace, Perth, WA 6000"));
            Assert.AreEqual("Phone number is not valid\r\nParameter name: preferredPhoneNumber", ex.Message);
        }
        [TestCase("0444444444")]
        [TestCase("0412345678")]
        [TestCase("0212345678")]
        [TestCase("0812345678")]
        public void AllowsValidFormatPreferredPhoneNumber(string expected)
        {
            var actual = new CustomerContact("Jane Doe", expected, "0856785678", "10 St Georges Terrace, Perth, WA 6000");
            Assert.AreEqual(expected, actual.PreferredPhoneNumber);
        }

        [Test]
        public void AllowsNullAlternatePhoneNumber()
        {
            var actual = new CustomerContact("Jane Doe", "0412341234", null, "10 St Georges Terrace, Perth, WA 6000");
            Assert.IsNull(actual.AlternatePhoneNumber);
        }

        [TestCase("0444444444")]
        [TestCase("0412345678")]
        [TestCase("0212345678")]
        [TestCase("0812345678")]
        public void AllowsValidFormatAlternatePhoneNumber(string expected)
        {
            var actual = new CustomerContact("Jane Doe", "0412341234", expected, "10 St Georges Terrace, Perth, WA 6000");
            Assert.AreEqual(expected, actual.AlternatePhoneNumber);
        }

        [TestCase(null)]
        [TestCase("")]
        public void RequiresPostalAddress(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new CustomerContact("Jane Doe", "0412341234", "0856785678", value));
            Assert.AreEqual("Postal address is required\r\nParameter name: postalAddress", ex.Message);
        }
    }
}
