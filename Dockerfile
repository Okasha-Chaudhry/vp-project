FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY StudyConnect.sln ./
COPY API/StudyConnect.API.csproj API/

RUN dotnet restore API/StudyConnect.API.csproj

COPY API/ API/

RUN dotnet publish API/StudyConnect.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["sh", "-c", "dotnet StudyConnect.API.dll --urls http://0.0.0.0:${PORT:-8080}"]
