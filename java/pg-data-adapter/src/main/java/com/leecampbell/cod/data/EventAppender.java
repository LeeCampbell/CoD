package com.leecampbell.cod.data;

import com.leecampbell.cod.domain.contracts.DomainEvent;
import org.postgresql.util.PGobject;

import java.sql.*;

final class EventAppender {

    private static final String INSERT_SQL =
            "INSERT INTO event_store.events "
                    + "(stream_type, stream_id, version, event_type, payload) "
                    + "VALUES "
                    + "(?, ?, ?, ?, ?)";

    private static final String SQL_SELECT_MAX_STREAM_VERSION =
            "SELECT coalesce(MAX(version), 0) FROM event_store.events WHERE stream_type = (?) AND stream_id = (?)";

    private static final String DUPLICATE_VERSION_SQL_EXCEPTION_MESSAGE =
            "Error in executeUpdate, ERROR: duplicate key value violates unique constraint \"events_pkey\"";

    private final String connectionUrl;

    EventAppender(String connectionUrl) {
        this.connectionUrl = connectionUrl;
    }

    void append(String streamType, String streamId, int expectedVersion, DomainEvent[] events) throws SQLException {
        try {
            write(streamType, streamId, expectedVersion, events);
        } catch (SQLException sqlEx) {
            if (sqlEx.getMessage().contains(DUPLICATE_VERSION_SQL_EXCEPTION_MESSAGE)) {
                int currentVersion;
                try (Connection conn = DriverManager.getConnection(connectionUrl)) {
                    currentVersion = getCurrentStreamVersion(conn, streamType, streamId);
                }
                throw new WrongExpectedVersionException(
                        streamType, streamId, currentVersion, expectedVersion);
            } else {
                throw sqlEx;
            }
        }
    }

    private void write(String streamType, String streamId, int expectedVersion, DomainEvent[] events) throws SQLException {
        int version = expectedVersion + 1;

        try (Connection conn = DriverManager.getConnection(connectionUrl)) {
            conn.setAutoCommit(false);
            ensureExpectedVersion(conn, streamType, streamId, expectedVersion);
            for (var event : events) {
                PreparedStatement statement = conn.prepareStatement(INSERT_SQL);
                PGobject payloadJson = toPgJson(event);

                statement.setString(1, streamType);
                statement.setString(2, streamId);
                statement.setInt(3, version);
                statement.setString(4, event.getClass().toString());
                statement.setObject(5, payloadJson);
                statement.executeUpdate();
                version++;
            }
            conn.commit();
        }
    }


    // Happy for a PostgreSql guru to figure out how to do this directly on the table via a constraint
    // -LC
    private void ensureExpectedVersion(Connection conn, String streamType, String streamId, int expectedVersion) throws SQLException {
        int currentVersion = getCurrentStreamVersion(conn, streamType, streamId);
        if (currentVersion != expectedVersion) {
            throw new WrongExpectedVersionException(
                    streamType, streamId, currentVersion, expectedVersion);
        }
    }

    private int getCurrentStreamVersion(Connection conn, String streamType, String streamId) throws SQLException {
        try (PreparedStatement statement = conn.prepareStatement(SQL_SELECT_MAX_STREAM_VERSION)) {
            statement.setString(1, streamType);
            statement.setString(2, streamId);
            ResultSet rs = statement.executeQuery();
            rs.next();
            return rs.getInt(1);
        }
    }

    private PGobject toPgJson(Object source) throws SQLException {
        String json = GsonUtils.serialize(source);
        PGobject jsonObject = new PGobject();
        jsonObject.setType("json");
        jsonObject.setValue(json);
        return jsonObject;
    }
}