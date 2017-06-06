CldrToDatabases
=============
Windows Console Application.

I was searching for localized countries data including some additional information to be used in a different set of my projects.  
Found a solution where to get a lot of this data at https://github.com/mj1856/TimeZoneNames.  
This project is very cool but for my needs it has only localized time zone names and doesn't store the data in a database.  
  
Data is downloaded from http://unicode.org and http://nodatime.org.  
All the data marked as 'deprecated' or 'private_use' is not inserted.  
I use Languages, Scripts, Territories (incl. Subdivisions), Currencies, TimeZones, MetaZones, WindowsZones and all available translations.  
My projects do not need any data that does not exist anymore or that is not in use anymore (is marked as 'deprecated').  
All the data is inserted into a set of different databases each using big data import options.


Databases
=============================================================================
- MS SQLServer
  - Version: Microsoft SQL Server Express (64-bit), 13.0.4202.2 (= 2016 Express)
- MySQL
  - Version: 5.7.9-log MySQL Community Server(GPL), Win64 (x86_64)
- PostgreSql
  - Version: PostgreSQL 9.6.2 on x86_64-pc-mingw64, compiled by gcc.exe (Rev5, Built by MSYS2 project) 4.9.2, 64-bit
- SQLite
  - Version: SQLite version 3.7.3
- Oracle
  - Version: Oracle Database Express Edition 11g Release 2 for Windows x64
- MongoDB
  - Version: 3.0.7

Usage
=============================================================================
1st you have to change the 'connectionStrings' in 'App.config' to your needs.

Read additional information for each Database in it's own class (folder 'DBWriters') start at functions 'CreateDB' or 'SaveDBData'.

Change variables and params on call of DataExtractor.Load(...) and DataToDBs.SaveDataToDBs(...) in 'Program.cs':

    Used data will be downloaded on first start and can be forced by setting param 'downloadData' to true.  
    If there is no data, it will be downloaded always.  
    If you want to store the downloaded data in a different folder, change variable 'projectPath' and 'dataPath' to your needs.  
    The complete folder structure will be created if it doesn't exist.  
    (You need create, modify and delete authorisation on folders and files!!)  
    If your database still exists, change param 'createDatabase' to false. (Database name has to be set in 'connectionStrings'!)  
    Note! If you run the program again without creating the database, be shure to empty the tables yourself before!  
    After all data is inserted it will be tested as count rows inserted to count rows should be inserted.  
    If count rows is not equal, a complete compare of inserted rows to rows that should be inserted will be done.  
    Change 'forceFullCompare' to true, if you always want to compare the rows.  

    Change the database (connectionStrings - name) to be filled in param 'DBType' and then run the application.


**Attention:**  
The Database (Schema) will be **destroyed** before and then created as new!!  
If your database and tables still exist or you have already created them you **must** change the param 'createDatabase' to false!!  
Or remove the create database part from the '\*\_Create\_DB.sql' file in folder 'DBScripts'.  
The scripts to create the database, tables and constraints are splitted because of speed up the inserts.  
All passwords in 'App.config'' are set as 'P_PPP_P'.  
I read it from file 'pwd.txt' in 'DataBuilder' folder.  
This file is **NOT** included in project! You have to create your own file containing only your password or change the function DataDB.GetPassword() to your needs.  
Do **NOT** use 'Password' like this in production! This is **NOT save!** You should store your 'Password' AND 'Username' better elsewhere and as encrypted.
