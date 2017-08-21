#pragma once

#ifdef NATIVESERIALCONNECTION_EXPORTS
#define NATIVESERIALCONNECTION_API __declspec(dllexport)
#else
#define NATIVESERIALCONNECTION_API __declspec(dllimport)
#endif

typedef char * string;

typedef const string cstring;

NATIVESERIALCONNECTION_API HANDLE OpenSerialPort(cstring portName, int readTimeout);

NATIVESERIALCONNECTION_API BOOL CloseSerialPort(HANDLE portHandle);

NATIVESERIALCONNECTION_API string ReadLine(HANDLE portHandle);

NATIVESERIALCONNECTION_API void FreeLine(string line);

NATIVESERIALCONNECTION_API int WriteLine(HANDLE portHandle, cstring line);
