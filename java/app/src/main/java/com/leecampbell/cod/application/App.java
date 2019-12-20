package com.leecampbell.cod.application;

import com.leecampbell.cod.data.PostgresqlRepository;
import com.leecampbell.cod.domain.services.CreateLoanCommandHandler;
import com.leecampbell.cod.domain.services.Repository;
import com.leecampbell.cod.web.CommandHandlerLoggerDecorator;
import com.leecampbell.cod.web.SparkServer;

public class App {
    public static void main(String[] args) {
        Repository repository = new PostgresqlRepository("jdbc:postgresql://cod-database:5432/postgres?user=postgres&password=mysecretpassword");
        CreateLoanCommandHandler createLoanCommandHandler = new CreateLoanCommandHandler(repository);

        SparkServer server = new SparkServer(new CommandHandlerLoggerDecorator<>(createLoanCommandHandler));
        server.serve();
    }
}