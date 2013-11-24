/***************************************************************************
 *   Copyright (C) 2007 by Sindre Aamås                                    *
 *   aamas@stud.ntnu.no                                                    *
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License version 2 as     *
 *   published by the Free Software Foundation.                            *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License version 2 for more details.                *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   version 2 along with this program; if not, write to the               *
 *   Free Software Foundation, Inc.,                                       *
 *   59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.             *
 ***************************************************************************/
#include "gambatte.h"
#include "cpu.h"
#include "savestate.h"
#include "statesaver.h"
#include "initstate.h"
#include "state_osd_elements.h"
#include <sstream>
#include <cstring>

static const std::string itos(const int i) {
	std::stringstream ss;
	ss << i;
	return ss.str();
}

static const std::string statePath(const std::string &basePath, const int stateNo) {
	return basePath + "_" + itos(stateNo) + ".gqs";
}

namespace gambatte {
struct GB::Priv {
	CPU cpu;
	int stateNo;
	bool gbaCgbMode;
	
	Priv() : stateNo(1), gbaCgbMode(false) {}
};
	
GB::GB() : p_(new Priv) {}

GB::~GB() {
	//if (p_->cpu.loaded())
	//	p_->cpu.saveSavedata();
	
	delete p_;
}

long GB::runFor(gambatte::uint_least32_t *const videoBuf, const int pitch,
			gambatte::uint_least32_t *const soundBuf, unsigned &samples) {
	if (!p_->cpu.loaded()) {
		samples = 0;
		return -1;
	}
	
	p_->cpu.setVideoBuffer(videoBuf, pitch);
	p_->cpu.setSoundBuffer(soundBuf);
	const long cyclesSinceBlit = p_->cpu.runFor(samples * 2);
	samples = p_->cpu.fillSoundBuffer();
	
	return cyclesSinceBlit < 0 ? cyclesSinceBlit : static_cast<long>(samples) - (cyclesSinceBlit >> 1);
}

void GB::reset(const std::uint32_t now) {
	if (p_->cpu.loaded()) {
		
		int length = p_->cpu.saveSavedataLength();
		char *s;
		if (length > 0)
		{
			s = (char *) std::malloc(length);
			p_->cpu.saveSavedata(s);
		}
		
		SaveState state;
		p_->cpu.setStatePtrs(state);
		setInitState(state, p_->cpu.isCgb(), p_->gbaCgbMode, now);
		p_->cpu.loadState(state);
		if (length > 0)
		{
			p_->cpu.loadSavedata(s);
			std::free(s);
		}
	}
}

void GB::setInputGetter(InputGetter *getInput) {
	p_->cpu.setInputGetter(getInput);
}

void GB::setReadCallback(void (*callback)(unsigned)) {
	p_->cpu.setReadCallback(callback);
}

void GB::setWriteCallback(void (*callback)(unsigned)) {
	p_->cpu.setWriteCallback(callback);
}

void GB::setExecCallback(void (*callback)(unsigned)) {
	p_->cpu.setExecCallback(callback);
}

void GB::setTraceCallback(void (*callback)(void *)) {
	p_->cpu.setTraceCallback(callback);
}

void GB::setScanlineCallback(void (*callback)(), int sl) {
	p_->cpu.setScanlineCallback(callback, sl);
}

void GB::setRTCCallback(std::uint32_t (*callback)()) {
	p_->cpu.setRTCCallback(callback);
}

void GB::setSaveDir(const std::string &sdir) {
	p_->cpu.setSaveDir(sdir);
}

int GB::load(const char *romfiledata, unsigned romfilelength, const std::uint32_t now, const unsigned flags) {
	//if (p_->cpu.loaded())
	//	p_->cpu.saveSavedata();
	
	const int failed = p_->cpu.load(romfiledata, romfilelength, flags & FORCE_DMG, flags & MULTICART_COMPAT);
	
	if (!failed) {
		SaveState state;
		p_->cpu.setStatePtrs(state);
		setInitState(state, p_->cpu.isCgb(), p_->gbaCgbMode = flags & GBA_CGB, now);
		p_->cpu.loadState(state);
		//p_->cpu.loadSavedata();
		
		p_->stateNo = 1;
		p_->cpu.setOsdElement(std::auto_ptr<OsdElement>());
	}
	
	return failed;
}

bool GB::isCgb() const {
	return p_->cpu.isCgb();
}

bool GB::isLoaded() const {
	return p_->cpu.loaded();
}

void GB::saveSavedata(char *dest) {
	if (p_->cpu.loaded())
		p_->cpu.saveSavedata(dest);
}
void GB::loadSavedata(const char *data) {
	if (p_->cpu.loaded())
		p_->cpu.loadSavedata(data);
}
int GB::saveSavedataLength() {
	if (p_->cpu.loaded())
		return p_->cpu.saveSavedataLength();
	else
		return -1;
}

bool GB::getMemoryArea(int which, unsigned char **data, int *length) {
	if (p_->cpu.loaded())
		return p_->cpu.getMemoryArea(which, data, length);
	else
		return false;
}

unsigned char GB::ExternalRead(unsigned short addr) {
	if (p_->cpu.loaded())
		return p_->cpu.ExternalRead(addr);
	else
		return 0;
}

void GB::ExternalWrite(unsigned short addr, unsigned char val) {
	if (p_->cpu.loaded())
		p_->cpu.ExternalWrite(addr, val);
}


void GB::setDmgPaletteColor(unsigned palNum, unsigned colorNum, unsigned rgb32) {
	p_->cpu.setDmgPaletteColor(palNum, colorNum, rgb32);
}

void GB::setCgbPalette(unsigned *lut) {
	p_->cpu.setCgbPalette(lut);
}

bool GB::loadState(std::istream &file) {
	if (p_->cpu.loaded()) {
	//	p_->cpu.saveSavedata();
		
		SaveState state;
		p_->cpu.setStatePtrs(state);
		
		if (StateSaver::loadState(state, file)) {
			p_->cpu.loadState(state);
			return true;
		}
	}

	return false;
}
/*
bool GB::saveState(const gambatte::uint_least32_t *const videoBuf, const int pitch) {
	if (saveState(videoBuf, pitch, statePath(p_->cpu.saveBasePath(), p_->stateNo))) {
		p_->cpu.setOsdElement(newStateSavedOsdElement(p_->stateNo));
		return true;
	}

	return false;
}

bool GB::loadState() {
	if (loadState(statePath(p_->cpu.saveBasePath(), p_->stateNo))) {
		p_->cpu.setOsdElement(newStateLoadedOsdElement(p_->stateNo));
		return true;
	}

	return false;
}
*/
bool GB::saveState(const gambatte::uint_least32_t *const videoBuf, const int pitch, std::ostream &file) {
	if (p_->cpu.loaded()) {
		SaveState state;
		p_->cpu.setStatePtrs(state);
		p_->cpu.saveState(state);
		return StateSaver::saveState(state, videoBuf, pitch, file);
	}

	return false;
}

void GB::selectState(int n) {
	n -= (n / 10) * 10;
	p_->stateNo = n < 0 ? n + 10 : n;
	
	if (p_->cpu.loaded())
		p_->cpu.setOsdElement(newSaveStateOsdElement(statePath(p_->cpu.saveBasePath(), p_->stateNo), p_->stateNo));
}

int GB::currentState() const { return p_->stateNo; }

const std::string GB::romTitle() const {
	if (p_->cpu.loaded()) {
		char title[0x11];
		std::memcpy(title, p_->cpu.romTitle(), 0x10);
		title[(title[0xF] & 0x80) ? 0xF : 0x10] = '\0';
		return std::string(title);
	}
	
	return std::string();
}

void GB::setGameGenie(const std::string &codes) {
	p_->cpu.setGameGenie(codes);
}

void GB::setGameShark(const std::string &codes) {
	p_->cpu.setGameShark(codes);
}

int GB::LinkStatus(int which) {
	return p_->cpu.LinkStatus(which);
}

void GB::GetRegs(int *dest) {
	p_->cpu.GetRegs(dest);
}

}
