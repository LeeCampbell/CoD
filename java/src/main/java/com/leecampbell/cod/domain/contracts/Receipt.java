package com.leecampbell.cod.domain.contracts;

import java.util.UUID;

public class Receipt {
    private final UUID aggregateId;
    private final int version;

    public Receipt(UUID aggregateId, int version) {
        if (aggregateId == null)
            throw new IllegalArgumentException("AggregateId must be non null");

        this.aggregateId = aggregateId;
        this.version = version;
    }

    public UUID aggregateId() {
        return this.aggregateId;
    }

    public int version() {
        return this.version;
    }
}