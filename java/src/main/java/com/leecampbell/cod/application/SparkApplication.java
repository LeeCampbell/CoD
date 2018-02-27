package com.leecampbell.cod.application;

import static spark.Spark.post;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.UUID;

import com.google.gson.Gson;
import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;
import com.leecampbell.cod.domain.services.*;

import spark.Request;
import spark.Response;

public class SparkApplication {
    private static final Gson gson = new Gson();

    public static void main(String[] args) {
        post("/Loan", "application/json", SparkApplication::createLoan, gson::toJson);
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
        return responseModel;
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