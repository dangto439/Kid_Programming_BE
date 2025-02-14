#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["KidPrograming/KidPrograming.csproj", "KidPrograming/"]
COPY ["KidPrograming.Contract.Repositories/KidPrograming.Contract.Repositories.csproj", "KidPrograming.Contract.Repositories/"]
COPY ["KidPrograming.Contract.Services/KidPrograming.Contract.Services.csproj", "KidPrograming.Contract.Services/"]
COPY ["KidPrograming.Repositories/KidPrograming.Repositories.csproj", "KidPrograming.Repositories/"]
COPY ["KidPrograming.Entity/KidPrograming.Entity.csproj", "KidPrograming.Entity/"]
COPY ["KidPrograming.Core/KidPrograming.Core.csproj", "KidPrograming.Core/"]
COPY ["KidPrograming.Services/KidPrograming.Services.csproj", "KidPrograming.Services/"]
RUN dotnet restore "./KidPrograming/KidPrograming.csproj"
COPY . .
WORKDIR "/src/KidPrograming"
RUN dotnet build "./KidPrograming.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KidPrograming.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KidPrograming.dll"]