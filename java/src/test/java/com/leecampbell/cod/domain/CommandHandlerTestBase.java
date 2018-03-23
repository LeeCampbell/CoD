package com.leecampbell.cod.domain;

import java.util.List;
import java.util.function.BiConsumer;
import java.util.function.Consumer;
import java.util.function.Function;
import java.util.function.Supplier;

import com.leecampbell.cod.domain.contracts.DomainCommand;
import com.leecampbell.cod.domain.contracts.DomainEvent;
import com.leecampbell.cod.domain.contracts.Receipt;
import com.leecampbell.cod.domain.services.CommandHandler;
import com.leecampbell.cod.domain.services.Repository;

public class CommandHandlerTestBase<TCommand extends DomainCommand> {
    
    private final StubRepository repository;
    private final CommandHandler<TCommand, Receipt> commandHandler;
    private TCommand command;

    protected CommandHandlerTestBase(Function<Repository, CommandHandler<TCommand, Receipt>> commandHandlerFactory){
        repository = new StubRepository();
        commandHandler = commandHandlerFactory.apply(repository);
    }

    protected TCommand getCommand(){
        return command;
    }

    protected void Given(Runnable initialiser){
        initialiser.run();
    }

    protected void When(Supplier<TCommand> commandFactory){
        command = commandFactory.get();
        commandHandler.handle(command);
    }
    
    protected void Then(BiConsumer<TCommand, List<DomainEvent>> assertion){
        List<DomainEvent> raisedEvents = repository.CommitedEvents();
        assertion.accept(command, raisedEvents);
    }
}
