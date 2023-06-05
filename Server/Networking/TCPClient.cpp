#include "TCPClient.h"
#include "iostream"
#include <list>
#include <chrono>
#include <thread>
#include "DBHandler.h"
#include "File.h"
#include "SHA256.h"

extern std::list<Client*> TCPClientList;
constexpr int BufferSize = 4096;

void Client::OnClientConnect()
{
	std::cout << "Client Connected - Time( " << time(0) << " )" << "IP( " << Client::IpAddress << " )" << "\n";
	std::thread thread([&]
		{
			ClientThread(); // create a thread for the client
		});
	thread.detach();
}
std::string Client::OnKeyRedeem(std::string PacketContent)
{
	std::string Key = PacketContent;
	Database db;
	if (!Client::LoggedIn)
		return "Packet Loss Error";

	std::string Ret = db.RedeemProduct(Client::Username, Key);
	return Ret;
}


void Client::ClientThread()
{
	/*
	So if you dont know much about networking this screams WTF!!! NO NO NO DONT. Luckily windows allows us to have 1000+ while true threads running due to them all basically just going to recv
	recv is allowing us to basically remove all the thread time as our while true is only hit down to the recv and then cool windows magic schedules it on the thread manager.
	I have ran this open with loads of users and constantly open never removing old threads and never had over 10% cpu usage on an 8 core server.
	*/
	while (true)
	{

		if (!Client::SentKey)
			return; // dead end function
		if (Client::Dead)
			break; // dead, end loop
		std::string Message = Client::ReceiveText(); // halts everything here, goes to recieve a message
		if (Message.size() == 0)
			return; // this single line prevents dead clients using loads of cpu
		if (Message == "Ping") // basic heartbeat system
		{
			/*
			If a debugger is attached then a breakpoint will be activated likely preventing execution to be fast enough
			Since execution is too slow they miss the heartbeat, run out of time and they cant debug anymore. This has actually worked very will in some of my future eft projects defending against russian morons
			*/
			std::cout << time(0) << " \n";
			Client::HeatbeatTime = time(0) + 75;
			std::cout << Client::HeatbeatTime << "\n"; // debug
		}
		if (time(0) > Client::HeatbeatTime)
		{
			std::cout << "Heartbeat Dead" << "\n";
			Client::OnClientDisconnect(); // kill the client, failed to respond to heartbeat

		}
		if (Message == "Disconnected") // this is called on exit by the client so we expect to hear this to dispose of the client
			Client::OnClientDisconnect();
		if (Message.substr(0, 7) == "Version")
		{
			Database database; // database instance
			std::string version = Message.substr(7, Message.length() - 7);
			if (std::to_string(Client::ClientVersion) == version) // compare the version
				Client::SendText("Valid Version"); // tell the client that its valid
			else
			{
				ByteArray content = database.GetStreamFile("Client.exe"); // load client into memory, hindsight we should use a dictionary
				// so we have to send the client size of the array then we can send the client
				Client::SendText("Invalid Version"); // tell the client version is invalid
				Client::InvalidVersion = true;
			}

		}
		if (Message.substr(0, 8) == "Version1" && InvalidVersion)
		{
			// client has told us the version invalid has been recieved
			Database database; 
			ByteArray content = database.GetStreamFile("Client.exe");// load client into memory, hindsight we should use a dictionary
			File versionfile;
			versionfile.Array = content;
			versionfile.TCPClient = this;
			versionfile.SendFile(); // send the client
		}
		if (!InvalidVersion)
		{
			if (Message.substr(0, 8) == "Register") // user wants to register
			{
				if (!Message.length() > 8)
					continue;
				if (!(Message.find("|") != std::string::npos)) // check if we have our seperating character
					continue; // see if the register command was set up correctly

				std::string RegisterString = Message.substr(8, Message.length() - 8); // cut out the register
				std::string Ret = Client::OnClientRegister(RegisterString); /// register the client
				Client::SendText(Ret); // send the response from the register function
				std::cout << Ret << "\n";

			}
			if (Message.substr(0, 6) == "Redeem") // redeeming a key
			{
				if (!Message.length() > 6)
					continue;
				if (!(Message.find("-") != std::string::npos)) // check if we have our seperating character
					continue;
				std::string KeyString = Message.substr(6, Message.length() - 6);
				std::string Ret = Client::OnKeyRedeem(KeyString);
				std::cout << Ret << "\n";
				Client::SendText(Ret);


			}
			if (Message.substr(0, 8) == "DataSize") // get screenshot file size
			{
				std::string size = Message.substr(8, Message.length() - 8);
				ScreenshotSize = size;
				Client::SendText("ok"); // tell them we recieved it
			}
			if (Message == "GetProducts") // get product list for the user
			{
				std::cout << Message << "\n";
				Database database;

				if (Client::LoggedIn)
					Client::SendText(database.GetActiveProducts(Client::Username)); // loop our products


				std::cout << database.GetActiveProducts(Client::Username) << "\n"; // debug print them
			}
			if (Message == "SendCheat")
			{
				/*
				For this we must explain the injection method:
				So we inject our cheat by using a legit file, taking it and placing it in the base directory
				if you're familiar with .net 7 you may know that load a dll from the base directory file if its a dependency part of .net's config update that happened in .net 7
				*/
				Database database;
				if (LoggedIn)
				{

					ByteArray content = database.GetStreamFile(Client::Username, "7Days", "0Harmony.dll"); // get our modified harmony dll, send it to the client
					File file;
					file.TCPClient = this;
					file.Array = content;
					file.SendFile();
				}
				else
				{
					database.BanUser(Client::Username, "Banned For: SnC"); // bad requestm ban. Maybe error handle this a bit more?
				}
			}
			if (Message == "SendOriginal")
			{
				// send the original harmony so we can replace the modified one so it doesn't raise any eyebrows with integrity errors
				Database database;
				if (LoggedIn)
				{
					ByteArray content = database.GetStreamFile(Client::Username, "7Days", "Original0Harmony.dll");
					File file;
					file.TCPClient = this;
					file.Array = content;
					file.SendFile();
				}
				else
				{
					database.BanUser(Client::Username, "Banned For: SnO");
				}
			}
			if (Message.substr(0, 5) == "Login")
			{
				if (!Message.length() > 5)
					continue; // blank login
				if (!(Message.find("|") != std::string::npos)) // check if we have our seperating character
					continue; 

				std::string LoginString = Message.substr(5, Message.length() - 5);
				std::string Ret = Client::OnClientLogin(LoginString); // process the login
				Client::SendText(Ret);
				std::cout << Ret << "\n";

			}
			if (Message == "SendingMuchNeededInformation") 
			{
				// screenshot

				if (!Client::LoggedIn)
					continue; // check a login

				std::cout << "Screenshotted" << "\n";
				Database db;
				File file;
				file.TCPClient = this;
				file.GetFile(); // recieve the screenshot

				//	if (std::to_string(bytearray.size()) == Client::ScreenshotSize)
				db.StoreScreenshot(file.Array, Client::Username); // save it to disk
				Client::ScreenShotted = true;
				std::cout << "finished \n";

			}


			if (Client::Socket == INVALID_SOCKET)
				Client::OnClientDisconnect();
		}
	}
}
std::string Client::OnClientLogin(std::string PacketContent)
{
	Database database;

	int specialchar = 0;
	int specialcharpos[4098];  // temporary awful fix to prevent people spamming the "|" char and breaking the server
	std::string character = "|";
	try
	{
		for (std::string::size_type i = 0; i < PacketContent.size(); i++) // check every char to see how many of our seperators we have, allows us to check if someone used it in their username or password and return an error
		{
			if (PacketContent[i] == character[0])
			{
				specialcharpos[specialchar] = i;
				specialchar++;
			}
		}


		//	std::cout << "Special Char: " << specialcharpos[2] - specialcharpos[1] << "\n";
		if (specialchar != 4)
			return "Invalid Character Used";

		std::string Username = PacketContent.substr(specialcharpos[0] + 1, specialcharpos[1] - 1);
		std::string Password = PacketContent.substr(specialcharpos[1] + 1, (specialcharpos[2] - specialcharpos[1]) - 1);
		std::string Hwid = PacketContent.substr(specialcharpos[3] + 1, ((specialcharpos[4] - specialcharpos[3]) - 3));
		std::string ReadableHwid = PacketContent.substr(specialcharpos[2] + 1, ((specialcharpos[3] - specialcharpos[2]) - 2));

		std::string Result = database.LoginUser(Username, sha256(Password), Hwid, ReadableHwid, Client::IpAddress);
		if (Result != "Successful Login")
			return Result;

		Client::LoggedIn = true;
		Client::Username = Username;
	
		Client::SendText(Result);
		return Result;
	}
	catch (const std::exception&)
	{
		return "Failed";
	}
}
std::string Client::OnClientRegister(std::string PacketContent)
{
	// register clients and check them all lowercase. 
	Database database;
	int specialchar = 0;
	int specialcharpos[4098];  // temporary awful fix to prevent people spamming the "|" char and breaking the server
	std::string character = "|";
	try
	{
		for (std::string::size_type i = 0; i < PacketContent.size(); i++) // check every char to see how many of our seperators we have, allows us to check if someone used it in their username or password and return an error
		{
			if (PacketContent[i] == character[0])
			{
				specialcharpos[specialchar] = i;
				specialchar++;
			}
		}


		if (specialchar != 4)
			return "Invalid Character Used";
		

		std::string Username = PacketContent.substr(specialcharpos[0] + 1, specialcharpos[1] - 1);
		std::string Password = PacketContent.substr(specialcharpos[1] + 1, (specialcharpos[2] - specialcharpos[1]) - 1);
		std::string Hwid = PacketContent.substr(specialcharpos[3] + 1, ((specialcharpos[4] - specialcharpos[3]) - 3));
		std::string ReadableHwid = PacketContent.substr(specialcharpos[2] + 1, ((specialcharpos[3] - specialcharpos[2]) - 2));
		return database.CreateUser(Username, sha256(Password), Hwid, ReadableHwid, Client::IpAddress);
	}
	catch (const std::exception&)
	{
		return "Failed";
	}

}
void Client::OnClientDisconnect()
{

	closesocket(Client::Socket);
	Client::Dead = true;

}
ByteArray Client::GetEncryptionKey()
{
	return Client::Encryption.GetKey();
}
int Client::SendRawBytes(ByteArray& Bytes)
{
	int32_t Result = send(Client::Socket, (char*)Bytes.data(), (int)Bytes.size(), 0);

	std::cout << "[ => ] Sending bytes to " << Client::IpAddress << "\n";
	return Result;
}
void Client::SendRawText(std::string Text)
{

}
ByteArray Client::ReceiveRawBytes()
{
	ByteArray	ReceivedBytes;
	uint8_t		RecvBuffer[BufferSize];

	while (true)
	{
		int32_t Received = recv(Client::Socket, (char*)RecvBuffer, BufferSize, 0);

		if (Received < 0)
			break;

		for (int n = 0; n < Received; ++n)
		{
			ReceivedBytes.push_back(RecvBuffer[n]);
		}

		if (Received < BufferSize)
			break;

	}

	return ReceivedBytes;
}
void Client::SendBytes(ByteArray& Bytes)
{
	// Encrypt outgoing data.
	ByteArray Encrypted = Encryption.Encrypt(Bytes);

	SendRawBytes(Encrypted);
}
int Client::_SendBytes(ByteArray& Bytes)
{

	ByteArray Encrypted = Encryption.Encrypt(Bytes);


	return SendRawBytes(Encrypted);
}
ByteArray Client::ReceiveBytes()
{
	ByteArray ReceivedBytes = ReceiveRawBytes();

	// Decrypt incoming data.
	ByteArray Decrypted = Encryption.Decrypt(ReceivedBytes);

	return Decrypted;
}
void Client::SendText(std::string Text)
{

	std::string Send = Text;
	ByteArray plaintext(Send.begin(), Send.end());
	Client::SendBytes(plaintext);
}

std::string Client::ReceiveText()
{
	ByteArray rray = Client::ReceiveBytes();

	std::string str(rray.begin(), rray.end());

	return str;
}
