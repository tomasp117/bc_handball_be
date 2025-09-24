# Pou�it� ASP.NET runtime pro b�h aplikace
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Pou�it� .NET SDK pro build aplikace
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Kop�rov�n� a obnova z�vislost� API a Core projekt�
COPY ["bc_handball_be.API/bc_handball_be.API.csproj", "bc_handball_be.API/"]
COPY ["bc_handball_be.Core/bc_handball_be.Core.csproj", "bc_handball_be.Core/"]
RUN dotnet restore "./bc_handball_be.API/bc_handball_be.API.csproj"

# Kop�rov�n� cel�ho k�du do kontejneru
COPY . .

# Build API projektu
WORKDIR "/src/bc_handball_be.API"
RUN dotnet build "./bc_handball_be.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publikov�n� API projektu
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./bc_handball_be.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image pro b�h aplikace
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p wwwroot/images \
    && chmod -R 755 wwwroot \
    && chmod -R 775 wwwroot/images

# Pou�it� CMD m�sto ENTRYPOINT pro flexibilitu
CMD ["dotnet", "bc_handball_be.API.dll", "--urls", "http://0.0.0.0:8080"]
