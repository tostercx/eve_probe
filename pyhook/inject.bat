@echo off
injector --process-name exefile.exe --module-name %cd%\pyhook.dll --inject
pause
injector --process-name exefile.exe --module-name %cd%\pyhook.dll --eject
