using System;
using NUnit.Framework;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.Domain.Tests.Contracts
{
    [TestFixture]
    public sealed class CreateLoanCommandFixture
    {
        private static readonly Guid ValidCommandId = Guid.NewGuid();
        private static readonly Guid ValidAggregateId = Guid.NewGuid();
        private static readonly DateTimeOffset ValidCreatedOn = DateTimeOffset.Now;
        private static readonly CustomerContact ValidCustomerContact = new CustomerContact("Bob Fuller", "0412345678", null, "10 Wayward ave");
        private static readonly BankAccount ValidBankAccount = new BankAccount("066-000", "12345678");
        private static readonly Duration ValidDuration = new Duration(12, DurationUnit.Month);
        private const decimal ValidLoanAmount = 1000m;
        private const PaymentPlan ValidPaymentPlan = PaymentPlan.Weekly;

        [Test]
        public void RejectsZeroValueForCommandId()
        {
            var ex = Assert.Throws<ArgumentException>(() => new CreateLoanCommand(Guid.Empty,
                                                          ValidAggregateId,
                                                          ValidCreatedOn,
                                                          ValidCustomerContact,
                                                          ValidBankAccount,
                                                          ValidPaymentPlan,
                                                          ValidLoanAmount,
                                                          ValidDuration));
            Assert.AreEqual("CommandId must be non default value\r\nParameter name: commandId", ex.Message);
        }

        [Test]
        public void RejectsZeroValueForAggregateId()
        {
            var ex = Assert.Throws<ArgumentException>(() => new CreateLoanCommand(ValidCommandId,
                                                          Guid.Empty,
                                                          ValidCreatedOn,
                                                          ValidCustomerContact,
                                                          ValidBankAccount,
                                                          ValidPaymentPlan,
                                                          ValidLoanAmount,
                                                          ValidDuration));
            Assert.AreEqual("AggregateId must be non default value\r\nParameter name: aggregateId", ex.Message);
        }

        [Test]
        public void RejectsZeroValueForCreatedOn()
        {
            var ex = Assert.Throws<ArgumentException>(() => new CreateLoanCommand(ValidCommandId,
                                                          ValidAggregateId,
                                                          default(DateTimeOffset), 
                                                          ValidCustomerContact,
                                                          ValidBankAccount,
                                                          ValidPaymentPlan,
                                                          ValidLoanAmount,
                                                          ValidDuration));
            Assert.AreEqual("CreatedOn must be non default value\r\nParameter name: createdOn", ex.Message);
        }
        [Test]
        public void RejectsNullValueForCustomerContact()
        {
            var ex = Assert.Throws<ArgumentException>(() => new CreateLoanCommand(ValidCommandId,
                                                          ValidAggregateId,
                                                          ValidCreatedOn,
                                                          null,
                                                          ValidBankAccount,
                                                          ValidPaymentPlan,
                                                          ValidLoanAmount,
                                                          ValidDuration));
            Assert.AreEqual("CustomerContact must be non null\r\nParameter name: customerContact", ex.Message);
        }

        [Test]
        public void RejectsNullValueForBankAccount()
        {
            var ex = Assert.Throws<ArgumentException>(() => new CreateLoanCommand(ValidCommandId,
                                                          ValidAggregateId,
                                                          ValidCreatedOn,
                                                          ValidCustomerContact,
                                                          null,
                                                          ValidPaymentPlan,
                                                          ValidLoanAmount,
                                                          ValidDuration));
            Assert.AreEqual("BankAccount must be non null\r\nParameter name: bankAccount", ex.Message);
        }

        [Test]
        public void RejectsZeroValueForPaymentPlan()
        {
            var ex = Assert.Throws<ArgumentException>(() => new CreateLoanCommand(ValidCommandId,
                                                          ValidAggregateId,
                                                          ValidCreatedOn,
                                                          ValidCustomerContact,
                                                          ValidBankAccount,
                                                          PaymentPlan.None, 
                                                          ValidLoanAmount,
                                                          ValidDuration));
            Assert.AreEqual("PaymentPlan must be non default value\r\nParameter name: paymentPlan", ex.Message);
        }

        [Test]
        public void RejectsZeroValueForAmount()
        {
            var ex = Assert.Throws<ArgumentException>(() => new CreateLoanCommand(ValidCommandId,
                                                          ValidAggregateId,
                                                          ValidCreatedOn,
                                                          ValidCustomerContact,
                                                          ValidBankAccount,
                                                          ValidPaymentPlan,
                                                          0,
                                                          ValidDuration));
            Assert.AreEqual("Amount must be non zero value\r\nParameter name: amount", ex.Message);
        }

        [Test]
        public void RejectsNullValueForTerm()
        {
            var ex = Assert.Throws<ArgumentException>(() => new CreateLoanCommand(ValidCommandId,
                                                          ValidAggregateId,
                                                          ValidCreatedOn,
                                                          ValidCustomerContact,
                                                          ValidBankAccount,
                                                          ValidPaymentPlan,
                                                          ValidLoanAmount,
                                                          null));
            Assert.AreEqual("Term must be non null\r\nParameter name: term", ex.Message);
        }
    }
}