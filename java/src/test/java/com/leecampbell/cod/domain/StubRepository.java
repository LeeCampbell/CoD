package com.leecampbell.cod.domain;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.UUID;

import com.leecampbell.cod.domain.contracts.DomainEvent;
import com.leecampbell.cod.domain.model.Loan;
import com.leecampbell.cod.domain.services.Repository;;

public class StubRepository implements Repository {
    private final HashMap<UUID, Loan> items = new HashMap<>();
    private final ArrayList<DomainEvent> committedEvents = new ArrayList<>();

    public List<DomainEvent> CommitedEvents() {
        return Collections.unmodifiableList(committedEvents);
    }

    public Loan Get(UUID id) {
        Loan item;
        if (!items.containsKey(id)) {
            item = new Loan(id);
            items.put(id, item);
        }
        return items.get(id);
    }

    public void Save(Loan item) {
        DomainEvent[] events = item.GetUncommittedEvents();
        for (DomainEvent event : events) {
            committedEvents.add(event);
        }
        item.ClearUncommittedEvents();
    }
}