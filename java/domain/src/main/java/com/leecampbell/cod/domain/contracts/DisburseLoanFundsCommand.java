package com.leecampbell.cod.domain.contracts;

import java.time.OffsetDateTime;
import java.util.UUID;

public final class DisburseLoanFundsCommand extends DomainCommand {
    private final OffsetDateTime transactionDate;

    public DisburseLoanFundsCommand(UUID commandId, UUID aggregateId, OffsetDateTime transactionDate) {
        super(commandId, aggregateId);
        this.transactionDate = transactionDate;
    }

    public OffsetDateTime getTransactionDate() {
        return this.transactionDate;
    }
}