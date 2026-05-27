#!/usr/bin/env bash
docker-compose -f ./typescript/docker-compose.yml up --build --detach

bash -c 'while [[ "$(curl -s -o /dev/null -w ''%{http_code}'' localhost:8080)" != "200" ]]; do sleep 5; done'

curl -i -H "Content-Type: application/json" -X POST -d @scripts/CreateLoanExamplePayload.json http://localhost:8080/Loan

docker-compose -f ./typescript/docker-compose.yml down
