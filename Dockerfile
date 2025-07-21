# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files only
COPY DeliasWebsite.sln ./
COPY DeliasWebsite/DeliasWebsite.Web.csproj DeliasWebsite/
COPY DeliasWebsite.Core/DeliasWebsite.Core.csproj DeliasWebsite.Core/

# Restore NuGet packages
RUN dotnet restore

# Copy the rest of the source code
COPY DeliasWebsite ./DeliasWebsite
COPY DeliasWebsite.Core ./DeliasWebsite.Core

# Publish the web app
WORKDIR /src/DeliasWebsite
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose for Render and Railway (binds to PORT env variable)
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 10000
ENTRYPOINT ["dotnet", "DeliasWebsite.Web.dll"]
