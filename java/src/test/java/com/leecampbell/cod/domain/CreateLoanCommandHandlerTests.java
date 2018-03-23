package com.leecampbell.cod.domain;

import static org.junit.Assert.assertEquals;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.UUID;

import com.leecampbell.cod.domain.contracts.BankAccount;
import com.leecampbell.cod.domain.contracts.CreateLoanCommand;
import com.leecampbell.cod.domain.contracts.CustomerContact;
import com.leecampbell.cod.domain.contracts.Duration;
import com.leecampbell.cod.domain.contracts.DurationUnit;
import com.leecampbell.cod.domain.contracts.LoanCreatedEvent;
import com.leecampbell.cod.domain.contracts.PaymentPlan;
import com.leecampbell.cod.domain.services.CreateLoanCommandHandler;

import org.junit.Test;

public class CreateLoanCommandHandlerTests extends CommandHandlerTestBase<CreateLoanCommand> {

    public CreateLoanCommandHandlerTests() {
        super((repo) -> new CreateLoanCommandHandler(repo));
    }

    @Test
    public void LoanCreatedEventIsRaised() {
        Given(() -> { });
        When(this::CreateLoan);
        Then((cmd, raisedEvents) -> {
            LoanCreatedEvent actual = (LoanCreatedEvent) raisedEvents.get(0);
            assertEquals(cmd.createdOn(), actual.createdOn());
            assertEquals(cmd.amount(), actual.amount());
            assertEquals(cmd.term(), actual.term());
            assertEquals(cmd.paymentPlan(), actual.paymentPlan());
        });
    }

    private CreateLoanCommand CreateLoan() {
        CustomerContact customerContact = new CustomerContact("Jane Doe", "0412341234", "0856785678",
                "10 St Georges Terrace, Perth, WA 6000");
        BankAccount bankAccount = new BankAccount("066-000", "12345678");

        return new CreateLoanCommand(UUID.randomUUID(), UUID.randomUUID(),
                OffsetDateTime.of(2001, 2, 3, 4, 5, 6, 7, ZoneOffset.UTC), customerContact, bankAccount,
                PaymentPlan.Weekly, BigDecimal.valueOf(1000), new Duration(12, DurationUnit.Month));
    }
}