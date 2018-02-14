using System;
using System.Collections.Generic;
using Xunit;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.Domain.Tests
{
    public abstract class TakingPaymentBase : Specification<Loan, TakePaymentCommand, TransactionReceipt>
    {
        protected DateTimeOffset CreatedDate { get; }
        protected decimal LoanAmount { get; }
        protected DateTimeOffset TransactionDate { get; }
        protected decimal TransactionAmount { get; }


        public TakingPaymentBase(DateTimeOffset createdDate,
            decimal loanAmount,
            DateTimeOffset transactionDate,
            decimal transactionAmount)
        {
            CreatedDate = createdDate;
            LoanAmount = loanAmount;
            TransactionDate = transactionDate;
            TransactionAmount = transactionAmount;
            Execute();
        }

        protected override IEnumerable<Event> Given()
        {
            yield return new LoanCreatedEvent(CreatedDate, LoanAmount, new Duration(12, DurationUnit.Month), PaymentPlan.Weekly);
            yield return new LoanCustomerContactChangedEvent("bob", "0444444444", "0812341234", "10 Random Street");
            yield return new LoanBankAccountChangedEvent("066-000", "12345678");
            yield return new LoanDisbursedFundsEvent(CreatedDate.AddHours(1), -LoanAmount, new BankAccount("066-000", "12345678"));
        }

        protected override TakePaymentCommand When()
        {
            return new TakePaymentCommand(Guid.NewGuid(), Sut.Id, TransactionAmount, TransactionDate);
        }

        protected override IHandler<TakePaymentCommand, TransactionReceipt> CreateHandler()
        {
            return new TakePaymentCommandHandler(Repository);
        }
    }

    public sealed class TakePaymentPriorToCreation : TakingPaymentBase
    {
        public TakePaymentPriorToCreation() : base(
            createdDate: new DateTime(2017, 06, 05),
            loanAmount: 2000m,
            transactionDate: new DateTime(2017, 06, 04),
            transactionAmount: 123.45m)
        {
        }

        [Fact]
        public void ThrowsInvalidArgEx()
        {
            Assert.IsAssignableFrom<ArgumentException>(Caught);
            Assert.Equal("Transaction date can not be prior to loan creation\r\nParameter name: command", Caught.Message);
        }
    }

    public sealed class TakePaymentForZeroAmount : TakingPaymentBase
    {
        public TakePaymentForZeroAmount() : base(
            createdDate: new DateTime(2000, 01, 01),
            loanAmount: 2000m,
            transactionDate: new DateTime(2000, 01, 02),
            transactionAmount: 0.00m)
        {
        }

        [Fact]
        public void ThrowsInvalidArgEx()
        {
            Assert.IsAssignableFrom<ArgumentException>(Caught);
            Assert.Equal("Transaction amount must be positive\r\nParameter name: command", Caught.Message);
        }
    }

    public sealed class TakePaymentForNegativeAmount : TakingPaymentBase
    {
        public TakePaymentForNegativeAmount() : base(
            createdDate: new DateTime(2000, 01, 01),
            loanAmount: 2000m,
            transactionDate: new DateTime(2000, 01, 02),
            transactionAmount: -1.00m)
        {
        }

        [Fact]
        public void ThrowsInvalidArgEx()
        {
            Assert.IsAssignableFrom<ArgumentException>(Caught);
            Assert.Equal("Transaction amount must be positive\r\nParameter name: command", Caught.Message);
        }
    }

    public sealed class TakingSuccessfulPayment : TakingPaymentBase
    {
        public TakingSuccessfulPayment() : base(
            createdDate: new DateTimeOffset(2017, 06, 05, 04, 30, 20, TimeSpan.FromHours(10)),
            loanAmount: 2000m,
            transactionDate: new DateTimeOffset(2017, 06, 12, 10, 00, 00, TimeSpan.FromHours(10)),
            transactionAmount: 123.45m)
        {
        }

        [Fact]
        public void PaymentTakenEventRaised()
        {
            var actual = (PaymentTakenEvent) Produced[0];
            Assert.Equal(TransactionDate, actual.TransactionDate);
            Assert.Equal(TransactionAmount, actual.Amount);
        }
    }

    public sealed class LoanSettlement : TakingPaymentBase
    {
        public LoanSettlement() : base(
            createdDate: new DateTime(2000, 01, 01),
            loanAmount: 2000m,
            transactionDate: new DateTime(2000, 01, 02),
            transactionAmount: 2000.00m)
        {
        }

        [Fact]
        public void PaymentTakenEventRaised()
        {
            var actual = (PaymentTakenEvent) Produced[0];
            Assert.Equal(TransactionDate, actual.TransactionDate);
            Assert.Equal(TransactionAmount, actual.Amount);
        }

        [Fact]
        public void LoanSettledEventRaised()
        {
            var actual = (LoanSettledEvent) Produced[1];
            Assert.Equal(TransactionDate, actual.TransactionDate);
        }
    }
    
    public sealed class LoanOverPayment : TakingPaymentBase
    {
        public LoanOverPayment() : base(
            createdDate: new DateTime(2000, 01, 01),
            loanAmount: 2000m,
            transactionDate: new DateTime(2000, 01, 02),
            transactionAmount: 2001.00m)
        {
        }

        [Fact]
        public void PaymentTakenEventRaised()
        {
            var actual = (PaymentTakenEvent) Produced[0];
            Assert.Equal(TransactionDate, actual.TransactionDate);
            Assert.Equal(TransactionAmount, actual.Amount);
        }

        [Fact]
        public void LoanSettledEventRaised()
        {
            var actual = (LoanSettledEvent) Produced[1];
            Assert.Equal(TransactionDate, actual.TransactionDate);
        }
        [Fact]
        public void LoanOverPaidEventRaised()
        {
            var actual = (LoanOverPaidEvent) Produced[2];
            Assert.Equal(TransactionDate, actual.TransactionDate);
            Assert.Equal(TransactionAmount - LoanAmount, actual.Amount);
        }
    }
}