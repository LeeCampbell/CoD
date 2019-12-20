package com.leecampbell.cod.data;

/**
 * Unchecked exception thrown when an attempt is made to write to a stream with the incorrect version.
 */
public class WrongExpectedVersionException extends RuntimeException {
    public WrongExpectedVersionException(String streamType, String streamId, int currentVersion, int expectedVersion) {
        super(String.format("Expected stream ('%s', '%s') to be at version %d, but was at %d.", streamType, streamId, expectedVersion, currentVersion));
    }

}
