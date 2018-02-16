package com.leecampbell.cod.domain;

import static org.junit.Assert.assertEquals;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.UUID;

import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;

import org.junit.Test;

public class CreateALoanTest {

    private CreateLoanCommand cmd;
    private Loan loan;

    public CreateALoanTest() {
        cmd = CreateCommand();
        loan = new Loan(cmd.AggregateId());
        loan.Create(cmd);
    }

    @Test
    public void loanCreatedEventIsRaised() {
        LoanCreatedEvent actual = (LoanCreatedEvent) loan.GetUncommittedEvents()[0];

        assertEquals(cmd.createdOn(), actual.createdOn());
        assertEquals(cmd.amount(), actual.amount());
        assertEquals(cmd.term(), actual.term());
        assertEquals(cmd.paymentPlan(), actual.paymentPlan());
    }

    @Test
    public void loanCustomerContactChangedEventRaised() {
        LoanCustomerContactChangedEvent actual = (LoanCustomerContactChangedEvent) loan.GetUncommittedEvents()[1];
        assertEquals(cmd.customerContact().name(), actual.customerContact().name());
        assertEquals(cmd.customerContact().preferredPhoneNumber(), actual.customerContact().preferredPhoneNumber());
        assertEquals(cmd.customerContact().alternatePhoneNumber(), actual.customerContact().alternatePhoneNumber());
        assertEquals(cmd.customerContact().postalAddress(), actual.customerContact().postalAddress());
    }

    @Test
    public void loanBankAccountChangedEventRaised() {
        LoanBankAccountChangedEvent actual = (LoanBankAccountChangedEvent) loan.GetUncommittedEvents()[2];
        assertEquals(cmd.bankAccount().bsb(), actual.bankAccount().bsb());
        assertEquals(cmd.bankAccount().accountNumber(), actual.bankAccount().accountNumber());
    }

    @Test
    public void subsequentCallToCreateThrows(){
        CreateLoanCommand cmd2 = CreateCommand();
        try {
            loan.Create(cmd2);    
        } catch (UnsupportedOperationException e) {
            assertEquals("Loan already created.", e.getMessage());
        }
    }

    @Test
    public void commitClearsUncommittedEvents() {
        loan.ClearUncommittedEvents();
        DomainEvent[] events = loan.GetUncommittedEvents();

        assertEquals(0, events.length);
    }

    private CreateLoanCommand CreateCommand() {
        CustomerContact customerContact = new CustomerContact("Jane Doe", "0412341234", "0856785678",
                "10 St Georges Terrace, Perth, WA 6000");
        BankAccount bankAccount = new BankAccount("066-000", "12345678");

        return new CreateLoanCommand(UUID.randomUUID(), UUID.randomUUID(),
                OffsetDateTime.of(2001, 2, 3, 4, 5, 6, 7, ZoneOffset.UTC), customerContact, bankAccount,
                PaymentPlan.Weekly, BigDecimal.valueOf(1000), new Duration(12, DurationUnit.Month));
    }
}