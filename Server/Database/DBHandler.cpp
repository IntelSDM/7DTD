
#include "DBHandler.h"
#include <iostream>
#include <fstream> 
#include <filesystem>
#include <ctime>
#include <sstream>


#pragma warning(disable : 4996) // unsafe warning



void Database::CreateDB()
{

	Database::CreateDir(DBDir + "/Database");
	Database::CreateDir(DBDir + "/Globals");
	Database::CreateDir(DBDir + "/Globals/Products");
	Database::CreateFilename(GlobalDir, "Products.txt");
	Database::CreateFilename(DBDir + "/Globals", "BannedHwid.txt");
	Database::CreateFilename(DBDir + "/Globals", "Frozen.txt");
}
void Database::CreateProduct(std::string Product)
{
	std::string Dir = GlobalDir + "/" + Product;
	if (!Database::DoesDirectoryExist(Dir))
		Database::CreateDir(Dir);


}
ByteArray Database::GetStreamFile(std::string Username, std::string Product, std::string File)
{
	std::string ret = "Error";
	std::string CachedUsername = Username;
	std::for_each(CachedUsername.begin(), CachedUsername.end(), [](char& c)
		{
			c = ::tolower(c); // To lower the username so we dont need anything case sensitive

		});
	Username = CachedUsername;
	std::string UserDir = DBDir + "/Database/" + Username;
	// check for active product, return file bytearray
	if (!Database::DoesFileExist(GlobalDir + "/products", File))
	{
		ret = "File Doesn't Exist";
		ByteArray empty(ret.begin(), ret.end());
		return empty;
	}

	std::string Products = Database::GetActiveProducts(Username);
	std::istringstream input;
	std::string str;
	input.str(Products);
	std::string character = "-";
	bool ProductExist = false;
	while (std::getline(input, str))
	{
		int specialchar = 0;
		int specialcharpos[1050];
		for (std::string::size_type i = 0; i < str.size(); i++)
		{
			if (str[i] == character[0])
			{
				specialcharpos[specialchar] = i;
				specialchar++;
			}
		}
		std::string ProductName = str.substr(0, specialcharpos[0]);
		if (ProductName == Product)
		{
			ProductExist = true;
		}
	}
	if (!ProductExist)
	{
		ret = "No Product";
		ByteArray empty(ret.begin(), ret.end());
		return empty;
	}


	ByteArray Contents;
	std::ifstream Files(GlobalDir + "/products/" + File, std::ios::in | std::ios::binary);
	ByteArray empty(ret.begin(), ret.end());
	if (!Files.is_open())
		return empty;

	// Do not skip white-space, read file.
	Files.unsetf(std::ios::skipws);
	Contents.insert(
		Contents.begin(),
		std::istream_iterator<uint8_t>(Files),
		std::istream_iterator<uint8_t>()
	);

	if (Contents.empty())
		return empty;
	Files.close();

	return Contents;

}
ByteArray Database::GetStreamFile(std::string File)
{
	std::string ret = "Error";

	// check for active product, return file bytearray
	if (!Database::DoesFileExist(GlobalDir + "/products", File))
	{
		ret = "File Doesn't Exist";
		ByteArray empty(ret.begin(), ret.end());
		return empty;
	}

	ByteArray Contents;
	std::ifstream Files(GlobalDir + "/products/" + File, std::ios::in | std::ios::binary);
	ByteArray empty(ret.begin(), ret.end());
	if (!Files.is_open())
		return empty;

	// Do not skip white-space, read file.
	Files.unsetf(std::ios::skipws);
	Contents.insert(
		Contents.begin(),
		std::istream_iterator<uint8_t>(Files),
		std::istream_iterator<uint8_t>()
	);

	if (Contents.empty())
		return empty;
	Files.close();

	return Contents;

}
void Database::StoreScreenshot(ByteArray Data, std::string Username)
{
	std::time_t t = std::time(0);
	std::tm* now = std::gmtime(&t);
	int year = now->tm_year + 1900;
	int month = now->tm_mon + 1;
	int day = now->tm_mday;
	int hour = now->tm_hour;
	int minute = now->tm_min;
	int second = now->tm_sec;
	std::string TimeNow = std::to_string(second) + "s-" + std::to_string(minute) + "m-" + std::to_string(hour) + "h-" + std::to_string(day) + "d-" + std::to_string(month) + "mo-" + std::to_string(year) + "y" + ".jpg";
	std::string UserDir = DBDir + "/Database/" + Username;
	std::string ScreenshotDir = DBDir + "/Database/" + Username + "/Screenshots/";



	if (Database::DoesFileExist(ScreenshotDir, TimeNow))
		return;
	// save from memory to file
	std::ofstream fout(ScreenshotDir + TimeNow, std::ios::binary);
	fout.write((char*)Data.data(), Data.size());

}

void Database::FreezeProduct(std::string Product)
{ // https://stackoverflow.com/questions/67273/how-do-you-iterate-through-every-file-directory-recursively-in-standard-c
	std::string text = Database::ReadFileAsString(GlobalDir, "Frozen.txt");
	std::istringstream input;
	std::string str;
	input.str(text);
	std::string ret;

	std::string character = "-";

	while (std::getline(input, str))
	{
		int specialchar = 0;
		int specialcharpos[1050];
		for (std::string::size_type i = 0; i < str.size(); i++)
		{
			if (str[i] == character[0])
			{
				specialcharpos[specialchar] = i;
				specialchar++;
			}
		}
		std::string ProductName = str.substr(0, specialcharpos[0]);
		int Time = stoi(str.substr(specialcharpos[0] + 1, str.length() - specialcharpos[0] + 1));
	//	std::cout << Time << "\n";
	//	std::cout << ProductName << "\n";
		if (str != "" && ProductName != Product)
			ret = ret + str + "\n";

	}

	ret = ret + Product + "-" + std::to_string(time(NULL));

	Database::WriteFileAsString(GlobalDir, "Frozen.txt", ret);

}
// add subscription freezing
std::string Database::GenerateKey(std::string Product, std::string Vendor, int Time)
{
	srand((unsigned)time(NULL) * __rdtsc()); // always random using the clock count	to calculate the random seed
	static const char alphanum[] =
		"0123456789"
		"ABCDEFGHIJKLMNOPQRSTUVWXYZ"
		"abcdefghijklmnopqrstuvwxyz";
	std::string tmp;
	tmp.reserve(24);


	std::string Part1 = Product + "-";
	std::string Part2 = Vendor + "-";
	std::string Part3 = std::to_string(Time) + "-";

	for (int i = 0; i < 24; ++i) {
		tmp += alphanum[rand() % (sizeof(alphanum) - 1)];
	}
	std::string Part4 = tmp;
	std::string Key = Part1 + Part2 + Part3 + Part4;

	std::string db = Database::ReadFileAsString(GlobalDir, "Products.txt");
	if (db.length() > 0)
		db = db + "\n" + Key;
	else
		db = Key;
	Database::WriteFileAsString(GlobalDir, "Products.txt", db);
	return Key;
}
void Database::UnFreezeProduct(std::string Product)
{
	if (!Database::DoesFileExist(Database::GlobalDir, "Products.txt"))
		return;


	std::filesystem::path DefaultPath = Database::DBDir + "/Database/ ";
	for (std::filesystem::path dirEntry : std::filesystem::recursive_directory_iterator(Database::DBDir + "/Database")) // take the username and just use the username with dbdir/database
	{
		std::string UserDir = dirEntry.u8string();

		if (dirEntry.has_extension() == true)
			continue;
		if (dirEntry.relative_path().remove_filename() != DefaultPath.relative_path().remove_filename())
			continue;


		// loop through directory characters and find the \ to then find the username and create a real full directory
		int charpos = 0;
		std::string character1 = "\\";
		for (std::string::size_type i = 0; i < UserDir.size(); i++)
		{
			if (UserDir[i] == character1[0])
			{
				charpos = i;
			}
		}
		UserDir = Database::DBDir + "/Database/" + UserDir.substr(charpos + 1, UserDir.length() - charpos + 1);

		if (!Database::DoesFileExist(UserDir, "Products.txt"))
			continue;

		//	std::cout << time(NULL) << std::endl; 

			// loop through the frozen products and player products
		std::string LocalKeys = Database::ReadFileAsString(UserDir, "Products.txt");
		RemoveSpaces(LocalKeys);
		std::istringstream input;
		std::string str;
		input.str(LocalKeys);
		std::string ret;
		std::string character = "-";

		std::string text = Database::ReadFileAsString(GlobalDir, "Frozen.txt");
		std::istringstream input1;
		std::string str1;
		input1.str(text);

		std::list<std::string> FrozenSubs;
		std::string Write = "";
		std::string WriteFrozen = "";

		int it1 = 0;
		int it2 = 0;
		while (std::getline(input1, str1))
		{
			FrozenSubs.push_back(str1);
		}
		while (std::getline(input, str))
		{
			int specialchar = 0;
			int specialcharpos[1050];
			for (std::string::size_type i = 0; i < str.size(); i++)
			{
				if (str[i] == character[0])
				{
					specialcharpos[specialchar] = i;
					specialchar++;
				}
			}
			std::string ProductName = str.substr(0, specialcharpos[0]);
			std::string SellerName = str.substr(specialcharpos[0] + 1, specialcharpos[0] - 1);
			int ProductTime = stoi(str.substr(specialcharpos[1] + 1, specialcharpos[2]));
			int ProductTimeDays;
			bool IsFrozen = false;
			int FrozenTime = 0;

			for (std::string string : FrozenSubs)
			{
				// check if the string name is equal to our product if so do this shit and remove the line to make a new string to write to the file and remove the frozen shit	
				int specialchar1 = 0;
				int specialcharpos1[1050];
				for (std::string::size_type i = 0; i < string.size(); i++)
				{
					if (string[i] == character[0])
					{
						specialcharpos1[specialchar1] = i;
						specialchar1++;
					}
				}
				std::string FrozenProductName = string.substr(0, specialcharpos1[0]);

				if (FrozenProductName != Product)
				{

					if (WriteFrozen.find(string) == std::string::npos)
					{
						if (it2 == 0)
							WriteFrozen = WriteFrozen + string;
						else
							WriteFrozen = WriteFrozen + "\n" + string;
						it2++;
					}
					continue;
				}
				if (FrozenProductName == ProductName)
					IsFrozen = true;

				if (IsFrozen)
				{
					std::string Cached = string.substr(specialcharpos1[0] + 1, string.length() - specialcharpos1[0] + 1);
					int FrozenTime = atoi(Cached.c_str()); // stoi not working, its returning 0 when it has a clear int value
					if (ProductTime < FrozenTime)
						continue; // check if the subscription isn't from before it was frozen

					int difference = time(NULL) - FrozenTime;
					ProductTime = ProductTime + difference;
				}


			}

			if (it1 == 0)
				Write = Write + ProductName + "-" + SellerName + "-" + std::to_string(ProductTime);
			else
				Write = Write + "\n" + ProductName + "-" + SellerName + "-" + std::to_string(ProductTime);
			it1 = it1 + 1;

			if (IsFrozen)
				continue;
		}


		Database::WriteFileAsString(UserDir, "Products.txt", Write);
		Database::WriteFileAsString(GlobalDir, "Frozen.txt", WriteFrozen);





	}

	//std::cout << dirEntry << std::endl;


}
std::string Database::GetActiveProducts(std::string Username) // in the future make it return the sellerid so then we can do custom builds for sellers
{
	std::string CachedUsername = Username;
	std::for_each(CachedUsername.begin(), CachedUsername.end(), [](char& c)
		{
			c = ::tolower(c);

		});
	Username = CachedUsername;
	std::string UserDir = DBDir + "/Database/" + Username;
	if (!Database::DoesFileExist(Database::GlobalDir, "Products.txt"))
		return "Product DB Error";
	if (!Database::DoesFileExist(UserDir, "Products.txt"))
		return "Local DB Error";

	std::string LocalKeys = Database::ReadFileAsString(UserDir, "Products.txt");
	std::istringstream input;
	std::string str;
	input.str(LocalKeys);
	std::string ret;
	std::string character = "-";

	std::string text = Database::ReadFileAsString(GlobalDir, "Frozen.txt");
	std::istringstream input1;
	std::string str1;
	input1.str(text);

	std::list<std::string> FrozenSubs;
	try
	{
		while (std::getline(input1, str1))
		{
			FrozenSubs.push_back(str1);
		}
		while (std::getline(input, str))
		{
			int specialchar = 0;
			int specialcharpos[1050];
			for (std::string::size_type i = 0; i < str.size(); i++)
			{
				if (str[i] == character[0])
				{
					specialcharpos[specialchar] = i;
					specialchar++;
				}
			}
			std::string ProductName = str.substr(0, specialcharpos[0]);
			int ProductTime = stoi(str.substr(specialcharpos[1] + 1, specialcharpos[2]));
			std::string SellerName = str.substr(specialcharpos[0] + 1, specialcharpos[0] - 1);
			int ProductTimeDays;
			bool IsFrozen = false;
			int FrozenTime = 0;

			for (std::string string : FrozenSubs)
			{
				int specialchar1 = 0;
				int specialcharpos1[1050];
				for (std::string::size_type i = 0; i < string.size(); i++)
				{
					if (string[i] == character[0])
					{
						specialcharpos1[specialchar1] = i;
						specialchar1++;
					}
				}
				std::string FrozenProductName = string.substr(0, specialcharpos1[0]);
				if (FrozenProductName == ProductName)
					IsFrozen = true;

				if (IsFrozen)
				{
					std::string Cached = string.substr(specialcharpos1[0] + 1, string.length() - specialcharpos1[0] + 1);
					int FrozenTime = atoi(Cached.c_str()); // stoi not working, its returning 0 when it has a clear int value
					if (ProductTime < FrozenTime)
						continue; // check if the subscription isn't from before it was frozen


					ret = ret + ProductName + "-" + SellerName + "-" + "Frozen" + "\n";


				}

			}
			if (IsFrozen)
				continue; // stop frozen subscriptions showing the time left if it is still valid

			if (ProductTime > time(NULL)) // return the time left of the sub in days
			{
				ProductTimeDays = ProductTime - time(NULL);
				ProductTimeDays = ProductTimeDays / 86400;
				ret = ret + ProductName + "-" + SellerName + "-" + std::to_string(ProductTimeDays) + "\n";
				continue;
			}

		}
	}
	catch (std::exception)
	{
		return "No Active Products";
	}


	if (ret.length() > 0)
		return ret;
	else
		return "No Active Products";

}
std::string Database::RedeemProduct(std::string Username, std::string Key)
{
	std::string CachedUsername = Username;

	std::for_each(CachedUsername.begin(), CachedUsername.end(), [](char& c)
		{
			c = ::tolower(c);

		});
	Username = CachedUsername;
	try
	{

		//std::cout << "cc" << "\n";
		std::string LogDir = DBDir + "/Database/" + Username + "/Logs";
		std::string UserDir = DBDir + "/Database/" + Username;
		if (!Database::DoesFileExist(Database::GlobalDir, "Products.txt"))
			return "Product DB Error";
		if (!Database::DoesFileExist(UserDir, "Products.txt"))
			return "Local DB Error";
		std::string Keys = Database::ReadFileAsString(Database::GlobalDir, "Products.txt");
		if (!(Keys.find(Key) != std::string::npos))
			return "Invalid Key";

		std::istringstream input;
		std::string str;
		input.str(Keys);
		std::string ret;
		while (std::getline(input, str))
		{
			RemoveSpaces(str);

			if (!(str.find(Key) != std::string::npos) && str != "\n" && str.length() > 0 && str != "") // remove the key
				ret = ret + str + "\n";
		}
		//std::cout << "vv" << "\n";
		int specialchar = 0;
		int specialcharpos[1050];
		std::string character = "-";
		for (std::string::size_type i = 0; i < Key.size(); i++)
		{
			if (Key[i] == character[0])
			{
				specialcharpos[specialchar] = i;
				specialchar++;
			}
		}
		if (specialchar != 3)
			return "Invalid Key";

		//std::cout << "ff" << "\n";
		Database::WriteFileAsString(Database::GlobalDir, "Products.txt", ret);
		std::string LocalKeys = Database::ReadFileAsString(UserDir, "Products.txt");
		std::istringstream input2;
		input2.str(LocalKeys);
		std::string ret2;
		int TimeToAdd = 0;
		std::string KeySeller = Key.substr(specialcharpos[0] + 1, specialcharpos[0] - 1);
		while (std::getline(input2, str))
		{
			RemoveSpaces(str);
			//std::cout << "ww" << "\n";
			int specialchar1 = 0;
			int specialcharpos1[1050];
			for (std::string::size_type i = 0; i < str.size(); i++)
			{
				if (str[i] == character[0])
				{
					specialcharpos1[specialchar1] = i;
					specialchar1++;
				}
			}
			std::string ProductName = str.substr(0, specialcharpos1[0]);
			std::string ProductSeller = str.substr(specialcharpos1[0] + 1, specialcharpos1[0] - 1);
			std::cout << str.substr(specialcharpos1[1] + 1, specialcharpos1[2]) << "\n";
			int ProductTime = stoi(str.substr(specialcharpos1[1] + 1, specialcharpos1[2]));
			//std::cout << ProductSeller << "\n";
			//std::cout << KeySeller << "\n";
			if (ProductName == Key.substr(0, specialcharpos[0]) && ProductSeller == KeySeller) // check if product name is the same and product seller
			{
				if (ProductTime > time(0))
					TimeToAdd = TimeToAdd + (ProductTime - time(0)); // add old subscription time and remove dead keys
			}
			else
			{
				ret2 = ret2 + str + "\n";
			}

		}

		RemoveSpaces(ret2);
		// normalize key:
		Key = Key.substr(0, specialcharpos[2]); // remove random string
		std::cout << Key << "\n";
		int StoredTime = stoi(Key.substr(specialcharpos[1] + 1, specialcharpos[2])); // convert time of sub in seconds into an int 
		std::cout << StoredTime << "\n";
		int StartTime = time(NULL); // time in seconds till now
		Key = Key.substr(0, specialcharpos[1] + 1);
		Key = Key + std::to_string(StartTime + StoredTime + TimeToAdd); // add sub time and time to get end of sub date
		ret2 = ret2 + Key;
		Database::WriteFileAsString(UserDir, "Products.txt", ret2);

		std::time_t t = std::time(0);
		std::tm* now = std::gmtime(&t);
		int year = now->tm_year + 1900;
		int month = now->tm_mon + 1;
		int day = now->tm_mday;
		int hour = now->tm_hour;
		int minute = now->tm_min;
		int second = now->tm_sec;
		std::string TimeNow = std::to_string(minute) + "m-" + std::to_string(hour) + "h-" + std::to_string(day) + "d-" + std::to_string(month) + "mo-" + std::to_string(year) + "y" + ".txt";

		std::list<std::string> Log;
		Log.push_back("Activated Key At: " + std::to_string(second) + "s " + std::to_string(minute) + "m " + std::to_string(hour) + "h " + std::to_string(day) + "d " + std::to_string(month) + "mo " + std::to_string(year) + "y");
		Database::WriteLog(LogDir, TimeNow, Log);

		return "Subscription Added";
	}
	catch (std::exception)
	{
		return "Invalid Key";
	}
}
std::string Database::LoginUser(std::string Username, std::string Password, std::string Hwid, std::string ReadableHwid, std::string IpAddress)
{

	std::string CachedUsername = Username;
	std::for_each(CachedUsername.begin(), CachedUsername.end(), [](char& c)
		{
			c = ::tolower(c); // To lower the username so we dont need anything case sensitive

		});
	Username = CachedUsername;
	std::string UserDir = DBDir + "/Database/" + Username;
	std::string LogDir = DBDir + "/Database/" + Username + "/Logs";
	RemoveSpaces(Hwid);
	RemoveLines(Hwid);
	std::time_t t = std::time(0);
	std::tm* now = std::gmtime(&t);
	int year = now->tm_year + 1900;
	int month = now->tm_mon + 1;
	int day = now->tm_mday;
	int hour = now->tm_hour;
	int minute = now->tm_min;
	int second = now->tm_sec;
	std::string TimeNow = std::to_string(minute) + "m-" + std::to_string(hour) + "h-" + std::to_string(day) + "d-" + std::to_string(month) + "mo-" + std::to_string(year) + "y" + ".txt";


	if (!Database::DoesDirectoryExist(UserDir))
		return "Invalid Username";
	if (!(Database::ReadFileAsString(UserDir, "Password.txt") == Password))
	{
		std::list<std::string> IPLog;
		IPLog.push_back("(Invalid Password)User IP At: " + std::to_string(second) + "s " + std::to_string(minute) + "m " + std::to_string(hour) + "h " + std::to_string(day) + "d " + std::to_string(month) + "mo " + std::to_string(year) + "y" + "\n" + IpAddress);
		Database::WriteLog(LogDir, "IP.txt", IPLog);

		return "Invalid Password";
	}
	if (!(Database::ReadFileAsString(UserDir, "Hwid.txt") == Hwid))
	{
		// Note the hwid so we can reset to it so people cant get it reset to nothing and sell the account
		Database::WriteFileAsString(UserDir, "PendingHwid.txt", Hwid);
		Database::WriteFileAsString(UserDir, "PendingReadableHwid.txt", ReadableHwid);


		std::list<std::string> IPLog;
		IPLog.push_back("(Invalid Hwid) User IP At: " + std::to_string(second) + "s " + std::to_string(minute) + "m " + std::to_string(hour) + "h " + std::to_string(day) + "d " + std::to_string(month) + "mo " + std::to_string(year) + "y" + "\n" + IpAddress);
		Database::WriteLog(LogDir, "IP.txt", IPLog);

		std::list<std::string> HwidLog;
		HwidLog.push_back("Invalid Hwid At: " + std::to_string(second) + "s " + std::to_string(minute) + "m " + std::to_string(hour) + "h " + std::to_string(day) + "d " + std::to_string(month) + "mo " + std::to_string(year) + "y" + "\n" + ReadableHwid);
		Database::WriteLog(LogDir, "Hwid.txt", HwidLog);
		return "Invalid Hwid";
	}
	if (Database::ReadFileAsString(UserDir, "Banned.txt").find("True") != std::string::npos)
	{
		return Database::ReadFileAsString(UserDir, "BanReason.txt");
	}
	if (Database::ReadFileAsString(UserDir, "Banned.txt").find("Hwid") != std::string::npos)
	{
		// check if this hwid they are logging in as is logged, ban it if it isn't
		// loop through lines in the list, find the unique id, mobo serial and disk serials
		if (!Database::ReadFileAsString(Database::GlobalDir, "BannedHwid.txt").find(Hwid) != std::string::npos)
		{
			std::list<std::string> HwidBan;
			HwidBan.push_back(Hwid);
			Database::WriteLog(Database::GlobalDir, "BannedHwid.txt", HwidBan);
		}


		return Database::ReadFileAsString(UserDir, "BanReason.txt");
	}

	if (Database::ReadFileAsString(Database::GlobalDir, "BannedHwid.txt").find(Hwid) != std::string::npos)
	{
		Database::WriteFileAsString(UserDir, "BannedHwid.txt", "Hwid");
		Database::WriteFileAsString(UserDir, "BanReason.txt", "Banned: Ban Evasion");
		return "Banned: Ban Evasion";
	}



	std::list<std::string> IPLog;
	IPLog.push_back("(Valid Login) User IP At: " + std::to_string(second) + "s " + std::to_string(minute) + "m " + std::to_string(hour) + "h " + std::to_string(day) + "d " + std::to_string(month) + "mo " + std::to_string(year) + "y" + "\n" + IpAddress);
	Database::WriteLog(LogDir, "IP.txt", IPLog);

	std::list<std::string> Log;
	Log.push_back("Successful Login At: " + std::to_string(second) + "s " + std::to_string(minute) + "m " + std::to_string(hour) + "h " + std::to_string(day) + "d " + std::to_string(month) + "mo " + std::to_string(year) + "y");
	Database::WriteLog(LogDir, TimeNow, Log);
	return "Successful Login";

}
std::string Database::CreateUser(std::string Username, std::string Password, std::string Hwid, std::string ReadableHwid, std::string IpAddress)
{
	std::string CachedUsername = Username;

	std::for_each(CachedUsername.begin(), CachedUsername.end(), [](char& c)
		{
			c = ::tolower(c); // To lower the username so we dont need anything case sensitive

		});
	Username = CachedUsername;
	if (!Database::DoesDirectoryExist(DBDir + "/Database"))
	{
		return "Server Database Error, Please Report";
	}
	// check if user exists
	if (Database::DoesDirectoryExist(DBDir + "/Database/" + Username))
	{
		return "User Already Exists";

	}
	if (Username.length() > 64)
		return "Username Too Long";
	if (Password.length() > 64)
		return "Password Too Long";
	if (Username.length() < 3)
		return "Username Too Short";
	if (Password.length() < 8)
		return "Password Too Short";
	if (Username.find("\\n") != std::string::npos || Username.find("\\") != std::string::npos || Username.find("\\t") != std::string::npos)
		return "Invalid Username";

	if (Password.find("\\n") != std::string::npos || Password.find("\\") != std::string::npos || Password.find("\\t") != std::string::npos)
		return "Invalid Password";

	Database::CreateDir(DBDir + "/Database/" + Username);
	Database::CreateDir(DBDir + "/Database/" + Username + "/Logs");
	Database::CreateDir(DBDir + "/Database/" + Username + "/Screenshots");
	Database::CreateFilename(DBDir + "/Database/" + Username, "Username.txt");
	Database::CreateFilename(DBDir + "/Database/" + Username, "Password.txt");
	Database::CreateFilename(DBDir + "/Database/" + Username, "Hwid.txt");
	Database::CreateFilename(DBDir + "/Database/" + Username, "ReadableHwid.txt");
	Database::CreateFilename(DBDir + "/Database/" + Username, "PendingHwid.txt");
	Database::CreateFilename(DBDir + "/Database/" + Username, "PendingReadableHwid.txt");
	Database::CreateFilename(DBDir + "/Database/" + Username, "Banned.txt");
	Database::CreateFilename(DBDir + "/Database/" + Username, "BanReason.txt");
	Database::CreateFilename(DBDir + "/Database/" + Username, "Products.txt");
	RemoveSpaces(Hwid);
	RemoveLines(Hwid);
	Database::WriteFileAsString(DBDir + "/Database/" + Username, "Username.txt", Username);
	Database::WriteFileAsString(DBDir + "/Database/" + Username, "Password.txt", Password);
	Database::WriteFileAsString(DBDir + "/Database/" + Username, "ReadableHwid.txt", ReadableHwid);
	Database::WriteFileAsString(DBDir + "/Database/" + Username, "Hwid.txt", Hwid);
	std::string LogDir = DBDir + "/Database/" + Username + "/Logs";


	std::time_t t = std::time(0);
	std::tm* now = std::gmtime(&t);


	int year = now->tm_year + 1900;
	int month = now->tm_mon + 1;
	int day = now->tm_mday;
	int hour = now->tm_hour;
	int minute = now->tm_min;
	int second = now->tm_sec;
	std::string TimeNow = std::to_string(minute) + "m-" + std::to_string(hour) + "h-" + std::to_string(day) + "d-" + std::to_string(month) + "mo-" + std::to_string(year) + "y" + ".txt";


	std::list<std::string> Log;
	Log.push_back("User Creared At: " + std::to_string(second) + "s " + std::to_string(minute) + "m " + std::to_string(hour) + "h " + std::to_string(day) + "d " + std::to_string(month) + "mo " + std::to_string(year) + "y");
	Database::WriteLog(LogDir, TimeNow, Log);

	std::list<std::string> HwidLog;
	HwidLog.push_back("User Hwid At: " + std::to_string(second) + "s " + std::to_string(minute) + "m " + std::to_string(hour) + "h " + std::to_string(day) + "d " + std::to_string(month) + "mo " + std::to_string(year) + "y" + "\n" + ReadableHwid);
	Database::WriteLog(LogDir, "Hwid.txt", HwidLog);

	std::list<std::string> IPLog;
	IPLog.push_back("(Account Creation) User IP At: " + std::to_string(second) + "s " + std::to_string(minute) + "m " + std::to_string(hour) + "h " + std::to_string(day) + "d " + std::to_string(month) + "mo " + std::to_string(year) + "y" + "\n" + IpAddress);
	Database::WriteLog(LogDir, "IP.txt", IPLog);

	std::cout << Username << " User Created \n";

	return "User Created";

}

#pragma region Log
void Database::WriteLog(std::string Dir, std::string FileName, std::list<std::string> Content)
{
	// write to the file if it already exists, else make a new file and write to it
	if (Database::DoesFileExist(Dir, FileName))
	{
		std::string contents = "---End Of Log---\n---Start Of Log---\n";
		int i = 0;
		for (std::string line : Content)
		{
			i++;
			contents = contents + line + " " + "\n";

		}
		contents = contents + "---End Of Log---" + "\n" + "\n";
		Database::WriteLine(Database::GetLogLines(Dir, FileName), Dir, FileName, contents);
	}
	else
	{
		Database::CreateFilename(Dir, FileName);
		Database::AddFileLines(1000, Dir, FileName);
		std::string contents = "---Start Of Log--- \n";
		int i = 0;
		for (std::string line : Content)
		{
			i++;
			contents = contents + line + " " + "\n";
			if (i == Content.size())
				contents = contents + "---End Of Log---" + "\n";
		}
		Database::WriteFileAsString(Dir, FileName, contents);


	}
}
std::list<std::string> Database::ReadLog(std::string Dir, std::string FileName)
{
	std::list<std::string> Result;
	if (!Database::DoesFileExist(Dir, FileName))
	{
		Result.push_back("Invalid Dir");
		return Result;
	}

	int test = 0;

	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;
	std::ifstream file(result);

	std::string str;
	std::list<std::string> lines;
	while (std::getline(file, str))
	{


	}

	return Result;
}
int Database::GetLogLines(std::string Dir, std::string FileName)
{


	std::list<std::string> linelist;
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;

	std::ifstream file(result);
	std::string str;
	int line = linelist.size();
	int i = 0;
	while (std::getline(file, str))
	{
		if (str == "---End Of Log---")
			line = i;
		i++;

	}


	return line;
}
int Database::AmountOfLogs(std::string Dir, std::string FileName)
{
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;

	std::ifstream file(result);
	std::string str;
	int i = 0;
	while (std::getline(file, str))
	{
		if (str == "---End Of Log---")
			i++;

	}
	return i;
}
std::string Database::GetLogOfContent(std::string Dir, std::string FileName, std::string Content)
{
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;

	std::string Result = "Null";

	for (int i = 0; i < Database::AmountOfLogs(Dir, FileName); i++)
	{
		std::string log = GetLog(Dir, FileName, i);
		if (log.find(Content) != std::string::npos)
			Result = log;

	}
	return Result;

}
std::string Database::GetLog(std::string Dir, std::string FileName, int Index)
{
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;
	int amount = Database::AmountOfLogs(Dir, FileName);
	if (Index > amount)
		return "";

	std::ifstream file(result);
	std::string str;
	int i = 0;
	std::string Result = "";

	while (std::getline(file, str))
	{
		if (str == "---Start Of Log---")
			i++;
		if (i == amount)
		{
			if (str != "---End Of Log---")
				Result = Result + str;
		}
	}

}
#pragma endregion Log

#pragma region FileAsString
void Database::WriteFileAsString(std::string Dir, std::string FileName, std::string Content)
{
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;

	std::ofstream writefile(result);

	writefile.clear();
	writefile << Content;
	writefile.close();
}
void Database::WriteEncryptedFileAsString(std::string Dir, std::string FileName, std::string Content)
{
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;

	std::ofstream writefile(result);

	std::string rawkey = "pGfvSA08PDn6yre0vwqJzyJnsHwmnSHx";
	ByteArray bytekey(rawkey.begin(), rawkey.end());
	Encryption encryption;
	encryption.Start();
	ByteArray Enc = encryption.EncryptText(Content, bytekey);
	std::string encryptedtext(Enc.begin(), Enc.end());

	writefile.clear();
	writefile << encryptedtext;
	writefile.close();
	std::cout << encryptedtext;
}
std::string Database::ReadFileAsString(std::string Dir, std::string FileName)
{

	if (!Database::DoesFileExist(Dir, FileName))
	{
		return "Invalid Dir";
	}
	int test = 0;
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;
	std::ifstream file(result);

	std::ifstream t(result);
	t.seekg(0, std::ios::end);
	size_t size = t.tellg();
	std::string buffer(size, ' ');
	t.seekg(0);
	t.read(&buffer[0], size);
	return buffer;


}
std::string Database::ReadEncryptedFileAsString(std::string Dir, std::string FileName)
{
	if (!Database::DoesFileExist(Dir, FileName))
	{
		return "Invalid Dir";
	}
	int test = 0;
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;
	std::ifstream file(result);
	std::string str;
	std::string End;
	std::list<std::string> lines;
	std::string rawkey = "pGfvSA08PDn6yre0vwqJzyJnsHwmnSHx";
	ByteArray bytekey(rawkey.begin(), rawkey.end());
	Encryption encryption;
	encryption.Start();
	while (std::getline(file, str))
	{
		if (str != "")
			lines.push_back(str);

	}
	// allows us to measure amount of lines so we can add in \ns into the string that might be removed from writing to file
	if (lines.size() > 1)
	{
		for (std::string content : lines)
		{

			// add the string if it isn't the last line with a /n else just add the string
			if (lines.size() < test)
				End += str + "\\n";
			else
				End += str;
			test++;
		}
		ByteArray strtobyte(End.begin(), End.end());
		std::string Enc = encryption.DecryptText(strtobyte, bytekey);
		return Enc;
	}
	else
	{
		ByteArray strtobyte(str.begin(), str.end());
		std::string Enc = encryption.DecryptText(strtobyte, bytekey);
		End = Enc;
	}
	return End;

	return "Null";
}
#pragma endregion FileAsString

#pragma region Read/WriteLines
std::string Database::ReadLine(int LineNum, std::string Dir, std::string FileName)
{
	if (!Database::DoesFileExist(Dir, FileName))
	{
		return "Invalid File";
	}
	int test = 0;
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;
	std::ifstream file(result);
	std::string str;
	while (std::getline(file, str))
	{
		if (test == LineNum)
			return str;
		test++;
	}

	return "Null";
}
std::string Database::ReadEncrypted(int LineNum, std::string Dir, std::string FileName)
{
	if (!Database::DoesFileExist(Dir, FileName))
	{
		return "Invalid File";
	}
	int test = 0;
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;
	std::ifstream file(result);
	std::string str;
	std::string rawkey = "pGfvSA08PDn6yre0vwqJzyJnsHwmnSHx";
	ByteArray bytekey(rawkey.begin(), rawkey.end());
	Encryption encryption;
	encryption.Start();

	while (std::getline(file, str))
	{
		if (test == LineNum)
		{
			ByteArray strtobyte(str.begin(), str.end());
			std::string Enc = encryption.DecryptText(strtobyte, bytekey);
			return Enc;
		}
		test++;
	}

	return "Null";
}
void Database::WriteLine(int LineNum, std::string Dir, std::string FileName, std::string Contents)
{
	if (!Database::DoesFileExist(Dir, FileName))
	{
		return;
	}
	int test = 0;
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;

	std::ifstream file(result);
	std::string str;
	std::list<std::string> lines;
	while (std::getline(file, str))
	{
		// write our contents if its the right line, otherwise write old the original contents
		if (test == LineNum)
			lines.push_back(Contents + "\n");
		else
			lines.push_back(str + "\n");
		test++;
	}
	std::ofstream writefile(result);
	writefile.clear(); // remove old lines as we have them cached. 
	for (std::string content : lines)
	{
		// loop all our lines and write it to the file
		writefile << content;
	}
	writefile.close();

}
void Database::WriteEncrypted(int LineNum, std::string Dir, std::string FileName, std::string Contents)
{
	if (!Database::DoesFileExist(Dir, FileName))
	{
		return;
	}

	int test = 0;
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;

	std::ifstream file(result);
	std::string str;
	std::list<std::string> lines;


	std::string rawkey = "pGfvSA08PDn6yre0vwqJzyJnsHwmnSHx";
	ByteArray bytekey(rawkey.begin(), rawkey.end());
	Encryption encryption;
	encryption.Start();
	ByteArray Enc = encryption.EncryptText(Contents, bytekey);
	std::string encryptedtext(Enc.begin(), Enc.end());

	while (std::getline(file, str))
	{

		// write our contents if its the right line, otherwise write old the original contents
		if (test == LineNum)
			lines.push_back(encryptedtext + "\n");
		else
			lines.push_back(str + "\n");
		test++;
	}
	std::ofstream writefile(result);
	writefile.clear(); // remove old lines as we have them cached. 
	for (std::string content : lines)
	{
		// loop all our lines and write it to the file
		writefile << content;
	}
	writefile.close();

}
#pragma endregion Read/WriteLines

#pragma region LineUtil
void Database::AddFileLines(int NumOfLines, std::string Dir, std::string FileName)
{
	if (!Database::DoesFileExist(Dir, FileName))
	{
		return;
	}

	int test = 0;
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;

	std::ifstream file(result);
	std::string str;
	std::list<std::string> lines;
	std::ofstream writefile(result);
	writefile.clear(); // remove old lines as we have them cached. 
	for (int i = 1; i <= NumOfLines; ++i)
	{
		writefile << "\n";
	}
	writefile.close();

}




void Database::CreateDir(std::string Dir)
{
	if (Database::DoesDirectoryExist(Dir))
	{
		return;
	}
	std::filesystem::path path = Dir;
	std::filesystem::create_directory(path);

}
void Database::CreateFilename(std::string Dir, std::string Filename)
{
	if (Database::DoesFileExist(Dir, Filename))
	{
		return;
	}
	std::string result;
	if (Dir.substr(Dir.length()) == "/") // Handle the format by checking the last char.
		result = Dir + Filename;
	else
		result = Dir + "/" + Filename;

	std::ofstream file(result);
	file.close();

}
bool Database::DoesDirectoryExist(std::string Dir)
{
	std::filesystem::path path = Dir;

	if (!std::filesystem::exists(path))
		return false;
	return true;
}
// kinda obsolete as doesdirectoryexist does the same job. Here for organization
bool Database::DoesFileExist(std::string Dir, std::string FileName)
{
	std::string result;
	if (Dir.substr(Dir.length()) == "/")
		result = Dir + FileName;
	else
		result = Dir + "/" + FileName;

	std::filesystem::path path = result;
	if (!std::filesystem::exists(path))
		return false;
	return true;
}
#pragma endregion LineUtil

#pragma region String Modification
void Database::RemoveSpaces(std::string& input)
{
	std::string::iterator end_pos = std::remove(input.begin(), input.end(), ' ');
	input.erase(end_pos, input.end());

}
void Database::RemoveLines(std::string& input)
{
	std::string::iterator end_pos = std::remove(input.begin(), input.end(), '\n');
	input.erase(end_pos, input.end());

}
#pragma endregion String Modification