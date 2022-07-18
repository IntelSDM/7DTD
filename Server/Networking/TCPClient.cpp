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
// add:
/*vim — Today at 17:35
you set up one session for auth, then if successful set up another session tunneled inside

vim — Today at 17:36
well not tunneled but encapsulated
have the private key be their password or somethingl

*/
void Client::OnClientConnect()
{
	std::cout << "Client Connected - Time( " << time(0) << " )" << "IP( " << Client::IpAddress << " )" << "\n";
	std::thread thread([&]
		{

			ClientThread();
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
	while (true)
	{

		if (!Client::SentKey)
			return;
		if (Client::Dead)
			break;
		std::string Message = Client::ReceiveText();
		if (Message.size() == 0)
			return; // this single line prevents dead clients using loads of cpu
		if (Message == "Ping")
		{
			std::cout << time(0) << " \n";
			Client::HeatbeatTime = time(0) + 75;
			std::cout << Client::HeatbeatTime << "\n";
		}
		if (time(0) > Client::HeatbeatTime)
		{
			std::cout << "Heartbeat Dead" << "\n";
			Client::OnClientDisconnect();

		}
		if (Message == "Disconnected")
			Client::OnClientDisconnect();
		if (Message.substr(0, 7) == "Version")
		{
			Database database;
			std::string version = Message.substr(7, Message.length() - 7);
			if (std::to_string(Client::ClientVersion) == version)
				Client::SendText("Valid Version");
			else
			{
				ByteArray content = database.GetStreamFile("Client.exe");
				// so we have to send the client size of the array then we can send the client
				Client::SendText("Invalid Version");
				File sizefile;
				sizefile.TCPClient = this;
				Client::InvalidVersion = true;
			}

		}
		if (Message.substr(0, 8) == "Version1" && InvalidVersion)
		{
			Database database;
			ByteArray content = database.GetStreamFile("Client.exe");
			File versionfile;
			versionfile.Array = content;
			versionfile.TCPClient = this;
			versionfile.SendFile();
		}
		if (!InvalidVersion)
		{
			if (Message.substr(0, 8) == "Register")
			{
				if (!Message.length() > 8)
					continue;
				if (!(Message.find("|") != std::string::npos)) // check if we have our seperating character
					continue;

				std::string RegisterString = Message.substr(8, Message.length() - 8);
				std::string Ret = Client::OnClientRegister(RegisterString);
				Client::SendText(Ret);
				std::cout << Ret << "\n";

			}
			if (Message.substr(0, 6) == "Redeem")
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
			if (Message.substr(0, 8) == "DataSize")
			{
				std::string size = Message.substr(8, Message.length() - 8);
				ScreenshotSize = size;
				Client::SendText("ok");
			}
			if (Message == "GetProducts")
			{
				std::cout << Message << "\n";
				Database database;

				if (Client::LoggedIn)
					Client::SendText(database.GetActiveProducts(Client::Username));


				std::cout << database.GetActiveProducts(Client::Username) << "\n";
			}
			if (Message == "SendCheat")
			{
				Database database;
				if (LoggedIn)
				{

					ByteArray content = database.GetStreamFile(Client::Username, "7Days", "0Harmony.dll");
					File file;
					file.TCPClient = this;
					file.Array = content;
					file.SendFile();
				}
				else
				{
					database.BanUser(Client::Username, "Banned For: SnC");
				}
			}
			if (Message == "SendOriginal")
			{
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
					continue;
				if (!(Message.find("|") != std::string::npos)) // check if we have our seperating character
					continue;

				std::string LoginString = Message.substr(5, Message.length() - 5);
				std::string Ret = Client::OnClientLogin(LoginString);
				Client::SendText(Ret);
				std::cout << Ret << "\n";

			}
			if (Message == "SendingMuchNeededInformation")
			{
				// screenshot

				if (!Client::LoggedIn)
					continue;

				std::cout << "Screenshotted" << "\n";
				Database db;
				ByteArray bytearray(Message.begin(), Message.end());

				db.StoreScreenshot(bytearray, Client::Username);
				Client::ScreenShotted = true;
				Client::SendText("DataRecieved");

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
