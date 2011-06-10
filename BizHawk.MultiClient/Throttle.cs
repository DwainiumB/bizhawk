﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

//this throttle is nitsuja's fine-tuned techniques from desmume

namespace BizHawk.MultiClient
{
	class Throttle
	{
		int lastskiprate;
		int framestoskip;
		int framesskipped;
		public bool skipnextframe;

		public bool signal_frameAdvance;
		public bool signal_unthrottle;
		public bool signal_continuousframeAdvancing; //continuousframeAdvancing

		public int cfg_frameskiprate { get { return Global.Config.FrameSkip; } }
		public bool cfg_frameLimit { get { return Global.Config.LimitFramerate; } }
		public bool cfg_autoframeskipenab { get { return Global.Config.AutoMinimizeSkipping; } }

		public void Step(bool allowSleep, int forceFrameSkip)
		{
			int skipRate = (forceFrameSkip < 0) ? cfg_frameskiprate : forceFrameSkip;
			int ffSkipRate = (forceFrameSkip < 0) ? 3 : forceFrameSkip;

			if (lastskiprate != skipRate)
			{
				lastskiprate = skipRate;
				framestoskip = 0; // otherwise switches to lower frameskip rates will lag behind
			}

			if (!skipnextframe || forceFrameSkip == 0 || signal_frameAdvance || (signal_continuousframeAdvancing && !signal_unthrottle))
			{
				framesskipped = 0;

				if (framestoskip > 0)
					skipnextframe = true;
			}
			else
			{
				framestoskip--;

				if (framestoskip < 1)
					skipnextframe = false;
				else
					skipnextframe = true;

				framesskipped++;

				//NDS_SkipNextFrame();
			}

			if (signal_unthrottle)
			{
				if (framesskipped < ffSkipRate)
				{
					skipnextframe = true;
					framestoskip = 1;
				}
				if (framestoskip < 1)
					framestoskip += ffSkipRate;
			}
			else if ((/*autoframeskipenab && frameskiprate ||*/ cfg_frameLimit) && allowSleep)
			{
				SpeedThrottle();
			}

			if (cfg_autoframeskipenab && cfg_frameskiprate!=0)
			{
				if (!signal_frameAdvance && !signal_continuousframeAdvancing)
				{
					AutoFrameSkip_NextFrame();
					if (framestoskip < 1)
						framestoskip += AutoFrameSkip_GetSkipAmount(0, skipRate);
				}
			}
			else
			{
				if (framestoskip < 1)
					framestoskip += skipRate;
			}

			if (signal_frameAdvance && allowSleep)
			{
				//this logic has been replaced by some logic in steprunloop_core.
				//really, it should be moved back here somehow later.

				//frameAdvance = false;
				//emu_halt();
				//SPU_Pause(1);
			}
			//if (execute && emu_paused && !frameAdvance)
			//{
			//    // safety net against running out of control in case this ever happens.
			//    Unpause(); Pause();
			//}
		}

		static ulong GetCurTime()
		{
			if (tmethod == 1)
			{
				ulong tmp;
				QueryPerformanceCounter(out tmp);
				return (ulong)tmp;
			}
			else
			{
				return (ulong)GetTickCount();
			}
		}

		[DllImport("kernel32.dll")]
		static extern uint GetTickCount();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool QueryPerformanceCounter(out ulong lpPerformanceCount);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool QueryPerformanceFrequency(out ulong frequency);

		static int tmethod;
		static ulong afsfreq;
		static ulong tfreq;

		static Throttle()
		{
			tmethod = 0;
			if (QueryPerformanceFrequency(out afsfreq))
				tmethod = 1;
			else
				afsfreq = 1000;
			tfreq = afsfreq << 16;
		}

		public Throttle()
		{
		}

		public void SetCoreFps(double desired_fps)
		{
			core_desiredfps = (ulong)(65536 * desired_fps);
			SetSpeedPercent(pct);
		}

		int pct = -1;
		public void SetSpeedPercent(int percent)
		{
            if (pct == percent) return;
			pct = percent;
			float fraction = percent / 100.0f;
			desiredfps = (ulong)(core_desiredfps * fraction);
			desiredspf = 65536.0f / desiredfps;
			AutoFrameSkip_IgnorePreviousDelay();
		}

		ulong core_desiredfps;
		ulong desiredfps;
		float desiredspf;

		ulong ltime;
		ulong beginticks = 0, endticks = 0, preThrottleEndticks = 0;
		float fSkipFrames = 0;
		float fSkipFramesError = 0;
		int lastSkip = 0;
		float lastError = 0;
		float integral = 0;

		public void AutoFrameSkip_IgnorePreviousDelay()
		{
			beginticks = GetCurTime();

			// this seems to be a stable way of allowing the skip frames to
			// quickly adjust to a faster environment (e.g. after a loadstate)
			// without causing oscillation or a sudden change in skip rate
			fSkipFrames *= 0.5f;
		}

		void AutoFrameSkip_BeforeThrottle()
		{
			preThrottleEndticks = GetCurTime();
		}

		void AutoFrameSkip_NextFrame()
		{
			endticks = GetCurTime();

			// calculate time since last frame
			ulong diffticks = endticks - beginticks;
			float diff = (float)diffticks / afsfreq;

			// calculate time since last frame not including throttle sleep time
			if (preThrottleEndticks == 0) // if we didn't throttle, use the non-throttle time
				preThrottleEndticks = endticks;
			ulong diffticksUnthrottled = preThrottleEndticks - beginticks;
			float diffUnthrottled = (float)diffticksUnthrottled / afsfreq;


			float error = diffUnthrottled - desiredspf;


			// reset way-out-of-range values
			if (diff > 1)
				diff = 1;
			if (error > 1 || error < -1)
				error = 0;
			if (diffUnthrottled > 1)
				diffUnthrottled = desiredspf;

			float derivative = (error - lastError) / diff;
			lastError = error;

			integral = integral + (error * diff);
			integral *= 0.99f; // since our integral isn't reliable, reduce it to 0 over time.

			// "PID controller" constants
			// this stuff is probably being done all wrong, but these seem to work ok
			const float Kp = 40.0f;
			const float Ki = 0.55f;
			const float Kd = 0.04f;

			float errorTerm = error * Kp;
			float derivativeTerm = derivative * Kd;
			float integralTerm = integral * Ki;
			float adjustment = errorTerm + derivativeTerm + integralTerm;

			// apply the output adjustment
			fSkipFrames += adjustment;

			// if we're running too slowly, prevent the throttle from kicking in
			if (adjustment > 0 && fSkipFrames > 0)
				ltime -= tfreq / desiredfps;

			preThrottleEndticks = 0;
			beginticks = GetCurTime();
		}

		int AutoFrameSkip_GetSkipAmount(int min, int max)
		{
			int rv = (int)fSkipFrames;
			fSkipFramesError += fSkipFrames - rv;

			// resolve accumulated fractional error
			// where doing so doesn't push us out of range
			while (fSkipFramesError >= 1.0f && rv <= lastSkip && rv < max)
			{
				fSkipFramesError -= 1.0f;
				rv++;
			}
			while (fSkipFramesError <= -1.0f && rv >= lastSkip && rv > min)
			{
				fSkipFramesError += 1.0f;
				rv--;
			}

			// restrict skip amount to requested range
			if (rv < min)
				rv = min;
			if (rv > max)
				rv = max;

			// limit maximum error accumulation (it's mainly only for fractional components)
			if (fSkipFramesError >= 4.0f)
				fSkipFramesError = 4.0f;
			if (fSkipFramesError <= -4.0f)
				fSkipFramesError = -4.0f;

			// limit ongoing skipframes to requested range + 1 on each side
			if (fSkipFrames < min - 1)
				fSkipFrames = (float)min - 1;
			if (fSkipFrames > max + 1)
				fSkipFrames = (float)max + 1;

			//	printf("%d", rv);

			lastSkip = rv;
			return rv;
		}


		void SpeedThrottle()
		{
			AutoFrameSkip_BeforeThrottle();

		waiter:
			if (signal_unthrottle)
				return;

			ulong ttime = GetCurTime();

			if ((ttime - ltime) < (tfreq / desiredfps))
			{
				ulong sleepy;
				sleepy = (tfreq / desiredfps) - (ttime - ltime);
				sleepy *= 1000;
				if (tfreq >= 65536)
					sleepy /= afsfreq;
				else
					sleepy = 0;
				if (sleepy >= 10)
				{
					Thread.Sleep((int) (sleepy/2));
						// reduce it further beacuse Sleep usually sleeps for more than the amount we tell it to
				}
				else if (sleepy > 0) // spin for <1 millisecond waits
				{
					Thread.Sleep(0);
				}
				//SwitchToThread(); // limit to other threads on the same CPU core for other short waits
				goto waiter;
			}
			if ((ttime - ltime) >= (tfreq * 4 / desiredfps))
				ltime = ttime;
			else
				ltime += tfreq / desiredfps;
		}
	}
}