# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY DeliasWebsite.sln ./
COPY DeliasWebsite/DeliasWebsite.csproj DeliasWebsite/
COPY DeliasWebsite.Core/DeliasWebsite.Core.csproj DeliasWebsite.Core/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source
COPY . .

# Publish the web project
WORKDIR /src/DeliasWebsite
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "DeliasWebsite.dll"]
