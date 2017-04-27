namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class CustomerContact
    {
        public CustomerContact(string name, string preferredPhoneNumber, string alternatePhoneNumber, string postalAddress)
        {
            Name = name;
            PreferredPhoneNumber = preferredPhoneNumber;
            AlternatePhoneNumber = alternatePhoneNumber;
            PostalAddress = postalAddress;
        }

        public string Name { get; }
        public string PreferredPhoneNumber { get; }
        public string AlternatePhoneNumber { get; }
        public string PostalAddress { get; }
    }
}