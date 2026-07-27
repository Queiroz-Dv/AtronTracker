FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "AtronPlatform/WebApi/AtronPlatform.WebApi.csproj"
RUN dotnet publish "AtronPlatform/WebApi/AtronPlatform.WebApi.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /app/publish .

EXPOSE 8080

CMD ["sh", "-c", "dotnet AtronPlatform.WebApi.dll --urls http://0.0.0.0:${PORT:-8080}"]
