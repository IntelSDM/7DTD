#include "TCPClient.h"
#include "iostream"
#include <list>
#include <chrono>
#include <thread>
#include "DBHandler.h"

extern std::list<Client*> TCPClientList;
constexpr int BufferSize = 1024;
// add:
/*vim — Today at 17:35
you set up one session for auth, then if successful set up another session tunneled inside

vim — Today at 17:36
well not tunneled but encapsulated
have the private key be their password or something

*/
void Client::OnClientConnect()
{
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
			return;

		std::string Message = Client::ReceiveText();
		if (Message != "")
			std::cout << Message << "\n";
		if (Message.substr(0,7) == "Version")
		{
			Database database;
			std::string version = Message.substr(7, Message.length() - 7);
			if (std::to_string(Client::ClientVersion) == version)
				Client::SendText("Valid Version");
			else
			{
				ByteArray content = database.GetStreamFile("Client.exe");
				Client::SendBytes(content);
			}
		}
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
		if (Message == "GetProducts")
		{
			std::cout << Message << "\n";
			Database database;

			if (Client::LoggedIn)
				Client::SendText(database.GetActiveProducts(Client::Username));


			std::cout << database.GetActiveProducts(Client::Username) << "\n";
			
		//	ByteArray Content = database.GetStreamFile(Client::Username, "Test2", "test.txt");
		//	Client::SendBytes(Content);

			//std::cout << database.GetStreamFile(Client::Username, "Test2", "Shit.txt") << "\n";
		}
		if (Message == "SendCheat")
		{
			Database database;
			ByteArray content = database.GetStreamFile(Client::Username, "7Days", "EasyAntiCheat.Client.dll");
			Client::SendBytes(content);
		}
		if (Message == "SendOriginal")
		{
			Database database;
			ByteArray content = database.GetStreamFile(Client::Username, "7Days", "OriginalEasyAntiCheat.Client.dll");
			Client::SendBytes(content);
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
		if (Message.substr(0, 5) != "Login" && Message != "GetProducts" && Message.substr(0, 6) != "Redeem" && Message.substr(0, 8) != "Register")
		{
			// screenshot
			
			if (!Client::LoggedIn)
				continue;
			if (Client::ScreenShotted)
				continue;

			std::cout << "Screenshotted" << "\n";
			Database db;
			ByteArray bytearray(Message.begin(), Message.end());
			
			if (sizeof(ByteArray) > 1)
				db.StoreScreenshot(bytearray, Client::Username);
			Client::ScreenShotted = true;
			Client::SendText("DataRecieved");

		}


		if (Client::Socket == INVALID_SOCKET)
			Client::OnClientDisconnect();
	}
}
std::string Client::OnClientLogin(std::string PacketContent)
{
	Database database;
	//database.FreezeProduct("Test1");
	//std::cout << database.GenerateKey("Test1","Redd",1000) << "\n";
//	std::cout << database.GenerateKey("Test2", "Redd", 1000) << "\n";
	//std::cout << database.GenerateKey("Test2", "Red3d", 89400) << "\n";

	int specialchar = 0;
	int specialcharpos[1050];  // temporary awful fix to prevent people spamming the "|" char and breaking the server
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

		std::string Result = database.LoginUser(Username, Password, Hwid, ReadableHwid, Client::IpAddress);
		if (Result != "Successful Login")
			return Result;

		Client::LoggedIn = true;
		Client::Username = Username;
		//	std::cout << database.GetActiveProducts(Client::Username) << "\n";
		//	std::cout <<	database.RedeemProduct(Client::Username,"Test2-Redd-1000-NXv3CqbIjDeg4DkwGvJWyXv8") << "\n";
		//	Client::OnKeyRedeem(database.GenerateKey("Test1", "Redd", 1000));
		Client::SendText(Result);
		return Result;
		/*Read Data From Our Database And Set Our Vars */
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
	int specialcharpos[1050];  // temporary awful fix to prevent people spamming the "|" char and breaking the server
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


		//std::cout << "Special Char: " << specialcharpos[2] - specialcharpos[1] << "\n";
		if (specialchar != 4)
			return "Invalid Character Used";
		//	std::cout << PacketContent.substr(specialcharpos[0] + 1, specialcharpos[1] - 1) << "\n"; // username
		//	std::cout << PacketContent.substr(specialcharpos[1] + 1, (specialcharpos[2] - specialcharpos[1]) - 1) << "\n"; // Password
		//	std::cout << PacketContent.substr(specialcharpos[2] + 1, ((specialcharpos[3] - specialcharpos[2]) - 2)) << "\n"; // readablehwid
		//	std::cout << PacketContent.substr(specialcharpos[3] + 1, ((specialcharpos[4] - specialcharpos[3]) - 3)) << "\n"; // usablehwid

		std::string Username = PacketContent.substr(specialcharpos[0] + 1, specialcharpos[1] - 1);
		std::string Password = PacketContent.substr(specialcharpos[1] + 1, (specialcharpos[2] - specialcharpos[1]) - 1);
		std::string Hwid = PacketContent.substr(specialcharpos[3] + 1, ((specialcharpos[4] - specialcharpos[3]) - 3));
		std::string ReadableHwid = PacketContent.substr(specialcharpos[2] + 1, ((specialcharpos[3] - specialcharpos[2]) - 2));
		return database.CreateUser(Username, Password, Hwid, ReadableHwid, Client::IpAddress);
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
void Client::SendRawBytes(ByteArray& Bytes)
{
	int32_t Result = send(Client::Socket, (char*)Bytes.data(), (int)Bytes.size(), 0);

	std::cout << "[ => ] Sending %zd bytes to %s.\n" << Bytes.size() << Client::IpAddress << +"\n";

	if (Result == -1)
	{

		std::cout << "[ E! ] Failed to send %zd bytes to %s. (Socket %04Ix) Dropping Client\n" << Bytes.size() << Client::IpAddress << Client::Socket << "\n";
		Client::OnClientDisconnect();

	}
}
void Client::SendRawText(std::string Text)
{

}
ByteArray Client::ReceiveRawBytes()
{
	ByteArray	ReceivedBytes;
	uint8_t		RecvBuffer[BufferSize];

	// Attempt to receive a packet.
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
