#!/bin/bash
set -e


if ! psql --username "$POSTGRES_USER" --dbname postgres --tuples-only --command \
    "SELECT 1 FROM pg_database WHERE datname = '$NOTIFICATIONS_DB';" | grep -q 1; then
    psql --username "$POSTGRES_USER" --dbname postgres --command \
        "CREATE DATABASE \"$NOTIFICATIONS_DB\";"
fi