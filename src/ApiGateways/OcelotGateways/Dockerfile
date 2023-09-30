#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ApiGateways/OcelotGateways/OcelotGateways.csproj", "ApiGateways/OcelotGateways/"]
COPY ["BuildingBlocks/Common.Logging/Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
RUN dotnet restore "ApiGateways/OcelotGateways/OcelotGateways.csproj"
COPY . .
WORKDIR "ApiGateways/OcelotGateways"
RUN dotnet build "OcelotGateways.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OcelotGateways.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OcelotGateways.dll"]