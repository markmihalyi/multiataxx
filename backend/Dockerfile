# .NET 8 SDK image for AI
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-ai
WORKDIR /app
COPY ./AI.Abstractions ./AI.Abstractions
COPY ./AI ./AI
RUN ln -s /app/AI.Abstractions /app/AI/../AI.Abstractions
RUN dotnet publish ./AI/AI.csproj -c Release -o /app/out

# .NET 9 SDK image for Backend
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-backend
WORKDIR /app
COPY ./AI.Abstractions ./AI.Abstractions
COPY ./Backend ./Backend
RUN dotnet publish ./Backend/Backend.csproj -c Release -o /app/out

# .NET 9 SDK image for running Backend with AI
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS start
WORKDIR /app
COPY --from=build-ai /app/out ./
COPY --from=build-backend /app/out ./
ENTRYPOINT ["dotnet", "Backend.dll"]