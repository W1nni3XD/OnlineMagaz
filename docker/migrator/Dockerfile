FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src

RUN apt-get update \
    && apt-get install -y --no-install-recommends postgresql-client \
    && rm -rf /var/lib/apt/lists/*

RUN dotnet tool install --global dotnet-ef --version 10.0.6
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY OnlineShop.API/OnlineShop.API.csproj OnlineShop.API/
COPY OnlineShop.Domain/OnlineShop.Domain.csproj OnlineShop.Domain/
RUN dotnet restore OnlineShop.API/OnlineShop.API.csproj

COPY . .
RUN dotnet build OnlineShop.API/OnlineShop.API.csproj -c Release --no-restore

COPY docker/migrator/entrypoint.sh /entrypoint.sh
COPY docker/db/init/admin.sql /seed-admin.sql
RUN sed -i 's/\r$//' /entrypoint.sh \
    && chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]