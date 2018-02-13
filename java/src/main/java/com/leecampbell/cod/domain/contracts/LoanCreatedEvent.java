package com.leecampbell.cod.domain.contracts;

import java.math.BigDecimal;
import java.time.OffsetDateTime;

public final class LoanCreatedEvent extends DomainEvent
{
    private final OffsetDateTime createdOn;
    private final BigDecimal amount;
    
    //TODO: Complete this port -LC
    public LoanCreatedEvent(OffsetDateTime createdOn, BigDecimal amount) {
        this.createdOn = createdOn;
        this.amount = amount;
    }

    public OffsetDateTime createdOn(){
        return createdOn;
    }
    
    public BigDecimal amount(){
        return amount;
    }
}