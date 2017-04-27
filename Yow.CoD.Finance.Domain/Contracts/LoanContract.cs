using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class LoanContract
    {
        public DateTime SignedOn { get; }
        public DateTime ApprovedOn { get; }
        public string ApprovedBy { get; }
        public Uri DocumentId { get; }
        public PaymentPlan PaymentPlan { get; }
        public FeeModel FeeModel { get; }
    }
}