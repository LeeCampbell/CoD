package com.leecampbell.cod.domain.contracts;

import java.util.UUID;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

final class Arg {
    public static void IsNotNullOrEmpty(String value, String argName){
        if (value == null || value.isEmpty())
            throw new IllegalArgumentException(String.format("%s cannot be null or empty", argName));
    }

    private static final UUID ZERO_UUID = new UUID(0, 0);
    public static void IsNotNullOrEmpty(UUID value, String argName){
        if (value == null || value.equals(ZERO_UUID))
            throw new IllegalArgumentException(String.format("%s cannot be null or empty", argName));
    }
    
    public static Matcher Matches(String value, Pattern regexPattern, String argName)
    {
        Matcher matcher = regexPattern.matcher(value);
        if (!matcher.matches())
            throw new IllegalArgumentException(String.format("%s is not valid", argName));
        return matcher;
    }
}