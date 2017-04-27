using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.Domain.Tests
{
    public abstract class TakingPaymentBase : Specification<Loan, TakePaymentCommand>
    {
        protected readonly DateTimeOffset CreatedDate = new DateTimeOffset(2017, 06, 05, 04, 30, 20, TimeSpan.FromHours(10));
        protected readonly decimal LoanAmount = 2000m;
        
        protected override IEnumerable<Event> Given()
        {
            yield return new LoanCreatedEvent(CreatedDate, LoanAmount, new Duration(12, DurationUnit.Month), PaymentPlan.Weekly);
            yield return new LoanCustomerContactChangedEvent("bob", "0444444444", "0812341234", "10 Random Street");
            yield return new LoanBankAccountChangedEvent("066-000", "12345678");
            yield return new LoanDisbursedFundsEvent(CreatedDate.AddHours(1), -LoanAmount, new BankAccount("066-000", "12345678") );
        }

        protected override IHandler<TakePaymentCommand> CreateHandler()
        {
            return new TakePaymentCommandHandler(Repository);
        }
    }

    public sealed class TakingPayment : TakingPaymentBase
    {
        private readonly DateTimeOffset _transactionDate = new DateTimeOffset(2017, 06, 12, 10, 00, 00, TimeSpan.FromHours(10));

        protected override TakePaymentCommand When()
        {
            return new TakePaymentCommand(Guid.NewGuid(), Sut.Id, 123.45m, _transactionDate);
        }
        
        [Test]
        public void PaymentTakenEventRaised()
        {
            var actual = (PaymentTakenEvent)Produced[0];
            Assert.That(actual.TransactionDate, Is.EqualTo(_transactionDate));
            Assert.That(actual.Amount, Is.EqualTo(123.45m));
        }
    }

    public sealed class TakePaymentPriorToCreation : TakingPaymentBase
    {
        protected override TakePaymentCommand When()
        {
            return new TakePaymentCommand(Guid.NewGuid(), Sut.Id, 123.45m, CreatedDate.AddDays(-1));
        }

        [Test]
        public void ThrowsInvalidArgEx()
        {
            Assert.That(Caught, Is.InstanceOf<ArgumentException>()
                .And.Message.EqualTo("Transaction date can not be prior to loan creation\r\nParameter name: command"));
        }
    }
    public sealed class TakePaymentForZeroAmount : TakingPaymentBase
    {
        protected override TakePaymentCommand When()
        {
            return new TakePaymentCommand(Guid.NewGuid(), Sut.Id, 0m, CreatedDate.AddDays(1));
        }

        [Test]
        public void ThrowsInvalidArgEx()
        {
            Assert.That(Caught, Is.InstanceOf<ArgumentException>()
                .And.Message.EqualTo("Transaction amount must be positive\r\nParameter name: command"));
        }
    }
    public sealed class TakePaymentForNegativeAmount : TakingPaymentBase
    {
        protected override TakePaymentCommand When()
        {
            return new TakePaymentCommand(Guid.NewGuid(), Sut.Id, -1m, CreatedDate.AddDays(1));
        }

        [Test]
        public void ThrowsInvalidArgEx()
        {
            Assert.That(Caught, Is.InstanceOf<ArgumentException>()
                .And.Message.EqualTo("Transaction amount must be positive\r\nParameter name: command"));
        }
    }

    public sealed class LoanSettlement : TakingPaymentBase
    {
        private readonly DateTimeOffset _transactionDate = new DateTimeOffset(2017, 06, 20, 10, 00, 00, TimeSpan.FromHours(10));
        protected override TakePaymentCommand When()
        {
            return new TakePaymentCommand(Guid.NewGuid(), Sut.Id, LoanAmount, _transactionDate);
        }

        [Test]
        public void LoanSettledEventRaised()
        {
            var actual = Produced.OfType<LoanSettledEvent>().Single();
            Assert.That(actual.TransactionDate, Is.EqualTo(_transactionDate));
        }
    }

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