package com.leecampbell.cod.domain.contracts;

public final class Duration {
    private final int length;
    private final DurationUnit unit;

    public Duration(int length, DurationUnit unit) {
        this.length = length;
        this.unit = unit;
    }

    public int length() {
        return this.length;
    }

    public DurationUnit Unit() {
        return this.unit;
    }

    public String toString() { 
        return "Duration{length:" + this.length + ", unit:" + this.unit + "}";
    } 
}