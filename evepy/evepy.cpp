#include <windows.h>
#include <stdio.h>

typedef int (__stdcall *PYRUN_SIMPLESTRING)(const char *str);
typedef void (__stdcall *PY_INITIALIZE)();

PYRUN_SIMPLESTRING PyRun_SimpleString;
PY_INITIALIZE Py_Initialize;

HMODULE hModule;

BOOL getEvePath(char *path, DWORD len)
{
  HKEY hKey;
  LONG res;
  DWORD type = REG_SZ;
  
  // for steamed versoin on win64
  res=RegOpenKeyEx(HKEY_LOCAL_MACHINE,"Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 8500",0,KEY_ALL_ACCESS | KEY_WOW64_64KEY,&hKey);
  if(res!=ERROR_SUCCESS) return FALSE;
  
  res=RegQueryValueEx(hKey,"InstallLocation", NULL, &type, (LPBYTE)path, &len);
  RegCloseKey(hKey);
  
  if(res!=ERROR_SUCCESS) return FALSE;
  return TRUE;
}

int main()
{
    char buf[8192];
    char path[MAX_PATH];
    
    // get EVE path
    if(getEvePath(path, MAX_PATH))
    {
      strcat(path, "\\SharedCache\\tq");
      printf("EVE dir: %s\n", path);
    }
    else
    {
      printf("Failed to find eve path :(\n");
      return 1;
    }
    
    // MUST be dona before loading the dll
    sprintf(buf, "%s\\code.ccp;%s\\bin", path, path);
    _putenv_s("PYTHONPATH", buf);
    
    // get python
    sprintf(buf, "%s\\bin\\python27.dll", path);
    hModule = LoadLibrary(buf);
    PyRun_SimpleString  = (PYRUN_SIMPLESTRING)  GetProcAddress(hModule, "PyRun_SimpleString");
    Py_Initialize       = (PY_INITIALIZE)       GetProcAddress(hModule, "Py_Initialize");
    
    // display debug info
    printf("PyRun_SimpleString: %X\n", (unsigned int)PyRun_SimpleString);
    printf("Py_Initialize:      %X\n", (unsigned int)Py_Initialize);
    printf("\n");
    
    Py_Initialize();
    
    PyRun_SimpleString("import sys, code");
    PyRun_SimpleString("print 'Testing, testing, 123'");
    PyRun_SimpleString("code.interact()");
    
    return 0;
}
