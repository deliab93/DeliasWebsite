# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY DeliasWebsite.sln ./
COPY DeliasWebsite/DeliasWebsite.Web.csproj DeliasWebsite/
COPY DeliasWebsite.Core/DeliasWebsite.Core.csproj DeliasWebsite.Core/

# Restore NuGet packages
RUN dotnet restore

# Copy all source code
COPY DeliasWebsite/ DeliasWebsite/
COPY DeliasWebsite.Core/ DeliasWebsite.Core/

# Publish the app
WORKDIR /src/DeliasWebsite
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Bind to environment port (Render, Railway, etc.)
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 8080

ENTRYPOINT ["dotnet", "DeliasWebsite.Web.dll"]
