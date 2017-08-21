#include "main.h"

int main()
{
	HANDLE port = OpenSerialPort("COM1", 500);
	if (port == INVALID_HANDLE_VALUE)
	{
		int reason = GetLastError();
		switch (reason)
		{
		case ERROR_FILE_NOT_FOUND:
			fprintf(stderr, "Failed to open serial port: The specified port does not exist.\n");
		case ERROR_ACCESS_DENIED:
			fprintf(stderr, "Failed to open serial port: The specified port is in use.\n");
		default:
			fprintf(stderr, "Failed to open serial port: Unknown error.\n");
		}
		return reason;
	}
	WriteLine(port, "E=GET SERIAL\r\n"); // Get board serial number
	string line = ReadLine(port);
	if (line != NULL)
	{
		printf("Got serial number:\n");
		printf(line);
		printf("\n\n");
	}
	else
	{
		fprintf(stderr, "Failed to read from port: %d", GetLastError());
	}

	CloseSerialPort(port);
    return 0;
}

