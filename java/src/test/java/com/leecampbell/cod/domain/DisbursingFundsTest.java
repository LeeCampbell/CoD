package com.leecampbell.cod.domain;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.fail;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.time.temporal.ChronoUnit;
import java.util.UUID;

import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;

import org.junit.Test;

public final class DisbursingFundsTest {

    private final OffsetDateTime _createdDate = OffsetDateTime.of(2017, 06, 05, 04, 30, 20, 0, ZoneOffset.ofHours(10));
    private final BigDecimal _loanAmount = BigDecimal.valueOf(2000);
    private final OffsetDateTime _transactionDate;
    private final DisburseLoanFundsCommand cmd;
    private final Loan loan;

    public DisbursingFundsTest() {
        
        loan = new Loan(UUID.randomUUID());
        loan.ApplyEvent(new LoanCreatedEvent(_createdDate, new Duration(12, DurationUnit.Month), PaymentPlan.Weekly, _loanAmount));
        loan.ApplyEvent(new LoanCustomerContactChangedEvent("bob", "0444444444", "0812341234", "10 Random Street"));
        loan.ApplyEvent(new LoanBankAccountChangedEvent("066-000", "12345678"));


        _transactionDate = _createdDate.plus(1, ChronoUnit.HOURS);
        cmd = new DisburseLoanFundsCommand(UUID.randomUUID(), loan.id(), _transactionDate);
        loan.DisburseFunds(cmd);
    }

    @Test
    public void loanDisbursedFundsEventRaised() {
        LoanDisbursedFundsEvent actual = (LoanDisbursedFundsEvent)loan.GetUncommittedEvents()[0];
        assertEquals(_transactionDate, actual.transactionDate());
        assertEquals(_loanAmount.negate(), actual.amount());
    }

    @Test
    public void subsequenceAttemptsToDisburseThrow() {
        DisburseLoanFundsCommand cmd2 = new DisburseLoanFundsCommand(UUID.randomUUID(), loan.id(), _transactionDate);
        try {
            loan.DisburseFunds(cmd2);
            fail("Should reject duplicate calls to loan.DisburseFunds(..)");
        } catch (UnsupportedOperationException e) {
            assertEquals("Funds are already disbursed.", e.getMessage());
        }
    }
}