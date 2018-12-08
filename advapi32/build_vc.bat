@echo off
del advapi32.exp
del advapi32.lib
del advapi32.obj
del advapi32.dll
rem call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin\vcvars32.bat"
rem call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\vcvars32.bat"
call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Auxiliary\Build\vcvars32.bat" x86
cl /O2 advapi32.cpp /LD /Feadvapi32.dll /link /DEF:advapi32.def
