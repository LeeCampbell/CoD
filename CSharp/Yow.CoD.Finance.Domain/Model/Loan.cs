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
            if (Version > 0) throw new InvalidOperationException("Loan already created.");
            if (command.Amount < 50m || 2000 < command.Amount) throw new InvalidOperationException("Only loan amounts between $50.00 and $2000.00 are supported.");
            if (!IsUnder2Years(command.Term)) throw new InvalidOperationException("Only loan terms up to 2 years are supported.");

            AddEvent(new LoanCreatedEvent(command.CreatedOn, command.Amount, command.Term, command.PaymentPlan));
            AddEvent(new LoanCustomerContactChangedEvent(command.CustomerContact.Name, command.CustomerContact.PreferredPhoneNumber, command.CustomerContact.AlternatePhoneNumber, command.CustomerContact.PostalAddress));
            AddEvent(new LoanBankAccountChangedEvent(command.BankAccount.Bsb, command.BankAccount.AccountNumber));
        }

        public void DisburseFunds(DisburseLoanFundsCommand command)
        {
            if (_disbursedOn != default) throw new InvalidOperationException("Funds are already disbursed.");

            var disburseToAccount = new Contracts.BankAccount(_bankAccount.Bsb, _bankAccount.AccountNumber);
            AddEvent(new LoanDisbursedFundsEvent(command.TransactionDate, -_loanAmount, disburseToAccount));
        }

        public void TakePayment(TakePaymentCommand command)
        {
            if (command.TransactionDateTime < _createdOn)
                throw new ArgumentException("Transaction date can not be prior to loan creation", nameof(command));
            if (command.Amount <= decimal.Zero)
                throw new ArgumentException("Transaction amount must be positive", nameof(command));

            AddEvent(new PaymentTakenEvent(Guid.NewGuid().ToString(), command.TransactionDateTime, command.Amount));
            if (_balance >= decimal.Zero)
                AddEvent(new LoanSettledEvent(command.TransactionDateTime));
            if (_balance > decimal.Zero)
                AddEvent(new LoanOverPaidEvent(command.TransactionDateTime, _balance));
        }



        //public void ReverseDisbursement(ReverseDisbursement command)
        //{
        //}
        //public void ReversePayment(ReversePaymentCommand command)
        //{
        //}
        //public void RefundPayment(RefundPaymentCommand command)
        //{
        //}
        //public void PaymentMissed(?? command)
        //{
        //}
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
            _balance = decimal.Zero; //Balance is zero until we disburse the money.
        }

        private void Handle(LoanDisbursedFundsEvent e)
        {
            _disbursedOn = e.TransactionDate;
            _balance += e.Amount;
        }

        private void Handle(LoanCustomerContactChangedEvent e)
        {
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

        private static bool IsUnder2Years(Duration duration)
        {
            switch (duration.Unit)
            {
                case DurationUnit.Day:
                    return duration.Length < 365 * 2;
                case DurationUnit.Week:
                    return duration.Length < 52 * 2;
                case DurationUnit.Month:
                    return duration.Length < 12 * 2;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}