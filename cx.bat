@echo off

set MYDATE=%date:~10,4%-%date:~7,2%-%date:~4,2%
set OUTPUTDIR=%USERPROFILE%\Checkmarx
set OUTPUTFILE=%OUTPUTDIR%\Checkmarx %MYDATE%.html

mkdir "%OUTPUTDIR%" 2> nul

checkmarxreports not-false-positives -s checkmarx.gmgmt.dimensiondata.com -x "Ceryx Cloud Control" -o "%OUTPUTFILE%"

start "%OUTPUTFILE%" "%OUTPUTFILE%"
