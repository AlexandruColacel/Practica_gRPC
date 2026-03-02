FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /Repartidor
# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /Repartidor
COPY --from=build /Repartidor/out .
ENTRYPOINT ["dotnet", "Repartidor.dll"]