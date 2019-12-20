package com.leecampbell.cod.domain.contracts;

import java.util.UUID;

public interface Receipt {
    UUID getAggregateId();

    int getVersion();
}