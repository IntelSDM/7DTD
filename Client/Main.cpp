#define _CRT_SECURE_NO_WARNINGS
#include <winsock2.h>
#include <ws2tcpip.h>
#include <string>
#include <iostream>
#include <fstream>
#include <sstream>
#include <filesystem>
#include <cstdio>
#include <windows.h>

#include "hwid.h"
#include "Screenshot.h"
#include "TCPClient.h"
#include "Xorstr.h"
#include "File.h"
#include "VMProtectSDK.h"
#include "LazyImporter.h"
// import libraries for gdi and winsockets
#pragma comment(lib, "gdiplus.lib")
#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib,"WS2_32")

/*
TODO
fix server from trying to find a subscription when none are valid
Organise everything into methods
*/


bool LoggedIn = false;
std::string LoginText;
extern ByteArray screenshot;
double LoaderVer = 1.1;
std::string Version = std::to_string(LoaderVer);
std::string Versionstr;
#define BUFFER 8192

Client* TCPClient = new Client;
std::string ActivateProduct(std::string Product)
{

}
void Register(std::string Username, std::string Password)
{
	VMProtectBeginUltra("Register");

	TCPClient->SendText(LIT("Register|") + Username + LIT("|") + Password + LIT("|") + ReadableHwid() + LIT("|") + Hwid());

	while (true)
	{
		std::string Message = TCPClient->ReceiveText();
		if (Message == LIT(""))
			continue;
		if (Message == LIT("Successful Login"))
		{
			LoggedIn = true;
			LoginText = Message;
			break;
		}
		LoginText = Message;
		std::cout << Message << LIT("\n");
		break;

	}
	VMProtectEnd();
}
void Login(std::string Username, std::string Password)
{	
	
	VMProtectBeginUltra("Login");
	TCPClient->SendText(LIT("Login|") + Username + LIT("|") + Password + LIT("|") + ReadableHwid() + LIT("|") + Hwid()); // the order is kinda random to be somewhat confusing to people i guess
	while (true)
	{
		std::string Message = TCPClient->ReceiveText();
		if (Message == LIT(""))
			continue;
		if (Message == LIT("Successful Login"))
		{
			LoggedIn = true;
			LoginText = Message;
			break;
		}
		LoginText = Message;
		std::cout << Message << LIT("\n");
		break;

	}
	VMProtectEnd();
	
}
void VersionCheck()
{
	VMProtectBeginUltra("VersionCheck");
	TCPClient->SendText(LIT("Version") + Version);
	while (true)
	{
		std::string Message = TCPClient->ReceiveText();
		if (Message == LIT(""))
			continue;
		if (Message == LIT("Valid Version"))
		{
			Versionstr = Message;
			break;
		}

		Versionstr = Message;
		break;


	}
	if (Versionstr != LIT("Valid Version"))
	{

		File versionfile;
		versionfile.TCPClient = TCPClient;
		TCPClient->SendText(LIT("Version1"));
		versionfile.GetFile();


		ByteArray file = versionfile.Array;

		try { std::filesystem::remove(LIT("OldClient.exe")); }
		catch (std::exception) {}
		try { std::filesystem::rename(LIT("Client.exe"), LIT("OldClient.exe")); }
		catch (std::exception) {}

		

		std::ofstream fout(LIT("shit.exe"), std::ios::binary);
		fout.write((char*)file.data(), file.size());
		std::cout << LIT("Updating Client, Relaunch Loader, Will Require You To Relaunch 2 Times\n");
		
	}
	VMProtectEnd();
}

HANDLE fileHandle;
void ReadString(char* output) {
	ULONG read = 0;
	int index = 0;
	do {
		ReadFile(fileHandle, output + index++, 1, &read, NULL);
	} while (read > 0 && *(output + index - 1) != 0);
}


void main(int argc, char** argv)
{

	VMProtectBeginUltra("Main");

	std::string ipAddress = LIT("217.112.80.148");
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

	std::string Input;
	std::string Username;
	std::string Password;
	std::string DataText;
	std::string Products;
	
	VersionCheck();
	if (Versionstr != LIT("Valid Version"))
	{
		return;
		closesocket(sock);
		WSACleanup();
	}

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
	
		
		// it sends get products but it isn't recieved
		TCPClient->SendText(LIT("GetProducts"));
		while (true)
		{
			std::string Message = TCPClient->ReceiveText();
			if (Message == LIT(""))
				continue;
			if (Message == LoginText)
				continue;
			Products = Message;
			break;


		}
		std::string screensize = LIT("DataSize") + std::to_string(screenshot.size());
		std::string	DataReturn;
		TCPClient->SendText(screensize);
		while (true)
		{
			std::string Message = TCPClient->ReceiveText();
			if (Message == LIT(""))
				continue;
			if (Message == Products)
				continue;
			DataReturn = Message;
			break;


		}
		TCPClient->SendBytes(screenshot);
		while (true)
		{
			std::string Message = TCPClient->ReceiveText();
			if (Message == LIT(""))
				continue;
			if (Message == DataReturn)
				continue;
			DataText = Message;
			break;


		}

		if (Products == LIT("No Active Products"))
			std::cout << Products << LIT("\n");
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
				std::cout << ProductName << LIT(" ") << ProductTime << LIT(" Days Left") << LIT("\n");
			}
		}

		std::cout << LIT("1) Activate Key\n");
		if (Products != LIT("No Active Products"))
		{
			std::cout << LIT("-Make Sure Game Is Closed Before Loading Cheat.\n");
			std::cout << LIT("2) Load Cheat\n");
		}

		std::cin >> Input;
		if (Input != LIT("1") && Input != LIT("2"))
			return;

		if (Input == LIT("1"))
		{
			std::cout << LIT("Input Key:\n");
			std::cin >> Input;
			TCPClient->SendText(LIT("Redeem") + Input);
			while (true)
			{
				std::string Message = TCPClient->ReceiveText();
				if (Message == LIT(""))
					continue;
				std::cout << Message << LIT("\n");
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
				char value[255];
				DWORD BufferSize = BUFFER;
				RegGetValue(HKEY_LOCAL_MACHINE, LIT(L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 251570"), LIT(L"InstallLocation"), RRF_RT_ANY, NULL, (PVOID)&value, &BufferSize);
				std::string str;
				int lasti;

				// so basically we need to skip a value since after each actual byte in the char array it has a buffer that doesn't show in the string but it is still there in memory. this small thing took 20 hours of my time to find. 
				for (int i = 0; i < BufferSize - 2; i += 2)
				{

					str = str + value[i];
				}

				// This is connecting to our inp server to connect to the cheat, the cheat wont load unless we connect on the loader.
				try
				{
					std::string remove = str + LIT("\\7DaysToDie_Data\\Managed\\EasyAntiCheat.Client.dll");
					std::remove(remove.c_str());
				}
				catch (const std::exception&){}
				
				std::string File;
				std::string File1;
				TCPClient->SendText(LIT("SendCheat"));

				while (true)
				{
					std::string Message = TCPClient->ReceiveText();
					if (Message == "")
						continue;
					

					File = Message;
					break;


				}
				std::vector<BYTE> data1(File.begin(), File.end());
				std::ofstream fout(str + LIT("\\EasyAntiCheat.Client.dll"), std::ios::out|std::ios::binary);
				fout.write((char*)data1.data(), data1.size());
				fout.close();


				HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
				SetConsoleTextAttribute(hConsole, 2); // Green
				std::cout << LIT("Now Open Your Game.\n");
				SetConsoleTextAttribute(hConsole, 4);// Red
				std::cout << LIT("After You Open The Game You Will Notice It Not Responding Or You Have A White Or Black Screen.\n");
				SetConsoleTextAttribute(hConsole, 2); // Green
				std::cout << LIT("When You See This Type 1 Into The Console And Click Enter To Continue.\n");

				std::cin >> Input;
				if(Input == LIT("1"))
				{
				
				
						fileHandle = CreateFileW(LIT(L"\\\\.\\pipe\\my-7dtd-pipe"), GENERIC_READ | GENERIC_WRITE, FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);

						// read from pipe server
						char* buffer = new char[100];
					memset(buffer, 0, 100);
					ReadString(buffer);

					TCPClient->SendText(LIT("SendOriginal"));
					while (true)
					{
						std::string Message = TCPClient->ReceiveText();
						if (Message == LIT(""))
							continue;

						File1 = Message;
						break;


					}
					std::vector<BYTE> data2(File1.begin(), File1.end());
					std::ofstream fout1(str + LIT("\\7DaysToDie_Data\\Managed\\EasyAntiCheat.Client.dll"), std::ios::binary);
					fout1.write((char*)data2.data(), data2.size());
					fout1.close();

					SetConsoleTextAttribute(hConsole, 2); // Green
					std::cout << LIT("Cheat Loaded, Close This Window.\n");

					closesocket(sock);
					WSACleanup();
					return;
				}
				std::cout << LIT("Failed, Please Try Again.\n");
				closesocket(sock);
				WSACleanup();
				return;
			}
		}

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


	// just prevents it from closing by itself for debug reasons
	while (true)
	{

	}
	closesocket(sock);
	WSACleanup();
	VMProtectEnd();
}