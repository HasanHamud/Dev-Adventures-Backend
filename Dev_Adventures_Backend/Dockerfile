FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Dev_Adventures_Backend/Dev_Adventures_Backend.csproj", "Dev_Adventures_Backend/"]
COPY ["Dev_Db/Dev_Db.csproj", "Dev_Db/"]
COPY ["Dev_Models/Dev_Models.csproj", "Dev_Models/"]

RUN dotnet restore "./Dev_Adventures_Backend/Dev_Adventures_Backend.csproj"

COPY . .
WORKDIR "/src/Dev_Adventures_Backend"
RUN dotnet build "./Dev_Adventures_Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dev_Adventures_Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 5101

ENTRYPOINT ["dotnet", "Dev_Adventures_Backend.dll"]
