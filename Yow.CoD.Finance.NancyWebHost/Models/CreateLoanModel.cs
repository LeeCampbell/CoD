using System;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.NancyWebHost.Models
{
    public sealed class CreateLoanModel
    {
        public string CustomerName { get; set; }
        public string PreferredPhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string PostalAddress { get; set; }
        public string BankBsb { get; set; }
        public string BankAccount { get; set; }
        public decimal LoanAmount { get; set; }

        public CreateLoanCommand ToCommand()
        {
            var customerContact = new CustomerContact(CustomerName, PreferredPhoneNumber, AlternatePhoneNumber,
                PostalAddress);
            var bankAccount = new BankAccount(BankBsb, BankAccount);
            var duration = new Duration(12, DurationUnit.Month);
            var command = new CreateLoanCommand(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.Now,
                customerContact,
                bankAccount,
                PaymentPlan.Weekly,
                LoanAmount,
                duration);
            return command;
        }
    }
}