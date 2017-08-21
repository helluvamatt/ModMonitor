#include "stdafx.h"
#include "NativeSerialConnection.h"

#define BUFFER_SIZE (64)

#define MAX_LINE_LENGTH (BUFFER_SIZE * 16) // 1024

#define STRING(n) char[n]

int ReadBytes(HANDLE portHandle, HLOCAL buffer, int maxToRead);

BOOL StrEndsWith(string haystack, string needle);

HANDLE OpenSerialPort(cstring portName, int readTimeout)
{
	HANDLE handle = CreateFileA(portName, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
	if (handle == INVALID_HANDLE_VALUE)
	{
		return handle;
	}

	COMMTIMEOUTS commTimeouts = { 0 };
	commTimeouts.ReadIntervalTimeout = -1;
	commTimeouts.ReadTotalTimeoutMultiplier = -1;
	commTimeouts.ReadTotalTimeoutConstant = readTimeout;
	commTimeouts.WriteTotalTimeoutConstant = 0;
	commTimeouts.WriteTotalTimeoutMultiplier = 0;
	if (!SetCommTimeouts(handle, &commTimeouts))
	{
		CloseHandle(handle);
		return INVALID_HANDLE_VALUE;
	}
	return handle;
}

BOOL CloseSerialPort(HANDLE portHandle)
{
	return CloseHandle(portHandle);
}

string ReadLine(HANDLE portHandle)
{
	string line = (string)LocalAlloc(LMEM_ZEROINIT, MAX_LINE_LENGTH);
	int charsRead = 0;
	while (!StrEndsWith(line, "\r") && !StrEndsWith(line, "\n") && !StrEndsWith(line, "\r\n") && charsRead < MAX_LINE_LENGTH)
	{
		HLOCAL buffer = LocalAlloc(0, BUFFER_SIZE);
		int bytesRead = ReadBytes(portHandle, buffer, BUFFER_SIZE);
		if (bytesRead < 1)
		{
			LocalFree(buffer);
			LocalFree(line);
			return NULL;
		}
		if (bytesRead > (MAX_LINE_LENGTH - charsRead)) bytesRead = (MAX_LINE_LENGTH - charsRead);
		memcpy(line + charsRead, buffer, bytesRead);
		charsRead += bytesRead;
		LocalFree(buffer);
	}
	return line;
}

void FreeLine(string line)
{
	LocalFree(line);
}

int WriteLine(HANDLE portHandle, cstring line)
{
	// Line comes to us with newline characters already added
	int written;
	BOOL result = WriteFile(portHandle, line, (int)strlen(line), &written, NULL);
	if (!result)
	{
		written = -1;
	}
	return written;
}

int ReadBytes(HANDLE portHandle, HLOCAL buffer, int maxToRead)
{
	int bytesRead;
	BOOL result = ReadFile(portHandle, buffer, maxToRead, &bytesRead, NULL);
	if (!result)
	{
		bytesRead = -1;
	}
	return bytesRead;
}

BOOL StrEndsWith(string haystack, string needle)
{
	// Trivial case where haystack is smaller than the needle
	if (strlen(haystack) < strlen(needle)) return FALSE;

	int offset = (int)strlen(haystack) - (int)strlen(needle);
	return strcmp(haystack + offset, needle) == 0;
}
