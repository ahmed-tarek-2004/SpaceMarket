
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY ["EcommercePlatform/Ecommerce.API.csproj", "EcommercePlatform/"]
COPY ["Ecommerce.DataAccess/Ecommerce.DataAccess.csproj", "Ecommerce.DataAccess/"]
COPY ["Ecommerce.Entities/Ecommerce.Entities.csproj", "Ecommerce.Entities/"]
COPY ["Ecommerce.Utilities/Ecommerce.Utilities.csproj", "Ecommerce.Utilities/"]

RUN dotnet restore "EcommercePlatform/Ecommerce.API.csproj"


COPY . .


WORKDIR /src/EcommercePlatform
RUN dotnet publish "Ecommerce.API.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5258
ENV ASPNETCORE_URLS=http://+:5258
ENTRYPOINT ["dotnet", "Ecommerce.API.dll"]
