package com.leecampbell.cod.domain.contracts;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.util.UUID;

public final class CreateLoanCommand extends DomainCommand
{
    
    private final OffsetDateTime createdOn;
    private final CustomerContact customerContact;
    private final BigDecimal amount;
    
    //TODO: Complete this port -LC
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
            this.amount = amount;
    }

    public OffsetDateTime createdOn(){
        return createdOn;
    }

    public CustomerContact customerContact(){
        return customerContact;
    }
    
    public BigDecimal amount(){
        return amount;
    }
}
