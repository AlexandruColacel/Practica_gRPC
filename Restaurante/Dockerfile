FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /Restaurante
# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /Restaurante
COPY --from=build /Restaurante/out .
ENTRYPOINT ["dotnet", "Restaurante.dll"]