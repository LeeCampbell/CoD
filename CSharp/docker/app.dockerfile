FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as builder

COPY ./Yow.CoD.Finance.sln /home/netsdk/
WORKDIR /home/netsdk
COPY ./Yow.CoD.Finance.Application/*.csproj Yow.CoD.Finance.Application/
COPY ./Yow.CoD.Finance.Domain/*.csproj Yow.CoD.Finance.Domain/
COPY ./Yow.CoD.Finance.Domain.Tests/*.csproj Yow.CoD.Finance.Domain.Tests/
COPY ./Yow.CoD.Finance.SqlDataAdapter/*.csproj Yow.CoD.Finance.SqlDataAdapter/
COPY ./Yow.CoD.Finance.WebCommsAdapter/*.csproj Yow.CoD.Finance.WebCommsAdapter/

RUN dotnet restore

#TODO: Not sure why --no-restore isn't working yet -LC
COPY ./Yow.CoD.Finance.Application ./Yow.CoD.Finance.Application
COPY ./Yow.CoD.Finance.Domain ./Yow.CoD.Finance.Domain
COPY ./Yow.CoD.Finance.Domain.Tests ./Yow.CoD.Finance.Domain.Tests
COPY ./Yow.CoD.Finance.SqlDataAdapter ./Yow.CoD.Finance.SqlDataAdapter
COPY ./Yow.CoD.Finance.WebCommsAdapter ./Yow.CoD.Finance.WebCommsAdapter
RUN dotnet build --no-restore --configuration Release
RUN dotnet test --no-build --no-restore --configuration Release
RUN dotnet publish --no-build --configuration Release --output ./deploy

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY --from=builder /home/netsdk/deploy/ ./
CMD dotnet ./Yow.CoD.Finance.Application.dll
EXPOSE 80