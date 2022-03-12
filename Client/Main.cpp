#define _CRT_SECURE_NO_WARNINGS
#include <winsock2.h>
#include <ws2tcpip.h>
#include <string>
#include "TCPClient.h"
#include <iostream>
#pragma comment(lib,"WS2_32")
#include <Xorstr.h>
#include "hwid.h"
#include <fstream>
#include <sstream>
#include "Screenshot.h"



#pragma comment(lib, "gdiplus.lib")
#pragma comment(lib, "ws2_32.lib")

//https://stackoverflow.com/questions/34444865/gdi-take-screenshot-multiple-monitors
// https://superkogito.github.io/blog/CaptureScreenUsingGdiplus.html
// Send the Screenshot to the server and dont let it touch disk, on the server do a file size check to prevent malicious abuse.

/*
TODO
Add hwid and screenshot cpp files
fix server from trying to find a subscription when none are valid
Loader Version Check
*/


bool LoggedIn = false;
std::string LoginText;
extern ByteArray screenshot;

Client* TCPClient = new Client;
std::string ActivateProduct(std::string Product)
{

}
void Register(std::string Username, std::string Password)
{


	TCPClient->SendText(LIT("Register|") + Username + LIT("|") + Password + LIT("|") + ReadableHwid() + LIT("|") + Hwid());

	while (true)
	{
		std::string Message = TCPClient->ReceiveText();
		if (Message == "")
			continue;
		if (Message == LIT("Successful Login"))
		{
			LoggedIn = true;
			LoginText = Message;
			break;
		}
		LoginText = Message;
		std::cout << Message << "\n";
		break;

	}

}
void Login(std::string Username, std::string Password)
{
	

	TCPClient->SendText(LIT("Login|") + Username + LIT("|") + Password + LIT("|") + ReadableHwid() + LIT("|") + Hwid()); // the order is kinda random to be somewhat confusing to people i guess
	while (true)
	{
		std::string Message = TCPClient->ReceiveText();
		if (Message == "")
			continue;
		if (Message == LIT("Successful Login"))
		{
			LoggedIn = true;
			LoginText = Message;
			break;
		}
		LoginText = Message;
		std::cout << Message << "\n";
		break;

	}
}
void main()
{


	std::string ipAddress = LIT("127.0.0.1");
	int port = 54000;


	WSAData data;
	WORD ver = MAKEWORD(2, 2);
	int wsResult = WSAStartup(ver, &data);
	if (wsResult != 0)
	{
		return;
	}

	SOCKET sock = socket(AF_INET, SOCK_STREAM, 0);
	if (sock == INVALID_SOCKET)
	{
		WSACleanup();
		return;
	}

	sockaddr_in hint;
	hint.sin_family = AF_INET;
	hint.sin_port = htons(port);
	inet_pton(AF_INET, ipAddress.c_str(), &hint.sin_addr);

	int connResult = connect(sock, (sockaddr*)&hint, sizeof(hint));
	if (connResult == SOCKET_ERROR)
	{
		std::cerr << LIT("Can't connect to server") << std::endl;
		closesocket(sock);
		WSACleanup();
		return;
	}

	// create our client class.
	ByteArray array;

	TCPClient->Socket = sock;
	Encryption Encryption;
	Encryption.Start();
	TCPClient->Encryption = Encryption;
	TCPClient->GetEncryptionKey();


	// only use readable hwid so we can easily find disk serials and mobo serials and processorid to ban people with

	std::string Input;
	std::string Username;
	std::string Password;
	std::string DataText;
	std::string Products;

	std::cout << LIT("1) Login\n");
	std::cout << LIT("2) Register\n");
	std::cin >> Input;

	if (Input != LIT("1") && Input != LIT("2"))
		return;

	std::cout << std::string(100, '\n');
	if (Input == LIT("1"))
	{
		std::cout << LIT("Username: ");
		std::cin >> Input;
		Username = Input;
		std::cout << LIT("Password: ");
		std::cin >> Input;
		Password = Input;
		Login(Username, Password);
		if (!LoggedIn)
		{
			return;
		}
		Screenshot();
		TCPClient->SendBytes(screenshot);
		while (true)
		{
			std::string Message = TCPClient->ReceiveText();
			if (Message == "")
				continue;
			if (Message == LoginText)
				continue;
			DataText = Message;
			break;


		}

		TCPClient->SendText(LIT("GetProducts"));
		while (true)
		{
			std::string Message = TCPClient->ReceiveText();
			if (Message == "")
				continue;
			if (Message == LoginText)
				continue;
			Products = Message;
			break;


		}

		if (Products == LIT("No Active Products"))
			std::cout << Products << "\n";
		else
		{
			std::istringstream input;
			std::string str;
			input.str(Products);
			while (std::getline(input, str))
			{
				if (str == LIT(""))
					continue;
				std::string character = LIT("-");
				int specialchar = 0;
				int specialcharpos[200];
				for (std::string::size_type i = 0; i < str.size(); i++)
				{
					if (str[i] == character[0])
					{
						specialcharpos[specialchar] = i;
						specialchar++;
					}
				}
				std::string ProductName = str.substr(0, specialcharpos[0]);
				std::string ProductTime = str.substr(specialcharpos[1] + 1, specialcharpos[1]);
				std::cout << ProductName << LIT(" ") << ProductTime << LIT(" Days Left") << "\n";
			}
		}

		std::cout << LIT("1) Activate Key\n");
		if (Products != LIT("No Active Products"))
			std::cout << LIT("2) Load Cheat\n");

		std::cin >> Input;
		if (Input != LIT("1") && Input != LIT("2"))
			return;

		if (Input == LIT("1"))
		{
			std::cin >> Input;
			TCPClient->SendText(LIT("Redeem") + Input);
			while (true)
			{
				std::string Message = TCPClient->ReceiveText();
				if (Message == "")
					continue;
				std::cout << Message << "\n";
				break;
			}
			closesocket(sock);
			WSACleanup();
			return;
		}
		if (Products != LIT("No Active Products"))
		{
			if (Input == LIT("2"))
			{
				TCPClient->SendText(LIT("Load7DTD"));

			}
		}

		//std::cout << Products << "\n";

	}

	if (Input == LIT("2"))
	{
		std::cout << LIT("Username: ");
		std::cin >> Input;
		Username = Input;
		std::cout << LIT("Password: ");
		std::cin >> Input;
		Password = Input;
		Register(Username, Password);
		closesocket(sock);
		WSACleanup();
		return;
	}



	while (true)
	{
		//std::string Message = TCPClient->ReceiveText();
	//	std::cout << Message << "\n";

	}
	closesocket(sock);
	WSACleanup();

}