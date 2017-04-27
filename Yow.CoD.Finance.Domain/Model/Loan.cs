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

        //CustomerContact
        //Banking
        //Contract
        //  DateSigned, Approved, DocumentId
        //  Payment Plan
        //  Fee model
        //Account
        //  Balance
        //  Due
        //  State
        //  Transactions (?) else just read model
        //      Effective Disbursement Date



        private CustomerContact _customerContact;
        private BankAccount _bankAccount;

        public Loan(Guid loanId) : base(loanId)
        {
        }



        public void Create(CreateLoanCommand command)
        {
            //TODO: Check if command is valid (first command. structure is valid)

            AddEvent(new LoanCreatedEvent(command.CreatedOn, command.Amount, command.Term, command.PaymentPlan));
            AddEvent(new LoanCustomerContactChangedEvent(command.CustomerContact.Name, command.CustomerContact.PreferredPhoneNumber, command.CustomerContact.AlternatePhoneNumber, command.CustomerContact.PostalAddress));
            AddEvent(new LoanBankAccountChangedEvent(command.BankAccount.Bsb, command.BankAccount.AccountNumber));
            //TODO: Repayment Schedule updated event
        }

        //public void DisburseFunds(DisburseFundsCommand command)
        //{

        //}

        //public void ReverseDisbursement(ReverseDisbursement command)
        //{

        //}

        public void TakePayment(TakePaymentCommand command)
        {
            //Check if Command is valid (settled, duplicate command)

            //Raise Payment Taken Event
            AddEvent(new PaymentTakenEvent(command.TransactionDateTime, command.Amount));
            //If affects payment schedule
            //  raise payment schedule change event
            //If Settled
            //  raise loan settled event
            //If over paid
            //  raise overpaid event
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
    }
}