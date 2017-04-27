using System;
using System.Collections.Generic;
using NUnit.Framework;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.Domain.Tests
{
    public sealed class TakingPayment : Specification<Loan, TakePaymentCommand>
    {
        private readonly DateTimeOffset _createdDate = new DateTimeOffset(2017, 06, 05, 04, 30, 20, TimeSpan.FromHours(10));
        private readonly DateTimeOffset _transactionDate = new DateTimeOffset(2017, 06, 12, 10, 00, 00, TimeSpan.FromHours(10));
        protected override IEnumerable<Event> Given()
        {
            yield return new LoanCreatedEvent(_createdDate, 2000, new Duration(12, DurationUnit.Month), PaymentPlan.Weekly);
            yield return new LoanCustomerContactChangedEvent("bob", "0444444444", "0812341234", "10 Random Street");
            yield return new LoanBankAccountChangedEvent("066-000", "12345678");
        }

        protected override TakePaymentCommand When()
        {
            return new TakePaymentCommand(Guid.NewGuid(), Sut.Id, 123.45m, _transactionDate);
        }

        protected override IHandler<TakePaymentCommand> CreateHandler()
        {
            return new TakePaymentCommandHandler(Repository);
        }

        [Test]
        public void PaymentTakenEventRaised()
        {
            var actual = (PaymentTakenEvent)Produced[0];
            Assert.That(actual.TransactionDate, Is.EqualTo(_transactionDate));
            Assert.That(actual.Amount, Is.EqualTo(123.45m));
        }
    }
    
    //Validate command 
    //  negative amount
    //  old tx date
    

    //public sealed class LoanSettlement : Specification<Loan, TakePaymentCommand>
    //{
    //}

    //public sealed class OverPayment : Specification<Loan, TakePaymentCommand>
    //{
    //    private readonly DateTimeOffset _createdDate = new DateTimeOffset(2017, 06, 05, 04, 30, 20, TimeSpan.FromHours(10));
    //    private readonly DateTimeOffset _transactionDate = new DateTimeOffset(2017, 06, 12, 10, 00, 00, TimeSpan.FromHours(10));
    //    protected override IEnumerable<Event> Given()
    //    {
    //        yield return new LoanCreatedEvent(_createdDate, 2000, new Duration(12, DurationUnit.Month), PaymentPlan.Weekly);
    //        yield return new LoanCustomerContactChangedEvent("bob", "0444444444", "0812341234", "10 Random Street");
    //        yield return new LoanBankAccountChangedEvent("066-000", "12345678");
    //    }

    //    protected override TakePaymentCommand When()
    //    {
    //        return new TakePaymentCommand(Guid.NewGuid(), 2100m, _transactionDate);
    //    }

    //    protected override IHandler<TakePaymentCommand> CreateHandler()
    //    {
    //        return new TakePaymentCommandHandler(Repository);
    //    }

    //    [Test]
    //    public void PaymentTakenEventRaised()
    //    {
    //        var actual = (PaymentTakenEvent)Produced[0];
    //        Assert.That(actual.TransactionDate, Is.EqualTo(_transactionDate));
    //        Assert.That(actual.Amount, Is.EqualTo(_command.Amount));
    //    }
    //}
}