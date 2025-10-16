# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files
COPY ["EcommercePlatform/Ecommerce.API.csproj", "EcommercePlatform/"]
COPY ["Ecommerce.DataAccess/Ecommerce.DataAccess.csproj", "Ecommerce.DataAccess/"]
COPY ["Ecommerce.Entities/Ecommerce.Entities.csproj", "Ecommerce.Entities/"]
COPY ["Ecommerce.Utilities/Ecommerce.Utilities.csproj", "Ecommerce.Utilities/"]

# Restore dependencies
RUN dotnet restore "EcommercePlatform/Ecommerce.API.csproj"

# Copy the rest of the files
COPY . .

# Build and publish
WORKDIR /src/EcommercePlatform
RUN dotnet publish "Ecommerce.API.csproj" -c Release -o /app/publish

# Stage 2: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5258
ENV ASPNETCORE_URLS=http://+:5258
ENTRYPOINT ["dotnet", "Ecommerce.API.dll"]
