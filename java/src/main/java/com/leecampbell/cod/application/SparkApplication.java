package com.leecampbell.cod.application;

import static spark.Spark.*;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.UUID;
import com.google.gson.Gson;
import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;
import com.leecampbell.cod.domain.services.CreateLoanCommandHandler;
import com.leecampbell.cod.domain.services.Repository;

import spark.Request;
import spark.Response;

public class SparkApplication {
    private static final Gson gson = new Gson();

    public static void main(String[] args) {
        get("/hello", (req, res) -> "Hello, World! GFY");

        post("/Loan", "application/json", SparkApplication::createLoan);
        get("/foo", "application/json", (req, res) -> {
            Repository repository = new FakeRepository();
            CreateLoanCommandHandler handler = new CreateLoanCommandHandler(repository);
            CreateLoanCommand command = CreateCommand();

            Receipt r = handler.handle(command);
            return r.aggregateId().toString();
        });
    }

    private static Object createLoan(Request request, Response response) {
        String requestBody = request.body();
        CreateLoanModel requestModel = gson.fromJson(requestBody, CreateLoanModel.class);
        CreateLoanCommand command = requestModel.toCommand();

        Repository repository = new FakeRepository();
        CreateLoanCommandHandler handler = new CreateLoanCommandHandler(repository);
        Receipt receipt = handler.handle(command);

        LoanCreatedModel responseModel = new LoanCreatedModel();
        responseModel.loanId = receipt.aggregateId();
        return gson.toJson(responseModel);
    }

    private static CreateLoanCommand CreateCommand() {
        CustomerContact customerContact = new CustomerContact("Jane Doe", "0412341234", "0856785678",
                "10 St Georges Terrace, Perth, WA 6000");
        BankAccount bankAccount = new BankAccount("066-000", "12345678");

        return new CreateLoanCommand(UUID.randomUUID(), UUID.randomUUID(),
                OffsetDateTime.of(2001, 2, 3, 4, 5, 6, 7, ZoneOffset.UTC), customerContact, bankAccount,
                PaymentPlan.Weekly, BigDecimal.valueOf(1000), new Duration(12, DurationUnit.Month));
    }

    static final class FakeRepository implements Repository {
        private final ArrayList<DomainEvent> commitedEvents = new ArrayList<DomainEvent>();

        public Loan Get(UUID id) {
            return new Loan(id);
        }

        public void Save(Loan item) {
            List<DomainEvent> temp = Arrays.asList(item.GetUncommittedEvents());
            commitedEvents.addAll(temp);

            item.ClearUncommittedEvents();
        }
    }
}