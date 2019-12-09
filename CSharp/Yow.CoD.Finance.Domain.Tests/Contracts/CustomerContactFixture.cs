using System;
using Xunit;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.Domain.Tests.Contracts
{
    public sealed class CustomerContactFixture
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RequiresCustomerName(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new CustomerContact(value, "0412341234", "0856785678", "10 St Georges Terrace, Perth, WA 6000"));
            Assert.Equal("Name is required (Parameter 'name')", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RequiresPreferredPhoneNumber(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new CustomerContact("Jane Doe", value, "0856785678", "10 St Georges Terrace, Perth, WA 6000"));
            Assert.Equal("Phone number is required (Parameter 'preferredPhoneNumber')", ex.Message);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("1234")]
        [InlineData("04123456")]
        [InlineData("04123456789012")]//Too long
        public void RejectsInvalidFormatPreferredPhoneNumber(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new CustomerContact("Jane Doe", value, "0856785678", "10 St Georges Terrace, Perth, WA 6000"));
            Assert.Equal("Phone number is not valid (Parameter 'preferredPhoneNumber')", ex.Message);
        }
        [Theory]
        [InlineData("0444444444")]
        [InlineData("0412345678")]
        [InlineData("0212345678")]
        [InlineData("0812345678")]
        public void AllowsValidFormatPreferredPhoneNumber(string expected)
        {
            var actual = new CustomerContact("Jane Doe", expected, "0856785678", "10 St Georges Terrace, Perth, WA 6000");
            Assert.Equal(expected, actual.PreferredPhoneNumber);
        }

        [Fact]
        public void AllowsNullAlternatePhoneNumber()
        {
            var actual = new CustomerContact("Jane Doe", "0412341234", null, "10 St Georges Terrace, Perth, WA 6000");
            Assert.Null(actual.AlternatePhoneNumber);
        }

        [Theory]
        [InlineData("0444444444")]
        [InlineData("0412345678")]
        [InlineData("0212345678")]
        [InlineData("0812345678")]
        public void AllowsValidFormatAlternatePhoneNumber(string expected)
        {
            var actual = new CustomerContact("Jane Doe", "0412341234", expected, "10 St Georges Terrace, Perth, WA 6000");
            Assert.Equal(expected, actual.AlternatePhoneNumber);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RequiresPostalAddress(string value)
        {
            var ex = Assert.Throws<ArgumentException>(() => new CustomerContact("Jane Doe", "0412341234", "0856785678", value));
            Assert.Equal("Postal address is required (Parameter 'postalAddress')", ex.Message);
        }
    }
}
