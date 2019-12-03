package com.leecampbell.cod.domain.contracts;

import java.math.BigDecimal;
import java.time.OffsetDateTime;

public final class LoanDisbursedFundsEvent extends DomainEvent {

    private final OffsetDateTime transactionDate;
    private final BigDecimal amount;
    private final BankAccount disbursedTo;

    public LoanDisbursedFundsEvent(OffsetDateTime transactionDate, BigDecimal amount, BankAccount disbursedTo) {
        this.transactionDate = transactionDate;
        this.amount = amount;
        this.disbursedTo = disbursedTo;
    }

    public OffsetDateTime getTransactionDate() {
        return this.transactionDate;
    }

    public BigDecimal getAmount() {
        return this.amount;
    }

    public BankAccount getDisbursedTo() {
        return this.disbursedTo;
    }
}