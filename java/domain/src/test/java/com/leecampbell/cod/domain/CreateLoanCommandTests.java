package com.leecampbell.cod.domain;

import com.leecampbell.cod.domain.contracts.BankAccount;
import com.leecampbell.cod.domain.contracts.CreateLoanCommand;
import com.leecampbell.cod.domain.contracts.CustomerContact;
import com.leecampbell.cod.domain.contracts.Duration;
import com.leecampbell.cod.domain.contracts.DurationUnit;
import com.leecampbell.cod.domain.contracts.PaymentPlan;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.fail;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.util.UUID;
import org.junit.Test;
import org.junit.experimental.theories.*;
import org.junit.runner.RunWith;

@RunWith(Theories.class)
public final class CreateLoanCommandTests {
    @DataPoints("nullEmptyUuids")
    public static final UUID[] nullEmptyUuids = { null, new UUID(0,0) };

    private static final UUID ValidCommandId = UUID.randomUUID();
    private static final UUID ValidAggregateId = UUID.randomUUID();
    private static final OffsetDateTime ValidCreatedOn = OffsetDateTime.now();
    private static final CustomerContact ValidCustomerContact = new CustomerContact("Bob Fuller", "0412345678", null, "10 Wayward ave");
    private static final BankAccount ValidBankAccount = new BankAccount("066-000", "12345678");
    private static final Duration ValidDuration = new Duration(12, DurationUnit.Month);
    private static final BigDecimal ValidLoanAmount = BigDecimal.valueOf(1000);
    private static final PaymentPlan ValidPaymentPlan = PaymentPlan.Weekly;

    @Theory
    public void rejectsZeroValueForCommandId(@FromDataPoints("nullEmptyUuids") UUID id) {
        try {
            new CreateLoanCommand(id, 
                ValidAggregateId, 
                ValidCreatedOn, 
                ValidCustomerContact,
                ValidBankAccount, 
                ValidPaymentPlan, 
                ValidLoanAmount, 
                ValidDuration);
            fail("CreateLoanCommand should reject null value for commandId");
        } catch (IllegalArgumentException e) {
            assertEquals("commandId cannot be null or empty", e.getMessage());
        }
    }

    @Theory
    public void rejectsZeroValueForAggregateId(@FromDataPoints("nullEmptyUuids") UUID id) {
        try {
            new CreateLoanCommand(ValidCommandId, 
                id, 
                ValidCreatedOn, 
                ValidCustomerContact,
                ValidBankAccount, 
                ValidPaymentPlan, 
                ValidLoanAmount, 
                ValidDuration);
            fail("CreateLoanCommand should reject null value for aggregateId");
        } catch (IllegalArgumentException e) {
            assertEquals("aggregateId cannot be null or empty", e.getMessage());
        }
    }

    @Test
    public void rejectsNullCreatedOn()
    {
        try {
            new CreateLoanCommand(ValidCommandId, 
                ValidAggregateId, 
                null,
                ValidCustomerContact,
                ValidBankAccount, 
                ValidPaymentPlan, 
                ValidLoanAmount, 
                ValidDuration);
            fail("CreateLoanCommand should reject null value for createdOn argument");
        } catch (IllegalArgumentException e) {
            assertEquals("createdOn cannot be null", e.getMessage());
        }
    }

    @Test
    public void rejectsNullCustomerContact()
    {
        try {
            new CreateLoanCommand(ValidCommandId, 
                ValidAggregateId, 
                ValidCreatedOn,
                null,
                ValidBankAccount, 
                ValidPaymentPlan, 
                ValidLoanAmount, 
                ValidDuration);
            fail("CreateLoanCommand should reject null value for customerContact argument");
        } catch (IllegalArgumentException e) {
            assertEquals("customerContact cannot be null", e.getMessage());
        }
    }

    @Test
    public void rejectsNullBankAccount()
    {
        try {
            new CreateLoanCommand(ValidCommandId, 
                ValidAggregateId, 
                ValidCreatedOn,
                ValidCustomerContact,
                null, 
                ValidPaymentPlan, 
                ValidLoanAmount, 
                ValidDuration);
            fail("CreateLoanCommand should reject null value for bankAccount argument");
        } catch (IllegalArgumentException e) {
            assertEquals("bankAccount cannot be null", e.getMessage());
        }
    }

    @Test
    public void rejectsZeroValueForPaymentPlan()
    {
        try {
            new CreateLoanCommand(ValidCommandId, 
                ValidAggregateId, 
                ValidCreatedOn,
                ValidCustomerContact,
                ValidBankAccount, 
                PaymentPlan.None, 
                ValidLoanAmount, 
                ValidDuration);
            fail("CreateLoanCommand should reject zero/None value for paymentPlan argument");
        } catch (IllegalArgumentException e) {
            assertEquals("paymentPlan cannot be None", e.getMessage());
        }
    }
    @Test
    public void rejectsNullForAmount()
    {
        try {
            new CreateLoanCommand(ValidCommandId, 
                ValidAggregateId, 
                ValidCreatedOn,
                ValidCustomerContact,
                ValidBankAccount, 
                ValidPaymentPlan, 
                null, 
                ValidDuration);
            fail("CreateLoanCommand should reject null value for amount argument");
        } catch (IllegalArgumentException e) {
            assertEquals("amount cannot be null", e.getMessage());
        }
    }

    @Test
    public void rejectsNullForTerm()
    {
        try {
            new CreateLoanCommand(ValidCommandId, 
                ValidAggregateId, 
                ValidCreatedOn,
                ValidCustomerContact,
                ValidBankAccount, 
                ValidPaymentPlan, 
                ValidLoanAmount, 
                null);
            fail("CreateLoanCommand should reject null value for term argument");
        } catch (IllegalArgumentException e) {
            assertEquals("term cannot be null", e.getMessage());
        }
    }
}