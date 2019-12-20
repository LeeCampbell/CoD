package com.leecampbell.cod.domain;

import static org.junit.Assert.assertEquals;
import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.Arrays;
import java.util.Collections;
import java.util.UUID;

import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;
import com.leecampbell.cod.domain.services.CreateLoanCommandHandler;

import org.junit.Assume;
import org.junit.Test;
import org.junit.experimental.theories.*;
import org.junit.runner.RunWith;

@RunWith(Theories.class)
public final class CreateLoanCommandHandlerTests extends CommandHandlerTestBase<CreateLoanCommand> {

    private static final UUID validCommandId = UUID.randomUUID();
    private static final UUID validAggregateId = UUID.randomUUID();
    private final OffsetDateTime validCreatedOn = OffsetDateTime.of(2001, 2, 3, 4, 5, 6, 7, ZoneOffset.UTC);
    private final CustomerContact validCustomerContact = new CustomerContact("Jane Doe", "0412341234", "0856785678",
            "10 St Georges Terrace, Perth, WA 6000");
    private final BankAccount validBankAccount = new BankAccount("066-000", "12345678");
    private static final BigDecimal validAmount = BigDecimal.valueOf(1000);
    private static final Duration validTerm = new Duration(12, DurationUnit.Month);

    public CreateLoanCommandHandlerTests() {
        super(CreateLoanCommandHandler::new);
    }

    @DataPoints("validDurations")
    public static final Duration[] validDurations = { new Duration(1, DurationUnit.Day),
            new Duration(729, DurationUnit.Day), new Duration(1, DurationUnit.Week),
            new Duration(103, DurationUnit.Week), new Duration(1, DurationUnit.Month),
            new Duration(23, DurationUnit.Month) };

    @Theory
    public void loanCreatedEventIsRaised(@FromDataPoints("validDurations") Duration term) {
        given(Collections::emptyList);
        when(() -> createLoan(term));
        then((cmd, raisedEvents) -> {
            LoanCreatedEvent actual = (LoanCreatedEvent) raisedEvents.get(0);
            assertEquals(cmd.createdOn(), actual.getCreatedOn());
            assertEquals(cmd.amount(), actual.getAmount());
            assertEquals(cmd.term(), actual.getTerm());
            assertEquals(cmd.paymentPlan(), actual.getPaymentPlan());
        });
    }

    @Test
    public void loanCustomerContactChangedEventRaised() {
        given(Collections::emptyList);
        when(this::createLoan);
        then((cmd, raisedEvents) -> {
            LoanCustomerContactChangedEvent actual = (LoanCustomerContactChangedEvent) raisedEvents.get(1);
            assertEquals(cmd.customerContact().name(), actual.customerContact().name());
            assertEquals(cmd.customerContact().preferredPhoneNumber(), actual.customerContact().preferredPhoneNumber());
            assertEquals(cmd.customerContact().alternatePhoneNumber(), actual.customerContact().alternatePhoneNumber());
            assertEquals(cmd.customerContact().postalAddress(), actual.customerContact().postalAddress());
        });
    }

    @Test
    public void loanBankAccountChangedEventRaised() {
        given(Collections::emptyList);
        when(this::createLoan);
        then((cmd, raisedEvents) -> {
            LoanBankAccountChangedEvent actual = (LoanBankAccountChangedEvent) raisedEvents.get(2);
            assertEquals(cmd.bankAccount().getBsb(), actual.getBankAccount().getBsb());
            assertEquals(cmd.bankAccount().getAccountNumber(), actual.getBankAccount().getAccountNumber());
        });
    }

    @Test
    public void receiptIsReturned() {
        given(Collections::emptyList);
        when(this::createLoan);
        assertEquals(getCommand().getAggregateId(), getReceipt().getAggregateId());
        assertEquals(3, getReceipt().getVersion());
    }

    @Test
    public void subsequentCallsToCreateThrows() {
        given(() -> Arrays.asList(createdLoan(validAggregateId)));
        when(this::createLoan);
        thenThrew(UnsupportedOperationException.class, (cmd, ex) -> assertEquals("Loan already created.", ex.getMessage()));
    }

    //TODO: Move to an AggregateRootBase test? -LC
    @Test
    public void commitClearsUncommittedEvents() {
        CreateLoanCommand cmd = createLoan();
        Loan loan = new Loan(cmd.getAggregateId());
        loan.create(cmd);
        Assume.assumeTrue(loan.getUncommittedEvents().length > 0);

        loan.clearUncommittedEvents();
        DomainEvent[] events = loan.getUncommittedEvents();

        assertEquals(0, events.length);
    }

    @DataPoints("invalidAmounts")
    public static final BigDecimal[] invalidAmounts = { BigDecimal.valueOf(-1), BigDecimal.ZERO, BigDecimal.valueOf(49),
            BigDecimal.valueOf(2001), };

    @Theory
    public void creatingLoanRejectsAmountValuesBelow50Above2000(@FromDataPoints("invalidAmounts") BigDecimal amount) {
        given(Collections::emptyList);
        when(() -> createLoan(amount));
        thenThrew(UnsupportedOperationException.class, (cmd, ex) -> assertEquals("Only loan amounts between $50.00 and $2000.00 are supported.", ex.getMessage()));
    }

    @DataPoints("overlimitDurations")
    public static final Duration[] overlimitDurations = { new Duration(730, DurationUnit.Day),
            new Duration(105, DurationUnit.Week), new Duration(25, DurationUnit.Month) };

    @Theory
    public void rejectCreatingLoansOver2years(@FromDataPoints("overlimitDurations") Duration term) {
        given(Collections::emptyList);
        when(() -> createLoan(term));
        thenThrew(UnsupportedOperationException.class, (cmd, ex) -> assertEquals("Only loan terms up to 2 years are supported.", ex.getMessage()));
    }

    @Test
    public void rejectCreatingLoansWithUnknownDuration() {
        given(Collections::emptyList);
        when(() -> createLoan(new Duration(1, DurationUnit.None)));
        thenThrew(IllegalArgumentException.class, (cmd, ex) -> assertEquals("Unsupported duration", ex.getMessage()));
    }

    private CreateLoanCommand createLoan() {
        return new CreateLoanCommand(validCommandId, validAggregateId, validCreatedOn, validCustomerContact,
                validBankAccount, PaymentPlan.Weekly, validAmount, validTerm);
    }

    private CreateLoanCommand createLoan(Duration term) {
        return new CreateLoanCommand(validCommandId, validAggregateId, validCreatedOn, validCustomerContact,
                validBankAccount, PaymentPlan.Weekly, validAmount, term);
    }

    private CreateLoanCommand createLoan(BigDecimal amount) {
        return new CreateLoanCommand(validCommandId, validAggregateId, validCreatedOn, validCustomerContact,
                validBankAccount, PaymentPlan.Weekly, amount, validTerm);
    }

    private EventEnvelope createdLoan(UUID aggregateId) {
        LoanCreatedEvent payload = new LoanCreatedEvent(validCreatedOn, validTerm, PaymentPlan.Weekly, validAmount);
        return new StubEventEnvelope(1, "Loan", aggregateId.toString(), 1, payload);
    }
}