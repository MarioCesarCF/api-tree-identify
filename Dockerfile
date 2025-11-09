# ------------------ Build stage ------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SWIA.csproj", "./"]
RUN dotnet restore "./SWIA.csproj"
COPY . .
RUN dotnet publish "./SWIA.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ------------------ Runtime stage ------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Porta padrão do Render é 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Carrega o .env se existir (opcional no Render)
# Isso é ignorado se as variáveis já estiverem configuradas no ambiente
COPY .env* ./

ENTRYPOINT ["dotnet", "SWIA.dll"]
