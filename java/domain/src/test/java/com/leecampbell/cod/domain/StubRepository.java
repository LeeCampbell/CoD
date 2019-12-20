package com.leecampbell.cod.domain;

import java.util.*;

import com.leecampbell.cod.domain.contracts.*;
import com.leecampbell.cod.domain.model.Loan;
import com.leecampbell.cod.domain.services.Repository;

public class StubRepository implements Repository {
    private final HashMap<UUID, Loan> items = new HashMap<>();
    private final ArrayList<EventEnvelope> storedEvents = new ArrayList<>();
    private final ArrayList<DomainEvent> committedEvents = new ArrayList<>();

    public void Load(Iterable<EventEnvelope> events){
        for (EventEnvelope event : events) {
            storedEvents.add(event);
        }
    }
    
    public List<DomainEvent> CommittedEvents() {
        return Collections.unmodifiableList(committedEvents);
    }

    public Loan get(UUID id) {
        Loan item;
        if (!items.containsKey(id)) {
            item = new Loan(id);
            String streamId = id.toString();
            for (EventEnvelope event : storedEvents) {
                if(event.getStreamName().equals("Loan") && event.getStreamId().equals(streamId)){
                    item.applyEvent(event.getPayload());
                }
            }
            
            items.put(id, item);
        }
        return items.get(id);
    }

    public void save(Loan item) {
        DomainEvent[] events = item.getUncommittedEvents();
        committedEvents.addAll(Arrays.asList(events));
        item.clearUncommittedEvents();
    }
}