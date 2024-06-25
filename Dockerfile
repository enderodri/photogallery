# Use the official .NET 8 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
COPY entrypoint.sh .

# Ensure the entrypoint script is executable
RUN chmod +x entrypoint.sh

# Set the entrypoint
ENTRYPOINT ["./entrypoint.sh"]
