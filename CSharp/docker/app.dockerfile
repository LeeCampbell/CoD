FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as builder

COPY . /home/netsdk/
WORKDIR /home/netsdk

RUN dotnet build --configuration Release
RUN dotnet test --no-build --no-restore --configuration Release
RUN dotnet publish --no-build --configuration Release --output ./deploy

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY --from=builder /home/netsdk/deploy/ ./
CMD dotnet ./Yow.CoD.Finance.Application.dll
EXPOSE 80