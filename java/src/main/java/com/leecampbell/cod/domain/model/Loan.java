package com.leecampbell.cod.domain.model;

import com.leecampbell.cod.domain.contracts.*;
import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.util.UUID;

public final class Loan extends AggregateRoot {
    private static final BigDecimal MIN_AMOUNT = new BigDecimal(50);
    private static final BigDecimal MAX_AMOUNT = new BigDecimal(2000);

    private OffsetDateTime createdOn;
    private BigDecimal loanAmount;
    private BigDecimal balance;
    private BankAccount bankAccount;
    private OffsetDateTime disbursedOn;

    public Loan(UUID loanId) {
        super(loanId);
    }

    public void Create(CreateLoanCommand command) {

        if (version() > 0)
            throw new UnsupportedOperationException("Loan already created.");
        if (command.amount().compareTo(MIN_AMOUNT) < 0 || MAX_AMOUNT.compareTo(command.amount()) < 0)
            throw new UnsupportedOperationException("Only loan amounts between $50.00 and $2000.00 are supported.");
        if (!isUnder2Years(command.term()))
            throw new UnsupportedOperationException("Only loan terms up to 2 years are supported.");

        AddEvent(new LoanCreatedEvent(command.createdOn(), command.term(), command.paymentPlan(), command.amount()));
        AddEvent(new LoanCustomerContactChangedEvent(command.customerContact().name(),
                command.customerContact().preferredPhoneNumber(), command.customerContact().alternatePhoneNumber(),
                command.customerContact().postalAddress()));
        AddEvent(new LoanBankAccountChangedEvent(command.bankAccount().bsb(), command.bankAccount().accountNumber()));
    }

    public void DisburseFunds(DisburseLoanFundsCommand command) {
        if (disbursedOn != null)
            throw new UnsupportedOperationException("Funds are already disbursed.");

        BankAccount disburseToAccount = new BankAccount(bankAccount.bsb(), bankAccount.accountNumber());
        AddEvent(new LoanDisbursedFundsEvent(command.transactionDate(), loanAmount.negate(), disburseToAccount));
    }

    public void TakePayment(TakePaymentCommand command) {
        if (createdOn == null)
            throw new UnsupportedOperationException("Take payment attempted on an uncreated loan.");
        if (command.transactionDate().compareTo(createdOn) < 0)
            throw new UnsupportedOperationException("Transaction date can not be prior to loan creation.");
        if (command.transactionAmount().compareTo(BigDecimal.ZERO) <= 0)
            throw new UnsupportedOperationException("Transaction amount must be positive.");

        AddEvent(new PaymentTakenEvent(UUID.randomUUID().toString(), command.transactionDate(),
                command.transactionAmount()));
        if (balance.compareTo(BigDecimal.ZERO) >= 0)
            AddEvent(new LoanSettledEvent(command.transactionDate()));
        if (balance.compareTo(BigDecimal.ZERO) > 0)
            AddEvent(new LoanOverPaidEvent(command.transactionDate(), balance));
    }
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

    private void handle(LoanDisbursedFundsEvent e) {
        disbursedOn = e.transactionDate();
        balance = balance.add(e.amount());
    }

    private void handle(LoanCustomerContactChangedEvent e) {
    }

    private void handle(LoanBankAccountChangedEvent e) {
        this.bankAccount = e.bankAccount();
    }

    private void handle(PaymentTakenEvent e) {
        balance = balance.add(e.amount());
    }

    private void handle(LoanSettledEvent e) {
    }

    private void handle(LoanOverPaidEvent e) {
    }

    private static boolean isUnder2Years(Duration duration) {
        switch (duration.Unit()) {
        case Day:
            return duration.length() < 365 * 2;
        case Week:
            return duration.length() < 52 * 2;
        case Month:
            return duration.length() < 12 * 2;
        default:
            throw new IllegalArgumentException("Unsupported duration");
        }
    }
}