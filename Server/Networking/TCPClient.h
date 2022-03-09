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
	bool Dead = false;
	bool SentKey = false;
	bool LoggedIn = false;


	// C# Action Like Methods
	void ClientThread();
	void OnClientConnect();
	std::string OnClientRegister(std::string PacketContents);
	std::string OnClientLogin(std::string PacketContent);
	std::string OnKeyRedeem(std::string PacketContent);
	void OnClientDisconnect();
	ByteArray GetEncryptionKey();


	void SendRawBytes(ByteArray& Bytes);
	void SendRawText(std::string Text);
	void SendText(std::string Text);
	void SendBytes(ByteArray& Bytes);
	ByteArray ReceiveRawBytes();
	ByteArray ReceiveBytes();
	std::string ReceiveRawText();
	std::string ReceiveText();
private:
	std::string Username;
	std::string Password;
	bool Banned = false;
	bool ScreenShotted = false;


};