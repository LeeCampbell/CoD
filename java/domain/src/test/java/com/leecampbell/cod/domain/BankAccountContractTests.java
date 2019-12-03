package com.leecampbell.cod.domain;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.fail;

import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;

import com.leecampbell.cod.domain.contracts.BankAccount;

import org.junit.experimental.theories.DataPoints;
import org.junit.experimental.theories.FromDataPoints;
import org.junit.experimental.theories.ParametersSuppliedBy;
import org.junit.experimental.theories.Theories;
import org.junit.experimental.theories.Theory;
import org.junit.runner.RunWith;

@RunWith(Theories.class)
public class BankAccountContractTests {
    @DataPoints("nullEmptyStrings")
    public static final String[] nullEmptyStrings = { "", null };

    @DataPoints("invalidBsb")
    public static final String[] invalidBsb = { "Abc-123", "Abc123", "00600", "066-00", "066-0000", "0066-000" };
    @DataPoints("invalidAccountNumber")
    public static final String[] invalidAccountNumber = { "1234567890123", //Too long
            " 123", //Spaces
            "1",    //Minimum of 3
            "12",   //Minimum of 3
            "123a", "a123" };

    @DataPoints("validAccountNumber")
    public static final String[] validAccountNumber = { "123", "123", "123456", "123456789012" };

    @Theory
    public void requiresBsb(@FromDataPoints("nullEmptyStrings") String value) {
        try {
            new BankAccount(value, "12345678");
            fail("BankAccount should reject null or empty bsb value");
        } catch (IllegalArgumentException e) {
            assertEquals("bsb cannot be null or empty", e.getMessage());
        }
    }

    @Theory
    public void rejectsInvalidBsb(@FromDataPoints("invalidBsb") String value) {
        try {
            new BankAccount(value, "12345678");
            fail("BankAccount should reject invalid bsb value");
        } catch (IllegalArgumentException e) {
            assertEquals("bsb is not valid", e.getMessage());
        }
    }

    @Retention(RetentionPolicy.RUNTIME)
    @ParametersSuppliedBy(BsbSupplier.class)
    public @interface validBsbValues {
    }

    @Theory
    public void acceptsValidBsb(@validBsbValues Pair<String, String> inputExpectedPair) {
        BankAccount actual = new BankAccount(inputExpectedPair.getLeft(), "12345678");
        assertEquals(inputExpectedPair.getRight(), actual.getBsb());
    }

    @Theory
    public void requiresAccountNumber(@FromDataPoints("nullEmptyStrings") String value) {
        try {
            new BankAccount("066-000", value);
            fail("BankAccount should reject null or empty accountNumber value");
        } catch (IllegalArgumentException e) {
            assertEquals("accountNumber cannot be null or empty", e.getMessage());
        }
    }

    @Theory
    public void rejectsInvalidAccountNumber(@FromDataPoints("invalidAccountNumber") String value) {
        try {
            new BankAccount("066-000", value);
            fail("BankAccount should reject invalid accountNumber value");
        } catch (IllegalArgumentException e) {
            assertEquals("accountNumber is not valid", e.getMessage());
        }
    }

    @Theory
    public void acceptsValidAccountNumber(@FromDataPoints("validAccountNumber") String value) {
        BankAccount actual = new BankAccount("066-000", value);
        assertEquals(value, actual.getAccountNumber());
    }
}