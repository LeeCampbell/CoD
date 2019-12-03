package com.leecampbell.cod.application;

import com.leecampbell.cod.domain.services.CreateLoanCommandHandler;
import com.leecampbell.cod.domain.services.Repository;

public class App {
    public static void main(String[] args) {
        Repository repository = new FakeRepository();
        CreateLoanCommandHandler createLoanCommandHandler = new CreateLoanCommandHandler(repository);

        SparkServer server = new SparkServer(new CommandHandlerLoggerDecorator<>(createLoanCommandHandler));
        server.serve();
    }
}