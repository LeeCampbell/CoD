package com.leecampbell.cod.domain;

import com.leecampbell.cod.domain.contracts.DomainEvent;
import com.leecampbell.cod.domain.contracts.EventEnvelope;

/**
 * StubEventEnvelope
 */
public class StubEventEnvelope implements EventEnvelope {
    private final long sequenceId;
    private final String streamName;
    private final String streamId;
    private final int version;
    private final DomainEvent payload;

    public StubEventEnvelope(long sequenceId, String streamName, String streamId, int version, DomainEvent payload) {
        this.sequenceId = sequenceId;
        this.streamName = streamName;
        this.streamId = streamId;
        this.version = version;
        this.payload = payload;
    }

    /**
     * @return the sequenceId
     */
    public long getSequenceId() {
        return sequenceId;
    }

    /**
     * @return the streamName
     */
    public String getStreamName() {
        return streamName;
    }

    /**
     * @return the streamId
     */
    public String getStreamId() {
        return streamId;
    }

    /**
     * @return the version
     */
    public int getVersion() {
        return version;
    }

    /**
     * @return the payload
     */
    public DomainEvent getPayload() {
        return payload;
    }
    
}