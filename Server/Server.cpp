#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include "TCPClient.h"
#include "DBHandler.h"
#pragma comment(lib,"WS2_32")
#include <iostream>
#include <list>
#include <thread>
#include <fstream>
#include <chrono>
#include <functional>
#include <sstream>


std::list<Client*> TCPClientList;
void AcceptClients(SOCKET listening, sockaddr_in client, sockaddr_in hint);
void TakeInput();

void main()
{
	Database db;
	db.CreateDB();
	//db.UnFreezeProduct("Test2");
	//db.FreezeProduct("Test2");
	
//	db.FreezeProduct("dd");
	WSADATA wsData;
	WORD ver = MAKEWORD(2, 2);
	int wsOk = WSAStartup(ver, &wsData);
	if (wsOk != 0)
	{
		std::cerr << "Can't Initialize winsock! Quitting" << std::endl;
		return;
	}
	SOCKET listening = socket(AF_INET, SOCK_STREAM, 0);
	if (listening == INVALID_SOCKET)
	{
		std::cerr << "Can't create a socket! Quitting" << std::endl;
		return;
	}

	sockaddr_in hint;
	hint.sin_family = AF_INET;
	hint.sin_port = htons(54000);
	hint.sin_addr.S_un.S_addr = INADDR_ANY;

	bind(listening, (sockaddr*)&hint, sizeof(hint));

	listen(listening, SOMAXCONN);

	sockaddr_in client;
	std::thread thread(AcceptClients, listening, client, hint);
	std::thread thread2(TakeInput);
	thread.join();
	thread2.join();
	closesocket(listening);
	WSACleanup();
	system("pause");
}

void TakeInput()
{
	while (true)
	{
		Database database;
		std::string Text;
		std::cin >> Text;
		if (Text.find("M") != std::string::npos)
		{
			std::cout << database.GenerateKey("7Days", "Standard", 2592000) << "\n";
		}
		if (Text.find("W") != std::string::npos)
		{
			std::cout << database.GenerateKey("7Days", "Standard", 604800) << "\n";
		}
		if (Text.find("3") != std::string::npos)
		{
			std::cout << database.GenerateKey("7Days", "Standard", 259200) << "\n";
		}
		if (Text.find("CreateKey") != std::string::npos)
		{
			try
			{
				std::string character = "_";
				int specialchar = 0;
				int specialcharpos[150];
				for (std::string::size_type i = 0; i < Text.size(); i++)
				{
					if (Text[i] == character[0])
					{
						specialcharpos[specialchar] = i;
						specialchar++;
					}
				}

				std::string Product = Text.substr(specialcharpos[0] + 1, specialcharpos[0] - 1);
				//	std::string Time;
				//	std::string Vendor;
				std::cout << Product << "\n";
			}
			catch (std::exception)
			{
				std::cout << "Invalid Command\n";
			}
		}
		//database.GenerateKey();
	}
}

void AcceptClients(SOCKET listening, sockaddr_in client, sockaddr_in hint)
{

	while (true)
	{

		int clientSize = sizeof(client);
		SOCKET socket;
		if ((socket = accept(listening, (SOCKADDR*)&client, &clientSize)) != INVALID_SOCKET)
		{
			std::string IP = inet_ntoa(hint.sin_addr);
			Client* CreateTCPClient = new Client;
			CreateTCPClient->Socket = socket;
			CreateTCPClient->IpAddress = IP;
			Encryption Encryption;
			Encryption.Start();
			CreateTCPClient->Encryption = Encryption;
			ByteArray EncryptionKey = CreateTCPClient->GetEncryptionKey();
			CreateTCPClient->SendRawBytes(EncryptionKey);
			CreateTCPClient->SentKey = true;
			for (Client* TCPClient : TCPClientList)
			{
				if (TCPClient->Dead)
					continue;
				// drop old clients
				if (TCPClient->Socket == CreateTCPClient->Socket)
					TCPClient->OnClientDisconnect();

			}
			if (CreateTCPClient->Socket != INVALID_SOCKET)
				TCPClientList.push_back(CreateTCPClient);


			CreateTCPClient->OnClientConnect();

		}

	}
}