using System;

namespace Yow.CoD.Finance.Domain.Model
{
    internal sealed class CustomerContact
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

    internal sealed class LoanContract
    {
        public DateTime SignedOn { get; }
        public DateTime ApprovedOn { get; }
        public string ApprovedBy { get; }
        public Uri DocumentId { get; }
        public PaymentPlan PaymentPlan { get; }
        public IFeeModel FeeModel { get; }
    }

    internal sealed class PaymentPlan
    {
        //Weekly
    }
    internal interface IFeeModel
    {
        //Interest
    }
}