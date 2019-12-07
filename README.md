# Cost of Dependencies

This repo is the supporting code for a presentation written for *YOW! West* conference 2017.
http://west.yowconference.com.au/speakers/lee-campbell/

The presentation was represented at YOW By Night in Sydney and Brisbane later in the same year.
http://nights.yowconference.com.au/upcoming/lee-campbell-cost-dependency-brisbane-nov-8-sydney-nov-9/

The code aims to show how a Domain Model implementation can be written with no dependencies other than the platform it is based on i.e. in this case .NET.

Video of the talk can be found on youtube
https://www.youtube.com/watch?v=T6HjgV9WSCQ&feature=youtu.be

## How to run

This repo has both C# (.NET) and Java examples.

### C# / .NET

Currently this is a mix of the older CSPROJ format and the new.
The easiest way to run it is to open _.\CSharp\Yow.CoD.Finance.sln_ and hit F5

To send commands to the web endpoint either use Powershell:

```bat
cd .\scripts
Invoke-WebRequest -ContentType "application/json" -Headers @{"accept"="application/json"} -Method Post -InFile "CreateLoanExamplePayload.json" -UseBasicParsing -Uri "http://localhost:64181/Loan"
```

### Java

```bat
cd .\java
gradlew run
```

At another prompt issue the curl command

```bat
cd .\scripts
curl -i -H "Content-Type: application/json" -X POST -d @CreateLoanExamplePayload.json http://localhost:4567/Loan
```

which should return a reponse similar to :

```bat
HTTP/1.1 200 OK
Date: Tue, 27 Feb 2018 04:59:18 GMT
Content-Type: text/html;charset=utf-8
Transfer-Encoding: chunked
Server: Jetty(9.3.2.v20150730)

{"loanId":"5d7fdd20-a0d4-40ea-b54e-addb8dabb436"}
```
