#define _CRT_SECURE_NO_WARNINGS
#include "File.h"
#include <iostream>
#include <ctime>
#include "DBHandler.h"
int length;

void File::GetFile()
{
	/*
	Take the size, we know it will contain no characters so we can just use stoi to get an int value
	Change the array size to the size of the file
	Until that size is reached it will listen for incomming bytes
	*/

	File::TCPClient->HeatbeatTime = time(0) + 75;
	std::string testtxt = File::TCPClient->ReceiveText();
	size_t Size = ntohl(std::stof(testtxt));
	constexpr size_t ChunkSize = 4096;
	size_t Total = 0;

	while (Size > 0)
	{

		ByteArray Bytes = File::TCPClient->ReceiveRawBytes();
		if (Bytes.size() <= 0) {
			break;
		}
		if (Size < Bytes.size()) // last packet will be under the byte size
			break;
		Size -= Bytes.size();
		Total += Bytes.size();
		for (uint8_t byte : Bytes)
		{
			File::Array.push_back(byte);

		}
	}
	File::TCPClient->Encryption.Decrypt(File::Array);

}
template<typename T>
std::vector<T> Slice(std::vector<T> const& v, int m, int n)
{
	/*
	Uses a template to pass any data type
	Calculates the position of our first and end data
	Returns a new vector list of our data inbetween the stated values
	*/
	auto first = v.cbegin() + m;
	auto last = v.cbegin() + n;
	std::vector<T> vec(first, last);

	return vec;
}

void File::SendFile()
{
	/*
	Encrypts the array of data
	Calculates the size of the encrypted array and sends it to the node
	Splits the array into sendable amounts(4096 bytes) and sends them
	When the size is 0 or going to be 0 after the next packet it breaks the loop
	*/
	File::TCPClient->HeatbeatTime = time(0) + 75;
	size_t Size = File::Array.size();
	size_t NetworkSize = htonl(Size);
	File::TCPClient->SendText(std::to_string(NetworkSize));
	
	size_t Sent = 0;
	int i = 0;
	int iiGet;
	while (Size > 0) {
		iiGet = (Size < 4095) ?
			Size : 4095; // the packet is actually 4096 but randomly the size is always size+1, not sure why
		std::cout << iiGet << "\n";
		ByteArray Bytes = Slice(File::Array, i * 4095, (i * 4095) + iiGet);
		File::TCPClient->SendRawBytes(Bytes);
		if (Bytes.size() <= 0)
			break;
		if (Size < Bytes.size()) // last packet will be under the byte size
			break;
		Sent += Bytes.size();
		Size -= Bytes.size();
		i++;
	}
}
