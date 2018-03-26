package com.leecampbell.cod.domain.services;

import java.util.UUID;

import com.leecampbell.cod.domain.contracts.Receipt;

final class CommandReceipt implements Receipt {
    private final UUID aggregateId;
    private final int version;

    public CommandReceipt(UUID aggregateId, int version) {
        this.aggregateId = aggregateId;
        this.version = version;
    }

    public UUID getAggregateId() {
        return this.aggregateId;
    }

    public int getVersion() {
        return this.version;
    }
}