package com.leecampbell.cod.web;

import java.util.UUID;

public final class LoanCreatedModel {
    private final UUID loanId;
    
    public LoanCreatedModel(UUID loanId) {
        this.loanId = loanId;
    }

    public UUID getLoanId() {
		return loanId;
	}
}