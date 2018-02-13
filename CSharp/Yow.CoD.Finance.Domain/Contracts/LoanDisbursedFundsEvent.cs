using System;

namespace Yow.CoD.Finance.Domain.Contracts
{
    public sealed class LoanDisbursedFundsEvent : Event
    {
        public LoanDisbursedFundsEvent(DateTimeOffset transactionDate, decimal amount, BankAccount disbursedTo)
        {
            TransactionDate = transactionDate;
            Amount = amount;
            DisbursedTo = disbursedTo;
        }

        public DateTimeOffset TransactionDate { get; }        
        public decimal Amount { get; }
        public BankAccount DisbursedTo { get; }
    }
}