@echo off
del pyhook.obj
del pyhook.dll
rem yep... VS10!
call "C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\bin\vcvars32.bat"
cl /O2 pyhook.cpp /MD /LD /Fepyhook.dll /link
