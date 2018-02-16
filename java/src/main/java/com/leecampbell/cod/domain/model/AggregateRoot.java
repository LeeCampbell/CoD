package com.leecampbell.cod.domain.model;

import com.leecampbell.cod.domain.contracts.DomainEvent;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.*;

public abstract class AggregateRoot {

    private final UUID id;
    private final List<DomainEvent> uncommittedEvents = new ArrayList<DomainEvent>();
    //private static Map<String, Method> mutatorMethods = new HashMap<String, Method>();
    private int version;

    protected AggregateRoot(UUID id) {
        this.id = id;
    }

    public UUID id(){
        return id;
    }

    public void ApplyEvent(DomainEvent payload) {
        Class<? extends AggregateRoot> rootType = this.getClass();
        Class<? extends DomainEvent> eventType = payload.getClass();
        try {
            Method handler = getMethod(payload);
            handler.setAccessible(true);
			handler.invoke(this, payload);
		} catch (IllegalAccessException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IllegalArgumentException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (InvocationTargetException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (NoSuchMethodException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (SecurityException e) {
			// TODO Auto-generated catch block
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

    protected int version() {
        return version;
    }

    protected void AddEvent(DomainEvent uncommittedEvent) {
        uncommittedEvents.add(uncommittedEvent);
        ApplyEvent(uncommittedEvent);
    }

    private Method getMethod(DomainEvent payload) throws NoSuchMethodException, SecurityException {
        Class<? extends AggregateRoot> rootType = this.getClass();
        Class<? extends DomainEvent> eventType = payload.getClass();

        Method method = null;

        try {

            // assume protected or private...

            method = rootType.getDeclaredMethod("handle", eventType);

        } catch (Exception e) {

            // then public...

            method = rootType.getMethod("handle", eventType);
        }

        return method;
    }
}