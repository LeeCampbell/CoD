package com.leecampbell.cod.domain;

import java.util.ArrayList;
import java.util.List;
import org.junit.experimental.theories.ParameterSignature;
import org.junit.experimental.theories.ParameterSupplier;
import org.junit.experimental.theories.PotentialAssignment;

public final class BsbSupplier extends ParameterSupplier {
    public BsbSupplier(){

    }
    @Override
    public List<PotentialAssignment> getValueSources(ParameterSignature signature) {
        ArrayList<PotentialAssignment> result = new ArrayList<PotentialAssignment>();
        result.add(PotentialAssignment.forValue("066000", new Pair<>("066000", "066-000")));
        result.add(PotentialAssignment.forValue("123456", new Pair<>("123456", "123-456")));
        result.add(PotentialAssignment.forValue("066-000", new Pair<>("066-000", "066-000")));
        result.add(PotentialAssignment.forValue("123-456", new Pair<>("123-456", "123-456")));
        return result;
    }
}