package com.leecampbell.cod.domain.contracts;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public final class CustomerContact
{
    private static final Pattern PhoneNumberRegex = Pattern.compile("^0\\d{9}$");

    private final String name;
    private final String preferredPhoneNumber;
    private final String alternatePhoneNumber;
    private final String postalAddress;

    public CustomerContact(String name, String preferredPhoneNumber, String alternatePhoneNumber, String postalAddress)
    {
        Arg.IsNotNullOrEmpty(name, "name");
        Arg.IsNotNullOrEmpty(preferredPhoneNumber, "preferredPhoneNumber");
        Arg.IsNotNullOrEmpty(postalAddress, "postalAddress");           
        if(!isValidPhoneNumber(preferredPhoneNumber))
            throw new IllegalArgumentException("preferredPhoneNumber is not valid");
        if (alternatePhoneNumber!=null && !isValidPhoneNumber(alternatePhoneNumber))
            throw new IllegalArgumentException("alternatePhoneNumber is not valid");

        this.name = name;
        this.preferredPhoneNumber = preferredPhoneNumber;
        this.alternatePhoneNumber = alternatePhoneNumber;
        this.postalAddress = postalAddress;
    }

    public String name() { return this.name; }
    public String preferredPhoneNumber() { return this.preferredPhoneNumber; }
    public String alternatePhoneNumber() { return this.alternatePhoneNumber; }
    public String postalAddress() { return this.postalAddress; }

    private static boolean isValidPhoneNumber(String preferredPhoneNumber)
    {
        Matcher matcher = PhoneNumberRegex.matcher(preferredPhoneNumber);
        return matcher.matches();
    }
}