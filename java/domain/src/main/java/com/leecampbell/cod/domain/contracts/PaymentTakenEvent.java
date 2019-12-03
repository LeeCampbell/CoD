package com.leecampbell.cod.domain.contracts;

import java.math.BigDecimal;
import java.time.OffsetDateTime;

public final class PaymentTakenEvent extends DomainEvent {
    
    private final String transactionId;
    private final OffsetDateTime transactionDateTime;
    private final BigDecimal amount;

    public PaymentTakenEvent(String transactionId, OffsetDateTime transactionDateTime, BigDecimal amount) {
        this.transactionId = transactionId;
        this.transactionDateTime = transactionDateTime;
        this.amount = amount;
    }

    public String transactionId() {
        return this.transactionId;
    }

    public OffsetDateTime transactionDateTime() {
        return this.transactionDateTime;
    }

    public BigDecimal amount() {
        return this.amount;
    }
}