//
// Copyright (c) 2004 K. Wilkins
//
// This software is provided 'as-is', without any express or implied warranty.
// In no event will the authors be held liable for any damages arising from
// the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
//
// 2. Altered source versions must be plainly marked as such, and must not
//    be misrepresented as being the original software.
//
// 3. This notice may not be removed or altered from any source distribution.
//

//////////////////////////////////////////////////////////////////////////////
//                       Handy - An Atari Lynx Emulator                     //
//                          Copyright (c) 1996,1997                         //
//                                 K. Wilkins                               //
//////////////////////////////////////////////////////////////////////////////
// System object class                                                      //
//////////////////////////////////////////////////////////////////////////////
//                                                                          //
// This class provides the glue to bind of of the emulation objects         //
// together via peek/poke handlers and pass thru interfaces to lower        //
// objects, all control of the emulator is done via this class. Update()    //
// does most of the work and each call emulates one CPU instruction and     //
// updates all of the relevant hardware if required. It must be remembered  //
// that if that instruction involves setting SPRGO then, it will cause a    //
// sprite painting operation and then a corresponding update of all of the  //
// hardware which will usually involve recursive calls to Update, see       //
// Mikey SPRGO code for more details.                                       //
//                                                                          //
//    K. Wilkins                                                            //
// August 1997                                                              //
//                                                                          //
//////////////////////////////////////////////////////////////////////////////
// Revision History:                                                        //
// -----------------                                                        //
//                                                                          //
// 01Aug1997 KW Document header added & class documented.                   //
//                                                                          //
//////////////////////////////////////////////////////////////////////////////

#define SYSTEM_CPP

//#include <crtdbg.h>
//#define	TRACE_SYSTEM

#include "system.h"

CSystem::CSystem(const uint8 *game, uint32 gamesize, const uint8 *bios, uint32 biossize, int pagesize0, int pagesize1, bool lowpass)
{
	// load lynxboot.img
	mRom = new CRom(bios, biossize);

	mCart = new CCart(game, gamesize, pagesize0, pagesize1);
	mRam = new CRam();

	mMikie = new CMikie(*this);
	mSusie = new CSusie(*this);

	// Instantiate the memory map handler
	mMemMap = new CMemMap(*this);

	// Now the handlers are set we can instantiate the CPU as is will use handlers on reset
	mCpu = new C65C02(*this);

	mMikie->mikbuf.set_sample_rate(44100, 60);
	mMikie->mikbuf.clock_rate((long int)(16000000 / 4));
	mMikie->mikbuf.bass_freq(60);
	mMikie->miksynth.volume(0.50);
	mMikie->miksynth.treble_eq(lowpass ? -35 : 0);

	// Now init is complete do a reset, this will cause many things to be reset twice
	Reset();
}

CSystem::~CSystem()
{
	delete mCart;
	delete mRom;
	delete mRam;
	delete mCpu;
	delete mMikie;
	delete mSusie;
	delete mMemMap;
}

void CSystem::Reset()
{
	gSystemCycleCount=0;
	gNextTimerEvent=0;
	// gCPUBootAddress=0;
	gSystemIRQ=FALSE;
	gSystemNMI=FALSE;
	gSystemCPUSleep=FALSE;
	gSystemHalt=FALSE;
	gSuzieDoneTime = 0;

	mMemMap->Reset();
	mCart->Reset();
	mRom->Reset();
	mRam->Reset();
	mMikie->Reset();
	mSusie->Reset();
	mCpu->Reset();
}

/*
static int Load(MDFNFILE *fp)
{
	try
	{
		lynxie = new CSystem(fp->data, fp->size);

		switch(lynxie->CartGetRotate())
		{
		case CART_ROTATE_LEFT:
			MDFNGameInfo->rotated = MDFN_ROTATE270;
			break;

		case CART_ROTATE_RIGHT:
			MDFNGameInfo->rotated = MDFN_ROTATE90;
			break;
		}

		memcpy(MDFNGameInfo->MD5, lynxie->mCart->MD5, 16);
		MDFNGameInfo->GameSetMD5Valid = FALSE;

		MDFN_printf(_("ROM:       %dKiB\n"), (lynxie->mCart->InfoROMSize + 1023) / 1024);
		MDFN_printf(_("ROM CRC32: 0x%08x\n"), lynxie->mCart->CRC32());
		MDFN_printf(_("ROM MD5:   0x%s\n"), md5_context::asciistr(MDFNGameInfo->MD5, 0).c_str());

		MDFNGameInfo->fps = (uint32)(59.8 * 65536 * 256);

		if(MDFN_GetSettingB("lynx.lowpass"))
		{
			lynxie->mMikie->miksynth.treble_eq(-35);
		}
		else
		{
			lynxie->mMikie->miksynth.treble_eq(0);
		}

}
*/

void CSystem::Advance(int buttons, uint32 *vbuff, int16 *sbuff, int &sbuffsize)
{
	// this check needs to occur at least once every 250 million cycles or better
	mMikie->CheckWrap();


	SetButtonData(buttons);

	uint32 start = gSystemCycleCount;

	// audio start frame
	mMikie->startTS = start;

	mMikie->mpDisplayCurrent = vbuff;

	// go to next frame end, or no more than 200,000 cycles to avoid exploding the output buffer (was set at 60ms limit)
	while (mMikie->mpDisplayCurrent && gSystemCycleCount - start < 200000)
	// while (gSystemCycleCount - start < 700000) // what's the magic significance?
	{
		Update();
	}

	// total cycles executed is now gSystemCycleCount - start
	mMikie->mikbuf.end_frame((gSystemCycleCount - start) >> 2);
	sbuffsize = mMikie->mikbuf.read_samples(sbuff, sbuffsize) / 2;

}


/*
static MDFNSetting LynxSettings[] =
{
	{ "lynx.rotateinput", MDFNSF_NOFLAGS,	gettext_noop("Virtually rotate D-pad along with screen."), NULL, MDFNST_BOOL, "1" },
	{ "lynx.lowpass", MDFNSF_CAT_SOUND,	gettext_noop("Enable sound output lowpass filter."), NULL, MDFNST_BOOL, "1" },
	{ NULL }
};
*/

/*
static const InputDeviceInputInfoStruct IDII[] =
{
	{ "a", "A (outer)", 8, IDIT_BUTTON_CAN_RAPID, NULL },
	{ "b", "B (inner)", 7, IDIT_BUTTON_CAN_RAPID, NULL },
	{ "option_2", "Option 2 (lower)", 5, IDIT_BUTTON_CAN_RAPID, NULL },
	{ "option_1", "Option 1 (upper)", 4, IDIT_BUTTON_CAN_RAPID, NULL },

	{ "left", "LEFT ←", 	2, IDIT_BUTTON, "right",		{ "up", "right", "down" } },
	{ "right", "RIGHT →", 	3, IDIT_BUTTON, "left", 		{ "down", "left", "up" } },
	{ "up", "UP ↑", 	0, IDIT_BUTTON, "down",		{ "right", "down", "left" } },
	{ "down", "DOWN ↓", 	1, IDIT_BUTTON, "up", 		{ "left", "up", "right" } },
	{ "pause", "PAUSE", 6, IDIT_BUTTON, NULL },
};
*/

/*
static const FileExtensionSpecStruct KnownExtensions[] =
{
	{ ".lnx", gettext_noop("Atari Lynx ROM Image") },
	{ NULL, NULL }
};
*/

/*
MDFNGI EmulatedLynx =
{
	"lynx",
	"Atari Lynx",
	KnownExtensions,
	MODPRIO_INTERNAL_HIGH,
	NULL,
	&InputInfo,
	Load,
	TestMagic,
	NULL,
	NULL,
	CloseGame,
	SetLayerEnableMask,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	false,
	StateAction,
	Emulate,
	SetInput,
	DoSimpleCommand,
	LynxSettings,
	MDFN_MASTERCLOCK_FIXED(16000000),
	0,

	false, // Multires possible?

	160,   // lcm_width
	102,   // lcm_height
	NULL,  // Dummy


	160,	// Nominal width
	102,	// Nominal height

	160,	// Framebuffer width
	102,	// Framebuffer height

	2,     // Number of output sound channels
};
*/
