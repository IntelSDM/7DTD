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
	Database db; // db instance
	db.CreateDB(); // creates the database if it doesn't exist
	WSADATA wsData;
	WORD ver = MAKEWORD(2, 2);
	int wsOk = WSAStartup(ver, &wsData); // start the server
	if (wsOk != 0)
	{
		std::cout << "Can't Initialize winsock!" << std::endl;
		return;
	}
	SOCKET listening = socket(AF_INET, SOCK_STREAM, 0); // create socket instance
	if (listening == INVALID_SOCKET)
	{
		std::cout << "Can't create a socket!" << std::endl;
		return;
	}

	sockaddr_in hint;
	hint.sin_family = AF_INET; // declare the ip and port and connection rules
	hint.sin_port = htons(54000);
	hint.sin_addr.S_un.S_addr = INADDR_ANY;

	bind(listening, (sockaddr*)&hint, sizeof(hint)); // bind the connection rules to the listening socket

	listen(listening, SOMAXCONN); // keep socket open

	sockaddr_in client;
	std::thread thread(AcceptClients, listening, client, hint); // client thread to constantly take new clients
	std::thread thread2(TakeInput); // input thread to take input without break pointing the program
	
	thread.join();
	thread2.join();
	closesocket(listening);
	WSACleanup();
	system("pause");
}

void TakeInput()
{
	/*
	This is just basic for now, eventually i will add more commands but this serves only as a 7 days to die cheat so we have basic phrases to generate keys.
	M creates a month key, W Creates a week key and 3 creates a 3 day key. Create key is currently broken.
	*/
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

	}
}

void AcceptClients(SOCKET listening, sockaddr_in client, sockaddr_in hint)
{
/*
This thread will wait for any incomming clients and then accept them. 
It also drops any clients that are assumed dead.
It calls starting function for the client as well which creates the client thread.
*/
	while (true)
	{

		int clientSize = sizeof(client);
		SOCKET socket;
		if ((socket = accept(listening, (SOCKADDR*)&client, &clientSize)) != INVALID_SOCKET) // only act here if accept throws a correct response(valid connection)
		{
			std::string IP = inet_ntoa(client.sin_addr); // get the ip
			Client* CreateTCPClient = new Client; // make a client class
			CreateTCPClient->Socket = socket; // set socket instance
			CreateTCPClient->IpAddress = IP; // set ip 
			Encryption Encryption; // create encryption instance
			Encryption.Start(); // create keys
			CreateTCPClient->Encryption = Encryption; // set the key instance
			ByteArray EncryptionKey = CreateTCPClient->GetEncryptionKey(); // get the key
			CreateTCPClient->SendRawBytes(EncryptionKey); // send the key
			CreateTCPClient->SentKey = true;
			for (Client* TCPClient : TCPClientList)
			{
				// since this is called only on connection, clean up the client list here.

				if (TCPClient->Dead)
					continue;
				// drop old clients
				if (TCPClient->Socket == CreateTCPClient->Socket)
					TCPClient->OnClientDisconnect();

			}
			if (CreateTCPClient->Socket != INVALID_SOCKET)
				TCPClientList.push_back(CreateTCPClient);

			// call our init function for the client
			CreateTCPClient->OnClientConnect();

		}

	}
}