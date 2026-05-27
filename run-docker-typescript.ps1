docker-compose -f ./typescript/docker-compose.yml up --build --detach

do {
    Start-Sleep -Seconds 5
    try { $status = (Invoke-WebRequest -Uri http://localhost:8080 -UseBasicParsing).StatusCode } catch { $status = 0 }
} while ($status -ne 200)

Invoke-RestMethod -Method Post -Uri http://localhost:8080/Loan -ContentType "application/json" -InFile scripts/CreateLoanExamplePayload.json

docker-compose -f ./typescript/docker-compose.yml down
