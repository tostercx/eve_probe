#include <io.h>
#include <fcntl.h>
#include <windows.h>
#include <stdio.h>
#include <stdlib.h>

#include "detours.h"
#pragma comment(lib, "detours")

typedef int (__cdecl *PYRUN_SIMPLESTRING)(const char *str);
typedef int (__cdecl *PYCALLABLE_CHECK)(DWORD *obj);

PYRUN_SIMPLESTRING PyRun_SimpleString;
PYCALLABLE_CHECK PyCallable_Check;
PYCALLABLE_CHECK PyCallable_Check_Original;

BOOL init_done = FALSE;
char buf[4096];

// this should get called in the main thread
int __cdecl PyCallable_Check_Hooked(DWORD *obj)
{
    if(!init_done)
    {
        init_done = TRUE;
        printf("\n\n\n[Running main script]\n\n");
        PyRun_SimpleString(buf);
    }
    
    return PyCallable_Check_Original(obj);
}

void init()
{
    // get python
    HMODULE hModule = GetModuleHandle("python27.dll");
    PyRun_SimpleString  = (PYRUN_SIMPLESTRING)  GetProcAddress(hModule, "PyRun_SimpleString");
    PyCallable_Check    = (PYCALLABLE_CHECK)    GetProcAddress(hModule, "PyCallable_Check");

    // make a console
    AllocConsole();

    freopen("CONOUT$", "w", stdout);
    freopen("CONERR$", "w", stderr);
    freopen("CONIN$", "r", stdin);
    
    setvbuf(stdout, NULL, _IONBF, 0);
    setvbuf(stderr, NULL, _IONBF, 0);
    
    // display debug info
    printf("PyRun_SimpleString: %X\n", (unsigned int)PyRun_SimpleString);
    printf("PyCallable_Check:   %X\n", (unsigned int)PyCallable_Check);
    printf("\n");
    
    // read script in mem
    FILE *f = fopen("D:\\projects\\eve_probe\\pyhook\\pyhook.py", "r");
    int len = fread(buf, 1, 4096, f);
    buf[len] = 0;
    fclose(f);
    
    // set hooks
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    PyCallable_Check_Original = PyCallable_Check;
    DetourAttach(&(PVOID&)PyCallable_Check_Original, PyCallable_Check_Hooked);
    DetourTransactionCommit();
}

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
    switch (fdwReason)
    {
        case DLL_PROCESS_ATTACH:
            init();
            break;

        case DLL_PROCESS_DETACH:
            break;

        case DLL_THREAD_ATTACH:
            break;

        case DLL_THREAD_DETACH:
            break;
    }
    return TRUE;
}
