rem This is a Copy from:
rem http://stackoverflow.com/questions/9534136/how-to-create-a-new-database-after-initally-installing-oracle-database-11g-expre
rem Folders changed to Oracle default file locations. (Except: Log-files are inserted into BD-Dir)

REM ASSUMPTIONS
rem oracle xe has been installed
rem oracle_home has been set
rem oracle_sid has been set
rem oracle service is running

rem Change the parameter values like app_name, ora_dir etc.
rem  Run the file with administrative privileges.
rem The batch file creates a basic oracle database.
rem Note : May take much time (say around 30mins)

REM SET PARAMETERS
set char_set =al32utf8
set nls_char_set =al16utf16

set ora_dir=C:\oraclexe\app\oracle\oradata
set app_name=MyDB
set db_name=%app_name%_db
set db_sid=%db_name%_sid
set db_ins=%db_name%_ins
set sys_passwd=x3y5z7
set system_passwd=1x4y9z

set max_log_files=32
set max_log_members=4
set max_log_history=100
set max_data_files=254
set max_instances=1

set db_dir=%ora_dir%\%db_name%

set instl_temp_dir=%db_dir%\instl\script

set system_dir=%db_dir%\system
set user_dir=%db_dir%\user
set undo_dir=%db_dir%\undo
set sys_aux_dir=%db_dir%\sysaux
set temp_dir=%db_dir%\temp
set control_dir=%db_dir%\control

set pfile_dir=%db_dir%\pfile
set data_dir=%db_dir%\data
set index_dir=%db_dir%\index
set log_dir=%db_dir%\log
set backup_dir=%db_dir%\backup
set archive_dir=%db_dir%\archive

set data_dir=%db_dir%\data
set index_dir=%db_dir%\index
set log_dir=%db_dir%\log
set backup_dir=%db_dir%\backup
set archive_dir=%db_dir%\archive
set undo_dir=%db_dir%\undo
set default_dir=%db_dir%\default

set system_tbs=%db_name%_system_tbs
set user_tbs=%db_name%_user_tbs
set sys_aux_tbs=%db_name%_sys_aux_tbs
set temp_tbs=%db_name%_temp_tbs
set control_tbs=%db_name%_control_tbs

set data_tbs=%db_name%_data_tbs
set index_tbs=%db_name%_index_tbs
set log_tbs=%db_name%_log_tbs
set backup_tbs=%db_name%_backup_tbs
set archive_tbs=%db_name%_archive_tbs
set undo_tbs=%db_name%_undo_tbs
set default_tbs=%db_name%_default_tbs

set system_file=%system_dir%\%db_name%_system.dbf
set user_file=%user_dir%\%db_name%_user.dbf
set sys_aux_file=%sys_aux_dir%\%db_name%_sys_aux.dbf
set temp_file=%temp_dir%\%db_name%_temp.dbf
set control_file=%control_dir%\%db_name%_control.dbf

set data_file=%data_dir%\%db_name%_data.dbf
set index_file=%index_dir%\%db_name%_index.dbf
set backup_file=%backup_dir%\%db_name%_backup.dbf
set archive_file=%archive_dir%\%db_name%_archive.dbf
set undo_file=%undo_dir%\%db_name%_undo.dbf
set default_file=%default_dir%\%db_name%_default.dbf

set log1_file=%log_dir%\%db_name%_log1.log
set log2_file=%log_dir%\%db_name%_log2.log
set log3_file=%log_dir%\%db_name%_log3.log

set init_file=%pfile_dir%\init%db_sid%.ora
set db_create_file=%instl_temp_dir%\createdb.sql
set db_drop_file=dropdb.sql

set db_create_log=%instl_temp_dir%\db_create.log
set db_drop_log=db_drop.log

set oracle_sid=%db_sid%

REM WRITE DROP DATABASE SQL COMMANDS TO FILE
echo shutdown immediate;>%db_drop_file%
echo startup mount exclusive restrict;>>%db_drop_file%
echo drop database;>>%db_drop_file%

REM EXECUTE DROP DATABASE SQL COMMANDS FROM THE FILE    
rem sqlplus -s "/as sysdba" @"%db_drop_file%">%db_drop_log%

REM DELETE WINDOWS ORACLE SERVICE
rem oradim -delete -sid %db_sid%

REM CREATE DIRECTORY STRUCTURE
md %system_dir%
md %user_dir%
md %sys_aux_dir%
md %temp_dir%
md %control_dir%

md %pfile_dir%
md %data_dir%
md %index_dir%
md %log_dir%
md %backup_dir%
md %archive_dir%
md %undo_dir%
md %default_dir%
md %instl_temp_dir%

REM WRITE INIT FILE PARAMETERS TO INIT FILE
echo db_name='%db_name%'>%init_file%
echo memory_target=1024m>>%init_file%
echo processes=150>>%init_file%
echo sessions=20>>%init_file%
echo audit_file_dest=%user_dir%>>%init_file%
echo audit_trail ='db'>>%init_file%
echo db_block_size=8192>>%init_file%
echo db_domain=''>>%init_file%
echo diagnostic_dest=%db_dir%>>%init_file%
echo dispatchers='(protocol=tcp) (service=%app_name%xdb)'>>%init_file%
echo shared_servers=4>>%init_file%
echo open_cursors=300>>%init_file%
echo remote_login_passwordfile='exclusive'>>%init_file%
echo undo_management=auto>>%init_file%
echo undo_tablespace='%undo_tbs%'>>%init_file%
echo control_files = ("%control_dir%\control1.ora", "%control_dir%\control2.ora")>>%init_file%
echo job_queue_processes=4>>%init_file%
echo db_recovery_file_dest_size = 10g>>%init_file%
echo db_recovery_file_dest=%log_dir%>>%init_file%
echo compatible ='11.2.0'>>%init_file%

REM WRITE DB CREATE AND ITS RELATED SQL COMMAND TO FILE    
echo startup nomount pfile='%init_file%';>>%db_create_file%
echo.>>%db_create_file%

echo create database %db_name%>>%db_create_file%
echo user sys identified by %sys_passwd%>>%db_create_file%
echo user system identified by %system_passwd%>>%db_create_file%
echo logfile group 1 ('%log1_file%') size 100m,>>%db_create_file%
echo group 2 ('%log2_file%') size 100m,>>%db_create_file%
echo group 3 ('%log3_file%') size 100m>>%db_create_file%
echo maxlogfiles %max_log_files%>>%db_create_file%
echo maxlogmembers %max_log_members%>>%db_create_file%
echo maxloghistory %max_log_history%>>%db_create_file%
echo maxdatafiles %max_data_files%>>%db_create_file%
echo character set %char_set %>>%db_create_file%
echo national character set %nls_char_set %>>%db_create_file%
echo extent management local>>%db_create_file%
echo datafile '%system_file%' size 325m reuse>>%db_create_file%
echo sysaux datafile '%sys_aux_file%' size 325m reuse>>%db_create_file%
echo default tablespace %default_tbs%>>%db_create_file%
echo datafile '%default_file%'>>%db_create_file%
echo size 500m reuse autoextend on maxsize unlimited>>%db_create_file%
echo default temporary tablespace %temp_tbs%>>%db_create_file%
echo tempfile '%temp_file%'>>%db_create_file%
echo size 20m reuse>>%db_create_file%
echo undo tablespace %undo_tbs%>>%db_create_file%
echo datafile '%undo_file%'>>%db_create_file%
echo size 200m reuse autoextend on maxsize unlimited;>>%db_create_file%
echo.>>%db_create_file%

echo @?\rdbms\admin\catalog.sql>>%db_create_file%
echo.>>%db_create_file%

echo @?\rdbms\admin\catproc.sql>>%db_create_file%
echo.>>%db_create_file%

echo create spfile from pfile='%init_file%';>>%db_create_file%
echo.>>%db_create_file%

echo shutdown immediate;>>%db_create_file%
echo.>>%db_create_file%

echo startup;>>%db_create_file%
echo.>>%db_create_file%

echo show parameter spfile;>>%db_create_file%
echo.>>%db_create_file%

REM CREATE WINDOWS ORACLE SERVICE
oradim -new -sid %db_sid% -startmode auto

REM EXECUTE DB CREATE SQL COMMANDS FROM FILE
sqlplus -s "/as sysdba" @"%db_create_file%">%db_create_log%

pause