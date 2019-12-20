package com.leecampbell.cod.web;

import com.leecampbell.cod.domain.services.CommandHandler;
import com.leecampbell.cod.domain.contracts.DomainCommand;
import com.leecampbell.cod.domain.contracts.Receipt;

/**
 * CommandHandlerLoggerDecorator<T> implements CommandHandler<T>
 */
public final class CommandHandlerLoggerDecorator<TCommand extends DomainCommand, TReceipt extends Receipt>
        implements CommandHandler<TCommand, TReceipt> {
    private final CommandHandler<TCommand, TReceipt> inner;

    public CommandHandlerLoggerDecorator(CommandHandler<TCommand, TReceipt> inner) {
        this.inner = inner;
    }

    public TReceipt handle(TCommand command) {
        System.out.printf("Processing Command %s for AggregateId %s", command.getClass().getCanonicalName(),
                command.getAggregateId());
        System.out.println();
        try {
            TReceipt receipt = inner.handle(command);
            System.out.printf("Successfully processed Command %s for AggregateId %s",
                    command.getClass().getCanonicalName(), command.getAggregateId());
            System.out.println();
            return receipt;
        } catch (Exception e) {
            System.out.printf("Failed to process Command %s for AggregateId %s", command.getClass().getCanonicalName(),
                    command.getAggregateId());
            System.out.println();
            throw e;
        }
    }
}