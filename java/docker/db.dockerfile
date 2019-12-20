FROM postgres:11.1-alpine

COPY ./pg-data-adapter/src/main/resources/* /docker-entrypoint-initdb.d/