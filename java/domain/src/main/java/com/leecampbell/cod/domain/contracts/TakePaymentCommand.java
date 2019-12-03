package com.leecampbell.cod.domain.contracts;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.util.UUID;

public final class TakePaymentCommand extends DomainCommand {
    
    private final OffsetDateTime transactionDate;
    private final BigDecimal transactionAmount;

    public TakePaymentCommand(UUID commandId, UUID aggregateId, BigDecimal transactionAmount, OffsetDateTime transactionDate) {
        super(commandId, aggregateId);
        this.transactionAmount = transactionAmount;
        this.transactionDate = transactionDate;
    }

    public BigDecimal getTransactionAmount(){
        return this.transactionAmount;
    }

    public OffsetDateTime getTransactionDate() {
        return this.transactionDate;
    }
}