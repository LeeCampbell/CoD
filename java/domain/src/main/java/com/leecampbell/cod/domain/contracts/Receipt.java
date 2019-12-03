package com.leecampbell.cod.domain.contracts;

import java.util.UUID;

public interface Receipt {
    public UUID getAggregateId();

    public int getVersion();
}