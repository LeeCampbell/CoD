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

    public UUID getId(){
        return id;
    }

    public int getVersion() {
        return version;
    }

    public void applyEvent(DomainEvent payload) {
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

    public DomainEvent[] getUncommittedEvents() {
        DomainEvent[] snapshot = new DomainEvent[uncommittedEvents.size()];
        snapshot = uncommittedEvents.toArray(snapshot);
        return snapshot;
    }

    public void clearUncommittedEvents() {
        uncommittedEvents.clear();
    }
    
    protected void addEvent(DomainEvent uncommittedEvent) {
        uncommittedEvents.add(uncommittedEvent);
        applyEvent(uncommittedEvent);
    }

    private Method getMethod(DomainEvent payload) throws NoSuchMethodException, SecurityException {
        Class<? extends AggregateRoot> rootType = this.getClass();
        Class<? extends DomainEvent> eventType = payload.getClass();

        return rootType.getDeclaredMethod("handle", eventType);
    }
}