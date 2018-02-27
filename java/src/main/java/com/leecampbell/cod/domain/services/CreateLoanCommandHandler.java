package com.leecampbell.cod.domain.services;

import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;

public class CreateLoanCommandHandler implements CommandHandler<CreateLoanCommand, Receipt>
    {
        private final Repository _repository;

        public CreateLoanCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public Receipt handle(CreateLoanCommand command)
        {
            Loan loan = _repository.Get(command.aggregateId());
            loan.Create(command);
            _repository.Save(loan);
            return new Receipt(loan.id(), loan.version());
        }
    }