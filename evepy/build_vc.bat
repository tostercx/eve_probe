@echo off
@del evepy.exe
@del evepy.obj
@call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\bin\vcvars32.bat"
@call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\vcvars32.bat"
cl /O2 evepy.cpp
