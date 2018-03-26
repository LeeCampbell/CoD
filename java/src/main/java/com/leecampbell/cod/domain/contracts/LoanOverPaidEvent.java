package com.leecampbell.cod.domain.contracts;

import java.math.BigDecimal;
import java.time.OffsetDateTime;

public final class LoanOverPaidEvent extends DomainEvent{
    
    private final OffsetDateTime transactionDateTime;
    private final BigDecimal amount;

    public LoanOverPaidEvent(OffsetDateTime transactionDateTime, BigDecimal amount) {
        this.transactionDateTime = transactionDateTime;
        this.amount = amount;
    }

    public OffsetDateTime getTransactionDateTime() {
        return this.transactionDateTime;
    }

    public BigDecimal getAmount() {
        return this.amount;
    }
}