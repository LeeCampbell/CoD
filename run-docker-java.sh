#!/user/bin/env bash
docker-compose -f ./java/docker-compose.yml up --build

curl -i -H "Content-Type: application/json" -X POST -d @CreateLoanExamplePayload.json http://localhost:8080/Loan

docker-compose -f ./java/docker-compose.yml down