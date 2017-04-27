using System;
using System.Collections.Generic;
using NUnit.Framework;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.Domain.Tests
{
    public class CreatingALoan : Specification<Loan, CreateLoanCommand>
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

        }
        protected override IEnumerable<Event> Given()
        {
            yield break;
        }

        protected override CreateLoanCommand When()
        {
            return _command;
        }

        protected override IHandler<CreateLoanCommand> CreateHandler()
        {
            return new CreateLoanCommandHandler(Repository);
        }

        [Test]
        public void LoanCreatedEventRaised()
        {
            var actual = (LoanCreatedEvent)Produced[0];
            Assert.That(actual.CreatedOn, Is.EqualTo(_command.CreatedOn));
            Assert.That(actual.Amount, Is.EqualTo(_command.Amount));
            Assert.That(actual.Term, Is.EqualTo(_command.Term));
            Assert.That(actual.PaymentPlan, Is.EqualTo(_command.PaymentPlan));
        }

        [Test]
        public void LoanCustomerContactChangedEventRaised()
        {
            var actual = (LoanCustomerContactChangedEvent)Produced[1];
            Assert.That(actual.CustomerContact.Name, Is.EqualTo(_command.CustomerContact.Name));
            Assert.That(actual.CustomerContact.PreferredPhoneNumber, Is.EqualTo(_command.CustomerContact.PreferredPhoneNumber));
            Assert.That(actual.CustomerContact.AlternatePhoneNumber, Is.EqualTo(_command.CustomerContact.AlternatePhoneNumber));
            Assert.That(actual.CustomerContact.PostalAddress, Is.EqualTo(_command.CustomerContact.PostalAddress));
        }

        [Test]
        public void LoanBankAccountChangedEventRaised()
        {
            var actual = (LoanBankAccountChangedEvent)Produced[2];
            Assert.That(actual.BankAccount.Bsb, Is.EqualTo(_command.BankAccount.Bsb));
            Assert.That(actual.BankAccount.AccountNumber, Is.EqualTo(_command.BankAccount.AccountNumber));
        }
    }
}