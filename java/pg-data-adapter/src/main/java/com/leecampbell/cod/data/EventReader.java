package com.leecampbell.cod.data;

import com.leecampbell.cod.domain.contracts.DomainEvent;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;

final class EventReader {
    private static final String SELECT_SQL_BY_STREAM_ID =
            "SELECT event_type, payload FROM event_store.events WHERE stream_type = (?) AND stream_id = (?) ORDER BY version ASC";

    private final String connectionUrl;

    public EventReader(String connectionUrl) {
        this.connectionUrl = connectionUrl;
    }

    final List<? extends DomainEvent> readStreamInstance(
            String streamType, String streamId) throws SQLException, ClassNotFoundException {
        try (Connection conn = DriverManager.getConnection(connectionUrl)) {
            try (PreparedStatement statement = conn.prepareStatement(SELECT_SQL_BY_STREAM_ID)) {
                statement.setString(1, streamType);
                statement.setString(2, streamId);
                try (ResultSet rs = statement.executeQuery()) {
                    return mapToEvents(rs);
                }
            }
        }
    }

    private List<? extends DomainEvent> mapToEvents(ResultSet resultSet) throws SQLException, ClassNotFoundException {
        List<DomainEvent> output = new ArrayList<>();
        while (resultSet.next()) {
            String eventType = resultSet.getString(1);
            String payload = resultSet.getString(2);

            Class cls = Class.forName(eventType);
            DomainEvent evt = (DomainEvent) GsonUtils.deserialize(payload, cls);

            output.add(evt);
        }

        return output;
    }
}
