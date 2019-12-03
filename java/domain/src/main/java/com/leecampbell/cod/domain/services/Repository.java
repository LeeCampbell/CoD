package com.leecampbell.cod.domain.services;

import com.leecampbell.cod.domain.model.Loan;
import java.util.UUID;

public interface Repository{
    Loan get(UUID id);
    void save(Loan item);
}