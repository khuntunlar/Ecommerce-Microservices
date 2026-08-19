FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/CatalogService.Domain/CatalogService.Domain.csproj src/CatalogService.Domain/
COPY src/CatalogService.Application/CatalogService.Application.csproj src/CatalogService.Application/
COPY src/CatalogService.Infrastructure/CatalogService.Infrastructure.csproj src/CatalogService.Infrastructure/
COPY src/CatalogService.Api/CatalogService.Api.csproj src/CatalogService.Api/
RUN dotnet restore src/CatalogService.Api/CatalogService.Api.csproj

COPY src src
RUN dotnet publish src/CatalogService.Api/CatalogService.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.Api.dll"]
