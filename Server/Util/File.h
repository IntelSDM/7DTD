#pragma once
#include "TCPClient.h"

class File
{
public:
	void GetFile();
	void SendFile();

	ByteArray Array;
	Client* TCPClient;
};