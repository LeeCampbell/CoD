package com.leecampbell.cod.domain;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNull;
import static org.junit.Assert.fail;

import com.leecampbell.cod.domain.contracts.CustomerContact;

import org.junit.Test;
import org.junit.experimental.theories.DataPoints;
import org.junit.experimental.theories.FromDataPoints;
import org.junit.experimental.theories.Theories;
import org.junit.experimental.theories.Theory;
import org.junit.runner.RunWith;

@RunWith(Theories.class)
public final class CustomerContactTests {
    @DataPoints("nullEmptyStrings")
    public static final String[] nullEmptyStrings = { "", null };

    @DataPoints("invalidPhoneNumbers")
    public static final String[] invalidPhoneNumbers = { "0", "1234", "04123456", "04123456789012" };

    @DataPoints("validPhoneNumbers")
    public static final String[] validPhoneNumbers = { "0444444444", "0412345678", "0212345678", "0812345678" };

    @Theory
    public void requiresCustomerName(@FromDataPoints("nullEmptyStrings") String value) {
        try {
            new CustomerContact(value, "0412341234", "0856785678", "10 St Georges Terrace, Perth, WA 6000");
            fail("CustomerContact should reject null or empty 'name' argument.");
        } catch (IllegalArgumentException e) {
            assertEquals("name cannot be null or empty", e.getMessage());
        }
    }

    @Theory
    public void requiresPreferredPhoneNumber(@FromDataPoints("nullEmptyStrings") String value) {
        try {
            new CustomerContact("Jane Doe", value, "0856785678", "10 St Georges Terrace, Perth, WA 6000");
            fail("CustomerContact should reject null or empty 'preferredPhoneNumber' argument.");
        } catch (IllegalArgumentException e) {
            assertEquals("preferredPhoneNumber cannot be null or empty", e.getMessage());
        }
    }

    @Theory
    public void rejectsInvalidFormatPreferredPhoneNumber(@FromDataPoints("invalidPhoneNumbers") String value) {
        try {
            new CustomerContact("Jane Doe", value, "0856785678", "10 St Georges Terrace, Perth, WA 6000");
            fail("CustomerContact should reject invalid preferredPhoneNumber argument.");
        } catch (IllegalArgumentException e) {
            assertEquals("preferredPhoneNumber is not valid", e.getMessage());
        }
    }

    @Theory
    public void allowsValidFormatPreferredPhoneNumber(@FromDataPoints("validPhoneNumbers") String expected) {
        CustomerContact actual = new CustomerContact("Jane Doe", expected, "0856785678",
                "10 St Georges Terrace, Perth, WA 6000");
        assertEquals(expected, actual.preferredPhoneNumber());
    }

    @Test
    public void allowsNullAlternatePhoneNumber() {
        CustomerContact actual = new CustomerContact("Jane Doe", "0412341234", null,
                "10 St Georges Terrace, Perth, WA 6000");
        assertNull(actual.alternatePhoneNumber());
    }

    @Theory
    public void allowsValidFormatAlternatePhoneNumber(@FromDataPoints("validPhoneNumbers") String expected) {
        CustomerContact actual = new CustomerContact("Jane Doe", "0412341234", expected,
                "10 St Georges Terrace, Perth, WA 6000");
        assertEquals(expected, actual.alternatePhoneNumber());
    }

    @Theory
    public void RequiresPostalAddress(@FromDataPoints("nullEmptyStrings") String value) {
        try {
            new CustomerContact("Jane Doe", "0412341234", "0856785678", value);
            fail("CustomerContact should reject null or empty 'postalAddress' argument.");
        } catch (IllegalArgumentException e) {
            assertEquals("postalAddress cannot be null or empty", e.getMessage());
        }
    }
}