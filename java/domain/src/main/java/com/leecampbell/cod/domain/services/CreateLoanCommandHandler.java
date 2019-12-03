package com.leecampbell.cod.domain.services;

import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;

public final class CreateLoanCommandHandler implements CommandHandler<CreateLoanCommand, Receipt> {
    private final Repository _repository;

    public CreateLoanCommandHandler(Repository repository) {
        _repository = repository;
    }

    public Receipt handle(CreateLoanCommand command) {
        Loan loan = _repository.get(command.getAggregateId());
        loan.create(command);
        _repository.save(loan);
        return new CommandReceipt(loan.getId(), loan.getVersion());
    }
}