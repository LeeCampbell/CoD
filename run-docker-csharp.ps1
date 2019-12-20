docker-compose -f ./csharp/docker-compose.yml up --build --detach

$count = 0
do{
    try{
        Invoke-WebRequest -Uri 'http://localhost:8080/health' -UseBasicParsing -DisableKeepAlive | out-null
        $success = $true
    }
    catch{
        Write-Output "Waiting another 5 seconds for web server to be up"
        Start-sleep -Seconds 5
    }
    $count++
}until($count -eq 5 -or $success)
if($success){
    try {
        Invoke-WebRequest -ContentType "application/json" -Headers @{"accept"="application/json"} -Method Post -InFile ".\scripts\CreateLoanExamplePayload.json" -UseBasicParsing -Uri "http://localhost:8080/Loan"    
    } catch [System.Net.WebException] {
        Write-Error $_.Exception.Message
        Write-Error ($_.Exception.Response | ConvertTo-Json)
    } catch {
        Write-Host ($_ | ConvertTo-Json)
    }
    
} else {
    # fail
    Write-Error "Was unable to ping web server"
}
docker-compose -f ./csharp/docker-compose.yml down