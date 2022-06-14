
#pragma once
#include <winsock2.h>
#include <ws2tcpip.h>
#include <string>
#include "Encryption.h"

class Client
{

public:
	SOCKET Socket;
	std::string IpAddress;
	Encryption Encryption;


	// Gets encryption key from server
	void GetEncryptionKey();
	// Gets private encryption key
	ByteArray EKey();

	int SendRawBytes(ByteArray& Bytes);
	void SendText(std::string Text);
	void SendBytes(ByteArray& Bytes);
	int _SendBytes(ByteArray& Bytes);
	ByteArray ReceiveRawBytes();
	ByteArray ReceiveBytes();
	std::string ReceiveText();
private:
	std::string Username;
	std::string Password;
	ByteArray EncryptionKey;

};
