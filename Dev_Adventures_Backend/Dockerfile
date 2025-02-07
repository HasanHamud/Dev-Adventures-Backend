# Stage 1: Build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["Dev_Adventures_Backend/Dev_Adventures_Backend.csproj", "Dev_Adventures_Backend/"]
COPY ["Dev_Db/Dev_Db.csproj", "Dev_Db/"]
COPY ["Dev_Models/Dev_Models.csproj", "Dev_Models/"]

# Restore dependencies
RUN dotnet restore "./Dev_Adventures_Backend/Dev_Adventures_Backend.csproj"

# Copy the remaining files and build
COPY . .
WORKDIR "/src/Dev_Adventures_Backend"
RUN dotnet build "./Dev_Adventures_Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: Publish the project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dev_Adventures_Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published files
COPY --from=publish /app/publish .

# Expose the application port (if needed)
EXPOSE 8080

# Define the entry point
ENTRYPOINT ["dotnet", "Dev_Adventures_Backend.dll"]
