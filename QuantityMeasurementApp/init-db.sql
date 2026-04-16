-- Creates the second database for qma-service
-- qma_db is already created by POSTGRES_DB env var
SELECT 'CREATE DATABASE qma_service_db OWNER qma_user'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'qma_service_db')\gexec
