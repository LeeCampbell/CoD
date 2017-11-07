using System;
using System.Collections.Generic;
using Xunit;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.Domain.Tests
{
    public class CreatingALoan : Specification<Loan, CreateLoanCommand, Receipt>
    {
        private readonly CreateLoanCommand _command;

        public CreatingALoan()
        {
            var customerContact = new CustomerContact("Jane Doe", "0412341234", "0856785678", "10 St Georges Terrace, Perth, WA 6000");
            var bankAccount = new BankAccount("066-000", "12345678");
            _command = new CreateLoanCommand(
                commandId: Guid.NewGuid(),
                aggregateId: Guid.NewGuid(),
                createdOn: new DateTimeOffset(2001, 2, 3, 4, 5, 6, TimeSpan.Zero),
                customerContact: customerContact,
                bankAccount: bankAccount,
                paymentPlan: PaymentPlan.Weekly,
                amount: 1000,
                term: new Duration(12, DurationUnit.Month));
            Execute();
        }
        protected override IEnumerable<Event> Given()
        {
            yield break;
        }

        protected override CreateLoanCommand When()
        {
            return _command;
        }

        protected override IHandler<CreateLoanCommand, Receipt> CreateHandler()
        {
            return new CreateLoanCommandHandler(Repository);
        }

        [Fact]
        public void LoanCreatedEventRaised()
        {
            var actual = (LoanCreatedEvent)Produced[0];
            Assert.Equal(_command.CreatedOn, actual.CreatedOn);
            Assert.Equal(_command.Amount, actual.Amount);
            Assert.Equal(_command.Term, actual.Term);
            Assert.Equal(_command.PaymentPlan, actual.PaymentPlan);
        }

        [Fact]
        public void LoanCustomerContactChangedEventRaised()
        {
            var actual = (LoanCustomerContactChangedEvent)Produced[1];
            Assert.Equal(_command.CustomerContact.Name, actual.CustomerContact.Name);
            Assert.Equal(_command.CustomerContact.PreferredPhoneNumber, actual.CustomerContact.PreferredPhoneNumber);
            Assert.Equal(_command.CustomerContact.AlternatePhoneNumber, actual.CustomerContact.AlternatePhoneNumber);
            Assert.Equal(_command.CustomerContact.PostalAddress, actual.CustomerContact.PostalAddress);
        }

        [Fact]
        public void LoanBankAccountChangedEventRaised()
        {
            var actual = (LoanBankAccountChangedEvent)Produced[2];
            Assert.Equal(_command.BankAccount.Bsb, actual.BankAccount.Bsb);
            Assert.Equal(_command.BankAccount.AccountNumber, actual.BankAccount.AccountNumber);
        }
    }

    public class CreatingALoanMutlipleTimes : Specification<Loan, CreateLoanCommand, Receipt>
    {
        public CreatingALoanMutlipleTimes()
        {
            Execute();
        }

        protected override IEnumerable<Event> Given()
        {
            yield return new LoanCreatedEvent(new DateTime(2000, 01, 01), 2000m, new Duration(12, DurationUnit.Month), PaymentPlan.Weekly);
        }

        protected override CreateLoanCommand When()
        {
            var customerContact = new CustomerContact("Jane Doe", "0412341234", "0856785678", "10 St Georges Terrace, Perth, WA 6000");
            var bankAccount = new BankAccount("066-000", "12345678");
            return new CreateLoanCommand(
                commandId: Guid.NewGuid(),
                aggregateId: Sut.Id,
                createdOn: new DateTimeOffset(2001, 2, 3, 4, 5, 6, TimeSpan.Zero),
                customerContact: customerContact,
                bankAccount: bankAccount,
                paymentPlan: PaymentPlan.Weekly,
                amount: 1000,
                term: new Duration(12, DurationUnit.Month));
        }

        protected override IHandler<CreateLoanCommand, Receipt> CreateHandler()
        {
            return new CreateLoanCommandHandler(Repository);
        }

        [Fact]
        public void Throws()
        {
            Assert.IsAssignableFrom<InvalidOperationException>(Caught);
            Assert.Equal("Loan already created.", Caught.Message);
        }
    }

    public abstract class CreatingALoanWithInvalidAmounts : Specification<Loan, CreateLoanCommand, Receipt>
    {
        private readonly decimal _amount;
        private readonly string _expectedError;

        public CreatingALoanWithInvalidAmounts(decimal amount, string expectedError)
        {
            _amount = amount;
            _expectedError = expectedError;
            Execute();
        }

        protected override IEnumerable<Event> Given()
        {
            yield break;
        }

        protected override CreateLoanCommand When()
        {
            var customerContact = new CustomerContact("Jane Doe", "0412341234", "0856785678", "10 St Georges Terrace, Perth, WA 6000");
            var bankAccount = new BankAccount("066-000", "12345678");
            return new CreateLoanCommand(
                commandId: Guid.NewGuid(),
                aggregateId: Sut.Id,
                createdOn: new DateTimeOffset(2001, 2, 3, 4, 5, 6, TimeSpan.Zero),
                customerContact: customerContact,
                bankAccount: bankAccount,
                paymentPlan: PaymentPlan.Weekly,
                amount: _amount,
                term: new Duration(12, DurationUnit.Month));
        }

        protected override IHandler<CreateLoanCommand, Receipt> CreateHandler()
        {
            return new CreateLoanCommandHandler(Repository);
        }

        [Fact]
        public void Throws()
        {
            Assert.IsAssignableFrom<InvalidOperationException>(Caught);
            Assert.Equal(_expectedError, Caught.Message);
        }
    }


    public sealed class CreatingALoanOver2000Dollars : CreatingALoanWithInvalidAmounts
    {
        public CreatingALoanOver2000Dollars()
            : base(2001, "Only loan amounts between $50.00 and $2000.00 are supported.")
        {
        }
    }
    public sealed class CreatingALoanUnder50Dollars : CreatingALoanWithInvalidAmounts
    {
        public CreatingALoanUnder50Dollars()
            : base(49, "Only loan amounts between $50.00 and $2000.00 are supported.")
        {
        }
    }

    public abstract class CreatingALoanWithInvalidTerm : Specification<Loan, CreateLoanCommand, Receipt>
    {
        private readonly Duration _term;
        private readonly string _expectedError;

        public CreatingALoanWithInvalidTerm(Duration term, string expectedError)
        {
            _term = term;
            _expectedError = expectedError;
            Execute();
        }

        protected override IEnumerable<Event> Given()
        {
            yield break;
        }

        protected override CreateLoanCommand When()
        {
            var customerContact = new CustomerContact("Jane Doe", "0412341234", "0856785678", "10 St Georges Terrace, Perth, WA 6000");
            var bankAccount = new BankAccount("066-000", "12345678");
            return new CreateLoanCommand(
                commandId: Guid.NewGuid(),
                aggregateId: Sut.Id,
                createdOn: new DateTimeOffset(2001, 2, 3, 4, 5, 6, TimeSpan.Zero),
                customerContact: customerContact,
                bankAccount: bankAccount,
                paymentPlan: PaymentPlan.Weekly,
                amount: 1500,
                term: _term);
        }

        protected override IHandler<CreateLoanCommand, Receipt> CreateHandler()
        {
            return new CreateLoanCommandHandler(Repository);
        }

        [Fact]
        public void Throws()
        {
            Assert.IsAssignableFrom<InvalidOperationException>(Caught);
            Assert.Equal(_expectedError, Caught.Message);
        }
    }

    public sealed class CreatingALoanOver24Months : CreatingALoanWithInvalidTerm
    {
        public CreatingALoanOver24Months()
            : base(new Duration(25, DurationUnit.Month), "Only loan terms up to 2 years are supported.")
        {
        }
    }
    public sealed class CreatingALoanOver104Weeks : CreatingALoanWithInvalidTerm
    {
        public CreatingALoanOver104Weeks()
            : base(new Duration(105, DurationUnit.Week), "Only loan terms up to 2 years are supported.")
        {
        }
    }
}