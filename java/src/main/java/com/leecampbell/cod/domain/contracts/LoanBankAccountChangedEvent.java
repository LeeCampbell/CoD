package com.leecampbell.cod.domain.contracts;

public final class LoanBankAccountChangedEvent extends DomainEvent {
    private final BankAccount bankAccount;

    public LoanBankAccountChangedEvent(String bankAccountBsb, String bankAccountAccountNumber) {
        this.bankAccount = new BankAccount(bankAccountBsb, bankAccountAccountNumber);
    }

    public BankAccount getBankAccount() {
        return bankAccount;
    }
}