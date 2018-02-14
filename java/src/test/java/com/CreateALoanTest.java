package com;

/*
 * This Java source file was generated by the Gradle 'init' task.
 */
import org.junit.Test;
import static org.junit.Assert.*;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.util.UUID;

import com.leecampbell.cod.domain.contracts.CreateLoanCommand;
import com.leecampbell.cod.domain.contracts.DomainEvent;
import com.leecampbell.cod.domain.model.Loan;

public class CreateALoanTest {
    @Test
    public void loanCreatedEventIsRaised() {
        UUID loanId = UUID.randomUUID();
        Loan loan = new Loan(loanId);

        CreateLoanCommand cmd = new CreateLoanCommand(OffsetDateTime.now(), new BigDecimal(1000));
        loan.Create(cmd);

        DomainEvent[] events = loan.GetUncommittedEvents();

        //assertTrue("Creating a loan should emit a loan created event", events.length);
        assertEquals(1, events.length);
    }

    @Test
    public void loanCommitClearUncommittedEvents() {
        UUID loanId = UUID.randomUUID();
        Loan loan = new Loan(loanId);

        CreateLoanCommand cmd = new CreateLoanCommand(OffsetDateTime.now(), new BigDecimal(1000));
        loan.Create(cmd);
        loan.ClearUncommittedEvents();
        DomainEvent[] events = loan.GetUncommittedEvents();

        //assertTrue("Creating a loan should emit a loan created event", events.length);
        assertEquals(0, events.length);
    }
}
