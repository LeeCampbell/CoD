package com.leecampbell.cod.domain;

import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertNull;
import static org.junit.Assert.assertTrue;

import java.util.List;
import java.util.function.*;

import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.services.CommandHandler;
import com.leecampbell.cod.domain.services.Repository;

public abstract class CommandHandlerTestBase<TCommand extends DomainCommand> {

    private final StubRepository repository;
    private final CommandHandler<TCommand, Receipt> commandHandler;
    private TCommand command;
    private Exception caughtException;
    private Receipt receipt;

    protected CommandHandlerTestBase(Function<Repository, CommandHandler<TCommand, Receipt>> commandHandlerFactory) {
        repository = new StubRepository();
        commandHandler = commandHandlerFactory.apply(repository);
    }

    protected TCommand getCommand() {
        return command;
    }

    protected Receipt getReceipt() {
        return receipt;
    }

    protected void given(Supplier<Iterable<EventEnvelope>> initialiser) {
        Iterable<EventEnvelope> events = initialiser.get();
        repository.Load(events);
    }

    protected void when(Supplier<TCommand> commandFactory) {
        command = commandFactory.get();
        try {
            receipt = commandHandler.handle(command);
        } catch (Exception e) {
            caughtException = e;
        }
    }

    protected void then(BiConsumer<TCommand, List<DomainEvent>> assertion) {
        assertNull(caughtException);

        List<DomainEvent> raisedEvents = repository.CommittedEvents();
        assertion.accept(command, raisedEvents);
    }
    
    protected <TException extends Exception> void thenThrew(Class<TException> type, BiConsumer<TCommand, TException> assertion) {
        assertNotNull("No exception thrown", caughtException);
        assertTrue(type.isAssignableFrom(caughtException.getClass()));
        assertion.accept(command, (TException)caughtException);
    }
}
