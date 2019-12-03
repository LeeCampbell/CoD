package com.leecampbell.cod.domain.services;

import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;

public final class DisburseLoanFundsCommandHandler implements CommandHandler<DisburseLoanFundsCommand, Receipt> {
    private final Repository _repository;

    public DisburseLoanFundsCommandHandler(Repository repository) {
        _repository = repository;
    }

    public Receipt handle(DisburseLoanFundsCommand command) {
        Loan loan = _repository.get(command.getAggregateId());
        loan.disburseFunds(command);
        _repository.save(loan);
        return new CommandReceipt(loan.getId(), loan.getVersion());
    }
}