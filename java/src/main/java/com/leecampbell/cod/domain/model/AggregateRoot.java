package com.leecampbell.cod.domain.model;

import com.leecampbell.cod.domain.contracts.DomainEvent;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.*;

public abstract class AggregateRoot {

    private final UUID id;
    private final List<DomainEvent> uncommittedEvents = new ArrayList<DomainEvent>();
    private int version;

    protected AggregateRoot(UUID id) {
        this.id = id;
    }

    public UUID id(){
        return id;
    }

    public int version() {
        return version;
    }

    public void ApplyEvent(DomainEvent payload) {
        try {
            Method handler = getMethod(payload);
            handler.setAccessible(true);
			handler.invoke(this, payload);
		} catch (IllegalAccessException e) {
			e.printStackTrace();
		} catch (InvocationTargetException e) {
			e.printStackTrace();
		} catch (NoSuchMethodException e) {
			e.printStackTrace();
        }
        
        version++;
    }

    public DomainEvent[] GetUncommittedEvents() {
        DomainEvent[] snapshot = new DomainEvent[uncommittedEvents.size()];
        snapshot = uncommittedEvents.toArray(snapshot);
        return snapshot;
    }

    public void ClearUncommittedEvents() {
        uncommittedEvents.clear();
    }
    
    protected void AddEvent(DomainEvent uncommittedEvent) {
        uncommittedEvents.add(uncommittedEvent);
        ApplyEvent(uncommittedEvent);
    }

    private Method getMethod(DomainEvent payload) throws NoSuchMethodException, SecurityException {
        Class<? extends AggregateRoot> rootType = this.getClass();
        Class<? extends DomainEvent> eventType = payload.getClass();

        return rootType.getDeclaredMethod("handle", eventType);
    }
}