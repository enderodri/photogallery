# Use the official .NET 8 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Install dotnet-ef tool
RUN dotnet tool install --global dotnet-ef

# Copy everything else and build
COPY . ./
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet publish -c Release -o out

# Run migrations
RUN dotnet ef database update

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Set the entrypoint
ENTRYPOINT ["dotnet", "PhotoGallery.dll"]
