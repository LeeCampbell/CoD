package com.leecampbell.cod.domain.services;

import com.leecampbell.cod.domain.contracts.DomainCommand;
import com.leecampbell.cod.domain.contracts.Receipt;

public interface CommandHandler<TCommand extends DomainCommand, TReceipt extends Receipt>{
    TReceipt handle(TCommand command);
}