#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8087
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Fiap.Hackatoon.Order.Api/Fiap.Hackatoon.Order.Api.csproj", "Fiap.Hackatoon.Order.Api/"]
COPY ["Fiap.Hackatoon.Order.Application/Fiap.Hackatoon.Order.Application.csproj", "Fiap.Hackatoon.Order.Application/"]
COPY ["Fiap.Hackatoon.Order.Domain/Fiap.Hackatoon.Order.Domain.csproj", "Fiap.Hackatoon.Order.Domain/"]
COPY ["Fiap.Hackatoon.Order.Infrastructure/Fiap.Hackatoon.Order.Infrastructure.csproj", "Fiap.Hackatoon.Order.Infrastructure/"]
RUN dotnet restore "./Fiap.Hackatoon.Order.Api/Fiap.Hackatoon.Order.Api.csproj"
COPY . .
WORKDIR "/src/Fiap.Hackatoon.Order.Api"
RUN dotnet build "./Fiap.Hackatoon.Order.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fiap.Hackatoon.Order.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fiap.Hackatoon.Order.Api.dll"]