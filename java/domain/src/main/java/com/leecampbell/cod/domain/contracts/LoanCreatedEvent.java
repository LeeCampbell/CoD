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

    public OffsetDateTime getCreatedOn() {
        return createdOn;
    }

    public Duration getTerm() {
        return term;
    }

    public PaymentPlan getPaymentPlan(){
        return paymentPlan;
    }

    public BigDecimal getAmount() {
        return amount;
    }
}