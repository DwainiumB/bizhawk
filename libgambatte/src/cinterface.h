#ifndef CINTERFACE_H
#define CINTERFACE_H

// these are all documented on the C# side

extern "C"
{
	__declspec(dllexport) void *gambatte_create();
	__declspec(dllexport) void gambatte_destroy(void *core);

	__declspec(dllexport) int gambatte_load(void *core, const char *romfiledata, unsigned romfilelength, long long now, unsigned flags);

	__declspec(dllexport) long gambatte_runfor(void *core, unsigned long *videobuf, int pitch, short *soundbuf, unsigned *samples);

	__declspec(dllexport) void gambatte_reset(void *core, long long now);

	__declspec(dllexport) void gambatte_setdmgpalettecolor(void *core, unsigned palnum, unsigned colornum, unsigned rgb32);

	__declspec(dllexport) void gambatte_setcgbpalette(void *core, unsigned *lut);

	__declspec(dllexport) void gambatte_setinputgetter(void *core, unsigned (*getinput)(void));

	__declspec(dllexport) void gambatte_setreadcallback(void *core, void (*callback)(unsigned));

	__declspec(dllexport) void gambatte_setwritecallback(void *core, void (*callback)(unsigned));

	__declspec(dllexport) void gambatte_setexeccallback(void *core, void (*callback)(unsigned));

	__declspec(dllexport) void gambatte_settracecallback(void *core, void (*callback)(void *));

	__declspec(dllexport) void gambatte_setscanlinecallback(void *core, void (*callback)(), int sl);

	__declspec(dllexport) void gambatte_setrtccallback(void *core, long long (*callback)());

	__declspec(dllexport) void gambatte_setsavedir(void *core, const char *sdir);

	__declspec(dllexport) int gambatte_iscgb(void *core);

	__declspec(dllexport) int gambatte_isloaded(void *core);

	__declspec(dllexport) void gambatte_savesavedata(void *core, char *dest);
	__declspec(dllexport) void gambatte_loadsavedata(void *core, const char *data);
	__declspec(dllexport) int gambatte_savesavedatalength(void *core);

	//__declspec(dllexport) int gambatte_savestate(void *core, const unsigned long *videobuf, int pitch);

	//__declspec(dllexport) int gambatte_loadstate(void *core);

	__declspec(dllexport) int gambatte_savestate(void *core, const unsigned long *videobuf, int pitch, char **data, unsigned *len);
	__declspec(dllexport) void gambatte_savestate_destroy(char *data);

	__declspec(dllexport) int gambatte_loadstate(void *core, const char *data, unsigned len);

	//__declspec(dllexport) void gambatte_selectstate(void *core, int n);

	//__declspec(dllexport) int gambatte_currentstate(void *core);

	__declspec(dllexport) const char *gambatte_romtitle(void *core);

	__declspec(dllexport) void gambatte_setgamegenie(void *core, const char *codes);

	__declspec(dllexport) void gambatte_setgameshark(void *core, const char *codes);

	__declspec(dllexport) int gambatte_getmemoryarea(void *core, int which, unsigned char **data, int *length);

	__declspec(dllexport) unsigned char gambatte_cpuread(void *core, unsigned short addr);

	__declspec(dllexport) void gambatte_cpuwrite(void *core, unsigned short addr, unsigned char val);

	__declspec(dllexport) int gambatte_linkstatus(void *core, int which);

	__declspec(dllexport) void gambatte_getregs(void *core, int *dest);

}



#endif
