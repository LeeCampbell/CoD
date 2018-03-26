package com.leecampbell.cod.domain;

import static org.junit.Assert.assertEquals;

import java.math.BigDecimal;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.time.temporal.ChronoUnit;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.UUID;

import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.services.DisburseLoanFundsCommandHandler;

import org.junit.Test;

public final class DisburseLoanFundsCommandHandlerTest extends CommandHandlerTestBase<DisburseLoanFundsCommand> {
    private final OffsetDateTime _createdDate = OffsetDateTime.of(2017, 06, 05, 04, 30, 20, 0, ZoneOffset.ofHours(10));
    private final BigDecimal _loanAmount = BigDecimal.valueOf(2000);
    private final String bankAccountBsb = "066-000";
    private final String bankAccountNumber = "12345678";

    public DisburseLoanFundsCommandHandlerTest() {
        super((repo) -> new DisburseLoanFundsCommandHandler(repo));
    }

    @Test
    public void loanDisbursedFundsEventRaised() {
        UUID loanId = UUID.randomUUID();
        given(() -> loanCreated(loanId));
        when(() -> disburseFunds(loanId));
        then((cmd, raisedEvents) -> {
            LoanDisbursedFundsEvent actual = (LoanDisbursedFundsEvent) raisedEvents.get(0);
            assertEquals(cmd.getTransactionDate(), actual.getTransactionDate());
            assertEquals(_loanAmount.negate(), actual.getAmount());
            assertEquals(bankAccountBsb, actual.getDisbursedTo().getBsb());
            assertEquals(bankAccountNumber, actual.getDisbursedTo().getAccountNumber());
        });
    }

    @Test
    public void receiptIsReturned() {
        UUID loanId = UUID.randomUUID();
        given(() -> loanCreated(loanId));
        when(() -> disburseFunds(loanId));
        assertEquals(loanId, getReceipt().getAggregateId());
        assertEquals(4, getReceipt().getVersion());
    }

    @Test
    public void subsequenceAttemptsToDisburseThrow() {
        UUID loanId = UUID.randomUUID();
        given(() -> loanDisbursed(loanId));
        when(() -> disburseFunds(loanId));
        thenThrew(UnsupportedOperationException.class, (cmd, ex) -> {
            assertEquals("Funds are already disbursed.", ex.getMessage());
        });
    }

    private List<EventEnvelope> loanCreated(UUID aggregateId) {
        List<DomainEvent> payloads = Arrays.asList(
                new LoanCreatedEvent(OffsetDateTime.of(2017, 06, 05, 04, 30, 20, 0, ZoneOffset.ofHours(10)),
                        new Duration(12, DurationUnit.Month), PaymentPlan.Weekly, _loanAmount),
                new LoanCustomerContactChangedEvent("bob", "0444444444", "0812341234", "10 Random Street"),
                new LoanBankAccountChangedEvent(bankAccountBsb, bankAccountNumber));

        //TODO: Any ideas on how to make this just `return payloads.map((idx, i)->new StubEventEnvelope(idx, "Loan", aggregateId.toString(), idx, i));`
        ArrayList<EventEnvelope> envelopes = new ArrayList<>();
        int idx = 0;
        for (DomainEvent payload : payloads) {
            idx++;
            envelopes.add(new StubEventEnvelope(idx, "Loan", aggregateId.toString(), idx, payload));
        }

        return envelopes;
    }

    private Iterable<EventEnvelope> loanDisbursed(UUID aggregateId) {
        List<EventEnvelope> events = loanCreated(aggregateId);
        LoanCreatedEvent createdEvent = (LoanCreatedEvent) events.get(0).getPayload();
        LoanBankAccountChangedEvent bankEvent = (LoanBankAccountChangedEvent) events.get(2).getPayload();
        EventEnvelope last = events.get(events.size() - 1);

        events.add(new StubEventEnvelope(last.getSequenceId() + 1, last.getStreamName(), last.getStreamId(),
                last.getVersion() + 1,
                new LoanDisbursedFundsEvent(createdEvent.getCreatedOn().plus(1, ChronoUnit.HOURS), _loanAmount,
                        bankEvent.getBankAccount())));

        return events;
    }

    private DisburseLoanFundsCommand disburseFunds(UUID loanId) {
        OffsetDateTime transactionDate = _createdDate.plus(1, ChronoUnit.HOURS);
        return new DisburseLoanFundsCommand(UUID.randomUUID(), loanId, transactionDate);
    }
}