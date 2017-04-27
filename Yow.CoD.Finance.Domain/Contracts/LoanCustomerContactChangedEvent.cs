namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class LoanCustomerContactChangedEvent : Event
    {
        public LoanCustomerContactChangedEvent(string customerContactName, string customerContactPreferredPhoneNumber, string customerContactAlternatePhoneNumber, string customerContactPostalAddress)
        {
            CustomerContact = new CustomerContact(customerContactName, customerContactPreferredPhoneNumber, customerContactAlternatePhoneNumber, customerContactPostalAddress);
        }

        public CustomerContact CustomerContact { get; }
    }
}