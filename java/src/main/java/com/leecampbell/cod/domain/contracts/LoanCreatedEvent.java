package com.leecampbell.cod.domain.contracts;

import java.math.BigDecimal;
import java.time.OffsetDateTime;

public final class LoanCreatedEvent extends DomainEvent {
    private final OffsetDateTime createdOn;
    private final Duration term;
    private final PaymentPlan paymentPlan;
    private final BigDecimal amount;

    public LoanCreatedEvent(OffsetDateTime createdOn, Duration term, PaymentPlan paymentPlan, BigDecimal amount) {
        this.createdOn = createdOn;
        this.term = term;
        this.paymentPlan = paymentPlan;
        this.amount = amount;
    }

    public OffsetDateTime createdOn() {
        return createdOn;
    }

    public Duration term() {
        return term;
    }

    public PaymentPlan paymentPlan(){
        return paymentPlan;
    }

    public BigDecimal amount() {
        return amount;
    }
}