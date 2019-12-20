package com.leecampbell.cod.application;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.UUID;

import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;
import com.leecampbell.cod.domain.services.*;

final class FakeRepository implements Repository {
    private final ArrayList<DomainEvent> committedEvents = new ArrayList<>();

    public Loan get(UUID id) {
        return new Loan(id);
    }

    public void save(Loan item) {
        List<DomainEvent> temp = Arrays.asList(item.getUncommittedEvents());
        committedEvents.addAll(temp);

        item.clearUncommittedEvents();
    }
}