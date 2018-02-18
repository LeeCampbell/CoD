package com.leecampbell.cod.domain.contracts;

import java.time.OffsetDateTime;

public final class LoanSettledEvent extends DomainEvent{
    private final OffsetDateTime transactionDateTime;

    public LoanSettledEvent(OffsetDateTime transactionDateTime) {
        this.transactionDateTime = transactionDateTime;
    }

    public OffsetDateTime transactionDateTime(){
        return this.transactionDateTime;
    }
}