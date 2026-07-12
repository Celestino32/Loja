FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY LojaVirtual.slnx .
COPY src/LojaVirtual.Domain/LojaVirtual.Domain.csproj src/LojaVirtual.Domain/
COPY src/LojaVirtual.Application/LojaVirtual.Application.csproj src/LojaVirtual.Application/
COPY src/LojaVirtual.Infrastructure/LojaVirtual.Infrastructure.csproj src/LojaVirtual.Infrastructure/
COPY src/LojaVirtual.Api/LojaVirtual.Api.csproj src/LojaVirtual.Api/
COPY tests/LojaVirtual.UnitTests/LojaVirtual.UnitTests.csproj tests/LojaVirtual.UnitTests/
COPY tests/LojaVirtual.IntegrationTests/LojaVirtual.IntegrationTests.csproj tests/LojaVirtual.IntegrationTests/

RUN dotnet restore src/LojaVirtual.Api/LojaVirtual.Api.csproj

COPY . .

RUN dotnet publish src/LojaVirtual.Api/LojaVirtual.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LojaVirtual.Api.dll"]
