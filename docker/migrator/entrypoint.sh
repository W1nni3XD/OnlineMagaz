#!/bin/sh
set -eu

DB_HOST="${DB_HOST:-db}"
DB_PORT="${DB_PORT:-5432}"
DB_USER="${DB_USER:-postgres}"
DB_NAME="${DB_NAME:-OnlineShopDb}"
DB_PASSWORD="${DB_PASSWORD:-postgres123}"

echo "Waiting for PostgreSQL at ${DB_HOST}:${DB_PORT}..."
until pg_isready -h "${DB_HOST}" -p "${DB_PORT}" -U "${DB_USER}" >/dev/null 2>&1; do
  sleep 2
done

echo "PostgreSQL is available. Applying migrations..."
dotnet ef database update \
  --project /src/OnlineShop.API/OnlineShop.API.csproj \
  --startup-project /src/OnlineShop.API/OnlineShop.API.csproj \
  --configuration Release \
  --no-build

echo "Applying admin seed script..."
export PGPASSWORD="${DB_PASSWORD}"
psql -h "${DB_HOST}" -p "${DB_PORT}" -U "${DB_USER}" -d "${DB_NAME}" -f /seed-admin.sql

echo "Migrations and seed applied successfully."