-- =============================================
-- Author      : Karl Hochfilzer
-- Create date : 2017-04-03
-- Description : Example to Create DB "MyDB".

-- !!! Do not use unless you know what you are doing !!!
-- !!! This is only an example !!!
-- !!! You need a running Oracle Instance !!!
-- !!! Read first: https://docs.oracle.com/cd/E11882_01/server.112/e25494/create.htm#ADMIN002
-- Doesn't work if you have a local installed: Oracle Database Express Edition (e.g. 11g Release 2 for Windows x64)
-- Express Edition default DB is named as 'XE'
-- Database Express Edition Getting Started Guide:
-- https://docs.oracle.com/cd/E17781_01/admin.112/e18585/toc.htm#XEGSG102
-- Create a new Database on Express, have a look at file: 'createOraDbWinExpress_11_2.bat'
-- Before use, change or create the defined folders and set your passwords (sys_password and system_password)!
-- =============================================
-- Script to create DB

-- DB: "MyDB"

CREATE DATABASE "MyBD"
   USER SYS IDENTIFIED BY sys_password
   USER SYSTEM IDENTIFIED BY system_password
   LOGFILE GROUP 1 ('/u01/logs/my/redo01a.log','/u02/logs/my/redo01b.log') SIZE 100M BLOCKSIZE 512,
           GROUP 2 ('/u01/logs/my/redo02a.log','/u02/logs/my/redo02b.log') SIZE 100M BLOCKSIZE 512,
           GROUP 3 ('/u01/logs/my/redo03a.log','/u02/logs/my/redo03b.log') SIZE 100M BLOCKSIZE 512
   MAXLOGFILES 5
   MAXLOGMEMBERS 5
   MAXLOGHISTORY 1
   MAXDATAFILES 100
   CHARACTER SET AL32UTF8
   NATIONAL CHARACTER SET AL16UTF16
   EXTENT MANAGEMENT LOCAL
   DATAFILE '/u01/app/oracle/oradata/mynewdb/system01.dbf' SIZE 325M REUSE
   SYSAUX DATAFILE '/u01/app/oracle/oradata/mynewdb/sysaux01.dbf' SIZE 325M REUSE
   DEFAULT TABLESPACE users
      DATAFILE '/u01/app/oracle/oradata/mynewdb/users01.dbf'
      SIZE 500M REUSE AUTOEXTEND ON MAXSIZE UNLIMITED
   DEFAULT TEMPORARY TABLESPACE tempts1
      TEMPFILE '/u01/app/oracle/oradata/mynewdb/temp01.dbf'
      SIZE 20M REUSE
   UNDO TABLESPACE undotbs
      DATAFILE '/u01/app/oracle/oradata/mynewdb/undotbs01.dbf'
      SIZE 200M REUSE AUTOEXTEND ON MAXSIZE UNLIMITED;

-- Additionals you normally should do:
-- Setting the Database Time Zone
--   SET TIME_ZONE='+00:00';
-- Specify an Instance Identifier (SID)
--   set ORACLE_SID=MyBD
-- Create a Server Parameter File
--   CREATE SPFILE FROM PFILE;
-- Create an Instance (-STARTMODE MANUAL or AUTO, -PFILE is your "init'ORACLE_SID'.ora" file)
--   oradim -NEW -SID MyBD -STARTMODE MANUAL -PFILE full_path_to_the_text_initialization_parameter_file