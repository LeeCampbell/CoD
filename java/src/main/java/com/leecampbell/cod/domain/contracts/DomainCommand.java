package com.leecampbell.cod.domain.contracts;

import java.util.UUID;

public abstract class DomainCommand {
    private final UUID commandId;
    private final UUID aggregateId;

    protected DomainCommand(UUID commandId, UUID aggregateId) {
        Arg.IsNotNullOrEmpty(commandId, "commandId");
        Arg.IsNotNullOrEmpty(aggregateId, "aggregateId");
        
        this.commandId = commandId;
        this.aggregateId = aggregateId;
    }

    public UUID getCommandId() {
        return commandId;
    }

    public UUID getAggregateId() {
        return aggregateId;
    }
}