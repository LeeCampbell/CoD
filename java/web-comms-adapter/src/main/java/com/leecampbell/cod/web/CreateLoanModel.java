package com.leecampbell.cod.web;

import com.leecampbell.cod.domain.contracts.*;
import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.util.UUID;

public final class CreateLoanModel {
    public String customerName;
    public String preferredPhoneNumber;
    public String alternatePhoneNumber;
    public String postalAddress;
    public String bankBsb;
    public String bankAccount;
    public BigDecimal loanAmount;

    public CreateLoanCommand toCommand() {
        CustomerContact customerContact = new CustomerContact(customerName, preferredPhoneNumber, alternatePhoneNumber,
                postalAddress);
        BankAccount account = new BankAccount(bankBsb, bankAccount);
        Duration duration = new Duration(12, DurationUnit.Month);
        return new CreateLoanCommand(UUID.randomUUID(), UUID.randomUUID(), OffsetDateTime.now(),
                customerContact, account, PaymentPlan.Weekly, loanAmount, duration);
    }
}
