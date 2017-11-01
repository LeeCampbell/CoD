using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.Domain.Tests
{
    public sealed class DisbursingFunds : Specification<Loan, DisburseLoanFundsCommand, Receipt>
    {
        private readonly DateTimeOffset _createdDate = new DateTimeOffset(2017, 06, 05, 04, 30, 20, TimeSpan.FromHours(10));
        private readonly decimal _loanAmount = 2000m;
        private DateTimeOffset _transactionDate;

        public DisbursingFunds()
        {
            Execute();
        }

        protected override IEnumerable<Event> Given()
        {
            yield return new LoanCreatedEvent(_createdDate, _loanAmount, new Duration(12, DurationUnit.Month), PaymentPlan.Weekly);
            yield return new LoanCustomerContactChangedEvent("bob", "0444444444", "0812341234", "10 Random Street");
            yield return new LoanBankAccountChangedEvent("066-000", "12345678");
        }

        protected override DisburseLoanFundsCommand When()
        {
            _transactionDate = _createdDate.AddHours(1);
            return new DisburseLoanFundsCommand(Guid.NewGuid(), Sut.Id, _transactionDate);
        }

        protected override IHandler<DisburseLoanFundsCommand, Receipt> CreateHandler()
        {
            return new DisburseLoanFundsCommandHandler(Repository);
        }

        [Fact]
        public void LoanDisbursedFundsEventRaised()
        {
            var actual = (LoanDisbursedFundsEvent)Produced.Single();
            Assert.Equal(_transactionDate, actual.TransactionDate);
            Assert.Equal(-_loanAmount, actual.Amount);
        }
    }

    public sealed class DisbursingFundsMultipleTimes : Specification<Loan, DisburseLoanFundsCommand, Receipt>
    {
        private readonly DateTimeOffset _createdDate = new DateTimeOffset(2017, 06, 05, 04, 30, 20, TimeSpan.FromHours(10));

        public DisbursingFundsMultipleTimes()
        {
            Execute();
        }

        protected override IEnumerable<Event> Given()
        {
            var loanAmount = 2000m;
            yield return new LoanCreatedEvent(_createdDate, loanAmount, new Duration(12, DurationUnit.Month), PaymentPlan.Weekly);
            yield return new LoanCustomerContactChangedEvent("bob", "0444444444", "0812341234", "10 Random Street");
            yield return new LoanBankAccountChangedEvent("066-000", "12345678");
            yield return new LoanDisbursedFundsEvent(_createdDate.AddHours(1), loanAmount, new BankAccount("066-000", "12345678"));
        }

        protected override DisburseLoanFundsCommand When()
        {
            return new DisburseLoanFundsCommand(Guid.NewGuid(), Sut.Id, _createdDate.AddHours(1));
        }

        protected override IHandler<DisburseLoanFundsCommand, Receipt> CreateHandler()
        {
            return new DisburseLoanFundsCommandHandler(Repository);
        }

        [Fact]
        public void Throws()
        {
            Assert.IsAssignableFrom<InvalidOperationException>(Caught);
            Assert.Equal("Funds are already disbursed.", Caught.Message);
        }
    }
}