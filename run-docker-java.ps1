docker-compose -f ./java/docker-compose.yml up --build --detach

$count = 0
do{
    try{
        Invoke-WebRequest -Uri 'http://localhost:8080/' -UseBasicParsing -DisableKeepAlive | out-null
        $success = $true
    }
    catch{
        Write-Output "Waiting another 5 seconds for web server to be up"
        Start-sleep -Seconds 5
    }
    $count++
}until($count -eq 5 -or $success)
if($success){
    Invoke-WebRequest -ContentType "application/json" -Headers @{"accept"="application/json"} -Method Post -InFile ".\scripts\CreateLoanExamplePayload.json" -UseBasicParsing -Uri "http://localhost:8080/Loan"
} else {
    # fail
}
docker-compose -f ./java/docker-compose.yml down