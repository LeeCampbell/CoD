using System;
using Yow.CoD.Finance.Domain.Contracts;

namespace Yow.CoD.Finance.Domain.Model
{
    //Make focus on Taking Payment
    //  Reduces balance
    //  May re-calculate payment schedule
    //  May transition the state of the loan (paid, overpaid, out arrears and back into active)

    public class Loan : AggregateRoot
    {
        private CustomerContact _customerContact;
        private BankAccount _bankAccount;
        private DateTimeOffset _createdOn;
        private DateTimeOffset _disbursedOn;
        private decimal _loanAmount;
        private decimal _balance;


        public Loan(Guid loanId) : base(loanId)
        {
            RegisterHandler<LoanCreatedEvent>(Handle);
            RegisterHandler<LoanDisbursedFundsEvent>(Handle);
            RegisterHandler<LoanCustomerContactChangedEvent>(Handle);
            RegisterHandler<LoanBankAccountChangedEvent>(Handle);
            RegisterHandler<PaymentTakenEvent>(Handle);
            RegisterHandler<LoanSettledEvent>(Handle);
            RegisterHandler<LoanOverPaidEvent>(Handle);
        }

        public void Create(CreateLoanCommand command)
        {
            //TODO: If Version > 0 throw
            if(Version>0) throw new InvalidOperationException("Loan already created.");
            //TODO: If command.Amount > 2000 throw
            //TODO: If command.Term > 2yrs throw

            AddEvent(new LoanCreatedEvent(command.CreatedOn, command.Amount, command.Term, command.PaymentPlan));
            AddEvent(new LoanCustomerContactChangedEvent(command.CustomerContact.Name, command.CustomerContact.PreferredPhoneNumber, command.CustomerContact.AlternatePhoneNumber, command.CustomerContact.PostalAddress));
            AddEvent(new LoanBankAccountChangedEvent(command.BankAccount.Bsb, command.BankAccount.AccountNumber));
            //TODO: Repayment Schedule updated event
        }

        public void DisburseFunds(DisburseLoanFundsCommand command)
        {
            var disburseToAccount = new Contracts.BankAccount(_bankAccount.Bsb, _bankAccount.AccountNumber);
            AddEvent(new LoanDisbursedFundsEvent(command.TransactionDate, -_loanAmount, disburseToAccount));
        }

        //public void ReverseDisbursement(ReverseDisbursement command)
        //{

        //}

        public void TakePayment(TakePaymentCommand command)
        {
            //Check if Command is valid (settled, duplicate command)
            if (command.TransactionDateTime < _createdOn)
                throw new ArgumentException("Transaction date can not be prior to loan creation", nameof(command));
            if (command.Amount <= decimal.Zero)
                throw new ArgumentException("Transaction amount must be positive", nameof(command));

            //Raise Payment Taken Event
            AddEvent(new PaymentTakenEvent(command.TransactionDateTime, command.Amount));
            //If affects payment schedule
            //  raise payment schedule change event
            if (_balance >= decimal.Zero)
                AddEvent(new LoanSettledEvent(command.TransactionDateTime));
            if (_balance > decimal.Zero)
                AddEvent(new LoanOverPaidEvent(command.TransactionDateTime, _balance));
        }

        //public void ReversePayment(ReversePaymentCommand command)
        //{

        //}
        //public void RefundPayment(RefundPaymentCommand command)
        //{

        //}
        ////public void PaymentMissed(?? command)
        ////{

        ////}

        //public void SetRepaymentMethod(SetRepaymentMethodCommand command)
        //{

        //}

        //public void FailLoan(FailLoanCommand command)
        //{

        //}

        //public void CancelLoan(CancelLoanCommand command)
        //{

        //}

        private void Handle(LoanCreatedEvent e)
        {
            _createdOn = e.CreatedOn;
            _loanAmount = e.Amount;
            _balance = decimal.Zero; //Only owe money once disbursed.
        }

        private void Handle(LoanDisbursedFundsEvent e)
        {
            _disbursedOn = e.TransactionDate;
            _balance += e.Amount;
        }

        private void Handle(LoanCustomerContactChangedEvent e)
        {
            _customerContact = new CustomerContact(
                e.CustomerContact.Name,
                e.CustomerContact.PreferredPhoneNumber,
                e.CustomerContact.AlternatePhoneNumber,
                e.CustomerContact.PostalAddress);
        }
        private void Handle(LoanBankAccountChangedEvent e)
        {
            _bankAccount = new BankAccount(e.BankAccount.Bsb, e.BankAccount.AccountNumber);
        }

        private void Handle(PaymentTakenEvent e)
        {
            _balance += e.Amount;
        }

        private void Handle(LoanSettledEvent e)
        {
        }
        private void Handle(LoanOverPaidEvent e)
        {
        }
    }
}