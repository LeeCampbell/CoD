package com.leecampbell.cod.domain.contracts;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

public final class BankAccount {
    private static final Pattern BsbRegex = Pattern.compile("^(?<bank>\\d{3})-?(?<branch>\\d{3})$");
    private static final Pattern AccountNumberRegex = Pattern.compile("^\\d{3,12}$");

    private final String bsb;
    private final String accountNumber;

    public BankAccount(String bsb, String accountNumber) {
        Arg.IsNotNullOrEmpty(bsb, "bsb");
        Arg.IsNotNullOrEmpty(accountNumber, "accountNumber");        
        Arg.Matches(accountNumber, AccountNumberRegex, "accountNumber");
        Matcher bsbMatcher = Arg.Matches(bsb, BsbRegex, "bsb");

        String bank = bsbMatcher.group("bank");
        String branch = bsbMatcher.group("branch");
        this.bsb = String.format("%s-%s", bank, branch);
        this.accountNumber = accountNumber;
    }

    public String bsb() {
        return this.bsb;
    }

    public String accountNumber() {
        return this.accountNumber;
    }
}