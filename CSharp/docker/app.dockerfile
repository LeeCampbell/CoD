FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as builder
COPY . /home/netsdk/
WORKDIR /home/netsdk

RUN dotnet restore ./Yow.CoD.Finance.sln
RUN dotnet build -c=Release ./Yow.CoD.Finance.sln
RUN dotnet test ./Yow.CoD.Finance.sln --no-build /p:Configuration=Release
RUN dotnet publish --no-restore --configuration Release --output ./deploy

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY --from=builder /home/netsdk/deploy/ ./
CMD dotnet ./Yow.CoD.Finance.Application.dll
EXPOSE 80