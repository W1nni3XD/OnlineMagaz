FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY OnlineShop.Web/OnlineShop.Web.csproj OnlineShop.Web/
COPY OnlineShop.Domain/OnlineShop.Domain.csproj OnlineShop.Domain/
RUN dotnet restore OnlineShop.Web/OnlineShop.Web.csproj

COPY . .
RUN dotnet restore OnlineShop.Web/OnlineShop.Web.csproj
RUN dotnet publish OnlineShop.Web/OnlineShop.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:7193/
EXPOSE 7193

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OnlineShop.Web.dll"]