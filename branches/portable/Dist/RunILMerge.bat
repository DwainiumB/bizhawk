@echo please copy "C:\Windows\Microsoft.NET\Framework\v3.0\WPF\PresentationUI.dll" to this directory. Dunno why.
ilmerge /v4 /target:WinExe /out:..\output\EmuHawk.merged.exe ..\output\EmuHawk.exe ..\output\BizHawk.Client.Common.dll ..\output\BizHawk.Common.dll ..\output\BizHawk.Emulation.Common.dll ..\output\BizHawk.Emulation.Cores.dll ..\output\BizHawk.Emulation.DiscSystem.dll "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\PresentationFramework.dll" PresentationUI.dll