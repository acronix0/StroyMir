#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SimpleShop/SimpleShop.WebApi.csproj", "SimpleShop/"]
COPY ["SimpleShop.Repo/SimpleShop.Repo.csproj", "SimpleShop.Repo/"]
COPY ["StroyMirCore/SimpleShop.Core.csproj", "StroyMirCore/"]
COPY ["SimpleShop.Service/SimpleShop.Service.csproj", "SimpleShop.Service/"]
RUN dotnet restore "./SimpleShop/./SimpleShop.WebApi.csproj"
COPY . .
WORKDIR "/src/SimpleShop"
RUN dotnet build "./SimpleShop.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SimpleShop.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleShop.WebApi.dll"]