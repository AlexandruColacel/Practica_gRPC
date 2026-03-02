FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /Cliente
# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o out
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /Cliente
COPY --from=build /Cliente/out .
ENTRYPOINT ["dotnet", "Cliente.dll"]