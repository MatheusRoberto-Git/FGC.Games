# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto
COPY ["FGC.Games.Presentation/FGC.Games.Presentation.csproj", "FGC.Games.Presentation/"]
COPY ["FGC.Games.Application/FGC.Games.Application.csproj", "FGC.Games.Application/"]
COPY ["FGC.Games.Infrastructure/FGC.Games.Infrastructure.csproj", "FGC.Games.Infrastructure/"]
COPY ["FGC.Games.Domain/FGC.Games.Domain.csproj", "FGC.Games.Domain/"]

# Restore
RUN dotnet restore "FGC.Games.Presentation/FGC.Games.Presentation.csproj"

# Copia todo o código
COPY . .

# Build
WORKDIR "/src/FGC.Games.Presentation"
RUN dotnet build "FGC.Games.Presentation.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "FGC.Games.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FGC.Games.Presentation.dll"]