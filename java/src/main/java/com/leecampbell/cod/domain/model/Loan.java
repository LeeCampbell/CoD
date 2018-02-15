package com.leecampbell.cod.domain.model;

import com.leecampbell.cod.domain.contracts.CreateLoanCommand;
import com.leecampbell.cod.domain.contracts.LoanCreatedEvent;
import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.util.UUID;

public final class Loan extends AggregateRoot {
    private static final BigDecimal MIN_AMOUNT = new BigDecimal(50);
    private static final BigDecimal MAX_AMOUNT = new BigDecimal(2000);

    private OffsetDateTime createdOn;
    private BigDecimal loanAmount;
    private BigDecimal balance;

    public Loan(UUID loanId) {
        super(loanId);
    }

    public void Create(CreateLoanCommand command) {

        if (version() > 0)
            throw new UnsupportedOperationException("Loan already created.");
        if (command.amount().compareTo(MIN_AMOUNT) < 0 || MAX_AMOUNT.compareTo(command.amount()) < 0)
            throw new UnsupportedOperationException("Only loan amounts between $50.00 and $2000.00 are supported.");
        //if (!IsUnder2Years(command.Term)) throw new UnsupportedOperationException("Only loan terms up to 2 years are supported.");

        AddEvent(new LoanCreatedEvent(command.createdOn(), command.amount()));
        //AddEvent(new LoanCustomerContactChangedEvent(command.CustomerContact.Name, command.CustomerContact.PreferredPhoneNumber, command.CustomerContact.AlternatePhoneNumber, command.CustomerContact.PostalAddress));
        //AddEvent(new LoanBankAccountChangedEvent(command.BankAccount.Bsb, command.BankAccount.AccountNumber));
    }

    //public void DisburseFunds(DisburseLoanFundsCommand command) {
    //}
    //public void TakePayment(TakePaymentCommand command) {
    //}
    //public void ReverseDisbursement(ReverseDisbursement command) {
    //}
    //public void ReversePayment(ReversePaymentCommand command) {
    //}
    //public void RefundPayment(RefundPaymentCommand command) {
    //}
    //public void PaymentMissed(?? command) {
    //}
    //public void SetRepaymentMethod(SetRepaymentMethodCommand command) {
    //}
    //public void FailLoan(FailLoanCommand command) {
    //}
    //public void CancelLoan(CancelLoanCommand command) {
    //}

    private void handle(LoanCreatedEvent e) {
        createdOn = e.createdOn();
        loanAmount = e.amount();
        balance = BigDecimal.ZERO; //Balance is zero until we disburse the money.
    }

    // private void Handle(LoanDisbursedFundsEvent e) {
    //     _disbursedOn = e.TransactionDate;
    //     _balance += e.Amount;
    // }

    // private void Handle(LoanCustomerContactChangedEvent e) {
    // }

    // private void Handle(LoanBankAccountChangedEvent e) {
    //     _bankAccount = new BankAccount(e.BankAccount.Bsb, e.BankAccount.AccountNumber);
    // }

    // private void Handle(PaymentTakenEvent e) {
    //     _balance += e.Amount;
    // }

    // private void Handle(LoanSettledEvent e) {
    // }

    // private void Handle(LoanOverPaidEvent e) {
    // }

    // private static bool IsUnder2Years(Duration duration) {
    //     switch (duration.Unit) {
    //     case DurationUnit.Day:
    //         return duration.Length < 365 * 2;
    //     case DurationUnit.Week:
    //         return duration.Length < 52 * 2;
    //     case DurationUnit.Month:
    //         return duration.Length < 12 * 2;
    //     default:
    //         throw new ArgumentOutOfRangeException();
    //     }
    // }
}