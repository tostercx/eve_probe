// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the ADVAPI32_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// ADVAPI32_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef ADVAPI32_EXPORTS
#define ADVAPI32_API __declspec(dllexport)
#else
#define ADVAPI32_API __declspec(dllimport)
#endif

// This class is exported from the advapi32.dll
class ADVAPI32_API Cadvapi32 {
public:
	Cadvapi32(void);
	// TODO: add your methods here.
};

extern ADVAPI32_API int nadvapi32;

ADVAPI32_API int fnadvapi32(void);
