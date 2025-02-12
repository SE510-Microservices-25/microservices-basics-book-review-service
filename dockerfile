FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY Domain/*.csproj ./Domain/
COPY Application/*.csproj ./Application/
COPY WebApi/*.csproj ./WebApi/
COPY Mocks.DataAccess/*.csproj ./Mocks.DataAccess/

RUN dotnet restore

COPY . .

RUN dotnet publish "WebApi/WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "WebApi.dll"]
