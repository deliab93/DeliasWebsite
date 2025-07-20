# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY DeliasWebsite/*.csproj ./DeliasWebsite.Web/
COPY DeliasWebsite.Core/*.csproj ./DeliasWebsite.Core/
RUN dotnet restore

COPY . .
RUN dotnet publish DeliasWebsite/DeliasWebsite.Web.csproj -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
EXPOSE 80
ENTRYPOINT ["dotnet", "DeliasWebsite.Web.dll"]
