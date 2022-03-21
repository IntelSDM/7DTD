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
#include <Windows.h>
#include "Xorstr.h"
#include "Hwid.h"
#include "VMProtectSDK.h"


std::string exec(const char* cmd) {
	VMProtectBeginUltra("ExecuteSystem");
	std::array<char, 128> buffer;
	std::string result;
	std::shared_ptr<FILE> pipe(_popen(cmd, "r"), _pclose);
	while (!feof(pipe.get())) {
		if (fgets(buffer.data(), 128, pipe.get()) != NULL)
			result += buffer.data();
	}
	return result;
	VMProtectEnd();
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
	VMProtectBeginUltra("GetDiskSerials");
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
	VMProtectEnd();
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

std::string ReadableHwid()
{
	VMProtectBeginUltra("ReadableHwid");
	std::string Ram = LIT("Ram Information: ") + GetRamInformation();
	std::string Drives = LIT("Drive Information: ") + GetDiskInformation();
	std::string Gpu = LIT("Gpu Information: ") + GetGpuName();
	std::string Mobo = LIT("Mobo Information: ") + GetMoboInformation();
	std::string Cpu = LIT("Cpu Information: ") + GetCpuInfo() + LIT("\nUnique ID: ") + GetProcessorID() + LIT("\n");
	return Ram + Drives + Gpu + Mobo + Cpu;
	VMProtectEnd();
}
std::string Hwid()
{
	VMProtectBeginUltra("Hwid");
	std::string HWRamSpeed = GetRamSpeed();
	std::string HWRamCapacity = GetRamAmountGB();
	std::string HWRamPartNum = GetRamPartNumber();
	std::string HWDriveSerial = GetDiskSerials();
	std::string HWGpuName = GetGpuName();
	std::string HWProcessorID = GetProcessorID();
	std::string HWMoboName = GetMoboName();
	std::string HWProcessorName = GetCpuInfo();
	std::string HWMoboSerialNumber = GetMoboSerialNumber();
	return HWRamSpeed + HWRamCapacity + HWRamPartNum + HWGpuName + HWDriveSerial + HWMoboSerialNumber + HWProcessorName + HWMoboName + HWProcessorID;
	VMProtectEnd();
}