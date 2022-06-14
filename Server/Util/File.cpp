#define _CRT_SECURE_NO_WARNINGS
#include "File.h"
#include <iostream>
#include <ctime>
#include "DBHandler.h"
// https://www.codeproject.com/Articles/8701/Network-Transfer-Of-Files-Using-MFC-s-CSocket-Clas
/*
Send all the bytes in small amounts 4096 kb to be exact
once it is all sent we will wait 1 second and then send "FileSent" and this will break the waiting loop on client
we could calculate size on the client but i don't like the chances of inaccuracy
we can also take the time it takes to finish the transaction to prevent long connections
we send the bytes encrypted and recieve them raw, rebuild the byte array in the client and then decrypt the byte array
*/
int length;

void File::GetFile()
{
	size_t Size = stoi(File::TCPClient->ReceiveText());

	Size = ntohl(Size);
	File::Array.resize(Size);

	constexpr size_t ChunkSize = 4096;
	size_t Total = 0;

	while (Size > 0) {
		//	int ret = read(&out[total], to_read);
		ByteArray Bytes = File::TCPClient->ReceiveBytes();
		if (Bytes.size() <= 0) {
			break;
		}

		Size -= Bytes.size();
		Total += Bytes.size();
	}

	std::cout << "Size: " << Size << "\n";
	std::cout << "Total: " << Total << "\n";

}
template<typename T>
std::vector<T> Slice(std::vector<T> const& v, int m, int n)
{
	auto first = v.cbegin() + m;
	auto last = v.cbegin() + n;
	std::cout << "Sliced" << "\n";
	std::cout << "First:" << m << "\n";
	std::cout << "Last:" << n << "\n";
	std::cout << "Size:" << v.size() << "\n";
	std::vector<T> vec(first, last); // this causes issues


	return vec;
}

void File::SendFile()
{
	size_t Size = File::Array.size();
	size_t NetworkSize = htonl(Size);
	File::TCPClient->SendText(std::to_string(NetworkSize));
	// make new bytearray of the data we want to actually send
	size_t Sent = 0;
	int i = 0;
	int iiGet;
	while (Size > 0) {
		iiGet = (Size < 4095) ?
			Size : 4095; // the packet is actually 4096 but randomly the size is always size+1, not sure why
		std::cout << "Size: " << Size << "\n";
		std::cout << "Sent: " << Sent << "\n";
		std::cout << iiGet << "\n";
		ByteArray Bytes = Slice(File::Array, i * 4095, (i * 4095) + iiGet);
		File::TCPClient->SendRawBytes(Bytes);
		std::cout << "Sent" << "\n";
		if (Bytes.size() <= 0)
			break;
		if (Size < Bytes.size()) // last packet will be under the byte size
			break;
		Sent += Bytes.size();
		Size -= Bytes.size();
		i++;
	}
	std::cout << "-Size: " << Size << "\n";
	std::cout << "-Sent: " << Sent << "\n";
}
