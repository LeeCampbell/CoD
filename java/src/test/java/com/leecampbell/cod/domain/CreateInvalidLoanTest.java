package com.leecampbell.cod.domain;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.fail;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.UUID;

import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;

import org.junit.Test;
import org.junit.experimental.theories.*;
import org.junit.runner.RunWith;

@RunWith(Theories.class)
public class CreateInvalidLoanTest {
    private Loan loan;
    private static final UUID validCommandId = UUID.randomUUID();
    private static final UUID validAggregateId = UUID.randomUUID();
    private final OffsetDateTime validCreatedOn = OffsetDateTime.of(2001, 2, 3, 4, 5, 6, 7, ZoneOffset.UTC);
    private final CustomerContact validCustomerContact = new CustomerContact("Jane Doe", "0412341234", "0856785678", "10 St Georges Terrace, Perth, WA 6000");
    private final BankAccount validBankAccount = new BankAccount("066-000", "12345678");
    private static final BigDecimal validAmount = BigDecimal.valueOf(1000);
    private static final Duration validTerm = new Duration(12, DurationUnit.Month);

    @DataPoints("invalidAmounts")
    public static final BigDecimal[] invalidSmallAmounts = { BigDecimal.valueOf(-1), BigDecimal.ZERO, BigDecimal.valueOf(49),BigDecimal.valueOf(2001),  };
    

    @Theory
    public void creatingLoanRejectsAmountValuesBelow50Above2000(@FromDataPoints("invalidAmounts") BigDecimal amount) {
        CreateLoanCommand cmd = new CreateLoanCommand(validCommandId, validAggregateId, validCreatedOn, validCustomerContact, validBankAccount, PaymentPlan.Weekly, amount, validTerm);
        loan = new Loan(cmd.AggregateId());
        try {
            loan.Create(cmd);
            fail("Create should throw when amount below 50 or above 2000.");
        } catch (UnsupportedOperationException e) {
            assertEquals("Only loan amounts between $50.00 and $2000.00 are supported.", e.getMessage());
        }
    }

    @Test
    public void rejectCreatingLoansOver24Months(){
        Duration term = new Duration(25, DurationUnit.Month);
        CreateLoanCommand cmd = new CreateLoanCommand(validCommandId, validAggregateId, validCreatedOn, validCustomerContact, validBankAccount, PaymentPlan.Weekly, validAmount, term);
        loan = new Loan(cmd.AggregateId());
        try {
            loan.Create(cmd);
            fail("Create should throw when term exceeds 25 months.");
        } catch (UnsupportedOperationException e) {
            assertEquals("Only loan terms up to 2 years are supported.", e.getMessage());
        }
    }

    
    @Test
    public void rejectCreatingLoansOver104Weeks(){
        Duration term = new Duration(105, DurationUnit.Week);
        CreateLoanCommand cmd = new CreateLoanCommand(validCommandId, validAggregateId, validCreatedOn, validCustomerContact, validBankAccount, PaymentPlan.Weekly, validAmount, term);
        loan = new Loan(cmd.AggregateId());
        try {
            loan.Create(cmd);
            fail("Create should throw when term exceeds 104 weeks.");
        } catch (UnsupportedOperationException e) {
            assertEquals("Only loan terms up to 2 years are supported.", e.getMessage());
        }
    }
}