-- =============================================
-- Author      : Karl Hochfilzer
-- Create date : 2017-04-03
-- Description : To Create DB "MyDB", for owner 'postgres'

-- =============================================
-- Script to create DB

-- DB: "MyDB"

CREATE DATABASE "MyDB"
  WITH OWNER = postgres
       ENCODING = 'UTF8'
       TABLESPACE = pg_default
       CONNECTION LIMIT = -1;

COMMENT ON DATABASE "MyDB"
  IS 'Database to store schema with Countries data.';
