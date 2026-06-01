#!/usr/bin/env bash
docker-compose -f ./typescript/docker-compose.yml up --build --detach

echo "Waiting for application to be ready..." 
bash -c 'while [[ "$(curl -s -o /dev/null -w ''%{http_code}'' localhost:8080)" != "200" ]]; do echo "." && sleep 1; done'

curl -i -H "Content-Type: application/json" -X POST -d @scripts/CreateLoanExamplePayload.json http://localhost:8080/Loan
echo ""
echo ""

docker-compose -f ./typescript/docker-compose.yml stop --timeout 10
EXIT_CODE=$(docker inspect cod-ts-application --format='{{.State.ExitCode}}')
echo "Application container exit code: ${EXIT_CODE}"
docker-compose -f ./typescript/docker-compose.yml down
if [ "$EXIT_CODE" != "0" ]; then
  echo "ERROR: Expected graceful shutdown (exit code 0), got ${EXIT_CODE}"
  exit 1
fi
