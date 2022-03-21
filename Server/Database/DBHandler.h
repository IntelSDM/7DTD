#pragma once
#include <string>
#include "Encryption.h"
#include <list>
#include <Tuple>
class Database
{
public:

	std::string CreateUser(std::string Username, std::string Password, std::string Hwid, std::string ReadableHwid, std::string IpAddress);
	std::string LoginUser(std::string Username, std::string Password, std::string Hwid, std::string ReadableHwid, std::string IpAddress);
	std::string RedeemProduct(std::string Username, std::string Key);
	std::string GenerateKey(std::string Product, std::string Vendor, int Time);
	std::string GetActiveProducts(std::string Username);
	ByteArray GetStreamFile(std::string Username, std::string Product, std::string File);
	ByteArray GetStreamFile(std::string File);
	void FreezeProduct(std::string Product);
	void UnFreezeProduct(std::string Product);
	void CreateProduct(std::string Product);
	void CreateDB();
	void StoreScreenshot(ByteArray Data, std::string Username);
private:
	std::string DBDir = "C:\\Users\\Administrator\\Documents\\CatHack";
	std::string GlobalDir = DBDir + "/Globals";

	void WriteLog(std::string Dir, std::string FileName, std::list<std::string> Content);
	int AmountOfLogs(std::string Dir, std::string FileName);
	std::list<std::string> ReadLog(std::string Dir, std::string FileName);
	std::string GetLog(std::string Dir, std::string FileName, int Index);
	std::string GetLogOfContent(std::string Dir, std::string FileName, std::string Content);

	void WriteLine(int LineNum, std::string Dir, std::string FileName, std::string Contents);
	std::string ReadLine(int LineNum, std::string Dir, std::string FileName);
	void WriteEncrypted(int LineNum, std::string Dir, std::string FileName, std::string Contents);
	std::string ReadEncrypted(int LineNum, std::string Dir, std::string FileName);

	std::string ReadFileAsString(std::string Dir, std::string FileName);
	std::string ReadEncryptedFileAsString(std::string Dir, std::string FileName);
	void WriteFileAsString(std::string Dir, std::string FileName, std::string Content);
	void WriteEncryptedFileAsString(std::string Dir, std::string FileName, std::string Content);

	void AddFileLines(int NumOfLines, std::string Dir, std::string FileName);
	//void AddFileLines(int StartLine, int NumOfLines, std::string Dir, std::string FileName);
	int GetLogLines(std::string Dir, std::string FileName);

	void CreateDir(std::string Dir);
	void CreateFilename(std::string Dir, std::string Filename);

	bool DoesDirectoryExist(std::string Dir);
	bool DoesFileExist(std::string Dir, std::string FileName);

	void RemoveLines(std::string& input);
	void RemoveSpaces(std::string& input);
};