using System;
using Xunit;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.Domain.Tests.Contracts
{
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

        [Fact]
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
            Assert.Equal("CommandId must be non default value (Parameter 'commandId')", ex.Message);
        }

        [Fact]
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
            Assert.Equal("AggregateId must be non default value (Parameter 'aggregateId')", ex.Message);
        }

        [Fact]
        public void RejectsZeroValueForCreatedOn()
        {
            var ex = Assert.Throws<ArgumentException>(() => new CreateLoanCommand(ValidCommandId,
                                                          ValidAggregateId,
                                                          default, 
                                                          ValidCustomerContact,
                                                          ValidBankAccount,
                                                          ValidPaymentPlan,
                                                          ValidLoanAmount,
                                                          ValidDuration));
            Assert.Equal("CreatedOn must be non default value (Parameter 'createdOn')", ex.Message);
        }
        [Fact]
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
            Assert.Equal("CustomerContact must be non null (Parameter 'customerContact')", ex.Message);
        }

        [Fact]
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
            Assert.Equal("BankAccount must be non null (Parameter 'bankAccount')", ex.Message);
        }

        [Fact]
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
            Assert.Equal("PaymentPlan must be non default value (Parameter 'paymentPlan')", ex.Message);
        }

        [Fact]
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
            Assert.Equal("Amount must be non zero value (Parameter 'amount')", ex.Message);
        }

        [Fact]
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
            Assert.Equal("Term must be non null (Parameter 'term')", ex.Message);
        }
    }
}