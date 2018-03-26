package com.leecampbell.cod.domain.contracts;

public interface EventEnvelope {
    /**
     * @return the sequenceId
     */
    long getSequenceId();

    /**
     * @return the streamName
     */
    String getStreamName();

    /**
     * @return the streamId
     */
    String getStreamId();

    /**
     * @return the version
     */
    int getVersion();

    /**
     * @return the payload
     */
    DomainEvent getPayload();
}