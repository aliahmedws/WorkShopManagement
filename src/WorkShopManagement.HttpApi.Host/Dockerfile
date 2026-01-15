# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution
COPY . .

# Publish the host project
WORKDIR /src/src/WorkShopManagement.HttpApi.Host
RUN dotnet restore WorkShopManagement.HttpApi.Host.csproj
RUN dotnet publish WorkShopManagement.HttpApi.Host.csproj -c Release -o /app/publish --no-restore

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Cloud Run listens on 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WorkShopManagement.HttpApi.Host.dll"]
