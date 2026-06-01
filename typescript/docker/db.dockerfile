FROM postgres:16-alpine

COPY ./packages/pg-data-adapter/src/sql/* /docker-entrypoint-initdb.d/
