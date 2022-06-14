#pragma once
#include "TCPClient.h"

class File
{
public:
	void GetFile();
	void SendFile();
	ByteArray Array;
	int Left;
	Client* TCPClient;
private:
	int BytesLeftToRecieve;
	bool Recieved = false;
};