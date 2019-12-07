package com.leecampbell.cod.web;

import static spark.Spark.post;

import com.google.gson.Gson;
import com.leecampbell.cod.domain.contracts.CreateLoanCommand;
import com.leecampbell.cod.domain.contracts.Receipt;
import com.leecampbell.cod.domain.services.CommandHandler;

import spark.Request;
import spark.Response;

public class SparkServer {
    private static final Gson gson = new Gson();
    private final CommandHandler<CreateLoanCommand, Receipt> createLoanCommandHandler;

    public SparkServer(CommandHandler<CreateLoanCommand, Receipt> createLoanCommandHandler) {
        this.createLoanCommandHandler = createLoanCommandHandler;
    }

    public void serve(){
        post("/Loan", "application/json", this::createLoan, gson::toJson);
    }

    private Object createLoan(Request request, Response response) {
        CreateLoanModel requestModel = gson.fromJson(request.body(), CreateLoanModel.class);
        CreateLoanCommand command = requestModel.toCommand();
        Receipt receipt = createLoanCommandHandler.handle(command);
        return new LoanCreatedModel(receipt.getAggregateId());
    }
}