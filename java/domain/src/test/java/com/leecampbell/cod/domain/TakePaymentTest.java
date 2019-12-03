package com.leecampbell.cod.domain;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.time.temporal.ChronoUnit;
import java.util.UUID;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertThat;
import static org.junit.Assert.fail;

import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;

import org.hamcrest.core.IsInstanceOf;
import org.junit.Test;
import org.junit.experimental.theories.DataPoints;
import org.junit.experimental.theories.FromDataPoints;
import org.junit.experimental.theories.Theories;
import org.junit.experimental.theories.Theory;
import org.junit.runner.RunWith;

@RunWith(Theories.class)
public final class TakePaymentTest {
    private static final OffsetDateTime CreatedDate = OffsetDateTime.of(2001, 2, 3, 4, 5, 6, 7, ZoneOffset.UTC);
    private static final BigDecimal LoanAmount = BigDecimal.valueOf(2000);
    private static final OffsetDateTime ValidTransactionDate = CreatedDate.plus(5, ChronoUnit.DAYS);
    private static final BigDecimal ValidPaymentAmount = BigDecimal.valueOf(123.45);

    @DataPoints("invalidAmounts")
    public static final BigDecimal[] invalidAmounts = { BigDecimal.valueOf(-1), BigDecimal.ZERO };

    private final Loan loan;

    public TakePaymentTest() {

        loan = new Loan(UUID.randomUUID());
        loan.applyEvent(new LoanCreatedEvent(CreatedDate, new Duration(12, DurationUnit.Month), PaymentPlan.Weekly,
                LoanAmount));
        loan.applyEvent(new LoanCustomerContactChangedEvent("bob", "0444444444", "0812341234", "10 Random Street"));
        loan.applyEvent(new LoanBankAccountChangedEvent("066-000", "12345678"));
        loan.applyEvent(new LoanDisbursedFundsEvent(CreatedDate.plus(1, ChronoUnit.HOURS), LoanAmount.negate(),
                new BankAccount("066-000", "12345678")));
    }

    @Test
    public void takingPaymentOnUnCreatedLoanThrows() {
        Loan uncreatedLoan = new Loan(UUID.randomUUID());
        TakePaymentCommand cmd = new TakePaymentCommand(UUID.randomUUID(), loan.getId(), ValidPaymentAmount,
                ValidTransactionDate);
        try {
            uncreatedLoan.takePayment(cmd);
            fail("Should reject takePayment on uncreated loan.");
        } catch (UnsupportedOperationException e) {
            assertEquals("Take payment attempted on an uncreated loan.", e.getMessage());
        }
    }

    @Test
    public void takingPaymentBeforeCreationDateThrows() {
        TakePaymentCommand cmd = new TakePaymentCommand(UUID.randomUUID(), loan.getId(), ValidPaymentAmount,
                CreatedDate.plus(-1, ChronoUnit.SECONDS));
        try {
            loan.takePayment(cmd);
            fail("Should reject takePayment on prior to creation date.");
        } catch (UnsupportedOperationException e) {
            assertEquals("Transaction date can not be prior to loan creation.", e.getMessage());
        }
    }

    @Theory
    public void takingPaymentOfZeroOrNegativeAmountThrows(@FromDataPoints("invalidAmounts") BigDecimal amount) {
        TakePaymentCommand cmd = new TakePaymentCommand(UUID.randomUUID(), loan.getId(), amount, ValidTransactionDate);
        try {
            loan.takePayment(cmd);
            fail("Should reject takePayment with zero or negative amount.");
        } catch (UnsupportedOperationException e) {
            assertEquals("Transaction amount must be positive.", e.getMessage());
        }
    }

    @Test
    public void takingAPaymentRaisedPaymentTakenEvent() {
        TakePaymentCommand cmd = new TakePaymentCommand(UUID.randomUUID(), loan.getId(), ValidPaymentAmount, ValidTransactionDate);
        loan.takePayment(cmd);

        assertEquals(1, loan.getUncommittedEvents().length);
        PaymentTakenEvent paymentTakenEvent = (PaymentTakenEvent)loan.getUncommittedEvents()[0];
        assertNotNull(paymentTakenEvent.transactionId());
        assertEquals(ValidTransactionDate, paymentTakenEvent.transactionDateTime());
        assertEquals(ValidPaymentAmount, paymentTakenEvent.amount());

    }

    @Test
    public void takingPaymentForLoanBalanceRaisesLoanSettledEvent(){
        TakePaymentCommand cmd = new TakePaymentCommand(UUID.randomUUID(), loan.getId(), LoanAmount, ValidTransactionDate);
        loan.takePayment(cmd);

        assertEquals(2, loan.getUncommittedEvents().length);
        assertThat("Expecting first event to be PaymentTakenEvent", loan.getUncommittedEvents()[0], IsInstanceOf.instanceOf(PaymentTakenEvent.class));
                
        LoanSettledEvent loanSettledEvent = (LoanSettledEvent)loan.getUncommittedEvents()[1];
        assertEquals(ValidTransactionDate, loanSettledEvent.getTransactionDateTime());
    }

    @Test
    public void takingPaymentForMoreThanLoanBalanceRaisesLoanSettledEvent(){
        BigDecimal overPaidBy = BigDecimal.valueOf(1);
        TakePaymentCommand cmd = new TakePaymentCommand(UUID.randomUUID(), loan.getId(), LoanAmount.add(overPaidBy), ValidTransactionDate);
        loan.takePayment(cmd);

        assertEquals(3, loan.getUncommittedEvents().length);
        assertThat("Expecting first event to be PaymentTakenEvent", loan.getUncommittedEvents()[0], IsInstanceOf.instanceOf(PaymentTakenEvent.class));
        assertThat("Expecting second event to be LoanSettledEvent", loan.getUncommittedEvents()[1], IsInstanceOf.instanceOf(LoanSettledEvent.class));
                
        LoanOverPaidEvent loanOverPaidEvent = (LoanOverPaidEvent)loan.getUncommittedEvents()[2];
        assertEquals(ValidTransactionDate, loanOverPaidEvent.getTransactionDateTime());
        assertEquals(overPaidBy, loanOverPaidEvent.getAmount());
    }
}