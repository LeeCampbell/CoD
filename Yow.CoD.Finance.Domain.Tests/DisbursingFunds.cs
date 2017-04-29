using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.Domain.Tests
{
    public sealed class DisbursingFunds : Specification<Loan, DisburseLoanFundsCommand, Receipt>
    {
        private readonly DateTimeOffset CreatedDate = new DateTimeOffset(2017, 06, 05, 04, 30, 20, TimeSpan.FromHours(10));
        private readonly decimal LoanAmount = 2000m;
        private DateTimeOffset _transactionDate;

        protected override IEnumerable<Event> Given()
        {
            yield return new LoanCreatedEvent(CreatedDate, LoanAmount, new Duration(12, DurationUnit.Month), PaymentPlan.Weekly);
            yield return new LoanCustomerContactChangedEvent("bob", "0444444444", "0812341234", "10 Random Street");
            yield return new LoanBankAccountChangedEvent("066-000", "12345678");
        }

        protected override DisburseLoanFundsCommand When()
        {
            _transactionDate = CreatedDate.AddHours(1);
            return new DisburseLoanFundsCommand(Guid.NewGuid(), Sut.Id, _transactionDate);
        }

        protected override IHandler<DisburseLoanFundsCommand, Receipt> CreateHandler()
        {
            return new DisburseLoanFundsCommandHandler(Repository);
        }

        [Test]
        public void LoanDisbursedFundsEventRaised()
        {
            var actual = (LoanDisbursedFundsEvent)Produced.Single();
            Assert.That(actual.TransactionDate, Is.EqualTo(_transactionDate));
            Assert.That(actual.Amount, Is.EqualTo(-LoanAmount));
        }
    }

    public sealed class DisbursingFundsMultipleTimes : Specification<Loan, DisburseLoanFundsCommand, Receipt>
    {
        private readonly DateTimeOffset _createdDate = new DateTimeOffset(2017, 06, 05, 04, 30, 20, TimeSpan.FromHours(10));

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

        [Test]
        public void Throws()
        {
            Assert.That(Caught, Is.InstanceOf<InvalidOperationException>()
               .And.Message.EqualTo("Funds are already disbursed."));
        }
    }
}