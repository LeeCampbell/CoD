package com.leecampbell.cod.data;

import com.leecampbell.cod.domain.contracts.DomainEvent;
import com.leecampbell.cod.domain.model.Loan;
import com.leecampbell.cod.domain.services.Repository;

import java.sql.SQLException;
import java.util.UUID;

public final class PostgresqlRepository implements Repository {
    private final EventAppender writer;
    private final EventReader reader;

    public PostgresqlRepository(String connectionUrl){
        this.reader = new EventReader(connectionUrl);
        this.writer = new EventAppender(connectionUrl);
    }

    @Override
    public Loan get(UUID id) {
        try {
            var currentState = reader.readStreamInstance("loan", id.toString());
            var loan = new Loan(id);
            for (var event : currentState) {
                loan.applyEvent(event);
            }
            return loan;
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void save(Loan item) {
        try {
            var uncommittedEvents = item.getUncommittedEvents();
            int expectedVersion = item.getVersion() - uncommittedEvents.length;
            writer.append("loan", item.getId().toString(), expectedVersion, uncommittedEvents);
        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }
}

