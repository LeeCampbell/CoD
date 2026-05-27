FROM postgres:16-alpine

COPY ./pg-data-adapter/src/sql/* /docker-entrypoint-initdb.d/
