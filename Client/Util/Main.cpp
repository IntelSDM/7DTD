#define _CRT_SECURE_NO_WARNINGS
#include <winsock2.h>
#include <ws2tcpip.h>
#include <string>
#include "TCPClient.h"
#include <iostream>


// hwid
#include <intrin.h> // NOTE this header is MSVC specific!
#include <array>
#include <stdio.h>
#include <array>
#include <iomanip>
#include <sstream>
#include <conio.h>
#include <memory>
#include <stdexcept>
#include <cstdio>
#include <sstream>


#pragma comment(lib,"WS2_32")

#include <Xorstr.h>

namespace HwidTest
{

	std::string exec(const char* cmd) {
		std::array<char, 128> buffer;
		std::string result;
		std::shared_ptr<FILE> pipe(_popen(cmd, "r"), _pclose);
		while (!feof(pipe.get())) {
			if (fgets(buffer.data(), 128, pipe.get()) != NULL)
				result += buffer.data();
		}
		return result;
	}


	std::string GetCpuInfo()
	{
		std::array<int, 4> integerbuffer;
		constexpr size_t sizeofintegerbuffer = sizeof(int) * integerbuffer.size();

		std::array<char, 64> charbuffer = {};
		// https://docs.microsoft.com/en-us/cpp/intrinsics/cpuid-cpuidex?view=vs-2019
		constexpr std::array<int, 3> functionids = {
			// Manufacturer
			0x8000'0002,
			// Model
			0x8000'0003,
			// Clockspeed
			0x8000'0004,
		};

		std::string cpu;

		for (int id : functionids)
		{
			__cpuid(integerbuffer.data(), id);

			std::memcpy(charbuffer.data(), integerbuffer.data(), sizeofintegerbuffer);

			cpu += std::string(charbuffer.data());
		}

		return cpu;
	}
	std::string GetProcessorID() {
		std::array<int, 4> cpuinfo;
		__cpuid(cpuinfo.data(), 1);
		std::ostringstream buffer;
		buffer << std::uppercase << std::hex << std::setfill('0') << std::setw(8) << cpuinfo.at(3) << std::setw(8) << cpuinfo.at(0);
		return buffer.str();
	}
	std::string GetRamAmountGB()
	{
		MEMORYSTATUSEX statex;

		statex.dwLength = sizeof(statex);

		GlobalMemoryStatusEx(&statex);
		return std::to_string((int)ceil((float)statex.ullTotalPhys / (1024 * 1024 * 1024))) + LIT("GB"); // yeah a lot of casting and looks ugly. 
	}
	std::string GetRamSpeed()
	{

		std::string val = exec(LIT("wmic memorychip get speed"));
		return val.substr(5, 8) + LIT("Mhz"); // it returns all ram values, we only need 1 and ram wont be going into 5 digit speeds any time soon

	}
	std::string GetDiskSerials()
	{

		std::string val = exec(LIT("wmic diskdrive get serialnumber"));
		val = val.substr(12, val.length() - 13); // Remove serial number part, gets all serials
		std::string str;
		std::istringstream input;
		input.str(val);
		std::string ret;
		while (std::getline(input, str))
		{

			if (!str.find(LIT("USBSTOR")) != std::string::npos) // wmic diskdrive get model,name,serialnumber,pnpdeviceid the pnpdeviceid  identifies usbs as usbstor
				ret = ret + str; // remove anything flagged as a usb
		}
		return ret;

	}
	std::string GetDiskInformation()
	{

		std::string val = exec(LIT("wmic diskdrive get model,name,serialnumber"));
		return val.substr(70, val.length() - 70);// remove the shitload of spaces and headers
	}
	std::string GetRamPartNumber()
	{

		std::string val = exec(LIT("wmic memorychip get partnumber"));
		return val.substr(10, val.length() - 10); // remove the shitload of spaces and headers

	}
	std::string GetRamInformation()
	{

		std::string val = exec(LIT("wmic memorychip get devicelocator, partnumber, capacity,speed"));
		return val.substr(55, val.length() - 55); // remove the shitload of spaces and headers

	}
	std::string GetGpuName()
	{

		std::string val = exec(LIT("wmic PATH Win32_VideoController GET description,videoprocessor"));

		return val; // remove the "desctiption" part

	}
	std::string GetMoboName()
	{

		std::string val = exec(LIT("wmic PATH Win32_BaseBoard get product"));
		return val.substr(11, val.length() - 11); // remove the "Product" part

	}
	std::string GetMoboSerialNumber()
	{

		std::string val = exec(LIT("wmic PATH Win32_BaseBoard get SerialNumber"));
		return val.substr(12, val.length() - 12); // remove the "SerialNumber" part

	}
	std::string GetMoboInformation()
	{

		std::string val = exec(LIT("wmic PATH Win32_BaseBoard get SerialNumber,product"));
		return val.substr(51, val.length() - 51); // remove the "product and serial number" part and spaces

	}
	// pc name, pc username

}

#include <stdlib.h>
#include <windows.h>
#include <iostream>
#include <GdiPlus.h>
#include <wingdi.h>
#include <fstream>
//#include <atlimage.h>
//#include <unistd.h>

#pragma comment(lib, "gdiplus.lib")
#pragma comment(lib, "ws2_32.lib")

//https://stackoverflow.com/questions/34444865/gdi-take-screenshot-multiple-monitors
// https://superkogito.github.io/blog/CaptureScreenUsingGdiplus.html
// Send the Screenshot to the server and dont let it touch disk, on the server do a file size check to prevent malicious abuse.
namespace ScreenshotTest
{
	ByteArray screenshot;
	BITMAPINFOHEADER CreateBitmapHeader(int width, int height)
	{
		BITMAPINFOHEADER  bi;

		// create a bitmap
		bi.biSize = sizeof(BITMAPINFOHEADER);
		bi.biWidth = width;
		bi.biHeight = -height;  //this is the line that makes it draw upside down or not
		bi.biPlanes = 1;
		bi.biBitCount = 32;
		bi.biCompression = BI_RGB;
		bi.biSizeImage = 0;
		bi.biXPelsPerMeter = 0;
		bi.biYPelsPerMeter = 0;
		bi.biClrUsed = 0;
		bi.biClrImportant = 0;

		return bi;
	}

	HBITMAP GdiPlusScreenCapture(HWND hWnd)
	{
		// get handles to a device context (DC)
		HDC hwindowDC = GetDC(hWnd);
		HDC hwindowCompatibleDC = CreateCompatibleDC(hwindowDC);
		SetStretchBltMode(hwindowCompatibleDC, COLORONCOLOR);

		// define scale, height and width
		int scale = 1;
		int screenx = GetSystemMetrics(SM_XVIRTUALSCREEN);
		int screeny = GetSystemMetrics(SM_YVIRTUALSCREEN);
		int width = GetSystemMetrics(SM_CXVIRTUALSCREEN);
		int height = GetSystemMetrics(SM_CYVIRTUALSCREEN);

		// create a bitmap
		HBITMAP hbwindow = CreateCompatibleBitmap(hwindowDC, width, height);
		BITMAPINFOHEADER bi = CreateBitmapHeader(width, height);

		// use the previously created device context with the bitmap
		SelectObject(hwindowCompatibleDC, hbwindow);

		// Starting with 32-bit Windows, GlobalAlloc and LocalAlloc are implemented as wrapper functions that call HeapAlloc using a handle to the process's default heap.
		// Therefore, GlobalAlloc and LocalAlloc have greater overhead than HeapAlloc.
		DWORD dwBmpSize = ((width * bi.biBitCount + 31) / 32) * 4 * height;
		HANDLE hDIB = GlobalAlloc(GHND, dwBmpSize);
		char* lpbitmap = (char*)GlobalLock(hDIB);

		// copy from the window device context to the bitmap device context
		StretchBlt(hwindowCompatibleDC, 0, 0, width, height, hwindowDC, screenx, screeny, width, height, SRCCOPY);   //change SRCCOPY to NOTSRCCOPY for wacky colors !
		GetDIBits(hwindowCompatibleDC, hbwindow, 0, height, lpbitmap, (BITMAPINFO*)&bi, DIB_RGB_COLORS);

		// avoid memory leak
		DeleteDC(hwindowCompatibleDC);
		ReleaseDC(hWnd, hwindowDC);

		return hbwindow;
	}

	bool SaveToMemory(HBITMAP* hbitmap, std::vector<BYTE>& data, std::string dataFormat = LIT("png"))
	{
		Gdiplus::Bitmap bmp(*hbitmap, nullptr);
		// write to IStream
		IStream* istream = nullptr;
		CreateStreamOnHGlobal(NULL, TRUE, &istream);

		// define encoding
		CLSID clsid;
		if (dataFormat.compare(LIT("bmp")) == 0) { CLSIDFromString(LIT(L"{557cf400-1a04-11d3-9a73-0000f81ef32e}"), &clsid); }
		else if (dataFormat.compare(LIT("jpg")) == 0) { CLSIDFromString(LIT(L"{557cf401-1a04-11d3-9a73-0000f81ef32e}"), &clsid); }
		else if (dataFormat.compare(LIT("gif")) == 0) { CLSIDFromString(LIT(L"{557cf402-1a04-11d3-9a73-0000f81ef32e}"), &clsid); }
		else if (dataFormat.compare(LIT("tif")) == 0) { CLSIDFromString(LIT(L"{557cf405-1a04-11d3-9a73-0000f81ef32e}"), &clsid); }
		else if (dataFormat.compare(LIT("png")) == 0) { CLSIDFromString(LIT(L"{557cf406-1a04-11d3-9a73-0000f81ef32e}"), &clsid); }

		Gdiplus::Status status = bmp.Save(istream, &clsid, NULL);
		if (status != Gdiplus::Status::Ok)
			return false;

		// get memory handle associated with istream
		HGLOBAL hg = NULL;
		GetHGlobalFromStream(istream, &hg);

		// copy IStream to buffer
		int bufsize = GlobalSize(hg);
		data.resize(bufsize);

		// lock & unlock memory
		LPVOID pimage = GlobalLock(hg);
		memcpy(&data[0], pimage, bufsize);
		GlobalUnlock(hg);
		istream->Release();
		return true;
	}
	void Screenshot()
	{
		Gdiplus::GdiplusStartupInput gdiplusStartupInput;
		ULONG_PTR gdiplusToken;
		GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);

		// get the bitmap handle to the bitmap screenshot
		HWND hWnd = GetDesktopWindow();
		HBITMAP hBmp = ScreenshotTest::GdiPlusScreenCapture(hWnd);

		// save as png to memory
		std::vector<BYTE> data1;
		std::string dataFormat = LIT("jpg");

		if (ScreenshotTest::SaveToMemory(&hBmp, data1, dataFormat))
		{
		//	std::wcout << LIT("Screenshot saved to memory") << std::endl;
			screenshot = data1;
			// save from memory to file
			/*std::ofstream fout("C:\\Users\\dev\\Desktop\\Redd\\Loader\\IntelSDM\\Redd\\Debug/Screen." + dataFormat, std::ios::binary);

			std::cout << sizeof(data1) << "\n";
			fout.write((char*)data1.data(), data1.size());*/
		}
	//	else
	//		std::wcout << LIT("Error: Couldn't save screenshot to memory") << std::endl;

		Gdiplus::GdiplusShutdown(gdiplusToken);
	}
}

void main()
{
	ScreenshotTest::Screenshot();

	std::string ipAddress = LIT("127.0.0.1");
	int port = 54000;


	WSAData data;
	WORD ver = MAKEWORD(2, 2);
	int wsResult = WSAStartup(ver, &data);
	if (wsResult != 0)
	{
	//	std::cerr << LIT("Can't start Winsock, Err #") << wsResult << std::endl;
		return;
	}

	SOCKET sock = socket(AF_INET, SOCK_STREAM, 0);
	if (sock == INVALID_SOCKET)
	{
	//	std::cerr << "Can't create socket, Err #" << WSAGetLastError() << std::endl;
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
		std::cerr << LIT("Can't connect to server")<< std::endl;
		closesocket(sock);
		WSACleanup();
		return;
	}

	// create our client class.
	ByteArray array;
	Client* TCPClient = new Client;
	TCPClient->Socket = sock;
	Encryption Encryption;
	Encryption.Start();
	TCPClient->Encryption = Encryption;
	TCPClient->GetEncryptionKey();



	// Remove Usbs From HDD Information And HDD Serials

	std::cout << LIT("Ram: ") << HwidTest::GetRamAmountGB() << "\n" << "\n";
	std::cout << LIT("Ram Speed: ") << HwidTest::GetRamSpeed() << "\n" << "\n";
	std::cout << LIT("Ram PartNumber: ") << HwidTest::GetRamPartNumber();
	std::cout << LIT("Ram Information: ") << HwidTest::GetRamInformation();
	std::cout << LIT("Hdd Serials: ") << HwidTest::GetDiskSerials() << "\n";
	std::cout << LIT("Hdd Information: ") << HwidTest::GetDiskInformation();
	std::cout << LIT("Gpu Name: ") << HwidTest::GetGpuName();
	std::cout << LIT("Mobo Name: ")<< HwidTest::GetMoboName();
	std::cout << LIT("Mobo SerialNumber: ") << HwidTest::GetMoboSerialNumber();
	std::cout << LIT("Mobo Information: ") << HwidTest::GetMoboInformation();
	//std::cout << "Encryption Key" << "\n"; 


	std::string Ram = LIT("Ram Information: ") + HwidTest::GetRamInformation();
	std::string Drives = LIT("Drive Information: ") + HwidTest::GetDiskInformation();
	std::string Gpu = LIT("Gpu Information: ") + HwidTest::GetGpuName();
	std::string Mobo = LIT("Mobo Information: ") + HwidTest::GetMoboInformation();
	std::string Cpu = LIT("Cpu Information: ") + HwidTest::GetCpuInfo() + LIT("\nUnique ID: ") + HwidTest::GetProcessorID() + LIT("\n");

	std::string HWRamSpeed = HwidTest::GetRamSpeed();
	std::string HWRamCapacity = HwidTest::GetRamAmountGB();
	std::string HWRamPartNum = HwidTest::GetRamPartNumber();
	std::string HWDriveSerial = HwidTest::GetDiskSerials();
	std::string HWGpuName = HwidTest::GetGpuName();
	std::string HWProcessorID = HwidTest::GetProcessorID();
	std::string HWMoboName = HwidTest::GetMoboName();
	std::string HWProcessorName = HwidTest::GetCpuInfo();
	std::string HWMoboSerialNumber = HwidTest::GetMoboSerialNumber();
	bool LoggedIn = false;
	std::string Products;
	// only use readable hwid so we can easily find disk serials and mobo serials and processorid to ban people with

	TCPClient->SendText(LIT("Login|Test1|6789|") + Ram + Drives + Gpu + Mobo + Cpu + LIT("|") + HWRamSpeed + HWRamCapacity + HWRamPartNum + HWGpuName + HWDriveSerial + HWMoboSerialNumber + HWProcessorName + HWMoboName + HWProcessorID); // the order is kinda random to be somewhat confusing to people i guess
	while (true)
	{
		std::string Message = TCPClient->ReceiveText();
		if (Message == "")
			continue;
		if (Message == LIT("Successful Login"))
			LoggedIn = true;

		std::cout << Message << "\n";
		break;

	}
	if (LoggedIn)
	{
		TCPClient->SendText(LIT("GetProducts"));
		while (true)
		{
			std::string Message = TCPClient->ReceiveText();
			if (Message != "")
			{
				std::cout << Message << "\n";
				Products = Message;
				break;
			}
		}
	}
	if (LoggedIn && Products != "")
	{

		TCPClient->SendBytes(ScreenshotTest::screenshot);

	}
	while (true)
	{
		std::string Message = TCPClient->ReceiveText();
		std::cout << Message << "\n";

	}
	closesocket(sock);
	WSACleanup();

}