FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY OnlineShop.API/OnlineShop.API.csproj OnlineShop.API/
COPY OnlineShop.Domain/OnlineShop.Domain.csproj OnlineShop.Domain/
RUN dotnet restore OnlineShop.API/OnlineShop.API.csproj

COPY . .
RUN dotnet publish OnlineShop.API/OnlineShop.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5170/
EXPOSE 5170

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OnlineShop.API.dll"]