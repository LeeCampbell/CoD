using System;
using System.Text.RegularExpressions;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class CustomerContact
    {
        private static readonly Regex PhoneNumberRegex = new Regex("^0\\d{9}$", RegexOptions.Compiled);

        public CustomerContact(string name, string preferredPhoneNumber, string alternatePhoneNumber, string postalAddress)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));
            if (string.IsNullOrWhiteSpace(preferredPhoneNumber))
                throw new ArgumentException("Phone number is required", nameof(preferredPhoneNumber));
            if (string.IsNullOrWhiteSpace(postalAddress))
                throw new ArgumentException("Postal address is required", nameof(postalAddress));

            if(!IsValidPhoneNumber(preferredPhoneNumber))
                throw new ArgumentException("Phone number is not valid", nameof(preferredPhoneNumber));
            if (alternatePhoneNumber!=null && !IsValidPhoneNumber(alternatePhoneNumber))
                throw new ArgumentException("Phone number is not valid", nameof(alternatePhoneNumber));

            Name = name;
            PreferredPhoneNumber = preferredPhoneNumber;
            AlternatePhoneNumber = alternatePhoneNumber;
            PostalAddress = postalAddress;
        }

        public string Name { get; }
        public string PreferredPhoneNumber { get; }
        public string AlternatePhoneNumber { get; }
        public string PostalAddress { get; }

        private static bool IsValidPhoneNumber(string preferredPhoneNumber)
        {
            return PhoneNumberRegex.IsMatch(preferredPhoneNumber);
        }
    }
}