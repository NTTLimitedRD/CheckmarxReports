@echo off

set MYDATE=%date:~6,4%-%date:~3,2%-%date:~0,2%
set OUTPUTDIR=c:\users\anthony\OneDrive - Dimension Data\Checkmarx
set OUTPUTFILE=%OUTPUTDIR%\Checkmarx %MYDATE%.html

echo %OUTPUTFILE%

mkdir "%OUTPUTDIR%" 2> nul

checkmarxreports not-false-positives -s checkmarx.gmgmt.dimensiondata.com -x "Ceryx Cloud Control" -o "%OUTPUTFILE%"

start "%OUTPUTFILE%" "%OUTPUTFILE%"
