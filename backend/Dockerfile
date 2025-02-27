# Alap .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Futtatás induláskor
CMD dotnet publish Backend/Backend.csproj -c Release -o /app/out && dotnet /app/out/Backend.dll
