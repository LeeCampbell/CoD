#!/usr/bin/env bash
docker-compose -f ./csharp/docker-compose.yml up --build --detach

bash -c 'while [[ "$(curl -s -o /dev/null -w ''%{http_code}'' localhost:8080/health)" != "200" ]]; do sleep 5; done'

curl -i -H "Content-Type: application/json" -X POST -d @scripts/CreateLoanExamplePayload.json http://localhost:8080/Loan

docker-compose -f ./csharp/docker-compose.yml down