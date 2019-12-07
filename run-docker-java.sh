#!/user/bin/env bash
docker-compose -f ./java/docker-compose.yml up --build

timeout 30 bash -c 'while [[ "$(curl -s -o /dev/null -w ''%{http_code}'' localhost:8080)" != "200" ]]; do sleep 5; done' || false

curl -i -H "Content-Type: application/json" -X POST -d @scripts/CreateLoanExamplePayload.json http://localhost:8080/Loan

docker-compose -f ./java/docker-compose.yml down