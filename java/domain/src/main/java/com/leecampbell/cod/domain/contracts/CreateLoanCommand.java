package com.leecampbell.cod.domain.contracts;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.util.UUID;

public final class CreateLoanCommand extends DomainCommand
{
    private final OffsetDateTime createdOn;
    private final CustomerContact customerContact;
    private final BankAccount bankAccount;
    private final PaymentPlan paymentPlan;
    private final BigDecimal amount;
    private final Duration term;
    
    public CreateLoanCommand(
        UUID commandId, 
        UUID aggregateId,
        OffsetDateTime createdOn, 
        CustomerContact customerContact,
        BankAccount bankAccount,
        PaymentPlan paymentPlan,
        BigDecimal amount,
        Duration term) {
            super(commandId, aggregateId);

            if (createdOn == null) throw new IllegalArgumentException("createdOn cannot be null");
            if (customerContact == null) throw new IllegalArgumentException("customerContact cannot be null");
            if (bankAccount == null) throw new IllegalArgumentException("bankAccount cannot be null");
            if (paymentPlan == PaymentPlan.None) throw new IllegalArgumentException("paymentPlan cannot be None");
            if (amount == null) throw new IllegalArgumentException("amount cannot be null");
            if (term == null) throw new IllegalArgumentException("term cannot be null");


            this.createdOn = createdOn;
            this.customerContact = customerContact;
            this.bankAccount = bankAccount;
            this.paymentPlan = paymentPlan;
            this.amount = amount;
            this.term = term;
    }

    public OffsetDateTime createdOn(){
        return createdOn;
    }

    public CustomerContact customerContact(){
        return customerContact;
    }

    public BankAccount bankAccount(){
        return bankAccount;
    }

    public PaymentPlan paymentPlan(){
        return paymentPlan;
    }
    
    public BigDecimal amount(){
        return amount;
    }

    public Duration term(){
        return term;
    }
}
