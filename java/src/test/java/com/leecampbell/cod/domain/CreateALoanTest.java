package com.leecampbell.cod.domain;

import org.junit.*;
import static org.junit.Assert.*;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.UUID;
import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;

public class CreateALoanTest {
    @Test
    public void loanCreatedEventIsRaised() {
        CreateLoanCommand cmd = CreateCommand();
        Loan loan = new Loan(cmd.AggregateId());
        loan.Create(cmd);

        DomainEvent[] events = loan.GetUncommittedEvents();

        //assertTrue("Creating a loan should emit a loan created event", events.length);
        assertEquals(1, events.length);
    }

    @Test
    public void loanCommitClearUncommittedEvents() {
        CreateLoanCommand cmd = CreateCommand();
        Loan loan = new Loan(cmd.AggregateId());
        loan.Create(cmd);

        loan.ClearUncommittedEvents();
        DomainEvent[] events = loan.GetUncommittedEvents();

        //assertTrue("Creating a loan should emit a loan created event", events.length);
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