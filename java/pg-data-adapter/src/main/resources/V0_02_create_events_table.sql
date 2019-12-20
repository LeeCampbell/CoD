CREATE TABLE event_store.events
(
  position            BIGSERIAL UNIQUE NOT NULL,
  stream_type         TEXT             NOT NULL,
  stream_id           TEXT             NOT NULL,
  version             INT              NOT NULL,
  event_type          TEXT             NOT NULL,
  payload             JSON             NOT NULL,
  PRIMARY KEY (stream_type, stream_id, version)
);