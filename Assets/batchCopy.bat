@echo off
setlocal enabledelayedexpansion
set output=cs_files.txt
if exist %output% del %output%
for %%f in (*.cs) do (
  echo File: %%f >> %output%
  type %%f >> %output%
  echo. >> %output%
)
echo Done!