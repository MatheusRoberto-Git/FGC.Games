# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["FGC.Games.Presentation/FGC.Games.Presentation.csproj", "FGC.Games.Presentation/"]
COPY ["FGC.Games.Application/FGC.Games.Application.csproj", "FGC.Games.Application/"]
COPY ["FGC.Games.Infrastructure/FGC.Games.Infrastructure.csproj", "FGC.Games.Infrastructure/"]
COPY ["FGC.Games.Domain/FGC.Games.Domain.csproj", "FGC.Games.Domain/"]

RUN dotnet restore "FGC.Games.Presentation/FGC.Games.Presentation.csproj"

COPY . .

WORKDIR "/src/FGC.Games.Presentation"
RUN dotnet build "FGC.Games.Presentation.csproj" -c Release -o /app/build --no-restore

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "FGC.Games.Presentation.csproj" -c Release -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Stage 3: Runtime (Alpine)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

LABEL maintainer="FIAP Cloud Games" \
      version="2.0.0" \
      description="FGC Games API - Microsserviço de Jogos"

RUN apk add --no-cache curl icu-libs

RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -D appuser

WORKDIR /app

COPY --from=publish /app/publish .

RUN chown -R appuser:appgroup /app

USER appuser

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "FGC.Games.Presentation.dll"]